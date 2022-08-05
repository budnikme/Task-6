namespace Task6.Domain.DTOs;

public class PersonDto
{
    public int Id { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}