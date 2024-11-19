using Involved.HTF.Common;
using Involved.HTF.Common.Entities;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.NetworkInformation;

namespace Quadriallian.Cons
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
                var quads = JsonConvert.DeserializeObject<QuatralianDto>(jsonResponse);
                List<int> ints = translateQuadToInt(quads.QuatralianNumbers);

                string solution = convertIntToQuat(ints);

                var solutionResponse = await PostSample(htfClient, solution);
                string solutionString = await solutionResponse.Content.ReadAsStringAsync();
            }


            static async Task<HttpResponseMessage> CallSample(HttpClient htfClient)
            {
                return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/hard/puzzle");
            }

            static async Task<HttpResponseMessage> StartChallenge(HttpClient htfClient)
            {
                return await htfClient.GetAsync("https://app-htf-2024.azurewebsites.net/api/a/hard/start");
            }

            static async Task<HttpResponseMessage> PostSample(HttpClient htfClient, string body)
            {
                return await htfClient.PostAsJsonAsync("https://app-htf-2024.azurewebsites.net/api/a/hard/puzzle", body);
            }

            static List<int> translateQuadToInt(List<string> quads)
            {
                List<int> ints = new List<int>();

                foreach (var quad in quads)
                {
                    if (quad.Contains(" "))
                    {
                        List<string> contents = quad.Split(" ").ToList();
                        string power = contents[0];
                        string remainder = contents[1];

                        ints.Add(convertBigQuad(power, remainder));
                    }
                    else if (quad.Contains("Ⱄ"))
                    {
                        ints.Add(0);
                    }
                    else
                    {
                        ints.Add(convertSmallQuad(quad));
                    }
                }

                return ints;
            }
        }

        private static int convertSmallQuad(string quad)
        {
            int output = 0;

            foreach (char c in quad)
            {
                if (c == '|')
                {
                    output += 5;
                }
                else if (c == '·')
                {
                    output += 1;
                }
                else
                {
                    output += 0;
                }
            }

            return output;
        }

        private static int convertBigQuad(string power, string remainder)
        {
            int output = 0;

            int multiplier = convertSmallQuad(power);

            output += multiplier * 20;

            output += convertSmallQuad(remainder);

            return output;
        }

        private static string convertIntToQuat(List<int> ints)
        {
            int integer = 0;
            string output = "";
            bool addZero = false;

            foreach (int i in ints)
            {
                integer += i;
            }

            int numLength = integer.ToString().Length;
            char lastChar = integer.ToString()[integer.ToString().Length - 1];

            if (lastChar == '0')
            {
                addZero = true;
            }

            if (integer > 19)
            {
                int integerBig = integer / 20;

                int remainer = integerBig % 5;
                int bigNumber = integerBig / 5;

                for (int i = 0; i < bigNumber; i++)
                {
                    output += "|";
                }

                for (int i = 0; i < remainer; i++)
                {
                    output += "·";
                }

                output += " ";
                integer -= integerBig * 20;
            }

            if (integer < 20)
            {
                int remainer = integer % 5;
                int bigNumber = integer / 5;

                for (int i = 0; i < bigNumber; i++)
                {
                    output += "|";
                }

                for (int i = 0; i < remainer; i++)
                {
                    output += "·";
                }
            }

            if (addZero)
            {
                output += "Ⱄ";
            }

            return output;
        }
    }
}
