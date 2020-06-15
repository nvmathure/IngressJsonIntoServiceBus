using Bogus;
using HumanResource.DataElements;
using System;
using System.Collections.Generic;

namespace HumanResource.DataGenerator
{
    public class EmployeeDataGenerator
    {
        public static List<Employee> GenerateData(int sampleSize)
        {
            var employeeFaker =
                new Faker<Employee>()
                    .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                    .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(ConvertToBogus(u.Gender)))
                    .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(ConvertToBogus(u.Gender)))
                    .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                    .RuleFor(u => u.EmployeeId, f => f.IndexVariable++)
                    .RuleFor(u => u.DateOfBirth, f => f.Date.Past(65, DateTime.Today.AddYears(-25)));

            var employees = employeeFaker.Generate(sampleSize);

            return employees;
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
