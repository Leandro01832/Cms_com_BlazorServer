using MercadoPago.Config;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;

public class PixService
{
    public PixService()
    {
        // Define o Token de Acesso das suas credenciais no PSP
       // MercadoPagoConfig.AccessToken = accessToken;
    }

    public async Task<Payment> CriarCobrancaPixAsync(decimal valor, string emailCliente, string descricao)
    {
        var request = new PaymentCreateRequest
        {
            TransactionAmount = valor,
            Description = descricao,
            PaymentMethodId = "pix",
            Payer = new PaymentPayerRequest
            {
                Email = emailCliente
            },
            // URL da sua API para onde o PSP vai enviar o aviso de pagamento confirmado
            NotificationUrl = "https://seu-dominio.com/api/pix/webhook" 
        };

        var client = new PaymentClient();
        Payment payment = await client.CreateAsync(request);

        return payment;
    }
}