using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using mvc.Models.Entities;

namespace mvc.Models.Services.Infrastructure
{
    public class AdoNetUserStore : 
            IUserStore<ApplicationUser>,
            IUserClaimStore<ApplicationUser>,
            IUserEmailStore<ApplicationUser>,
            IUserPasswordStore<ApplicationUser>,
            IUserPhoneNumberStore<ApplicationUser>
            // IUserSecurityStampStore<ApplicationUser>,
            // IUserTwoFactorStore<ApplicationUser>,
            // IUserTwoFactorRecoveryCodeStore<ApplicationUser>,
            // IUserAuthenticatorKeyStore<ApplicationUser>,
            // IUserAuthenticationTokenStore<ApplicationUser>,
            // IUserLockoutStore<ApplicationUser>,
            // IUserLoginStore<ApplicationUser>,
            // IUserConfirmation<ApplicationUser>            
    {
        private readonly IDatabaseAccessor db;

        public AdoNetUserStore(IDatabaseAccessor db)
        {
            this.db = db;
        }

        // TODO: nuovo metodo execute query con canc token
        

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken token)
        {
            //await db.CommandAsync.......
            // int rows = $INSERT INTO AspNetusers (Id....) VALUES ({user.Id})...)
            // if rows > 0
            // return IdentityResult.Success;


            //var error = new IdentityError {Description = "errore bla bla bla"};
            //return  IdentityResult.Failed(error);
            // else
            //

            int affectedRows = await db.CommandAsync(@$"INSERT INTO AspNetUsers 
                (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, 
                SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, 
                LockoutEnd, LockoutEnabled, AccessFailedCount, FullName) 
                VALUES ({user.Id}, {user.UserName}, {user.NormalizedUserName}, {user.Email}, 
                {user.NormalizedEmail}, {user.EmailConfirmed}, {user.PasswordHash}, {user.SecurityStamp}, 
                {user.ConcurrencyStamp}, {user.PhoneNumber}, {user.PhoneNumberConfirmed}, 
                {user.TwoFactorEnabled}, {user.LockoutEnd}, {user.LockoutEnabled}, {user.AccessFailedCount}, 
                {user.FullName})", token);

                

            if (affectedRows > 0)
            {
                // restituisce OK
                return IdentityResult.Success;
            }
            // restituisce KO
            var error = new IdentityError { Description = "Could not insert user" };
            return IdentityResult.Failed(error);            


        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUsers WHERE Id={user.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "User could not be found" };
            return IdentityResult.Failed(error);
        }

        public void Dispose()
        {
            
        }



        public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE Id={userId}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }

        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE NormalizedUserName={normalizedUserName}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }

    

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }



        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync(@$"UPDATE AspNetUsers SET UserName={user.UserName}, 
                NormalizedUserName={user.NormalizedUserName}, Email={user.Email}, NormalizedEmail={user.NormalizedEmail}, 
                EmailConfirmed={user.EmailConfirmed}, PasswordHash={user.PasswordHash}, SecurityStamp={user.SecurityStamp}, 
                ConcurrencyStamp={user.ConcurrencyStamp}, PhoneNumber={user.PhoneNumber}, PhoneNumberConfirmed={user.PhoneNumberConfirmed}, 
                TwoFactorEnabled={user.TwoFactorEnabled}, LockoutEnd={user.LockoutEnd}, LockoutEnabled={user.LockoutEnabled}, 
                AccessFailedCount={user.AccessFailedCount}, FullName={user.FullName} WHERE Id={user.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Could not update user" };
            return IdentityResult.Failed(error);
        }

        #region Implementation of IUserEmailStore<ApplicationUser>

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE NormalizedEmail={normalizedEmail}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken token)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken token)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken token)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
    
        #endregion


        #region Implementation of IUserPasswordStore<ApplicationUser>
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken token)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken token)
        {
            bool hasPassword = user.PasswordHash != null;
            return Task.FromResult(hasPassword);
        }



        #endregion

        #region Implementation of IUserPhoneNumberStore<Application>
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken token)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken token)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation of IUserClaimStore<ApplicationUser>

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUserClaims WHERE UserId={user.Id}", token);
            List<Claim> claims = dataSet.Tables[0].AsEnumerable().Select(row => new Claim(
                type: Convert.ToString(row["ClaimType"]),
                value: Convert.ToString(row["ClaimValue"])
            )).ToList();
            return claims;
        }
        
        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (Claim claim in claims)
            {
                int affectedRows = await db.CommandAsync($"INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES ({user.Id}, {claim.Type}, {claim.Value})", token);
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("Couldn't add the claim");
                }
            }
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken token)
        {
            await RemoveClaimsAsync(user, new[] { claim }, token);
            await AddClaimsAsync(user, new[] { newClaim }, token);
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (Claim claim in claims)
            {
                int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUserClaims WHERE UserId={user.Id} AND ClaimType={claim.Type} AND ClaimValue={claim.Value}", token);
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("Couldn't remove the claim");
                }
            }
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT AspNetUsers.* FROM AspNetUserClaims INNER JOIN AspNetUsers ON AspNetUserClaims.UserId = AspNetUsers.Id WHERE AspNetUserClaims.ClaimType={claim.Type} AND AspNetUserClaims.ClaimValue={claim.Value}", token);
            List<ApplicationUser> users = dataSet.Tables[0].AsEnumerable().Select(row => ApplicationUser.FromDataRow(row)).ToList();
            return users;
        }
        #endregion

    

    }
}