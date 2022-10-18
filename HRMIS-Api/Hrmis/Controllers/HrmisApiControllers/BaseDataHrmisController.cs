using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using System.Data.Entity.Validation;
using Hrmis.Models.Dto;
using System;
using Microsoft.AspNet.Identity;
using Hrmis.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Org.BouncyCastle.Utilities;
using Hrmis.Models.Common;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("api/BaseDataHrmis")]
    public class BaseDataHrmisController : ApiController
    {
        #region With Code
        [Route("GetDivisions")]
        [HttpGet]
        [ResponseType(typeof(Division))]
        public async Task<List<Division>> GetDivisions(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var divisions = await db.Divisions.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToListAsync();
                return divisions;
            }
        }

        [Route("GetDistricts")]
        [HttpGet]
        [ResponseType(typeof(District))]
        public async Task<List<District>> GetDistricts(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var districts = await db.Districts.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).OrderBy(x=>x.Name).ToListAsync();
                return districts;
            }
        }

        [Route("GetTehsils")]
        [HttpGet]
        [ResponseType(typeof(Tehsil))]
        public async Task<List<Tehsil>> GetTehsils(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var tehsils = await db.Tehsils.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToListAsync();
                return tehsils;
            }
        }

        [Route("GetCategories")]
        [HttpGet]
        [ResponseType(typeof(HFCategory))]
        public async Task<List<HFCategory>> GetCategories(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var categories = await db.HFCategories.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToListAsync();
                return categories;
            }
        }

        [Route("GetTypes")]
        [HttpGet]
        [ResponseType(typeof(HFType))]
        public async Task<List<HFType>> GetTypes(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.HFTypes.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToListAsync();
                return types;
            }
        }

        #endregion

        public void barCodeZ()
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            barcode.Draw("123", 50, 10).Save(@"D:\BbC.png");

            Zen.Barcode.CodeQrBarcodeDraw qrcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            qrcode.Draw("123", 50, 10).Save(@"D:\qC.png");
        }

        #region With Code Count
        [Route("CountDivisions")]
        [HttpGet]
        [ResponseType(typeof(int))]
        public async Task<int> CountDivisions(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var divisions = await db.Divisions.CountAsync(x => x.Code.StartsWith(code) || x.Code.Equals(code));
                return divisions;
            }
        }

        [Route("CountDistricts")]
        [HttpGet]
        [ResponseType(typeof(District))]
        public async Task<int> CountDistricts(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var districts = await db.Districts.CountAsync(x => x.Code.StartsWith(code) || x.Code.Equals(code));
                return districts;
            }
        }

        [Route("CountTehsils")]
        [HttpGet]
        [ResponseType(typeof(int))]
        public async Task<int> CountTehsils(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var tehsils = await db.Tehsils.CountAsync(x => x.Code.StartsWith(code) || x.Code.Equals(code));
                return tehsils;
            }
        }

        [Route("CountCategories")]
        [HttpGet]
        [ResponseType(typeof(int))]
        public async Task<int> CountCategories(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var categories = await db.HFCategories.CountAsync(x => x.Code.StartsWith(code) || x.Code.Equals(code));
                return categories;
            }
        }

        [Route("CountTypes")]
        [HttpGet]
        [ResponseType(typeof(int))]
        public async Task<int> CountTypes(string code)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.HFTypes.CountAsync(x => x.Code.StartsWith(code) || x.Code.Equals(code));
                return types;
            }
        }
        #endregion

        #region Without Hf Code

        [Route("GetDivisions")]
        [HttpGet]
        [ResponseType(typeof(Division))]
        public async Task<List<Division>> GetDivisions()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var divisions = await db.Divisions.ToListAsync();
                return divisions;
            }
        }

        [Route("GetDistricts")]
        [HttpGet]
        [ResponseType(typeof(District))]
        public async Task<List<District>> GetDistricts()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var districts = await db.Districts.OrderBy(x=> x.Name).ToListAsync();
                return districts;
            }
        }


        [Route("GetTehsils")]
        [HttpGet]
        [ResponseType(typeof(Tehsil))]
        public async Task<List<Tehsil>> GetTehsils()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var tehsils = await db.Tehsils.ToListAsync();
                return tehsils;
            }
        }
        [Route("GetCategories")]
        [HttpGet]
        [ResponseType(typeof(HFCategory))]
        public async Task<List<HFCategory>> GetCategories()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var categories = await db.HFCategories.ToListAsync();
                return categories;
            }
        }
        [Route("GetTypes")]
        [HttpGet]
        [ResponseType(typeof(HFType))]
        public async Task<List<HFType>> GetTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var types = await db.HFTypes.ToListAsync();
                return types;
            }
        }

        [Route("GetDesignations")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<HrDesignationView>> GetDesignations()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrDesignationViews = await db.HrDesignationViews.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToListAsync();
                return hrDesignationViews;
            }
        }

        [Route("GetDesignationsVecancy")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<HrDesignationVecancyView>> GetDesignationsVecancy()
        {
            var myInClause = new int[] { 2390,2512, 294, 296, 2632, 499, 2389, 2511, 736, 2631, 1033, 1037, 1040, 2391, 2513, 1292, 2386, 2387, 2388 };
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string role = User.IsInRole("Secondary") ? "Secondary" :
                  User.IsInRole("Primary") ? "Primary" :
                  User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC" :
                  User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                  User.IsInRole("Administrator") ? "Administrator" :
                  User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                  // new role added by adnan 17/10/2022
                  User.IsInRole("Districts") ? "Districts" :
                  User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";
                IQueryable<HrDesignationView> query = db.HrDesignationViews.Where(x => x.IsActive == true).AsQueryable();
                if (role.Equals("PHFMC") || role.Equals("PHFMC Admin"))
                {
                    var hrDesignationPHFMCViews = await query.Where(x => !myInClause.Contains(x.Id)).OrderBy(x => x.Name).Select(y => new HrDesignationVecancyView { code = y.Id, name = y.Name, id = y.Id }).ToListAsync();
                    //var PhfmcDesignations= db.HrDesignationViews.Where(x => !myInClause.Contains(x.Id)).OrderBy(x => x.Name).ToListAsync();
                    return hrDesignationPHFMCViews;
                    //var PhfmcDesignations = await db.HrDesignationViews.Where(x => !myInClause.Contains(x.Id)).ToListAsync();

                }
                var hrDesignationViews = await query.OrderBy(x => x.Name).Select(y => new HrDesignationVecancyView { code = y.Id, name = y.Name, id = y.Id }).ToListAsync();
                return hrDesignationViews;
            }
        }
        [Route("GetDesignationsNonGazatted")]
        [HttpGet]
        public async Task<List<HrDesignationView>> GetDesignationsNonGazatted()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrDesignationViews = await db.HrDesignationViews.Where(x => x.IsActive == true && (x.Scale < 17 || x.Scale2 < 17)).ToListAsync();
                return hrDesignationViews;
            }
        }

        [Route("Designations/UnDefined/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUnAssignedCadreDesignations(int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {

                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var results = db.HrDesignationViews.Where(x => x.Cadre_Id == 13).OrderBy(x => x.Name).AsQueryable();
                    int totalRecords = results.Count();
                    var hrDesignationViews = await results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
                    return Ok(new { hrDesignationViews, totalRecords });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [Route("Designation/New/{username=username}")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateDesignation(string username, [FromBody] HrDesignationDto designation)
        {
            try
            {
                if (designation == null) return BadRequest("Provided record is Not Valid");
                if (string.IsNullOrWhiteSpace(designation.Name)) return BadRequest("Name can not be Empty");

                var desg = new HrDesignation();
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (db.Cadres.Count(x => x.Id == designation.Cadre_Id) == 0) return BadRequest("Provided Cadre is Not Valid");
                    if (db.HrScales.Count(x => x.Id == designation.HrScale_Id) == 0) return BadRequest("Provided Scale is Not Valid");

                    desg.Name = designation.Name;
                    desg.HrScale_Id = designation.HrScale_Id;
                    desg.Cadre_Id = designation.Cadre_Id;
                    desg.Created_By = User.Identity.Name;
                    desg.Creation_Date = DateTime.UtcNow.AddHours(5);
                    desg.Remarks = "Added By SDP";
                    desg.IsActive = true;
                    db.HrDesignations.Add(desg);
                    await db.SaveChangesAsync();
                    return Ok(desg);
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }




        [Route("Designation/Update/Name")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDesignationName([FromBody] HrDesignationDto designation)
        {
            try
            {
                if (designation == null) return BadRequest("Provided record is Not Valid");
                if (string.IsNullOrWhiteSpace(designation.Name)) return BadRequest("Name can not be Empty");

                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var desg = db.HrDesignations.Where(x => x.Id == designation.Id).FirstOrDefault();
                    if (desg == null) return BadRequest("Unable to find selected Post");
                    desg.Name = designation.Name;
                    desg.Created_By = User.Identity.Name;
                    desg.Creation_Date = DateTime.UtcNow.AddHours(5);
                    db.Entry(desg).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok(desg);
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [Route("UpdateDesignation")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDesignation([FromBody] HrDesignation hrDesig)
        {
            try
            {
                if (hrDesig == null) return BadRequest("Provided record is Not Valid");
                if (hrDesig.HrScale_Id > 16) return BadRequest("Only Non Gazatted Posts");
                if (hrDesig.HrScale_Id2 > 16) return BadRequest("Only Non Gazatted Posts");

                using (var db = new HR_System())
                {
                    HrDesignation hrDesignation = await db.HrDesignations.FirstOrDefaultAsync(x => x.Name.Equals(hrDesig.Name));
                    hrDesignation.Name = hrDesig.Name;
                    hrDesignation.HrScale_Id = hrDesig.HrScale_Id;
                    hrDesignation.HrScale_Id2 = hrDesig.HrScale_Id2;
                    hrDesignation.Cadre_Id = hrDesig.Cadre_Id;
                    hrDesignation.Created_By = User.Identity.Name;
                    hrDesignation.Creation_Date = DateTime.UtcNow.AddHours(5);
                    hrDesignation.Remarks = "Added By SDP";
                    hrDesignation.IsActive = true;
                    db.Entry(hrDesignation).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok(new { result = true });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("Designation/Search/{name}/{activato}")]
        [HttpGet]
        public async Task<IHttpActionResult> FindDesignation(string name, bool activato)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("Provided record is Not Valid");
                var searchWords = name.ToLower().Split(' ');
                var matched = new HrDesignation();
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //var results = db.HrDesignations.Where(x => searchWords.Any(k => x.Name.Contains(k))).OrderBy(x => x.Name).ToList();
                    var results = await db.HrDesignations.Where(x => x.Name.StartsWith(name) && x.IsActive == activato)
                        .OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name = x.Name, Scale = x.HrScale_Id, CadreId = x.Cadre_Id }).ToListAsync();
                    return Ok(results);
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("Designation/Remove/{Id}")]
        [HttpGet]
        public async Task<IHttpActionResult> RemoveDesignation(int Id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    HrDesignation hrDesignation = await db.HrDesignations.FirstOrDefaultAsync(x => x.Id == Id);
                    hrDesignation.HrScale_Id = null;
                    hrDesignation.HrScale_Id2 = null;
                    hrDesignation.Cadre_Id = null;
                    hrDesignation.Created_By = User.Identity.Name;
                    hrDesignation.Creation_Date = DateTime.UtcNow.AddHours(5);
                    hrDesignation.Remarks = "Removed By " + User.Identity.Name;
                    hrDesignation.IsActive = false;
                    db.Entry(hrDesignation).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok(new { result = true });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("GetCadres")]
        [HttpGet]
        [ResponseType(typeof(Cadre))]
        public async Task<List<CadreView>> GetCadres()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var cadres = await db.CadreViews.ToListAsync();
                return cadres;
            }
        }
        [Route("GetPostTypes")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(HrPost_Type))]
        public async Task<List<HrPost_TypeView>> GetPostTypes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrPost = await db.HrPost_TypeView.ToListAsync();
                return hrPost;
            }
        }
        [Route("GetScales")]
        [HttpGet]
        [ResponseType(typeof(HrScale))]
        public async Task<List<HrScale>> GetScales()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrScales = await db.HrScales.ToListAsync();
                return hrScales;
            }
        }

        #endregion

        [Route("GetCreatedLog")]
        [HttpGet]
        public IHttpActionResult GetCreatedLog()

        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var user = usermanger.FindById(userId);
                    string userHfmisCode = (user.hfmiscode != null ?
                            user.hfmiscode : user.TehsilID != null ?
                                user.TehsilID : user.DistrictID != null ?
                                    user.DistrictID : user.DivisionID != null ?
                                        user.DivisionID : "0");

                    List<UserCreatedLog> dtos = new List<UserCreatedLog>();
                 //   int totalRecords = db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(userHfmisCode)).Count();

                    ////return Ok(new
                    ////{
                    ////    result = true,
                    ////    logs = db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(userHfmisCode)).Take(100).ToList(),
                    ////    totalRecords = totalRecords
                    ////});


                    foreach (var logs in db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(userHfmisCode)).Take(100).ToList())
                    {
                        UserCreatedLog dto = new UserCreatedLog();
                        dto.Created_By = logs.Created_By;
                        dto.Created_Date = logs.Created_Date;
                        dto.nlog = logs.nlog;
                        dto.Param1 = logs.Param1;
                        dto.Param2 = logs.Param2;
                     //   dto.Count = db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(userHfmisCode)).Count();
                        dtos.Add(dto);
                    }
                return Ok(dtos);


                //  return Ok(new { result = true, logs = db.UserCreatedLogs.Where(x => x.HfmisCode.StartsWith(userHfmisCode)).Take(100).ToList() });
            }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
     


       
        [Route("GetDashboardDivisionWise/{level}/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDashboardDivisionWise(int level, string code)
        {
            using (var db = new HR_System())
            {
                List<DashboardModel> dmList = new List<DashboardModel>();
                int totalDHQ = 0, totalTHQ = 0, totalRHC = 0;
                int totalBHU = 0;
                int totalDisp = 0;
                int totalsph = 0;
                int totalmch = 0;
                int totaltrauma = 0;
                int totalmobDisp = 0;
                int totaltownHsp = 0;
                int totaleyeHsp = 0;
                int totaltbClinic = 0;
                int totaltbinstitute = 0;
                int totaltbFlysq = 0;
                int totaltbPolhsp = 0;
                int totaltbCivilhsp = 0;
                int totaltbFilterC = 0;
                int totaltbDHDC = 0;
                int totaltbMCEU = 0;
                int totaltbSchOP = 0;
                int totaltbMH = 0;
                int totaltbIDH = 0;
                int totaltbDentalC = 0;
                int totaltbCMC = 0;
                int totaltbSHC = 0;
                int totalPSHCD = 0;
                int totalDGHS = 0;
                int totalVP = 0;
                int totalPU = 0;
                int totalAdminOfT = 0;
                int totalAdminOfD = 0;
                int totalAdminOfDist = 0;

                int totalADceo = 0;
                int totalADdohp = 0;
                int totalADdohms = 0;
                int totalADdohhrmis = 0;
                int totalADdcirmnch = 0;
                int totalADdmphfmc = 0;

                if (level == 1)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (Division division in db.Divisions.ToList())
                    {
                        DashboardModel dm = new DashboardModel();
                        dm.Name = division.Name;
                        dm.PSHCD = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("001") && x.IsActive == true);
                        dm.DGHS = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("002") && x.IsActive == true);
                        dm.VP = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("003") && x.IsActive == true);
                        dm.PU = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("004") && x.IsActive == true);
                        dm.AdminOfT = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("005") && x.IsActive == true);
                        dm.AdminOfD = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("009") && x.IsActive == true);
                        dm.AdminOfDist = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("010") && x.IsActive == true);
                        dm.DHQ = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("011") && x.IsActive == true);
                        dm.THQ = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("012") && x.IsActive == true);
                        dm.RHC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("013") && x.IsActive == true);
                        dm.BHU = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("014") && x.IsActive == true);
                        dm.Disp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("015") && x.IsActive == true);
                        dm.sph = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("016") && x.IsActive == true);
                        dm.mch = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("017") && x.IsActive == true);
                        dm.trauma = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("021") && x.IsActive == true);
                        dm.mobDisp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("022") && x.IsActive == true);
                        dm.townHsp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("023") && x.IsActive == true);
                        dm.eyeHsp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("024") && x.IsActive == true);
                        dm.tbClinic = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("025") && x.IsActive == true);
                        dm.tbinstitute = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("026") && x.IsActive == true);
                        dm.tbFlysq = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("027") && x.IsActive == true);
                        dm.tbPolhsp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("028") && x.IsActive == true);
                        dm.tbCivilhsp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("029") && x.IsActive == true);
                        dm.tbFilterC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("030") && x.IsActive == true);
                        dm.tbDHDC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("032") && x.IsActive == true);
                        dm.tbMCEU = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("033") && x.IsActive == true);
                        dm.tbSchOP = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("034") && x.IsActive == true);
                        dm.tbMH = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("036") && x.IsActive == true);
                        dm.tbIDH = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("037") && x.IsActive == true);
                        dm.tbDentalC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("038") && x.IsActive == true);
                        dm.tbCMC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("039") && x.IsActive == true);
                        dm.tbSHC = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("040") && x.IsActive == true);

                        dm.ADceo = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("049") && x.IsActive == true);
                        dm.ADdohp = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("050") && x.IsActive == true);
                        dm.ADdohms = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("051") && x.IsActive == true);
                        dm.ADdohhrmis = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("054") && x.IsActive == true);
                        dm.ADdcirmnch = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("052") && x.IsActive == true);
                        dm.ADdmphfmc = await db.HFDetails.CountAsync(x => x.DivisionCode.Equals(division.Code) && x.HFTypeCode.Equals("053") && x.IsActive == true);

                        totalDHQ += dm.DHQ;
                        totalTHQ += dm.THQ;
                        totalRHC += dm.RHC;
                        totalBHU += dm.BHU;
                        totalDisp += dm.Disp;
                        totalsph += dm.sph;
                        totalmch += dm.mch;
                        totaltrauma += dm.trauma;
                        totalmobDisp += dm.mobDisp;
                        totaltownHsp += dm.townHsp;
                        totaleyeHsp += dm.eyeHsp;
                        totaltbClinic += dm.tbClinic;
                        totaltbinstitute += dm.tbinstitute;
                        totaltbFlysq += dm.tbFlysq;
                        totaltbPolhsp += dm.tbPolhsp;
                        totaltbCivilhsp += dm.tbCivilhsp;
                        totaltbFilterC += dm.tbFilterC;
                        totaltbDHDC += dm.tbDHDC;
                        totaltbMCEU += dm.tbMCEU;
                        totaltbSchOP += dm.tbSchOP;
                        totaltbMH += dm.tbMH;
                        totaltbIDH += dm.tbIDH;
                        totaltbDentalC += dm.tbDentalC;
                        totaltbCMC += dm.tbCMC;
                        totaltbSHC += dm.tbSHC;
                        totalPSHCD += dm.PSHCD;
                        totalDGHS += dm.DGHS;
                        totalVP += dm.VP;
                        totalPU += dm.PU;
                        totalAdminOfT += dm.AdminOfT;
                        totalAdminOfD += dm.AdminOfD;
                        totalAdminOfDist += dm.AdminOfDist;

                        totalADceo += dm.ADceo;
                        totalADdohp += dm.ADdohp;
                        totalADdohms += dm.ADdohms;
                        totalADdohhrmis += dm.ADdohhrmis;
                        totalADdcirmnch += dm.ADdcirmnch;
                        totalADdmphfmc += dm.ADdmphfmc;

                        dmList.Add(dm);
                    }
                    DashboardModel dmTotal = new DashboardModel();
                    dmTotal.Name = "Total";
                    dmTotal.PSHCD = totalPSHCD;
                    dmTotal.DGHS = totalDGHS;
                    dmTotal.VP = totalVP;
                    dmTotal.PU = totalPU;
                    dmTotal.AdminOfT = totalAdminOfT;
                    dmTotal.AdminOfD = totalAdminOfD;
                    dmTotal.AdminOfDist = totalAdminOfDist;
                    dmTotal.DHQ = totalDHQ;
                    dmTotal.THQ = totalTHQ;
                    dmTotal.RHC = totalRHC;
                    dmTotal.BHU = totalBHU;
                    dmTotal.Disp = totalDisp;
                    dmTotal.sph = totalsph;
                    dmTotal.mch = totalmch;
                    dmTotal.trauma = totaltrauma;
                    dmTotal.mobDisp = totalmobDisp;
                    dmTotal.townHsp = totaltownHsp;
                    dmTotal.eyeHsp = totaleyeHsp;
                    dmTotal.tbClinic = totaltbClinic;
                    dmTotal.tbinstitute = totaltbinstitute;
                    dmTotal.tbFlysq = totaltbFlysq;
                    dmTotal.tbPolhsp = totaltbPolhsp;
                    dmTotal.tbCivilhsp = totaltbCivilhsp;
                    dmTotal.tbFilterC = totaltbFilterC;
                    dmTotal.tbDHDC = totaltbDHDC;
                    dmTotal.tbMCEU = totaltbMCEU;
                    dmTotal.tbSchOP = totaltbSchOP;
                    dmTotal.tbMH = totaltbMH;
                    dmTotal.tbIDH = totaltbIDH;
                    dmTotal.tbDentalC = totaltbDentalC;
                    dmTotal.tbCMC = totaltbCMC;
                    dmTotal.tbSHC = totaltbSHC;

                    dmTotal.ADceo = totalADceo;
                    dmTotal.ADdohp = totalADdohp;
                    dmTotal.ADdohms = totalADdohms;
                    dmTotal.ADdohhrmis = totalADdohhrmis;
                    dmTotal.ADdcirmnch = totalADdcirmnch;
                    dmTotal.ADdmphfmc = totalADdmphfmc;

                    dmList.Add(dmTotal);
                }
                else if (level == 2)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (District district in db.Districts.Where(x => x.Code.StartsWith(code)).ToList())
                    {
                        DashboardModel dm = new DashboardModel();
                        dm.Name = district.Name;
                        dm.PSHCD = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("001"));
                        dm.DGHS = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("002"));
                        dm.VP = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("003"));
                        dm.PU = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("004"));
                        dm.AdminOfT = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("005"));
                        dm.AdminOfD = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("009"));
                        dm.AdminOfDist = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("010"));
                        dm.DHQ = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("011"));
                        dm.THQ = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("012"));
                        dm.RHC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("013"));
                        dm.BHU = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("014"));
                        dm.Disp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("015"));
                        dm.sph = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("016"));
                        dm.mch = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("017"));
                        dm.trauma = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("021"));
                        dm.mobDisp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("022"));
                        dm.townHsp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("023"));
                        dm.eyeHsp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("024"));
                        dm.tbClinic = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("025"));
                        dm.tbinstitute = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("026"));
                        dm.tbFlysq = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("027"));
                        dm.tbPolhsp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("028"));
                        dm.tbCivilhsp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("029"));
                        dm.tbFilterC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("030"));
                        dm.tbDHDC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("032"));
                        dm.tbMCEU = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("033"));
                        dm.tbSchOP = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("034"));
                        dm.tbMH = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("036"));
                        dm.tbIDH = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("037"));
                        dm.tbDentalC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("038"));
                        dm.tbCMC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("039"));
                        dm.tbSHC = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("040"));

                        dm.ADceo = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("049") && x.IsActive == true);
                        dm.ADdohp = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("050") && x.IsActive == true);
                        dm.ADdohms = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("051") && x.IsActive == true);
                        dm.ADdohhrmis = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("054") && x.IsActive == true);
                        dm.ADdcirmnch = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("052") && x.IsActive == true);
                        dm.ADdmphfmc = await db.HFDetails.CountAsync(x => x.DistrictCode.Equals(district.Code) && x.HFTypeCode.Equals("053") && x.IsActive == true);

                        totalDHQ += dm.DHQ;
                        totalTHQ += dm.THQ;
                        totalRHC += dm.RHC;
                        totalBHU += dm.BHU;
                        totalDisp += dm.Disp;
                        totalsph += dm.sph;
                        totalmch += dm.mch;
                        totaltrauma += dm.trauma;
                        totalmobDisp += dm.mobDisp;
                        totaltownHsp += dm.townHsp;
                        totaleyeHsp += dm.eyeHsp;
                        totaltbClinic += dm.tbClinic;
                        totaltbinstitute += dm.tbinstitute;
                        totaltbFlysq += dm.tbFlysq;
                        totaltbPolhsp += dm.tbPolhsp;
                        totaltbCivilhsp += dm.tbCivilhsp;
                        totaltbFilterC += dm.tbFilterC;
                        totaltbDHDC += dm.tbDHDC;
                        totaltbMCEU += dm.tbMCEU;
                        totaltbSchOP += dm.tbSchOP;
                        totaltbMH += dm.tbMH;
                        totaltbIDH += dm.tbIDH;
                        totaltbDentalC += dm.tbDentalC;
                        totaltbCMC += dm.tbCMC;
                        totaltbSHC += dm.tbSHC;
                        totalPSHCD += dm.PSHCD;
                        totalDGHS += dm.DGHS;
                        totalVP += dm.VP;
                        totalPU += dm.PU;
                        totalAdminOfT += dm.AdminOfT;
                        totalAdminOfD += dm.AdminOfD;
                        totalAdminOfDist += dm.AdminOfDist;

                        totalADceo += dm.ADceo;
                        totalADdohp += dm.ADdohp;
                        totalADdohms += dm.ADdohms;
                        totalADdohhrmis += dm.ADdohhrmis;
                        totalADdcirmnch += dm.ADdcirmnch;
                        totalADdmphfmc += dm.ADdmphfmc;

                        dmList.Add(dm);
                    }
                }
                else if (level == 3)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (Tehsil tehsil in db.Tehsils.Where(x => x.Code.StartsWith(code)).ToList())
                    {
                        DashboardModel dm = new DashboardModel();
                        dm.Name = tehsil.Name;
                        dm.PSHCD = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("001"));
                        dm.DGHS = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("002"));
                        dm.VP = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("003"));
                        dm.PU = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("004"));
                        dm.AdminOfT = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("005"));
                        dm.AdminOfD = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("009"));
                        dm.AdminOfDist = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("010"));
                        dm.DHQ = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("011"));
                        dm.THQ = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("012"));
                        dm.RHC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("013"));
                        dm.BHU = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("014"));
                        dm.Disp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("015"));
                        dm.sph = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("016"));
                        dm.mch = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("017"));
                        dm.trauma = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("021"));
                        dm.mobDisp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("022"));
                        dm.townHsp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("023"));
                        dm.eyeHsp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("024"));
                        dm.tbClinic = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("025"));
                        dm.tbinstitute = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("026"));
                        dm.tbFlysq = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("027"));
                        dm.tbPolhsp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("028"));
                        dm.tbCivilhsp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("029"));
                        dm.tbFilterC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("030"));
                        dm.tbDHDC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("032"));
                        dm.tbMCEU = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("033"));
                        dm.tbSchOP = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("034"));
                        dm.tbMH = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("036"));
                        dm.tbIDH = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("037"));
                        dm.tbDentalC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("038"));
                        dm.tbCMC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("039"));
                        dm.tbSHC = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("040"));

                        dm.ADceo = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("049") && x.IsActive == true);
                        dm.ADdohp = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("050") && x.IsActive == true);
                        dm.ADdohms = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("051") && x.IsActive == true);
                        dm.ADdohhrmis = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("054") && x.IsActive == true);
                        dm.ADdcirmnch = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("052") && x.IsActive == true);
                        dm.ADdmphfmc = await db.HFDetails.CountAsync(x => x.TehsilCode.Equals(tehsil.Code) && x.HFTypeCode.Equals("053") && x.IsActive == true);


                        totalDHQ += dm.DHQ;
                        totalTHQ += dm.THQ;
                        totalRHC += dm.RHC;
                        totalBHU += dm.BHU;
                        totalDisp += dm.Disp;
                        totalsph += dm.sph;
                        totalmch += dm.mch;
                        totaltrauma += dm.trauma;
                        totalmobDisp += dm.mobDisp;
                        totaltownHsp += dm.townHsp;
                        totaleyeHsp += dm.eyeHsp;
                        totaltbClinic += dm.tbClinic;
                        totaltbinstitute += dm.tbinstitute;
                        totaltbFlysq += dm.tbFlysq;
                        totaltbPolhsp += dm.tbPolhsp;
                        totaltbCivilhsp += dm.tbCivilhsp;
                        totaltbFilterC += dm.tbFilterC;
                        totaltbDHDC += dm.tbDHDC;
                        totaltbMCEU += dm.tbMCEU;
                        totaltbSchOP += dm.tbSchOP;
                        totaltbMH += dm.tbMH;
                        totaltbIDH += dm.tbIDH;
                        totaltbDentalC += dm.tbDentalC;
                        totaltbCMC += dm.tbCMC;
                        totaltbSHC += dm.tbSHC;
                        totalPSHCD += dm.PSHCD;
                        totalDGHS += dm.DGHS;
                        totalVP += dm.VP;
                        totalPU += dm.PU;
                        totalAdminOfT += dm.AdminOfT;
                        totalAdminOfD += dm.AdminOfD;
                        totalAdminOfDist += dm.AdminOfDist;

                        totalADceo += dm.ADceo;
                        totalADdohp += dm.ADdohp;
                        totalADdohms += dm.ADdohms;
                        totalADdohhrmis += dm.ADdohhrmis;
                        totalADdcirmnch += dm.ADdcirmnch;
                        totalADdmphfmc += dm.ADdmphfmc;
                        dmList.Add(dm);
                    }
                }


                return Ok(new
                {
                    result = true,
                    data = dmList,
                    totals = new
                    {
                        totalDHQ,
                        totalTHQ,
                        totalRHC,
                        totalBHU,
                        totalDisp,
                        totalsph,
                        totalmch,
                        totaltrauma,
                        totalmobDisp,
                        totaltownHsp,
                        totaleyeHsp,
                        totaltbClinic,
                        totaltbinstitute,
                        totaltbFlysq,
                        totaltbPolhsp,
                        totaltbCivilhsp,
                        totaltbFilterC,
                        totaltbDHDC,
                        totaltbMCEU,
                        totaltbSchOP,
                        totaltbMH,
                        totaltbIDH,
                        totaltbDentalC,
                        totaltbCMC,
                        totaltbSHC,
                        totalPSHCD,
                        totalDGHS,
                        totalVP,
                        totalPU,
                        totalAdminOfT,
                        totalAdminOfD,
                        totalAdminOfDist,
                        totalADceo,
                        totalADdohp,
                        totalADdohms,
                        totalADdohhrmis,
                        totalADdcirmnch,
                        totalADdmphfmc

            }
                });
            }
        }

        [Route("GetDashboardStaffStatus/{level}/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDashboardStaffStatus(int level, string code)
        {
            using (var db = new HR_System())
            {
                List<DashboardStatffStatus> dssList = new List<DashboardStatffStatus>();
                int totalProfiles = 0, totalSantctioned = 0, totalFilled = 0, totalvacant = 0;

                if (level == 1)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (Division division in db.Divisions.ToList())
                    {
                        DashboardStatffStatus dss = new DashboardStatffStatus();
                        dss.Name = division.Name;
                        dss.Profiles = await db.ProfileDetailsViews.CountAsync(x => x.District.Equals(division.Name));
                        foreach (var item in db.VpMViews.Where(x => x.HFMISCode.StartsWith(division.Code)).ToList())
                        {
                            dss.Santctioned += item.TotalSanctioned;
                            dss.Filled += Convert.ToInt32(item.TotalWorking == null ? 0 : item.TotalWorking);
                            dss.vacant = dss.Santctioned - dss.Filled;
                        }

                        totalProfiles += dss.Profiles;
                        totalSantctioned += dss.Santctioned;
                        totalFilled += dss.Filled;
                        totalvacant += dss.vacant;

                        dssList.Add(dss);
                    }
                    DashboardStatffStatus dssTotal = new DashboardStatffStatus();
                    dssTotal.Name = "Total";
                    dssTotal.Profiles = totalProfiles;
                    dssTotal.Santctioned = totalSantctioned;
                    dssTotal.Filled = totalFilled;
                    dssTotal.vacant = totalSantctioned - totalFilled;

                    dssList.Add(dssTotal);
                }
                else if (level == 2)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (District district in db.Districts.Where(x => x.Code.StartsWith(code)).ToList())
                    {
                        DashboardStatffStatus dss = new DashboardStatffStatus();
                        dss.Profiles = await db.ProfileDetailsViews.CountAsync(x => x.District.Equals(district.Name));
                        foreach (var item in db.VpMViews.Where(x => x.HFMISCode.StartsWith(district.Code)).ToList())
                        {
                            dss.Santctioned += item.TotalSanctioned;
                            dss.Filled += Convert.ToInt32(item.TotalWorking == null ? 0 : item.TotalWorking);
                            dss.vacant = dss.Santctioned - dss.Filled;
                        }
                        totalProfiles += dss.Profiles;
                        totalSantctioned += dss.Santctioned;
                        totalFilled += dss.Filled;
                        totalvacant += dss.vacant;

                        dssList.Add(dss);
                    }

                    DashboardStatffStatus dssTotal = new DashboardStatffStatus();
                    dssTotal.Name = "Total";
                    dssTotal.Profiles = totalProfiles;
                    dssTotal.Santctioned = totalSantctioned;
                    dssTotal.Filled = totalFilled;
                    dssTotal.vacant = totalSantctioned - totalFilled;
                    dssList.Add(dssTotal);

                }
                else if (level == 3)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (Tehsil tehsil in db.Tehsils.Where(x => x.Code.StartsWith(code)).ToList())
                    {
                        DashboardStatffStatus dss = new DashboardStatffStatus();
                        dss.Name = tehsil.Name;
                        dss.Profiles = await db.ProfileDetailsViews.CountAsync(x => x.Tehsil.Equals(tehsil.Name));
                        foreach (var item in db.VpMViews.Where(x => x.HFMISCode.StartsWith(tehsil.Code)).ToList())
                        {
                            dss.Santctioned += item.TotalSanctioned;
                            dss.Filled += Convert.ToInt32(item.TotalWorking == null ? 0 : item.TotalWorking);
                            dss.vacant = dss.Santctioned - dss.Filled;
                        }
                        totalProfiles += dss.Profiles;
                        totalSantctioned += dss.Santctioned;
                        totalFilled += dss.Filled;
                        totalvacant += dss.vacant;

                        dssList.Add(dss);
                    }

                    DashboardStatffStatus dssTotal = new DashboardStatffStatus();
                    dssTotal.Name = "Total";
                    dssTotal.Profiles = totalProfiles;
                    dssTotal.Santctioned = totalSantctioned;
                    dssTotal.Filled = totalFilled;
                    dssTotal.vacant = totalSantctioned - totalFilled;
                    dssList.Add(dssTotal);
                }
                return Ok(new
                {
                    result = true,
                    data = dssList,
                    totals = new
                    {
                        totalProfiles,
                        totalSantctioned,
                        totalFilled
                    }
                });
            }
        }

        [Route("GetHFBlocks/{HF_Id}")]
        [HttpGet]
        [ResponseType(typeof(HFBlock))]
        public async Task<List<HFBlock>> GetHFBlocks(int HF_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hFBlocks = await db.HFBlocks
                    .Where(x => x.HF_Id == HF_Id)
                    .ToListAsync();
                return hFBlocks;
            }
        }
        [Route("RmvHFBlocks/{id}")]
        [HttpGet]
        [ResponseType(typeof(HFBlock))]
        public async Task<IHttpActionResult> RmvHFBlocks(int id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hFBlock = await db.HFBlocks.FindAsync(id);
                if (hFBlock == null)
                {
                    return NotFound();
                }

                db.HFBlocks.Remove(hFBlock);
                await db.SaveChangesAsync();

                return Ok(new { result = true });
            }
        }
        [Route("GetHFWards/{HF_Id}")]
        [HttpGet]
        [ResponseType(typeof(HFWard))]
        public async Task<List<HFWard>> GetHFWards(int HF_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hFWards = await db.HFWards
                    .Where(x => x.HF_Id == HF_Id)
                    .ToListAsync();
                return hFWards;
            }
        }
        [Route("RmvHFWards/{id}")]
        [HttpGet]
        [ResponseType(typeof(HFWard))]
        public async Task<IHttpActionResult> RmvHFWards(int id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hFWard = await db.HFWards.FindAsync(id);
                if (hFWard == null)
                {
                    return NotFound();
                }

                db.HFWards.Remove(hFWard);
                await db.SaveChangesAsync();
                return Ok(new { result = true });
            }
        }
        [Route("GetHFService/{HF_Id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHFService(int HF_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hFServices = await db.HFServices
                    .Where(x => x.HF_Id == HF_Id).Include(x => x.Service).Select(x => new { x.Service.Name, x.HF_Id, x.HfmisCode, x.Services_Id })
                    .ToListAsync();
                return Ok(hFServices);
            }
        }
        [Route("RmvHFService/{id}/{HF_Id}")]
        [HttpGet]
        [ResponseType(typeof(HFService))]
        public async Task<IHttpActionResult> RmvHFService(int id, int HF_Id)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<HFService> hfsf = await db.HFServices.Where(x => x.HF_Id == HF_Id).ToListAsync();
                foreach (var hfservice in hfsf)
                {
                    if (hfservice.Services_Id == id)
                    {
                        db.HFServices.Remove(hfservice);
                    }
                }
                await db.SaveChangesAsync();
                return Ok(new { result = true });
            }
        }

        [Route("GetEmpModes")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(HrEmpMode))]
        public async Task<List<HrEmpMode>> GetEmpModes()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var empModes = await db.HrEmpModes.ToListAsync();
                return empModes;
            }
        }
        [Route("GetDepartments")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(HrDepartment))]
        public async Task<List<HrDepartment>> GetDepartments()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrDepartment = await db.HrDepartments.ToListAsync();
                return hrDepartment;
            }
        }
        [Route("getServices")]
        [HttpGet]
        [ResponseType(typeof(Service))]
        public async Task<List<Service>> GetServices()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var services = await db.Services.ToListAsync();
                return services;
            }
        }
        [Route("GetDomiciles")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetDomiciles()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var domiciles = await db.Domiciles.Where(x => x.ProvinceName.Equals("Punjab")).ToListAsync();
                return Ok(domiciles);
            }
        }
        [Route("GetLanguages")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLanguages()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var languages = await db.HrLanguages.ToListAsync();
                return Ok(languages);
            }
        }
        [Route("GetCourses")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCourses()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var languages = await db.Courses.ToListAsync();
                return Ok(languages);
            }
        }
        [Route("GetSpecializations")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSpecializations()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var specilizations = await db.Specializations.ToListAsync();
                return Ok(specilizations);
            }
        }
        [Route("GetQualifications")]
        [HttpGet]
        public async Task<IHttpActionResult> GetQualifications()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var qualifications = await db.Qualifications.ToListAsync();
                return Ok(qualifications);
            }
        }

        [Route("VPQuery")]
        [HttpGet]
        public async Task<IHttpActionResult> VPQuery()
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    List<HFVPStatus> HFVPStatuses = new List<HFVPStatus>();
                    foreach (var district in db.Districts.Where(x => x.Code.StartsWith("035")))
                    {
                        List<HFDetail> HFs = await db.HFDetails.Where(x => x.HFMISCode.StartsWith(district.Code) && x.IsActive == true).ToListAsync();
                        foreach (var hf in HFs)
                        {
                            List<VpMView> vpmViews = await db.VpMViews.Where(x => x.HFMISCode.Equals(hf.HFMISCode)).ToListAsync();
                            HFVPStatus hfVPStatus = new HFVPStatus();
                            foreach (var vpmView in vpmViews)
                            {
                                hfVPStatus.District = vpmView.HFMISCode.Substring(0, 6);
                                hfVPStatus.HFName = vpmView.HFName;
                                if (vpmView.BPS > 16)
                                {
                                    hfVPStatus.Gazatted++;
                                }
                                else if (vpmView.BPS <= 16)
                                {
                                    hfVPStatus.NonGazatted++;
                                }
                            }
                            if (hfVPStatus.HFName != null)
                            {
                                HFVPStatuses.Add(hfVPStatus);
                            }
                            else if (hfVPStatus.HFName == null)
                            {
                                hfVPStatus.District = hf.DistrictCode;
                                hfVPStatus.HFName = hf.FullName;
                                HFVPStatuses.Add(hfVPStatus);
                            }
                        }
                    }
                    return Ok(HFVPStatuses);
                }
                catch (Exception ex)
                {

                    return Ok(ex.Message);

                }
            }
        }
        [Route("VPQuery2/{districtCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> VPQuery2(string districtCode)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var BHUs = await db.HFDetails.Where(x => x.HFMISCode.StartsWith(districtCode)
                                                        && x.HFTypeCode.Equals("014") && x.IsActive == true).ToListAsync();

                int BHUwithDoc = 0;
                int BHUwithoutDoc = 0;
                foreach (var BHU in BHUs)
                {
                    List<VpMView> vpms = await db.VpMViews.Where(x => x.HFMISCode.Equals(BHU.HFMISCode) &&
                                                        (
                                                            x.Desg_Id == 1320
                                                            ||
                                                            x.Desg_Id == 802
                                                            ||
                                                            x.Desg_Id == 1085
                                                            ||
                                                            x.Desg_Id == 21
                                                            ||
                                                            x.Desg_Id == 22
                                                        )).ToListAsync();

                    int totalDoctors = 0;
                    foreach (var vpm in vpms)
                    {
                        totalDoctors += (int)vpm.TotalWorking;
                    }
                    if (totalDoctors > 0)
                    {
                        BHUwithDoc++;
                    }
                    else if (totalDoctors == 0)
                    {
                        BHUwithoutDoc++;
                    }
                }

                var RHCs = await db.HFDetails.Where(x => x.HFMISCode.StartsWith(districtCode)
                                                        && x.HFTypeCode.Equals("013") && x.IsActive == true).ToListAsync();

                int RHCwithDoc = 0;
                int RHCwithoutDoc = 0;
                foreach (var RHC in RHCs)
                {
                    List<VpMView> vpms = await db.VpMViews.Where(x => x.HFMISCode.Equals(RHC.HFMISCode) &&
                                                        (
                                                            x.Desg_Id == 1320
                                                            ||
                                                            x.Desg_Id == 802
                                                            ||
                                                            x.Desg_Id == 1085
                                                            ||
                                                            x.Desg_Id == 21
                                                            ||
                                                            x.Desg_Id == 22
                                                        )).ToListAsync();

                    int totalDoctors = 0;
                    foreach (var vpm in vpms)
                    {
                        totalDoctors += (int)vpm.TotalWorking;
                    }
                    if (totalDoctors > 0)
                    {
                        RHCwithDoc++;
                    }
                    else if (totalDoctors == 0)
                    {
                        RHCwithoutDoc++;
                    }
                }
                return Ok(new
                {
                    BHUs = BHUs.Count,
                    BHUwithDoc = BHUwithDoc,
                    BHUwithoutDoc = BHUwithoutDoc,
                    RHCs = RHCs.Count,
                    RHCwithDoc = RHCwithDoc,
                    RHCwithoutDoc = RHCwithoutDoc,
                });
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
    }
    public class HFVPStatus
    {
        public string District { get; set; }
        public string HFName { get; set; }
        public int Gazatted { get; set; }
        public int NonGazatted { get; set; }
    }
    public partial class HrDesignationVecancyView
    {
        public int id { get; set; }

        public int code { get; set; }
        public string name { get; set; }

    }
}
