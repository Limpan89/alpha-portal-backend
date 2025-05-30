﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

[Index(nameof(ClientName), IsUnique = true)]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Image { get; set; }

    [Column(TypeName = "Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    public virtual ClientBillingEntity? Billing { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}
