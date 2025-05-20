namespace Barman.OpenAI
{
    public interface IOpenAIService
    {
        public Task<string> AskAsync(string senderName, string question);
        public void RememberUserMessage(string senderName, string message);
        public void RememberAssistantMessage(string message);
    }
}

