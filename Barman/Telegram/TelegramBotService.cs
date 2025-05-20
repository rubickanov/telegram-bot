using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Barman.Telegram
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramBotService> _logger;
        private readonly ITelegramUpdateHandler _telegramUpdateHandler;

        public TelegramBotService(
            ITelegramBotClient botClient,
            ILogger<TelegramBotService> logger,
            ITelegramUpdateHandler telegramUpdateHandler)
        {
            _botClient = botClient;
            _logger = logger;
            _telegramUpdateHandler = telegramUpdateHandler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bot starting...");

            _botClient.StartReceiving(
                _telegramUpdateHandler.HandleUpdateAsync,
                _telegramUpdateHandler.HandleErrorAsync,
                new ReceiverOptions(),
                cancellationToken: stoppingToken
            );

            return Task.CompletedTask;
        }
    }
}