using BlazorServerCms.Data;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace BlazorServerCms.servicos
{
    public class ChatGpt
    {
        public async Task<ChatGptViewModel> buscar(IConfiguration Configuration, string prompt)
        {
            var token = Configuration.GetConnectionString("Token");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
            ChatGptInputModel chat = new ChatGptInputModel(prompt);
            var requestBody = JsonConvert.SerializeObject(chat);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
            var result = (ChatGptViewModel)await response.Content.ReadFromJsonAsync(typeof(ChatGptViewModel));

            return result;
        }
    }
}
