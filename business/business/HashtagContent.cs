using business;

namespace business.business
{

public class HashtagContent : BaseModel
{
    public virtual Hashtag Hashtag { get; set; }
    public long HashtagId { get; set; }
    public virtual Content Content { get; set; }
    public long ContentId { get; set; }
}

}