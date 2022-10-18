using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/DashboardHrmis")]
    public class DashboardHrmisController : ApiController
    {
        private readonly DashboardService _dashboardService;

        public DashboardHrmisController()
        {
            _dashboardService = new DashboardService();
        }

        [Authorize]
        [HttpGet]
        [Route("GetDashboardHrmis")]
        public IHttpActionResult GetDashboardHrmis()
        {
            try
            {
                using (var _db = new HR_System())
                {

                    var uxer = User.Identity.GetUserName();
                    SaveLoggedInInfo(uxer,"From Portal", "GetDashboardHrmis", null);

                    _db.Configuration.ProxyCreationEnabled = false;
                    List<usp_HFTypeWiseReport_Result> res = new List<usp_HFTypeWiseReport_Result>();
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var userRole = UserManager.GetRoles(User.Identity.GetUserId());
                    var userService = new UserService();
                    var user = userService.GetUser(User.Identity.GetUserId());
                    var hfmisCode = user.hfmiscode;
                    if (userRole.FirstOrDefault().Equals("South Punjab"))
                    {
                        var southHfCode = "1793"; 
                         res = _db.usp_HFTypeWiseReport(southHfCode).ToList();
                    }
                    else
                    {
                        res = _db.usp_HFTypeWiseReport(hfmisCode).ToList();
                    }
                 

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        


        public void SaveLoggedInInfo(string userId, string remarks, string remoteIp = null, string remoteDetail = null)
        {

            try
            {
                using (var db = new HR_System())
                {
                    string ip = HttpContext.Current.Request.UserHostAddress;
                    string httpXForwarded = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    string remote = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string ip5 = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"];
                    db.LoggedInLogs.Add(new LoggedInLog()
                    {
                        IPAddress = ip,
                        LoggedInDate = DateTime.Now,
                        UserId = userId,
                        ForwardedIPAddress = getIPAddress(HttpContext.Current.Request),
                        Browser = HttpContext.Current.Request.UserAgent,
                        //Remarks = remarks,
                        HttpXForwardedFor = httpXForwarded,
                        RemoteAddress = remote,
                        RemoteIP = remoteIp,
                        RemoteDetailJSON = remoteDetail
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }


        public static string getIPAddress(HttpRequest request)
        {
            string szIP = null;
            string szRemoteAddr = request.UserHostAddress;
            try
            {
                string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (szXForwardedFor == null)
                {
                    szIP = szRemoteAddr;
                }
                else
                {
                    szIP = szXForwardedFor;
                    if (szIP.IndexOf(",") > 0)
                    {
                        string[] arIPs = szIP.Split(',');

                        //foreach (string item in arIPs)
                        //{
                        //    if (!IsPrivateIpAddress(item))
                        //    {
                        //        return item;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return szIP;
        }




        [Route("DashboardFcAppFwdCount")]
        [HttpPost]
        public IHttpActionResult DashboardFcAppFwdCount([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Database.CommandTimeout = 0;
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
                    //var d = db.SP_OFC_WISE().ToList();
                    //var total = db.SP_TOT_OFC_WISE().ToList();
                    var d = db.SP_OFC_WISE_WITH_DATE(filter.From, filter.To, filter.Program ?? "").OrderBy(k => k.OrderBy).ToList();
                    var dept = db.SP_DEPT_WITH_DATE_PROGRAM(filter.From, filter.To, filter.Program ?? "").OrderBy(k => k.OrderBy).ToList();
                    //var d = db.SP_OFC_WISE_WITH_DATE(filter.From, filter.To, filter.Program ?? "").OrderByDescending(k => k.UnderProcess).ThenBy(p => p.OfficerDesignation).ToList();
                    var total = db.SP_TOT_OFC_WISE_WITH_DATE(filter.From, filter.To, filter.Program ?? "").ToList();
                    return Ok(new { d, total, dept });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("DashboardPendencyCount")]
        [HttpPost]
        public IHttpActionResult DashboardPendencyCount([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    var data = DashboardService.Getpendency();
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
                    if (filter.TypeId > 0)
                    {
                        var dept = db.SP_DEPT_PENDENCY_WITH_DATE_TYPE(filter.TypeId, filter.From, filter.To, filter.Program ?? "").OrderBy(k => k.OrderBy).ToList();
                        return Ok(new { dept, data });
                    }
                    else
                    {
                        var dept = db.SP_DEPT_PENDENCY_WITH_DATE(filter.From, filter.To, filter.Program ?? "").OrderBy(k => k.OrderBy).ToList();
                        return Ok(new { dept, data });
                    }
                   
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [AllowAnonymous]
        [Route("DashboardPendency3")]
        [HttpPost]
        public IHttpActionResult DashboardPendency3([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Database.CommandTimeout = 0;
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
                    List<string> officers = new List<string>()
                    {
                        "Director Legal",
                        "Statistical Officer (HISDU)",
                        "Senior Consultant",
                        "Section (VERTICAL PROGRAM)",
                        "Manager (MIS)",
                        "R & I Branch",
                        "Section (G-II)",
                        "HR Cell - HISDU",
                        "Consultant Finance",
                        "Director General Health Services",
                        "Facilitation Centre (HISDU)",
                        "Legal Consultant (III)",
                        "Order Generation Cell - HISDU",
                        "Parliamentarian Lounge",
                        "Procurement Cell (PMU)",
                        "Project Director (PMU)",
                        "Section (AHP-2)",
                        "Deputy Project Director (PMU)",
                        "Chief Executive Officer",
                        "Districts",
                        "Citizen Portal",
                        "Section (N)",
                        "Senior Law Officer",
                        "Sent Back to Issue Branch",
                        "Online Applicant"
                    };

                    var pendancy = db.uspPendancy5(filter.From, filter.To, null, filter.Type, "").Where(x => !officers.Contains(x.OfficerDesignation)).ToList();
                    return Ok(new { pendancy });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [AllowAnonymous]
        [Route("DashboardPendencyDistrict")]
        [HttpPost]
        public IHttpActionResult DashboardPendencyDistrict([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Database.CommandTimeout = 0;
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
                    List<string> officers = new List<string>()
                    {
                        "Director Legal",
                        "Statistical Officer (HISDU)",
                        "Senior Consultant",
                        "Section (VERTICAL PROGRAM)",
                        "Manager (MIS)",
                        "R & I Branch",
                        "Section (G-II)",
                        "HR Cell - HISDU",
                        "Consultant Finance",
                        "Director General Health Services",
                        "Facilitation Centre (HISDU)",
                        "Legal Consultant (III)",
                        "Order Generation Cell - HISDU",
                        "Parliamentarian Lounge",
                        "Procurement Cell (PMU)",
                        "Project Director (PMU)",
                        "Section (AHP-2)",
                        "Deputy Project Director (PMU)",
                        "Chief Executive Officer",
                        "Districts",
                        "Citizen Portal",
                        "Section (N)",
                        "Senior Law Officer",
                        "Sent Back to Issue Branch",
                        "Online Applicant"
                    };

                    var pendancy = db.uspPendancyDistrictWise(filter.From, filter.To, null, "", "", filter.DistrictCode).Where(x => !officers.Contains(x.OfficerDesignation)).ToList();
                    return Ok(new { pendancy });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [AllowAnonymous]
        [Route("DashboardPendency5")]
        [HttpPost]
        public IHttpActionResult DashboardPendency5([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Database.CommandTimeout = 0;
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
                    List<string> officers = new List<string>()
                    {
                        "Director Legal",
                        "Statistical Officer (HISDU)",
                        "Senior Consultant",
                        "Section (VERTICAL PROGRAM)",
                        "Manager (MIS)",
                        "R & I Branch",
                        "Section (G-II)",
                        "HR Cell - HISDU",
                        "Consultant Finance",
                        "Director General Health Services",
                        "Facilitation Centre (HISDU)",
                        "Legal Consultant (III)",
                        "Order Generation Cell - HISDU",
                        "Parliamentarian Lounge",
                        "Procurement Cell (PMU)",
                        "Project Director (PMU)",
                        "Section (AHP-2)",
                        "Deputy Project Director (PMU)",
                        "Chief Executive Officer",
                        "Districts",
                        "Citizen Portal",
                        "Section (N)",
                        "Senior Law Officer",
                        "Sent Back to Issue Branch",
                        "Online Applicant"
                    };

                    var pendancy = db.uspPendancy4(filter.From, filter.To, null, "", "").Where(x => !officers.Contains(x.OfficerDesignation)).ToList();

                    return Ok();
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetSectionReportNew")]
        [HttpPost]
        public IHttpActionResult GetSectionReportNew([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);

                    //var d = db.SP_OFC_WISE().ToList();
                    //var total = db.SP_TOT_OFC_WISE().ToList();
                    var d = db.uspApplicationTypeWiseOfficer_2(filter.From, filter.To, currentOfficer.Id).ToList();
                    var n = db.fts12(filter.From, filter.To, currentOfficer.Id).ToList();
                    return Ok(new { d, n });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetSectionReportNew22")]
        [HttpPost]
        public IHttpActionResult GetSectionReportNew22([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);

                    //var d = db.SP_OFC_WISE().ToList();
                    //var total = db.SP_TOT_OFC_WISE().ToList();
                    var d = db.uspApplicationTypeWiseOfficer_4(filter.From, filter.To, currentOfficer.Id, filter.SourceId).ToList();
                    var n = db.fts12(filter.From, filter.To, currentOfficer.Id).ToList();
                    return Ok(new { d, n });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("PUCDashboard")]
        [HttpPost]
        public IHttpActionResult PUCDashboard([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    List<string> officers = new List<string>()
                    {
                        "Director Legal",
                        "Statistical Officer (HISDU)",
                        "Senior Consultant",
                        "Section (VERTICAL PROGRAM)",
                        "Manager (MIS)",
                        "R & I Branch",
                        "Section (G-II)",
                        "HR Cell - HISDU",
                        "Consultant Finance",
                        "Director General Health Services",
                        "Facilitation Centre (HISDU)",
                        "Legal Consultant (III)",
                        "Order Generation Cell - HISDU",
                        "Parliamentarian Lounge",
                        "Procurement Cell (PMU)",
                        "Project Director (PMU)",
                        "Section (AHP-2)",
                        "Deputy Project Director (PMU)",
                        "Chief Executive Officer",
                        "Districts",
                        "Citizen Portal",
                        "Section (N)",
                        "Senior Law Officer",
                        "Sent Back to Issue Branch",
                        "Online Applicant"
                    };
                    var report = db.PUCDashboard(filter.From, filter.To, filter.FixedDate).Where(x => !officers.Contains(x.Officer)).OrderBy(x => x.OrderBy).ToList();
                    return Ok(report);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetMySectionReportNew")]
        [HttpPost]
        public IHttpActionResult GetMySectionReportNew([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.Id == filter.OfficerId);

                    //var d = db.SP_OFC_WISE().ToList();
                    //var total = db.SP_TOT_OFC_WISE().ToList();
                    var d = db.uspApplicationTypeWiseOfficer_2(filter.From, filter.To, currentOfficer.Id).ToList();
                    var n = db.fts12(filter.From, filter.To, currentOfficer.Id).ToList();
                    return Ok(new { d, n });
                    //return Ok(db.Database.SqlQuery<DashCountFwdApp>("select * from (select OfficerDesignation, OfficerCode, Count(OfficerDesignation) Total from ApplicationZView  where IsActive = 1 and OfficerDesignation is not null and Len(OfficerCode) = 5 group by OfficerDesignation , OfficerCode union SELECT 'Total Applications Forwarded' A , 0 B, Count(*) C FROM [HR_System].[dbo].[ApplicationZView] where IsActive = 1 and PandSOfficer_Id is not null) as x").OrderByDescending(x => x.Total).Take(11).ToList());
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetPensionCasesReport")]
        [HttpPost]
        public IHttpActionResult GetPensionCasesReport([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.Id == filter.OfficerId);

                    var pensionCases = db.uspPensionCases(filter.From, filter.To, "").ToList();
                    return Ok(pensionCases);
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetCRRReport")]
        [HttpPost]
        public IHttpActionResult GetCRRReport([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    var userId = User.Identity.GetUserId();
                    var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.Id == filter.OfficerId);

                    var crr = db.uspFileRecSectionWise().OrderBy(k => k.Issued).ToList();
                    return Ok(new { crr });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetPostingReport")]
        [HttpGet]
        public IHttpActionResult GetPostingReport()
        {
            using (var db = new HR_System())
            {
                try
                {
                    var res = db.MeritPostingViews.Where(x => x.Designation_Id == 302).GroupBy(x => new { x.PostingHF_Id, x.PostingHFName, x.PostingHFAC }).Select(x => new MeritPostingModel
                    {
                        Id = x.Key.PostingHF_Id,
                        HFName = x.Key.PostingHFName,
                        HFAC = x.Key.PostingHFAC,
                        Count = x.Count()
                    }).OrderByDescending(k => k.HFAC).ThenByDescending(p => p.Count).ToList();
                    return Ok(res);
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetFDOReportTypeWiseDate")]
        [HttpPost]
        public IHttpActionResult GetFDOReportTypeWiseDate([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    //var userId = User.Identity.GetUserId();
                    var d = db.SP_FCTYP_WISE_WITH_DATE_2(filter.From, filter.To, filter.UserId).OrderBy(x => x.ApplicationType).ToList();
                    return Ok(new { d });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetDiaryOfficeWiseDate")]
        [HttpPost]
        public IHttpActionResult GetDiaryOfficeWiseDate([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    //var userId = User.Identity.GetUserId();
                    var d = db.SP_DIARY_OFFICE_WISE_WITH_DATE(filter.From, filter.To, filter.UserId).ToList();
                    return Ok(new { d });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetRiBranchReportOfficeWiseDate")]
        [HttpPost]
        public IHttpActionResult GetRiBranchReportOfficeWiseDate([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    //var userId = User.Identity.GetUserId();
                    var d = db.SP_RI_OFFICE_WISE_WITH_DATE(filter.From, filter.To, filter.UserId).ToList();
                    return Ok(new { d });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetLawwingReportOfficeWiseDate")]
        [HttpPost]
        public IHttpActionResult GetLawwingReportOfficeWiseDate([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    //var userId = User.Identity.GetUserId();
                    var d = db.SP_LAWING_OFFICE_WISE_WITH_DATE(filter.From, filter.To, filter.UserId).ToList();
                    return Ok(new { d });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetCitizenPortalReportOfficeWiseDate")]
        [HttpPost]
        public IHttpActionResult GetCitizenPortalReportOfficeWiseDate([FromBody] FTSFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {
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
                    //var userId = User.Identity.GetUserId();
                    var d = db.SP_CP_OFFICE_WISE_WITH_DATE(filter.From, filter.To, filter.UserId).ToList();
                    return Ok(new { d });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetLawWingReport")]
        [HttpGet]
        public IHttpActionResult GetLawWingReport()
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var rpt = db.LawWingReports.ToList();
                    return Ok(rpt);
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }
        [HttpPost]
        [Route("GetApplications")]
        public IHttpActionResult GetApplications([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_dashboardService.GetApplications(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetETransferDashboard")]
        public IHttpActionResult GetETransferAppVp()
        {
            using (var db = new HR_System())
            {
                try
                {
                    var appVp = db.uspETransferAppVp().ToList();
                    var vp = db.uspETransferVp().ToList();
                    return Ok(new { appVp, vp });
                }

                catch (Exception ex)
                {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                }
            }
        }

        //[HttpPost]
        //[Route("GetApplicationChart")]
        //public IHttpActionResult GetApplicationChart([FromBody] ApplicationFilter filter)
        //{
        //    using (var db = new HR_System())
        //    {
        //        try
        //        {
        //            if (filter.From == null && filter.To == null)
        //            {
        //                filter.From = new DateTime(1970, 1, 1);
        //                filter.To = DateTime.Now.AddDays(1);
        //            }
        //            else if (filter.From != null && filter.To == null)
        //            {
        //                filter.To = DateTime.Now.AddDays(1);
        //            }
        //            else if (filter.From == null && filter.To == null)
        //            {
        //                filter.From = new DateTime(1970, 1, 1);
        //                filter.To = filter.To.Value.AddDays(1);
        //            }
        //            else if (filter.From != null && filter.To != null)
        //            {
        //                filter.To = filter.To.Value.AddDays(1);
        //            }
        //            //var userId = User.Identity.GetUserId();
        //            var d = db.uspApplicationCharts(filter.From, filter.To).ToList();


        //            var apmoQuery = db.APMOPrefsViews.AsQueryable();

        //            if (!string.IsNullOrEmpty(filter.Query))
        //            {
        //                apmoQuery.Where(x => x.FullName.ToLower().StartsWith(filter.Query.ToLower()));
        //            }
        //            int total = apmoQuery.Count();
        //            var apmo = apmoQuery.OrderBy(x => x.CNIC).ThenBy(k => k.DateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
        //            return Ok(new { d, apmo, total });
        //        }

        //        catch (Exception ex)
        //        {
        //            while (ex.InnerException != null)
        //            {
        //                ex = ex.InnerException;
        //            }
        //            Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
        //        }
        //    }
        //}
    }
}
