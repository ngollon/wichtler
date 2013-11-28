using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler.Configuration.Files
{
    internal interface ICsvParser
    {
        bool TryParse<T>(string file, out IList<T> value);
    }
}
