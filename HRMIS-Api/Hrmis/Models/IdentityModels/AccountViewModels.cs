using System;
using System.Collections.Generic;

namespace Hrmis.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class CreatePublicUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserDetail { get; set; }

        public string PhoneNumber { get; set; }
        public string responsibleuser { get; set; }
        public string hashynoty { get; set; }
        public object Identity { get; internal set; }
        public string HfTypeCode { get; set; }


        public List<string> roles { get; set; }
        public bool isUpdated { get; set; }
        public string Cnic { get; set; }
        public int ProfileId { get; set; }
    }
    public class CreateUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string EmailSecondary { get; set; }

        public string UserName { get; set; }
        
        public string UserDetail { get; set; }
        
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
                
        public string DivisionID { get; set; }
        
        public string DistrictID { get; set; }
        
        public string TehsilID { get; set; }
        
        public string hfmiscode { get; set; }



        public int? LevelID { get; set; }
        public int? DesigCode { get; set; }
        public bool isActive { get; set; }
        public string HfmisCodeNew { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberSecondary { get; set; }
        public string responsibleuser { get; set; }
        public string hashynoty { get; set; }
        public object Identity { get; internal set; }
        public string HfTypeCode { get; set; }


        public List<string> roles { get; set; }
        public bool isUpdated { get; set; }
        public string Cnic { get; set; }
        public int ProfileId { get; set; }
    }

    public class IdentityUserClaimModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
