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
        var response = await client.PostAsJsonAsync("webhook-test/e605c19c-794b-4043-ba84-0f48322cdef3", payload);
        
        response.EnsureSuccessStatusCode();
    }
}