namespace DAL.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
    }
}
