using Involved.HTF.Common.Dto;
using System.Net.Http.Json;

namespace Involved.HTF.Common;

public class HackTheFutureClient : HttpClient
{
    public HackTheFutureClient()
    {
        BaseAddress = new Uri("https://app-htf-2024.azurewebsites.net/");
    }

    public async Task Login(string teamname, string password)
    {
        var response = await GetAsync($"/api/team/token?teamname={teamname}&password={password}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("You weren't able to log in, did you provide the correct credentials?");
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
    }
}