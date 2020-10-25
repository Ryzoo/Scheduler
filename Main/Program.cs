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

                    //Todo [FIX] Something not allow to get configuration section
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