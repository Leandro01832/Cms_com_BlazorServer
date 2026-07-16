using System;
using System.Collections.Generic;
 using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

public class LiveKitService
{
    private readonly string _apiKey = "";
    private readonly string _apiSecret = "";

   public string GerarTokenAcesso(string nomeSala, string identidadeUsuario, bool ehStreamer)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var chaveMarcada = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiSecret));

    // Define as permissões baseadas no papel do usuário
    var videoGrants = new Dictionary<string, object>
    {
        { "roomJoin", true },
        { "room", nomeSala },
        { "canPublish", ehStreamer },       // Se for Streamer, pode transmitir vídeo/áudio
        { "canPublishData", ehStreamer },   // Se for Streamer, pode enviar dados
        { "canSubscribe", true }            // Todo mundo pode assistir
    };

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, identidadeUsuario),
        new Claim(JwtRegisteredClaimNames.Iss, _apiKey),
        new Claim(JwtRegisteredClaimNames.Name, identidadeUsuario),
        new Claim("video", System.Text.Json.JsonSerializer.Serialize(videoGrants), ClaimValueTypes.String)
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(2), 
        SigningCredentials = new SigningCredentials(chaveMarcada, SecurityAlgorithms.HmacSha256Signature)
    };

    tokenDescriptor.Claims = new Dictionary<string, object>();
    foreach (var claim in claims)
    {
        if (claim.Type == "video") tokenDescriptor.Claims.Add("video", videoGrants);
        else tokenDescriptor.Claims.Add(claim.Type, claim.Value);
    }

    var tokenCompilado = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(tokenCompilado);
}

    public async Task<(string url, string streamKey)> CriarIngressDoOBSAsync()
    {
        // Substitua pela URL gerada para o seu projeto no LiveKit Cloud (começa com https://)
        string urlApiLiveKit = "wss://instagleo-rx9jiwj0.livekit.cloud"; 

        using var client = new HttpClient();
        
        // O LiveKit Cloud autentica usando o Token de administrador que já geramos no passo anterior
        string tokenAdmin = GerarTokenAcesso("sala-principal", "admin-sistema", true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenAdmin);

        // Corpo da requisição dizendo que queremos uma entrada do tipo RTMP (OBS)
        var payload = new
        {
            input_type = 0, // 0 corresponde a URL_INPUT (RTMP) no LiveKit
            name = "transmissao-obs",
            room_name = "sala-principal",
            participant_identity = "streamer-obs",
            participant_name = "Usuario OBS"
        };

        var response = await client.PostAsJsonAsync($"{urlApiLiveKit}/twirp/livekit.Ingress/CreateIngress", payload);

        if (response.IsSuccessStatusCode)
        {
            // Lê os dados de conexão que o LiveKit gerou de volta
            var resultado = await response.Content.ReadFromJsonAsync<System.Text.Json.Nodes.JsonNode>();
            string rtmpUrl = resultado?["url"]?.ToString() ?? "";
            string streamKey = resultado?["stream_key"]?.ToString() ?? "";

            return (rtmpUrl, streamKey);
        }
        
        throw new Exception("Falha ao se comunicar com o LiveKit Cloud para criar o Ingress.");
    }


}