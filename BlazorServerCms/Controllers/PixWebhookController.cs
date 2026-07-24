using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/pix")]
public class PixWebhookController : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> ReceberNotificacaoPix([FromBody] dynamic payload)
    {
        // Exemplo de captura de notificação do Mercado Pago
        string action = payload?.action;
        string type = payload?.type;

        if (type == "payment")
        {
            long paymentId = payload?.data?.id;

            // 1. Busca os detalhes do pagamento no PSP usando o paymentId
            var client = new PaymentClient();
            Payment pagamento = await client.GetAsync(paymentId);

            // 2. Verifica se o status mudou para "approved" (Aprovado/Pago)
            if (pagamento.Status == PaymentStatus.Approved)
            {
                // TODO: Atualizar o seu banco de dados (ex: aprovar pedido, liberar conteúdo)
            }
        }

        // Responda sempre 200 OK para o PSP saber que a notificação foi entregue
        return Ok();
    }
}