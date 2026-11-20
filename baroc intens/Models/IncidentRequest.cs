using System;

namespace baroc_intens.Models;

public sealed class IncidentRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int CreatedByUserId { get; set; }
    public AppUser? CreatedByUser { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
