using Involved.HTF.Common;
using System.Text;
using System.Text.Json;

namespace BMedium
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HackTheFutureClient();
            bool loginSuccessful = await PerformLogin(client);

            if (loginSuccessful)
            {
                var dateValues = await FetchDateValuesFromApi(client);

                if (dateValues != null)
                {
                    await PostExpectedArrival(client, dateValues);
                }
            }
            else
            {
                Console.WriteLine("Exiting application.");
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

        private static async Task<DateValuesResponse> FetchDateValuesFromApi(HackTheFutureClient client)
        {
            try
            {
                var response = await client.GetAsync("/api/b/medium/sample");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch date values. Status code: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var dateValues = JsonSerializer.Deserialize<DateValuesResponse>(jsonResponse);

                if (dateValues != null)
                {
                    Console.WriteLine($"Send DateTime: {dateValues.SendDateTime}");
                    Console.WriteLine($"Travel Speed: {dateValues.TravelSpeed} lichtjaar/min");
                    Console.WriteLine($"Distance: {dateValues.Distance} lichtjaar");
                    Console.WriteLine($"Day Length: {dateValues.DayLength} uur");

                    return dateValues;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while fetching date values: {ex.Message}");
                return null;
            }
        }

        private static DateTime CalculateArrivalDateTime(string sendDateTime, double distance, double travelSpeed, int dayLength)
        {
            double travelTimeInMinutes = distance / travelSpeed;
            travelTimeInMinutes *= 2; 

            var sendDate = DateTime.Parse(sendDateTime);
            var arrivalDateTime = sendDate.AddMinutes(travelTimeInMinutes);

            int totalHours = arrivalDateTime.Hour;
            int totalMinutes = arrivalDateTime.Minute;

            if (totalHours >= dayLength)
            {
                int extraDays = totalHours / dayLength;
                int remainingHours = totalHours % dayLength;

                arrivalDateTime = arrivalDateTime
                    .AddDays(extraDays)                 
                    .AddHours(-totalHours)              
                    .AddHours(remainingHours);          
            }

            return arrivalDateTime;
        }


        private static async Task PostExpectedArrival(HackTheFutureClient client, DateValuesResponse dateValues)
        {
            try
            {
                var arrivalDateTime = CalculateArrivalDateTime(dateValues.SendDateTime, dateValues.Distance, dateValues.TravelSpeed, dateValues.DayLength);

                var formattedArrivalTime = arrivalDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                var content = new StringContent($"\"{formattedArrivalTime}\"", Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/b/medium/sample", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successfully sent expected arrival time: {formattedArrivalTime}");
                }
                else
                {
                    Console.WriteLine($"Failed to send expected arrival time. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while posting expected arrival time: {ex.Message}");
            }
        }
    }
}