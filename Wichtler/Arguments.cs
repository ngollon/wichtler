using Ookii.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler
{
    internal sealed class Arguments
    {
        [CommandLineArgument(Position = 0, IsRequired = true)]
        public string InputFile { get; set; }

        [CommandLineArgument(Position = 1, DefaultValue = 10.0)]
        public double CalculationTime { get; set; }

        [CommandLineArgument(Position = 2)]
        public string OutputFile { get; set; }

        [CommandLineArgument(Position = 3)]
        public string SmtpServer { get; set; }

        [CommandLineArgument(Position = 4)]
        public string SmtpUsername { get; set; }

        [CommandLineArgument(Position = 5)]
        public string SmtpPassword { get; set; }      
    }
}
