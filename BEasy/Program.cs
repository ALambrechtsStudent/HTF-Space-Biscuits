using Involved.HTF.Common;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BEasy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HackTheFutureClient();
            bool loginSuccessful = await PerformLogin(client);

            if (loginSuccessful)
            {
                var alphabet = await FetchAlphabetFromApi(client);
                var encryptedMessage = await FetchEncryptedMessageFromApi(client);

                if (encryptedMessage != null && alphabet != null)
                {
                    var decryptedMessage = DecryptMessage(encryptedMessage, alphabet);
                    var result = await PostResult(client, decryptedMessage);
                    string solutionString = await result.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Failed to fetch the necessary data.");
                }
            }
            else
            {
                Console.WriteLine("Exiting application. Please check your credentials.");
            }
        }

        private static async Task<bool> PerformLogin(HackTheFutureClient client)
        {
            try
            {
                await client.Login("Space-Biscuits", "672920f3-681d-459f-b71d-c49b6633d5ae");
                Console.WriteLine("Token set for authorization.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        private static async Task<Dictionary<string, string>> FetchAlphabetFromApi(HackTheFutureClient client)
        {
            try
            {
                var response = await client.GetAsync("/api/b/easy/alphabet");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch alphabet. Status code: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var alphabet = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                return alphabet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching alphabet: {ex.Message}");
                return null;
            }
        }

        private static async Task<string> FetchEncryptedMessageFromApi(HackTheFutureClient client)
        {
            try
            {
                var response = await client.GetAsync("/api/b/easy/puzzle");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch encrypted message. Status code: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);

                if (responseData != null && responseData.ContainsKey("alienMessage"))
                {
                    return responseData["alienMessage"];
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching encrypted message: {ex.Message}");
                return null;
            }
        }

        private static string DecryptMessage(string encryptedMessage, Dictionary<string, string> alphabet)
        {

            var alphabetDecryption = alphabet.ToDictionary(kv => kv.Value, kv => kv.Key);

            var decryptedMessage = new StringBuilder();

            foreach (var symbol in encryptedMessage)
            {
                if (alphabetDecryption.ContainsKey(symbol.ToString()))
                {
                    decryptedMessage.Append(alphabetDecryption[symbol.ToString()]);
                }
                else
                {
                    decryptedMessage.Append(symbol); 
                }
            }
            return decryptedMessage.ToString();
        }
        private static async Task<HttpResponseMessage> PostResult(HackTheFutureClient client, string result)
        {
            try
            {
                var url = "/api/b/easy/puzzle";

                var content = new StringContent($"\"{result}\"", Encoding.UTF8, "application/json");

                return await client.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while posting result: {ex.Message}");
                return null;
            }
        }
        static async Task<HttpResponseMessage> PostSample(HackTheFutureClient client, string body)
        {
            return await client.PostAsJsonAsync("/api/b/easy/puzzle", body);
        }
    }
}