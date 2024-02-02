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

            Console.WriteLine("Hello there! I'm your friendly pocket Librarian.");

            var gpt3 = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });

            var answer = false;

            while (answer == false)
            {
                Console.WriteLine("What book would you like to hear about?");

                var userTitle = Console.ReadLine();

                //Create a chat completion request
                var completionResult = await gpt3.ChatCompletion.CreateCompletion
                                       (new ChatCompletionCreateRequest()
                                       {
                                           Messages = new List<ChatMessage>(new ChatMessage[]
                                            { new ChatMessage("user", $"Give me an unbiased review for the book {userTitle} " +
                                        $"in under 200 words and make it sound like a reader wrote it, not a critic.") }),
                                           Model = "gpt-3.5-turbo-1106",
                                           Temperature = 0.5F,
                                           MaxTokens = 500,
                                           N = 1
                                       });

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

                Console.WriteLine("Would you like to try a different book? Yes or no");

                var userAnswer = Console.ReadLine();

                if (userAnswer.ToLower() == "yes")
                {
                    answer = false;
                }
                else
                {
                    answer = true;
                    Console.WriteLine("Goodbye then!");
                }
            }
        }
    }
}