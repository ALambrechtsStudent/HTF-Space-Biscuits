using Involved.HTF.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace Start
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HackTheFutureClient();
            bool loginSuccessful = await PerformLogin(client);

            if (loginSuccessful)
            {
                Console.WriteLine("You are now logged in!");
                var movements = await FetchMovementsFromApi(client);

                if (movements != null)
                {
                    int result = CalculateDepthAndDistance(movements);
                    Console.WriteLine($"Result: {result}");
                    var endresult = await PostCalculationResult(client, result);
                    string solutionString = await endresult.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Failed to fetch movements from the API.");
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

        private static async Task<List<string>> FetchMovementsFromApi(HackTheFutureClient client)
        {
            try
            {
                Console.WriteLine("Fetching movements from API...");
                var response = await client.GetAsync("/api/a/easy/puzzle");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch movements. Status code: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw JSON response: {jsonResponse}");

                // Deserialiseer naar het model
                var movementResponse = JsonSerializer.Deserialize<MovementResponse>(jsonResponse);
                if (movementResponse == null || string.IsNullOrEmpty(movementResponse.Commands))
                {
                    Console.WriteLine("No commands found in the response.");
                    return null;
                }

                var movements = movementResponse.Commands.Split(',').ToList();
                return movements;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching movements: {ex.Message}");
                return null;
            }
        }

        private static int CalculateDepthAndDistance(List<string> movements)
        {
            int depthPerMeter = 0; 
            int totalDistance = 0; 
            int totalDepth = 0;    

            foreach (var movement in movements)
            {
                string[] parts = movement.Trim().Split(' ');
                string direction = parts[0];
                int value = int.Parse(parts[1]);

                switch (direction)
                {
                    case "Down":
                        depthPerMeter += value; 
                        break;
                    case "Up":
                        depthPerMeter -= value; 
                        break;
                    case "Forward":
                        totalDistance += value;         
                        totalDepth += depthPerMeter * value; 
                        break;
                    default:
                        Console.WriteLine($"Unknown movement: {movement}");
                        break;
                }
            }
            return totalDistance * totalDepth; 
        }
        private static async Task<HttpResponseMessage> PostCalculationResult(HackTheFutureClient client, int result)
        {
            try
            {
                var url = "/api/a/easy/puzzle";

                var response = await client.PostAsJsonAsync(url, result);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static async Task<HttpResponseMessage> PostSample(HackTheFutureClient client, string body)
        {
            return await client.PostAsJsonAsync("/api/a/easy/puzzle", body);
        }
    }
}
