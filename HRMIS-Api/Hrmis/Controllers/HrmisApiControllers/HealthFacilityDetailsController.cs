using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using Hrmis.Models.CustomModels;
using Microsoft.AspNet.Identity;
using Hrmis.Models.Dto;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Hrmis.Models;

namespace Hrmis.Controllers.HrmisApiControllers
{
    //[Authorize]
    [RoutePrefix("api/HealthFacilityD")]
    public class HealthFacilityDetailsController : ApiController
    {
        private HR_System db = new HR_System();

        [Route("TypeWiseHealthFacilities/{hfmisCode}")]
        [HttpGet]
        public async Task<List<HFTypeCounts>> TypeWiseHealthFacilities(string hfmisCode)
        {
            try
            {


                List<HFTypeCounts> hfCounts = new List<HFTypeCounts>();
                foreach (HFType hftype in db.HFTypes)
                {
                    int count = await db.HealthFacilities.Where(x => x.HfmisCode.Equals(hfmisCode) || x.HfmisCode.StartsWith(hfmisCode)).CountAsync(x => x.HfmisCode.Substring(12, 3).Equals(hftype.Code));
                    if (count != 0)
                    {
                        hfCounts.Add(new HFTypeCounts(hftype.Name, count));
                    }
                }
                return hfCounts.OrderByDescending(x => x.Count).ToList();
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }
        [Route("SearchHealthFacilities/{hfmisCode}")]
        [HttpPost]
        public async Task<IHttpActionResult> SearchHealthFacilities([FromBody] ProfileQuery searchQuery, string hfmisCode)
        {
            try
            {
                string query = searchQuery.Query.ToLower();
                if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");
                string q = query;

                return Ok(await db.HFDetails
                    .Where(x => (x.FullName.StartsWith(query) || x.FullName.Contains(query)) &&
                    (x.HFMISCode.Equals(hfmisCode) || x.HFMISCode.StartsWith(hfmisCode)) && x.IsActive == true)
                    .Select(x => new { Id = x.Id, HFMISCode = x.HFMISCode, FullName = x.FullName, DistrictName = x.DistrictName, TehsilName = x.TehsilName })
                    .ToListAsync());
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }

        [Route("GetHF/{divID}/{distID}/{TehID}/{HFTypeID}/{currentPage}/{itemsPerPage}")]
        //[Route("GetHF")]
        [HttpGet]
        public IHttpActionResult GetHF(string divID, string distID, string TehID, string HFTypeID, int currentPage = 1, int itemsPerPage = 100)
        {
            string userId = User.Identity.GetUserId();
            if (userId != null)
            {
                using (var db = new HR_System())
                {

                    var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var user = usermanger.FindById(userId);
                    //  var hftypecode;
                    string userHfmisCode = (user.hfmiscode != null ?
                            user.hfmiscode : user.TehsilID != null ?
                                user.TehsilID : user.DistrictID != null ?
                                    user.DistrictID : user.DivisionID != null ?
                                        user.DivisionID : "0");

                    if (currentPage <= 0) currentPage = 1;
                    if (itemsPerPage <= 0) currentPage = 100;
                    IQueryable<HFList> hfs = null;
                    if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                    {
                        hfs = db.HFLists.Where(x => x.HFMISCode.StartsWith(userHfmisCode) && x.HFAC == 2 && x.IsActive == true).OrderBy(x => x.OrderBy).AsQueryable();
                    }
                    //  else if (User.IsInRole("TransferPosting") && user.HfTypeCode != null) //berc
                    else if (user.HfTypeCode != null) //berc
                    {
                        hfs = db.HFLists.Where(x => x.HFMISCode.StartsWith(userHfmisCode) && x.HFTypeCode == user.HfTypeCode && x.IsActive == true).OrderBy(x => x.OrderBy).AsQueryable();
                    }
                    else if (user.UserName.StartsWith("dc.")) //berc
                    {
                        List<string> dcHFTypes = new List<string>
                        {
                            "013",
                            "014",
                            "015",
                            "017",
                            "052"
                        };
                        hfs = db.HFLists.Where(x => x.HFMISCode.StartsWith(userHfmisCode) && dcHFTypes.Contains(x.HFTypeCode) && x.IsActive == true).OrderBy(x => x.OrderBy).AsQueryable();
                    }
                    else
                    {
                        hfs = db.HFLists.Where(x => x.HFMISCode.StartsWith(userHfmisCode) && x.IsActive == true).OrderBy(x => x.OrderBy).AsQueryable();
                    }



                    if (TehID != "0")
                    {
                        hfs = hfs.Where(x => x.TehsilCode.Equals(TehID)).OrderBy(x => x.OrderBy);
                    }
                    else if (TehID == "0")
                    {
                        if (distID != "0")
                        {
                            hfs = hfs.Where(x => x.DistrictCode.Equals(distID)).OrderBy(x => x.OrderBy);
                        }
                        else if (distID == "0")
                        {
                            if (divID != "0")
                            {
                                hfs = hfs.Where(x => x.DivisionCode.Equals(divID)).OrderBy(x => x.OrderBy);
                            }
                        }
                    }

                    if (HFTypeID != "0")
                    {
                        hfs = hfs.Where(x => x.HFTypeCode.Equals(HFTypeID));
                    }
                    hfs = hfs.OrderBy(x => x.OrderBy);
                    int totalRecords = hfs.Count();
                    List<HFList> healthFacilities = hfs.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                    List<DtoHFPic> picList = new List<DtoHFPic>();
                    picList = ReadHFPics(healthFacilities);
                    return Ok(new { healthFacilities, totalRecords, picList });
                }
            }
            else
            {
                return Unauthorized();
            }
        }
        [Route("GetHealthFacilityD/{code}")]
        [HttpGet]
        public async Task<List<HFDetail>> GetHealthFacilityDetails(string code = "")
        {
            try
            {
                string userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var user = usermanger.FindById(userId);
                    string userHfmisCode = (user.hfmiscode != null ?
                            user.hfmiscode : user.TehsilID != null ?
                                user.TehsilID : user.DistrictID != null ?
                                    user.DistrictID : user.DivisionID != null ?
                                        user.DivisionID : "0");
                    if (code.StartsWith(userHfmisCode))
                    {
                        if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                        {
                            return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.IsActive == true && x.HFAC == 2).ToListAsync();
                        }
                        //else if (User.IsInRole("TransferPosting") && user.HfTypeCode != null) //berc
                        else if (user.HfTypeCode != null) //berc
                        {
                            return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.HFTypeCode == user.HfTypeCode && x.IsActive == true).ToListAsync();
                        }
                        else
                        {
                            return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.IsActive == true).ToListAsync();
                        }
                    }
                    else
                    {
                        return null;

                    }
                }
                else
                {
                    return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.IsActive == true).ToListAsync();
                }

            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("GetHealthFacilityByTypeAndCode/{hfmisCodeNew}/{MapFlag}")]
        [HttpGet]
        public async Task<List<HFDetail>> GetHealthFacilityByTypeAndCode(string hfmisCodeNew, bool MapFlag)
        {
            try
            {
                if (MapFlag)
                {
                    return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(hfmisCodeNew) && x.HfmisOldCode != null && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();
                }
                else
                {
                    return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(hfmisCodeNew) && x.HfmisOldCode == null && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();
                }

            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("RemoveHF/{userhfmisCode}/{HF_Id}")]
        [HttpGet]
        public IHttpActionResult RemoveHF(string userhfmisCode, int HF_Id)
        {
            try
            {
                HealthFacility hf = db.HealthFacilities.FirstOrDefault(x => x.Id == HF_Id && x.HfmisCode.StartsWith(userhfmisCode));
                if (hf == null)
                {
                    return Ok(false);
                }
                Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hf.Entity_Lifecycle_Id);
                elc.IsActive = false;
                db.Entry(elc).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("HealthFacilityLazy/{code}/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilityDetailsLazy(string code = "", int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                if (currentPage <= 0) currentPage = 1;
                if (itemsPerPage <= 0) currentPage = 100;

                string userId = User.Identity.GetUserId();
                var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = usermanger.FindById(userId);
                string userHfmisCode = (user.hfmiscode != null ?
                        user.hfmiscode : user.TehsilID != null ?
                            user.TehsilID : user.DistrictID != null ?
                                user.DistrictID : user.DivisionID != null ?
                                    user.DivisionID : "0");
                var query = db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.HFMISCode.StartsWith(userHfmisCode) && x.IsActive == true).OrderBy(x => x.Id).AsQueryable();
                int totalRecords = query.Count();

                return Ok(new { healthFacilities = await query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync(), totalRecords });
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetHealthFacilitiesByTypes")]
        [HttpPost]
        public IHttpActionResult GetHealthFacilitiesByTypes([FromBody] List<string> hfTypesCodes)
        {
            try
            {
                var results = db.HFListPs.Where(x => hfTypesCodes.Contains(x.HFTypeCode) && x.IsActive == true).OrderBy(x => x.FullName).ToList();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("HealthFacilityLazyByTypeOnly/{code}/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilityDetailsLazyByTypeOnly(string code = "", int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                if (currentPage <= 0) currentPage = 1;
                if (itemsPerPage <= 0) currentPage = 100;

                string userId = User.Identity.GetUserId();
                var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = usermanger.FindById(userId);
                string userHfmisCode = (user.hfmiscode != null ?
                        user.hfmiscode : user.TehsilID != null ?
                            user.TehsilID : user.DistrictID != null ?
                                user.DistrictID : user.DivisionID != null ?
                                    user.DivisionID : "0");
                var query = db.HFDetails.Where(x => x.HFTypeCode.StartsWith(code) && x.HFMISCode.StartsWith(userHfmisCode) && x.IsActive == true).OrderBy(x => x.Id).AsQueryable();
                int totalRecords = query.Count();

                return Ok(new { healthFacilities = await query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync(), totalRecords });
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetFunctionalHealthFacilities/{code}")]
        [HttpGet]
        public async Task<List<HFDetail>> GetFunctionalHealthFacilities(string code = "033")
        {
            try
            {
                return await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.Status.Equals("Functional") && x.IsActive == true).ToListAsync();
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }


        [Route("CountHealthFacilities/{code}")]
        [HttpGet]
        public async Task<int> CountHealthFacilities(string code = "")
        {
            return await db.HFDetails.CountAsync(x => x.HFMISCode.StartsWith(code) && x.IsActive == true);
        }

        [Route("GetHealthFacilityById/{id}")]
        [HttpGet]
        [ResponseType(typeof(HFDetail))]
        public async Task<HFDetail> GetHealthFacilityDetail(int id)
        {
            return await db.HFDetails.FirstOrDefaultAsync(x => x.Id == id);
        }

        [Route("AddHealthFacility")]
        [HttpPost]
        [ResponseType(typeof(HealthFacility))]
        public IHttpActionResult PostHealthFacilityDetail(HealthFacility healthFacility)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (HealthFacilityDetailExists(healthFacility.Id))
            {
                HealthFacility hf = db.HealthFacilities.FirstOrDefault(x => x.Id == healthFacility.Id);
                hf.Name = healthFacility.Name;
                hf.HFAC = healthFacility.HFAC;
                db.Entry(hf).State = EntityState.Modified;
            }
            else
            {
                Entity_Lifecycle elc = new Entity_Lifecycle();
                elc.IsActive = true;
                elc.Created_By = User.Identity.GetUserName();
                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                elc.Entity_Id = 1;
                elc.Users_Id = User.Identity.GetUserId();
                db.Entity_Lifecycle.Add(elc);
                db.SaveChanges();
                healthFacility.Entity_Lifecycle_Id = elc.Id;
                db.HealthFacilities.Add(healthFacility);
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (HealthFacilityDetailExists(healthFacility.Id))
                    return Conflict();
                else if (!HealthFacilityDetailExists(healthFacility.Id))
                    return NotFound();
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            return Ok(new { id = healthFacility.Id });
        }

        [Route("AddHealthFacilityD")]
        [HttpPost]
        [ResponseType(typeof(HealthFacility))]
        public async Task<IHttpActionResult> PostHealthFacilityD(HealthFacility healthFacility)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (HealthFacilityDetailExists(healthFacility.Id))
            {
                db.Entry(healthFacility).State = EntityState.Modified;
            }
            try
            {
                await db.SaveChangesAsync();
                return Ok(new { id = healthFacility.Id });
            }
            catch (DbUpdateException)
            {
                if (HealthFacilityDetailExists(healthFacility.Id))
                    return Conflict();
                else if (!HealthFacilityDetailExists(healthFacility.Id))
                    return NotFound();
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException?.Message ?? ex.Message);
            }
        }
        [Route("UpdateHFGeoLocation/{HFId}/{oldHFMISCode}/{newHFMISCode}")]
        [HttpGet]
        public IHttpActionResult UpdateHFGeoLocation(int HFId, string oldHFMISCode, string newHFMISCode)
        {
            try
            {
                var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == HFId);
                hf.HfmisCode = newHFMISCode;
                db.Entry(hf).State = EntityState.Modified;

                db.Database.ExecuteSqlCommand("update VPMaster set HFMISCode='" + newHFMISCode + "' where HF_Id=" + HFId + ";");

                db.Database.ExecuteSqlCommand("update HFService set HfmisCode='" + newHFMISCode + "' where HF_Id=" + HFId + ";");

                db.Database.ExecuteSqlCommand("update MeritPreferences set HfmisCode='" + newHFMISCode + "' where HfmisCode='" + oldHFMISCode + "'");

                db.Database.ExecuteSqlCommand("update ESR set HfmisCodeFrom='" + newHFMISCode + "' where HF_Id_From = " + HFId + ";");
                db.Database.ExecuteSqlCommand("update ESR set HfmisCodeTo='" + newHFMISCode + "' where HF_Id_To = " + HFId + ";");

                db.SaveChanges();
                return Ok(newHFMISCode);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateHFMode/{HFId}/{ModeId}")]
        [HttpGet]
        public IHttpActionResult UpdateHFMode(int HFId, int ModeId)
        {
            try
            {
                HFMode hfMode = db.HFModes.FirstOrDefault(x => x.HF_Id == HFId);
                if (hfMode == null)
                {
                    hfMode = new HFMode()
                    {
                        Mode_Id = ModeId,
                        HF_Id = HFId,
                        ModeName = ModeId == 1 ? "BHU 24/7" : ModeId == 2 ? "BHU Plus" : "Other"
                    };
                    db.HFModes.Add(hfMode);
                }
                else
                {
                    hfMode.Mode_Id = ModeId;
                    hfMode.ModeName = ModeId == 1 ? "BHU 24/7" : ModeId == 2 ? "BHU Plus" : "Other";
                    db.Entry(hfMode).State = EntityState.Modified;
                }

                db.SaveChanges();
                return Ok(hfMode.ModeName);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetHFMode/{HFId}")]
        [HttpGet]
        public IHttpActionResult GetHFMode(int HFId)
        {
            try
            {
                return Ok(db.HFModes.FirstOrDefault(x => x.HF_Id == HFId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("AddHFBlocks")]
        [HttpPost]
        public async Task<IHttpActionResult> PostHFBlocks(HFBlock hfBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.HFBlocks.Add(hfBlock);
            try
            {
                await db.SaveChangesAsync();
                return Ok(new { hfBlock.Id });
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
        }
        [Route("AddHFWards")]
        [HttpPost]
        public async Task<IHttpActionResult> PostHFBlocks(HFWard hfWard)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HFWards.Add(hfWard);
            try
            {
                await db.SaveChangesAsync();
                return Ok(new { hfWard.Id });
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
        }
        [Route("EditHFWard/{Id}")]
        [HttpPost]
        public async Task<IHttpActionResult> EditHFWard([FromBody] HFWard hfWard, int Id)
        {
            if (hfWard == null) { return BadRequest("No Object"); }
            if (string.IsNullOrEmpty(hfWard.Name)) { return BadRequest("No Ward Name"); }
            try
            {
                var ward = db.HFWards.FirstOrDefault(x => x.Id == Id);
                ward.Name = hfWard.Name;
                ward.NoB = hfWard.NoB;
                ward.NoGB = hfWard.NoGB;
                ward.NoSB = hfWard.NoSB;
                await db.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
        }
        [Route("UpdateHFWards")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateHFWards(List<HFWard> hfWards)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                foreach (HFWard hfWard in hfWards)
                {
                    db.Entry(hfWard).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return Ok(new { result = true });
            }
            catch (DbUpdateException dbex)
            {
                return BadRequest(dbex.Message);
            }
        }
        [Route("AddHFServices")]
        [HttpPost]
        public async Task<IHttpActionResult> PostHFServices(HFService hfServices)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.HFServices.Add(hfServices);
            try
            {
                await db.SaveChangesAsync();
                return Ok(new { hfService = hfServices });
            }
            catch (DbUpdateException)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewHFMISCode/{hfCode}")]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> GetNewHFMISCode(string hfCode = "")
        {
            List<HealthFacility> healthFacilityList = await db.HealthFacilities.Where(x => x.HfmisCode.StartsWith(hfCode)).ToListAsync();
            List<int> hfnameCodes = new List<int>();
            foreach (var item in healthFacilityList)
            {
                hfnameCodes.Add(Convert.ToInt32(item.HfmisCode.Substring(15, 4)));
            }
            hfnameCodes.Sort();
            if (hfnameCodes.Count == 0) hfnameCodes.Add(1);
            else hfnameCodes.Add((hfnameCodes.Last() + 1));
            string hfnameCode = Convert.ToString(hfnameCodes.Last());
            if (hfnameCode.Length == 1)
                hfnameCode = "000" + hfnameCode;
            else if (hfnameCode.Length == 2)
                hfnameCode = "00" + hfnameCode;
            else if (hfnameCode.Length == 3)
                hfnameCode = "0" + hfnameCode;

            return hfCode + hfnameCode;
        }
        [Route("SetDesignationCadre/{postId}/{cadreId}")]
        [HttpPost]
        public async Task<IHttpActionResult> SetDesignationCadre(int postId, int cadreId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            HrDesignation currentDesignation = db.HrDesignations.FirstOrDefault(x => x.Id == postId);
            if (currentDesignation != null)
            {
                currentDesignation.Cadre_Id = cadreId;
                currentDesignation.Created_By = User.Identity.Name;
                currentDesignation.Creation_Date = DateTime.UtcNow.AddHours(5);

                db.Entry(currentDesignation).State = EntityState.Modified;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
            return Ok(new { id = currentDesignation.Id });
        }
        [Route("SetDesignationScale/{postId}/{scale}")]
        [HttpPost]
        public async Task<IHttpActionResult> SetDesignationScale(int postId, int scale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            HrDesignation currentDesignation = db.HrDesignations.FirstOrDefault(x => x.Id == postId);
            if (currentDesignation != null)
            {
                currentDesignation.HrScale_Id = scale;
                currentDesignation.Created_By = User.Identity.Name;
                currentDesignation.Creation_Date = DateTime.UtcNow.AddHours(5);
                db.Entry(currentDesignation).State = EntityState.Modified;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
            return Ok(new { id = currentDesignation.Id });
        }
        [Route("SetDesignationScale2/{postId}/{scale2}")]
        [HttpPost]
        public async Task<IHttpActionResult> SetDesignationScale2(int postId, int scale2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            HrDesignation currentDesignation = db.HrDesignations.FirstOrDefault(x => x.Id == postId);
            if (currentDesignation != null)
            {
                currentDesignation.HrScale_Id2 = scale2;
                currentDesignation.Created_By = User.Identity.Name;
                currentDesignation.Creation_Date = DateTime.UtcNow.AddHours(5);
                db.Entry(currentDesignation).State = EntityState.Modified;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException dbex)
            {
                return Ok(dbex.Message);
            }
            return Ok(new { id = currentDesignation.Id });
        }

        [Route("UploadPic/{HFId}")]
        public async Task<IHttpActionResult> FileUpload(int HFId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\HFPics\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";

                foreach (var file in provider.Contents)
                {
                    filename = Guid.NewGuid().ToString() + "_" + HFId + "_" + file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }

                    HFPhoto hfPhoto = new HFPhoto();
                    hfPhoto.HF_Id = HFId;
                    hfPhoto.ImagePath = filename;

                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 5;

                    db.Entity_Lifecycle.Add(eld);
                    await db.SaveChangesAsync();

                    hfPhoto.Entity_Lifecycle_Id = eld.Id;

                    db.HFPhotos.Add(hfPhoto);
                    await db.SaveChangesAsync();
                }
                return Ok(new { result = true, src = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ReadPic/{HFId}")]
        [HttpGet]
        public async Task<IHttpActionResult> ReadPic(int HFId)
        {
            try
            {
                List<HFPhoto> hfPhotos = await db.HFPhotos.Where(x => x.HF_Id == HFId).OrderByDescending(x => x.Id).ToListAsync();
                HFPhoto hfphoto = hfPhotos.First();
                if (hfphoto != null)
                {
                    return Ok(new { result = true, src = hfphoto.ImagePath });
                }
                else
                {
                    return Ok(new { result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public List<DtoHFPic> ReadHFPics(List<HFList> hfDetails)
        {
            try
            {
                List<DtoHFPic> dtoPics = new List<DtoHFPic>();
                foreach (var hfDetail in hfDetails)
                {
                    List<HFPhoto> hfPhotos = db.HFPhotos.Where(x => x.HF_Id == hfDetail.Id).OrderByDescending(x => x.Id).ToList();
                    if (hfPhotos.Count > 0)
                    {
                        HFPhoto hfphoto = hfPhotos.First();
                        dtoPics.Add(new DtoHFPic { HFId = hfDetail.Id, HFphotoPath = @"http://beta.hrmis.pshealth.punjab.gov.pk/Uploads/Files/HFPics/" + hfphoto.ImagePath });

                    }
                    else
                    {
                        dtoPics.Add(new DtoHFPic { HFId = hfDetail.Id, HFphotoPath = null });
                    }

                }
                return dtoPics;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<DtoHFEmployeeeCount> ContHFEmp(List<HFList> hfDetails)
        {
            try
            {
                List<DtoHFEmployeeeCount> dtoEmployeeeCount = new List<DtoHFEmployeeeCount>();
                foreach (var hfDetail in hfDetails)
                {
                    var temp = db.HrProfiles.Where(x => x.HealthFacility_Id == hfDetail.Id || x.HfmisCode == hfDetail.HFMISCode);
                    dtoEmployeeeCount.Add(new DtoHFEmployeeeCount
                    {
                        HFId = hfDetail.Id,
                        TotalEmp = temp.Count()
                        //Gazatted = temp.Where(x => x.CurrentGradeBPS > 16).Count(),
                        //NonGazatted = temp.Where(x => x.CurrentGradeBPS <= 16).Count()
                    });


                }
                return dtoEmployeeeCount;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("RemovePic/{HFId}")]
        [HttpGet]
        public async Task<IHttpActionResult> RemovePic(int HFId)
        {
            try
            {
                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\HFPics\";

                List<HFPhoto> hfphotos = db.HFPhotos.Where(x => x.HF_Id == HFId).ToList();

                foreach (var pic in hfphotos)
                {
                    if (File.Exists(RootPath + pic.ImagePath))
                    {
                        File.Delete(RootPath + pic.ImagePath); ;
                    }
                }

                db.Database.ExecuteSqlCommand("delete from HFPhotos where HF_Id=" + HFId);
                return Ok(new { result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void CreateDirectoryIfNotExists(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        [Route("BedsInfo/{hftypeCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> BedsInfo(string hftypeCode = "0")
        {
            try
            {

                //List<BedsInfo> bedsInfoes = await db.BedsInfoes.ToListAsync();
                //var dists = bedsInfoes.GroupBy(x => x.Districts).Select(x => new { District = x.Key, HFType  = x.Select(y => new { Name = y.HFTypes,Count = y.Count, Beds = y.Beds}).ToList() }).ToList();
                //


                return Ok(await db.BedsInfoes.Where(x => x.HFTypeCode.StartsWith(hftypeCode) && (x.Beds == 0 && x.Count > 0)).ToListAsync());
            }
            catch (DbEntityValidationException dbx) { return BadRequest(GetDbExMessage(dbx)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("GetHFApi")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHFApi()
        {
            try
            {
                return Ok(await db.HFLists.Where(x =>
              (x.HFTypeCode.Equals("011") ||
                x.HFTypeCode.Equals("012") ||
                x.HFTypeCode.Equals("013") ||
                x.HFTypeCode.Equals("014") ||
                x.HFTypeCode.Equals("015") ||
                x.HFTypeCode.Equals("016") ||
                x.HFTypeCode.Equals("017") ||
                x.HFTypeCode.Equals("023") ||
                x.HFTypeCode.Equals("025") ||
                x.HFTypeCode.Equals("029") ||
                x.HFTypeCode.Equals("036") ||
                x.HFTypeCode.Equals("037") ||
                x.HFTypeCode.Equals("039") ||
                x.HFTypeCode.Equals("040") ||
                x.HFTypeCode.Equals("043")) &&
                (x.Latitude != null &&
                x.Longitude != null) &&
                x.IsActive == true).ToListAsync());
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                    dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                        .Aggregate("",
                            (current, validationError) =>
                                current +
                                $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("SetGeomAndLatLong")]
        public IHttpActionResult SetGeomAndLatLong([FromBody] DtoGeom model)
        {
            try
            {
                if (model == null) return BadRequest("Data cannot be empty.");

                var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == model.HFId);
                if (hf == null) return BadRequest("Health Facility Not Found");

                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        hf.Latitude = model.Latitude;
                        hf.Longitude = model.Longitude;
                        db.SaveChanges();
                        var r1 = db.Database.ExecuteSqlCommand($"UPDATE HealthFacilities set GEOM=geometry::STGeomFromText(CONCAT('POINT (',Longitude,' ', Latitude,')'), 0) where id={model.HFId}");
                        var r2 = db.Database.ExecuteSqlCommand($"update HealthFacilities set GEOM.STSrid=4326 where id={model.HFId}");
                        transc.Commit();
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }

                return Ok("Saved");
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException.Message ?? ex.Message);
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



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HealthFacilityDetailExists(int id)
        {
            return db.HFDetails.Count(e => e.Id == id) > 0;
        }
    }
    public class DtoHFPic
    {
        public int HFId { get; set; }
        public string HFphotoPath { get; set; }
    }
    public class DtoHFEmployeeeCount
    {
        public int HFId { get; set; }
        public int TotalEmp { get; set; }
        public int Gazatted { get; set; }
        public int NonGazatted { get; set; }
    }


    public class DtoGeom
    {
        public int HFId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}