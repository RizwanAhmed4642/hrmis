using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Providers;
using Hrmis.Results;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.IdentityApiControllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            IdentityResult result = await UserManager.ChangePasswordAsync(userId, model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {

                return GetErrorResult(result);
            }
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var user = db.C_User.FirstOrDefault(x => x.Id.Equals(userId));
                user.hashynoty = model.NewPassword;
                db.SaveChanges();
            }
            return Ok(true);
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("SetAllPassword")]
        //public IHttpActionResult SetAllPassword()
        //{
        //    int count = 0;
        //    using (var db = new HR_System())
        //    {
        //        var users = db.C_User.Where(x => x.UserName.Length != 13).ToList();

        //        foreach (var user in users)
        //        {
        //            string password = string.Empty;
        //            password = Common.RandomString(5).ToLower();
        //            var res = EditPssword(password, user.UserName);
        //            count++;
        //            if (res == true)
        //            {
        //                Debug.WriteLine(count);
        //                Debug.WriteLine(user.UserName);
        //            }
        //            else
        //            {
        //                Debug.WriteLine(user.UserName + ";;;Error");
        //            }
        //        }
        //    }
        //    return Ok(true);
        //}

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName, null, null);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // Register multiple users
        [AllowAnonymous]
        [HttpGet]
        [Route("RegisterUsers")]
        public async Task<IHttpActionResult> RegisterUsers()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    Random random = new Random();
                    string chars = "ABCDEFGHJKMPRSTUVWYZ23456789";

                    for (int i = 30; i < 53; i++)
                    {
                        RegisterBindingModel model = new RegisterBindingModel();
                        model.UserName = "helpline" + i;

                        model.Password = new string(Enumerable.Repeat(chars, 5)
                      .Select(s => s[random.Next(s.Length)]).ToArray());
                        model.ConfirmPassword = model.Password;

                        model.Email = model.Password + "@gmail.com";

                        model.RoleName = "Helpline";
                        await Register(model);
                    }

                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                hashynoty = model.hashynoty,
                PhoneNumber = model.PhoneNumber,
                UserDetail = model.UserDetail,
                LevelID = model.LevelID,
                TehsilID = model.TehsilID,
                DistrictID = model.DistrictID,
                DivisionID = model.DivisionID,
                HfmisCodeNew = model.HfmisCodeNew,
                responsibleuser = User.Identity.GetUserName(),
                CreationDate = DateTime.UtcNow.AddHours(5),
                isActive = true
            };

            IdentityResult result = UserManager.Create(user, model.Password);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            else
            {
                UserManager.AddToRole(user.Id, model.RoleName);
                return Ok(true);
            }
        }

        [AllowAnonymous]
        [Route("RegisterJobApplicant")]
        public async Task<IHttpActionResult> RegisterJobApplicant(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.UserName = model.UserName.Replace("-", "");

            int ExistedUsers = UserManager.Users.Where(x => x.UserName.Trim().ToLower() == model.UserName.Trim().ToLower()).Count();

            if (ExistedUsers == 0)
            {
                string GeneratePassword = Common.RandomString(6);
                model.Password = GeneratePassword;
                model.ConfirmPassword = GeneratePassword;
                model.hashynoty = GeneratePassword;

                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    hashynoty = model.hashynoty,
                    PhoneNumber = model.PhoneNumber,
                    responsibleuser = User.Identity.GetUserName(),
                    CreationDate = DateTime.UtcNow.AddHours(5),
                    isActive = true
                };

                IdentityResult result = UserManager.Create(user, model.Password);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                else
                {
                    UserManager.AddToRole(user.Id, model.RoleName);
                    using (var db = new HR_System())
                    {

                        var merit = db.Merits.Where(x => x.CNIC == model.UserName).OrderByDescending(x => x.Id).FirstOrDefault();
                        merit.Status = "Existing";
                        db.SaveChanges();

                        var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == model.UserName);
                        Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = profile.MobileNo,
                                    Message = "Your computer generated password is: " + GeneratePassword + " against CNIC: " + profile.CNIC
                                }
                            });
                        string EmailMessage = "Your computer generated password is: " + GeneratePassword + " against CNIC: " + profile.CNIC;
                        Common.SendEmail(profile.EMaiL, "Primary Secondary Healthcare Department", EmailMessage);
                    }


                    return Ok(true);
                }
            }
            else
            {
                string GeneratePassword = Common.RandomString(6);
                EditPssword(GeneratePassword, model.UserName);

                using (var db = new HR_System())
                {
                    var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == model.UserName);
                    var merit = db.Merits.Where(x => x.CNIC == model.UserName).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (merit.Status == "New")
                    {
                        merit.Status = "Existing";
                        db.SaveChanges();
                    }

                    Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = merit.MobileNumber?.Replace("-",""),
                                    Message = "Your computer generated password is: " + GeneratePassword
                                }
                            });
                    string EmailMessage = "Your computer generated password is: " + GeneratePassword;
                    Common.SendEmail(merit.Email, "Primary Secondary Healthcare Department", EmailMessage);
                }
                return Ok(true);
            }

        }

        [HttpPost]
        [Route("Users")]
        public IHttpActionResult Users([FromBody] UsersFilter usersFilter)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    string userId = User.Identity.GetUserId();
                    string userName = User.Identity.GetUserName();

                    UserService _userService = new UserService();
                    string userHfmisCode = _userService.GetUser(userId).hfmiscode;
                    var query = _db.UsersUpdateds.Where(x => x.hfmis.StartsWith(userHfmisCode)).AsQueryable();
                    if (!userName.Equals("dpd"))
                    {
                        query = query.Where(x => x.isUpdated == true);
                    }
                    var roles = _db.C_Role.AsQueryable();
                    if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                    {
                        var roleIds = roles.Where(x => x.Name.ToLower().Contains("phfmc")).Select(s => s.Id).ToList();
                        var userIds = _db.Database.SqlQuery<string>("select UserId from _UserRole where RoleId in ('" + string.Join("','", roleIds) + "')").ToList();
                        query = query.Where(x => x.UserName.ToLower().Contains("phfmc") || x.UserName.ToLower().StartsWith("dm.") || userIds.Contains(x.Id)).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(usersFilter.UserName))
                    {
                        query = query.Where(x => x.UserName.ToLower().Contains(usersFilter.UserName)).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(usersFilter.RoleName))
                    {
                        query = query.Where(x => x.Role_Name.ToLower().Contains(usersFilter.RoleName.ToLower())).AsQueryable();
                    }

                    var totalRecords = query.Count();
                    var list = query.OrderBy(x => x.Role_Name).Skip(usersFilter.Skip).Take(usersFilter.PageSize).ToList();

                    var model = new TableResponse<UsersUpdated> { List = list, Count = totalRecords };

                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetUserById/{userId}")]
        public IHttpActionResult GetUserById(string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var user = _db.C_User.FirstOrDefault(x => x.Id.Equals(userId));
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserWithById/{userId}")]
        public IHttpActionResult GetUserWithById(string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var user = _db.UsersViews.FirstOrDefault(x => x.Id.Equals(userId));
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Boolean EditPssword(string Password, string UserName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Get the existing student from the db
            var user = UserManager.FindByName(UserName);

            // Update it with the values from the view model
            var passwordHasher = new PasswordHasher();
            if (!string.IsNullOrEmpty(Password))
            {
                user.PasswordHash = passwordHasher.HashPassword(Password);
                user.hashynoty = Password;
            }
            user.isActive = true;
            // Apply the changes if any to the db
            var result = UserManager.Update(user);
            if (result.Succeeded) return true;
            return false;
        }


        // POST api/Account/GetUser
        [AllowAnonymous]
        [Route("GetUser/{UserName}")]
        public async Task<IHttpActionResult> GetUser(string UserName)
        {
            if (UserName == "" || UserName == null)
            {
                return BadRequest("UserName is empty of null.");
            }

            ApplicationDbContext context = new ApplicationDbContext();
            int ExistedUsers = UserManager.Users.Where(x => x.UserName.Trim().ToLower() == UserName.Trim().ToLower()).Count();

            return (ExistedUsers == 0) ? Ok(true) : Ok(false);

        }// POST api/Account/GetUser
        [AllowAnonymous]
        [Route("GetUserPromotion/{UserName}")]
        public async Task<IHttpActionResult> GetUserPromotion(string UserName)
        {
            if (UserName == "" || UserName == null)
            {
                return BadRequest("UserName is empty of null.");
            }

            ApplicationDbContext context = new ApplicationDbContext();
            int ExistedUsers = UserManager.Users.Where(x => x.UserName.Trim().ToLower() == UserName.Trim().ToLower() && x.UserDetail.Equals("Promotion User")).Count();

            return (ExistedUsers == 0) ? Ok(true) : Ok(false);

        }
        [AllowAnonymous]
        [Route("GetUserFull/{UserName}")]
        public async Task<IHttpActionResult> GetUserFull(string UserName)
        {
            if (UserName == "" || UserName == null)
            {
                return BadRequest("UserName is empty of null.");
            }

            ApplicationDbContext context = new ApplicationDbContext();
            var user = UserManager.Users.FirstOrDefault(x => x.UserName.Trim().ToLower() == UserName.Trim().ToLower());

            return Ok(user);

        }
        // POST api/Account/GetRoles
        [AllowAnonymous]
        [Route("GetRoles")]
        public async Task<IHttpActionResult> GetRoles()
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                return Ok(roleManager.Roles.ToList());
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        public class UsersFilter : Paginator
        {
            public string UserName { get; set; }
            public string RoleName { get; set; }
        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
        private string GetDbExMessage(DbEntityValidationException dbx)
        {
            return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                .Aggregate("",
                    (current, validationError) =>
                        current +
                        $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        }
        #endregion
    }
}
