
using business.business;
using business.business.sistema;

public class UserPasskey : BaseModel
{    
    // Vincula a chave ao usuário do Identity
    public string? UserModelId { get; set; } 

    public virtual UserModel UserModel { get; set; } 
    
    // Identificador único da credencial (Gerado pelo navegador)
    public string CredentialId { get; set; } = string.Empty;
    
    // A chave pública em si (usada para conferir a assinatura no login)
    public string PublicKey { get; set; } = string.Empty;
    
    // Contador de uso (evita ataques de replicação de pacote)
    public uint Counter { get; set; }
}