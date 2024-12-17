namespace ProniaTask.Areas.Manage.ViewModels.Slider
{
    public class UpdateSliderVm
    {
        public int Id { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
    }
}
