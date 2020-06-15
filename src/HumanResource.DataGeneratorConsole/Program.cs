using Bogus;
using CommandLine;
using HumanResource.DataElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace HumanResource.DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    File.WriteAllText(
                        o.OutputFileName,
                        JsonConvert.SerializeObject(
                            EmployeeDataGenerator.GenerateData(o.SampleSize),
                            Formatting.Indented,
                            new JsonSerializerSettings()
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                });
        }

    }
}
