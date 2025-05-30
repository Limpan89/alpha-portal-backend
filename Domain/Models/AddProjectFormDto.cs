﻿using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class AddProjectFormDto
{
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public IFormFile? NewImage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string ClientId { get; set; } = null!;
    public string UserId { get; set; } = null!;
}
