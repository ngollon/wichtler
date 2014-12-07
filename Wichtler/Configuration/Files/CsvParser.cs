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
                using (var csvReader = new CsvHelper.CsvReader(fileReader, new CsvHelper.Configuration.CsvConfiguration() { Encoding = Encoding.UTF8 }))
                {
                    csvReader.Configuration.RegisterClassMap<PersonCsvMap>();
                    value = csvReader.GetRecords<T>().ToList();
                    return true;
                }
            }
            catch (IOException)
            {
                value = null;
                return false;
            }
            catch (CsvHelper.CsvHelperException ex)
            {
                value = null;
                return false;
            }
        }
    }
}
