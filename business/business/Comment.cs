using business.business;
using business.Group;
using System.ComponentModel.DataAnnotations;

namespace  business
{

    public class Comment : Content
    {
        public Int64? ContentId { get; set; }
        public virtual Content? Content { get; set; }
    }

}