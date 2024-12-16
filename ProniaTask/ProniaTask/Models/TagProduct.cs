using ProniaTask.Models.Base;

namespace ProniaTask.Models
{
    public class TagProduct:BaseEntity
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
