using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly ILog log = LogManager.GetLogger(typeof(ApplicationOAuthProvider));
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {


                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

                ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);


                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    log.Error($"Login Failed for Username {context.UserName} The username or password is incorrect.");
                    return;
                }
                if (!user.isUpdated)
                {
                    context.SetError("invalid_grant", "NA");
                    log.Error($"NA");
                    return;
                }
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user.UserName, user, context);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);

                log.Info($"User {context.UserName} is Authenticated.");
            }
            catch (Exception ex)
            {
                log.Error($" Login Exception for Username {context.UserName} with Exception Message {ex.Message}");
                throw;
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }



        public static AuthenticationProperties CreateProperties(string userName, ApplicationUser user, OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var contactList = db.C_UserContact.Where(x => x.UserId == user.Id && x.IsActive == true).ToList();

                IDictionary<string, string> data = new Dictionary<string, string>
                {
                    { "userName", userName }
                };
                if (user != null && context != null)
                {

                    int code = 0;
                    string emailPattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{2}@)", phonePattern = "(?<=\\d{4})\\d(?=\\d{2})";
                    string codedPhone = "", codedEmail = "";
                    var userService = new UserService();
                    var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                    var userRoles = userManager.GetRoles(user.Id);
                    //if (!string.IsNullOrEmpty(user.PhoneNumber))
                    //{
                    //    code = userService.SendAuthenticationCode(user.UserName, HttpContext.Current.IsDebuggingEnabled ? "03324862798" : user.PhoneNumber, HttpContext.Current.IsDebuggingEnabled ? "belalmughal@gmail.com" : user.Email);
                    //    codedPhone = Regex.Replace(user.PhoneNumber, phonePattern, m => new string('*', m.Length));
                    //}





                    codedEmail = Regex.Replace(user.Email, emailPattern, m => new string('*', m.Length));

                    data.Add("user", JsonConvert.SerializeObject(new
                    {
                        Id = user.Id,
                        DivisionCode = user.DivisionID,
                        DistrictCode = user.DistrictID,
                        TehsilCode = user.TehsilID,
                        HfmisCode = user.HfmisCodeNew,
                        UserName = user.UserName,
                        HfTypeCode = user.HfTypeCode,
                        cnic = user.Cnic,
                        phonenumber = user.PhoneNumber,
                        codedPhone = codedPhone,
                        codedEmail = codedEmail,
                        cpo = code,
                        RoleName = string.Join(",", userRoles)
                    }));

                    var nav = userService.GetNav(user.UserName, userRoles);

                    data.Add("nav", JsonConvert.SerializeObject(nav));
                    data.Add("contactList", JsonConvert.SerializeObject(contactList));
                }
                return new AuthenticationProperties(data);
            }
        }

        


    }
}