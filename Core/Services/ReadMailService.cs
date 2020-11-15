using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.CSV.Mappers;
using Core.CSV.Models;
using Core.DomainModels;
using Core.Interfaces.Services;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services
{
    public class ReadMailService : IReadMailService
    {
        private const int MaxLineToTake = 100;
        private readonly ILogger<ReadMailService> _logger;
        private readonly ICsvParserService _csvParserService;
        private readonly ICacheService _cacheService;
        private readonly IOptions<CsvFilePathSettings> _settings;
        
        public ReadMailService(ILogger<ReadMailService> logger, ICsvParserService csvParserService,
            IOptions<CsvFilePathSettings> settings, ICacheService cacheService)
        {
            _logger = logger;
            _settings = settings;
            _csvParserService = csvParserService;
            _cacheService = cacheService;
        }

        public List<ScheduledMailModel> ReadMail()
        {
            _logger.LogInformation("Starting reading file");
            var previousReadLine = _cacheService.GetReadLineCount();
            
            try
            {
                var welcomeMailDataList = _csvParserService
                    .ReadCsvFile<WelcomeMailDataCsvModel, WelcomeMailDataCsvMapper>(_settings.Value
                        .WelcomeMailFilePath, previousReadLine, MaxLineToTake);

                _logger.LogInformation($"Previous read {previousReadLine} lines.");
                _logger.LogInformation($"Now we try to read {welcomeMailDataList.Count} lines.");

                if (welcomeMailDataList.Count > 0)
                {
                    _logger.LogInformation($"Mails read");
                    _cacheService.SetReadLineCount(previousReadLine + welcomeMailDataList.Count);

                    return welcomeMailDataList
                        .Select(WelcomeMailDataCsvModel.ToDomainModel)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            _logger.LogInformation("Mails sent to scheduler");
            
            return new List<ScheduledMailModel>();
        }
    }
}