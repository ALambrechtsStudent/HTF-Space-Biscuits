using Involved.HTF.Common;
using Involved.HTF.Common.Entities;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace MoveShip.Cons
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var htfClient = new HackTheFutureClient();

            await htfClient.Login("Space-Biscuits", "672920f3-681d-459f-b71d-c49b6633d5ae");

            var start = await StartChallenge(htfClient);

            var response = await CallSample(htfClient);
            string responseString = await response.Content.ReadAsStringAsync();
            var movementResponse = JsonConvert.DeserializeObject<MovementResponse>(responseString);

            var commandsList = movementResponse.commands
                .Split(',')
                .Select(command => command.Trim())
                .ToList();

            var shipSub = new ShipSub();

            foreach (var command in commandsList)
            {
                Console.WriteLine(command);
                shipSub.ExecuteCommand(command);
            }

            Console.WriteLine(shipSub.Distance);
            var solutionResponse = await PostSample(htfClient, shipSub.Distance);
        }

        static async Task<HttpResponseMessage> CallSample(HttpClient htfClient)
        {
            return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/easy/sample");
        }

        static async Task<HttpResponseMessage> StartChallenge(HttpClient htfClient)
        {
            return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/easy/start");
        }

        static async Task<HttpResponseMessage> PostSample(HttpClient htfClient, int body)
        {
            return await htfClient.PostAsJsonAsync("https://app-htf-2024.azurewebsites.net/api/a/easy/sample", body);
        }
    }
}
