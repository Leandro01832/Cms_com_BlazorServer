using business;

namespace business.business
{

public class Hashtag : BaseModel
{
    public string Name { get; set; }

    public virtual List<HashtagContent> HashtagContent { get; set; }

    public string? UserModelId { get; set; }

    public virtual UserModel UserModel { get; set; }
}

}