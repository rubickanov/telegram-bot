using Barman.Utils;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace Barman.OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ILogger<OpenAIService> _logger;
        private readonly OpenAIClient _client;
        private readonly IOptions<OpenAISettings> _config;

        private readonly FixedHeadQueue<ChatMessage> _chatHistory;

        public OpenAIService(
            ILogger<OpenAIService> logger,
            OpenAIClient client,
            IOptions<OpenAISettings> config)
        {
            _logger = logger;
            _client = client;
            _config = config;

            _chatHistory = new FixedHeadQueue<ChatMessage>(config.Value.SystemPrompt, 50);
        }

        public async Task<string> AskAsync(string senderName, string question)
        {
            _logger.LogDebug("Sending request to OpenAI...");
            RememberUserMessage(senderName, question);
            var res = await _client.GetChatClient(_config.Value.Model).CompleteChatAsync(_chatHistory);

            string response = res.Value.Content[0].Text;
            _logger.LogDebug($"ChatCompletion response is {res.GetRawResponse().Content}");
            _logger.LogInformation($"ChatCompletion response is {response}");
            RememberAssistantMessage(response);
            return response;
        }

        public void RememberUserMessage(string senderName, string message)
        {
            UserChatMessage userChatMessage = new UserChatMessage(message);
            userChatMessage.ParticipantName = senderName;
            _chatHistory.Enqueue(userChatMessage);
        }

        public void RememberAssistantMessage(string message)
        {
            _chatHistory.Enqueue(new AssistantChatMessage(message));
        }
    }
}