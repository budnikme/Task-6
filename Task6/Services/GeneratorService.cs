using System.Text;
using Bogus;
using Bogus.DataSets;
using ServiceStack.Text;
using Task6.Domain.DTOs;
using Task6.Domain.Interfaces;

namespace Task6.Services;

public class GeneratorService : IGeneratorService
{
    private PropertiesDto Properties { get; set; } = null!;
    
    public List<PersonDto> Generate(PropertiesDto properties)
    {
        Properties = properties;
        SetSeed(Properties.Seed, Properties.Page);
        return GenerateFakeData();
    }
    private void SetSeed(string seed, int page)
    {
        Randomizer.Seed = new Random($"{seed}{page}".GetHashCode());
    }
    private List<PersonDto> GenerateFakeData()
    {
        var person = GetFakerData();
        if (Properties.ErrorProbability > 0)
        {
            GenerateErrors(Properties.ErrorProbability, person);
        }
        return person;
    }
    private List<PersonDto> GetFakerData()
    {
        int id = (Properties.Page == 1) ? 0 : Properties.Page * 10;
        return new Faker<PersonDto>(Properties.Locale)
            .StrictMode(true)
            .RuleFor(p => p.Id, f => id++)
            .RuleFor(p => p.Uuid, f => f.Random.Guid().ToString())
            .RuleFor(p => p.FullName, GenerateRegionalFullName)
            .RuleFor(p => p.Address,
                f => f.Address.StreetName() + " " + f.Random.Number(1, 500) + HouseOrApartmentGenerator() +
                     ", " + f.Address.City() + " " + f.Address.ZipCode())
            .RuleFor(p => p.Phone, f => FixErrorInPhoneNumber(f.Phone.PhoneNumber()))
            .Generate(Properties.Page == 1 ? 20 : 10);
    }
    private string FixErrorInPhoneNumber(string phone)
    {
        return phone.Contains('x') ? phone.Substring(0, phone.IndexOf('x')) : phone;
    }
    private string GenerateRegionalFullName(Faker f)
    {
        var gender = f.PickRandom<Name.Gender>();
        return Properties.Locale switch
        {
            "en_US" => f.Name.FirstName(gender) + " " + f.Lorem.Letter().ToUpper() + ". " + f.Name.LastName(),
            "pl" => f.Name.FirstName(gender) + " " + f.Name.FirstName(gender) + " " + f.Name.LastName(gender),
            "nb_NO" => f.Name.FullName(),
            _ => f.Name.FullName()
        };
    }
    private string HouseOrApartmentGenerator()
    {
        int random = Randomizer.Seed.Next(0, 2);
        return random == 0 ? $"{SetApartmentLetterByRegion()}{Randomizer.Seed.Next(1,1000)}" : " ";
    }
    private string SetApartmentLetterByRegion()
    {
        return Properties.Locale switch
        {
            "en_US" => " apt. ",
            "pl" => " m. ",
            _ => "-"
        };
    }
    private void GenerateErrors(double probability, List<PersonDto> person)
    {
        foreach (var p in person)
        {
            int errorsNumber = GetErrorsNumber(probability);
            ChooseFieldForError(p, errorsNumber);
        }
    }
    private void ChooseFieldForError(PersonDto person, int errorsNumber)
    {
        for (int i = 0; i < errorsNumber; i++)
        {
            var fieldToChange = Randomizer.Seed.Next(4);
            switch (fieldToChange)
            {
                case 0:
                    person.FullName = GenerateError(person.FullName);
                    break;
                case 1:
                    person.Address = GenerateError(person.Address);
                    break;
                case 2:
                    person.Phone = GenerateError(person.Phone);
                    break;
                case 3:
                    person.Uuid = GenerateError(person.Uuid);
                    break;
            }
        }
    }
    private string GenerateError(string value)
    {
        int error = Randomizer.Seed.Next(3);
        return error switch
        {
            0 => DeleteRandomCharacters(value),
            1 => AddRandomCharacter(value),
            2 => SwapRandomCharacters(value),
            _ => value
        };
    }
    private int GetErrorsNumber(double probability)
    {
        int frac = (int) ((probability % 1) * 100);
        int errorsNumber = (int) probability;
        if (Randomizer.Seed.Next(100) < frac)
        {
            errorsNumber++;
        }
        return errorsNumber;
    }
    private string DeleteRandomCharacters(string str)
    {
        char[] chars = str.ToCharArray();
        int index = Randomizer.Seed.Next(0, chars.Length);
        chars[index] = ' ';
        return new string(chars);
    }
    private string AddRandomCharacter(string str)
    {
        var chars = str.ToCharArray();
        var index = Randomizer.Seed.Next(0, chars.Length);
        var lorem = new Lorem(locale: Properties.Locale);
        var c = char.Parse(lorem.Letter());
        chars[index] = c;
        return new string(chars);
    }
    private string SwapRandomCharacters(string str)
    {
        var chars = str.ToCharArray();
        var index1 = Randomizer.Seed.Next(0, chars.Length);
        var index2 = Randomizer.Seed.Next(0, chars.Length);
        (chars[index1], chars[index2]) = (chars[index2], chars[index1]);
        return new string(chars);
    }
    private List<PersonDto> GenerateMultiplePages(int pageCount)
    {
        var person = new List<PersonDto>();
        Properties.Page = 1;
        for (var i = 0; i < pageCount; i++)
        {
            SetSeed(Properties.Seed, Properties.Page);
            person.AddRange(Generate(Properties));
            Properties.Page++;
        }
        return person;
    }
    public byte[] DownloadCsv(PropertiesDto properties)
    {
        Properties = properties;
        var person = GenerateMultiplePages(Properties.Page);
        var csv = CsvSerializer.SerializeToCsv(person);
        return new UTF8Encoding().GetBytes(csv);
    }
}