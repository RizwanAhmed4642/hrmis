using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels;
using Hrmis.Models.ViewModels.Application;
using Hrmis.Models.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Hrmis.Models.Services
{
    public class ApplicationService
    {
        private TransferPostingService _transferPostingService;
        private UserService _userService;

        public ApplicationService()
        {
            _transferPostingService = new TransferPostingService();
            _userService = new UserService();
        }

        public ApplicationFts GetApplication(int Id, int Tracking, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == Tracking && x.IsActive == true);
                    if (applicationFts.application == null)
                    {
                        return null;
                    }
                    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id && x.IsActive == true).ToList();
                    return applicationFts;
                    //if (userName.Equals("dpd"))
                    //{
                    //    applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.IsActive == true);
                    //    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                    //    return applicationFts;
                    //}
                    //else if (userName.StartsWith("pl") || userName.StartsWith("fdo"))
                    //{
                    //    applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.Users_Id.Equals(userId) && x.IsActive == true);
                    //    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                    //    return applicationFts;
                    //}
                    //else if (currentOfficer != null)
                    //{
                    //    if (currentOfficer.Designation_Id == 2265)
                    //    {
                    //        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.Users_Id == userId && x.IsActive == true);
                    //    }
                    //    else
                    //    {
                    //        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.PandSOfficer_Id == currentOfficer.Id && (x.IsPending == null || x.IsPending == false) && x.IsActive == true);
                    //    }
                    //    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                    //    return applicationFts;
                    //}
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public ApplicationFts GetApplicationData(int Id, string Type, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    if (Type.Equals("logs"))
                    {
                        applicationFts.applicationLogs = _db.ApplicationLogViews.Where(x => x.Application_Id == Id).OrderBy(x => x.DateTime).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("oldlogs"))
                    {
                        applicationFts.applicationForwardLogs = _db.ApplicationForwardLogs.Where(x => x.Application_Id == Id).OrderByDescending(x => x.DateTime).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("parliamentarian"))
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                        applicationFts.applicationPersonAppeared = _db.ApplicationPersonAppeareds.FirstOrDefault(x => x.Id == applicationFts.application.PersonAppeared_Id);
                        return applicationFts;
                    }
                    else if (Type.Equals("file"))
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                        var file = new List<DDS_Files>();
                        if (!string.IsNullOrEmpty(applicationFts.application.FileNumber))
                        {
                            file = filesACRService.GetDDSFilesByFileNumber(applicationFts.application.FileNumber);
                        }
                        if (file.Count == 0)
                        {
                            file = filesACRService.GetDDSFilesByCNIC(applicationFts.application.CNIC);
                        }
                        if (file.Count == 0)
                        {
                            file = filesACRService.GetDDSFilesByName(applicationFts.application.EmployeeName);
                        }
                        if (file.Count > 0)
                        {
                            applicationFts.File = file.FirstOrDefault();
                        }
                        return applicationFts;
                    }
                    else if (Type.Equals("filereqs"))
                    {
                        applicationFts.applicationFileRecositions = _db.ApplicationFileReqViews.Where(x => x.Application_Id == Id && x.IsActive == true).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("applicationattachments"))
                    {
                        applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == Id && x.IsActive == true).ToList();
                        return applicationFts;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<ApplicationLogView> GetApplicationLogs(int Id, int lastId, bool orderAsc, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.ApplicationLogViews.Where(x => x.Application_Id == Id).AsQueryable();
                    if (lastId != 0)
                    {
                        query = query.Where(x => x.Id > lastId).AsQueryable();
                        if (orderAsc)
                        {
                            query = query.OrderBy(x => x.DateTime).AsQueryable();
                        }
                        else
                        {
                            query = query.OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                    }
                    query = query.OrderBy(x => x.DateTime).AsQueryable();
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public TableResponse<ApplicationLogView> GetApplicationLogsLaw(int Id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.ApplicationLogViews.Where(x => x.Application_Id == Id && !string.IsNullOrEmpty(x.Remarks)).AsQueryable();
                    //if (lastId != 0)
                    //{
                    //    query = query.Where(x => x.Id > lastId).AsQueryable();
                    //    if (orderAsc)
                    //    {
                    //        query = query.OrderBy(x => x.DateTime).AsQueryable();
                    //    }
                    //    else
                    //    {
                    //        query = query.OrderByDescending(x => x.DateTime).AsQueryable();
                    //    }
                    //}
                    query = query.OrderBy(x => x.DateTime).AsQueryable();
                    return new TableResponse<ApplicationLogView>
                    {
                        List = query.ToList()
                    };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public bool RemoveApplication(int Id, int Tracking, string userName, string userId, ApplicationLog applicationLog)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (userName.Equals("dpd") || userName.Equals("pshd") || userName.Equals("managerfc") || userName.StartsWith("fdo") || userName.StartsWith("ri.") || userName.StartsWith("pl"))
                    {
                        var application = _db.ApplicationMasters.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.IsActive == true);
                        application.IsActive = false;
                        _db.SaveChanges();
                        CreateApplicationLog(applicationLog, userName, userId);
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool RemoveApplicationAttachment(int Id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var applicationAttachment = _db.ApplicationAttachments.FirstOrDefault(x => x.Id == Id);
                    if (applicationAttachment != null)
                    {
                        var application = _db.ApplicationMasters.FirstOrDefault(x => x.Id == applicationAttachment.Application_Id);
                        if (application != null && application.Created_By.Equals(userName))
                        {
                            applicationAttachment.IsActive = false;
                            _db.Entry(applicationAttachment).State = EntityState.Modified;
                            _db.SaveChanges();
                            if (applicationAttachment.Document_Id == 1)
                            {
                                application.IsSigned = false;
                                application.SignededAppAttachement_Id = null;
                                _db.Entry(application).State = EntityState.Modified;
                                _db.SaveChanges();
                                return true;
                            }
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool ChangeFileNumberOfApplication(ApplicationMaster application, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var applicationDb = _db.ApplicationMasters.FirstOrDefault(x => x.Id == application.Id && x.TrackingNumber == application.TrackingNumber && x.IsActive == true);
                    if (applicationDb.FileRequested == null || applicationDb.FileRequested == false)
                    {
                        applicationDb.FileNumber = application.FileNumber;
                        if (application.DDS_Id != null && application.DDS_Id != 0)
                        {
                            var ddsFile = _db.DDS_Files.FirstOrDefault(x => x.Id == application.DDS_Id);
                            if (ddsFile != null)
                            {
                                applicationDb.DDS_Id = application.DDS_Id;
                            }
                            else
                            {
                                applicationDb.DDS_Id = null;
                            }
                        }
                        else
                        {
                            applicationDb.DDS_Id = null;
                        }
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetApplications(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
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
                    if (!userName.Equals("dpd") && !userName.Equals("managerfc") && !userName.Equals("ri.incharge") && !userName.StartsWith("fts"))
                    {
                        query = query.Where(x => x.Users_Id.Equals(userId)).AsQueryable();
                    }
                    //if (userName.StartsWith("ceo."))
                    //{
                    //    var hfmisCode = _userService.GetUser(userId).hfmiscode;
                    //    query = query.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.ApplicationSource_Id == 3 && x.Status_Id == 1).AsQueryable();
                    //}
                    if (filter.Source_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationSource_Id == filter.Source_Id).AsQueryable();
                    }
                    if (filter.Status_Id != 0)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                    }
                    if (filter.Status_Id == 222)
                    {
                        //var vpMasters = 
                        query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                    }
                    if (filter.Type_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationType_Id == filter.Type_Id).AsQueryable();
                    }
                    if (filter.OfficeId != 0)
                    {
                        query = query.Where(x => x.PandSOfficer_Id == filter.OfficeId || x.ToOfficer_Id == filter.OfficeId);
                    }
                    if (filter.ForwardingOfficer_Id != 0)
                    {
                        query = query.Where(x => x.ForwardingOfficer_Id == filter.ForwardingOfficer_Id);
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
                            query = query.Where(x => x.TrackingNumber == number || x.DispatchNumber.Equals(number.ToString())).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchSubject.ToLower().Contains(filter.Query.ToLower()) ||
                            x.MobileNo.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchFrom.ToLower().Contains(filter.Query.ToLower())
                            ).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    if (countsOnly == false)
                    {
                        var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return new TableResponse<ApplicationView>() { Count = count, List = list };
                    }
                    else
                    {
                        return new TableResponse<ApplicationView>() { Count = count };
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetBarcodedApplications(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
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
                    query = query.Where(x => x.Users_Id.Equals(userId)).AsQueryable();
                    if (filter.Source_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationSource_Id == filter.Source_Id).AsQueryable();
                    }
                    if (filter.Status_Id != 0)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                    }
                    if (filter.Status_Id == 222)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                    }
                    if (filter.Type_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationType_Id == filter.Type_Id).AsQueryable();
                    }
                    if (filter.OfficeId != 0)
                    {
                        query = query.Where(x => x.PandSOfficer_Id == filter.OfficeId || x.ToOfficer_Id == filter.OfficeId);
                    }
                    if (filter.ForwardingOfficer_Id != 0)
                    {
                        query = query.Where(x => x.ForwardingOfficer_Id == filter.ForwardingOfficer_Id);
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
                            query = query.Where(x => x.TrackingNumber == number || x.DispatchNumber.Equals(number.ToString())).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchSubject.ToLower().Contains(filter.Query.ToLower()) ||
                            x.MobileNo.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchFrom.ToLower().Contains(filter.Query.ToLower())
                            ).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    if (countsOnly == false)
                    {
                        var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return new TableResponse<ApplicationView>() { Count = count, List = list };
                    }
                    else
                    {
                        return new TableResponse<ApplicationView>() { Count = count };
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetApplicationsLawwing(ApplicationFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null)
                    {
                        if (userName.ToLower().StartsWith("ceo."))
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.DesignationName.Equals("Chief Executive Officer"));
                        }
                        if (userName.Length == 13)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        }
                    }
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
                    if (userName.ToLower().StartsWith("ceo."))
                    {
                        var hfmisCode = _userService.GetUser(userId).hfmiscode;
                        query = query.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.ApplicationSource_Id == 3).AsQueryable();
                    }
                    query = query.Where(x => x.ForwardingOfficer_Id == currentOfficer.Id || x.PandSOfficer_Id == currentOfficer.Id).AsQueryable();
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


                    var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<ApplicationView>() { Count = count, List = list };


                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetInboxApplications(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null)
                    {
                        if (userName.ToLower().StartsWith("ceo."))
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.DesignationName.Equals("Chief Executive Officer"));
                        }
                        if (userName.Length == 13)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        }
                    }
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
                    if (userName.ToLower().StartsWith("ceo."))
                    {
                        var hfmisCode = _userService.GetUser(userId).hfmiscode;
                        query = query.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.ApplicationSource_Id == 3).AsQueryable();
                    }
                    if (filter.Pending)
                    {
                        query = query.Where(x => x.ToOfficer_Id == currentOfficer.Id && x.IsPending == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => x.PandSOfficer_Id == currentOfficer.Id).AsQueryable();
                    }
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
                        if (filter.Status_Id == 4)
                        {
                            query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3 || x.Status_Id == 4 || x.Status_Id == 5 || x.Status_Id == 6 || x.Status_Id == 7).AsQueryable();
                        }
                        else if (filter.Status_Id == 333)
                        {
                            query = query.Where(x => x.Status_Id == 10 || x.Status_Id == 1).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                        }
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
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchSubject.ToLower().Contains(filter.Query.ToLower()) ||
                            x.DispatchNumber.ToLower().Contains(filter.Query.ToLower())
                            ).AsQueryable();
                        }
                    }

                    var count = query.Count();

                    if (countsOnly == false)
                    {
                        //if (filter.Pending)
                        //{
                        //    var list = query.OrderByDescending(x => x.ForwardTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        //    return new TableResponse<ApplicationView>() { Count = count, List = list };
                        //}
                        //else
                        //{
                        //    var list = query.OrderByDescending(x => x.RecieveTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        //    return new TableResponse<ApplicationView>() { Count = count, List = list };
                        //}
                        var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return new TableResponse<ApplicationView>() { Count = count, List = list };
                    }
                    else
                    {
                        return new TableResponse<ApplicationView>() { Count = count };
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<ApplicationThumbView> GetSelectedInboxApplications(List<int> Ids, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<ApplicationThumbView> query = _db.ApplicationThumbViews.Where(x => Ids.Contains(x.Id) && x.IsActive == true).AsQueryable();

                    //var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    //if (currentOfficer == null)
                    //{
                    //    if (userName.ToLower().StartsWith("ceo."))
                    //    {
                    //        var hfmisCode = _userService.GetUser(userId).hfmiscode;
                    //        query = query.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.ApplicationSource_Id == 3).AsQueryable();
                    //    }else
                    //    {
                    //        return null;
                    //    }
                    //}

                    //if (currentOfficer.DesignationName == "Front Desk Officer")
                    //{
                    //    currentOfficer = null;
                    //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    //}
                    //if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    //{
                    //    currentOfficer = null;
                    //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    //}
                    //if (currentOfficer.DesignationName == "R & I Branch")
                    //{
                    //    currentOfficer = null;
                    //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    //}
                    //if (currentOfficer.DesignationName == "Senior Law Officer")
                    //{
                    //    currentOfficer = null;
                    //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 178);
                    //}
                    //if (currentOfficer.Code == 99999999)
                    //{
                    //    var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                    //    if (higherOfficer != null)
                    //    {
                    //        currentOfficer = null;
                    //        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                    //    }
                    //}
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetInboxApplicationsHisdu(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == filter.Officer_Id);
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
                    if (filter.Pending)
                    {
                        query = query.Where(x => x.ToOfficer_Id == currentOfficer.Id && x.IsPending == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => x.PandSOfficer_Id == currentOfficer.Id).AsQueryable();
                    }
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
                        if (filter.Status_Id == 4)
                        {
                            query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3).AsQueryable();
                        }
                        else if (filter.Status_Id == 333)
                        {
                            query = query.Where(x => x.Status_Id == 10 || x.Status_Id == 1).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                        }
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

                    if (countsOnly == false)
                    {
                        //if (filter.Pending)
                        //{
                        //    var list = query.OrderByDescending(x => x.ForwardTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        //    return new TableResponse<ApplicationView>() { Count = count, List = list };
                        //}
                        //else
                        //{
                        //    var list = query.OrderByDescending(x => x.RecieveTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        //    return new TableResponse<ApplicationView>() { Count = count, List = list };
                        //}
                        var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return new TableResponse<ApplicationView>() { Count = count, List = list };
                    }
                    else
                    {
                        return new TableResponse<ApplicationView>() { Count = count };
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicationView> GetSentApplications(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }

                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    var applicationLogIds = _db.ApplicationLogs.Where(x => x.FromOfficer_Id == currentOfficer.Id && x.IsActive == true).Select(x => x.Application_Id).Distinct().ToList();

                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.FromOfficer_Id == currentOfficer.Id && x.IsActive == true).AsQueryable();
                    //List<ApplicationView> query = _db.Database.SqlQuery<ApplicationView>("select * from ApplicationView where Id in (SELECT Application_Id FROM [HR_System].[dbo].[ApplicationForwardLog] where User_Id = '" + userId + "' and IsActive = 1 group by Application_Id)").ToList();
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
                    return new TableResponse<ApplicationView>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public TableResponse<ApplicationView> GetSummaries(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true).AsQueryable();
                    //List<ApplicationView> query = _db.Database.SqlQuery<ApplicationView>("select * from ApplicationView where Id in (SELECT Application_Id FROM [HR_System].[dbo].[ApplicationForwardLog] where User_Id = '" + userId + "' and IsActive = 1 group by Application_Id)").ToList();
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
                    if (currentOfficer.Id == 1)
                    {
                        query = query.Where(x => x.Created_By.Equals("pshd")).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => x.DispatchFrom.Equals(currentOfficer.DesignationName)).AsQueryable();
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
                    return new TableResponse<ApplicationView>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public TableResponse<ApplicationView> GetScannedDocuments(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.ApplicationSource_Id == 10 && x.IsActive == true).AsQueryable();
                    //List<ApplicationView> query = _db.Database.SqlQuery<ApplicationView>("select * from ApplicationView where Id in (SELECT Application_Id FROM [HR_System].[dbo].[ApplicationForwardLog] where User_Id = '" + userId + "' and IsActive = 1 group by Application_Id)").ToList();
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
                    //if (currentOfficer.Id == 1)
                    //{
                    //    query = query.Where(x => x.Created_By.Equals("pshd")).AsQueryable();
                    //}
                    //else
                    //{
                    //    query = query.Where(x => x.DispatchFrom.Equals(currentOfficer.DesignationName)).AsQueryable();
                    //}
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
                    return new TableResponse<ApplicationView>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<InboxOfficersViewModel> GetInboxOffices(bool pending, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null)
                    {
                        if (userName.ToLower().StartsWith("ceo."))
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.DesignationName.Equals("Chief Executive Officer"));
                        }
                        if (userName.Length == 13)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        }
                    }
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.ToOfficer_Id == currentOfficer.Id && x.IsPending == pending && x.IsActive == true).AsQueryable();
                    if (userName.ToLower().StartsWith("ceo."))
                    {
                        var hfmisCode = _userService.GetUser(userId).hfmiscode;
                        query = query.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.ApplicationSource_Id == 3).AsQueryable();
                    }
                    var offices = query.GroupBy(x => new { x.FromOfficer_Id, x.FromOfficerName }).Select(x => new InboxOfficersViewModel
                    {
                        Id = x.Key.FromOfficer_Id,
                        Name = x.Key.FromOfficerName,
                        Count = x.Count()
                    }).ToList();

                    return offices;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public OfficerFilesCount GetOfficerFilesCount(string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null)
                    {
                        if (userName.ToLower().StartsWith("ceo."))
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.DesignationName.Equals("Chief Executive Officer"));
                        }
                        if (userName.Length == 13)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        }
                    }
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.PandSOfficer_Id == currentOfficer.Id && x.ToOfficer_Id == null && x.IsActive == true).AsQueryable();
                    OfficerFilesCount officerFilesCount = new OfficerFilesCount();
                    officerFilesCount.Pending = query.Count();


                    IQueryable<ApplicationLog> recieved = _db.ApplicationLogs.Where(x => x.ToOfficer_Id == currentOfficer.Id && x.IsReceived == true && x.IsActive == true).AsQueryable();
                    officerFilesCount.Recieved = recieved.GroupBy(x => x.Application_Id).Count();

                    IQueryable<ApplicationLog> send = _db.ApplicationLogs.Where(x => x.FromOfficer_Id == currentOfficer.Id && x.IsActive == true).AsQueryable();
                    officerFilesCount.Sent = send.GroupBy(x => x.Application_Id).Count();

                    return officerFilesCount;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public OfficerFilesCount GetOfficerFilesFiles(int type, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer == null)
                    {
                        if (userName.ToLower().StartsWith("ceo."))
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.DesignationName.Equals("Chief Executive Officer"));
                        }
                        if (userName.Length == 13)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        }
                    }
                    if (currentOfficer == null) return null;
                    if (currentOfficer.DesignationName == "Front Desk Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                    }
                    if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                    }
                    if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                    }
                    if (currentOfficer.DesignationName == "R & I Branch")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                    }
                    if (currentOfficer.DesignationName == "Senior Law Officer")
                    {
                        currentOfficer = null;
                        currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                    }
                    if (currentOfficer.Code == 99999999)
                    {
                        var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                        if (higherOfficer != null)
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                        }
                    }
                    OfficerFilesCount officerFilesCount = new OfficerFilesCount();
                    if (type == 1)
                    {
                        IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.PandSOfficer_Id == currentOfficer.Id && x.ToOfficer_Id == null && x.IsActive == true).AsQueryable();
                        officerFilesCount.List = query.ToList();
                    }
                    else if (type == 2)
                    {
                        IQueryable<ApplicationLog> recieved = _db.ApplicationLogs.Where(x => x.ToOfficer_Id == currentOfficer.Id && x.IsReceived == true && x.IsActive == true).AsQueryable();
                        var sentFileIds = recieved.GroupBy(x => x.Application_Id).Select(x => x.Key.Value).ToList();
                        IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => sentFileIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                        officerFilesCount.List = query.ToList();

                    }
                    else if (type == 3)
                    {
                        IQueryable<ApplicationLog> send = _db.ApplicationLogs.Where(x => x.FromOfficer_Id == currentOfficer.Id && x.IsActive == true).AsQueryable();
                        var rcvedFileIds = send.GroupBy(x => x.Application_Id).Select(x => x.Key.Value).ToList();
                        IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => rcvedFileIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                        officerFilesCount.List = query.ToList();
                    }


                    return officerFilesCount;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<InboxOfficersViewModel> GetInboxOfficesHisdu(int officer_Id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    //var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == officer_Id);
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.ToOfficer_Id == officer_Id && x.IsPending == true && x.IsActive == true).AsQueryable();

                    var offices = query.GroupBy(x => new { x.FromOfficer_Id, x.FromOfficerName }).Select(x => new InboxOfficersViewModel
                    {
                        Id = x.Key.FromOfficer_Id,
                        Name = x.Key.FromOfficerName,
                        Count = x.Count()
                    }).ToList();

                    return offices;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public string CheckVacancy(int hf_Id, string hfmisCode, int designationId, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    //var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    //if (currentOfficer == null || !currentOfficer.Program.Equals("HISDU")) return "Unauthorized";
                    if (hfmisCode == null) return "No Vacancy Found";
                    var vpMaster = _db.VpMProfileViews.FirstOrDefault(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode) && x.Desg_Id == designationId);
                    if (vpMaster == null)
                    {
                        if (designationId == 802 || designationId == 1320 || designationId == 2404)
                        {
                            var vpMaster2 = _db.VpMProfileViews.FirstOrDefault(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode) && (x.Desg_Id == 802 || x.Desg_Id == 1320 || x.Desg_Id == 2404));
                            if (vpMaster2 == null)
                            {
                                return "No Vacancy Found";
                            }
                            if (vpMaster2.Vacant <= 0)
                            {
                                return "No Seat Vacant";
                            }
                            else if (vpMaster2.Vacant > 0)
                            {
                                return "SV";
                            }
                        }
                        else
                        {
                            return "No Vacancy Found";
                        }
                    }
                    if (vpMaster.Vacant <= 0)
                    {
                        return "No Seat Vacant";
                    }
                    else if (vpMaster.Vacant > 0)
                    {
                        return "SV";
                    }
                    return "Unauthorized";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public OrderDesignationResponse GetOrderDesignations(int hf_Id, string hfmisCode, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    OrderDesignationResponse orderDesignationResponse = new OrderDesignationResponse();
                    _db.Configuration.ProxyCreationEnabled = false;
                    orderDesignationResponse.vpMasters = _db.VpMasterProfileViews.Where(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode)).ToList();
                    //var vpDetails = _db.VpMProfileViews.Where(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode)).ToList();

                    return orderDesignationResponse;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool SubmitApplicationDocumentType(AppTypeDoc appTypeDoc, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var check = _db.AppTypeDocs.Where(x => appTypeDoc.ApplicationType_Id == x.ApplicationType_Id && appTypeDoc.Document_Id == x.Document_Id).ToList();
                    if (check.Count > 0) return false;
                    appTypeDoc.DateTime = DateTime.UtcNow.AddHours(5);
                    appTypeDoc.CreatedBy = userName;
                    appTypeDoc.User_Id = userId;

                    _db.AppTypeDocs.Add(appTypeDoc);
                    _db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ApplicationDocument SubmitApplicationDocument(ApplicationDocument document, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var check = _db.ApplicationDocuments.Where(x => x.Name == document.Name && x.IsActive == true).ToList();
                    if (check.Count > 0) return null;

                    document.DateTime = DateTime.UtcNow.AddHours(5);
                    document.CreatedBy = userName;
                    document.User_Id = userId;
                    document.IsActive = true;

                    _db.ApplicationDocuments.Add(document);
                    _db.SaveChanges();

                    return document;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public HrApplication SubmitHrApplication(HrApplication hrApplication, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    hrApplication.Datetime = DateTime.UtcNow.AddHours(5);
                    hrApplication.Username = userName;
                    hrApplication.UserId = userId;
                    hrApplication.IsActive = true;
                    _db.HrApplications.Add(hrApplication);
                    _db.SaveChanges();

                    return hrApplication;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool SubmitApplicationAttachments(List<ApplicationAttachment> applicationAttachments, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    foreach (var applicationAttachment in applicationAttachments)
                    {
                        ApplicationAttachment applicationAttachmentNew = new ApplicationAttachment();
                        applicationAttachmentNew.UploadPath = @"Uploads\Files\ApplicationAttachments\";
                        applicationAttachmentNew.Application_Id = applicationAttachment.Application_Id;
                        applicationAttachmentNew.Document_Id = applicationAttachment.Document_Id;
                        applicationAttachmentNew.IsActive = true;
                        applicationAttachmentNew.IsActive = true;
                        applicationAttachmentNew.IsBase64 = false;
                        _db.ApplicationAttachments.Add(applicationAttachmentNew);
                        _db.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ApplicationMaster> SubmitApplication(ApplicationMaster application, string userName, string userId, bool? administrativeOffice = false)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    string message = null;
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer != null)
                    {
                        if (currentOfficer.DesignationName == "Front Desk Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                        }
                        if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                        }
                        if (currentOfficer.DesignationName == "R & I Branch")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                        }
                        if (currentOfficer.DesignationName == "Citizen Portal")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 178);
                        }
                        if (currentOfficer.DesignationName == "Senior Law Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                        }
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                    }

                    if (currentOfficer == null)
                    {
                        return null;
                        //if (userName.ToLower().StartsWith("ceo."))
                        //{
                        //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 71);
                        //}
                        //if (userName.Length == 13)
                        //{
                        //    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                        //}
                    }
                    List<SMS> smsss = new List<SMS>();


                    if (userName.StartsWith("fdo") && !userName.Contains("south"))
                    {
                        application.ApplicationSource_Id = 1;
                    }
                    else if (userName.StartsWith("pl"))
                    {
                        application.ApplicationSource_Id = 2;
                    }
                    else if (userName.ToLower().StartsWith("ceo."))
                    {
                        application.ApplicationSource_Id = 4;
                    }
                    else if (userName.StartsWith("ri."))
                    {
                        application.ApplicationSource_Id = 5;
                        //application.ApplicationType_Id = 9;
                    }
                    else if (userName.StartsWith("cp."))
                    {
                        application.ApplicationSource_Id = 6;
                        application.ApplicationType_Id = 14;
                    }
                    else if (userName.Equals("slo"))
                    {
                        application.ApplicationSource_Id = 7;
                    }
                    else if (userName.Equals("pshd"))
                    {
                        application.ApplicationSource_Id = 8;
                    }
                    else if (userName.Contains("south"))
                    {
                        application.ApplicationSource_Id = 9;
                    }
                    else if (userName.StartsWith("office") || administrativeOffice == true)
                    {
                        application.ApplicationSource_Id = 10;
                    }
                    else
                    {
                        application.ApplicationSource_Id = 3;
                    }

                    if (application.IsPersonAppeared == false)
                    {
                        ApplicationPersonAppeared applicationPersonAppeared = application.ApplicationPersonAppeared;
                        _db.ApplicationPersonAppeareds.Add(applicationPersonAppeared);
                        _db.SaveChanges();
                        application.PersonAppeared_Id = applicationPersonAppeared.Id;
                    }
                    else
                    {
                        application.ApplicationPersonAppeared = null;
                    }
                    if (application.ApplicationType_Id == 1 && application.ApplicationSource_Id != 5)
                    {
                        if (application.FromDate.Value.Hour == 19)
                        {
                            application.FromDate = application.FromDate.Value.AddHours(5);
                        }
                        else
                        {
                            application.FromDate = application.FromDate;
                        }
                        if (application.ToDate.Value.Hour == 19)
                        {
                            application.ToDate = application.ToDate.Value.AddHours(5);
                        }
                        else
                        {
                            application.ToDate = application.ToDate;
                        }

                    }
                    else if (application.ApplicationType_Id == 10 && application.ApplicationSource_Id != 5)
                    {
                        if (application.AdhocExpireDate.Value.Hour == 19)
                        {
                            application.AdhocExpireDate = application.AdhocExpireDate.Value.AddHours(5);
                        }
                        else
                        {
                            application.AdhocExpireDate = application.AdhocExpireDate;
                        }
                    }


                    application.Created_Date = DateTime.UtcNow.AddHours(5);
                    application.Created_By = userName;
                    application.Users_Id = userId;
                    application.IsActive = true;

                    application.Status_Id = 9;
                    application.StatusTime = application.Created_Date;
                    application.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                    application.StatusByOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;

                    if (application.ApplicationSource_Id == 5 || application.ApplicationSource_Id == 8)
                    {
                        if (application.DispatchDated != null && application.DispatchDated.Value.Hour == 19)
                        {
                            application.DispatchDated = application.DispatchDated.Value.AddHours(5);
                        }
                        else
                        {
                            application.DispatchDated = application.DispatchDated;
                        }
                        application.Status_Id = 11;
                    }


                    if (application.ApplicationType_Id == 2 || application.ApplicationType_Id == 8)
                    {
                        VPMaster vpMaster = new VPMaster();
                        vpMaster = _db.VPMasters.FirstOrDefault(x => x.HF_Id == application.ToHF_Id && x.Desg_Id == application.ToDesignation_Id);
                        if (vpMaster != null)
                        {
                            application.VpMaster_Id = vpMaster.Id;
                        }
                    }

                    _db.ApplicationMasters.Add(application);
                    _db.SaveChanges();

                    application.TrackingNumber = application.Id + 9001;
                    _db.Entry(application).State = EntityState.Modified;
                    _db.SaveChanges();





                    if (!string.IsNullOrEmpty(application.MobileNo))
                    {
                        application.MobileNo = application.MobileNo.Replace("-", "");

                        if (application.ApplicationSource_Id == 1)
                        {
                            message = "Your application has been received at Facilitation Centre.\n\nTracking Number: " + application.TrackingNumber + "\n\nFrom: Primary and Secondary Healthcare Department.";
                        }
                        else if (application.ApplicationSource_Id == 2)
                        {
                            message = "Your application has been received at Parliamentarian Lounge.\n\nTracking Number: " + application.TrackingNumber + "\n\nFrom: Primary and Secondary Healthcare Department.";
                        }
                        else if (application.ApplicationSource_Id == 3)
                        {
                            message = "Your application has been recieved.\n\nTracking Number: " + application.TrackingNumber + "\n\nFrom: Primary and Secondary Healthcare Department.";
                        }
                        else if (application.ApplicationSource_Id == 5)
                        {
                            message = "Your application has been received at Diary and Dispatch Cell.\n\nTracking Number: " + application.TrackingNumber + "\n\nFrom: Primary and Secondary Healthcare Department.";
                        }

                        SMS sms = new SMS()
                        {
                            UserId = userId,
                            FKId = application.Id,
                            MobileNumber = application.MobileNo,
                            Message = message
                        };
                        //var jobId = BackgroundJob.Enqueue(() => Common.Common.SendSMSTelenor(sms));
                        //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                        //t.Start();
                        //await Common.Common.SendSMSTelenor(sms);
                        Common.Common.SendSMSTelenor(sms);

                        //var re = await Common.Common.SendSMSUfone(sms);


                    }
                    return application;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public ApplicationLog CreateApplicationLog(ApplicationLog applicationLog, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var application = _db.ApplicationMasters.FirstOrDefault(x => x.Id == applicationLog.Application_Id);
                var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                if (application != null)
                {
                    //if (application.ApplicationSource_Id == 10)
                    //{
                    //    applicationLog.ToStatus_Id = null;
                    //}
                }
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (currentOfficer == null)
                        {
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == applicationLog.FromOfficer_Id);
                        }
                        if (currentOfficer != null)
                        {
                            if (currentOfficer.DesignationName == "Front Desk Officer")
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                            }
                            if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                            }
                            if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                            }
                            if (currentOfficer.DesignationName == "R & I Branch")
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                            }
                            if (currentOfficer.DesignationName == "Senior Law Officer")
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                            }
                            if (currentOfficer.Code == 99999999)
                            {
                                var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                                if (higherOfficer != null)
                                {
                                    currentOfficer = null;
                                    currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                                }
                            }
                        }
                        if (currentOfficer == null)
                        {
                            if (userName.ToLower().StartsWith("ceo."))
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 71);
                            }
                            if (userName.ToLower().StartsWith("hr."))
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 70);
                            }
                            if (userName.Length == 13)
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                            }
                        }

                        applicationLog.DateTime = DateTime.UtcNow.AddHours(5);
                        applicationLog.CreatedBy = userName;
                        applicationLog.User_Id = userId;
                        applicationLog.IsActive = true;

                        if (applicationLog.Purpose_Id != null && applicationLog.Purpose_Id > 0)
                        {
                            application.Purpose_Id = applicationLog.Purpose_Id;
                            application.DueDate = applicationLog.DueDate;
                        }
                        if (applicationLog.ToOfficer_Id != null && applicationLog.ToOfficer_Id != 0)
                        {
                            var toOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == applicationLog.ToOfficer_Id);


                            application.ForwardTime = applicationLog.DateTime;
                            application.FromOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            application.FromOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;

                            // Facilitation or R&I Marked Application to concerned officer
                            //if (currentOfficer.Id == 79 || currentOfficer.Id == 144 || currentOfficer.Id == 354 || (currentOfficer.Id == 1 && application.ApplicationType_Id == 77))
                            //{
                            //    var appTypePendancy = _db.AppTypePendancies.FirstOrDefault(x => x.ApplicationType_Id == application.ApplicationType_Id && x.IsPendancy == true);
                            //    if (appTypePendancy != null)
                            //    {
                            //        var fromStatus = _db.ApplicationStatus.FirstOrDefault(x => x.Id == application.Status_Id);

                            //        applicationLog.FromStatus_Id = fromStatus.Id;
                            //        applicationLog.FromStatus = fromStatus.Name;

                            //        applicationLog.IsReceived = true;
                            //        applicationLog.Action_Id = 2;
                            //        applicationLog.FromOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            //        applicationLog.FromOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;
                            //        applicationLog.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            //        applicationLog.StatusByOfficer = currentOfficer != null ? currentOfficer.DesignationName : null;
                            //        _db.ApplicationLogs.Add(applicationLog);
                            //        _db.SaveChanges();

                            //        application.CurrentLog_Id = applicationLog.Id;
                            //        application.PandSOfficer_Id = toOfficer.Id;
                            //        application.PandSOfficerName = toOfficer.DesignationName;
                            //        application.Status_Id = applicationLog.ToStatus_Id == null ? application.Status_Id : applicationLog.ToStatus_Id;
                            //        application.StatusTime = applicationLog.DateTime;
                            //        application.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            //        application.StatusByOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;

                            //        application.IsPending = false;
                            //        application.RecieveTime = DateTime.UtcNow.AddHours(5);
                            //        _db.SaveChanges();
                            //        transc.Commit();
                            //        return applicationLog;
                            //    }
                            //    if (applicationLog.IsReceived == true)
                            //    {
                            //        application.PandSOfficer_Id = toOfficer.Id;
                            //        application.PandSOfficerName = toOfficer.DesignationName;
                            //        application.IsPending = !applicationLog.IsReceived;
                            //    }
                            //    else
                            //    {
                            //        application.ToOfficer_Id = toOfficer.Id;
                            //        application.ToOfficerName = toOfficer.DesignationName;
                            //        application.IsPending = true;
                            //    }
                            //}
                            //else
                            //{
                            //    application.ToOfficer_Id = toOfficer.Id;
                            //    application.ToOfficerName = toOfficer.DesignationName;
                            //    application.IsPending = true;
                            //}

                            application.ToOfficer_Id = toOfficer.Id;
                            application.ToOfficerName = toOfficer.DesignationName;
                            application.IsPending = true;
                            application.RecieveTime = null;
                            applicationLog.FromOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            applicationLog.FromOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;
                            applicationLog.Action_Id = 3;
                        }
                        if (applicationLog.ToStatus_Id != null && applicationLog.ToStatus_Id != 0)
                        {
                            var fromStatus = _db.ApplicationStatus.FirstOrDefault(x => x.Id == application.Status_Id);

                            applicationLog.FromStatus_Id = fromStatus.Id;
                            applicationLog.FromStatus = fromStatus.Name;

                            applicationLog.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            applicationLog.StatusByOfficer = currentOfficer != null ? currentOfficer.DesignationName : null;

                            application.Status_Id = applicationLog.ToStatus_Id;
                            application.StatusTime = applicationLog.DateTime;

                            application.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            application.StatusByOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;
                            if (applicationLog.FromStatus_Id == 9 && applicationLog.ToStatus_Id == 11)
                            {
                                applicationLog.Action_Id = 2;
                            }
                            else
                            {
                                applicationLog.Action_Id = 4;
                            }
                        }
                        if (application.Status_Id == 10 && applicationLog.Action_Id == 3)
                        {
                            applicationLog.ToStatus_Id = 1;
                            applicationLog.ToStatus = "Under Process";

                            application.Status_Id = 1;
                            application.StatusTime = applicationLog.DateTime;
                            application.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            application.StatusByOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;
                        }
                        if (applicationLog.ToStatus_Id == 2 && (application.ApplicationType_Id == 2 || application.ApplicationType_Id == 8))
                        {
                            VPMaster vpMaster = new VPMaster();
                            if (application.VpMaster_Id != null && application.VpMaster_Id != 0)
                            {
                                vpMaster = _db.VPMasters.FirstOrDefault(x => x.Id == application.VpMaster_Id);
                            }
                            else
                            {
                                vpMaster = _db.VPMasters.FirstOrDefault(x => x.HF_Id == application.ToHF_Id && x.Desg_Id == application.ToDesignation_Id);
                                if (vpMaster != null)
                                {
                                    application.VpMaster_Id = vpMaster.Id;
                                    _db.SaveChanges();
                                }

                            }
                            if (vpMaster != null)
                            {
                                vpMaster.TotalApprovals = vpMaster.TotalApprovals == null ? 0 : vpMaster.TotalApprovals;
                                vpMaster.TotalApprovals += 1;
                                _db.SaveChanges();
                            }
                        }
                        if (applicationLog.Remarks != null)
                        {
                            if (applicationLog.Action_Id != 3 && applicationLog.Action_Id != 4 && applicationLog.Action_Id != 17)
                            {
                                if (application.Status_Id == 10)
                                {
                                    applicationLog.Action_Id = 15;
                                    application.Status_Id = 1;
                                    application.StatusTime = applicationLog.DateTime;
                                    application.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                                    application.StatusByOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;

                                    applicationLog.FromStatus_Id = 10;
                                    applicationLog.FromStatus = "No Process Initiated";

                                    applicationLog.ToStatus_Id = 1;
                                    applicationLog.ToStatus = "Under Process";

                                    applicationLog.StatusByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                                    applicationLog.StatusByOfficer = currentOfficer != null ? currentOfficer.DesignationName : null;
                                }
                                else
                                {
                                    applicationLog.RemarksByOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                                    applicationLog.RemarksByOfficer = currentOfficer != null ? currentOfficer.DesignationName : null;
                                    applicationLog.Action_Id = 9;
                                }
                            }

                            application.RemarksTime = applicationLog.DateTime;
                        }

                        if (application.Status_Id == 11 && applicationLog.Action_Id != 17)
                        {
                            applicationLog.Action_Id = 2;
                            application.PandSOfficer_Id = currentOfficer != null ? currentOfficer.Id : 0;
                            application.PandSOfficerName = currentOfficer != null ? currentOfficer.DesignationName : null;
                        }
                        if (currentOfficer.Id == 70)
                        {
                            applicationLog.StatusByOfficer = currentOfficer.DesignationName;
                            applicationLog.StatusByOfficer_Id = currentOfficer.Id;
                            applicationLog.Action_Id = 10;
                        }
                        if (currentOfficer.Id == 155)
                        {
                            applicationLog.Action_Id = 12;
                        }
                        _db.ApplicationLogs.Add(applicationLog);
                        _db.SaveChanges();
                        application.CurrentLog_Id = applicationLog.Id;
                        _db.SaveChanges();
                        transc.Commit();
                        if (applicationLog.SMS_SentToApplicant == true)
                        {
                            List<SMS> smsList = new List<SMS>();
                            string message = null;
                            if (application.Status_Id == 2)
                            {
                                message = "Application Approved\n\nTracking No. " + application.TrackingNumber +
                                    " is approved by " + application.StatusByOfficerName +
                                    "\n\nRemarks:\n" + applicationLog.Remarks;
                            }
                            else if (application.Status_Id == 3)
                            {
                                message = "Application Rejected\n\nTracking No. " + application.TrackingNumber +
                                    " is rejected by " + application.StatusByOfficerName +
                                    "\n\nRemarks:\n" + applicationLog.Remarks;
                            }
                            else if (application.Status_Id == 8)
                            {
                                message = "Application Waiting Documents\n\nDear Applicant, Tracking No. " + application.TrackingNumber +
                                    " is under waiting documents by " + application.StatusByOfficerName +
                                    "\n\nRemarks:\n" + applicationLog.Remarks;
                            }
                            Common.Common.SendSMSTelenor(new SMS { Message = message, MobileNumber = application.MobileNo });
                        }
                        return applicationLog;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }

            }
        }
        public FileMoveMaster SubmitFileMovement(List<int> applicationIds, string userName, string userId)
        {

            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var applications = _db.ApplicationMasters.Where(x => applicationIds.Contains(x.Id)).ToList();
                var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (currentOfficer == null)
                        {
                            if (userName.ToLower().StartsWith("ceo."))
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 71);
                            }
                            else if (userName.Length == 13)
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                        }
                        if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                        }
                        if (currentOfficer.DesignationName == "R & I Branch")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                        }
                        if (currentOfficer.DesignationName == "Senior Law Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                        }
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                        FileMoveMaster fileMoveMaster = new FileMoveMaster();

                        //fileMoveMaster.FromOfficer_Id = applications.FirstOrDefault().FromOfficer_Id;
                        fileMoveMaster.ToOfficer_Id = applications.FirstOrDefault().ToOfficer_Id;
                        fileMoveMaster.FileType_Id = 1;
                        fileMoveMaster.IsRecieved = false;

                        fileMoveMaster.DateTime = DateTime.UtcNow.AddHours(5);
                        fileMoveMaster.CreatedBy = userName;
                        fileMoveMaster.User_Id = userId;
                        fileMoveMaster.IsActive = true;

                        _db.FileMoveMasters.Add(fileMoveMaster);
                        _db.SaveChanges();

                        fileMoveMaster.MID_Number = fileMoveMaster.Id + 1001;
                        _db.Entry(fileMoveMaster).State = EntityState.Modified;
                        _db.SaveChanges();

                        foreach (var application in applications)
                        {
                            FileMoveDetail fileMoveDetail = new FileMoveDetail();
                            fileMoveDetail.Application_Id = application.Id;
                            fileMoveDetail.Master_Id = fileMoveMaster.Id;
                            fileMoveDetail.DateTime = fileMoveMaster.DateTime;
                            fileMoveDetail.CreatedBy = userName;
                            fileMoveDetail.User_Id = userId;
                            fileMoveDetail.IsActive = true;
                            fileMoveMaster.IsRecieved = true;
                            fileMoveMaster.RecievedTime = fileMoveDetail.DateTime;

                            var app = _db.ApplicationMasters.FirstOrDefault(x => x.Id == application.Id);

                            if (app.Status_Id == 11 && currentOfficer.Code.Value.ToString().Length >= 5)
                            {
                                app.Status_Id = 10;
                                app.StatusTime = fileMoveMaster.DateTime;
                                app.StatusByOfficer_Id = currentOfficer.Id;
                                app.StatusByOfficerName = currentOfficer.DesignationName;
                            }
                            else if (app.Status_Id == 11 && currentOfficer.Code.Value.ToString().Length < 5)
                            {
                                app.Status_Id = 1;
                                app.StatusTime = fileMoveMaster.DateTime;
                                app.StatusByOfficer_Id = currentOfficer.Id;
                                app.StatusByOfficerName = currentOfficer.DesignationName;
                            }
                            var appLog = new ApplicationLog();
                            appLog.Application_Id = application.Id;
                            appLog.Action_Id = 14;

                            if (application.Status_Id == 11 && currentOfficer.Code.Value.ToString().Length >= 5)
                            {
                                appLog.FromStatus_Id = 11;
                                appLog.FromStatus = "Marked";

                                appLog.ToStatus_Id = 10;
                                appLog.ToStatus = "No Process Initiated";

                                appLog.StatusByOfficer_Id = currentOfficer.Id;
                                appLog.StatusByOfficer = currentOfficer.DesignationName;
                            }
                            else if (application.Status_Id == 11 && currentOfficer.Code.Value.ToString().Length < 5)
                            {
                                appLog.FromStatus_Id = 11;
                                appLog.FromStatus = "Marked";

                                appLog.ToStatus_Id = 1;
                                appLog.ToStatus = "Under Process";

                                appLog.StatusByOfficer_Id = currentOfficer.Id;
                                appLog.StatusByOfficer = currentOfficer.DesignationName;
                            }

                            var fromOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == app.FromOfficer_Id);
                            var toOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == app.ToOfficer_Id);

                            appLog.FromOfficer_Id = fromOfficer.Id;
                            fileMoveDetail.FromOfficer_Id = appLog.FromOfficer_Id;
                            appLog.FromOfficerName = fromOfficer.DesignationName;
                            appLog.ToOfficer_Id = toOfficer.Id;
                            fileMoveDetail.ToOfficer_Id = appLog.ToOfficer_Id;
                            appLog.ToOfficerName = toOfficer.DesignationName;
                            appLog.IsReceived = true;
                            appLog.ReceivedTime = fileMoveMaster.DateTime;
                            appLog.DateTime = appLog.ReceivedTime;
                            appLog.CreatedBy = userName;
                            appLog.User_Id = userId;
                            appLog.IsActive = true;
                            _db.ApplicationLogs.Add(appLog);
                            _db.SaveChanges();

                            app.IsPending = false;
                            app.RecieveTime = appLog.ReceivedTime;
                            app.CurrentLog_Id = appLog.Id;
                            app.PandSOfficer_Id = toOfficer.Id;
                            app.PandSOfficerName = toOfficer.DesignationName;

                            app.ToOfficer_Id = null;
                            app.ToOfficerName = null;
                            _db.Entry(app).State = EntityState.Modified;
                            _db.SaveChanges();

                            if (app.DDS_Id != null && app.DDS_Id != 0)
                            {
                                fileMoveDetail.DDS_Id = app.DDS_Id;
                            }
                            if (app.FileRequest_Id != null && app.FileRequest_Id != 0)
                            {
                                fileMoveDetail.FileRequisition_Id = app.FileRequest_Id;
                            }
                            _db.FileMoveDetails.Add(fileMoveDetail);
                            _db.SaveChanges();
                        }
                        transc.Commit();
                        return fileMoveMaster;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public FileMoveMaster SubmitFileMovement2(List<ApplicationMaster> applications, string userName, string userId)
        {

            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (currentOfficer == null)
                        {
                            if (userName.ToLower().StartsWith("ceo."))
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 71);
                            }
                            else if (userName.Length == 13)
                            {
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 164);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                        }
                        if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                        }
                        if (currentOfficer.DesignationName == "R & I Branch")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                        }
                        if (currentOfficer.DesignationName == "Senior Law Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                        }
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                        FileMoveMaster fileMoveMaster = new FileMoveMaster();

                        fileMoveMaster.FromOfficer_Id = currentOfficer.Id;
                        fileMoveMaster.ToOfficer_Id = applications.FirstOrDefault().PandSOfficer_Id;
                        fileMoveMaster.FileType_Id = 1;
                        fileMoveMaster.IsRecieved = false;

                        fileMoveMaster.DateTime = DateTime.UtcNow.AddHours(5);
                        fileMoveMaster.CreatedBy = userName;
                        fileMoveMaster.User_Id = userId;
                        fileMoveMaster.IsActive = true;

                        _db.FileMoveMasters.Add(fileMoveMaster);
                        _db.SaveChanges();

                        fileMoveMaster.MID_Number = fileMoveMaster.Id + 1001;
                        _db.Entry(fileMoveMaster).State = EntityState.Modified;
                        _db.SaveChanges();

                        foreach (var application in applications)
                        {
                            FileMoveDetail fileMoveDetail = new FileMoveDetail();
                            fileMoveDetail.Application_Id = application.Id;
                            fileMoveDetail.Master_Id = fileMoveMaster.Id;
                            fileMoveDetail.IsActive = true;
                            if (application.DDS_Id != null && application.DDS_Id != 0)
                            {
                                fileMoveDetail.DDS_Id = application.DDS_Id;
                            }
                            if (application.FileRequest_Id != null && application.FileRequest_Id != 0)
                            {
                                fileMoveDetail.FileRequisition_Id = application.FileRequest_Id;
                            }
                            _db.FileMoveDetails.Add(fileMoveDetail);
                            _db.SaveChanges();
                        }
                        transc.Commit();
                        return fileMoveMaster;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public FileMoveMaster SubmitFileMovementFDO(List<ApplicationMaster> applications, int officer_Id, string userName, string userId)
        {

            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == officer_Id);
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (currentOfficer == null) return null;
                        if (currentOfficer.DesignationName == "Front Desk Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                        }
                        if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                        }
                        if (currentOfficer.DesignationName == "R & I Branch")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                        }
                        if (currentOfficer.DesignationName == "Senior Law Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                        }
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                        FileMoveMaster fileMoveMaster = new FileMoveMaster();

                        fileMoveMaster.FromOfficer_Id = applications.FirstOrDefault().FromOfficer_Id;
                        fileMoveMaster.ToOfficer_Id = applications.FirstOrDefault().ToOfficer_Id;
                        fileMoveMaster.FileType_Id = 1;
                        fileMoveMaster.IsRecieved = false;

                        fileMoveMaster.DateTime = DateTime.UtcNow.AddHours(5);
                        fileMoveMaster.CreatedBy = userName;
                        fileMoveMaster.User_Id = userId;
                        fileMoveMaster.IsActive = true;

                        _db.FileMoveMasters.Add(fileMoveMaster);
                        _db.SaveChanges();

                        fileMoveMaster.MID_Number = fileMoveMaster.Id + 1001;
                        _db.Entry(fileMoveMaster).State = EntityState.Modified;
                        _db.SaveChanges();

                        foreach (var application in applications)
                        {
                            FileMoveDetail fileMoveDetail = new FileMoveDetail();
                            fileMoveDetail.Application_Id = application.Id;
                            fileMoveDetail.Master_Id = fileMoveMaster.Id;
                            fileMoveDetail.IsActive = true;


                            var app = _db.ApplicationMasters.FirstOrDefault(x => x.Id == application.Id);

                            if (app.Status_Id == 11)
                            {
                                app.Status_Id = 10;
                                app.StatusTime = fileMoveMaster.DateTime;
                                app.StatusByOfficer_Id = currentOfficer.Id;
                                app.StatusByOfficerName = currentOfficer.DesignationName;
                            }

                            var appLog = new ApplicationLog();
                            appLog.Application_Id = application.Id;
                            appLog.Action_Id = 14;

                            if (application.Status_Id == 11)
                            {
                                appLog.FromStatus_Id = 11;
                                appLog.FromStatus = "Marked";

                                appLog.ToStatus_Id = 10;
                                appLog.ToStatus = "No Process Initiated";

                                appLog.StatusByOfficer_Id = currentOfficer.Id;
                                appLog.StatusByOfficer = currentOfficer.DesignationName;
                            }

                            var fromOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == fileMoveMaster.FromOfficer_Id);
                            var toOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == fileMoveMaster.ToOfficer_Id);

                            appLog.FromOfficer_Id = fromOfficer.Id;
                            appLog.FromOfficerName = fromOfficer.DesignationName;
                            appLog.ToOfficer_Id = toOfficer.Id;
                            appLog.ToOfficerName = toOfficer.DesignationName;
                            appLog.IsReceived = true;
                            appLog.ReceivedTime = fileMoveMaster.DateTime;
                            appLog.DateTime = appLog.ReceivedTime;
                            appLog.CreatedBy = userName;
                            appLog.User_Id = userId;
                            appLog.IsActive = true;
                            _db.ApplicationLogs.Add(appLog);
                            _db.SaveChanges();

                            app.IsPending = false;
                            app.RecieveTime = appLog.ReceivedTime;
                            app.CurrentLog_Id = appLog.Id;
                            app.PandSOfficer_Id = toOfficer.Id;
                            app.PandSOfficerName = toOfficer.DesignationName;

                            app.ToOfficer_Id = null;
                            app.ToOfficerName = null;
                            _db.Entry(app).State = EntityState.Modified;
                            _db.SaveChanges();

                            if (app.DDS_Id != null && app.DDS_Id != 0)
                            {
                                fileMoveDetail.DDS_Id = app.DDS_Id;
                            }
                            if (app.FileRequest_Id != null && app.FileRequest_Id != 0)
                            {
                                fileMoveDetail.FileRequisition_Id = app.FileRequest_Id;
                            }
                            _db.FileMoveDetails.Add(fileMoveDetail);
                            _db.SaveChanges();
                        }
                        transc.Commit();
                        return fileMoveMaster;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }

        public ApplicationProfileViewModel SearchProfile(string cnic)
        {
            using (var _db = new HR_System())
            {
                cnic = cnic.Replace("-", "");
                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.ProfileSeniorityViews.Select(k => new ApplicationProfileViewModel
                {
                    Profile_Id = k.Id,
                    EmployeeName = k.EmployeeName,
                    FatherName = k.FatherName,
                    CNIC = k.CNIC,
                    DateOfBirth = k.DateOfBirth,
                    Gender = k.Gender,
                    MobileNo = k.MobileNo,
                    EMaiL = k.EMaiL,
                    JoiningGradeBPS = k.JoiningGradeBPS,
                    JoiningDate = k.JoiningDate,
                    CurrentGradeBPS = k.CurrentGradeBPS,
                    Address = k.PermanentAddress,
                    SeniorityNo = k.SeniorityNo,
                    Department_Id = k.Department_Id,
                    Designation_Id = k.Designation_Id,
                    EmpMode_Id = k.EmpMode_Id,
                    EmpStatus_Id = k.Status_Id,
                    HfmisCode = k.HfmisCode,
                    StatusName = k.StatusName,
                    HealthFacility = k.HealthFacility + ", " + k.Tehsil + ", " + k.District,
                    Designation_Name = k.Designation_Name,
                    HealthFacility_Id = k.HealthFacility_Id
                }).FirstOrDefault(x => x.CNIC.Equals(cnic));

                if (profile == null)
                {
                    profile = _db.ApplicationMasters.Select(k => new ApplicationProfileViewModel
                    {
                        Profile_Id = (int)k.Profile_Id,
                        EmployeeName = k.EmployeeName,
                        FatherName = k.FatherName,
                        CNIC = k.CNIC,
                        DateOfBirth = k.DateOfBirth,
                        Gender = k.Gender,
                        MobileNo = k.MobileNo,
                        EMaiL = k.EMaiL,
                        JoiningGradeBPS = k.JoiningGradeBPS,
                        CurrentGradeBPS = k.CurrentGradeBPS,
                        Address = k.DispatchFrom,
                        SeniorityNo = k.SeniorityNumber,
                        Department_Id = k.Department_Id,
                        Designation_Id = k.Designation_Id,
                        EmpMode_Id = k.EmpMode_Id,
                        EmpStatus_Id = k.Status_Id,
                        HfmisCode = k.HfmisCode,
                        HealthFacility_Id = k.HealthFacility_Id
                    }).FirstOrDefault(x => x.CNIC.Equals(cnic));
                }
                return profile;
            }

        }
        public List<HFList> SearchHealthFacilities(string query, string hfmiscode)
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
                        (x.HFMISCode.Equals(hfmiscode) || x.HFMISCode.StartsWith(hfmiscode)) && x.IsActive == true)
                        .ToList();
                    return hfs;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public ApplicationFts GetApplicationForLetter(int Id, int Tracking, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer != null)
                    {
                        if (currentOfficer.DesignationName == "Front Desk Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 79);
                        }
                        if (currentOfficer.DesignationName == "Front Desk Officer South Punjab")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 354);
                        }
                        if (currentOfficer.DesignationName == "Parliamentarian Lounge")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 80);
                        }
                        if (currentOfficer.DesignationName == "R & I Branch")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 144);
                        }
                        if (currentOfficer.DesignationName == "Senior Law Officer")
                        {
                            currentOfficer = null;
                            currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == 210);
                        }
                        if (currentOfficer.Code == 99999999)
                        {
                            var higherOfficer = _db.P_SConcernedOfficers.FirstOrDefault(x => x.ConcernedOfficer_Id == currentOfficer.Id);
                            if (higherOfficer != null)
                            {
                                currentOfficer = null;
                                currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == higherOfficer.Officer_Id);
                            }
                        }
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.IsActive == true);
                        applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                        return applicationFts;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<P_SOfficers> GetOfficersForLetter(int Id, int Tracking, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    List<P_SOfficers> officersCC = new List<P_SOfficers>();
                    var currentPSOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentPSOfficer != null)
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.TrackingNumber == Tracking && x.IsActive == true);
                        var psOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == applicationFts.application.PandSOfficer_Id);
                        applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                        foreach (var item in _db.P_SOfficers)
                        {
                            if (currentPSOfficer.Code.ToString().StartsWith(item.Code.ToString()) && item.Code != null)
                            {
                                officersCC.Add(item);
                            }
                        }
                        return officersCC;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public async Task<OrderResponse> GenerateOrder(ApplicationMaster applicationMaster, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                string type = "";
                ESR esr = new ESR();
                LeaveOrder leaveOrder = new LeaveOrder();
                HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == applicationMaster.Profile_Id);
                ApplicationLog applicationLog = new ApplicationLog();
                AppOrder appOrder = new AppOrder();
                var profileView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == applicationMaster.Profile_Id);
                if (profile == null)
                {
                    return null;
                }
                if (applicationMaster.ApplicationType_Id == 5)
                {
                    leaveOrder.Profile_Id = profile.Id;
                    leaveOrder.LeaveType_Id = applicationMaster.LeaveType_Id;
                    leaveOrder.TotalDays = applicationMaster.TotalDays;
                    leaveOrder.FileNumber = applicationMaster.FileNumber;
                    leaveOrder.OrderHTML = applicationMaster.RawText;
                    leaveOrder.SignedBy = applicationMaster.ForwardingOfficerName;
                    leaveOrder.Officer_Id = 30;
                    leaveOrder.CombinedOrder_Id = applicationMaster.ApplicationSource_Id;
                    type = "Leave Order";

                    if (applicationMaster.FromDate != null)
                    {
                        leaveOrder.FromDate = applicationMaster.FromDate.Value.Hour == 19 ? applicationMaster.FromDate.Value.AddHours(5) : applicationMaster.FromDate.Value;
                    }
                    if (applicationMaster.ToDate != null)
                    {
                        leaveOrder.ToDate = applicationMaster.ToDate.Value.Hour == 19 ? applicationMaster.ToDate.Value.AddHours(5) : applicationMaster.ToDate.Value;
                    }
                    if (applicationMaster.OrderDate != null)
                    {
                        leaveOrder.OrderDate = applicationMaster.OrderDate.Value.Hour == 19 ? applicationMaster.OrderDate.Value.AddHours(5) : applicationMaster.OrderDate.Value;
                    }
                    // move profile to PSHD if leave is more than 89 days and make seat vacant there 

                    if (leaveOrder.LeaveType_Id != 7 && leaveOrder.LeaveType_Id != 17 && leaveOrder.LeaveType_Id != 21 && leaveOrder.LeaveType_Id != 28)
                    {
                        if (leaveOrder.TotalDays > 89)
                        {
                            _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, userName, userId);

                            profile.HfmisCode = "0350020010030010002";
                            profile.HealthFacility_Id = 11606;

                            profile.WorkingHFMISCode = "0350020010030010002";
                            profile.WorkingHealthFacility_Id = 11606;

                            profile.Status_Id = 17;

                            if (profile.EntityLifecycle_Id == null)
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Created_By = userName + " (added after migration)";
                                elc.Users_Id = userId;
                                elc.IsActive = true;
                                elc.Entity_Id = 9;
                                db.Entity_Lifecycle.Add(elc);
                                db.SaveChanges();
                                profile.EntityLifecycle_Id = elc.Id;
                            }
                            Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                            emlProfile.Modified_By = userId;
                            emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                            emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                            emlProfile.Description = "Profile edited during order generation By " + userName;
                            db.Entity_Modified_Log.Add(emlProfile);
                            db.SaveChanges();
                        }
                    }

                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 59;
                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();
                    leaveOrder.EntityLifecycle_Id = eld.Id;
                    db.LeaveOrders.Add(leaveOrder);
                    db.SaveChanges();
                    appOrder.ELR_Id = leaveOrder.Id;

                    var userService = new UserService();
                    var hfmisCode = userService.GetUser(userId).hfmiscode;
                    //await SendSMS(profileView, hfmisCode, leaveOrder.Id, userId);
                }
                else
                {
                    esr.TransferTypeID = applicationMaster.ApplicationType_Id;
                    esr.Profile_Id = applicationMaster.Profile_Id;
                    esr.CNIC = profile.CNIC;
                    esr.DepartmentFrom = applicationMaster.Department_Id;
                    esr.DesignationFrom = applicationMaster.Designation_Id;
                    esr.HF_Id_From = applicationMaster.HealthFacility_Id;
                    esr.HfmisCodeFrom = applicationMaster.HfmisCode;
                    esr.BPSFrom = applicationMaster.CurrentScale;
                    //general transfer
                    if (esr.TransferTypeID == 4 || esr.TransferTypeID == 6)
                    {
                        type = "General Transfer";
                        esr.BPSTo = applicationMaster.ToScale;
                        esr.DesignationTo = applicationMaster.ToDesignation_Id;
                        esr.DepartmentTo = applicationMaster.ToDept_Id;
                        esr.HF_Id_To = applicationMaster.ToHF_Id;
                        esr.HfmisCodeTo = applicationMaster.ToHFCode;
                    }
                    //Disposal
                    else if (esr.TransferTypeID == 2)
                    {
                        type = "Disposal Order";
                        esr.DepartmentTo = applicationMaster.ToDept_Id;
                        esr.DisposalofID = applicationMaster.ToDept_Id;
                        var dispoalOf = db.DisposalOfs.FirstOrDefault(x => x.Id == esr.DisposalofID);
                        if (dispoalOf != null)
                        {
                            esr.Disposalof = dispoalOf.Name;
                        }
                        esr.HF_Id_To = applicationMaster.ToHF_Id;
                        esr.HfmisCodeTo = applicationMaster.ToHFCode;
                    }
                    //Adhoc Appointer
                    else if (esr.TransferTypeID == 8)
                    {
                        type = "Adhoc Appointment";
                        esr.BPSTo = applicationMaster.ToScale;
                        esr.DesignationTo = applicationMaster.ToDesignation_Id;
                        esr.DepartmentTo = applicationMaster.ToDept_Id;
                        esr.HF_Id_To = applicationMaster.ToHF_Id;
                        esr.HfmisCodeTo = applicationMaster.ToHFCode;
                        if (applicationMaster.AdhocExpireDate == null)
                        {
                            esr.AppointmentEffect = "with immediate effect";
                        }
                        else
                        {
                            esr.AppointmentEffect = "with effect from";
                            esr.AppointmentDate = applicationMaster.AdhocExpireDate.Value.Hour == 19 ? applicationMaster.AdhocExpireDate.Value.AddHours(5) : applicationMaster.AdhocExpireDate.Value;
                        }
                    }
                    esr.EmployeeFileNO = applicationMaster.FileNumber;
                    esr.SectionOfficer = applicationMaster.ForwardingOfficerName;
                    //esr.EsrSectionOfficerID = ????;
                    esr.MutualESR_Id = applicationMaster.ApplicationSource_Id;
                    esr.COMMENTS = applicationMaster.Reason;
                    esr.Remarks = applicationMaster.Remarks;
                    esr.OrderHTML = applicationMaster.RawText;
                    esr.ResponsibleUser = userName;
                    esr.IsActive = true;
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 5;

                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();

                    esr.EntityLifecycle_Id = eld.Id;
                    esr.ResponsibleUser = eld.Created_By;
                    db.ESRs.Add(esr);
                    profile.AddToEmployeePool = false;
                    db.SaveChanges();
                    appOrder.ESR_Id = esr.Id;
                }
                var profiles = new List<HrProfile>() { profile };
                var esrView = db.ESRViews.FirstOrDefault(x => x.Id == esr.Id);
                if (esrView != null)
                {
                    type = string.IsNullOrEmpty(type) ? "Order" : type;
                }
                LeaveOrderView leaveOrderView = db.LeaveOrderViews.FirstOrDefault(x => x.Id == leaveOrder.Id);

                if (applicationMaster.TrackingNumber != null && applicationMaster.TrackingNumber > 0)
                {
                    var application = db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == applicationMaster.TrackingNumber);
                    if (application != null)
                    {
                        applicationLog.Application_Id = application.Id;
                        //Order Issued
                        applicationLog.SMS_SentToApplicant = true;
                        //Check if Application status is not Approved
                        if (application.Status_Id != 2)
                        {
                            applicationLog.ToStatus_Id = 2;
                        }
                        CreateApplicationLog(applicationLog, userName, userId);
                        appOrder.Application_Id = application.Id;
                        appOrder.TrackingNo = application.TrackingNumber;
                        appOrder.IsActive = true;
                        appOrder.CreatedBy = userName;
                        appOrder.User_Id = userId;
                        appOrder.DateTime = DateTime.UtcNow.AddHours(5);
                        db.AppOrders.Add(appOrder);
                        db.SaveChanges();
                    }
                }

                if (applicationMaster.ApplicationType_Id != 5)
                {
                    await ChangeSystemWithOrder((int)esr.Id, userName, userId);
                }
                return new OrderResponse
                {
                    esr = esr,
                    esrView = esrView,
                    applicationMaster = applicationMaster,
                    leaveOrder = leaveOrder,
                    leaveOrderView = leaveOrderView,
                    Profiles = profiles,
                    orderType = type
                };
            }
        }
        public async Task<OrderResponse> GenerateMutualOrder(MutualOrderDto mutualOrderDto, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                ESR esr = new ESR();
                ESR esr2 = new ESR();
                HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == mutualOrderDto.applicationMaster.Profile_Id);
                HrProfile profile2 = db.HrProfiles.FirstOrDefault(x => x.Id == mutualOrderDto.applicationMaster2.Profile_Id);
                ApplicationLog applicationLog = new ApplicationLog();
                AppOrder appOrder = new AppOrder();
                var profileView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == mutualOrderDto.applicationMaster.Profile_Id);
                var profileView2 = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == mutualOrderDto.applicationMaster2.Profile_Id);
                if (profile == null || profile2 == null)
                {
                    return null;
                }

                esr.TransferTypeID = 1;
                esr.Profile_Id = mutualOrderDto.applicationMaster.Profile_Id;
                esr.CNIC = profile.CNIC;
                esr.DepartmentFrom = mutualOrderDto.applicationMaster.Department_Id;
                esr.DesignationFrom = mutualOrderDto.applicationMaster.Designation_Id;
                esr.HF_Id_From = mutualOrderDto.applicationMaster.HealthFacility_Id;
                esr.HfmisCodeFrom = mutualOrderDto.applicationMaster.HfmisCode;
                esr.BPSFrom = mutualOrderDto.applicationMaster.CurrentScale;

                esr.DepartmentTo = mutualOrderDto.applicationMaster2.Department_Id;
                esr.DesignationTo = mutualOrderDto.applicationMaster2.Designation_Id;
                esr.HF_Id_To = mutualOrderDto.applicationMaster2.HealthFacility_Id;
                esr.HfmisCodeTo = mutualOrderDto.applicationMaster2.HfmisCode;
                esr.BPSTo = mutualOrderDto.applicationMaster2.CurrentScale;

                esr.EmployeeFileNO = mutualOrderDto.applicationMaster.FileNumber;
                esr.SectionOfficer = mutualOrderDto.applicationMaster.ForwardingOfficerName;
                esr.MutualESR_Id = null;
                esr.COMMENTS = mutualOrderDto.applicationMaster.Reason;
                esr.Remarks = mutualOrderDto.applicationMaster.Remarks;
                esr.OrderHTML = mutualOrderDto.applicationMaster.RawText;
                esr.ResponsibleUser = userName;
                esr.IsActive = true;
                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                eld.Created_By = userName;
                eld.Users_Id = userId;
                eld.IsActive = true;
                eld.Entity_Id = 5;

                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();

                esr.EntityLifecycle_Id = eld.Id;
                esr.ResponsibleUser = eld.Created_By;
                db.ESRs.Add(esr);
                profile.AddToEmployeePool = false;
                db.SaveChanges();
                appOrder.ESR_Id = esr.Id;


                esr2.TransferTypeID = 1;
                esr2.Profile_Id = mutualOrderDto.applicationMaster2.Profile_Id;
                esr2.CNIC = profile2.CNIC;
                esr2.DepartmentFrom = mutualOrderDto.applicationMaster2.Department_Id;
                esr2.DesignationFrom = mutualOrderDto.applicationMaster2.Designation_Id;
                esr2.HF_Id_From = mutualOrderDto.applicationMaster2.HealthFacility_Id;
                esr2.HfmisCodeFrom = mutualOrderDto.applicationMaster2.HfmisCode;
                esr2.BPSFrom = mutualOrderDto.applicationMaster2.CurrentScale;

                esr2.DesignationTo = mutualOrderDto.applicationMaster.Designation_Id;
                esr2.DepartmentTo = mutualOrderDto.applicationMaster.Department_Id;
                esr2.HF_Id_To = mutualOrderDto.applicationMaster.HealthFacility_Id;
                esr2.HfmisCodeTo = mutualOrderDto.applicationMaster.HfmisCode;
                esr2.BPSTo = mutualOrderDto.applicationMaster.CurrentScale;

                esr2.EmployeeFileNO = mutualOrderDto.applicationMaster2.FileNumber;
                esr2.SectionOfficer = mutualOrderDto.applicationMaster2.ForwardingOfficerName;
                esr2.MutualESR_Id = (int)esr.Id;
                esr2.COMMENTS = mutualOrderDto.applicationMaster2.Reason;
                esr2.Remarks = mutualOrderDto.applicationMaster2.Remarks;
                esr2.OrderHTML = mutualOrderDto.applicationMaster2.RawText;
                esr2.ResponsibleUser = userName;
                esr2.IsActive = true;
                Entity_Lifecycle eld2 = new Entity_Lifecycle();
                eld2.Created_Date = DateTime.UtcNow.AddHours(5);
                eld2.Created_By = userName;
                eld2.Users_Id = userId;
                eld2.IsActive = true;
                eld2.Entity_Id = 5;

                db.Entity_Lifecycle.Add(eld2);
                db.SaveChanges();

                esr2.EntityLifecycle_Id = eld2.Id;
                esr2.ResponsibleUser = eld2.Created_By;
                db.ESRs.Add(esr2);
                profile2.AddToEmployeePool = false;
                db.SaveChanges();
                appOrder.ESR_Id = esr.Id;


                var profiles = new List<HrProfile>() { profile, profile2 };
                var esrView = db.ESRViews.FirstOrDefault(x => x.Id == esr.Id);
                var esrView2 = db.ESRViews.FirstOrDefault(x => x.Id == esr2.Id);

                await ChangeSystemWithOrder((int)esr.Id, userName, userId);

                if (mutualOrderDto.applicationMaster.TrackingNumber != null)
                {
                    var application = db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == mutualOrderDto.applicationMaster.TrackingNumber);
                    if (application != null)
                    {
                        applicationLog.Application_Id = application.Id;
                        //Order Issued
                        applicationLog.SMS_SentToApplicant = true;
                        //Check if Application status is not Approved
                        if (application.Status_Id != 2)
                        {
                            applicationLog.ToStatus_Id = 2;
                        }
                        CreateApplicationLog(applicationLog, userName, userId);
                        appOrder.Application_Id = application.Id;
                        appOrder.TrackingNo = application.TrackingNumber;
                        appOrder.IsActive = true;
                        appOrder.CreatedBy = userName;
                        appOrder.User_Id = userId;
                        appOrder.DateTime = DateTime.UtcNow.AddHours(5);
                        db.AppOrders.Add(appOrder);
                        db.SaveChanges();
                    }
                }

                return new OrderResponse
                {
                    esr = esr,
                    esr2 = esr,
                    esrView = esrView,
                    esrView2 = esrView2,
                    applicationMaster = mutualOrderDto.applicationMaster,
                    applicationMaster2 = mutualOrderDto.applicationMaster2,
                    Profiles = profiles
                };
            }
        }
        public async Task<bool> ChangeSystemWithOrder(int esrId, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                ESR esr = db.ESRs.FirstOrDefault(x => x.Id == esrId);
                HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == esr.Profile_Id);
                var profileView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == profile.Id);
                if (profile == null)
                {
                    return false;
                }
                if (esr.TransferTypeID != null)
                {
                    //Mutual Transfer
                    #region Mututal
                    if (esr.TransferTypeID == 1)
                    {
                        var esr2 = db.ESRs.FirstOrDefault(x => x.MutualESR_Id == esrId);

                        HrProfile profile2 = db.HrProfiles.FirstOrDefault(x => x.Id == esr2.Profile_Id);
                        string hfFrom2 = db.HFLists.Where(x => x.Id == esr2.HF_Id_From).FirstOrDefault().HFMISCode;

                        profile.HfmisCode = hfFrom2;
                        profile.HealthFacility_Id = esr2.HF_Id_From;

                        profile.WorkingHFMISCode = hfFrom2;
                        profile.WorkingHealthFacility_Id = esr2.HF_Id_From;

                        profile.Status_Id = 30;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile1 = new Entity_Modified_Log();
                        emlProfile1.Modified_By = userId;
                        emlProfile1.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile1.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile1.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile1);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();

                        profile2.HfmisCode = esr.HfmisCodeFrom;
                        profile2.HealthFacility_Id = esr.HF_Id_From;

                        profile2.WorkingHFMISCode = esr.HfmisCodeFrom;
                        profile2.WorkingHealthFacility_Id = esr.HF_Id_From;

                        profile2.Status_Id = 30;

                        if (profile2.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile2.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile2 = new Entity_Modified_Log();
                        emlProfile2.Modified_By = userId;
                        emlProfile2.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile2.Entity_Lifecycle_Id = (long)profile2.EntityLifecycle_Id;
                        emlProfile2.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile2);
                        profile2.AddToEmployeePool = false;
                        db.Entry(profile2).State = EntityState.Modified;
                        db.SaveChanges();

                        //save esr2
                        Entity_Lifecycle eld2 = new Entity_Lifecycle();
                        eld2.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld2.Created_By = userName;
                        eld2.Users_Id = userId;
                        eld2.IsActive = true;
                        eld2.Entity_Id = 5;

                        db.Entity_Lifecycle.Add(eld2);
                        db.SaveChanges();


                        esr2.EntityLifecycle_Id = eld2.Id;
                        esr2.ResponsibleUser = eld2.Created_By;
                        db.ESRs.Add(esr2);
                        db.SaveChanges();

                    }

                    #endregion
                    //At Disposal
                    if (esr.TransferTypeID == 2)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, userName, userId);

                        //DGHS
                        if (esr.DisposalofID == 1)
                        {
                            profile.HfmisCode = "0350020010030020001";
                            profile.HealthFacility_Id = 14426;

                            profile.WorkingHFMISCode = "0350020010030020001";
                            profile.WorkingHealthFacility_Id = 14426;
                            profile.Status_Id = 15;
                        }
                        //CDC
                        if (esr.DisposalofID == 2)
                        {
                            profile.HfmisCode = "0350020010030040009";
                            profile.HealthFacility_Id = 14681;

                            profile.WorkingHFMISCode = "0350020010030040009";
                            profile.WorkingHealthFacility_Id = 14681;
                            profile.Status_Id = 15;
                        }
                        //SHD or PSHD
                        if (esr.DisposalofID == 4 || esr.DisposalofID == 3)
                        {
                            profile.HfmisCode = "0350020010030010002";
                            profile.HealthFacility_Id = 11606;

                            profile.WorkingHFMISCode = "0350020010030010002";
                            profile.WorkingHealthFacility_Id = 11606;
                            profile.Status_Id = 15;
                        }
                        //edo
                        if (esr.DisposalofID == 6)
                        {
                            ProfileDetailsView profileDetailView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esr.Profile_Id);

                            HFList hf = db.HFLists.Where(x => x.IsActive == true && x.DivisionName.Equals(profileDetailView.District) && x.HFTypeCode.Equals("049")).FirstOrDefault();

                            if (hf != null)
                            {
                                profile.HfmisCode = hf.HFMISCode;
                                profile.HealthFacility_Id = hf.Id;

                                profile.WorkingHFMISCode = hf.HFMISCode;
                                profile.WorkingHealthFacility_Id = hf.Id;
                                profile.Status_Id = 15;
                            }
                        }
                        //ceo
                        if (esr.DisposalofID == 7)
                        {
                            ProfileDetailsView profileDetailView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esr.Profile_Id);

                            HFList hf = db.HFLists.Where(x => x.IsActive == true && x.DistrictCode.Equals(esr.Remarks) && x.HFTypeCode.Equals("049")).FirstOrDefault();

                            if (hf != null)
                            {
                                profile.HfmisCode = hf.HFMISCode;
                                profile.HealthFacility_Id = hf.Id;

                                profile.WorkingHFMISCode = hf.HFMISCode;
                                profile.WorkingHealthFacility_Id = hf.Id;
                                profile.Status_Id = 15;
                            }
                        }
                        if (esr.HF_Id_To > 0)
                        {
                            ProfileDetailsView profileDetailView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esr.Profile_Id);

                            HFList hf = db.HFLists.FirstOrDefault(x => x.Id == esr.HF_Id_To);

                            if (hf != null)
                            {
                                profile.HfmisCode = hf.HFMISCode;
                                profile.HealthFacility_Id = hf.Id;

                                profile.WorkingHFMISCode = hf.HFMISCode;
                                profile.WorkingHealthFacility_Id = hf.Id;
                                profile.Status_Id = 35;
                            }
                        }
                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Suspend
                    if (esr.TransferTypeID == 3)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, userName, userId);
                        profile.HfmisCode = "0350020010030010002";
                        profile.HealthFacility_Id = 11606;

                        profile.WorkingHFMISCode = "0350020010030010002";
                        profile.WorkingHealthFacility_Id = 11606;
                        profile.Status_Id = 27;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Resignation
                    if (esr.TransferTypeID == 12)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, userName, userId);
                        profile.HfmisCode = "0350020010030010002";
                        profile.HealthFacility_Id = 11606;

                        profile.WorkingHFMISCode = "0350020010030010002";
                        profile.WorkingHealthFacility_Id = 11606;
                        profile.Status_Id = 24;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // General Transfer
                    if (esr.TransferTypeID == 4 || esr.TransferTypeID == 6)
                    {
                        if (esr.TransferTypeID == 4)
                        {
                            _transferPostingService.UpdateVacancy(db, false, esr.HfmisCodeFrom, esr.DesignationFrom, profile.EmpMode_Id, userName, userId);
                        }

                        _transferPostingService.UpdateVacancy(db, true, esr.HfmisCodeTo, esr.DesignationTo, profile.EmpMode_Id, userName, userId);

                        profile.HfmisCode = esr.HfmisCodeTo;
                        profile.HealthFacility_Id = esr.HF_Id_To;

                        profile.WorkingHFMISCode = esr.HfmisCodeTo;
                        profile.WorkingHealthFacility_Id = esr.HF_Id_To;

                        profile.Designation_Id = esr.DesignationTo;
                        profile.WDesignation_Id = esr.DesignationTo;

                        profile.Status_Id = 30;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();

                        var _profileService = new ProfileService();
                        HrServiceHistory hrServiceHistory = new HrServiceHistory();
                        hrServiceHistory.Profile_Id = profile.Id;
                        hrServiceHistory.OrderNumber = esr.EmployeeFileNO;
                        hrServiceHistory.OrderDate = DateTime.UtcNow.AddHours(5);
                        hrServiceHistory.HF_Id = esr.HF_Id_To;
                        hrServiceHistory.EmpMode_Id = 13;
                        hrServiceHistory.Designation_Id = esr.DesignationTo;
                        hrServiceHistory.ESR_Id = esr.Id;
                        hrServiceHistory.Scale = (int)esr.BPSTo;
                        hrServiceHistory.PendingJoining = true;
                        var res = _profileService.SaveServiceHistory(hrServiceHistory, userName, userId);
                    }
                    // Adhoc Appointment
                    if (esr.TransferTypeID == 8)
                    {
                        _transferPostingService.UpdateVacancy(db, true, esr.HfmisCodeTo, esr.DesignationTo, profile.EmpMode_Id, userName, userId);

                        profile.HfmisCode = esr.HfmisCodeTo;
                        profile.HealthFacility_Id = esr.HF_Id_To;

                        profile.WorkingHFMISCode = esr.HfmisCodeTo;
                        profile.WorkingHealthFacility_Id = esr.HF_Id_To;

                        profile.Designation_Id = esr.DesignationTo;
                        profile.WDesignation_Id = esr.DesignationTo;

                        profile.Status_Id = 30;
                        profile.EmpMode_Id = 3;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Termination
                    if (esr.TransferTypeID == 10 || esr.TransferTypeID == 11)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, userName, userId);
                        if (esr.TransferTypeID == 10)
                        {
                            profile.Status_Id = 28;
                        }
                        if (esr.TransferTypeID == 11)
                        {
                            profile.HfmisCode = "0350020010030010002";
                            profile.HealthFacility_Id = 11606;

                            profile.WorkingHFMISCode = "0350020010030010002";
                            profile.WorkingHealthFacility_Id = 11606;
                        }

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = userId;
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + userName;
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var EsrProfile = db.HrProfiles.FirstOrDefault(x => x.Id == esr.Profile_Id);
                if (EsrProfile != null)
                {
                    //EsrProfile.Status_Id = 30; // Setting Profile status to Pending Transfer
                    //db.SaveChanges();

                    var userService = new UserService();
                    var hfmisCode = userService.GetUser(userId).hfmiscode;
                    //await SendSMS(profileView, hfmisCode, (int)esr.Id, userId);
                    //if (hfmisCode.Length == 6)
                    //{
                    //    SendSMS(profileView, hfmisCode);
                    //}
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> SendSMS(ProfileDetailsView EsrProfile, string hfmisCode, int esrId, string userId)
        {
            if (hfmisCode.Length == 1)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //List<SMS> smsy = new List<SMS>();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody

                    //};
                    //smsy.Add(sms);
                    //Common.Common.SMS_Send(smsy);
                    SMS sms = new SMS()
                    {
                        UserId = userId,
                        FKId = esrId,
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                    //t.Start();
                    //await Common.Common.SendSMSTelenor(sms);
                    return true;
                }
            }
            else if (hfmisCode.Length == 6)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "" && EsrProfile.District != null)
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at District Health Authority, " + EsrProfile.District + ".\nPlease visit http://pshealth.punjab.gov.pk/ for further details.";
                    //List<SMS> smsy = new List<SMS>();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody
                    //};
                    //smsy.Add(sms);
                    //Common.Common.SMS_Send(smsy);
                    SMS sms = new SMS()
                    {
                        UserId = userId,
                        FKId = esrId,
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                    //t.Start();
                    //await Common.Common.SendSMSTelenor(sms);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CreateOrderTrackingLog(ProfileDetailsView EsrProfile, string hfmisCode, int esrId, string userId)
        {
            if (hfmisCode.Length == 1)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //List<SMS> smsy = new List<SMS>();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody

                    //};
                    //smsy.Add(sms);
                    //Common.Common.SMS_Send(smsy);
                    SMS sms = new SMS()
                    {
                        UserId = userId,
                        FKId = esrId,
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                    //t.Start();
                    //await Common.Common.SendSMSTelenor(sms);
                    return true;
                }
            }
            else if (hfmisCode.Length == 6)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "" && EsrProfile.District != null)
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at District Health Authority, " + EsrProfile.District + ".\nPlease visit http://pshealth.punjab.gov.pk/ for further details.";
                    //List<SMS> smsy = new List<SMS>();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody
                    //};
                    //smsy.Add(sms);
                    //Common.Common.SMS_Send(smsy);
                    SMS sms = new SMS()
                    {
                        UserId = userId,
                        FKId = esrId,
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                    //t.Start();
                    //await Common.Common.SendSMSTelenor(sms);
                    return true;
                }
            }
            return false;
        }
    }
    public class ApplicationFilter : Paginator
    {
        public int Source_Id { get; set; }
        public bool Pending { get; set; }
        public string Query { get; set; }
        public string Program { get; set; }
        public int Type_Id { get; set; }
        public int Status_Id { get; set; }
        public int OfficeId { get; set; }
        public int Officer_Id { get; set; }
        public int ForwardingOfficer_Id { get; set; }
        public string Field { get; set; }
        public int? FieldValue { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
    public class OrderResponse
    {
        public ESR esr;
        public ESRView esrView;

        public ESR esr2;
        public ESRView esrView2;
        public LeaveOrder leaveOrder;
        public LeaveOrderView leaveOrderView;
        public ApplicationMaster applicationMaster;
        public ApplicationMaster applicationMaster2;
        public List<HrProfile> Profiles;
        public string orderType;
    }
    public class OrderDesignationResponse
    {
        public VpMasterProfileView vpMaster;
        public List<VpMasterProfileView> vpMasters;
        public List<VpDProfileView> vpDetails;
    }
    public class CombineOrderDto
    {
        public ApplicationMaster applicationMaster;
        public List<ApplicationMaster> applications;
    }
    public class OfficerFilesCount
    {
        public List<ApplicationView> List { get; set; }
        public int Pending { get; set; }
        public int Sent { get; set; }
        public int Recieved { get; set; }
    }
    public class MutualOrderDto
    {
        public ApplicationMaster applicationMaster;
        public ApplicationMaster applicationMaster2;
    }
}