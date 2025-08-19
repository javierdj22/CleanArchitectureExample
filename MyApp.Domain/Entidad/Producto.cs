namespace MyApp.Domain.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }  // Si Category es otra clase
    }
}
