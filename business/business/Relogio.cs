
using business;
using business.business;
using business.business.Group;

public class Relogio : BaseModel
{
    public long SubFiltroId { get; set; }
    public virtual SubFiltro SubFiltro {get;set;}
    public long ContentId { get; set; }    
    public virtual Content Content { get; set; }
    public string UserModelId { get; set; }
    public virtual UserModel UserModel {get;set;}
}