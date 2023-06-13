using business.div;

namespace business.Join
{
    public class DivContainer
    {
        public Int64? ContainerId { get; set; }
        public Int64? DivId { get; set; }
        public virtual Div? Div { get; set; }
        public virtual Container? Container { get; set; }
    }
}
