using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.CSV.Mappers;
using Core.CSV.Models;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Requests;
using Core.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Handlers
{
    public class ReadMailFileHandler : AsyncRequestHandler<ReadMailFileRequest>
    {
        private const int MaxLineToTake = 100;
        private readonly ILogger<ReadMailFileHandler> _logger;
        private readonly ICsvParserService _csvParserService;
        private readonly IOptions<CsvFilePathSettings> _settings;
        private readonly IStateRepository _stateRepository;
        private readonly IScheduledMailRepository _mailRepository;

        public ReadMailFileHandler(ILogger<ReadMailFileHandler> logger, ICsvParserService csvParserService,
            IOptions<CsvFilePathSettings> settings, IStateRepository stateRepository,
            IScheduledMailRepository mailRepository)
        {
            _logger = logger;
            _settings = settings;
            _csvParserService = csvParserService;
            _stateRepository = stateRepository;
            _mailRepository = mailRepository;
        }

        protected override async Task Handle(ReadMailFileRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handle ReadMailFileHandler");
            try
            {
                var previousReadLine = await _stateRepository.GetState<int?>(StateType.ReadLineCount);
                var startReadLine = previousReadLine ?? 0;
                var welcomeMailDataList = _csvParserService
                    .ReadCsvFile<WelcomeMailDataCsvModel, WelcomeMailDataCsvMapper>(_settings.Value
                        .WelcomeMailFilePath, startReadLine, MaxLineToTake);

                _logger.LogInformation($"Previous read {startReadLine} lines.");
                _logger.LogInformation($"Now we try read {welcomeMailDataList.Count} lines.");

                if (welcomeMailDataList.Count > 0)
                {
                    await _mailRepository
                        .AddMany(welcomeMailDataList
                            .Select(WelcomeMailDataCsvModel.ToDomainModel)
                            .ToList()
                        );

                    _logger.LogInformation($"Data read and wrote to database.");

                    await _stateRepository.SetState<int?>(StateType.ReadLineCount, welcomeMailDataList.Count.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            _logger.LogInformation("ReadMailFileHandler handled");
        }
    }
}