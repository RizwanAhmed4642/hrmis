using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Hrmis.Models.Extensions
{
    public static class CommonExtensions
    {
        public static string GetHfmisCodeNew(this IIdentity identity)
        {
            var claimIdentity = identity as ClaimsIdentity;
            var claim = claimIdentity?.FindFirst("HfmisCodeNew");
            if (claim != null)
            {
                if (!String.IsNullOrEmpty(claim.Value))
                {
                    return claim.Value;
                }

            }
            return null;
        }
    }
}