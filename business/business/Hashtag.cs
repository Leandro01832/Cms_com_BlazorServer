using business;

namespace business.business
{

public class Hashtag : BaseModel
{
    public string Name { get; set; }

    public virtual List<HashtagContent> HashtagContent { get; set; }
}

}