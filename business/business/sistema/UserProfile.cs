using business.business;
using business.business.Group;
using business.business.sistema;

public class UserProfile : BaseModel
{
    public string? UserModelId { get; set; } 
    public virtual UserModel? UserModel { get; set; }
    public long? PortalId { get; set; } 
    public virtual Portal? Portal { get; set; }    
    public bool Ativo { get; set; } 
}