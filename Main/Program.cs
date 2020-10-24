using System;
using System.Reflection;
using Core.Handlers;
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

namespace Core
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
                    services
                        .Configure<DbSettings>(o =>
                        {
                            //[FIX] Something not allow to get configuration section
                            o.ConnectionString = hostContext.Configuration["DbSettings:ConnectionString"];
                            o.Database = hostContext.Configuration["DbSettings:Database"];
                        })
                        .Configure<CsvFilePathSettings>(o =>
                        {
                            o.WelcomeMailFilePath = hostContext.Configuration["CsvFilePathSettings:WelcomeMailFilePath"];
                        })
                        .AddTransient<ICsvParserService, CsvParserService>()
                        .AddTransient<IScheduledMailRepository, ScheduledMailRepository>()
                        .AddTransient<IStateRepository, StateRepository>()
                        .AddMediatR(typeof(ReadMailFileHandler).GetTypeInfo().Assembly)
                        .AddSingleton<DatabaseContext>()
                        .AddHostedService<TaskIntervalRunner<ReadMailFileRequest>>()
                        .AddHostedService<TaskIntervalRunner<QueueScheduledMailRequest>>();
                });
    }
}