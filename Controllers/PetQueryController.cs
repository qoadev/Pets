using Microsoft.AspNetCore.Mvc;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetQueryController : ControllerBase
    {
        private readonly PetContext _context;

        public PetQueryController(PetContext context)
        {
            _context = context;
        }

        // GET: api/PetQuery
        [HttpGet]
        //[Authorize]
        public ActionResult<IEnumerable<Pet>> GetPets([FromQuery] string species = null, [FromQuery] string breed = null, [FromQuery] string gender = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
        {
            if (string.IsNullOrEmpty(species) && string.IsNullOrEmpty(breed) && string.IsNullOrEmpty(gender) && !minPrice.HasValue && !maxPrice.HasValue)
            {
                return BadRequest(new { Message = "At least one search criterion must be provided." });
            }

            var db = new Db();
            var query = "SELECT * FROM \"Pets\" WHERE 1=1";
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrEmpty(species))
            {
                query += " AND \"Species\" = @species";
                parameters.Add(new NpgsqlParameter("@species", species));
            }

            if (!string.IsNullOrEmpty(breed))
            {
                query += " AND \"Breed\" = @breed";
                parameters.Add(new NpgsqlParameter("@breed", breed));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query += " AND \"Gender\" = @gender";
                parameters.Add(new NpgsqlParameter("@gender", gender));
            }

            if (minPrice.HasValue)
            {
                query += " AND \"DesiredPrice\" >= @minPrice";
                parameters.Add(new NpgsqlParameter("@minPrice", minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query += " AND \"DesiredPrice\" <= @maxPrice";
                parameters.Add(new NpgsqlParameter("@maxPrice", maxPrice.Value));
            }

            DataTable result = db.ExecuteQuery(query, parameters.ToArray());

            // Преобразование DataTable в список объектов Pet
            var pets = new List<Pet>();
            foreach (DataRow row in result.Rows)
            {
                var pet = new Pet
                {
                    Id = (int)row["Id"],
                    Species = row["Species"].ToString(),
                    Breed = row["Breed"].ToString(),
                    Gender = row["Gender"].ToString(),
                    DesiredPrice = (decimal)row["DesiredPrice"],
                    Age = (int)row["Age"],
                    Description = row["Description"].ToString(),
                    ImageUrl = row["ImageUrl"].ToString()
                };
                pets.Add(pet);
            }

            return Ok(pets);
        }
    }
}
