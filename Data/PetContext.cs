using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using System.Text.Json.Serialization;

namespace WebApplication3.Data
{
    //[JsonSerializable(typeof(Pet))]
    public class PetContext : DbContext
    {
        public PetContext(DbContextOptions<PetContext> options) : base(options) { }
    
        public DbSet<Pet> Pets { get; set; }
        public DbSet<User> User { get; set; }
    }
    
    
}