using System.Text.Json.Serialization;

namespace RedisSearchProduct.Data.Products.Models;

public class Product
{
    public Product(string name, decimal price, Color color, Size size)
    {
        Name = name;
        Price = price;
        Color = color;
        Size = size;
    }

    [JsonConstructor]
    public Product() { }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public Color Color { get; set; }
    public Size Size { get; set; }
}

public enum Color
{
    Black,
    Grey,
    Blue,
    Red,
    White
}

public enum Size
{
    Small,
    Medium,
    Large,
    XL,
    XXL
}
