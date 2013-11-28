using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler.Configuration.CommandLine
{
    internal sealed class CommandLineParser : ICommandLineParser
    {
        public bool TryParse<T>(string[] args, out T value)
        {
            value = default(T);

            var parser = new Ookii.CommandLine.CommandLineParser(typeof(T));
            try
            {
                value = (T)parser.Parse(args);
                return true;
            }
            catch (Ookii.CommandLine.CommandLineArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GetSyntax<T>()
        {
            var parser = new Ookii.CommandLine.CommandLineParser(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                parser.WriteUsage(stringWriter, 0);
                return stringWriter.ToString();
            }
        }
    }
}
