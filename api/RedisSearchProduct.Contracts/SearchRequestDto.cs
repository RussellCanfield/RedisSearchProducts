namespace RedisSearchProduct.Contracts;

public record SearchRequestDto
{
    public string? Text { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public SearchRequestFilterDto[]? Filters { get; set; }
    public SearchRequestRangeDto? Range { get; set; }
    public SearchRequestSortDto? Sort { get; set; }
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

public record SearchRequestSortDto
{
    public string? Name { get; set; }
    public SearchRequestSortDirectionDto Direction { get; set; }
}

public enum SearchRequestSortDirectionDto
{
    Ascending,
    Descending
}