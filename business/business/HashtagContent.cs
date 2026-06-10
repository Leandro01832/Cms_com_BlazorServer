using business;

namespace business.business
{

public class HashtagContent 
{
    public virtual Hashtag Hashtag { get; set; }
    public long HashtagId { get; set; }
    public virtual Content Content { get; set; }
    public long ContentId { get; set; }
    
}

}