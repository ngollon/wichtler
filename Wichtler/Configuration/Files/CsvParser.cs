using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler.Configuration.Files
{
    internal sealed class CsvParser : ICsvParser
    {
        public bool TryParse<T>(string file, out IList<T> value)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);

            try
            {
                using (var fileReader = new StreamReader(file))
                using (var csvParser = new CsvHelper.CsvParser(fileReader))
                using (var csvReader = new CsvHelper.CsvReader(csvParser))
                {
                    value = csvReader.GetRecords<T>().ToList();
                    return true;
                }
            }
            catch (IOException)
            {
                value = null;
                return false;
            }
            catch (CsvHelper.CsvHelperException)
            {
                value = null;
                return false;
            }
        }
    }
}
