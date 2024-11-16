using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Virtual_Art_Gallery.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле электронной почты обязательно.")]
            [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты.")]
            [Display(Name = "Электронная почта")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Не раскрываем информацию о том, существует ли пользователь
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // Логика отправки электронной почты со ссылкой для сброса пароля
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
