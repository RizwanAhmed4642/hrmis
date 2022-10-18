using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/LawWing")]
    public class LawWingController : ApiController
    {
        [HttpPost]
        [Route("SubmitCallsList")]
        public IHttpActionResult Submit([FromBody] LawCallsListDto lawCalls)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    LawCallsList lawFile = new LawCallsList();
                    lawFile.CaseNumber = lawCalls.CaseNumber;
                    lawFile.CourtName = lawCalls.CourtName;
                    lawFile.CaseTitle = lawCalls.CaseTitle;
                    lawFile.LastDate = lawCalls.LastDate;
                    lawFile.NextDate = lawCalls.NextDate;
                    lawFile.Section_Id = lawCalls.Section_Id;
                    lawFile.DealingOfficer = lawCalls.DealingOfficer;
                    lawFile.DealingOfficer_Id = lawCalls.DealingOfficer_Id;
                    lawFile.Proceedings = lawCalls.Proceedings;
                    lawFile.Remarks = lawCalls.Remarks;

                    lawFile.IsActive = true;
                    lawFile.Created_Date = DateTime.UtcNow.AddHours(5);
                    lawFile.Created_By = User.Identity.GetUserName();
                    lawFile.Users_Id = User.Identity.GetUserId();
                    _db.LawCallsLists.Add(lawFile);
                    _db.SaveChanges();

                    foreach (var officer in lawCalls.concernedOfficers)
                    {
                        LawCallsListOfficer lawCallsListOfficer = new LawCallsListOfficer();
                        lawCallsListOfficer.CallsList_Id = lawFile.Id;
                        lawCallsListOfficer.Section_Id = officer.Id;
                        _db.LawCallsListOfficers.Add(lawCallsListOfficer);
                        _db.SaveChanges();
                    }
                    return Ok(new { file = lawCalls });
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SubmitCallsListRemarks")]
        public IHttpActionResult SubmitCallsListRemarks([FromBody] LawCallsListRemark lawCallsListRemark)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    lawCallsListRemark.IsActive = true;
                    lawCallsListRemark.Created_Date = DateTime.UtcNow.AddHours(5);
                    lawCallsListRemark.Created_By = User.Identity.GetUserName();
                    lawCallsListRemark.Users_Id = User.Identity.GetUserId();
                    _db.LawCallsListRemarks.Add(lawCallsListRemark);
                    _db.SaveChanges();

                    var officerIds = _db.LawCallsListOfficers.Where(x => x.CallsList_Id == lawCallsListRemark.CallsList_Id).Select(x => x.Section_Id).ToList();
                    var officers = _db.P_SOfficers.Where(x => officerIds.Contains(x.Id)).ToList();
                    var caseT = _db.LawCallsLists.FirstOrDefault(x => x.Id == lawCallsListRemark.CallsList_Id);
                    if (caseT != null)
                    {
                        foreach (var item in officers)
                        {
                            SMS sms = new SMS()
                            {
                                UserId = User.Identity.GetUserId(),
                                FKId = lawCallsListRemark.Id,
                                //MobileNumber = item.Contact,
                                MobileNumber = "03214677763",
                                Message = @"Law Wing Update:\nCase Number: " + caseT.CaseNumber + "\nCase Title: " + caseT.CaseTitle + "\nRemarks: " + lawCallsListRemark.Proceeding
                            };
                            Common.SendSMSTelenor(sms);
                        }
                    }
                    return Ok(new { file = lawCallsListRemark });
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCallsList")]
        public IHttpActionResult GetCallsList()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (User.IsInRole("Law wing"))
                    {
                        var callsList = _db.LawCallsListViews.Where(x => x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();
                        return Ok(callsList);
                    }
                    else
                    {
                        string userId = User.Identity.GetUserId();
                        var officer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                        if (officer != null)
                        {
                            var callsList = _db.LawCallsListViews.Where(x => x.Section_Id == officer.Id && x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();
                            return Ok(callsList);
                        }
                        else
                        {
                            var callsList = _db.LawCallsListViews.Where(x => x.IsActive == true).Skip(0).Take(0).OrderByDescending(k => k.Created_Date).ToList();
                            return Ok(callsList);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCauseList")]
        public IHttpActionResult GetCauseList([FromBody] LawFilters lawFilters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    //var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

                    DateTime today = DateTime.Now.Date;

                    if (User.IsInRole("Law wing"))
                    {
                        var query = _db.LawCallsListViews.Where(x => x.IsActive == true).AsQueryable();
                        if (lawFilters.Today == true)
                        {
                            query = query.Where(b => b.Created_Date.Value.Year == today.Year && b.Created_Date.Value.Month == today.Month && b.Created_Date.Value.Day == today.Day).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(lawFilters.Query))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().Contains(lawFilters.Query.ToLower()) || x.CaseTitle.ToLower().Contains(lawFilters.Query.ToLower())).AsQueryable();
                        }
                        int count = query.Count();
                        var causeList = query.OrderByDescending(k => k.Created_Date).Skip(lawFilters.Skip).Take(lawFilters.PageSize).ToList();
                        return Ok(new TableResponse<LawCallsListView> { List = causeList, Count = count });
                    }
                    else
                    {
                        string userId = User.Identity.GetUserId();
                        var officer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                        if (officer != null)
                        {
                            var query = _db.LawCallsListViews.Where(x => x.Section_Id == officer.Id && x.IsActive == true).AsQueryable();
                            if (lawFilters.Today == true)
                            {
                                query = query.Where(b => b.Created_Date.Value.Year == today.Year && b.Created_Date.Value.Month == today.Month && b.Created_Date.Value.Day == today.Day).AsQueryable();
                            }
                            if (!string.IsNullOrEmpty(lawFilters.Query))
                            {
                                query = query.Where(x => x.CaseNumber.ToLower().Contains(lawFilters.Query.ToLower()) || x.CaseTitle.ToLower().Contains(lawFilters.Query.ToLower())).AsQueryable();
                            }
                            int count = query.Count();
                            var causeList = query.OrderByDescending(k => k.Created_Date).Skip(lawFilters.Skip).Take(lawFilters.PageSize).ToList();
                            return Ok(new TableResponse<LawCallsListView> { List = causeList, Count = count });
                        }
                        else
                        {
                            return Ok(new TableResponse<object> { List = new List<object>(), Count = 0 });
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetCallsListRemarks/{Id}")]
        public IHttpActionResult GetCallsListRemarks(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    var callsList = _db.LawCallsListRemarks.Where(x => x.CallsList_Id == Id && x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();
                    return Ok(new { List = callsList });
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveCauseList/{id}")]
        public IHttpActionResult RemoveCauseList(int id)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    var callsList = _db.LawCallsLists.FirstOrDefault(x => x.Id == id);
                    if (callsList != null)
                    {
                        callsList.IsActive = false;
                        _db.Entry(callsList).State = EntityState.Modified;
                        _db.SaveChanges();
                        return Ok(true);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetCallsListMobile")]
        public IHttpActionResult GetCallsListMobile()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (User.IsInRole("Law wing"))
                    {
                        var callsList = _db.LawCallsListViews.Where(x => x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();
                        return Ok(new { List = callsList });
                    }
                    else
                    {
                        string userId = User.Identity.GetUserId();
                        var officer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                        if (officer != null)
                        {
                            var callsList = _db.LawCallsListViews.Where(x => x.Section_Id == officer.Id && x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();
                            return Ok(new { List = callsList });
                        }
                        else
                        {
                            var callsList = _db.LawCallsListViews.Where(x => x.IsActive == true).Skip(0).Take(0).OrderByDescending(k => k.Created_Date).ToList();
                            return Ok(new { List = callsList });
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
    }
    public class LawCallsListDto
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; }
        public string CaseTitle { get; set; }
        public string CourtName { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public Nullable<System.DateTime> NextDate { get; set; }
        public Nullable<int> Section_Id { get; set; }
        public string DealingOfficer { get; set; }
        public string DealingOfficer_Id { get; set; }
        public string Proceedings { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Users_Id { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public List<P_SOfficers> concernedOfficers { get; set; }
    }
    public class LawFilters : Paginator
    {
        public string Query { get; set; }
        public string CaseNumber { get; set; }
        public string CaseTitle { get; set; }
        public string CourtName { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public Nullable<System.DateTime> NextDate { get; set; }
        public Nullable<int> Section_Id { get; set; }
        public string DealingOfficer { get; set; }
        public string DealingOfficer_Id { get; set; }
        public string Proceedings { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Users_Id { get; set; }
        public Nullable<bool> Today { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public List<P_SOfficers> concernedOfficers { get; set; }
    }
}
