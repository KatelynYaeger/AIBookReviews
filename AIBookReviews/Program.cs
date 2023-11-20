using AIBookReviews;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace BookReviewsAI.Models
{
    public class Chat
    {
        static public async Task Main(String[] args)
        {
            var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

            var chatServices = new ServiceCollection()
               .AddOptions()
               .Configure<ChatGPT>(config.GetSection("ChatGPT"))
               .BuildServiceProvider();

            var chatSettings = chatServices.GetService<IOptions<ChatGPT>>();

            var apiKey = chatSettings.Value.ApiKey;

            Console.WriteLine("Hello there! I'm your friendly pocket Librarian. What book would you like me to talk about today?");

            var userTitle = Console.ReadLine();

            var gpt3 = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });

            //Create a chat completion request
            var completionResult = await gpt3.ChatCompletion.CreateCompletion
                                   (new ChatCompletionCreateRequest()
                                   {
                                       Messages = new List<ChatMessage>(new ChatMessage[]
                                        { new ChatMessage("user", $"Give me an unbiased review for the book {userTitle} in under 200 words" +
                                        $"and make it sound like a reader wrote it, not a critic") }),
                                       Model = "gpt-3.5-turbo",
                                       Temperature = 0.5F,
                                       MaxTokens = 3,
                                       N = 3
                                   }) ;

            // Check if the completion result was successful and handle the response
            if (completionResult.Successful)
            {
                foreach (var choice in completionResult.Choices)
                {
                    Console.WriteLine(choice.Message.Content);
                }
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }

            Console.ReadLine();

        }
    }
}