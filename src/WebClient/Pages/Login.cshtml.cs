using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages
{
    public class LoginModel : PageModel
    {

        private readonly IHttpContextAccessor _accessor;
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Email { get; set; }

        public LoginModel(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            Name = string.Empty;
            Email = string.Empty;
        }

        public async Task<IActionResult> OnPost()
        {
            var ctx = _accessor.HttpContext;


            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Email, Email)
                };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme));
            await ctx.SignInAsync(principal);

            return Redirect("/");
        }
    }
}
