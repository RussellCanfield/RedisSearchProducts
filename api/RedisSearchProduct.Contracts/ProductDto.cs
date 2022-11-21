using System;
namespace RedisSearchProduct.Contracts
{
	public record ProductDto
	{
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public ColorDto Color { get; set; }
        public SizeDto Size { get; set; }
    }

    public enum ColorDto
    {
        Black,
        Grey,
        Blue,
        Red,
        White
    }

    public enum SizeDto
    {
        Small,
        Medium,
        Large,
        XL,
        XXL
    }
}

