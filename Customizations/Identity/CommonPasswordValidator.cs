using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mvc.Customizations.Identity
{
    public class CommonPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class

    {
        private readonly string[] commons;

        public CommonPasswordValidator()
        {
            // puo' utilizzare la Dependency Injection
            commons = ["password", "abc", "123",];
            
            
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
        {
            IdentityResult result;
            if (commons.Any(common => password.Contains(common, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = IdentityResult.Failed(new IdentityError {Description = "Troppo banale dai!"});
            }
            else
            {
                result = IdentityResult.Success;
            }

            return Task.FromResult(result);
        }
    }
}