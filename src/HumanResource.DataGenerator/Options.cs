using CommandLine;

namespace HumanResource.DataGenerator
{
    /// <summary>
    /// Represents command line options for Data Generator
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets/Sets sample size of output file
        /// </summary>
        [Option('s', "sampleSize", Required = false, Default = 25, HelpText = "Number of records to be generated in sample")]
        public int SampleSize { get; set; }

        /// <summary>
        /// Gets/Sets name of output file
        /// </summary>
        [Option('o', "outputFileName", Required = true, HelpText = "Name of output file optionally along with full path")]
        public string OutputFileName { get; set; }
    }
}
