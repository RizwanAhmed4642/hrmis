using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hrmis.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??  new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}
