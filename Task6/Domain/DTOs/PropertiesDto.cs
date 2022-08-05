namespace Task6.Domain.DTOs;

public class PropertiesDto
{
    public string Locale { get; set; } = string.Empty;
    public double ErrorProbability { get; set; }
    public string Seed { get; set; } = string.Empty;
    public int Page { get; set; }
}