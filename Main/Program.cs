using System;
using System.IO;
using Core.Interfaces.Mails;
using Core.Interfaces.Services;
using Core.Services;
using Core.Settings;
using Core.Tasks;
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
                
                // TODO: Podział Core na bardziej core
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
                    
                    var csvFilePathSettings = new CsvFilePathSettings()
                    {
                        WelcomeMailFilePath =  Path.Combine(Directory.GetCurrentDirectory(), conf["CsvFilePathSettings:Name"])
                    };
                    
                    // TODO: https://docs.microsoft.com/pl-pl/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
                    var emailSettings = new EmailSettings()
                    {
                        From = conf["EmailSettings:From"],
                        Host = conf["EmailSettings:Host"],
                        Port = int.Parse(conf["EmailSettings:Port"]),
                        Username = conf["EmailSettings:Username"],
                        Password = conf["EmailSettings:Password"]
                    };
                    
                    services
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
                        .AddMemoryCache()
                        .AddTransient<ICsvParserService, CsvParserService>()
                        .AddTransient<IMailBuilderService, MailBuilderService>()
                        .AddTransient<ICacheService, CacheService>()
                        .AddTransient<ISendMailService, SendMailService>()
                        .AddTransient<IReadMailService, ReadMailService>()
                        .AddHostedService<ScheduleRunner>()
                        .AddFluentEmail(emailSettings.From)
                        .AddSmtpSender(emailSettings.Host, emailSettings.Port, emailSettings.Username, emailSettings.Password)
                        .AddRazorRenderer();
                });
    }
}