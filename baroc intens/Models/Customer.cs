namespace baroc_intens.Models;

public sealed class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ContactName { get; set; }
}
