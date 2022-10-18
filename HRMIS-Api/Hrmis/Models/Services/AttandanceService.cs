using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ImageProcessor;
using Hrmis.Models.ViewModels;
using Hrmis.Models.ViewModels.Application;
using Hrmis.Models.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Models.Services
{
    public class AttandanceService
    {
        private readonly UserService _userService;
        private C_User currentUser;
        public List<SearchResult> searchResults;

        public AttandanceService()
        {
            _userService = new UserService();
            searchResults = new List<SearchResult>();
        }

        public class AttandanceFilter : Paginator
        {
            public List<string> Name { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public Nullable<System.DateTime> LeaveFrom { get; set; }
            public Nullable<System.DateTime> LeaveTo { get; set; }
            public Nullable<System.DateTime> From { get; set; }
            public Nullable<System.DateTime> To { get; set; }
            public string EmployeeName { get; set; }
            public int HrId { get; set; }
            public int IsLate { get; set; }
            public string parm { get; set; }

            public string hfmisCode { get; set; }
            public string searchTerm { get; set; }
            public string roleName { get; set; }
            public int SubDeptID { get; set; }
        }

        //get AttandanceLog
        //public AttandanceDTO GetAttandanceLog(AttandanceFilter filter, string userName, string userId)
        //{
        //    using (var _db = new HR_System())
        //    {
        //        try
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;

        //            //var attandanceLog = _db.Database.SqlQuery<AttandanceList>(@"SELECT CONVERT(NVARCHAR(10), ulog.DateOnlyRecord, 103) AS date_att, usr.Name ,  Time_IN_OUT = STUFF(
        //            //(SELECT ' - ' + CONVERT(varchar(15), CAST(ulog1.DateTimeRecord AS TIME), 100) FROM dbo.UserLog AS ulog1
        //            //WHERE ulog1.DateOnlyRecord = ulog.DateOnlyRecord and ulog1.IndRegID = ulog.IndRegID FOR XML PATH('')), 1, 2, '') 
        //            //FROM dbo.UserInformation AS usr INNER JOIN dbo.UserLog AS ulog ON usr.EnrollNumber = ulog.IndRegID
        //            //group by ulog.DateOnlyRecord, usr.Name,ulog.IndRegID order by ulog.DateOnlyRecord desc").ToList();

        //            IQueryable<UserLogView> query = _db.UserLogViews.AsQueryable();

        //            var attandanceLog = query.GroupBy(x => new { x.IndRegID, x.DateOnlyRecord, x.Name, x.CNIC }).Select(x => new AttandanceList
        //            {
        //                Id = x.Key.IndRegID,
        //                date_att = x.Key.DateOnlyRecord,
        //                Name = x.Key.Name,
        //                CNIC = x.Key.CNIC
        //            }).OrderByDescending(k => k.date_att).ThenBy(y => y.Name).ToList();

        //            var times = query.GroupBy(x => new { x.IndRegID, x.DateOnlyRecord, x.DateTimeRecord }).Select(x => new AttandanceTime
        //            {
        //                Id = x.Key.IndRegID,
        //                date_att = x.Key.DateOnlyRecord,
        //                DateTime = x.Key.DateTimeRecord
        //            }).OrderBy(k => k.DateTime).ToList();
        //            var count = attandanceLog.Count();
        //            var Attlist = attandanceLog.Skip(filter.Skip).Take(filter.PageSize).ToList();
        //            return new AttandanceDTO { AttandanceList = new TableResponse<AttandanceList> { Count = count, List = Attlist }, AttandanceTime = times };

        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}



        public class AttandanceList
        {
            public DateTime? date_att { get; set; }
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Time_IN_OUT { get; set; }
            public string Total_hours { get; set; }
            public string CNIC { get; set; }
        }


        public class AttandanceTime
        {
            public int? Id { get; set; }
            public DateTime? date_att { get; set; }
            public DateTime? DateTime { get; set; }
        }
        public class AttandanceDTO
        {
            public TableResponse<AttandanceList> AttandanceList { get; set; }
            public List<AttandanceTime> AttandanceTime { get; set; }
        }

        //add Leave Record
        public EmpLeaveForm saveLeaveRec(EmpLeaveForm lev, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (lev.ID == 0)
                        {

                            if (lev.CBalance == null || lev.CBalance == 0)
                            {
                                lev.CBalance = 24;
                            }

                            lev.CBalance = lev.CBalance - lev.TotalDays;
                            lev.LeaveEnterDate = DateTime.UtcNow.AddHours(5);
                            var savablelev = BindLeaveSaveModel(lev);
                            if (savablelev == null) return null;
                            _db.EmpLeaveForms.Add(savablelev);

                            //var dbIndRegId = _db.Empattendances.FirstOrDefault(x => x.ProfileID == lev.ProfileID);

                            //var dbLog = _db.UserLogs.FirstOrDefault(x => x.IndRegID == dbIndRegId && x.DateOnlyRecord == lev.)

                            //UserLog ul = new UserLog();
                            //    string leaveStatus = null;

                            //    if (lev.LeaveStatusID == 1)
                            //    {
                            //        leaveStatus = "Sick Leave";
                            //    }
                            //    else if (lev.LeaveStatusID == 1)
                            //    {
                            //        leaveStatus = "Casual Leave";
                            //    }
                            //    else if (lev.LeaveStatusID == 1)
                            //    {
                            //        leaveStatus = "Short Leave";
                            //    }
                            //    ul.LogStatus = leaveStatus;

                            _db.SaveChanges();
                            transc.Commit();
                            return savablelev;
                            // }
                        }

                        else if (LeaveExists(lev.ID))
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var dbleave = _db.EmpLeaveForms.FirstOrDefault(x => x.ID == lev.ID);
                            var editables = bindLeaveEditModel(lev, dbleave);

                            _db.Entry(editables).State = EntityState.Modified;
                            _db.SaveChanges();
                            transc.Commit();
                            return editables;

                        }
                    }
                    catch (Exception ex)
                    {

                        transc.Rollback();
                        throw;
                    }
                }
            }
            return null;
        }

        //hftype database
        public bool LeaveExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var leave = _db.EmpLeaveForms.FirstOrDefault(x => x.ID == Id);
                    if (leave == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        ////get Leave list
        //public TableResponse<View_LeaveRecords> GetLeaveList(AttandanceFilter filter, string userName, string userId)
        //{
        //    using (var _db = new HR_System())
        //    {
        //        try
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;
        //            var LeaveList = _db.View_LeaveRecords.AsQueryable();

        //            var count = LeaveList.Count();
        //            var list = LeaveList.OrderByDescending(x => x.LeaveEnterDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
        //            return new TableResponse<View_LeaveRecords>() { Count = count, List = list };

        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}
        ////end get list


        //get Leave list
        public TableResponse<View_LeaveRecords> GetLeaveList(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var LeaveList = _db.View_LeaveRecords.AsQueryable();

                    if (filter.LeaveFrom != null)
                    {
                        LeaveList = LeaveList.Where(x => x.LeaveStart >= filter.LeaveFrom);
                    }
                    if (filter.LeaveTo != null)
                    {
                        LeaveList = LeaveList.Where(x => x.LeaveEnd <= filter.LeaveTo);
                    }
                    if (filter.EmployeeName != null)
                    {
                        LeaveList = LeaveList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
                    }

                    var count = LeaveList.Count();
                    var list = LeaveList.OrderByDescending(x => x.LeaveEnterDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    //    var list = LeaveList.OrderBy(x => x.SubDept_ID).OrderByDescending(x => x.IsHOD).ThenByDescending(x => x.LeaveEnterDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<View_LeaveRecords>() { Count = count, List = list };

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //end get list


        ////get attandance list
        //public TableResponse<HisduEmpDailyAttancucsSummery> getAttendanceList(AttandanceFilter filter, string userName, string userId)
        //{
        //    using (var _db = new HR_System())
        //    {
        //        try
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;
        //            var AttendanceList = _db.HisduEmpDailyAttancucsSummeries.AsNoTracking().AsQueryable();
        //            //     View_MonthlyAttendance
        //            if (filter.From != null)
        //            {
        //                AttendanceList = AttendanceList.Where(x => x.LogDate >= filter.From);
        //            }
        //            if (filter.To != null)
        //            {
        //                AttendanceList = AttendanceList.Where(x => x.LogDate <= filter.To);
        //            }
        //            if (filter.EmployeeName != null)
        //            {
        //                AttendanceList = AttendanceList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
        //            }

        //            var count = AttendanceList.Count();
        //            var list = AttendanceList.OrderByDescending(x => x.LogDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
        //            return new TableResponse<HisduEmpDailyAttancucsSummery>() { Count = count, List = list };

        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}
        ////end get list


        //get attandance list
        //daily attendance report
        public TableResponse<View_MonthlyAttendance1> getAttendanceList(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var AttendanceList = _db.View_MonthlyAttendance1.AsNoTracking().AsQueryable();

                    if (filter.From != null)
                    {
                        AttendanceList = AttendanceList.Where(x => x.LogDate >= filter.From);
                    }
                    if (filter.To != null)
                    {
                        AttendanceList = AttendanceList.Where(x => x.LogDate <= filter.To);
                    }
                    if (filter.EmployeeName != null && filter.EmployeeName != "")
                    {
                        AttendanceList = AttendanceList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
                    }

                    var count = AttendanceList.Count();
                    // var list = AttendanceList.OrderByDescending(x => x.LogDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var list = AttendanceList.OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<View_MonthlyAttendance1>() { Count = count, List = list };

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //end get list


        //get attandance list
        ////public TableResponse<View_EmpAttendance> getAttendanceList(AttandanceFilter filter, string userName, string userId)
        ////{
        ////    using (var _db = new HR_System())
        ////    {
        ////        try
        ////        {
        ////            _db.Configuration.ProxyCreationEnabled = false;
        ////            var AttendanceList = _db.View_EmpAttendance.AsNoTracking().AsQueryable();

        ////            if (filter.From != null)
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.LogDate >= filter.From);
        ////            }
        ////            if (filter.To != null)
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.LogDate <= filter.To);
        ////            }
        ////            if (filter.EmployeeName != null && filter.EmployeeName != "")
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
        ////            }

        ////            var count = AttendanceList.Count();
        ////            var list = AttendanceList.OrderByDescending(x => x.LogDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
        ////            return new TableResponse<View_EmpAttendance>() { Count = count, List = list };

        ////        }
        ////        catch (Exception)
        ////        {
        ////            throw;
        ////        }
        ////    }
        ////}
        //////end get list

        //////get attandance list
        ////public TableResponse<HisduEmpDailyAttancucsSummery> getAttendanceListRpt(AttandanceFilter filter, string userName, string userId)
        ////{
        ////    using (var _db = new HR_System())
        ////    {
        ////        try
        ////        {
        ////            _db.Configuration.ProxyCreationEnabled = false;
        ////            var AttendanceList = _db.HisduEmpDailyAttancucsSummeries.AsNoTracking().AsQueryable();

        ////            if (filter.From != null)
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.LogDate >= filter.From);
        ////            }
        ////            if (filter.To != null)
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.LogDate <= filter.To);
        ////            }
        ////            if (filter.EmployeeName != null && filter.EmployeeName != "")
        ////            {
        ////                AttendanceList = AttendanceList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
        ////            }

        ////            var count = AttendanceList.Count();
        ////            var list = AttendanceList.OrderByDescending(x => x.LogDate).ToList();
        ////            return new TableResponse<HisduEmpDailyAttancucsSummery>() { Count = count, List = list };

        ////        }
        ////        catch (Exception)
        ////        {
        ////            throw;
        ////        }
        ////    }
        ////}
        //////end get list

        //get attandance list
        public TableResponse<View_MonthlyAttendance1> getAttendanceListRpt(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var AttendanceList = _db.View_MonthlyAttendance1.AsNoTracking().AsQueryable();

                    if (filter.From != null)
                    {
                        AttendanceList = AttendanceList.Where(x => x.LogDate >= filter.From);
                    }
                    if (filter.To != null)
                    {
                        AttendanceList = AttendanceList.Where(x => x.LogDate <= filter.To);
                    }
                    if (filter.EmployeeName != null && filter.EmployeeName != "")
                    {
                        AttendanceList = AttendanceList.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
                    }

                    if (filter.SubDeptID > 0)
                    {
                      //  AttendanceList = AttendanceList.Where(x => x.SubDept_ID.ToString == filter.SubDeptID);
                    }
                    var count = AttendanceList.Count();
                    //  var list = AttendanceList.OrderByDescending(x => x.LogDate).ToList();
                    var list = AttendanceList.OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    return new TableResponse<View_MonthlyAttendance1>() { Count = count, List = list };

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //end get list

        public TableResponse<View_MonthlyAttendance1> getAttendanceRec(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var AttendanceRec = _db.View_MonthlyAttendance1.AsNoTracking().AsQueryable();

                    if (filter.HrId > 0)
                    {
                        AttendanceRec = AttendanceRec.Where(x => x.HrId == filter.HrId);
                    }
                    if (filter.Year > 0)
                    {
                        AttendanceRec = AttendanceRec.Where(x => x.Year == filter.Year);
                    }
                    if (filter.Month > 0)
                    {
                        AttendanceRec = AttendanceRec.Where(x => x.Month == filter.Month);
                    }
                    if (filter.parm == "Late")
                    {
                        if (filter.IsLate > 0)
                        {
                            AttendanceRec = AttendanceRec.Where(x => x.IsLate == filter.IsLate);
                        }
                    }

                    var count = AttendanceRec.Count();
                    var list = AttendanceRec.OrderBy(x => x.Id).ToList();
                    return new TableResponse<View_MonthlyAttendance1>() { Count = count, List = list };



                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //end get list




        //bind Leave save model
        private EmpLeaveForm BindLeaveSaveModel(EmpLeaveForm ClientLeave)
        {
            var leave = new EmpLeaveForm();
            //check if new or edit
            if (ClientLeave.ID != 0) { return null; }

            //required
            if (ClientLeave.ProfileID == null) { return null; }
            else { leave.ProfileID = ClientLeave.ProfileID; }

            if (ClientLeave.LeaveEnterDate == null) { return null; }
            else { leave.LeaveEnterDate = ClientLeave.LeaveEnterDate; }

            if (ClientLeave.LeaveTypeID == null) { return null; }
            else { leave.LeaveTypeID = ClientLeave.LeaveTypeID; }

            if (ClientLeave.LeaveFrom == null) { return null; }
            else { leave.LeaveFrom = ClientLeave.LeaveFrom; }

            if (ClientLeave.LeaveTo == null) { return null; }
            else { leave.LeaveTo = ClientLeave.LeaveTo; }

            if (ClientLeave.TotalDays == null) { return null; }
            else { leave.TotalDays = ClientLeave.TotalDays; }

            if (ClientLeave.Reason == null) { return null; }
            else { leave.Reason = ClientLeave.Reason; }

            if (ClientLeave.ContactInfo == null) { return null; }
            else { leave.ContactInfo = ClientLeave.ContactInfo; }

            if (ClientLeave.CBalance == null) { return null; }
            else { leave.CBalance = ClientLeave.CBalance; }

            // if (ClientLeave.ApprovalByID == null) { return null; }
            // else { leave.ApprovalByID = ClientLeave.ApprovalByID; }

            if (ClientLeave.LeaveStatusID == null) { return null; }
            else { leave.LeaveStatusID = ClientLeave.LeaveStatusID; }

            return leave;
        }

        private EmpLeaveForm bindLeaveEditModel(EmpLeaveForm ClientLeave, EmpLeaveForm leave)
        {

            //required
            if (ClientLeave.ProfileID == null) { return null; }
            else { leave.ProfileID = ClientLeave.ProfileID; }

            if (ClientLeave.LeaveEnterDate == null) { return null; }
            else { leave.LeaveEnterDate = ClientLeave.LeaveEnterDate; }

            if (ClientLeave.LeaveTypeID == null) { return null; }
            else { leave.LeaveTypeID = ClientLeave.LeaveTypeID; }

            if (ClientLeave.LeaveFrom == null) { return null; }
            else { leave.LeaveFrom = ClientLeave.LeaveFrom; }

            if (ClientLeave.LeaveTo == null) { return null; }
            else { leave.LeaveTo = ClientLeave.LeaveTo; }

            if (ClientLeave.TotalDays == null) { return null; }
            else { leave.TotalDays = ClientLeave.TotalDays; }

            if (ClientLeave.Reason == null) { return null; }
            else { leave.Reason = ClientLeave.Reason; }

            if (ClientLeave.ContactInfo == null) { return null; }
            else { leave.ContactInfo = ClientLeave.ContactInfo; }

            if (ClientLeave.CBalance == null) { return null; }
            else { leave.CBalance = ClientLeave.CBalance; }

            //if (ClientLeave.ApprovalByID == null) { return null; }
            //else { leave.ApprovalByID = ClientLeave.ApprovalByID; }

            if (ClientLeave.LeaveStatusID == null) { return null; }
            else { leave.LeaveStatusID = ClientLeave.LeaveStatusID; }

            return leave;
        }



        //        //get Leave list report
        //        public TableResponse<TotalLeaves> GetLeaveReport(AttandanceFilter filter, string userName, string userId)
        //        {
        //            using (var _db = new HR_System())
        //            {
        //                try
        //                {
        //_db.Configuration.ProxyCreationEnabled = false;

        //var TLeave = _db.Database.SqlQuery<TotalLeaves>(@"SELECT [Id]
        //,[HrId]
        //,[EmployeeName]
        //,[Year]
        //,[Month]
        //,[TotalLateDay]
        //,[PrasentDay]
        //FROM [HR_System].[dbo].[HisduMonthelyEmpReport]").ToList();

        //var count = TLeave.Count();
        //                    return TLeave;

        //                    //IQueryable<View_TotalLeaves> query = _db.View_TotalLeaves.Where(x => x.HfmisCode.Equals("0350020010030040001")).AsQueryable();

        //                    //if (filter.EmployeeName != null)
        //                    //{
        //                    //    query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).AsQueryable();
        //                    //}

        //                    //if (filter.LeaveFrom == null && filter.LeaveTo == null)
        //                    //{
        //                    //    filter.LeaveFrom = new DateTime(1970, 1, 1);
        //                    //    filter.LeaveTo = DateTime.Now.AddDays(1);
        //                    //}
        //                    //else if (filter.LeaveFrom != null && filter.LeaveTo == null)
        //                    //{
        //                    //    filter.LeaveTo = DateTime.Now.AddDays(1);
        //                    //}
        //                    //else if (filter.LeaveFrom == null && filter.LeaveTo == null)
        //                    //{
        //                    //    filter.LeaveFrom = new DateTime(1970, 1, 1);
        //                    //    filter.LeaveTo = filter.LeaveTo.Value.AddDays(1);
        //                    //}
        //                    //else if (filter.LeaveFrom != null && filter.LeaveTo != null)
        //                    //{
        //                    //    filter.LeaveTo = filter.LeaveTo.Value.AddDays(1);
        //                    //}
        //                    //if (filter.Month > 0)
        //                    //{
        //                    //    query = query.Where(x => x.LeaveFrom >= filter.Month);
        //                    //}
        //                    //if (filter.Year > 0)
        //                    //{
        //                    //    query = query.Where(x => x.LeaveTo <= filter.Year);
        //                    //}


        //                    //var count = query.Count();
        //                    //var list = query.OrderBy(x => x.ProfileId).Skip(filter.Skip).Take(filter.PageSize).ToList();
        //                    //return new TableResponse<View_TotalLeaves>() { Count = count, List = list };


        //                }
        //                catch (Exception)
        //                {
        //                    throw;
        //                }
        //            }
        //        }



        //get Leave list report
        public List<MonthlyLeaves> GetLeaveReport(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var TLeave = new List<MonthlyLeaves>();
                    if (filter.Month > 0)
                    {
                        TLeave = _db.Database.SqlQuery<MonthlyLeaves>(@"SELECT lev.[Id]
, lev.[HrId]
, lev.[EmployeeName]
, lev.Designation_Name, lev.SubDept_ID, lev.DName, lev.IsHOD
, lev.[Year]
, lev.[Month]
, DateName(month, DateAdd(month, lev.[Month], 0) - 1) AS[MonthName]
, IsNull(lev.[TotalLateDay], 0) as [TotalLateDay]
, IsNull(lev.[AvailedLate], 0) as [AvailedLate]
, IsNull(lev.[TotalLeave1], 0) as [TotalLeave1]
, lev.[PrasentDay]
, lev.[TotalLeave]
, lev.[TotalLeave1]
, lev.[abc]
, IsNull(lev.[TotalSL], 0) as [TotalSL]
, IsNull(lev.[TotalSick], 0) as [TotalSick]
, IsNull(lev.[TotalCasual], 0) as [TotalCasual]
, IsNull(lev.[AbsentCount], 0) as [AbsentCount]
, IsNull(lev.[AvailedLate], 0) + IsNull(lev.[AbsentCount], 0) + IsNull(lev.[TotalLeave], 0) as Total
, mb.BAL as [BAL]
FROM[dbo].[HisduMonthelyEmpReport1] lev, dbo.View_MonthlyBalance mb
where lev.HrId = mb.HrId and lev.Month = mb.Month and lev.Year = mb.Year").OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();

                        //TLeave = _db.Database.SqlQuery<MonthlyLeaves>(@"SELECT [Id]
                        //,[HrId]
                        //,[EmployeeName]
                        //,Designation_Name, SubDept_ID, DName, IsHOD
                        //,[Year]
                        //,[Month]
                        //,DateName( month , DateAdd( month , [Month] , 0 ) - 1 ) AS [MonthName]
                        //,IsNull([TotalLateDay],0) as [TotalLateDay]
                        //,IsNull([AvailedLate],0) as [AvailedLate]
                        //,IsNull([TotalLeave1],0) as [TotalLeave1]
                        //,[PrasentDay]
                        //,[TotalLeave]
                        //,[TotalLeave1]
                        //,[abc]
                        //,IsNull([TotalSL],0) as [TotalSL]
                        //,IsNull([TotalSick],0) as [TotalSick]
                        //,IsNull([TotalCasual],0) as [TotalCasual]
                        //,IsNull([AbsentCount],0) as [AbsentCount]
                        //,IsNull([AvailedLate],0) + IsNull([AbsentCount],0) + IsNull([TotalLeave],0) as Total
                        //, IsNull(CASE WHEN (lev.MONTH = 1 or lev.MONTH = 7) THEN (12 - (CONVERT(DECIMAL(10, 2), ISNULL(AvailedLate, 0)) + ISNULL(AbsentCount, 0) + ISNULL(TotalLeave, 0))) Else 
                        //((select (12 - (CONVERT(DECIMAL(10, 2), ISNULL(AvailedLate, 0)) + ISNULL(AbsentCount, 0) + ISNULL(TotalLeave, 0))) from [dbo].[HisduMonthelyEmpReport1] lev1 where lev1.YEAR = lev.YEAR and (lev1.MONTH = lev.MONTH - 1) and lev1.HrId = lev.HrId) - 
                        //((CONVERT(DECIMAL(10, 2), ISNULL(AvailedLate, 0)) + ISNULL(AbsentCount, 0) + ISNULL(TotalLeave, 0)))) END,0) as [BAL]
                        //FROM [dbo].[HisduMonthelyEmpReport1] lev").OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    }
                    else
                    {
                        TLeave = _db.Database.SqlQuery<MonthlyLeaves>(@"SELECT 
[HrId]
,[EmployeeName]
,Designation_Name, SubDept_ID, DName, IsHOD
,[Year]
,IsNull(SUM([TotalLateDay]),0) as [TotalLateDay]
,IsNull(SUM([AvailedLate]),0) as [AvailedLate]
,IsNull(SUM([TotalLeave1]),0) as [TotalLeave1]
,IsNull(SUM([TotalLeave]),0) as [TotalLeave]
,IsNull(SUM([TotalLeave1]),0) as [TotalLeave1]
,IsNull(SUM([abc]),0) as [abc]
,IsNull(SUM([TotalSL]),0) as [TotalSL]
,IsNull(SUM([TotalSick]),0) as [TotalSick]
,IsNull(SUM([TotalCasual]),0) as [TotalCasual]
,IsNull(SUM([AbsentCount]),0) as [AbsentCount]
,IsNull(SUM([AvailedLate]),0) + IsNull(SUM([AbsentCount]),0) + IsNull(SUM([TotalLeave]),0) as Total
,(select cb.BAL from View_CurrentBalance as cb where cb.HrId = lev.HrId) as [BAL]
FROM [dbo].[HisduMonthelyEmpReport1] lev
group by EmployeeName,HrId,Designation_Name, SubDept_ID, DName, IsHOD,[Year]").OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    }

                    if (filter.Month > 0)
                    {
                        TLeave = TLeave.Where(x => x.Month == filter.Month).OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    }
                    if (filter.Year > 0)
                    {
                        TLeave = TLeave.Where(x => x.Year == filter.Year).OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    }
                    if (filter.EmployeeName != null && filter.EmployeeName != "")
                    {
                        TLeave = TLeave.Where(x => x.EmployeeName.ToLower().Contains(filter.EmployeeName.ToLower())).OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).ToList();
                    }

                    var count = TLeave.Count();



                    //foreach (var lst in TLeave)
                    //{
                    //   var is_exists = _db.HISDU_MonthlyLeave.FirstOrDefault(x => x.ProfileID == lst.HrId && x.Year == lst.Year && x.Month == lst.Month);
                    //   //var is_exists = _db.HISDU_MonthlyLeave.FirstOrDefault(x => x.Year == lst.Year && x.Month == lst.Month);

                    //    if (is_exists == null) {
                    //        HISDU_MonthlyLeave mlev = new HISDU_MonthlyLeave();
                    //        mlev.ProfileID = lst.HrId;
                    //        mlev.Year = lst.Year;
                    //        mlev.Month = lst.Month;
                    //        mlev.Lates = lst.TotalLateDay;
                    //        mlev.ShortLeave = lst.TotalSL;
                    //        mlev.SickLeave = lst.TotalSick;
                    //        mlev.CasualLeave = lst.TotalCasual;
                    //        mlev.AvailedLate = lst.AvailedLate;
                    //        mlev.Absent = lst.AbsentCount;
                    //        mlev.TotalLeaves = lst.Total;
                    //        mlev.CBalance = lst.Bal;
                    //        mlev.EmployeeName = lst.EmployeeName;
                    //        mlev.Designation_Name = lst.Designation_Name;
                    //        mlev.SubDept_ID = lst.SubDept_ID;
                    //        mlev.DName = lst.DName;
                    //        mlev.CreatedBy = userName;
                    //        mlev.CreatedOn = DateTime.UtcNow.AddHours(5);
                    //        _db.HISDU_MonthlyLeave.Add(mlev);
                    //        _db.SaveChanges();
                    //    }
                    //    else {
                    //        var dbul = _db.HISDU_MonthlyLeave.FirstOrDefault(x => x.ProfileID == lst.HrId && x.Year == lst.Year && x.Month == lst.Month);

                    //        dbul.Lates = lst.TotalLateDay;
                    //        dbul.ShortLeave = lst.TotalSL;
                    //        dbul.SickLeave = lst.TotalSick;
                    //        dbul.CasualLeave = lst.TotalCasual;
                    //        _db.SaveChanges();
                    //    };   
                    //}

                    return TLeave;

                    // return Ok(true);

                    //var count = query.Count();
                    //var list = query.OrderBy(x => x.ProfileId).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    //return new TableResponse<View_TotalLeaves>() { Count = count, List = list }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public List<View_MonthlyAttendance1> GetLeaveRec(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var TLeaveRec = new List<View_MonthlyAttendance1>();

                    if (filter.parm == "Absent")
                    {
                        TLeaveRec = _db.Database.SqlQuery<View_MonthlyAttendance1>(@"Select * from View_MonthlyAttendance1 where TImeIn is null and TImeOut is null and DATENAME(weekday, LogDate) NOT IN ('Saturday', 'Sunday') and LogStatus='Absent'").ToList();
                    }
                    else if (filter.parm == "Casual")
                    {
                        TLeaveRec = _db.Database.SqlQuery<View_MonthlyAttendance1>(@"Select * from View_MonthlyAttendance1 where (LogStatus like 'Casual%' or LeaveStatus like 'Casual%')").ToList();
                    }
                    else if (filter.parm == "Sick")
                    {
                        TLeaveRec = _db.Database.SqlQuery<View_MonthlyAttendance1>(@"Select * from View_MonthlyAttendance1 where (LogStatus like 'Sick%' or LeaveStatus like 'Sick%')").ToList();
                    }
                    else if (filter.parm == "Short")
                    {
                        TLeaveRec = _db.Database.SqlQuery<View_MonthlyAttendance1>(@"Select * from View_MonthlyAttendance1 where (LogStatus like 'Short%' or LeaveStatus like 'Short%')").ToList();
                    }

                    if (filter.HrId > 0)
                    {
                        TLeaveRec = TLeaveRec.Where(x => x.HrId == filter.HrId).ToList();
                    }
                    if (filter.Month > 0)
                    {
                        TLeaveRec = TLeaveRec.Where(x => x.Month == filter.Month).ToList();
                    }
                    if (filter.Year > 0)
                    {
                        TLeaveRec = TLeaveRec.Where(x => x.Year == filter.Year).ToList();
                    }

                    var count = TLeaveRec.Count();
                    return TLeaveRec;


                    //  return null;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public class EmpYear
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        //public class Empattendance
        //{
        //    public int ProfileID { get; set; }
        //    public int? IndRegID { get; set; }
        //    public int? SubDept_ID { get; set; }
        //    public string SubDept_Name { get; set; }
        //}


        //public EmpYear GetYears()
        //{
        //    using (var _db = new HR_System())
        //    {
        //        try
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;
        //            var years = _db.Hisdu
        //            var count = years.Count();
        //            return years;
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //get Leave list
        public TotalLeaves GetTotalLeaves1(int PId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var TLeave1 = _db.Database.SqlQuery<TotalLeaves>(@"select distinct empl.Id as ProfileID, empl.EmployeeName, empl.CNIC as Cnic, empl.designation_name as Designation_Name, empl.HealthFacility, (select IsNULL(Sum(s3.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s3 where s3.LeaveTypeID=3 and s3.ProfileID = empl.Id) as TotalSL,
    (select IsNULL(Sum(s1.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s1 where s1.LeaveTypeID=1 and s1.ProfileID = empl.Id) as TotalSick,
    (select IsNULL(Sum(s2.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s2 where s2.LeaveTypeID=2 and s2.ProfileID = empl.Id) as TotalCasual,
    (select IsNULL(Sum(s4.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s4 where s4.ProfileID = empl.Id) as TotalLev,
    (select 24 - IsNULL(Sum(ba.TotalDays),0) FROM [dbo].[EmpLeaveForm] as ba where ba.ProfileID = empl.Id) as Bal
    FROM [dbo].[ProfileDetailsView] as empl where (empl.HfmisCode='0350020010030040001' or WorkingHFMISCode='0350020010030040001')").ToList();


                    var count = TLeave1.Count();
                    return TLeave1.FirstOrDefault(x => x.ProfileID.Equals(PId));

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        //get Leave list
        public TotalLeaves GetTotalLeaves(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var TLeave = _db.Database.SqlQuery<TotalLeaves>(@"select distinct empl.Id as ProfileID, empl.EmployeeName, empl.CNIC as Cnic, empl.designation_name as Designation_Name, empl.HealthFacility, (select IsNULL(Sum(s3.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s3 where s3.LeaveTypeID=3 and s3.ProfileID = empl.Id) as TotalSL,
    (select IsNULL(Sum(s1.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s1 where s1.LeaveTypeID=1 and s1.ProfileID = empl.Id) as TotalSick,
    (select IsNULL(Sum(s2.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s2 where s2.LeaveTypeID=2 and s2.ProfileID = empl.Id) as TotalCasual,
    (select IsNULL(Sum(s4.TotalDays),0) FROM [dbo].[EmpLeaveForm] as s4 where s4.ProfileID = empl.Id) as TotalLev,
    (select 24 - IsNULL(Sum(ba.TotalDays),0) FROM [dbo].[EmpLeaveForm] as ba where ba.ProfileID = empl.Id) as Bal
    FROM [dbo].[ProfileDetailsView] as empl where (empl.HfmisCode='0350020010030040001' or WorkingHFMISCode='0350020010030040001')").ToList();


                    var count = TLeave.Count();
                    return TLeave.FirstOrDefault(x => x.Cnic.Equals(cnic));

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public class MonthlyLeaves
        {
            //public int? Id  { get; set; }
            public int HrId { get; set; }
            public string EmployeeName { get; set; }
            public Nullable<int> Year { get; set; }
            public Nullable<int> Month { get; set; }
            public Nullable<int> TotalLateDay { get; set; }
            public Nullable<int> PrasentDay { get; set; }
            public decimal? TotalLeave { get; set; }
            public string MonthName { get; set; }
            public decimal? TotalSick { get; set; }
            public decimal? TotalCasual { get; set; }
            public decimal? TotalSL { get; set; }
            public string Designation_Name { get; set; }
            public Nullable<int> SubDept_ID { get; set; }
            public string DName { get; set; }
            public decimal? AvailedLate { get; set; }
            public decimal? TotalLeave1 { get; set; }
            public decimal? AbsentCount { get; set; }
            public decimal? Bal { get; set; }
            public decimal? Total { get; set; }
            public Nullable<int> IsHOD { get; set; }
        }

        public UserLog EditEmpStatus1(UserLog ul, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {

                        _db.Configuration.ProxyCreationEnabled = false;
                        var dbul = _db.UserLogs.FirstOrDefault(x => x.IndRegID == ul.IndRegID && x.DateOnlyRecord == ul.DateOnlyRecord);
                        var editables = bindStEditModel(ul, dbul);

                        _db.Entry(editables).State = EntityState.Modified;
                        _db.SaveChanges();
                        transc.Commit();
                        return editables;
                    }
                    catch (Exception ex)
                    {

                        transc.Rollback();
                        throw;
                    }

                }
                //  return null;
            }
        }

        private HISDU_MonthlyLeave bindLevEditModel(HISDU_MonthlyLeave ClientUl, HISDU_MonthlyLeave lev)
        {
            lev.Lates = ClientUl.Lates;
            lev.ShortLeave = ClientUl.ShortLeave;
            lev.Lates = ClientUl.Lates;
            lev.SickLeave = ClientUl.SickLeave;
            lev.CasualLeave = ClientUl.CasualLeave;
            lev.AvailedLate = ClientUl.AvailedLate;
            lev.Absent = ClientUl.Absent;
            lev.ShortLeave = ClientUl.ShortLeave;
            lev.TotalLeaves = ClientUl.TotalLeaves;
            lev.CBalance = ClientUl.CBalance;
            return lev;
        }

        private UserLog bindStEditModel(UserLog ClientUl, UserLog ul)
        {
            ul.LogStatus = ClientUl.LogStatus;
            ul.Remarks = ClientUl.Remarks;
            ul.IsLate = ClientUl.IsLate;
            return ul;
        }


        public List<MonthlyLeaves> GetMonths(AttandanceFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var TMonths = _db.Database.SqlQuery<MonthlyLeaves>(@"SELECT distinct [Month] as Month , DateName( month , DateAdd( month , [Month] , 0 ) - 1 ) AS MonthName, Year FROM [dbo].[HisduMonthelyEmpReport1]").ToList();
                    if (filter.Year > 0)
                    {
                        TMonths = TMonths.Where(x => x.Year == filter.Year).ToList();
                    }
                    var count = TMonths.Count();

                    return TMonths;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public class TotalLeaves
        {
            public int? ProfileID { get; set; }
            public string EmployeeName { get; set; }
            public string Cnic { get; set; }
            public string Designation_Name { get; set; }
            public string HealthFacility { get; set; }
            public decimal? TotalSick { get; set; }
            public decimal? TotalCasual { get; set; }
            public decimal? TotalSL { get; set; }
            public decimal? TotalLev { get; set; }
            public decimal? Bal { get; set; }
            public int? LeaveTypeID { get; set; }
            public Nullable<System.DateTime> LeaveFrom { get; set; }
            public Nullable<System.DateTime> LeaveTo { get; set; }
        }

        public TableResponse<View_HISDUProfileDetailsView> GetProfiles(AttandanceFilter filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<View_HISDUProfileDetailsView> query = _db.View_HISDUProfileDetailsView.Where(x => x.Status_Id == 2 && (x.HfmisCode == "0350020010030040001" || x.WorkingHFMISCode == "0350020010030040001")).AsQueryable();

                    //if (filters.hfmisCode != null)
                    //{
                    //    query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    //}               
                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    //  var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    //   var list = query.OrderBy(x => x.EmployeeName).ThenBy(x => x.SubDept_ID).ThenByDescending(x => x.Designation_HrScale_Id).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var list = query.OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<View_HISDUProfileDetailsView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<HFList> SearchHealthFacilities(string query, string userId)
        {
            currentUser = _userService.GetUser(userId);
            return SearchHealthFacilityOnly(query);
        }
        public List<HFList> SearchHealthFacilityOnly(string query)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = query.ToLower();

                    if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                    else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                    else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                    else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                    else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");

                    var hfs = _db.HFLists.Where(x => (x.HFMISCode.Equals(query) || x.FullName.StartsWith(query) || x.FullName.Contains(query)) &&
                        (x.HFMISCode.Equals(currentUser.hfmiscode) || x.HFMISCode.StartsWith(currentUser.hfmiscode)) && x.IsActive == true && x.HFMISCode == "0350020010030040001")
                        .ToList();
                    return hfs;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public View_HISDUProfileDetailsView GetProfile(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.View_HISDUProfileDetailsView.FirstOrDefault(x => x.CNIC.Equals(cnic));
            }
        }

        public Empattendance AddEmpAttendance(Empattendance empAt, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    using (var transc = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (EmpAtExists(empAt.ProfileID))
                            {
                                _db.Configuration.ProxyCreationEnabled = false;
                                var dbEmpAt = _db.Empattendances.FirstOrDefault(x => x.ProfileID == empAt.ProfileID);
                                var editableEmpAt = BindEmpAtEditModel(empAt, dbEmpAt);
                                if (editableEmpAt == null) return null;
                                _db.Entry(editableEmpAt).State = EntityState.Modified;
                                _db.SaveChanges();
                                transc.Commit();

                                return editableEmpAt;
                            }
                            else
                            {
                                var savableEmpAt = BindEmpAtSaveModel(empAt);
                                if (savableEmpAt == null) return null;

                                _db.Empattendances.Add(savableEmpAt);
                                _db.SaveChanges();
                                transc.Commit();
                                return savableEmpAt;
                            }
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public HrProfile AddProfile(HrProfile hrProfile, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    using (var transc = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (hrProfile.Id == 0)
                            {
                                var savableHrProfile = BindProfileSaveModel(hrProfile);
                                if (savableHrProfile == null) return null;
                                Entity_Lifecycle elc = new Entity_Lifecycle();

                                elc.IsActive = true;
                                elc.Created_By = userName;
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Entity_Id = 9;
                                elc.Users_Id = userId;
                                _db.Entity_Lifecycle.Add(elc);
                                _db.SaveChanges();

                                savableHrProfile.EntityLifecycle_Id = elc.Id;


                                _db.HrProfiles.Add(savableHrProfile);
                                _db.SaveChanges();
                                transc.Commit();

                                return savableHrProfile;
                            }
                            else if (ProfileExists(hrProfile.Id))
                            {
                                _db.Configuration.ProxyCreationEnabled = false;
                                var dbHrProfile = _db.HrProfiles.FirstOrDefault(x => x.Id == hrProfile.Id);
                                var editableHrProfile = BindProfileEditModel(hrProfile, dbHrProfile);
                                if (editableHrProfile == null) return null;

                                if (editableHrProfile.EntityLifecycle_Id == null)
                                {
                                    Entity_Lifecycle elc = new Entity_Lifecycle();
                                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                    elc.Created_By = userName + " (added after migration)";
                                    elc.Users_Id = userId;
                                    elc.IsActive = true;
                                    elc.Entity_Id = 9;
                                    _db.Entity_Lifecycle.Add(elc);
                                    _db.SaveChanges();
                                    editableHrProfile.EntityLifecycle_Id = elc.Id;
                                }
                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)editableHrProfile.EntityLifecycle_Id;
                                eml.Description = "Profile Updated By " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();

                                _db.Entry(editableHrProfile).State = EntityState.Modified;
                                _db.SaveChanges();
                                transc.Commit();

                                return editableHrProfile;
                            }
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private HrProfile BindProfileSaveModel(HrProfile clientProfile)
        {
            HrProfile hrProfile = new HrProfile();

            if (clientProfile.Id != 0) { return null; }
            if (hrProfile.Id != 0) { return null; }

            //Required
            if (clientProfile.EmployeeName == null) { return null; }
            else { hrProfile.EmployeeName = clientProfile.EmployeeName; }

            //Required
            if (clientProfile.FatherName == null) { return null; }
            else { hrProfile.FatherName = clientProfile.FatherName; }

            //Required
            if (clientProfile.CNIC == null) { return null; }
            else { hrProfile.CNIC = clientProfile.CNIC; }

            //Required
            if (clientProfile.DateOfBirth == null) { return null; }
            else
            {
                if (clientProfile.DateOfBirth.Value.Hour == 19) { hrProfile.DateOfBirth = clientProfile.DateOfBirth.Value.AddHours(5); }
                else { hrProfile.DateOfBirth = clientProfile.DateOfBirth; }
            }

            //Required
            if (clientProfile.Gender == null) { return null; }
            else { hrProfile.Gender = clientProfile.Gender; }

            //Required
            //if (clientProfile.Department_Id == null) { return null; }
            //else { hrProfile.Department_Id = clientProfile.Department_Id; }
            hrProfile.Department_Id = clientProfile.Department_Id;

            //Required
            if (clientProfile.HealthFacility_Id == null) { return null; }
            else { hrProfile.HealthFacility_Id = clientProfile.HealthFacility_Id; hrProfile.HfmisCode = clientProfile.HfmisCode; }

            //Required
            if (clientProfile.Designation_Id == null) { return null; }
            else { hrProfile.Designation_Id = clientProfile.Designation_Id; hrProfile.Postaanctionedwithscale = clientProfile.Postaanctionedwithscale; }

            //Required
            //if (clientProfile.JoiningGradeBPS == null) { return null; }
            //else { hrProfile.JoiningGradeBPS = clientProfile.JoiningGradeBPS; }

            hrProfile.JoiningGradeBPS = clientProfile.JoiningGradeBPS;

            //Required
            //if (clientProfile.CurrentGradeBPS == null) { return null; }
            //else { hrProfile.CurrentGradeBPS = clientProfile.CurrentGradeBPS; }

            hrProfile.CurrentGradeBPS = clientProfile.CurrentGradeBPS;

            //Required
            //if (clientProfile.EmpMode_Id == null) { return null; }
            //else { hrProfile.EmpMode_Id = clientProfile.EmpMode_Id; }
            hrProfile.EmpMode_Id = clientProfile.EmpMode_Id;
            //hrProfile.EmpMode_Id = clientProfile.EmpMode_Id;

            //Required
            if (clientProfile.Status_Id == null) { return null; }
            else { hrProfile.Status_Id = clientProfile.Status_Id; }

            //Required
            if (clientProfile.MobileNo == null) { return null; }
            else { hrProfile.MobileNo = clientProfile.MobileNo; }

            hrProfile.Domicile_Id = clientProfile.Domicile_Id;
            hrProfile.MaritalStatus = clientProfile.MaritalStatus;
            hrProfile.Religion_Id = clientProfile.Religion_Id;
            hrProfile.Language_Id = clientProfile.Language_Id;
            hrProfile.BloodGroup = clientProfile.BloodGroup;

            hrProfile.WorkingHealthFacility_Id = clientProfile.WorkingHealthFacility_Id;
            hrProfile.WorkingHFMISCode = clientProfile.WorkingHFMISCode;
            hrProfile.WDesignation_Id = clientProfile.WDesignation_Id;

            hrProfile.SeniorityNo = clientProfile.SeniorityNo;
            hrProfile.PersonnelNo = clientProfile.PersonnelNo;
            hrProfile.HoD = clientProfile.HoD;
            hrProfile.PresentPostingOrderNo = clientProfile.PresentPostingOrderNo;
            if (clientProfile.PresentPostingDate != null && clientProfile.PresentPostingDate.Value.Hour == 19) { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate.Value.AddHours(5); }
            else { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate; }

            hrProfile.Qualification_Id = clientProfile.Qualification_Id;
            hrProfile.Specialization_Id = clientProfile.Specialization_Id;
            hrProfile.AdditionalQualification = clientProfile.AdditionalQualification;
            hrProfile.AdditionalCharge = clientProfile.AdditionalCharge;
            hrProfile.Posttype_Id = clientProfile.Posttype_Id;
            hrProfile.Hfac = clientProfile.Hfac;
            if (clientProfile.DateOfFirstAppointment != null && clientProfile.DateOfFirstAppointment.Value.Hour == 19) { hrProfile.DateOfFirstAppointment = clientProfile.DateOfFirstAppointment.Value.AddHours(5); }
            else { hrProfile.DateOfFirstAppointment = clientProfile.DateOfFirstAppointment; }

            if (clientProfile.DateOfRegularization != null && clientProfile.DateOfRegularization.Value.Hour == 19) { hrProfile.DateOfRegularization = clientProfile.DateOfRegularization.Value.AddHours(5); }
            else { hrProfile.DateOfRegularization = clientProfile.DateOfRegularization; }

            if (clientProfile.ContractStartDate != null && clientProfile.ContractStartDate.Value.Hour == 19) { hrProfile.ContractStartDate = clientProfile.ContractStartDate.Value.AddHours(5); }
            else { hrProfile.ContractStartDate = clientProfile.ContractStartDate; }

            if (clientProfile.ContractEndDate != null && clientProfile.ContractEndDate.Value.Hour == 19) { hrProfile.ContractEndDate = clientProfile.ContractEndDate.Value.AddHours(5); }
            else { hrProfile.ContractEndDate = clientProfile.ContractEndDate; }

            if (clientProfile.LastPromotionDate != null && clientProfile.LastPromotionDate.Value.Hour == 19) { hrProfile.LastPromotionDate = clientProfile.LastPromotionDate.Value.AddHours(5); }
            else { hrProfile.LastPromotionDate = clientProfile.LastPromotionDate; }

            hrProfile.PrivatePractice = clientProfile.PrivatePractice;
            hrProfile.PermanentAddress = clientProfile.PermanentAddress;
            hrProfile.CorrespondenceAddress = clientProfile.CorrespondenceAddress;
            hrProfile.LandlineNo = clientProfile.LandlineNo;
            hrProfile.Faxno = clientProfile.Faxno;
            hrProfile.EMaiL = clientProfile.EMaiL;

            //hrProfile.EmpAt.IndRegID = clientProfile.EmpAt.IndRegID;
            //hrProfile.EmpAt.SubDept_ID = clientProfile.EmpAt.SubDept_ID;
            //hrProfile.EmpAt.ProfileID = clientProfile.EmpAt.ProfileID;

            return hrProfile;

        }
        private HrProfile BindProfileEditModel(HrProfile clientProfile, HrProfile hrProfile)
        {
            //check if new or edit
            if (clientProfile.Id == 0) { return null; }
            if (hrProfile.Id == 0) { return null; }

            //Required
            if (clientProfile.EmployeeName == null) { return null; }
            else { hrProfile.EmployeeName = clientProfile.EmployeeName; }

            //Required
            if (clientProfile.FatherName == null) { return null; }
            else { hrProfile.FatherName = clientProfile.FatherName; }

            //Required
            if (clientProfile.CNIC == null) { return null; }
            else { hrProfile.CNIC = clientProfile.CNIC; }

            //Required
            if (clientProfile.DateOfBirth == null) { return null; }
            else
            {
                if (clientProfile.DateOfBirth.Value.Hour == 19) { hrProfile.DateOfBirth = clientProfile.DateOfBirth.Value.AddHours(5); }
                else { hrProfile.DateOfBirth = clientProfile.DateOfBirth; }
            }
            //Required
            if (clientProfile.Gender == null) { return null; }
            else { hrProfile.Gender = clientProfile.Gender; }

            //Required
            //if (clientProfile.Department_Id == null) { return null; }
            //else { hrProfile.Department_Id = clientProfile.Department_Id; }
            hrProfile.Department_Id = clientProfile.Department_Id;

            //Required
            if (clientProfile.HealthFacility_Id == null) { return null; }
            else { hrProfile.HealthFacility_Id = clientProfile.HealthFacility_Id; hrProfile.HfmisCode = clientProfile.HfmisCode; }

            //Required
            if (clientProfile.Designation_Id == null) { return null; }
            else { hrProfile.Designation_Id = clientProfile.Designation_Id; hrProfile.Postaanctionedwithscale = clientProfile.Postaanctionedwithscale; }

            //Required
            //if (clientProfile.JoiningGradeBPS == null) { return null; }
            //else { hrProfile.JoiningGradeBPS = clientProfile.JoiningGradeBPS; }
            hrProfile.JoiningGradeBPS = clientProfile.JoiningGradeBPS;

            //Required
            //if (clientProfile.CurrentGradeBPS == null) { return null; }
            //else { hrProfile.CurrentGradeBPS = clientProfile.CurrentGradeBPS; }
            hrProfile.CurrentGradeBPS = clientProfile.CurrentGradeBPS;

            //Required
            //if (clientProfile.EmpMode_Id == null) { return null; }
            //else { hrProfile.EmpMode_Id = clientProfile.EmpMode_Id; }
            hrProfile.EmpMode_Id = clientProfile.EmpMode_Id;

            //Required
            if (clientProfile.Status_Id == null) { return null; }
            else { hrProfile.Status_Id = clientProfile.Status_Id; }

            //Required
            if (clientProfile.MobileNo == null) { return null; }
            else { hrProfile.MobileNo = clientProfile.MobileNo; }

            hrProfile.Domicile_Id = clientProfile.Domicile_Id;
            hrProfile.MaritalStatus = clientProfile.MaritalStatus;
            hrProfile.Religion_Id = clientProfile.Religion_Id;
            hrProfile.Language_Id = clientProfile.Language_Id;
            hrProfile.BloodGroup = clientProfile.BloodGroup;

            //
            hrProfile.WorkingHealthFacility_Id = clientProfile.WorkingHealthFacility_Id;
            hrProfile.WorkingHFMISCode = clientProfile.WorkingHFMISCode;
            hrProfile.WDesignation_Id = clientProfile.WDesignation_Id;

            hrProfile.SeniorityNo = clientProfile.SeniorityNo;
            hrProfile.PersonnelNo = clientProfile.PersonnelNo;
            hrProfile.HoD = clientProfile.HoD;
            hrProfile.PresentPostingOrderNo = clientProfile.PresentPostingOrderNo;

            if (clientProfile.PresentPostingDate != null && clientProfile.PresentPostingDate.Value.Hour == 19) { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate.Value.AddHours(5); }
            else { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate; }

            hrProfile.Qualification_Id = clientProfile.Qualification_Id;
            hrProfile.Specialization_Id = clientProfile.Specialization_Id;
            hrProfile.AdditionalQualification = clientProfile.AdditionalQualification;
            hrProfile.AdditionalCharge = clientProfile.AdditionalCharge;
            hrProfile.Posttype_Id = clientProfile.Posttype_Id;
            hrProfile.Hfac = clientProfile.Hfac;
            if (clientProfile.DateOfFirstAppointment != null && clientProfile.DateOfFirstAppointment.Value.Hour == 19) { hrProfile.DateOfFirstAppointment = clientProfile.DateOfFirstAppointment.Value.AddHours(5); }
            else { hrProfile.DateOfFirstAppointment = clientProfile.DateOfFirstAppointment; }

            if (clientProfile.DateOfRegularization != null && clientProfile.DateOfRegularization.Value.Hour == 19) { hrProfile.DateOfRegularization = clientProfile.DateOfRegularization.Value.AddHours(5); }
            else { hrProfile.DateOfRegularization = clientProfile.DateOfRegularization; }

            if (clientProfile.ContractStartDate != null && clientProfile.ContractStartDate.Value.Hour == 19) { hrProfile.ContractStartDate = clientProfile.ContractStartDate.Value.AddHours(5); }
            else { hrProfile.ContractStartDate = clientProfile.ContractStartDate; }

            if (clientProfile.ContractEndDate != null && clientProfile.ContractEndDate.Value.Hour == 19) { hrProfile.ContractEndDate = clientProfile.ContractEndDate.Value.AddHours(5); }
            else { hrProfile.ContractEndDate = clientProfile.ContractEndDate; }

            if (clientProfile.LastPromotionDate != null && clientProfile.LastPromotionDate.Value.Hour == 19) { hrProfile.LastPromotionDate = clientProfile.LastPromotionDate.Value.AddHours(5); }
            else { hrProfile.LastPromotionDate = clientProfile.LastPromotionDate; }

            hrProfile.PrivatePractice = clientProfile.PrivatePractice;
            hrProfile.PermanentAddress = clientProfile.PermanentAddress;
            hrProfile.CorrespondenceAddress = clientProfile.CorrespondenceAddress;
            hrProfile.LandlineNo = clientProfile.LandlineNo;
            hrProfile.Faxno = clientProfile.Faxno;
            hrProfile.EMaiL = clientProfile.EMaiL;

            //hrProfile.EmpAt.IndRegID = clientProfile.EmpAt.IndRegID;
            //hrProfile.EmpAt.SubDept_ID = clientProfile.EmpAt.SubDept_ID;
            //hrProfile.EmpAt.ProfileID = clientProfile.EmpAt.ProfileID;

            return hrProfile;

        }
        public bool ProfileExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == Id);
                    if (profile == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Empattendance BindEmpAtSaveModel(Empattendance clientEmpAt)
        {
            Empattendance empAt = new Empattendance();

            //if (clientEmpAt.ProfileID != 0) { return null; }
            //if (hrProfile.Id != 0) { return null; }

            //Required
            if (clientEmpAt.IndRegID == null) { return null; }
            else { empAt.IndRegID = clientEmpAt.IndRegID; }

            //Required
            if (clientEmpAt.ProfileID == 0) { return null; }
            else { empAt.ProfileID = clientEmpAt.ProfileID; }

            //Required
            if (clientEmpAt.SubDept_ID == null) { return null; }
            else { empAt.SubDept_ID = clientEmpAt.SubDept_ID; }

            return empAt;

        }
        private Empattendance BindEmpAtEditModel(Empattendance clientEmpAt, Empattendance empAt)
        {
            //check if new or edit
            if (clientEmpAt.ProfileID == 0) { return null; }
            if (empAt.ProfileID == 0) { return null; }

            //Required
            if (clientEmpAt.IndRegID == null) { return null; }
            else { empAt.IndRegID = clientEmpAt.IndRegID; }

            //Required
            if (clientEmpAt.ProfileID == 0) { return null; }
            else { empAt.ProfileID = clientEmpAt.ProfileID; }

            //Required
            if (clientEmpAt.SubDept_ID == null) { return null; }
            else { empAt.SubDept_ID = clientEmpAt.SubDept_ID; }
            return empAt;

        }

        public bool EmpAtExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var EmpAt = _db.Empattendances.FirstOrDefault(x => x.ProfileID == Id);
                    if (EmpAt == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public TableResponse<View_HISDUProfileDetailsView> GetProfilesInActive(AttandanceFilter filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<View_HISDUProfileDetailsView> query = _db.View_HISDUProfileDetailsView.Where(x => x.Status_Id == 16 && (x.HfmisCode == "0350020010030040001" || x.WorkingHFMISCode == "0350020010030040001")).AsQueryable();
                    //if (filters.hfmisCode != null)
                    //{
                    //    query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    //}                  
                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    var list = query.OrderBy(x => x.SubDept_ID).ThenByDescending(x => x.IsHOD).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    //       var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<View_HISDUProfileDetailsView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }

}