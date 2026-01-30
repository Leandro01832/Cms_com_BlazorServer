public class N8nService
{
    private readonly IHttpClientFactory _clientFactory;

    public N8nService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task EnviarDadosAsync(object payload)
    {
        var client = _clientFactory.CreateClient("n8nClient");
        var response = await client.PostAsJsonAsync("", payload);
        
        response.EnsureSuccessStatusCode();
    }
}