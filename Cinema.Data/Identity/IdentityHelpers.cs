using Cinema.Data.Database;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Identity
{
    public static class IdentityHelpers
    {
        public static ApplicationUser GetApplicationUser(this System.Security.Principal.IIdentity identity)
        {
            if (identity.IsAuthenticated)
            {
                using (var db = new CinemaContext())
                {
                    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                    return userManager.FindByName(identity.Name);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
