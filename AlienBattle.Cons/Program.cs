using Involved.HTF.Common;
using Involved.HTF.Common.Dto;
using Involved.HTF.Common.Entities;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AlienBattle.Cons
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var htfClient = new HackTheFutureClient();

            await htfClient.Login("Space-Biscuits", "672920f3-681d-459f-b71d-c49b6633d5ae");

            var start = await StartChallenge(htfClient);

            var response = await CallSample(htfClient);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into BattleOfNovaCentauriDto object
                var battleResult = JsonConvert.DeserializeObject<BattleOfNovaCentauriDto>(jsonResponse);

                var resultAfterFight = SimulateBattle(battleResult.TeamA, battleResult.TeamB);

                var solutionResponse = await PostSample(htfClient, resultAfterFight);
                string solutionString = await solutionResponse.Content.ReadAsStringAsync();
            }


            static async Task<HttpResponseMessage> CallSample(HttpClient htfClient)
            {
                return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/medium/puzzle");
            }

            static async Task<HttpResponseMessage> StartChallenge(HttpClient htfClient)
            {
                return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/medium/start");
            }

            static async Task<HttpResponseMessage> PostSample(HttpClient htfClient, GameResultDto body)
            {
                return await htfClient.PostAsJsonAsync("https://app-htf-2024.azurewebsites.net/api/a/medium/puzzle", body);
            }

            static GameResultDto SimulateBattle(List<Alien> teamA, List<Alien> teamB)
            {
                var alienA = teamA[0];
                var alienB = teamB[0];

                var aliveAliensA = NumberOfAliens(teamA);
                var aliveAliensB = NumberOfAliens(teamB);

                while (aliveAliensA > 0 && aliveAliensB > 0)
                {
                    if (alienA.Health <= 0 && (teamA.IndexOf(alienA) + 1) != teamA.Count)
                    {
                        alienA = teamA[teamA.IndexOf(alienA) + 1];
                    }
                    else if (alienB.Health <= 0 && (teamB.IndexOf(alienB) + 1) != teamB.Count)
                    {
                        alienB = teamB[teamB.IndexOf(alienB) + 1];
                    }

                    if (alienA.Speed >= alienB.Speed)
                    {
                        BattleAliens(alienA, alienB);
                    }
                    else
                    {
                        BattleAliens(alienB, alienA);
                    }

                    aliveAliensA = NumberOfAliens(teamA);
                    aliveAliensB = NumberOfAliens(teamB);
                }

                var result = new GameResultDto();

                if (NumberOfAliens(teamA) > 0)
                {
                    result.WinningTeam = "TeamA";
                    result.RemainingHealth = 0;
                    foreach (var alien in teamA)
                    {
                        if (alien.Health > 0)
                        {
                            result.RemainingHealth += alien.Health;
                        }
                    }
                }
                else
                {
                    result.WinningTeam = "TeamB";
                    result.RemainingHealth = 0;
                    foreach (var alien in teamB)
                    {
                        if (alien.Health > 0)
                        {
                            result.RemainingHealth += alien.Health;
                        }
                    }
                }

                Console.WriteLine(result);
                return result;
            }

            static int NumberOfAliens(List<Alien> team)
            {
                var aliveAliens = 0;

                foreach (var alien in team)
                {
                    if (alien.Health > 0)
                    {
                        aliveAliens++;
                    }
                }

                Console.WriteLine(aliveAliens);
                return aliveAliens;
            }

            static void BattleAliens(Alien alienA, Alien alienB)
            {
                while (alienA.Health > 0 && alienB.Health > 0)
                {
                    alienB.Health -= alienA.Strength;

                    if (alienB.Health > 0)
                    {
                        alienA.Health -= alienB.Strength;
                    }
                }
            }

        }
    }
}
