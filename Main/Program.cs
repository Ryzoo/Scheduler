using System;
using System.Reflection;
using Core.Handlers;
using Core.Interfaces.Mails;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Requests;
using Core.Services;
using Core.Settings;
using Core.Tasks;
using Database;
using Database.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
             * TODO
             * Remove mediator
             * Usunac mediator z konfiguracji
             * metody CreateHostBuilder
             * TaskIntervalRunner<ReadMailFileRequest> TaskIntervalRunner<QueueScheduledMailRequest>
             * TODO
             * Remove db
             * usunac polaczenie z baza z CreateHostBuilder i dalej
             * usunac modele itp
             * TODO
             * Przeniesc plik csv do plikow projektu
             * zmienic sciezke do niego w konfiguracji na skrocona
             *
             * Teraz mamy TaskIntervalRunner<ReadMailFileRequest> i TaskIntervalRunner<QueueScheduledMailRequest>
             * Powinnismy dodac nowy i uzywac tylko tego jednego
             * w nim musi byc zawarta cala logika - to znaczy przeslanie tam odpowiednich zaleznosci i odpalenie ich metod, nie pisanie wszystkiego w tym pliku
             * najpierw musimy zczytac informacje z pliku i potem od razu wyslac do przetworzenia/wyslania
             * musimy znacznik gdzies zapisywac, mozemy uzyc do tego cos w styliu db in memory
             * zawsze czytamy od znacznika + 100 i po przetworzeniu zapisujemy nowa pozycje znacznika
             * TODO
             * zmiast robic nowe  AddHostedService<TaskIntervalRunner<QueueScheduledMailRequest>>()
             * mozesz sprobowac nie uzywac tego ale hangfire
             */
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/scheduleAppLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var conf = hostContext.Configuration;

                    var dbSettings = new DbSettings()
                    {
                        ConnectionString = conf["DbSettings:ConnectionString"],
                        Database = conf["DbSettings:Database"]
                    };
                    var csvFilePathSettings = new CsvFilePathSettings()
                    {
                        WelcomeMailFilePath = conf["CsvFilePathSettings:WelcomeMailFilePath"]
                    };
                    var emailSettings = new EmailSettings()
                    {
                        From = conf["EmailSettings:From"],
                        Host = conf["EmailSettings:Host"],
                        Port = int.Parse(conf["EmailSettings:Port"]),
                        Username = conf["EmailSettings:Username"],
                        Password = conf["EmailSettings:Password"]
                    };
                    
                    services
                        .Configure<DbSettings>(o =>
                        {
                            o.Database = dbSettings.Database;
                            o.ConnectionString = dbSettings.ConnectionString;
                        })
                        .Configure<CsvFilePathSettings>(o =>
                        {
                            o.WelcomeMailFilePath = csvFilePathSettings.WelcomeMailFilePath;
                        })
                        .Configure<EmailSettings>(o =>
                        {
                            o.From = emailSettings.From;
                            o.Host = emailSettings.Host;
                            o.Port = emailSettings.Port;
                            o.Username = emailSettings.Username;
                            o.Password = emailSettings.Password;
                        })
                        .AddTransient<ICsvParserService, CsvParserService>()
                        .AddTransient<IScheduledMailRepository, ScheduledMailRepository>()
                        .AddTransient<IStateRepository, StateRepository>()
                        .AddTransient<IMailBuilderService, MailBuilderService>()
                        .AddMediatR(typeof(ReadMailFileHandler).GetTypeInfo().Assembly)
                        .AddSingleton<DatabaseContext>()
                        .AddHostedService<TaskIntervalRunner<ReadMailFileRequest>>()
                        .AddHostedService<TaskIntervalRunner<QueueScheduledMailRequest>>()
                        .AddFluentEmail(emailSettings.From)
                        .AddSmtpSender(emailSettings.Host, emailSettings.Port, emailSettings.Username, emailSettings.Password)
                        .AddRazorRenderer();
                });
    }
}