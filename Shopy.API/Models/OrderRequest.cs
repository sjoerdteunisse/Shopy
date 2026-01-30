using System.ComponentModel.DataAnnotations;
using Shopy.OrderService.Domain;

namespace Shopy.API.Models;

public record OrderRequest()
{
    [Required(ErrorMessage = "CustomerId is required")]
    [MaxLength(50, ErrorMessage = "CustomerId cannot exceed 50 characters")]
    public string CustomerId { get; init; } = string.Empty;

    [Required(ErrorMessage = "CustomerEmail is required")]
    [EmailAddress(ErrorMessage = "CustomerEmail must be a valid email")]
    public string CustomerEmail { get; init; } = string.Empty;

    [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
    public decimal Amount { get; init; }
    
    public string Status  => nameof(OrderStatus.Created); 
}