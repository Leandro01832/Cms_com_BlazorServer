using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("[controller]")]
public class AuthController : Controller
{
    // 1. Action para iniciar o fluxo de login com o Google
    [HttpGet("login-google")]
    public IActionResult LoginGoogle(string returnUrl = "/")
    {
        // Define para onde o usuário deve ser enviado APÓS o callback ser processado
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
        };

        // Dispara o Challenge usando o esquema do Google
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // 2. Action que recebe a resposta do Google
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
    {
        // Autentica contra o esquema temporário/cookie do protocolo externo
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            // Falha na autenticação (usuário cancelou ou ocorreu erro)
            return RedirectToAction("LoginFailure");
        }

        // Obtém as claims enviadas pelo Google (Nome, E-mail, ID único, etc.)
        var claims = result.Principal.Claims;
        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

        // AQUI: Você pode consultar/salvar o usuário no seu banco de dados
        // ex: await _userService.GetOrCreateUserAsync(email, name, googleId);

        // Redireciona o usuário para a página final pretendida
        return LocalRedirect(returnUrl);
    }

    // 3. Action para Logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Limpa o cookie de autenticação local
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("login-failure")]
    public IActionResult LoginFailure()
    {
        return View("Erro ao autenticar com o Google.");
    }
}