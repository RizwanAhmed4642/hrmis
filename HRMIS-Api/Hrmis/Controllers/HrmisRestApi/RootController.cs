using DPUruNet;
using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Common;
using Hrmis.Models.ViewModels.HealthFacility;
using Hrmis.Models.ViewModels.User;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using static Spire.Pdf.General.Render.Decode.Jpeg2000.j2k.codestream.HeaderInfo;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Root")]
    public class RootController : ApiController
    {
        private readonly RootService _rootService;
        public RootController()
        {
            _rootService = new RootService();
        }

        #region Covid

        [Route("SaveCovidFacility")]
        [HttpPost]
        public IHttpActionResult SaveCovidFacility([FromBody] CovidFacility covidFacility)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                if (covidFacility.Id == 0)
                {
                    covidFacility.IsActive = true;
                    covidFacility.DateTime = DateTime.UtcNow.AddHours(5);
                    covidFacility.CreatedBy = User.Identity.GetUserName();
                    covidFacility.UserId = User.Identity.GetUserId();
                    _db.CovidFacilities.Add(covidFacility);
                    _db.SaveChanges();
                }
                return Ok(covidFacility);
            }
        }
        [Route("SaveCovidStaff")]
        [HttpPost]
        public IHttpActionResult SaveCovidStaff([FromBody] CovidStaff covidStaff)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                if (covidStaff.Id == 0)
                {
                    covidStaff.IsActive = true;
                    covidStaff.DateTime = DateTime.UtcNow.AddHours(5);
                    covidStaff.CreatedBy = User.Identity.GetUserName();
                    covidStaff.UserId = User.Identity.GetUserId();
                    _db.CovidStaffs.Add(covidStaff);
                    _db.SaveChanges();
                }
                return Ok(covidStaff);
            }
        }
        [Route("GetCovidFacilityTypes")]
        [HttpGet]
        public IHttpActionResult GetCovidFacilityTypes()
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var types = _db.CovidFacilityTypes.Where(x => x.IsActive == true).ToList();
                return Ok(types);
            }
        }
        [Route("GetCovidFacilities/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetCovidFacilities(string hfmisCode)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var covidFacilities = _db.CovidFacilityViews.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).ToList();
                return Ok(covidFacilities);
            }
        }
        [Route("GetCovidStaff/{covidFacilityId}")]
        [HttpGet]
        public IHttpActionResult GetCovidStaff(int covidFacilityId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                List<CovidStaffView> covidstaffView = new List<CovidStaffView>();
                if (covidFacilityId == 0)
                {
                    covidstaffView = _db.CovidStaffViews.Where(x => x.IsActive == true).ToList();
                }
                else
                {
                    covidstaffView = _db.CovidStaffViews.Where(x => x.CovidFacilityId == covidFacilityId && x.IsActive == true).ToList();
                }
                return Ok(covidstaffView);
            }
        }
        [Route("RemoveCovidFacility/{id}")]
        [HttpGet]
        public IHttpActionResult RemoveCovidFacility(int id)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var covidFacility = _db.CovidFacilities.FirstOrDefault(x => x.Id == id);
                if (covidFacility != null)
                {
                    covidFacility.IsActive = false;
                    _db.Entry(covidFacility).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Ok(true);
                }
                return Ok(false);
            }
        }
        [Route("RemoveCovidStaff/{id}")]
        [HttpGet]
        public IHttpActionResult RemoveCovidStaff(int id)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var covidstaff = _db.CovidStaffs.FirstOrDefault(x => x.Id == id);
                if (covidstaff != null)
                {
                    covidstaff.IsActive = false;
                    _db.Entry(covidstaff).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Ok(true);
                }
                return Ok(false);
            }
        }
        #endregion

        [Route("GetHFCodMap/{hfmisCode}")]
        [HttpGet]
        public async Task<List<HF_Code_Map>> GetHFCodMap(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfCodes = await db.HF_Code_Map.Where(x => x.HrHfCode.StartsWith(hfmisCode)).ToListAsync();
                return hfCodes;
            }
        }
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
        [Route("GetDistrictGeoJson")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDistrictGeoJson()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string s = File.ReadAllText(HostingEnvironment.MapPath(@"~\Content\pakistan_districts.geojson"));
                return Ok(s);
            }
        }

        [HttpPost]
        [Route("GetPunjabOfficers")]
        public IHttpActionResult GetPunjabOfficers([FromBody] SearchQuery search)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.PunjabOfficers.AsQueryable();
                    if (!string.IsNullOrEmpty(search.Query))
                    {
                        data = data.Where(x => x.Name.StartsWith(search.Query));
                    }
                    return Ok(data.OrderBy(x => x.DesignationName).ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveHrProsting")]
        public IHttpActionResult SaveHrProsting([FromBody] HrPostingStatu hrPostingStatu)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;


                    hrPostingStatu.Datetime = DateTime.UtcNow.AddHours(5);
                    hrPostingStatu.Username = User.Identity.GetUserName();
                    hrPostingStatu.UserId = User.Identity.GetUserId();
                    hrPostingStatu.IsActive = true;
                    db.HrPostingStatus.Add(hrPostingStatu);
                    db.SaveChanges();

                    return Ok(hrPostingStatu);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetHrProsting")]
        public IHttpActionResult GetHrProsting([FromBody] Filter filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.HrPostingStatus.ToList();

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveHrProsting/{id}")]
        public IHttpActionResult RemoveHrProsting(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var hrPosting = db.HrPostingStatus.FirstOrDefault(x => x.Id == id);
                    if (hrPosting != null)
                    {
                        hrPosting.IsActive = false;
                        db.SaveChanges();
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #region With Code
        [Route("GetDivisions/{hfmisCode}")]
        [HttpGet]
        public async Task<List<Division>> GetDivisions(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (hfmisCode.Length > 3)
                {
                    hfmisCode = hfmisCode.Substring(0, 3);
                }
                
                if (User.IsInRole("South Punjab"))
                {
                    return await db.Divisions.Where(x => x.Code.Equals("031") || x.Code.Equals("032") || x.Code.Equals("036")).OrderBy(x => x.Name).ToListAsync();
                }
                if (User.IsInRole("PHFMC Admin"))
                {
                    return await db.Divisions.Where(x => x.Code.Equals("031") || x.Code.Equals("032") || x.Code.Equals("033") || x.Code.Equals("34") || x.Code.Equals("035") || x.Code.Equals("036") || x.Code.Equals("037") || x.Code.Equals("038") || x.Code.Equals("039")).OrderBy(x => x.Name).ToListAsync();
                }
                var divisions = await db.Divisions.Where(x => (x.Code.StartsWith(hfmisCode) || x.Code.Equals(hfmisCode)) && !x.Name.StartsWith("Islam")).OrderBy(x => x.Name).ToListAsync();
                return divisions;
            }
        }

        [Route("GetDistricts/{hfmisCode}")]
        [HttpGet]
        public async Task<List<District>> GetDistricts(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (hfmisCode.Length > 6)
                {
                    hfmisCode = hfmisCode.Substring(0, 6);
                }
                string role = User.IsInRole("Secondary") ? "Secondary" :
                 User.IsInRole("Primary") ? "Primary" :
                 User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC" :
                 User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                 User.IsInRole("Administrator") ? "Administrator" :
                 User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                 //Added New role  District Against Chief Executive Officer
                 User.IsInRole("Districts") ? "Districts" :
                 User.IsInRole("Order Generation") ? "Order Generation" :

                 User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";


                //if (role.Equals("PHFMC"))
                //{
                //    var districts = await db.PHFMC_Districts.Where(x => x.DistrictCode.StartsWith(hfmisCode) || x.DistrictCode.Equals(hfmisCode)).Select(k => new District()                {
                //        Id = (int)k.District_Id,
                //        Name = k.DistrictName,
                //        Code = k.DistrictCode,
                //    }).ToListAsync();
                //    return districts;
                //}
                //else
                //{
                if (User.IsInRole("South Punjab"))
                {
                    return await db.Districts.Where(x => Common.southDistricts.Contains(x.Code)).OrderBy(x => x.Name).ToListAsync();
                }
                if (User.IsInRole("PHFMC Admin"))
                {
                    return await db.Districts.Where(x => Common.phfmcDistrictCodes.Contains(x.Code)).OrderBy(x => x.Name).ToListAsync();
                }
                if (hfmisCode.Equals("!Islo"))
                {
                    var dists = await db.Districts.Where(x => !x.Name.Equals("Islamabad") && x.Code.StartsWith("0")).OrderBy(x => x.Name).ToListAsync();
                    return dists;
                }
                var districts = await db.Districts.Where(x => !x.Name.Equals("Islamabad") && (x.Code.StartsWith(hfmisCode) || x.Code.Equals(hfmisCode))).OrderBy(x => x.Name).ToListAsync();
                return districts;

                //}
            }
        }

        [Route("GetDistrictsForDailyWagers/{hfmisCode}")]
        [HttpGet]
        public async Task<List<District>> GetDistrictsForDailyWagers(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (hfmisCode.Length > 6)
                {
                    hfmisCode = hfmisCode.Substring(0, 6);
                }
                var districts = await db.Districts.Where(x => (x.Code.StartsWith(hfmisCode) || x.Code.Equals(hfmisCode))).OrderBy(x => x.Name).ToListAsync();
                return districts;
                //}
            }
        }

        [Route("GetDistrictsLatLong/{hfmisCode}")]
        [HttpGet]
        public async Task<List<DistrictLatLong>> GetDistrictsLatLong(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (hfmisCode.Length > 6)
                {
                    hfmisCode = hfmisCode.Substring(0, 6);
                }
                string role = User.IsInRole("Secondary") ? "Secondary" :
                 User.IsInRole("Primary") ? "Primary" :
                 User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC" :
                 User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                 User.IsInRole("Administrator") ? "Administrator" :
                 User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                 //Added New role  District Against Chief Executive Officer
                 User.IsInRole("Districts") ? "Districts" :
                 User.IsInRole("Order Generation") ? "Order Generation" :

                 User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";


                //if (role.Equals("PHFMC"))
                //{
                //    var districts = await db.PHFMC_Districts.Where(x => x.DistrictCode.StartsWith(hfmisCode) || x.DistrictCode.Equals(hfmisCode)).Select(k => new District()                {
                //        Id = (int)k.District_Id,
                //        Name = k.DistrictName,
                //        Code = k.DistrictCode,
                //    }).ToListAsync();
                //    return districts;
                //}
                //else
                //{
                if (hfmisCode.Equals("!Islo"))
                {
                    var dists = await db.DistrictLatLongs.Where(x => !x.Name.Equals("Islamabad") && x.Code.StartsWith("0")).OrderBy(x => x.Name).ToListAsync();
                    return dists;
                }
                var districts = await db.DistrictLatLongs.Where(x => x.Code.StartsWith(hfmisCode) || x.Code.Equals(hfmisCode)).OrderBy(x => x.Name).ToListAsync();
                return districts;

                //}
            }
        }

        [Route("GetDistrictsVacancy/{DesignationId}")]
        [HttpGet]
        public List<DistrictsVacancyModel> GetDistrictsVacancy(int DesignationId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<int> designationIds = new List<int>();
                if (DesignationId > 0)
                {
                    designationIds.Add(DesignationId);
                    if (DesignationId == 802 || DesignationId == 1320)
                    {
                        designationIds.Add(2404);
                    }
                }
                var districts = db.VpMastProfileViews.Where(x => x.HFAC == 1 && designationIds.Contains(x.Desg_Id) && !x.DistrictName.Equals("Islamabad")).GroupBy(x => new { x.DistrictCode, x.DistrictName }).Select(x => new DistrictsVacancyModel
                {
                    Code = x.Key.DistrictCode,
                    Name = x.Key.DistrictName,
                    Count = x.Sum(l => (int)(l.Vacant + l.Adhoc))
                }).ToList();
                return districts;
            }
        }

        [Route("GetTehsils/{hfmisCode}")]
        [HttpGet]
        public async Task<List<Tehsil>> GetTehsils(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                if (User.IsInRole("South Punjab"))
                {
                    return await db.Tehsils.Where(x => Common.southTehsils.Contains(x.Code)).OrderBy(x => x.Name).ToListAsync();
                }
                if (hfmisCode.Length > 9)
                {
                    hfmisCode = hfmisCode.Substring(0, 9);
                }
                var tehsils = await db.Tehsils.Where(x => x.Code.StartsWith(hfmisCode) || x.Code.Equals(hfmisCode)).OrderBy(x => x.Name).ToListAsync();
                return tehsils;
            }
        }

        [Route("GetVPQuota/{hfmisCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVPQuota(string hfmisCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var vpQuota = db.VPQuota().Where(x => x.DistrictCode.StartsWith(hfmisCode)).OrderBy(x => x.HFTypeCode).ToList();
                var vpQuotaDesignation = db.VPQuotaDesignation().OrderBy(x => x.DesignationName).ToList();
                var vpQuotaDistrict = db.VPQuotaDistrict().Where(x => x.DistrictCode.StartsWith(hfmisCode)).OrderBy(x => x.DistrictName).ToList();
                var vpQuotaDistrictDesignation = db.VPQuotaDistrictDesignation().Where(x => x.DistrictCode.StartsWith(hfmisCode)).OrderBy(x => x.DesignationName).ToList();
                return Ok(new { vpQuota, vpQuotaDesignation, vpQuotaDistrict, vpQuotaDistrictDesignation, });
            }
        }
        [Route("GetVacancyQuota")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyQuota()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var vpQuota = await db.VacancyQuotas.Where(x => x.IsActive == true).ToListAsync();
                return Ok(vpQuota);
            }
        }
        [Route("GetCategories")]
        [HttpGet]
        public async Task<List<HFCategory>> GetCategories()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var categories = await db.HFCategories.ToListAsync();
                return categories;
            }
        }
        [Route("GetServices")]
        [HttpGet]
        public async Task<List<Service>> GetServices()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var services = await db.Services.ToListAsync();
                return services;
            }
        }
        [Route("GetQualificationType")]
        [HttpGet]
        public async Task<List<QualificationType>> GetQualificationType()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var qualificationTypes = await db.QualificationTypes.ToListAsync();
                return qualificationTypes;
            }
        }
        [Route("GetDegrees/{qualificationId}")]
        [HttpGet]
        public async Task<List<Degree>> GetDegrees(int qualificationId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (qualificationId == 0)
                {
                    var degrees = await db.Degrees.Where(x => x.IsActive == true).ToListAsync();
                    return degrees;
                }
                else
                {
                    var degrees = await db.Degrees.Where(x => x.QualificationTypeId == qualificationId && x.IsActive == true).ToListAsync();
                    return degrees;
                }
            }
        }
        [Route("RemoveHFService/{HFId}/{serivceId}")]
        [HttpGet]
        public IHttpActionResult RemoveHFService(int HFId, int serivceId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var hfService = db.HFServices.FirstOrDefault(x => x.HF_Id == HFId && x.Services_Id == serivceId);
                    db.HFServices.Remove(hfService);
                    db.SaveChanges();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetWards")]
        [HttpGet]
        public async Task<IHttpActionResult> GetWards()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var wards = await db.Wards.Where(x => x.IsActive == true).ToListAsync();
                return Ok(wards);
            }
        }
        [Route("GetWardsCustom/{HF_Id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetWardsCustom(int HF_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfWardsBeds = await db.HFWardBedsViews.Where(x => x.HF_Id == HF_Id && x.IsActive == true).OrderByDescending(k => k.Total).ThenBy(l => l.Name).ToListAsync();
                var hfWardsBedsIds = hfWardsBeds.Select(k => k.Ward_Id).ToList();
                var wards = await db.Wards.Where(x => x.IsActive == true && !hfWardsBedsIds.Contains(x.Id)).OrderBy(k => k.Name).ToListAsync();
                return Ok(new { wards, hfWardsBeds });
            }
        }
        [Route("GetPunjabOfficers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPunjabOfficers()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var punjabOfficers = await db.PunjabOfficers.OrderBy(x => x.DesignationName).ToListAsync();
                return Ok(punjabOfficers);
            }
        }
        [Route("GetCategory/{id}")]
        [HttpGet]
        public async Task<HFCategory> GetCategory(int id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var category = await db.HFCategories.FirstOrDefaultAsync(x => x.Id == id);
                return category;
            }
        }

        


        [Route("GetTypes")]
        [HttpGet]
        public async Task<List<HFType>> GetTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (User.IsInRole("Primary"))
                {

                    return await db.HFTypes.Where(x => x.HFCat_Id == 3).OrderBy(x => x.OrderBy).ThenBy(j => j.Name).ToListAsync();
                }
                if (User.IsInRole("Secondary"))
                {
                    var userHfTypes = new List<string>() { "011", "012" ,"068"};
                    return await db.HFTypes.Where(x => userHfTypes.Contains(x.Code)).OrderBy(x => x.OrderBy).ThenBy(j => j.Name).ToListAsync();
                }
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {

                    return await db.HFTypes.Where(x =>x.Id  != 61).OrderBy(x => x.OrderBy).ThenBy(j => j.Name).ToListAsync();
                    
                }
                if (User.IsInRole("Health Facility"))
                {
                    var _userService = new UserService();
                    string hfCode = _userService.GetUser(User.Identity.GetUserId()).hfmiscode;

                    return await db.HFTypes.Where(x => x.Code.Equals(hfCode.Substring(12, 3))).OrderBy(x => x.OrderBy).ThenBy(j => j.Name).ToListAsync();
                }
                var types = await db.HFTypes.OrderBy(x => x.OrderBy).ThenBy(j => j.Name).ToListAsync();
                return types;
            }
        }
        [Route("GetCadres")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCadres()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var cadres = await db.Cadres.ToListAsync();
                return Ok(cadres);
            }
        }
        [Route("GetHFAC")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHFAC()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {

                    var hfacss= await db.HFACs.Where(x => x.Id.Equals("2")).ToListAsync();
                    return Ok(hfacss);
                    //return await db.HFACs.Where(x => x.Id.Equals("2")).ToListAsync();
                    ////return await db.HFACs.Where(x => x.Code.Equals("2")).OrderBy(x => x.Name).ToListAsync();
                }
                    var hfacs = await db.HFACs.ToListAsync();
                return Ok(hfacs);
            }
        }
        [Route("GetApplicationTypes")]
        [HttpGet]
        public async Task<List<ApplicationType>> GetApplicationTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.ApplicationTypes.OrderBy(x => x.Name).ToListAsync();
                return types;
            }
        }
        //[Route("GetApplicationTypes")]
        //[HttpPost]
        //public async Task<IHttpActionResult> GetApplicationTypes([FromBody] SMS sms)
        //{
        //    sms.UserId = User.Identity.GetUserId();
        //    return Ok(await Common.SendSMSTelenor(sms));
        //}

        [Route("GetApplicationTypesActive")]
        [HttpGet]
        public async Task<List<ApplicationType>> GetApplicationTypesActive()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.ApplicationTypes.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToListAsync();
                return types;
            }
        }
        [Route("GetApplicationStatus")]
        [HttpGet]
        public async Task<List<ApplicationStatu>> GetApplicationStatus()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var statuses = await db.ApplicationStatus.OrderBy(x => x.Name).ToListAsync();
                return statuses;
            }
        }

        [Route("GetApplicationMutualCode/{FirstProfile_Id}/{SecondProfile_Id}")]
        [HttpGet]
        public IHttpActionResult GetApplicationMutualCode(int FirstProfile_Id, int SecondProfile_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (FirstProfile_Id > 0 && SecondProfile_Id > 0)
                {
                    var random = new Random();
                    var firstProfile = db.HrProfiles.FirstOrDefault(x => x.Id == FirstProfile_Id);
                    var secondProfile = db.HrProfiles.FirstOrDefault(x => x.Id == SecondProfile_Id);
                    ApplicationMutualCode applicationMutualCode = db.ApplicationMutualCodes.FirstOrDefault(x => x.FirstProfile_Id == FirstProfile_Id && x.SecondProfile_Id == SecondProfile_Id);
                    if (applicationMutualCode == null)
                    {
                        if (firstProfile != null && secondProfile != null)
                        {
                            if (firstProfile.MobileNo != null && secondProfile.MobileNo != null)
                            {

                                applicationMutualCode = new ApplicationMutualCode();
                                applicationMutualCode.FirstProfile_Id = FirstProfile_Id;
                                applicationMutualCode.FirstCode = random.Next(1000, 4999);
                                applicationMutualCode.FirstCodeTime = DateTime.UtcNow.AddHours(5);

                                applicationMutualCode.SecondProfile_Id = SecondProfile_Id;
                                applicationMutualCode.SecondCode = random.Next(5000, 9999);
                                applicationMutualCode.SecondCodeTime = DateTime.UtcNow.AddHours(5);

                                db.ApplicationMutualCodes.Add(applicationMutualCode);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        applicationMutualCode.FirstProfile_Id = FirstProfile_Id;
                        applicationMutualCode.FirstCode = random.Next(1000, 4999);
                        applicationMutualCode.FirstCodeTime = DateTime.UtcNow.AddHours(5);

                        applicationMutualCode.SecondProfile_Id = SecondProfile_Id;
                        applicationMutualCode.SecondCode = random.Next(5000, 9999);
                        applicationMutualCode.SecondCodeTime = DateTime.UtcNow.AddHours(5);
                        applicationMutualCode.Verified = null;
                        applicationMutualCode.VerifyTime = null;
                        applicationMutualCode.SecondVerified = null;
                        applicationMutualCode.SecondVerifyTime = null;
                        db.Entry(applicationMutualCode).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (applicationMutualCode.Id > 0)
                    {
                        SMS sms = new SMS()
                        {
                            UserId = User.Identity.GetUserId(),
                            FKId = firstProfile.Id,
                            MobileNumber = firstProfile.MobileNo,
                            Message = "Verification Code for mututal transfer: " + applicationMutualCode.FirstCode.ToString() + "\nCNIC: " + firstProfile.CNIC
                        };
                        Thread t = new Thread(() => Common.SendSMSTelenor(sms));
                        t.Start();
                        SMS sms2 = new SMS()
                        {
                            UserId = User.Identity.GetUserId(),
                            FKId = firstProfile.Id,
                            MobileNumber = secondProfile.MobileNo,
                            Message = "Verification Code for mututal transfer: " + applicationMutualCode.SecondCode.ToString() + "\nCNIC: " + secondProfile.CNIC
                        };
                        Thread t2 = new Thread(() => Common.SendSMSTelenor(sms2));
                        t2.Start();
                        return Ok(applicationMutualCode);
                    }
                }
                return Ok(false);
            }
        }

        [Route("CheckApplicationMutualCode/{cnic}")]
        [HttpGet]
        public IHttpActionResult CheckApplicationMutualCode(string cnic)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (!string.IsNullOrEmpty(cnic))
                {
                    var random = new Random();
                    var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (profile != null)
                    {
                        ApplicationMutualCode applicationMutualCode = db.ApplicationMutualCodes.FirstOrDefault(x => x.SecondProfile_Id == profile.Id);
                        return Ok(applicationMutualCode);
                    }
                }
                return Ok(false);
            }
        }
        [Route("VerifyMutualCode/{FirstProfile_Id}/{SecondProfile_Id}/{MutualCodeOne}/{MutualCodeTwo}")]
        [HttpGet]
        public IHttpActionResult VerifyMutualCode(int FirstProfile_Id, int SecondProfile_Id, int MutualCodeOne, int MutualCodeTwo)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (FirstProfile_Id > 0 && SecondProfile_Id > 0 && MutualCodeOne > 0 && MutualCodeTwo == 0)
                {
                    ApplicationMutualCode applicationMutualCode = db.ApplicationMutualCodes.FirstOrDefault(x => x.FirstProfile_Id == FirstProfile_Id && x.SecondProfile_Id == SecondProfile_Id);
                    if (applicationMutualCode != null)
                    {
                        if (applicationMutualCode.FirstCode == MutualCodeOne)
                        {
                            applicationMutualCode.Verified = true;
                            applicationMutualCode.VerifyTime = DateTime.UtcNow.AddHours(5);
                            db.Entry(applicationMutualCode).State = EntityState.Modified;
                            db.SaveChanges();
                            return Ok(true);
                        }
                    }
                }
                else if (FirstProfile_Id > 0 && SecondProfile_Id > 0 && MutualCodeOne == 0 && MutualCodeTwo > 0)
                {
                    ApplicationMutualCode applicationMutualCode = db.ApplicationMutualCodes.FirstOrDefault(x => x.FirstProfile_Id == FirstProfile_Id && x.SecondProfile_Id == SecondProfile_Id);
                    if (applicationMutualCode != null)
                    {
                        if (applicationMutualCode.SecondCode == MutualCodeTwo)
                        {
                            applicationMutualCode.SecondVerified = true;
                            applicationMutualCode.SecondVerifyTime = DateTime.UtcNow.AddHours(5);
                            db.Entry(applicationMutualCode).State = EntityState.Modified;
                            db.SaveChanges();
                            return Ok(true);
                        }
                    }
                }
                return Ok(false);
            }
        }
        [Route("VerifySecondMutualCode/{mutualId}/{secondCode}")]
        [HttpGet]
        public IHttpActionResult VerifySecondMutualCode(int mutualId, int secondCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                ApplicationMutualCode applicationMutualCode = db.ApplicationMutualCodes.FirstOrDefault(x => x.Id == mutualId);
                if (applicationMutualCode != null)
                {
                    if (applicationMutualCode.SecondCode == secondCode)
                    {
                        applicationMutualCode.SecondVerified = true;
                        applicationMutualCode.SecondVerifyTime = DateTime.UtcNow.AddHours(5);
                        db.Entry(applicationMutualCode).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(applicationMutualCode);
                    }
                }
                return Ok(false);
            }
        }
        [Route("GetDepartmentsHealth")]
        [HttpGet]
        public async Task<List<HrDepartment>> GetDepartmentsHealth()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<int> deptIds = new List<int> { 25, 28 };
                var departments = await db.HrDepartments.Where(x => deptIds.Contains(x.Id)).OrderBy(x => x.Name).ToListAsync();
                return departments;
            }
        }
        [Route("GetOrderTypes")]
        [HttpGet]
        public async Task<List<TransferType>> GetOrderTypes()
        {
            
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.TransferTypes.Where(x => x.Id != 9).OrderBy(x => x.Name).ToListAsync();
                return types;
            }
        }
        [Route("GetLeaveTypes")]
        [HttpGet]
        public async Task<List<LeaveType>> GetLeaveTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.LeaveTypes.OrderBy(x => x.LeaveType1).ToListAsync();
                return types;
            }
        }
        [Route("GetEmploymentModes")]
        [HttpGet]
        public async Task<List<HrEmpMode>> GetEmploymentModes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var empModes = await db.HrEmpModes.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToListAsync();
                return empModes;
            }
        }
        [Route("GetDisposalOf")]
        [HttpGet]
        public async Task<List<DisposalOf>> GetDisposalOf()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var disposalOfs = await db.DisposalOfs.OrderBy(x => x.Name).ToListAsync();
                return disposalOfs;
            }
        }
        [Route("GetPostTypes")]
        [HttpGet]
        public async Task<List<HrPost_Type>> GetPostTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var post_Type = await db.HrPost_Type.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToListAsync();
                return post_Type;
            }
        }
        [Route("GetFileRequestStatuses")]
        [HttpGet]
        public async Task<List<FileRequestStatu>> GetFileRequestStatuses()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var fileReqStatus = await db.FileRequestStatus.OrderBy(x => x.ReuestStatus).ToListAsync();
                return fileReqStatus;
            }
        }
        [Route("GetApplicationSources")]
        [HttpGet]
        public async Task<IHttpActionResult> GetApplicationSources()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = await db.ApplicationSources.OrderBy(x => x.Name).ToListAsync();
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfileStatuses")]
        [HttpGet]
        public async Task<List<HrProfileStatu>> GetProfileStatuses()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var profileStatus = await db.HrProfileStatus.OrderBy(x => x.Name).ToListAsync();
                return profileStatus;
            }
        }
        [Route("GetHealthFacilities/{hfmisCode}/{deptId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilities(string hfmisCode, int? deptId)
        {
            using (var db = new HR_System())
            {
                if (deptId == null || deptId == 0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var hfs = await db.HFLists.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                    return Ok(hfs);
                }
                else
                {
                    int hfAc = deptId == 28 ? 4 : 0;
                    if (hfAc == 0)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var hfs = await db.HFLists.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                        return Ok(hfs);
                    }
                    else
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var hfs = await db.HFLists.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.HFAC == hfAc && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                        return Ok(hfs);
                    }

                }
            }
        }
        [Route("GetHealthFacilitiesByType/{hfmisCode}/{deptId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilitiesByType(string hfmisCode, int? deptId)
        {
            using (var db = new HR_System())
            {
                var hfTypeCodes = new List<string>() { "011",
"012",
"013",
"017",
"021",
"023",
"024",
"025",
"029",
"036",
"039",
"068",
"043" };
                if (deptId == null || deptId == 0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var hfs = await db.HFLists.Where(x => hfTypeCodes.Contains(x.HFTypeCode) && x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                    return Ok(hfs);
                }
                else
                {
                    int hfAc = deptId == 28 ? 4 : 1;
                    db.Configuration.ProxyCreationEnabled = false;
                    if (hfAc == 4)
                    {
                        var hfs = await db.HFLists.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.HFAC == hfAc && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                        return Ok(hfs);
                    }
                    else
                    {
                        var hfs = await db.HFLists.Where(x => hfTypeCodes.Contains(x.HFTypeCode) && x.HFMISCode.StartsWith(hfmisCode) && x.HFAC == hfAc && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                        return Ok(hfs);
                    }
                }
            }
        }
        [AllowAnonymous]
        [Route("GetHealthFacilitiesAll")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilitiesAll()
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                var hfs = db.HFListPs.Where(x => x.HFTypeCode != "014" && x.HFTypeCode != "015" && x.IsActive == true).ToList();
                return Ok(hfs);
            }
        }

        [Route("GetHealthFacilitiesByTypeName")]
        [HttpPost]
        public async Task<IHttpActionResult> GetHealthFacilitiesByTypeName([FromBody] List<string> TypeNames)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfs = await db.HFLists.Where(x => TypeNames.Contains(x.HFTypeName) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                return Ok(hfs);
            }
        }
        [Route("GetProfileAttachmentTypes")]
        [HttpGet]
        public async Task<IHttpActionResult> GetProfileAttachmentTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var res = await db.ProfileAttachmentTypes.ToListAsync();
                return Ok(res);
            }
        }
        [Route("GetHealthFacilitiesByTypeCode")]
        [HttpPost]
        public async Task<IHttpActionResult> GetHealthFacilitiesByTypeCode([FromBody] List<string> TypeCodes)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfs = await db.HFLists.Where(x => TypeCodes.Contains(x.HFTypeCode) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                return Ok(hfs);
            }
        }
        [Route("GetHealthFacilitiesAtDisposal")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilitiesAtDisposal()
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var hfIds = new List<int>() {
                        //Primary and secondary Healthcare Department
                        11606,
                        //Specialize Healthcare Department
                        11606,
                          //Chief Drug Controller
                        14681,
                    };
                    var hfs = await db.HFLists.Where(x => (
                    hfIds.Contains(x.Id) ||
                    x.HFTypeCode.Equals("049")
                    ) && x.IsActive == true).OrderBy(x => x.FullName).Select(k => new HFDrpopdownViewModel() { Id = k.Id, Name = k.FullName, HfmisCode = k.HFMISCode }).ToListAsync();
                    return Ok(hfs);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetPandSOfficers/{type}")]
        [HttpGet]
        public async Task<List<P_SOfficers>> GetPandSOfficers(string type)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string userId = User.Identity.GetUserId();
                var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));

                if (type.Equals("section"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Code != null && x.Code.Value.ToString().Length == 5 && !x.DesignationName.Equals("Front Desk Officer")).OrderBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("fts"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Code != null && x.Code.Value.ToString().Length <= 5 && !x.DesignationName.Equals("Front Desk Officer") && !x.Program.Equals("Public") && !x.Program.Equals("South") && !x.Program.Equals("Punjab")).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("asds"))
                {
                    var data = await db.P_SOfficers.Where(x => (x.Code != null && x.Code.Value.ToString().Length <= 3) || x.Program.Equals("Punjab")).OrderBy(k => k.DesignationName).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("punjab"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Program.Equals("Punjab")).OrderBy(x => x.OrderBy == null).ThenBy(x => x.OrderBy).ToListAsync();
                    return data;
                }
                else if (type.Equals("south"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Program.Equals("South") && x.Code.Value > 0).OrderBy(x => x.OrderBy == null).ThenBy(x => x.OrderBy).ToListAsync();
                    return data;
                }
                else if (type.Equals("district"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Program.Equals("District")).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("dg health"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Id == 69 || x.Id == 373 || x.Id == 374).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("ceo"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Id == 79).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("phfmc"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Program.Equals("PHFMC")).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("hisduInbox"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Id == 50 || x.Id == 70 || x.Id == 75 || x.Id == 84 || x.Id == 143).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("all"))
                {
                    var data = await db.P_SOfficers.Where(x => !x.DesignationName.Equals("Front Desk Officer")).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("admn"))
                {
                    var data = await db.P_SOfficers.Where(x => x.Program.Equals("Admin") && x.Code != null && x.Code.Value.ToString().Length <= 5).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("allCrr"))
                {
                    var data = await db.P_SOfficers.Where(x => !x.DesignationName.Equals("Front Desk Officer")).OrderByDescending(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.Name).ToListAsync();
                    return data;
                }
                else if (type.Equals("ds.as"))
                {
                    if (currentOfficer == null) return null;
                    var concernedOfficerIds = db.P_SConcernedOfficers.Where(x => x.Officer_Id == currentOfficer.Id).Select(k => k.ConcernedOfficer_Id).ToList();
                    var data = await db.P_SOfficers.Where(x => concernedOfficerIds.Contains(x.Id)).OrderBy(k => k.OrderBy == null).ThenBy(x => x.OrderBy).ThenBy(x => x.DesignationName).ToListAsync();
                    return data;
                }
                else if (type.Equals("concerned"))
                {
                    //string userId = User.Identity.GetUserId();
                    //var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    //if (currentOfficer == null) return null;
                    //var concernedOfficerIds = db.P_SConcernedOfficers.Where(x => x.Officer_Id == currentOfficer.Id).Select(k => k.ConcernedOfficer_Id).ToList();
                    //string officerCodeComparable = Convert.ToString(currentOfficer.Code.Value);
                    int? posId = 0;
                    if (currentOfficer != null) posId = currentOfficer.Id;
                    var programs = new List<string>
                    {
                        "DEPT",
                        "PHFMC",
                        "PMU",
                        "Budget",
                        "DG",
                        "Law Wing",
                        "Law",
                        "District",
                        "DG",
                        "Drug Control",
                        "PSH",
                        "Finance",
                        "Public",
                        "Parliamentarian Lounge"
                    };
                    var data = await db.P_SOfficers.Where(x => x.Code != null && x.Code.ToString().Length <= 5 && x.Id != posId && !programs.Contains(x.Program)).OrderBy(x => x.DesignationName).ToListAsync();
                    //var data = await db.P_SOfficers.Where(x => concernedOfficerIds.Contains(x.Id)).OrderBy(x => x.OrderBy).ToListAsync();
                    return data;
                }
                else
                {
                    var data = await db.P_SOfficers.Where(x => x.Code != null && x.DesignationName != null).ToListAsync();
                    return data;
                }

            }
        }
        [Route("GetAllPSOfficers")]
        [HttpPost]
        public IHttpActionResult GetAllPSOfficers([FromBody] PandSOfficerFilters filters)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.PandSOfficerViews.AsQueryable();

                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        query = query.Where(x => x.Name.Contains(filters.Query)
                        || x.DesignationName.Contains(filters.Query)
                        || x.CNIC.Contains(filters.Query)
                        || x.Contact.Contains(filters.Query)
                        ).AsQueryable();
                    }
                    if (filters.Designation_Id != 0)
                    {
                        query = query.Where(x => x.Designation_Id == filters.Designation_Id).AsQueryable();
                    }
                    var data = query.ToList();
                    return Ok(new { data });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetOfficerData/{user_Id}/{officerId}")]
        [HttpGet]
        public IHttpActionResult GetOfficerData(string user_Id, int officerId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.PandSOfficerViews.AsQueryable();
                    if (!string.IsNullOrEmpty(user_Id) && officerId != 0)
                    {
                        var concernedOfficerIds = _db.P_SConcernedOfficers.Where(x => x.Officer_Id == officerId).Select(k => k.ConcernedOfficer_Id).ToList();
                        var concernedOfficers = _db.P_SOfficers.Where(x => concernedOfficerIds.Contains(x.Id)).ToList();

                        var concernedDesignationIds = _db.P_SConcernedDesignations.Where(x => x.Officer_Id == officerId).Select(k => k.Designation_Id).ToList();
                        var concernedDesignations = _db.HrDesignations.Where(x => concernedDesignationIds.Contains(x.Id) && x.IsActive == true).ToList();

                        var concernedCadreIds = _db.P_SConcernedCadres.Where(x => x.Officer_Id == officerId).Select(k => k.Cadre_Id).ToList();
                        var concernedCadres = _db.CadreViews.Where(x => concernedCadreIds.Contains(x.id)).ToList();

                        var officer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(user_Id) && x.Id == officerId);
                        var fingerPrints = _db.FingerPrints.FirstOrDefault(x => x.PandSOfficer_Id == officer.Id);
                        return Ok(new { officer, concernedOfficers, concernedDesignations, concernedCadres, fingerPrints });
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SaveOfficerData")]
        [HttpPost]
        public IHttpActionResult SaveOfficerData([FromBody] PandSOfficerFilters objectDto)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(objectDto.User_Id) && objectDto.OfficerId != 0)
                    {
                        var officer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(objectDto.User_Id) && x.Id == objectDto.OfficerId);
                        if (officer != null)
                        {
                            //Concerned Officer
                            if (objectDto.tableType == 1)
                            {
                                if (objectDto.add)
                                {
                                    //Add
                                    var newConcernedOfficer = new P_SConcernedOfficers();
                                    newConcernedOfficer.Officer_Id = officer.Id;
                                    newConcernedOfficer.ConcernedOfficer_Id = objectDto.concernedId;
                                    _db.P_SConcernedOfficers.Add(newConcernedOfficer);
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //Remove
                                    var concernedOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.Officer_Id == objectDto.OfficerId && x.ConcernedOfficer_Id == objectDto.concernedId);
                                    if (concernedOfficer != null)
                                    {
                                        _db.P_SConcernedOfficers.Remove(concernedOfficer);
                                        _db.SaveChanges();
                                    }
                                }
                            }
                            //Concerned Designation
                            if (objectDto.tableType == 2)
                            {
                                if (objectDto.add)
                                {
                                    //Add
                                    var newConcernedDesignation = new P_SConcernedDesignations();
                                    newConcernedDesignation.Officer_Id = officer.Id;
                                    newConcernedDesignation.Designation_Id = objectDto.concernedId;
                                    _db.P_SConcernedDesignations.Add(newConcernedDesignation);
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //Remove
                                    var concernedDesignation = _db.P_SConcernedDesignations.FirstOrDefault(x => x.Officer_Id == objectDto.OfficerId && x.Designation_Id == objectDto.concernedId);
                                    if (concernedDesignation != null)
                                    {
                                        _db.P_SConcernedDesignations.Remove(concernedDesignation);
                                        _db.SaveChanges();
                                    }
                                }
                            }
                            //Concerned Cadre
                            if (objectDto.tableType == 3)
                            {
                                if (objectDto.add)
                                {
                                    //Add
                                    var newConcernedCadre = new P_SConcernedCadres();
                                    newConcernedCadre.Officer_Id = officer.Id;
                                    newConcernedCadre.Cadre_Id = objectDto.concernedId;
                                    _db.P_SConcernedCadres.Add(newConcernedCadre);
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    //Remove
                                    var concernedCadre = _db.P_SConcernedCadres.FirstOrDefault(x => x.Officer_Id == objectDto.OfficerId && x.Cadre_Id == objectDto.concernedId);
                                    if (concernedCadre != null)
                                    {
                                        _db.P_SConcernedCadres.Remove(concernedCadre);
                                        _db.SaveChanges();
                                    }
                                }

                            }
                            //Finger Prints
                            if (objectDto.tableType == 4)
                            {
                                bool fpExist = true;
                                FingerPrint fp = _db.FingerPrints.FirstOrDefault(x => x.PandSOfficer_Id == objectDto.OfficerId);
                                if (fp == null)
                                {
                                    fpExist = false;
                                    fp = new FingerPrint();
                                }
                                if (objectDto.fpNumber == 1) fp.FP1 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(objectDto.fprint.metaData));
                                if (objectDto.fpNumber == 2) fp.FP2 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(objectDto.fprint.metaData));
                                if (objectDto.fpNumber == 3) fp.FP3 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(objectDto.fprint.metaData));
                                if (objectDto.fpNumber == 4) fp.FP4 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(objectDto.fprint.metaData));
                                if (objectDto.fpNumber == 5) fp.FP5 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(objectDto.fprint.metaData));

                                fp.PandSOfficer_Id = objectDto.OfficerId;
                                fp.User_Id = objectDto.User_Id;
                                fp.DateTime = DateTime.UtcNow.AddHours(5);

                                if (fpExist) _db.Entry(fp).State = EntityState.Modified;
                                else _db.FingerPrints.Add(fp);
                                _db.SaveChanges();
                                officer.FingerPrint_Id = fp.Id;
                                _db.Entry(officer).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                            return Ok(true);
                        }
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetPSOfficersFingers/{officerId}")]
        [HttpGet]
        public IHttpActionResult GetPSOfficersFingers(int officerId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var res = _db.FingerPrints.FirstOrDefault(x => x.PandSOfficer_Id == officerId);
                    if (res != null)
                    {
                        var fmd1 = new FingerprintSdk().Getbase64FromFMD(res.FP1);
                        var fmd2 = new FingerprintSdk().Getbase64FromFMD(res.FP2);
                        var fmd3 = new FingerprintSdk().Getbase64FromFMD(res.FP3);
                        var fmd4 = new FingerprintSdk().Getbase64FromFMD(res.FP4);
                        var fmd5 = new FingerprintSdk().Getbase64FromFMD(res.FP5);
                        var list = new List<string>() { fmd1, fmd2, fmd3, fmd4, fmd5 };
                        return Ok(list);
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                    throw;
                }
            }
        }
        [Route("SavePSOfficer")]
        [HttpPost]
        public IHttpActionResult SavePSOfficer([FromBody] P_SOfficers officer)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (officer.Id == 0)
                    {
                        var newOfficer = new P_SOfficers();
                        newOfficer.Name = officer.Name;
                        newOfficer.CNIC = officer.CNIC;
                        newOfficer.Contact = officer.Contact;
                        newOfficer.DesignationName = officer.DesignationName;
                        newOfficer.Program = officer.Program;
                        newOfficer.Designation_Id = officer.Designation_Id;

                        if (newOfficer.Designation_Id == 1042) newOfficer.OrderBy = 1; //Secretary
                        else if (newOfficer.Designation_Id == 1341) newOfficer.OrderBy = 2; //Additional Secretary
                        else if (newOfficer.Designation_Id == 1632) newOfficer.OrderBy = 4; //Deputy Secretary
                        else if (newOfficer.Designation_Id == 1952) newOfficer.OrderBy = 5; //Section Officer
                        else newOfficer.OrderBy = 99;
                        newOfficer.User_Id = officer.User_Id;

                        _db.P_SOfficers.Add(newOfficer);
                        _db.SaveChanges();
                    }
                    else
                    {

                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                    throw;
                }
            }
        }
        [Route("GetApplicationDocuments/{applicationTypeId}")]
        [HttpGet]
        public async Task<List<ApplicationDocument>> GetApplicationDocuments(int applicationTypeId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<int> docIds = db.AppTypeDocs.Where(x => x.ApplicationType_Id == applicationTypeId).Select(k => k.Document_Id).ToList();
                if (docIds != null && docIds.Count == 0)
                {
                    docIds = new List<int> { 81, 84 };
                }
                var data = await db.ApplicationDocuments.Where(x => docIds.Contains(x.Id) && x.IsActive == true).OrderBy(x => x.OrderBy).ThenBy(x => x.Name).ToListAsync();
                return data;
            }
        }
        [Route("GetApplicationDocs")]
        [HttpGet]
        public IHttpActionResult GetApplicationDocs()
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.ApplicationDocuments.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList();
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                    throw;
                }
            }
        }
        [Route("GetOrderDocuments/{typeId}")]
        [HttpGet]
        public async Task<List<ApplicationDocument>> GetOrderDocuments(int typeId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<int> docIds = null;
                docIds = new List<int> { 16 };
                var data = await db.ApplicationDocuments.Where(x => docIds.Contains(x.Id) && x.IsActive == true).OrderBy(x => x.OrderBy).ThenBy(x => x.Name).ToListAsync();
                return data;
            }
        }
        [Route("GetInquiryStatus")]
        [HttpGet]
        public async Task<List<InquiryStatu>> GetInquiryStatus()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = await db.InquiryStatus.OrderBy(x => x.Name).ToListAsync();
                return data;
            }
        }
        [Route("GetAppTypePendancy/{typeId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAppTypePendancy(int typeId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = await db.AppTypePendancies.FirstOrDefaultAsync(x => x.ApplicationType_Id == typeId);
                return Ok(data);
            }
        }
        [Route("GetPenaltyType")]
        [HttpGet]
        public async Task<List<PenaltyType>> GetPenaltyType()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = await db.PenaltyTypes.OrderBy(x => x.Name).ToListAsync();
                return data;
            }
        }
        #endregion

        #region Dashboard
        //[Route("GetDashboardInfo")]
        //[HttpGet]
        //public IHttpActionResult GetDashboardInfo()
        //{
        //    try
        //    {
        //        Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        #endregion

        [HttpPost]
        [Route("GetPetitioners")]
        public IHttpActionResult GetPetitioners([FromBody] SearchQuery search)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.LawPetitioners.AsQueryable();
                    if (!string.IsNullOrEmpty(search.Query))
                    {
                        data = data.Where(x => x.Name.StartsWith(search.Query) && x.IsEnable == true);
                    }
                    return Ok(data.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetRepresentatives")]
        public IHttpActionResult GetRepresentatives([FromBody] SearchQuery search)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.LawRepresentatives.AsQueryable();
                    if (!string.IsNullOrEmpty(search.Query))
                    {
                        data = data.Where(x => x.Name.StartsWith(search.Query) && x.IsEnable == true);
                    }
                    return Ok(data.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetJudges")]
        public IHttpActionResult GetJudges([FromBody] SearchQuery search)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.LawJudges.AsQueryable();
                    if (!string.IsNullOrEmpty(search.Query))
                    {
                        data = data.Where(x => x.Name.StartsWith(search.Query) && x.IsEnable == true);
                    }
                    return Ok(data.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFUCs/{hfmisCode}")]
        public IHttpActionResult GetHFUCs(string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var data = db.HFUCs.AsQueryable();
                    var district = db.Districts.FirstOrDefault(x => x.Code.Equals(hfmisCode));
                    if (district != null)
                    {
                        var list = data.Where(x => x.District.ToLower().Equals(district.Name.ToLower())).OrderBy(x => x.UC).ToList();
                        return Ok(list);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetHFUCsForDailyWages/{hfmisCode}")]
        public IHttpActionResult GetHFUCsForDailyWages(string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                   
                        var tehsil = db.HFLists.Where(x => x.UcName != null && x.TehsilCode.Equals(hfmisCode))
                       .Select(x => new
                       {
                           UcCode = x.Id,
                           Name = x.UcName
                       }
                       )
                       .ToList();
                        if (tehsil != null)
                        {
                            return Ok(tehsil);
                        }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllHFUCsForDailyWages")]
        public IHttpActionResult GetAllHFUCsForDailyWages()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                   
                        var tehsil = db.HFLists.Where(x => x.UcName != null)
                      .Select(x => new
                      {
                          UcCode = x.Id,
                          Name = x.UcName,
                          TehsilCode = x.TehsilCode
                      }
                      )
                      .ToList();
                        if (tehsil != null)
                        {
                            return Ok(tehsil);
                        }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBankForDailyWages")]
        public IHttpActionResult GetBankForDailyWages()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var tehsil = db.hc_hfbankdetails.DistinctBy(x=>x.Id)
                        .Select(x => new
                        {
                            Id = x.bankId,
                            Name = x.bank_name   
                        }
                        )
                        .ToList();
                    if (tehsil != null)
                    {
                        return Ok(tehsil);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("SearchTracking/{Id}/{Tracking}")]
        public IHttpActionResult SearchTracking(int Id, int Tracking)
        {
            try
            {

                ApplicationService _applicationService = new ApplicationService();
                var app = _applicationService.GetApplication(Id, Tracking, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (app != null && app.application != null)
                {
                    using (var db = new HR_System())
                    {
                        ApplicationTracking applicationTracking = new ApplicationTracking();
                        applicationTracking.Application_Id = app.application.Id;
                        applicationTracking.TrackingNumber = app.application.TrackingNumber;
                        applicationTracking.Source_Id = app.application.ApplicationSource_Id;
                        applicationTracking.Status_Id = app.application.Status_Id;
                        applicationTracking.Officer_Id = app.application.PandSOfficer_Id;
                        applicationTracking.Log_Id = app.application.CurrentLog_Id;
                        applicationTracking.IsActive = true;
                        applicationTracking.UserId = User.Identity.GetUserId();
                        applicationTracking.Username = User.Identity.GetUserName();
                        applicationTracking.Datetime = DateTime.UtcNow.AddHours(5);
                        db.ApplicationTrackings.Add(applicationTracking);
                        db.SaveChanges();
                    }
                }
                return Ok(app);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("Search")]
        public IHttpActionResult Search([FromBody] SearchQuery searchQuery)
        {
            try
            {
                string role = User.IsInRole("Secondary") ? "Secondary" :
                    User.IsInRole("Primary") ? "Primary" :
                    User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC Admin" :
                    User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                    User.IsInRole("Administrator") ? "Administrator" :
                    User.IsInRole("PACP") ? "PACP" :
                    User.IsInRole("South Punjab") ? "South Punjab" :
                    User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                    //Added New role  District Against Chief Executive Officer
                    User.IsInRole("Districts") ? "Districts" :
                    User.IsInRole("Order Generation") ? "Order Generation" :

                    User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";

                return Ok(_rootService.Search(searchQuery.Query, User.Identity.GetUserId(), role, User.Identity.GetUserName()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SearchHealthFacilities")]
        public IHttpActionResult SearchHealthFacilities([FromBody] SearchQuery searchQuery)
        {
            try
            {
                return Ok(_rootService.SearchHealthFacilities(searchQuery.Query, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SearchHealthFacilitiesAll")]
        public IHttpActionResult SearchHealthFacilitiesAll([FromBody] SearchQuery searchQuery)
        {
            try
            {
                return Ok(_rootService.SearchHealthFacilitiesAll(searchQuery.Query));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SearchVacancy")]
        public IHttpActionResult SearchVacancy([FromBody] SearchQuery searchQuery)
        {
            try
            {
                return Ok(_rootService.SearchVacancy(searchQuery.Query, searchQuery.designationId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveEntityLog")]
        public IHttpActionResult SaveEntityLog([FromBody] Entity_Log entity_Log)
        {
            try
            {
                if (entity_Log.Entity_Id == null || entity_Log.Entity_Id <= 0 || entity_Log.FK_Id <= 0 || entity_Log.FK_Id == null)
                {
                    return BadRequest("Null");
                }
                return Ok(_rootService.SaveEntityLog(entity_Log, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SearchEmployees")]
        [HttpPost]
        public async Task<IHttpActionResult> SearchEmployees([FromBody] ProfileFilters profileFilters)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (IsCNIC(profileFilters.searchTerm))
                {
                    profileFilters.searchTerm = profileFilters.searchTerm.Replace("-", "");
                    var profiles = await db.ProfileListViews.Where(x => x.CNIC.Equals(profileFilters.searchTerm))
                   .OrderBy(x => x.EmployeeName).ToListAsync();
                    return Ok(profiles);
                }
                return Ok(404);
            }
        }
        [Route("CalcDate/{fromDate}/{toDate}/{totalDays}")]
        [HttpGet]
        public IHttpActionResult CalcDate(string fromDate, string toDate, int totalDays)
        {
            try
            {
                if (toDate.Equals("noDate"))
                {
                    return Ok(Common.ToDate(fromDate, totalDays));

                }
                else
                {
                    return Ok(Common.CalculateDays(fromDate, toDate));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetHrMarkings/{desgnationId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHrMarkings(int desgnationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var hrMarkings = await db.HrMarkings.FirstOrDefaultAsync(x => x.Designation_Id == desgnationId && x.IsActive == true);
                    return Ok(hrMarkings);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SaveHrMarkings")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveHrMarkings([FromBody] HrMarking hrMarking)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (hrMarking.Id > 0)
                    {
                        hrMarking.ModifiedUserId = User.Identity.GetUserId();
                        hrMarking.ModifiedUserName = User.Identity.GetUserName();
                        hrMarking.ModifiedDateTime = DateTime.UtcNow.AddHours(5);
                        hrMarking.IsActive = true;
                        db.Entry(hrMarking).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Ok(hrMarking);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SaveOfficer")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveOfficer([FromBody] P_SOfficers p_SOfficer)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var offcier = db.P_SOfficers.FirstOrDefault(x => x.Id == p_SOfficer.Id);
                    db.Configuration.ProxyCreationEnabled = false;
                    if (p_SOfficer.Id > 0)
                    {
                        if (offcier != null)
                        {
                            offcier.Name = p_SOfficer.Name;
                            db.Entry(offcier).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        return Ok(offcier);
                    }
                    else
                    {
                        var newOfficer = new P_SOfficers();
                        newOfficer.Name = p_SOfficer.Name;
                        newOfficer.DesignationName = p_SOfficer.DesignationName;
                        newOfficer.User_Id = p_SOfficer.User_Id;
                        newOfficer.Program = p_SOfficer.Program;
                        newOfficer.Code = p_SOfficer.Code;
                        newOfficer.Designation_Id = p_SOfficer.Designation_Id;
                        newOfficer.CNIC = p_SOfficer.CNIC;
                        newOfficer.Contact = p_SOfficer.Contact;
                        db.P_SOfficers.Add(newOfficer);
                        await db.SaveChangesAsync();
                        return Ok(newOfficer);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetHFVacs/{hfmisCode}/{DesgId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHFVacs(string hfmisCode, int DesgId)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                //var hfs = await db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();

                List<int> designationIds = new List<int>();
                if (DesgId > 0)
                {
                    designationIds.Add(DesgId);
                    if (DesgId == 802 || DesgId == 1320)
                    {
                        designationIds.Add(2404);
                    }
                }
                var hfs = await db.VpMastProfileViews.Where(x => x.HFAC == 1 && x.HFMISCode.StartsWith(hfmisCode) && designationIds.Contains(x.Desg_Id)).OrderByDescending(x => (x.Vacant + x.Adhoc)).ThenByDescending(k => k.TotalSanctioned).ToListAsync();
                return Ok(hfs);

            }
        }
        [Route("GetHFApplications/{HFId}/{TOHFId}/{DesgId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHFApplications(int HFId, int TOHFId, int DesgId)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                //var hfs = await db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();

                var applicationFromCounts = await db.ApplicationViews.Where(x => x.HealthFacility_Id == HFId && x.Designation_Id == DesgId && x.Status_Id == 1 && x.ApplicationSource_Id == 3 && x.IsActive == true).OrderBy(k => k.Created_Date).ToListAsync();
                var applicationToCounts = await db.ApplicationViews.Where(x => x.ToHF_Id == TOHFId && x.Designation_Id == DesgId && x.Status_Id == 1 && x.ApplicationSource_Id == 3 && x.IsActive == true).OrderBy(k => k.Created_Date).ToListAsync();
                return Ok(new { applicationFromCounts, applicationToCounts });

            }
        }
        [Route("UserRight")]
        [HttpPost]
        public IHttpActionResult UserRight([FromBody] C_UserRight userRight)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                if (!string.IsNullOrEmpty(userRight.User_Id))
                {
                    var userRightDb = db.C_UserRight.FirstOrDefault(x => x.User_Id.Equals(userRight.User_Id));
                    if (userRightDb != null)
                    {
                        userRightDb.AddFacility = userRight.AddFacility;
                        userRightDb.AddProfile = userRight.AddProfile;
                        userRightDb.AddVacancy = userRight.AddVacancy;
                        userRightDb.EditFacility = userRight.EditFacility;
                        userRightDb.EditProfile = userRight.EditProfile;
                        userRightDb.EditVacancy = userRight.EditVacancy;
                        userRightDb.RemoveFacility = userRight.RemoveFacility;
                        userRightDb.RemoveProfile = userRight.RemoveProfile;
                        userRightDb.RemoveVacancy = userRight.RemoveVacancy;

                        userRightDb.ModifiedBy = User.Identity.GetUserName();
                        userRightDb.ModifiedByUserId = User.Identity.GetUserId();
                        userRightDb.ModifiedDateTime = DateTime.UtcNow.AddHours(5);

                        db.Entry(userRightDb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        userRight.CreatedBy = User.Identity.GetUserName();
                        userRight.CreatedByUserId = User.Identity.GetUserId();
                        userRight.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        db.C_UserRight.Add(userRight);
                        db.SaveChanges();
                    }
                }
                return Ok(userRight);
            }
        }
        [Route("GetApplicationPurposes")]
        [HttpGet]
        public IHttpActionResult GetApplicationPurposes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var applicationPurposes = db.ApplicationPurposes.ToList();
                return Ok(applicationPurposes);
            }
        }
        [Route("GetPensionDocuments")]
        [HttpGet]
        public IHttpActionResult PensionDocuments()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var pensionDocuments = db.PensionDocuments.ToList();
                return Ok(pensionDocuments);
            }
        }
        [Route("GetOfficerStampId")]
        [HttpGet]
        public IHttpActionResult GetOfficerStampId()
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                var officerStamps = db.ShowOfficerStamps.ToList();
                return Ok(officerStamps);
            }
        }
        [Route("GetUserRight")]
        [HttpGet]
        public IHttpActionResult GetUserRight()
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                var userRightDb = db.C_UserRight.ToList();
                return Ok(userRightDb);
            }
        }
        [Route("GetUserRightById")]
        [HttpPost]
        public IHttpActionResult GetUserRightById([FromBody] C_UserRight userRight)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                if (!string.IsNullOrEmpty(userRight.User_Id))
                {
                    var userRightDb = db.C_UserRight.FirstOrDefault(x => x.User_Id.Equals(userRight.User_Id));
                    if (userRightDb != null)
                    {
                        return Ok(userRightDb);
                    }
                }
                return Ok(false);
            }
        }
        [Route("SubscribeAlert/{profileId}/{vpMasterId}")]
        [HttpGet]
        public IHttpActionResult SubscribeAlert(int profileId, int vpMasterId)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                VpSubscriber vpSubscriber = null;
                if (profileId > 0 && vpMasterId > 0)
                {
                    vpSubscriber = new VpSubscriber();
                    vpSubscriber.Profile_Id = profileId;
                    vpSubscriber.VPMaster_Id = vpMasterId;
                    db.VpSubscribers.Add(vpSubscriber);
                    db.SaveChanges();
                }
                return Ok(vpSubscriber);

            }
        }
        [Route("SetRequiredAppDocument/{id}/{isRequired}")]
        [HttpGet]
        public IHttpActionResult SetRequiredAppDocument(int id, bool isRequired)
        {
            using (var db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                var applicationDocument = db.ApplicationDocuments.FirstOrDefault(x => x.Id == id);
                if (applicationDocument != null)
                {
                    applicationDocument.IsRequired = isRequired;
                    db.Entry(applicationDocument).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(applicationDocument);
                }
                return Ok(false);
            }
        }
        [Route("GetDesignations")]
        [HttpPost]
        public async Task<TableResponse<HrDesignationView>> GetDesignations(DesignationFilterModel filters)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                string role = User.IsInRole("Secondary") ? "Secondary" :
                   User.IsInRole("Primary") ? "Primary" :
                   User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC" :
                   User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                   User.IsInRole("Administrator") ? "Administrator" :
                   User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                 //Added New role  District Against Chief Executive Officer
                 User.IsInRole("Districts") ? "Districts" :
                 User.IsInRole("Order Generation") ? "Order Generation" :

                   User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";
                IQueryable<HrDesignationView> query = _db.HrDesignationViews.Where(x => x.IsActive == true).AsQueryable(); ;
                var count = query.Count();
                //if (role.Equals("PHFMC"))
                //{
                //    var phfmcDesignationIds = _db.PHFMC_Designations.Select(k => k.Id).ToList();
                //    query = query.Where(x => phfmcDesignationIds.Contains(x.Id));
                //}
                var list = await query.OrderBy(x => x.Name).Skip(filters.Skip).Take(filters.PageSize).ToListAsync();
                return new TableResponse<HrDesignationView> { List = list, Count = count };
            }
        }
        [Route("GetConsultantDesignations")]
        [HttpGet]
        public async Task<TableResponse<HrDesignationView>> GetConsultantDesignations()
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                List<int> designations = new List<int>()
                {
                    362,365,368,369,373,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313, 2140, 2626
                };
                IQueryable<HrDesignationView> query = _db.HrDesignationViews.Where(x => designations.Contains(x.Id) && x.IsActive == true).AsQueryable(); ;
                var count = query.Count();

                var list = await query.ToListAsync();
                return new TableResponse<HrDesignationView> { List = list, Count = count };
            }
        }
        [Route("GetAdhocDesignations")]
        [HttpGet]
        public IHttpActionResult GetAdhocDesignations()
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                List<int> designations = new List<int>()
                {
                    362,365,368,369,373,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313,21,22,802,1085,932,936,1157,1320,302,431,582,903,447,1060, 2404
                };
                var res = _db.HrDesignations.Where(x => designations.Contains(x.Id) && x.IsActive == true).OrderBy(x => x.Name).ToList(); ;
                return Ok(res);
            }
        }
        [Route("GetMeritActiveDesignations")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMeritActiveDesignations()
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var list = await _db.MeritActiveDesignations.Where(x => x.Desg_Id > 0).OrderByDescending(x => x.Id).ToListAsync(); ;
                return Ok(list);
            }
        }
        [Route("GetCurrentDate")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentDate()
        {
            using (var _db = new HR_System())
            {
                return Ok(DateTime.UtcNow.AddHours(5));
            }
        }
        [Route("GetDesignationsCadreWise/{Id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDesignationsCadreWise(int Id)
        {
            using (var _db = new HR_System())
            {

                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<int> dIds = new List<int> { 325, 446, 486, 488, 489, 812 };
                    var data = _db.HrDesignations.Where(x => dIds.Contains(x.Id) && x.IsActive == true).ToList();
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetVacantPosts/{designationId}/{profileId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacantPosts(int designationId, int profileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    //if(profileId == 0)
                    // {
                    //     var data = _db.VpMasterProfileViews.Where(x => x.Desg_Id == designationId 
                    //     && x.Vacant > 0 
                    //     && !x.DistrictName.Equals("Lahore") 
                    //     && !x.DistrictName.Equals("Islamabad") 
                    //     && (x.HFTypeCode.Equals("011") || x.HFTypeCode.Equals("012")))
                    //     .OrderBy(k => k.HFName)
                    //     .ThenBy(k => k.DistrictName)
                    //     .ToList();
                    //     return Ok(data);
                    // }else
                    // {
                    //     var data = _db.VpMProfileViews.Where(x => x.Desg_Id == designationId && x.Vacant > 0 && (x.HFTypeCode.Equals("011") || x.HFTypeCode.Equals("012"))).ToList();
                    //     return Ok(data);
                    // }
                    //&& !x.DistrictName.Equals("Lahore")
                    // && !x.DistrictName.Equals("Islamabad")
                    var data = _db.HFOpenedViews.Where(x => x.Designation_Id == designationId

                       && x.IsActive == true)
                       .OrderBy(k => k.FullName)
                       .ThenBy(k => k.DistrictName)
                       .ToList();
                    return Ok(data);
                    //if (profileId == 0)
                    //{

                    //}
                    //else
                    //{
                    //    var data = _db.VpMProfileViews.Where(x => x.Desg_Id == designationId && x.Vacant > 0 && (x.HFTypeCode.Equals("011") || x.HFTypeCode.Equals("012"))).ToList();
                    //    return Ok(data);
                    //}
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetDesignationsFiltered")]
        [HttpPost]
        public async Task<IHttpActionResult> GetDesignationsFiltered(DesignationFilterModel filters)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<HrDesignationView> query = _db.HrDesignationViews.Where(x => x.IsActive == true).AsQueryable();
                    if (filters.HF_Id != 0)
                    {
                        var vpMasters = _db.VPMasters.Where(x => x.HF_Id == filters.HF_Id).Select(k => k.Desg_Id).Distinct().ToList();
                        query = query.Where(x => vpMasters.Contains(x.Id)).AsQueryable();
                    }
                    var list = await query.OrderBy(x => x.Name).ToListAsync();
                    return Ok(list);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("GetHFOpened")]
        [HttpPost]
        public IHttpActionResult GetHFOpened([FromBody] Filter filter)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var hfOpenedViews = _db.HFOpenedViews.Where(x => x.FullName.Contains(filter.Query) && x.Designation_Id == filter.DesignationId && x.IsActive == true).ToList();
                        return Ok(hfOpenedViews);
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("SaveSession")]
        public IHttpActionResult SaveSession([FromBody] C_SessionLog sessionLog)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    if (sessionLog.IP != null)
                    {
                        sessionLog.Username = User.Identity.GetUserName();
                        sessionLog.User_Id = User.Identity.GetUserId();
                        sessionLog.LoginTime = DateTime.UtcNow.AddHours(5);
                        _db.C_SessionLog.Add(sessionLog);
                        _db.SaveChanges();
                        return Ok(true);
                    }
                    else return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetCurrentOfficer")]
        public IHttpActionResult GetCurrentOfficer()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    if (currentOfficer != null)
                    {
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                        return Ok(currentOfficer);
                    }
                    else return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetDisabilityTypes")]
        public IHttpActionResult GetDisabilityTypes()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var res = _db.Disabilities.ToList();
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetUserActivity")]
        public IHttpActionResult GetUserActivity([FromBody] UserActivityFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var _userService = new UserService();
                    string hfmisCode = _userService.GetUser(User.Identity.GetUserId()).hfmiscode;
                    var query = _db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(hfmisCode)).AsQueryable();

                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        query = query.Where(x => x.Created_By.Contains(filters.Query)
                        || x.Param1.Contains(filters.Query)
                        || x.Param2.Contains(filters.Query)
                        || x.nlog.Contains(filters.Query)
                        ).AsQueryable();
                    }


                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.Created_Date).Skip(filters.Skip).Take(filters.PageSize).ToList();

                    return Ok(new TableResponse<UserCreatedLog> { List = list, Count = count });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetUserVpProfileActivity")]
        public IHttpActionResult GetUserVpProfileActivity([FromBody] UserActivityFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var _userService = new UserService();
                    string hfmisCode = _userService.GetUser(User.Identity.GetUserId()).hfmiscode;
                    var query = _db.UserCreatedVPProfileLogs.Where(x => x.HfmisCode.StartsWith(hfmisCode)).AsQueryable();
                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        query = query.Where(x => x.Created_By.Contains(filters.Query)
                        || x.HFName.Contains(filters.Query)
                        || x.DsgName.Contains(filters.Query)
                        || x.nlog.Contains(filters.Query)
                        ).AsQueryable();
                    }
                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.Created_Date).Skip(filters.Skip).Take(filters.PageSize).ToList();

                    return Ok(new TableResponse<UserCreatedVPProfileLog> { List = list, Count = count });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult GetContactList(ApplicationUser user)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var contactList = db.C_UserContact.Where(x => x.UserId == user.Id && x.IsActive == true).ToList();
                    return Ok(contactList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("SendSMS")]
        [HttpPost]
        public IHttpActionResult SendSMSCode([FromBody] SmsCode smsObj)
        {
            int code = 0;
            string codedPhone = "";
            string phonePattern = "(?<=\\d{4})\\d(?=\\d{2})";
            var userService = new UserService();
            ResponseCode resCode = new ResponseCode();
            try
            {
                if (!string.IsNullOrEmpty(smsObj.PhoneNumber))
                {
                    code = userService.SendAuthenticationCode(smsObj.Username, HttpContext.Current.IsDebuggingEnabled ? "03324862798" : smsObj.PhoneNumber, HttpContext.Current.IsDebuggingEnabled ? "belalmughal@gmail.com" : smsObj.Email);
                    codedPhone = Regex.Replace(smsObj.PhoneNumber, phonePattern, m => new string('*', m.Length));
                    resCode.code = code;
                    resCode.codedPhone = codedPhone;
                }
                return Ok(resCode);

                //using (var db = new HR_System())
                //{
                //    db.Configuration.ProxyCreationEnabled = false;
                //    var contactList = db.C_UserContact.Where(x => x.UserId == user.Id && x.IsActive == true).ToList();
                //    return Ok(contactList);
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SaveDermaApplication")]
        public IHttpActionResult SaveDermaApplication([FromBody] DermaApplication obj)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dermaApp = _db.DermaApplications.FirstOrDefault(x => x.CNIC.Equals(obj.CNIC));
                    try
                    {
                        bool newRecord = false;
                        if (obj == null) return null;
                        if (dermaApp == null)
                        {
                            dermaApp = new DermaApplication();
                            dermaApp.IsActive = true;
                            dermaApp.CreatedDate = DateTime.UtcNow.AddHours(5);
                            newRecord = true;
                        }
                        dermaApp.Name = obj.Name;
                        dermaApp.FatherName = obj.FatherName;
                        dermaApp.CNIC = obj.CNIC;
                        dermaApp.PhoneNumber = obj.PhoneNumber;
                        dermaApp.Designation = obj.Designation;
                        dermaApp.Course = obj.Course;
                        dermaApp.IsInterested = obj.IsInterested;
                        if (newRecord)
                        {
                            _db.DermaApplications.Add(dermaApp);
                        }
                        _db.SaveChanges();
                        return Ok(true);
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetHrPostingMode")]
        public IHttpActionResult GetHrPostingMode()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return Ok(db.HrPostingModes.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public bool IsCNIC(string query)
        {
            query = query.Replace("-", "");
            if (query.Length == 13)
            {
                var isNumber = long.TryParse(query, out long cnic);
                return isNumber;
            }
            else
            {
                return false;
            }
        }

    }
    public class NamesPostModel
    {
        public List<string> Names { get; set; }
    }
    public class MeritPostingModel
    {
        public int? Id { get; set; }
        public string HFName { get; set; }
        public int? HFAC { get; set; }
        public int Count { get; set; }
    }
    public class DistrictsVacancyModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class SmsCode
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    public class ResponseCode
    {
        public int code { get; set; }
        public string codedPhone { get; set; }
    }




}
