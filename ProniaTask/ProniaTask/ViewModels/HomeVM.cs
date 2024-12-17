using ProniaTask.Models;

namespace ProniaTask.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}
