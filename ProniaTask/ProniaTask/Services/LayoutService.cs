using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Services
{
    public class LayoutService
    {
        AppDBContext _context;

        public LayoutService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string,string>> GetSetting()
        {
            var setting=_context.Settings.ToDictionary(x=>x.Key,x=>x.Value);
            return setting;
        }
    }
}
