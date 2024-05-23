using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using mvc.Models.Entities;

namespace mvc.Customizations.Identity
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public CustomClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            // aggiunta di un claim personalizzato, da riutilizzare nelle pagine senza accedere al DB
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("NomeCompleto", user.FullName));
            return identity;
        }



    }
}