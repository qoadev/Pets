using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly PetContext _context;

        public PetController(PetContext context)
        {
            _context = context;
        }

        // GET: api/Pet
        [HttpGet]
        //[Authorize]
        public ActionResult<IEnumerable<Pet>> GetPets()
        {
            var db = new Db();
            var query = "SELECT * FROM \"Pets\"";
            DataTable result = db.ExecuteQuery(query);

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

        // DELETE: api/Pet/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            try
            {
                var db = new Db();
                var query = "DELETE FROM \"Pets\" WHERE \"Id\" = @id;";
                var rowsAffected = db.ExecuteNonQuery(query, new NpgsqlParameter("@id", id));

                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "Pet deleted successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Pet not found." });
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки (если у вас есть настроенное логирование)
                return StatusCode(500, new { Message = "An error occurred while deleting the pet." });
            }
        }

        // POST: api/Pet
        [HttpPost]
        public async Task<IActionResult> AddPet([FromBody] Pet pet)
        {
            try
            {
                var db = new Db();
                var query = "INSERT INTO \"Pets\" (\"Species\", \"Breed\", \"Gender\", \"DesiredPrice\", \"Age\", \"Description\", \"ImageUrl\") VALUES (@species, @breed, @gender, @desiredPrice, @age, @description, @imageUrl);";
                var parameters = new[]
                {
                    new NpgsqlParameter("@species", pet.Species),
                    new NpgsqlParameter("@breed", pet.Breed),
                    new NpgsqlParameter("@gender", pet.Gender),
                    new NpgsqlParameter("@desiredPrice", pet.DesiredPrice),
                    new NpgsqlParameter("@age", pet.Age),
                    new NpgsqlParameter("@description", pet.Description),
                    new NpgsqlParameter("@imageUrl", pet.ImageUrl)
                };
                var rowsAffected = db.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "Pet added successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to add pet." });
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки (если у вас есть настроенное логирование)
                return StatusCode(500, new { Message = "An error occurred while adding the pet." });
            }
        }

        // PUT: api/Pet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, [FromBody] Pet pet)
        {
            try
            {
                var db = new Db();
                var query = "UPDATE \"Pets\" SET \"Species\" = @species, \"Breed\" = @breed, \"Gender\" = @gender, \"DesiredPrice\" = @desiredPrice, \"Age\" = @age, \"Description\" = @description, \"ImageUrl\" = @imageUrl WHERE \"Id\" = @id;";
                var parameters = new[]
                {
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@species", pet.Species),
                    new NpgsqlParameter("@breed", pet.Breed),
                    new NpgsqlParameter("@gender", pet.Gender),
                    new NpgsqlParameter("@desiredPrice", pet.DesiredPrice),
                    new NpgsqlParameter("@age", pet.Age),
                    new NpgsqlParameter("@description", pet.Description),
                    new NpgsqlParameter("@imageUrl", pet.ImageUrl)
                };
                var rowsAffected = db.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "Pet updated successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Pet not found." });
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки (если у вас есть настроенное логирование)
                return StatusCode(500, new { Message = "An error occurred while updating the pet." });
            }
        }
    }
}
