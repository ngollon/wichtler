using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler.Configuration.CommandLine
{
    internal interface ICommandLineParser
    {
        bool TryParse<T>(string[] args, out T value);
    }
}
