using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.IdentityApiControllers
{
    [RoutePrefix("api/StaffManage")]
    public class StaffManageApiController : ApiController
    {
        private HR_System db = new HR_System();

        private ApplicationUserManager _userManager;

        PasswordHasher passwordHasher = new PasswordHasher();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpGet]
        [Route("AddRole/{name}")]
        public async Task<IHttpActionResult> AddRole(string name)
        {
            try
            {
                var context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                if (string.IsNullOrWhiteSpace(name)) return BadRequest("Invalid Role Name");
                if (roleManager.FindByName(name) != null) return Ok(false);
                await roleManager.CreateAsync(new IdentityRole(name));
                return Ok(true);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("GetClaims/{userId}")]
        public async Task<IHttpActionResult> SaveClaim(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return BadRequest("No User Id to Process");
                var user = GetUserById(userId);
                var claims = GetUserClaimsById(userId);
                return Ok(new { user, claims });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("SaveClaims")]
        public async Task<IHttpActionResult> SaveClaim([FromBody]C_UserClaim userClaim)
        {
            try
            {
                if (userClaim == null) return BadRequest("No Data to Process");
                var user = GetUserById(userClaim.UserId);
                var claims = GetUserClaimsById(userClaim.UserId);

                if (userClaim.UserId == null)
                {
                    var typeValueMatch = claims.FirstOrDefault(x => x.ClaimType == userClaim.ClaimType && x.ClaimValue == userClaim.ClaimValue);
                    if (typeValueMatch == null)
                    {
                        db.C_UserClaim.Add(userClaim);
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    var findtype = db.C_UserClaim.FirstOrDefault(x => x.ClaimType == userClaim.ClaimType);
                    if (findtype != null)
                    {
                        findtype.ClaimValue = userClaim.ClaimValue;
                    }
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("RemoveClaim/{id}")]
        public async Task<IHttpActionResult> RemoveClaim([FromBody]int id)
        {
            try
            {

                var obj = db.C_UserClaim.FirstOrDefault(x => x.Id == id);
                if (obj == null) return Ok(false);
                db.C_UserClaim.Remove(obj);
                await db.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AddUser")]
        public async Task<IHttpActionResult> AddUser(CreateUserViewModel model)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                string password = string.Empty;
                password = Common.RandomString(5).ToLower();

                var user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                if (model.DivisionID != null)
                {
                    user.DivisionID = model.DivisionID.Equals("000") ? null : model.DivisionID;
                }
                if (model.DistrictID != null)
                {
                    user.DistrictID = model.DistrictID.Equals("000") ? null : model.DistrictID;
                }
                if (model.TehsilID != null)
                {
                    user.TehsilID = model.TehsilID.Equals("000") ? null : model.TehsilID;
                }
                if (model.hfmiscode != null)
                {
                    user.hfmiscode = model.hfmiscode.Equals("000") ? null : model.hfmiscode;
                    user.HfmisCodeNew = model.hfmiscode.Equals("000") ? null : model.hfmiscode;
                }
                user.isActive = true;
                user.LevelID = model.LevelID ?? 9;
                user.PhoneNumber = model.PhoneNumber;
                model.Password = password;
                model.ConfirmPassword = password;
                user.hashynoty = password;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = User.Identity.GetUserId();
                user.HfTypeCode = model.HfTypeCode;
                user.isUpdated = true;
                user.Cnic = model.Cnic;
                user.ProfileId = model.ProfileId;

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.roles != null && model.roles.Any())
                    {
                        foreach (var role in model.roles)
                        {
                            if (!string.IsNullOrEmpty(role))
                            {
                                UserManager.AddToRole(user.Id, role);
                            }
                        }
                    }
                    var resultf = await UserManager.UpdateAsync(user);
                    if (resultf.Succeeded)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        bool alerted = false;
                        if (model.ProfileId > 0)
                        {
                            var profile = db.HrProfiles.FirstOrDefault(x => x.Id == model.ProfileId);
                            alerted = new UserService().AlertRegisteredUser(profile, user, User.Identity.GetUserName());
                        }
                    
                        return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent" : ""));
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EditUser")]
        public async Task<IHttpActionResult> EditUser(CreateUserViewModel model)
        {
            try
            {
                var context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                _userManager = UserManager;
                var user = await _userManager.FindByIdAsync(model.Id);

                string password = string.Empty;
                password = Common.RandomString(5).ToLower();

                user.Email = model.Email;
                if (model.DivisionID != null)
                {
                    user.DivisionID = model.DivisionID.Equals("000") ? null : model.DivisionID;
                }
                if (model.DistrictID != null)
                {
                    user.DistrictID = model.DistrictID.Equals("000") ? null : model.DistrictID;
                }
                if (model.TehsilID != null)
                {
                    user.TehsilID = model.TehsilID.Equals("000") ? null : model.TehsilID;
                }
                if (model.hfmiscode != null)
                {
                    user.hfmiscode = model.hfmiscode.Equals("000") ? null : model.hfmiscode;
                    user.HfmisCodeNew = model.hfmiscode.Equals("000") ? null : model.hfmiscode;
                }
                user.isActive = true;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.hashynoty = password;
                user.UserDetail = model.UserDetail;
                user.PhoneNumber = model.PhoneNumber;
                user.DesigCode = model.DesigCode;
                user.ModifiedDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = User.Identity.GetUserId();
                user.HfTypeCode = model.HfTypeCode;
                user.isUpdated = model.isUpdated;
                user.Cnic = model.Cnic;
                user.ProfileId = model.ProfileId;

                if (model.roles.Any())
                {
                    if (user.Roles.Any())
                    {
                        //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                        var userRoles = user.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                        //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                        _userManager.RemoveFromRoles(model.Id, userRoles);
                    }

                    _userManager.Update(user);

                    foreach (var role in model.roles)
                    {
                        if (!string.IsNullOrEmpty(role))
                        {
                            _userManager.AddToRole(user.Id, role);
                        }
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    using (var db = new HR_System())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        bool alerted = false;
                        if (model.ProfileId > 0)
                        {
                            var profile = db.HrProfiles.FirstOrDefault(x => x.Id == model.ProfileId);
                            alerted = new UserService().AlertRegisteredUser(profile, user, User.Identity.GetUserName());
                        }

                        return Ok("User Edit Successfull" + (alerted ? " - SMS and E-mail notification has been sent" : ""));
                    }
                }

                // If we got this far, something failed, redisplay form
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("EditUserPhoneEmail")]
        public async Task<IHttpActionResult> EditUserPhoneEmail(CreateUserViewModel model)
        {
            try
            {
                var context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                _userManager = UserManager;
                var user = await _userManager.FindByIdAsync(model.Id);



                user.Email = model.Email;


                user.PhoneNumber = model.PhoneNumber;
                user.ModifiedDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = User.Identity.GetUserId();
                user.Cnic = model.Cnic;
                user.isUpdated = true;


                var result = await _userManager.UpdateAsync(user);
                //if (result.Succeeded)
                //{
                //    using (var db = new HR_System())
                //    {
                //        db.Configuration.ProxyCreationEnabled = false;
                //        var profile = db.HrProfiles.FirstOrDefault(x => x.Id == model.ProfileId);
                //        var alerted = new UserService().AlertRegisteredUser(profile, user, User.Identity.GetUserName());
                //        return Ok("User Edit Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " + profile.EmployeeName : ""));
                //    }
                //}

                // If we got this far, something failed, redisplay form
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetRoles")]
        public IHttpActionResult GetRoles()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                UserService _userService = new UserService();
                var currentUser = _userService.GetUser(User.Identity.GetUserId());
                if (currentUser.DivisionID == null)
                {
                    using (var _db = new HR_System())
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        var roles = _db.C_Role.Where(x => x.IsActive == true ).ToList();


                        return Ok(roles.Select(x => x.Name).ToList());
                    }
                        
                    //return Ok(db.C_Role.Select(x => x.Name ).ToList());
                }
                else
                {
                    List<string> newRoles = new List<string>();
                    newRoles.Add("Chief Executive Officer");
                    newRoles.Add("Health Facility");
                    newRoles.Add("Office Institutes");
                    newRoles.Add("PHFMC");
                    newRoles.Add("Primary");
                    newRoles.Add("Secondary");
                    newRoles.Add("District Computer Operator");
                    newRoles.Add("AdhocScrutiny");
                    newRoles.Sort();
                    return Ok(newRoles);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllModules")]
        public async Task<IHttpActionResult> GetAllModules()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    return Ok(await _db.C_ErpModule.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAllComponents")]
        public async Task<IHttpActionResult> GetAllComponents()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    return Ok(await _db.C_ErpComponent.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public List<C_User> GetUserById(string userId)
        {
            var user = db.C_User.Where(x => x.Id.Equals(userId)).ToList();
            return user;
        }
        public List<C_UserClaim> GetUserClaimsById(string userId)
        {
            var claims = db.C_UserClaim.Where(x => x.UserId.Equals(userId)).ToList();
            return claims;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
