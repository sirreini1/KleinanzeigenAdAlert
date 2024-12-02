using KleinanzeigenAdAlert;
using KleinanzeigenAdAlert.core.Kleinanzeigen;
using KleinanzeigenAdAlert.core.Kleinanzeigen.services;
using KleinanzeigenAdAlert.core.Telegram;
using KleinanzeigenAdAlert.DB;
using KleinanzeigenAdAlert.DB.repositories;
using KleinanzeigenAdAlert.extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

var services = new ServiceCollection()
    .AddDbContext<AppDbContext>()
    .AddSingleton<Application>()
    .AddSingleton<IFlatAdRepository, FlatAdRepository>()
    .AddSingleton<IUserPairChatIdRepository, UserPairChatIdRepository>()
    .AddSingleton<TelegramBotClient>(_ => TelegramBotClientFactory.CreateBotClient())
    .AddSingleton<ITelegramMessageUserService, TelegramMessageUserUserService>()
    .AddSingleton<IAdMonitoringService, AdMonitoringService>()
    .AddSingleton<IAdCleanupService, AdCleanupService>()
    .AddTelegramCommands() // Add all telegram commands
    .AddLogging(configure => { configure.AddConsole(); }) // Add logging
    .BuildServiceProvider();

var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application");

try
{
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    context.Database.Migrate();

    // Resolve all commands to ensure they are instantiated and add themselves as eventlisteners to the telegram client
    services.ResolveAllTelegramCommands();

    var app = services.GetRequiredService<Application>();
    await app.StartApp();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while starting the application");
}