#nullable disable
namespace Barman.OpenAI
{
    public class OpenAISettings
    {
        public string Model { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
        public string SystemPrompt { get; set; }
    }
}