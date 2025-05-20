using Barman;
using Telegram.Bot;
using System.Text;
using Barman.OpenAI;
using Barman.Telegram;
using OpenAI;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
var telegramToken = builder.Configuration["BotSettings:BotToken"];
Console.WriteLine($"Telegram token: {telegramToken}");
if (string.IsNullOrEmpty(telegramToken))
{
    Console.WriteLine("Bot token not found in secrets. Press any key to exit...");
    Console.ReadKey();
    return;
}

var openAIKey = builder.Configuration["BotSettings:OpenAIKey"];
Console.WriteLine($"OpenAI API key : {openAIKey}");
if (string.IsNullOrEmpty(openAIKey))
{
    Console.WriteLine("OpenAI api key not found in secrets. Press any key to exit...");
    Console.ReadKey();
    return;
}

builder.Services.AddSingleton(new OpenAIClient(openAIKey));
builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(telegramToken));
builder.Services.AddSingleton<ITelegramUpdateHandler, TelegramTelegramUpdateHandler>();
builder.Services.AddHostedService<TelegramBotService>();

await builder.Build().RunAsync();