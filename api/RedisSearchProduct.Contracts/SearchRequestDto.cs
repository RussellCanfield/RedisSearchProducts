namespace RedisSearchProduct.Contracts;

public record SearchRequestDto
{
    public SearchRequestFilterDto? Filters { get; set; }
    public SearchRequestRangeDto? Range { get; set; }
    public SearchRequestSortDirectionDto Sort { get; set; }
}

public record SearchRequestFilterDto
{
    public string? Name { get; set; }
    public string[]? Values { get; set; }
}

public record SearchRequestRangeDto
{
    public string? Name { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}

public enum SearchRequestSortDirectionDto
{
    Ascending,
    Descending
}