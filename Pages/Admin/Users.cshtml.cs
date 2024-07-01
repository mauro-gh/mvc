using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using mvc.Models.Entities;
using mvc.Models.InputModels.Users;

namespace mvc.Pages
{
    public class Users : PageModel
    {
       
        private readonly ILogger<Users> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public UserRoleInputModel Input {get;set;}

        public Users(ILogger<Users> logger, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            _logger = logger;
        }


        public IActionResult OnGet()
        {
            ViewData["Title"] = "Gestione utenti";
            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }

            // Cerca utente utilizzzando email
            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(Input.Email), "Indirizzo email non valido");
                return OnGet(); // non vengono persi i valori del form
            }

            // Recupera tutti i claims
            IList<Claim> claimsAssegnati = await _userManager.GetClaimsAsync(user);

            Claim claimDaAssegnare = new(ClaimTypes.Role, Input.Role.ToString());

            // Per verificare che non abbia gia' il ruolo assegnato
            foreach (Claim claim in claimsAssegnati) 
            {
                var aaa = claim.Type;
            }
            // meglio ancora:

            if (claimsAssegnati.Any(c => c.Type == claimDaAssegnare.Type && c.Value == claimDaAssegnare.Value))
            {   
                ModelState.AddModelError(nameof(Input.Role), $@"Il ruolo {Input.Role} è già assegnato all'utente {Input.Email}");
                return OnGet(); // non vengono persi i valori del form
            }

            IdentityResult result = await _userManager.AddClaimAsync(user, claimDaAssegnare);
            if (result.Succeeded)
            {
                TempData["MessaggioDiConferma"] =  $@"Il ruolo {Input.Role} è stato assegnato all'utente {Input.Email}";
            }
            else
            {
                ModelState.AddModelError(string.Empty, $@"Operazione di aggiunta ruolo {Input.Role} fallita: {result.Errors.FirstOrDefault()?.Description}");
                return OnGet(); // non vengono persi i valori del form
            }


            // effettua redirect con metodo get e modelview completamente vuoto (come forzare un F5)
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRevokeAsync()
        {
        if (!ModelState.IsValid)
            {
                return OnGet();
            }

            // Cerca utente utilizzzando email
            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(Input.Email), "Indirizzo email non valido");
                return OnGet(); // non vengono persi i valori del form
            }

            // Recupera tutti i claims
            IList<Claim> claimsAssegnati = await _userManager.GetClaimsAsync(user);

            Claim claimDaAssegnare = new(ClaimTypes.Role, Input.Role.ToString());

            // Per verificare che non abbia gia' il ruolo assegnato
            foreach (Claim claim in claimsAssegnati) 
            {
                var aaa = claim.Type;
            }
            // meglio ancora:

            if (!claimsAssegnati.Any(c => c.Type == claimDaAssegnare.Type && c.Value == claimDaAssegnare.Value))
            {   
                ModelState.AddModelError(nameof(Input.Role), $@"Il ruolo {Input.Role} non è assegnato all'utente {Input.Email}");
                return OnGet(); // non vengono persi i valori del form
            }

            IdentityResult result = await _userManager.RemoveClaimAsync(user, claimDaAssegnare);
            if (result.Succeeded)
            {
                TempData["MessaggioDiConferma"] =  $@"Il ruolo {Input.Role} è stato revocato all'utente {Input.Email}";
            }
            else
            {
                ModelState.AddModelError(string.Empty, $@"Operazione di revoca ruolo {Input.Role} fallita: {result.Errors.FirstOrDefault()?.Description}");
                return OnGet(); // non vengono persi i valori del form
            }


            // effettua redirect con metodo get e modelview completamente vuoto (come forzare un F5)
            return RedirectToPage();
        }
    }
}