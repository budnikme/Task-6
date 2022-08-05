using Task6.Domain.DTOs;

namespace Task6.Domain.Interfaces;

public interface IGeneratorService
{
    List<PersonDto> Generate(PropertiesDto properties);
    byte[] DownloadCsv(PropertiesDto properties);
}