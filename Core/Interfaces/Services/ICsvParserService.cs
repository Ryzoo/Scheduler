using System.Collections.Generic;
using CsvHelper.Configuration;

namespace Core.Interfaces.Services
{
    public interface ICsvParserService
    {
        public IReadOnlyCollection<TModel> ReadCsvFile<TModel, TMapper>(string filePath)
            where TMapper : ClassMap<TModel>;
    }
}