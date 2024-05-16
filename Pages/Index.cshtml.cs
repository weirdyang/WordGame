using Microsoft.AspNetCore.Mvc.RazorPages;

namespace wordgame.Pages
{
    public class IndexModel : PageModel
    {
        public bool Random { get; set; }
        public void OnGet(bool? random)
        {
            Random = (random is null || random.GetValueOrDefault(false) is false)
                ? false
                : true;
        }
    }
}
