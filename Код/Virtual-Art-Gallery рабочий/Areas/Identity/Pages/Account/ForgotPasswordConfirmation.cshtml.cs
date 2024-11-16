using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Virtual_Art_Gallery.Areas.Identity.Pages.Account
{
    public class ForgotPasswordConfirmationModel : PageModel
    {
        public void OnGet()
        {
            // Здесь можно добавить любую логику, которая потребуется при загрузке страницы подтверждения.
            // Например, логику для отслеживания статистики или отображения пользовательских сообщений.
        }
    }
}
