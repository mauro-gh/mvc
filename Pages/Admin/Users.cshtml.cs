using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using mvc.Models.Entities;
using mvc.Models.Enums;
using mvc.Models.InputModels.Users;

namespace mvc.Pages
{
    [Authorize(Roles = nameof(Role.Administrator))]
    public class Users : PageModel
    {
       // VIDEO 182.00.01
       // Admin: admin@example.com, vanna.palermo@example.com
       // Docente: salvatore.marino@example.com, sandro.longo@example.com
       // User: anastasia.trentini@example.com

        private readonly ILogger<Users> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public UserRoleInputModel Input {get;set;}
        public IList<ApplicationUser> ListaUtenti { get; private set; } // <--- nessun attributo, la valorizziamo da codice e non dal model binder

        public Users(ILogger<Users> logger, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]  // in questo modo funziona anche con richieste Get
        public Role InRole {get;set;} 


        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Title"] = "Gestione utenti";
            Claim claim = new Claim(ClaimTypes.Role, InRole.ToString()); //es Administrator, arriva da URL in base al tab selezionato (0 admin 1 docente)
            ListaUtenti = await _userManager.GetUsersForClaimAsync(claim);  // <--- se la lista e' ad esporre alla view, necessario creare la prop 
            return Page();

            // TODO: visualizzare tutti gli utenti col proprio ruolo assegnato

            
        }

        public async Task<IActionResult> OnPostAssignAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            // Cerca utente utilizzzando email
            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(Input.Email), "Indirizzo email non valido");
                return await OnGetAsync(); // non vengono persi i valori del form
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
                return await OnGetAsync(); // non vengono persi i valori del form
            }

            IdentityResult result = await _userManager.AddClaimAsync(user, claimDaAssegnare);
            if (result.Succeeded)
            {
                TempData["MessaggioDiConferma"] =  $@"Il ruolo {Input.Role} è stato assegnato all'utente {Input.Email}";
            }
            else
            {
                ModelState.AddModelError(string.Empty, $@"Operazione di aggiunta ruolo {Input.Role} fallita: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(); // non vengono persi i valori del form
            }


            // effettua redirect con metodo get e modelview completamente vuoto (come forzare un F5)
            return RedirectToPage(new { inrole = (int) InRole}); // con overload per non perdere in querystring InRole
        }

        public async Task<IActionResult> OnPostRevokeAsync()
        {
        if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            // Cerca utente utilizzzando email
            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(Input.Email), "Indirizzo email non valido");
                return await OnGetAsync(); // non vengono persi i valori del form
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
                return await OnGetAsync(); // non vengono persi i valori del form
            }

            IdentityResult result = await _userManager.RemoveClaimAsync(user, claimDaAssegnare);
            if (result.Succeeded)
            {
                TempData["MessaggioDiConferma"] =  $@"Il ruolo {Input.Role} è stato revocato all'utente {Input.Email}";
            }
            else
            {
                ModelState.AddModelError(string.Empty, $@"Operazione di revoca ruolo {Input.Role} fallita: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(); // non vengono persi i valori del form
            }


            // effettua redirect con metodo get e modelview completamente vuoto (come forzare un F5)
            return RedirectToPage(new { inrole = (int) InRole}); // con overload per non perdere in querystring InRole
        }
    }
}