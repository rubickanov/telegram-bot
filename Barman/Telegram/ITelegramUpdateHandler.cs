using Telegram.Bot;
using Telegram.Bot.Types;

namespace Barman.Telegram;

public interface ITelegramUpdateHandler
{
    Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken);
    Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken);
}