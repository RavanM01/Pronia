using ProniaTask.Models.Base;

namespace ProniaTask.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public List<TagProduct> TagProducts { get; set; }
    }
}
