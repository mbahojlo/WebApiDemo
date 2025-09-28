using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.Models;

public class TopicsRequest
{
    [Required]
    public Dictionary<string, double>? Topics { get; set; }
}
