using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using business.business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BlazorServerCms.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using business.business.sistema;

namespace BlazorServerCms.Controllers
{
    [ApiController]
    [Route("api/webauthn")]
    public class WebAuthnController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        public SignInManager<UserModel> SignInManager { get; }
        public ApplicationDbContext Context { get; }

        public WebAuthnController(UserManager<UserModel> userManager,
         SignInManager<UserModel> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            SignInManager = signInManager;
            Context = context;
        }

        [HttpPost("register-options")]
        public async Task<IActionResult> GetRegisterOptions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Usuário não autenticado.");

            var challenge = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            
            var opcoes = new
            {
                publicKey = new
                {
                    challenge = challenge,
                    rp = new { name = "Instagleo" },
                    user = new { 
                        id = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id)), 
                        name = user.UserName, 
                        displayName = user.UserName 
                    },
                    pubKeyCredParams = new[] { new { type = "public-key", alg = -7 } },
                    timeout = 60000,
                    authenticatorSelection = new { authenticatorAttachment = "platform", userVerification = "required", residentKey = "required" }
                }
            };

            return Ok(opcoes);
        }
    
   
// ... outros usings
        [AllowAnonymous]
       [HttpPost("login-options")]
        public async Task<IActionResult> GetLoginOptions([FromForm] string username)
        {
            // 1. Gera um desafio (Challenge) aleatório e seguro para o login
            var desafioGerado = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

            var opcoes = new
            {
                publicKey = new
                {
                    challenge = desafioGerado, // Resolvido o conflito de escopo aqui
                    timeout = 60000,
                    rpId = "localhost", // Deve ser exatamente igual ao usado no registro
                    userVerification = "required"
                }
            };

            return Ok(opcoes);
        }

        [HttpPost("register-credential")]
        public async Task<IActionResult> RegisterCredential([FromBody] System.Text.Json.JsonElement dadosBiometria)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var novaChave = new UserPasskey
            {
                UserModelId = user.Id,
                CredentialId = dadosBiometria.GetProperty("id").GetString() ?? "",
                // Salvando a chave pública simulada (em produção usa-se a biblioteca Fido2NetLib para extrair a chave real)
                PublicKey = dadosBiometria.GetProperty("response").GetProperty("attestationObject").GetString() ?? ""
            };

            Context.UserPasskey.Add(novaChave);
            await Context.SaveChangesAsync();

            return  Ok(new { success = true });
        }
   
    }

  
}