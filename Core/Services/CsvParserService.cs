using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Interfaces.Services;
using CsvHelper;
using CsvHelper.Configuration;

namespace Core.Services
{
    public class CsvParserService : ICsvParserService
    {
        public IReadOnlyCollection<TModel> ReadCsvFile<TModel, TMapper>(string filePath, int startLine, int toTakeCount)
        where TMapper : ClassMap<TModel>
        {
            try
            {
                using var reader = new StreamReader(filePath, Encoding.Default);
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture);
                csv.Configuration.RegisterClassMap<TMapper>();
                
                return csv.GetRecords<TModel>()
                    .Take(toTakeCount)
                    .ToList();
                
                // return csv.GetRecords<TModel>()
                //     .Skip(startLine)
                //     .Take(toTakeCount)
                //     .ToList();
            }
            catch (UnauthorizedAccessException e)
            {
                throw new Exception(e.Message);
            }
            catch (FieldValidationException e)
            {
                throw new Exception(e.Message);
            }
            catch (CsvHelperException e)
            {
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}