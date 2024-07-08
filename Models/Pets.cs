namespace WebApplication3.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Species { get; set; } 
        public string Breed { get; set; } 
        public string Gender { get; set; }
        public decimal DesiredPrice { get; set; } 
        public int Age { get; set; }
        public string Description { get; set; } 
        public string ImageUrl { get; set; } 
    }
}