using Microsoft.AspNetCore.Mvc;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetCompareController : ControllerBase
    {
        private readonly PetContext _context;

        public PetCompareController(PetContext context)
        {
            _context = context;
        }

        // GET: api/PetCompare
        [HttpGet]
        public ActionResult ComparePets([FromQuery] int petId1, [FromQuery] int petId2)
        {
            if (petId1 <= 0 || petId2 <= 0)
            {
                return BadRequest(new { Message = "Both pet IDs must be valid and greater than zero." });
            }

            var db = new Db();
            var query = "SELECT * FROM \"Pets\" WHERE \"Id\" = @id1 OR \"Id\" = @id2";
            var parameters = new[]
            {
                new NpgsqlParameter("@id1", petId1),
                new NpgsqlParameter("@id2", petId2)
            };

            DataTable result = db.ExecuteQuery(query, parameters);

            if (result.Rows.Count < 2)
            {
                return NotFound(new { Message = "One or both pets not found." });
            }

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

            var pet1 = pets[0].Id == petId1 ? pets[0] : pets[1];
            var pet2 = pets[0].Id == petId2 ? pets[0] : pets[1];

            var comparisonResult = new
            {
                Pet1 = pet1,
                Pet2 = pet2,
                Comparison = new
                {
                    SpeciesMatch = pet1.Species == pet2.Species,
                    BreedMatch = pet1.Breed == pet2.Breed,
                    GenderMatch = pet1.Gender == pet2.Gender,
                    PriceDifference = Math.Abs(pet1.DesiredPrice - pet2.DesiredPrice),
                    AgeDifference = Math.Abs(pet1.Age - pet2.Age)
                }
            };

            return Ok(comparisonResult);
        }
    }
}
