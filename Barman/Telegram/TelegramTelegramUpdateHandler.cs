using Barman.OpenAI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Barman.Telegram
{
    public class TelegramTelegramUpdateHandler : ITelegramUpdateHandler
    {
        private readonly ILogger<TelegramTelegramUpdateHandler> _logger;
        private readonly IOpenAIService _openAI;

        private const int CALLS_TO_UPDATE_CACHED_NAME = 10;
        private string _cachedBotName = string.Empty;
        private int _cachedUsagesCounter = 0;

        public TelegramTelegramUpdateHandler(ILogger<TelegramTelegramUpdateHandler> logger, IOpenAIService openAI)
        {
            _logger = logger;
            _openAI = openAI;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received update of type: {UpdateType}", update.Type);

            if (update.Type == UpdateType.Message)
            {
                if (update.Message?.Text != null)
                {
                    if (update.Message.Chat.Type != ChatType.Supergroup && update.Message.Chat.Type != ChatType.Group)
                    {
                        return;
                    }

                    string senderName = "Unknown";
                    if (update.Message.From != null)
                    {
                        senderName = update.Message.From.FirstName;
                    }

                    _openAI.RememberUserMessage(senderName, update.Message.Text);

                    string botName = await GetBotName(bot);
                    if (!update.Message.Text.Contains(botName, StringComparison.InvariantCultureIgnoreCase) &&
                        update.Message.ReplyToMessage?.From?.Id != bot.BotId)
                    {
                        return;
                    }

                    string response = await _openAI.AskAsync(senderName, update.Message.Text);
                    //string response = "Прошло";
                    ReplyParameters replyParameters = new ReplyParameters();
                    replyParameters.MessageId = update.Message.Id;
                    await bot.SendMessage(
                        chatId: update.Message.Chat.Id,
                        response, replyParameters: replyParameters,
                        cancellationToken: cancellationToken);
                }
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<string> GetBotName(ITelegramBotClient bot)
        {
            if (string.IsNullOrEmpty(_cachedBotName) || _cachedUsagesCounter == CALLS_TO_UPDATE_CACHED_NAME)
            {
                string name = await bot.GetMyName();
                _cachedBotName = name;
            }

            _cachedUsagesCounter++;
            return _cachedBotName;
        }
    }
}