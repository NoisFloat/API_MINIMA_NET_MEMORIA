using Microsoft.EntityFrameworkCore;
namespace PizzaStore.Modelos
{
    public class Pizza
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
