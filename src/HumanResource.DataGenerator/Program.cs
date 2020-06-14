using Bogus;
using CommandLine;
using HumanResource.DataElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace HumanResource.DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var employeeFaker =
                        new Faker<Employee>()
                            .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                            .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(ConvertToBogus(u.Gender)))
                            .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(ConvertToBogus(u.Gender)))
                            .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                            .RuleFor(u => u.EmployeeId, f => f.IndexVariable++)
                            .RuleFor(u => u.DateOfBirth, f => f.Date.Past(65, DateTime.Today.AddYears(-25)));

                    var employees = employeeFaker.Generate(o.SampleSize);

                    File.WriteAllText(
                        o.OutputFileName,
                        JsonConvert.SerializeObject(
                            employees,
                            Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));

                });
        }

        private static Bogus.DataSets.Name.Gender ConvertToBogus(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return Bogus.DataSets.Name.Gender.Male;
                case Gender.Female:
                    return Bogus.DataSets.Name.Gender.Female;
                default:
                    throw new NotSupportedException($"Value {gender.ToString()} is not supported");
            }
        }

    }
}
