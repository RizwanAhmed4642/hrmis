using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Application;
using Hrmis.Models.ViewModels.Common;
using Hrmis.Models.ViewModels.Vacancy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Public")]
    public class PublicController : ApiController
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

        [AllowAnonymous]
        [Route("GetProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfileByCNIC(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                using (var _db = new HR_System())
                {
                    var userId = User.Identity.GetUserId();
                    var name = User.Identity.GetUserName();
                    var user = db.C_User.FirstOrDefault(x => x.Id.Equals(userId));

                    var district = db.Districts.FirstOrDefault(x => x.Code == user.DistrictID);

                    if (name.StartsWith("ceo") && user.DistrictID != null)
                    {

                        _db.Configuration.ProxyCreationEnabled = false;
                        var findprofile = _db.ProfileDetailsViews.Where(x => x.District == district.Name).ToList();
                        var profile = findprofile.FirstOrDefault(x => x.CNIC.Equals(cnic));
                        if (profile != null)
                        {
                            return Ok(profile);
                        }
                        else
                        {
                            return Ok(false);
                            //return Ok(new { Status = false, Message = "Profile Does Not Exist In Ditrict", Data = "" });
                        }



                    }
                    else
                    {

                        db.Configuration.ProxyCreationEnabled = false;
                        ProfileDetailsView profileDetailsView = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                        if (profileDetailsView != null)
                        {

                            return Ok(profileDetailsView);
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                }

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetJobs")]
        public IHttpActionResult GetJobs()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobs = db.JobViews.Where(x => x.IsActive == true).OrderBy(k => k.OrderBy).ToList();
                    return Ok(jobs);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetJobByDesignation/{designationId}")]
        public IHttpActionResult GetJobByDesignation(int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobs = db.JobViews.FirstOrDefault(x => x.Designation_Id == designationId && x.IsActive == true);
                    return Ok(jobs);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetJobDocumentsRequired/{jobId}")]
        public IHttpActionResult GetJobDocumentsRequired(int jobId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobDocumentsRequired = db.JobDocuments.Where(x => x.IsActive == true).OrderBy(k => k.OrderBy).ToList();
                    return Ok(jobDocumentsRequired);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetJobPreferences/{jobId}")]
        public IHttpActionResult GetJobPreferences(int jobId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobPreferences = db.JobHFViews.Where(x => x.Job_Id == jobId && x.IsActive == true).ToList();
                    return Ok(jobPreferences);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("GetRegularProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetRegularProfileByCNIC(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    ProfileDetailsView profileDetailsView = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic) && x.EmpMode_Id == 13);
                    if (profileDetailsView != null)
                    {
                        return Ok(profileDetailsView);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [AllowAnonymous]
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
                password = Common.RandomString(5).ToUpper();

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
                user.LevelID = model.LevelID ?? 99;
                user.PhoneNumber = model.PhoneNumber;
                model.Password = password;
                model.ConfirmPassword = password;
                user.hashynoty = password;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = user.UserName;
                user.HfTypeCode = model.HfTypeCode;
                user.isUpdated = true;
                user.Cnic = model.UserName;
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
                        var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(user.UserName));
                        if (profile != null)
                        {
                            var promotionApply = new PromotionApply();
                            promotionApply.User_Id = user.Id;
                            promotionApply.Profile_Id = profile.Id;
                            promotionApply.CNIC = user.UserName;
                            promotionApply.Status = "Registered";
                            promotionApply.RegisterTime = DateTime.UtcNow.AddHours(5);

                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = "System";
                            elc.Users_Id = "System";
                            elc.IsActive = true;
                            elc.Entity_Id = 905;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();

                            promotionApply.EntityLifecycle_Id = elc.Id;
                            db.PromotionApplies.Add(promotionApply);
                            db.SaveChanges();
                        }
                        var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                        return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                    }

                }
                else
                {
                    var userDb = db.C_User.FirstOrDefault(x => x.UserName.Equals(user.UserName));
                    if (userDb != null)
                    {
                        _userManager = UserManager;
                        var userEdit = await _userManager.FindByIdAsync(userDb.Id);
                        userEdit.PasswordHash = passwordHasher.HashPassword(password);
                        userEdit.hashynoty = password;
                        userEdit.ModifiedDate = DateTime.UtcNow.AddHours(5);
                        userEdit.isUpdated = model.isUpdated;

                        if (model.roles.Any())
                        {
                            if (userEdit.Roles.Any())
                            {
                                //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                                var userRoles = userEdit.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                                //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                                _userManager.RemoveFromRoles(userDb.Id, userRoles);
                            }

                            _userManager.Update(userEdit);

                            foreach (var role in model.roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    _userManager.AddToRole(userEdit.Id, role);
                                }
                            }
                        }

                        var result2 = await _userManager.UpdateAsync(userEdit);
                        if (result2.Succeeded)
                        {
                            var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(userEdit.UserName));
                            var promotionApplyDb = db.PromotionApplyViews.FirstOrDefault(x => x.CNIC.Equals(userEdit.UserName));
                            if (profile != null && promotionApplyDb == null)
                            {
                                var promotionApply = new PromotionApply();
                                promotionApply.User_Id = userEdit.Id;
                                promotionApply.Profile_Id = profile.Id;
                                promotionApply.CNIC = userEdit.UserName;
                                promotionApply.Status = "Registered";
                                promotionApply.RegisterTime = DateTime.UtcNow.AddHours(5);

                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Created_By = "System";
                                elc.Users_Id = "System";
                                elc.IsActive = true;
                                elc.Entity_Id = 905;
                                db.Entity_Lifecycle.Add(elc);
                                db.SaveChanges();

                                promotionApply.EntityLifecycle_Id = elc.Id;
                                db.PromotionApplies.Add(promotionApply);
                                db.SaveChanges();
                            }
                            var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                            return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                        }
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("RegisterOnlineApplicant")]
        public async Task<IHttpActionResult> RegisterOnlineApplicant(CreateUserViewModel model)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                string password = string.Empty;
                password = Common.RandomString(5).ToUpper();

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
                user.LevelID = model.LevelID ?? 99;
                user.PhoneNumber = model.PhoneNumber;
                model.Password = password;
                model.ConfirmPassword = password;
                user.hashynoty = password;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = "System";
                user.HfTypeCode = model.HfTypeCode;
                user.isUpdated = true;
                user.Cnic = model.UserName;
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
                        var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                        return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                    }
                }
                else
                {
                    var userDb = db.C_User.FirstOrDefault(x => x.UserName.Equals(user.UserName));
                    if (userDb != null)
                    {
                        _userManager = UserManager;
                        var userEdit = await _userManager.FindByIdAsync(userDb.Id);
                        userEdit.PasswordHash = passwordHasher.HashPassword(password);
                        userEdit.hashynoty = password;
                        userEdit.ModifiedDate = DateTime.UtcNow.AddHours(5);
                        userEdit.isUpdated = true;
                        userEdit.ProfileId = model.ProfileId;
                        if (model.roles.Any())
                        {
                            if (userEdit.Roles.Any())
                            {
                                //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                                var userRoles = userEdit.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                                //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                                _userManager.RemoveFromRoles(userDb.Id, userRoles);
                            }

                            _userManager.Update(userEdit);

                            foreach (var role in model.roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    _userManager.AddToRole(userEdit.Id, role);
                                }
                            }
                        }

                        var result2 = await _userManager.UpdateAsync(userEdit);
                        if (result2.Succeeded)
                        {
                            var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                            return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                        }
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("AddUserPHFMC")]
        public async Task<IHttpActionResult> AddUserPHFMC(CreateUserViewModel model)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                //string password = string.Empty;
                //password = Common.RandomString(5).ToUpper();

                var user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.isActive = true;
                user.LevelID = 909;
                user.PhoneNumber = model.PhoneNumber;
                user.hashynoty = model.Password;
                user.PasswordHash = passwordHasher.HashPassword(model.Password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = "System";
                user.isUpdated = true;
                user.Cnic = model.UserName;

                var result = await UserManager.CreateAsync(user, model.Password);
                IdentityResult resultf = null;
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
                    resultf = await UserManager.UpdateAsync(user);
                }
                else
                {
                    var userDb = db.C_User.FirstOrDefault(x => x.UserName.Equals(user.UserName));
                    if (userDb != null)
                    {
                        _userManager = UserManager;
                        var userEdit = await _userManager.FindByIdAsync(userDb.Id);
                        userEdit.PasswordHash = passwordHasher.HashPassword(model.Password);
                        userEdit.hashynoty = model.Password;
                        userEdit.ModifiedDate = DateTime.UtcNow.AddHours(5);
                        userEdit.isUpdated = model.isUpdated;

                        if (model.roles.Any())
                        {
                            if (userEdit.Roles.Any())
                            {
                                //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                                var userRoles = userEdit.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                                //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                                _userManager.RemoveFromRoles(userDb.Id, userRoles);
                            }

                            _userManager.Update(userEdit);

                            foreach (var role in model.roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    _userManager.AddToRole(userEdit.Id, role);
                                }
                            }
                        }
                        resultf = await _userManager.UpdateAsync(userEdit);
                    }


                }
                if (resultf.Succeeded)
                {
                    var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                    return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " + user.PhoneNumber : ""));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("AddUserAdhoc")]
        public async Task<IHttpActionResult> AddUserAdhoc(CreateUserViewModel model)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                string password = string.Empty;
                password = Common.RandomString(5).ToUpper();

                var user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.isActive = true;
                user.LevelID = 909;
                user.PhoneNumber = model.PhoneNumber;
                user.hashynoty = password;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = "System";
                user.isUpdated = true;
                user.Cnic = model.UserName;
                model.Password = user.hashynoty;

                var result = await UserManager.CreateAsync(user, password);
                IdentityResult resultf = null;
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
                    resultf = await UserManager.UpdateAsync(user);
                }
                else
                {
                    var userDb = db.C_User.FirstOrDefault(x => x.UserName.Equals(user.UserName));
                    if (userDb != null)
                    {
                        _userManager = UserManager;
                        var userEdit = await _userManager.FindByIdAsync(userDb.Id);
                        userEdit.PasswordHash = passwordHasher.HashPassword(password);
                        userEdit.hashynoty = password;
                        userEdit.UserDetail = model.UserDetail;
                        userEdit.ModifiedDate = DateTime.UtcNow.AddHours(5);
                        userEdit.isUpdated = true;
                        model.Password = userEdit.hashynoty;

                        if (model.roles.Any())
                        {
                            if (userEdit.Roles.Any())
                            {
                                //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                                var userRoles = userEdit.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                                //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                                _userManager.RemoveFromRoles(userDb.Id, userRoles);
                            }

                            _userManager.Update(userEdit);

                            foreach (var role in model.roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    _userManager.AddToRole(userEdit.Id, role);
                                }
                            }
                        }
                        resultf = await _userManager.UpdateAsync(userEdit);
                    }


                }
                if (resultf.Succeeded)
                {
                    var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                    return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " + user.PhoneNumber : ""));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("FetchCovidData")]
        public IHttpActionResult GetCovidData()
        {
            try
            {
                RootObject result = new RootObject();
                string url = @"https://docs.google.com/spreadsheets/d/e/2PACX-1vQuDj0R6K85sdtI8I-Tc7RCx8CnIxKUQue0TCUdrFOKDw9G3JRtGhl64laDd3apApEvIJTdPFJ9fEUL/pubhtml?gid=0&single=true";

                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    int tableIndexStart = responseFromServer.IndexOf("<tbody");
                    int tableIndexEnd = responseFromServer.IndexOf("</tbody>");
                    string table = responseFromServer.Substring(tableIndexStart, (tableIndexEnd - tableIndexStart) + 8);
                    return Ok(ParseHTML(table));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool ParseHTML(string html)
        {
            List<COVIDTrend> counts = new List<COVIDTrend>();
            html = Regex.Replace(html, @"[\""]", "", RegexOptions.None);
            IDictionary<string, string> map = new Dictionary<string, string>()
                    {
                       {@"s0>",string.Empty},
                       {@"s1>",string.Empty},
                       {@"s0 softmerge><div class=softmerge-inner style=width: 97px; left: -1px;>",string.Empty},
                       {@"</div>",string.Empty},
                       {@"</td>",string.Empty},
                       {@"</tr></tbody>",string.Empty},
                       {@"</tr><tr style='height:20px;'>",string.Empty},
                    };
            var thS = html.Split(new string[] { "<th id=" }, StringSplitOptions.None).ToList();

            var dataRows = new List<List<string>>();
            for (int index = 1; index < thS.Count; index++)
            {
                var element = thS[index];
                var trS = element.Split(new string[] { "<td class=" }, StringSplitOptions.None).ToList();
                dataRows.Add(trS);
            }

            var d = new List<string>();

            for (int index = 1; index < dataRows.Count; index++)
            {
                var element = dataRows[index];
                string Nation = "";
                int ConfirmedCase = 0;
                int Death = 0;
                int Recover = 0;
                for (int index2 = 2; index2 < element.Count; index2++)
                {
                    var element2 = element[index2];
                    string val = element2;
                    var regex = new Regex(String.Join("|", map.Keys));
                    val = regex.Replace(val, m => map[m.Value]);
                    try
                    {

                        if (index2 == 2)
                        {
                            Nation = val;
                        }
                        if (index2 == 3)
                        {
                            ConfirmedCase = Convert.ToInt32(val);
                        }
                        if (index2 == 4)
                        {
                            Death = Convert.ToInt32(val);
                        }
                        if (index2 == 5)
                        {
                            Recover = Convert.ToInt32(val);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                counts.Add(new COVIDTrend { Nation = Nation, ConfirmedCase = ConfirmedCase, Death = Death, Recover = Recover });
            }
            var covidTrendsDB = db.COVIDTrends.ToList();
            if (covidTrendsDB.Count < counts.Count)
            {
                var dbNations = covidTrendsDB.Select(x => x.Nation).ToList();
                var newCountries = counts.Where(x => !dbNations.Contains(x.Nation)).ToList();
                foreach (var covidTrend in newCountries)
                {
                    covidTrend.DateTime = DateTime.UtcNow.AddHours(5);
                    db.COVIDTrends.Add(covidTrend);
                    db.SaveChanges();
                }

            }
            foreach (var covidTrend in covidTrendsDB)
            {
                var count = counts.FirstOrDefault(x => x.Nation.ToLower().Equals(covidTrend.Nation.ToLower()));
                if (count != null)
                {
                    covidTrend.DateTime = DateTime.UtcNow.AddHours(5);
                    CovidTrendLog ctl = new CovidTrendLog();
                    ctl.Description = covidTrend.Nation + ":";
                    bool changed = false;
                    if (count.ConfirmedCase != covidTrend.ConfirmedCase)
                    {
                        ctl.ConfirmedBefore = covidTrend.ConfirmedCase;
                        ctl.ConfirmedAfter = count.ConfirmedCase;
                        ctl.Description += " Confirmed " + (ctl.ConfirmedAfter - ctl.ConfirmedBefore);
                        covidTrend.ConfirmedCase = count.ConfirmedCase;
                        changed = true;
                    }
                    if (count.Death != covidTrend.Death)
                    {
                        ctl.DeathsBefore = covidTrend.Death;
                        ctl.DeathsAfter = count.Death;
                        ctl.Description += " Death " + (ctl.DeathsAfter - ctl.DeathsBefore);
                        covidTrend.Death = count.Death;
                        changed = true;
                    }
                    if (count.Recover != covidTrend.Recover)
                    {
                        ctl.RecoveredBefore = covidTrend.Recover;
                        ctl.RecoveredAfter = count.Recover;
                        ctl.Description += " Recovered " + (ctl.RecoveredAfter - ctl.RecoveredBefore);
                        covidTrend.Recover = count.Recover;
                        changed = true;
                    }
                    if (changed == true)
                    {
                        db.Entry(covidTrend).State = EntityState.Modified;
                        db.SaveChanges();
                        ctl.DateTime = covidTrend.DateTime;
                        ctl.CovidTrend_Id = covidTrend.Id;
                        db.CovidTrendLogs.Add(ctl);
                        db.SaveChanges();
                    }
                }
            }
            return true;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetCovidDBData")]
        public IHttpActionResult GetCovidDBData()
        {
            try
            {
                var covidData = db.COVIDTrends.Where(l => l.ConfirmedCase > 0).OrderByDescending(k => k.ConfirmedCase).ToList();
                return Ok(covidData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("AddEUser")]
        public async Task<IHttpActionResult> AddEUser(CreateUserViewModel model)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                string password = string.Empty;
                password = Common.RandomString(5).ToUpper();

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
                user.LevelID = model.LevelID ?? 99;
                user.PhoneNumber = model.PhoneNumber;
                model.Password = password;
                model.ConfirmPassword = password;
                user.hashynoty = password;
                user.PasswordHash = passwordHasher.HashPassword(password);
                user.UserDetail = model.UserDetail;
                user.DesigCode = model.DesigCode;
                user.CreationDate = DateTime.UtcNow.AddHours(5);
                user.responsibleuser = "System";
                user.HfTypeCode = model.HfTypeCode;
                user.isUpdated = true;
                user.Cnic = model.UserName;
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
                        var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                        return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                    }

                }
                else
                {
                    var userDb = db.C_User.FirstOrDefault(x => x.UserName.Equals(user.UserName));
                    if (userDb != null)
                    {
                        _userManager = UserManager;
                        var userEdit = await _userManager.FindByIdAsync(userDb.Id);
                        userEdit.PasswordHash = passwordHasher.HashPassword(password);
                        userEdit.hashynoty = password;
                        userEdit.ModifiedDate = DateTime.UtcNow.AddHours(5);
                        userEdit.isUpdated = model.isUpdated;

                        if (model.roles.Any())
                        {
                            if (userEdit.Roles.Any())
                            {
                                //model.roles.Add(roleManager.FindById(role?..RoleId).Name);
                                var userRoles = userEdit.Roles.Select(x => roleManager.FindById(x.RoleId).Name).ToArray();
                                //var userRoles = user.Roles.Select(x => x.ToString()).ToArray();
                                _userManager.RemoveFromRoles(userDb.Id, userRoles);
                            }

                            _userManager.Update(userEdit);

                            foreach (var role in model.roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    _userManager.AddToRole(userEdit.Id, role);
                                }
                            }
                        }

                        var result2 = await _userManager.UpdateAsync(userEdit);
                        if (result2.Succeeded)
                        {
                            var alerted = await new UserService().AlertPublicUser(model, User.Identity.GetUserName());
                            return Ok("User Registration Successfull" + (alerted ? " - SMS and E-mail notification has been sent to " : ""));
                        }
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("GetVCode")]
        public IHttpActionResult GetVCode(C_UserAuth userAuth)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                int code = 0;
                int.TryParse(Common.RandomCodeString(4).ToUpper(), out code);
                userAuth.Code = code;
                string message = code + " is your HRMIS verification code.";
                SMS sms = new SMS()
                {
                    UserId = "Public",
                    FKId = 0,
                    MobileNumber = userAuth.MobileNo,
                    Message = message
                };
                Thread t = new Thread(() => Common.SendSMSTelenor(sms));
                t.Start();

                var userAuthDb = db.C_UserAuth.FirstOrDefault(x => x.MobileNo.Equals(userAuth.MobileNo) && x.Email.Equals(userAuth.Email) && x.CNIC.Equals(userAuth.CNIC));
                if (userAuthDb == null)
                {
                    userAuth.DateTime = DateTime.UtcNow.AddHours(5);
                    userAuth.ExpiryDateTime = userAuth.DateTime.Value.AddMinutes(30);
                    db.C_UserAuth.Add(userAuth);
                }
                else
                {
                    userAuthDb.Code = userAuth.Code;
                    userAuthDb.DateTime = DateTime.UtcNow.AddHours(5);
                    userAuthDb.ExpiryDateTime = userAuthDb.DateTime.Value.AddMinutes(45);
                    db.Entry(userAuthDb).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Ok(code);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyVCode")]
        public IHttpActionResult VerifyVCode(C_UserAuth userAuth)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var userAuthDb = db.C_UserAuth.FirstOrDefault(x => x.MobileNo.Equals(userAuth.MobileNo) && x.Email.Equals(userAuth.Email) && x.CNIC.Equals(userAuth.CNIC));
                if (userAuthDb == null)
                {
                    return Ok("No Code Found.");
                }
                else
                {
                    if (userAuthDb.Code == userAuth.Code)
                    {
                        userAuthDb.VerifyDateTime = DateTime.UtcNow.AddHours(5);
                        db.Entry(userAuthDb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }



        //[AllowAnonymous]
        //[HttpGet]
        //[Route("GetHFList")]
        //public IHttpActionResult GetHFList()
        //{
        //    try
        //    {
        //        db.Configuration.ProxyCreationEnabled = false;
        //        //var hfIds = db.VPMasters.Where(x => (x.Desg_Id == 21 || x.Desg_Id == 22) && (x.TotalSanctioned - x.TotalWorking) > 0).Select(k => k.HF_Id).ToList();
        //        //var hfs = db.HFListPs.Where(x => (hfIds.Contains(x.Id) || x.HFCategoryName.Equals("Tertiary")) && x.IsActive == true).OrderBy(k => k.FullName).ToList();
        //        var hfs = db.HFOpenedLHRViews.ToList();
        //        return Ok(hfs);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        [AllowAnonymous]
        [HttpGet]
        [Route("GetHFList/{designationId}")]
        public IHttpActionResult GetHFList(int designationId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                //var hfIds = db.VPMasters.Where(x => (x.Desg_Id == 21 || x.Desg_Id == 22) && (x.TotalSanctioned - x.TotalWorking) > 0).Select(k => k.HF_Id).ToList();
                //var hfs = db.HFListPs.Where(x => (hfIds.Contains(x.Id) || x.HFCategoryName.Equals("Tertiary")) && x.IsActive == true).OrderBy(k => k.FullName).ToList();
                var hfs = db.HFOpenedLHRViews.Where(x => x.Designation_Id == designationId).ToList();
                return Ok(hfs);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("WaitingDocumentSMS")]
        //public bool WaitingDocumentSMS()
        //{
        //    try
        //    {
        //        db.Configuration.ProxyCreationEnabled = false;
        //        var res = db.ApplicationViews.Where(x => x.IsActive == true && x.Status_Id == 8).ToList();
        //        string MessageBody = "";
        //        string reminder = "1st";
        //        var todaysDate = DateTime.Today;

        //        foreach (var item in res)
        //        {

        //            if (item.DateDiff <= 7)
        //            {
        //                reminder = "1st";
        //            }
        //            if (item.DateDiff > 7 && item.DateDiff <= 14)
        //            {
        //                reminder = "1st";
        //            }
        //            if (item.DateDiff < 7)
        //            {
        //                reminder = "1st";
        //            }
        //            MessageBody = @"" + reminder + @" Reminder
        //                                                Dear applicant,
        //                                                Your application is pending for process due to 
        //                                                missing documnets against the tracking id 12345 in Section (North)
        //                                                After 3rd reminder the application will be automaticaly disposed.
        //                                                Regards..................";
        //            SMS sms = new SMS()
        //            {
        //                UserId = user.Id,
        //                FKId = 0,
        //                //MobileNumber = "03214677763",
        //                MobileNumber = user.PhoneNumber,
        //                Message = MessageBody
        //            };
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        [AllowAnonymous]
        [HttpGet]
        [Route("TestSms")]
        public bool TestSms()
        {
            try
            {

                string MessageBody = "";
                MessageBody = @"" + @"پنجاب کے معزز شہری !        ہیپا ٹائٹس ایک قابلِ علاج مرض ہے  
پنجاب کے تمام ہسپتالوں میں ہیپا ٹائٹس کے علاج و معالجہ  کی  سہولیات مفت  مہیا کی جا رہی ہیں۔ ہیپا ٹائٹس  بی اور سی خون کے ذریعے پھیلنے والی خطر ناک بیماریا ں ہیں جنہیں  کالا یرقان بھی کہتے ہیں۔  کالا یرقان جگر کے کینسر اور جگر  فیل ہونے کا سبب بن سکتا ہے ۔آپ سے درخواست ہے کہ اپنے  بارے میں جانیئے کہ کہیں آپ ہیپا ٹائٹس بی یا سی میں مبتلا تو نہیں ؟ اس کے لئے محکمہ صحت پنجاب کی جانب سے قائم کردہ قریبی ہیپا ٹائٹس کلینک پر تشریف لائیں اور   مفت ٹیسٹ اور مفت علاج و معالجہ  سے استفادہ حاصل کریں ۔ ڈاکٹر یاسمین راشد ، وزیرِ صحت پنجاب
";
                SMS sms = new SMS()
                {

                    MobileNumber = "03214677763",
                    Message = MessageBody
                };
                Common.SendSMSTelenor(sms);
                //Common.SMS_Send()
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [AllowAnonymous]
        [Route("GetApplicationStatus")]
        [HttpGet]
        public IHttpActionResult GetApplicationStatus()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var statusIds = new List<int>
                {
                    1,2,3,8
                };
                var statuses = db.ApplicationStatus.Where(x => statusIds.Contains(x.Id)).OrderBy(x => x.Name).ToList();
                return Ok(statuses);
            }
        }
        [AllowAnonymous]
        [Route("GetAdhocuser/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetAdhocuser(string cnic)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var user = db.C_User.FirstOrDefault(x => x.UserName.Equals(cnic));
                var applicant = db.AdhocApplicantViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                return Ok(new { user, applicant });
            }
        }
        [AllowAnonymous]
        [Route("GetVp/{designationId}")]
        [HttpGet]
        public IHttpActionResult GetVp(int designationId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (designationId == 8121)
                {

                    var vp = db.VpMastProfileViews.Where(x => x.Desg_Id == 812 && x.HFTypeCode == "011").ToList();
                    return Ok(vp);
                }
                else if (designationId == 8122)
                {
                    var vp = db.VpMastProfileViews.Where(x => x.Desg_Id == 812 && x.HFTypeCode == "012").ToList();
                    return Ok(vp);
                }
                else if (designationId == 8122)
                {
                    var vp = db.VpMastProfileViews.Where(x => x.Desg_Id == 812 && x.HFTypeCode == "068").ToList();
                    return Ok(vp);
                }

                else
                {
                    var vp = db.VpMastProfileViews.Where(x => x.Desg_Id == designationId).ToList();
                    return Ok(vp);
                }

            }
        }

        [AllowAnonymous]
        [Route("GetEmpOnLeaveSum")]
        [HttpGet]
        public IHttpActionResult GetEmpOnLeaveSum()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.EmpOnLeaveSummary().ToList();
                return Ok(res);
            }
        }
        [AllowAnonymous]
        [Route("GetEmpLeaveExpSum")]
        [HttpGet]
        public IHttpActionResult GetEmpLeaveExpSum()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.EmpLeaveExpiredSummary().ToList();
                return Ok(res);
            }
        }
        [AllowAnonymous]
        [Route("AwaitingPostingSum")]
        [HttpGet]
        public IHttpActionResult AwaitingPostingSum()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.AwaitingPostingSummary().ToList();
                return Ok(res);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("GetSummaries")]
        public IHttpActionResult GetSummaries([FromBody] ApplicationFilter filter)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.Created_By.Equals("pshd") && x.IsActive == true).AsQueryable();
                    if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From != null && filter.To == null)
                    {
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    else if (filter.From != null && filter.To != null)
                    {
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    if (filter.From != null)
                    {
                        query = query.Where(x => x.Created_Date >= filter.From).AsQueryable();
                    }
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.Created_Date <= filter.To).AsQueryable();
                    }
                    if (filter.OfficeId != 0)
                    {
                        query = query.Where(x => x.FromOfficer_Id == filter.OfficeId);
                    }
                    if (filter.Status_Id != 0)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                    }
                    if (filter.Type_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationType_Id == filter.Type_Id).AsQueryable();
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            filter.Skip = 0;
                            query = query.Where(x => x.TrackingNumber == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.ForwardTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return Ok(new TableResponse<ApplicationView>() { Count = count, List = list });
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetSummariesTemp")]
        public IHttpActionResult GetSummariesTemp()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.Created_By.Equals("pshd") && x.IsActive == true).AsQueryable();
                    var list = query.OrderByDescending(x => x.ForwardTime).ToList();
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("GetApplication/{Id}")]
        [HttpGet]
        public IHttpActionResult GetApplication(int Id)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                    if (applicationFts.application == null)
                    {
                        return null;
                    }
                    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id && x.IsActive == true).ToList();
                    return Ok(applicationFts);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [AllowAnonymous]
        [Route("GetApplicationLogs/{Id}")]
        [HttpGet]
        public IHttpActionResult GetApplicationLogs(int Id)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.ApplicationLogViews.Where(x => x.Application_Id == Id && x.IsActive == true).AsQueryable();
                    query = query.OrderBy(x => x.DateTime).AsQueryable();
                    return Ok(query.ToList());
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [AllowAnonymous]
        [Route("GetVpByHF/{HFId}")]
        [HttpGet]
        public IHttpActionResult GetVpByHF(int HFId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var vp = db.VpMastProfileViews.Where(x => x.HF_Id == HFId).OrderByDescending(x => x.BPS).ToList();
                return Ok(vp);
            }
        }
        [AllowAnonymous]
        [Route("GetLeavesExpired")]
        [HttpGet]
        public IHttpActionResult GetLeavesExpired()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.LeavesExpireds.ToList();
                return Ok(res);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetPreferences/{cnic}")]
        public IHttpActionResult GetPreferences(string cnic)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.APMOPrefsViews.Where(x => x.CNIC.Equals(cnic)).ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddPreference")]
        public IHttpActionResult AddPreference(APMOPref pref)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                pref.DateTime = DateTime.UtcNow.AddHours(5);
                db.APMOPrefs.Add(pref);
                db.SaveChanges();
                return Ok(pref);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("SubmitApplicationMO/{profileId}/{acceptance}/{mobileNumber}")]
        public IHttpActionResult SubmitApplicationMO(int profileId, int acceptance, string mobileNumber)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var hrA = db.HrAcceptances.FirstOrDefault(x => x.Profile_Id == profileId);
                if (hrA != null)
                {
                    return Ok(hrA);
                }

                HrAcceptance hrAcceptance = new HrAcceptance();
                hrAcceptance.Accepted = acceptance == 1 ? true : false;
                hrAcceptance.Opportunity_Id = 1;
                hrAcceptance.Profile_Id = profileId;
                hrAcceptance.IsActive = true;
                hrAcceptance.Username = User.Identity.GetUserName();
                hrAcceptance.DateTime = DateTime.UtcNow.AddHours(5);
                hrAcceptance.UserId = User.Identity.GetUserId();
                db.HrAcceptances.Add(hrAcceptance);
                db.SaveChanges();
                string userId = User.Identity.GetUserId();
                string MessageBody = @"Your preferences has been received on HRMIS (Preferences Portal)\nYou can change your preferences before portal is closed\nRegards,\nHealth Information and Services Delivery Unit\nPrimary and Secondary Healthcare Department";

                SMS sms2 = new SMS()
                {
                    UserId = userId,
                    FKId = 0,
                    MobileNumber = mobileNumber,
                    Message = MessageBody
                };
                Thread t2 = new Thread(() => Common.SendSMSTelenor(sms2));
                t2.Start();
                return Ok(hrAcceptance);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("GetEmployeesOnLeave")]
        [HttpPost]
        public async Task<IHttpActionResult> GetEmployeesOnLeave([FromBody] FTSFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.EmployeesOnLeaves.AsQueryable();
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.ToDate < filter.To).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filter.SignedBy))
                    {
                        query = query.Where(x => x.SignedBy.Equals(filter.SignedBy)).AsQueryable();
                    }
                    var Count = query.Count();
                    var List = await query.OrderBy(x => x.ToDate).Skip(filter.Skip).Take(filter.PageSize).ToListAsync();
                    return Ok(new TableResponse<EmployeesOnLeave>
                    {
                        Count = Count,
                        List = List
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("GetEmployeesLeaveExpired")]
        [HttpPost]
        public async Task<IHttpActionResult> GetEmployeesLeaveExpired([FromBody] FTSFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.EmployeesLeaveExpireds.AsQueryable();
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.ToDate < filter.To).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filter.SignedBy))
                    {
                        query = query.Where(x => x.SignedBy.Equals(filter.SignedBy)).AsQueryable();
                    }
                    var Count = query.Count();
                    var List = await query.OrderByDescending(x => x.ToDate).Skip(filter.Skip).Take(filter.PageSize).ToListAsync();
                    return Ok(new TableResponse<EmployeesLeaveExpired>
                    {
                        Count = Count,
                        List = List
                    });
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("GetAwaitingPostingApps")]
        [HttpPost]
        public async Task<IHttpActionResult> GetAwaitingPostingApps([FromBody] FTSFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.AwaitingPostingAppViews.AsQueryable();

                    if (filter.OfficerId > 0)
                    {
                        query = query.Where(x => x.ForwardingOfficer_Id == filter.OfficerId).AsQueryable();
                    }

                    var Count = query.Count();
                    var List = await query.OrderByDescending(x => x.Datetime).Skip(filter.Skip).Take(filter.PageSize).ToListAsync();
                    return Ok(new TableResponse<AwaitingPostingAppView>
                    {
                        Count = Count,
                        List = List
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("GetCrrSummary")]
        [HttpPost]
        public IHttpActionResult GetCrrSummary([FromBody] FTSFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.uspCrrPendancy(filter.From, filter.To, null).ToList();
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("VPReport/{hfmisCode}")]
        public IHttpActionResult VPReport(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    string query;
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = @" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like @param group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc";
                    string CodeParam = string.Format("{0}%", hfmisCode);
                    var report = _db.Database.SqlQuery<VacancyViewModel>(query, new SqlParameter("@param", CodeParam)).ToList();
                    return Ok(report);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetApplicationTrack/{dispatchTracking}")]
        public IHttpActionResult GetApplicationTrack(string dispatchTracking)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var application = db.ApplicationViews.FirstOrDefault(x => x.DispatchNumber.Equals(dispatchTracking));
                return Ok(application);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetApplicationMO/{profileId}")]
        public IHttpActionResult GetApplicationMO(int profileId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                return Ok(db.HrAcceptances.FirstOrDefault(x => x.Profile_Id == profileId && x.Accepted == true));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("RemovePreference/{id}")]
        public IHttpActionResult RemovePreference(int id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.APMOPrefs.Find(id);
                db.APMOPrefs.Remove(res);
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [Route("AddMeritProfile")]
        public IHttpActionResult AddMeritProfile(Merit merit)
        {
            try
            {
                if (merit.Id == 0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    Entity_Lifecycle elc = new Entity_Lifecycle();
                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                    elc.Created_By = User.Identity.GetUserName();
                    elc.Users_Id = User.Identity.GetUserId();
                    elc.IsActive = true;
                    elc.Entity_Id = 9090;
                    db.Entity_Lifecycle.Add(elc);
                    db.SaveChanges();
                    merit.EntityLifecylcle_Id = elc.Id;
                    db.Merits.Add(merit);
                    db.SaveChanges();
                }
                else
                {
                    if (merit.EntityLifecylcle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = User.Identity.GetUserName();
                        elc.Users_Id = User.Identity.GetUserId();
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        db.Entity_Lifecycle.Add(elc);
                        db.SaveChanges();
                        merit.EntityLifecylcle_Id = elc.Id;
                    }
                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)merit.EntityLifecylcle_Id;
                    eml.Description = "Profile Updated By " + User.Identity.GetUserName();
                    db.Entity_Modified_Log.Add(eml);
                    db.SaveChanges();
                    db.Entry(merit).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Ok(merit);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [Route("VerifyMeritProfile")]
        public IHttpActionResult VerifyMeritProfile(Merit merit)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var meritDb = db.Merits.FirstOrDefault(x => x.Id == merit.Id);

                meritDb.Name = merit.Name;
                meritDb.FatherName = merit.FatherName;
                meritDb.DOB = merit.DOB;
                meritDb.Domicile_Id = merit.Domicile_Id;
                meritDb.Religion_Id = merit.Religion_Id;
                meritDb.MaritalStatus = merit.MaritalStatus;
                meritDb.MobileNumber = merit.MobileNumber;
                meritDb.MobileSec = merit.MobileSec;
                meritDb.Email = merit.Email;
                meritDb.Address = merit.Address;

                if (meritDb.Status == "New")
                {
                    meritDb.Status = "ProfileBuilt";
                }

                db.Entry(meritDb).State = EntityState.Modified;
                db.SaveChanges();

                Entity_Modified_Log eml = new Entity_Modified_Log();
                eml.Modified_By = User.Identity.GetUserId();
                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                eml.Entity_Lifecycle_Id = (long)merit.EntityLifecylcle_Id;
                eml.Description = "Merit Profile Updated by " + User.Identity.GetUserName();
                db.Entity_Modified_Log.Add(eml);
                db.SaveChanges();
                return Ok(meritDb);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [Route("UploadProfilePhoto/{cnic}")]
        public async Task<IHttpActionResult> FileUpload(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\MeritPhotos\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        filename = cnic + "_23.jpg";

                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                        {
                            throw new Exception(
                                "Unable to Upload. File Size must be less than 5 MB and File Format must be " +
                                string.Join(",", validExtensions));
                        }

                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }



                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetMeritProfile/{cnic}")]
        public IHttpActionResult GetMeritProfile(string cnic)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.MeritsViews.OrderByDescending(x => x.Id).FirstOrDefault(x => x.CNIC.Equals(cnic) && x.IsDisabled != true);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("GetHealthWorkers/{skip}/{pageSize}")]
        public IHttpActionResult GetHealthWorkers(int skip, int pageSize)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = db.CovidStaffViews.OrderBy(x => x.OriginHealthFacility).Skip(skip).Take(pageSize).ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetCachedHFByHfmisCode/{hfmisCode}")]
        public IHttpActionResult GetCachedHFByHfmisCode(string hfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = new HealthFacilityService().GetCachedHFByHfmisCode(hfmisCode);
                return Ok(new { res.Count, List = res });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
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
