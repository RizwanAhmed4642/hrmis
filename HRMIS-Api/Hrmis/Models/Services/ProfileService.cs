using DPUruNet;
using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace Hrmis.Models.Services
{
    public class ProfileService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ProfileService));
        public ProfileDetailsView GetProfile(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
            }
        }

        public HrSeniorityApplicationView GetSeniorityApplicantProfile(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var data = _db.HrSeniorityApplicationViews.FirstOrDefault(x => x.CNIC.Equals(cnic));

                if (data == null)
                {
                    return null;
                    //data = new HrSeniorityApplicationView();
                    //var hrprofile = _db.HrProfileDetailsViews.FirstOrDefault(c => c.CNIC.Equals(cnic));

                    //if (hrprofile != null)
                    //{
                    //    data.CNIC = hrprofile.CNIC;
                    //    data.EmployeeName = hrprofile.EmployeeName;
                    //    data.FatherName = hrprofile.FatherName;
                    //    data.Gender = hrprofile.Gender;
                    //    data.DateOfBirth = hrprofile.DateOfBirth;
                    //    data.Domicile_Id = hrprofile.Domicile_Id;
                    //    data.MaritalStatus = hrprofile.MaritalStatus;
                    //    data.MobileNo = hrprofile.MobileNo;
                    //    data.EMaiL = hrprofile.EMaiL;
                    //    data.PermanentAddress = hrprofile.PermanentAddress;
                    //    data.ModeId = hrprofile.EmpMode_Id;
                    //    data.ContractStartDate = hrprofile.ContractStartDate;
                    //    data.ContractEndDate = hrprofile.ContractEndDate;
                    //    data.LastPromotionDate = hrprofile.LastPromotionDate;
                    //    data.DateOfRegularization = hrprofile.DateOfRegularization;
                    //    data.LastPromotionDate = hrprofile.LastPromotionDate;
                    //}
                    //else
                    //    return null; 

                }
                return data;
            }
        }


        public HrHealthWorkerView GetHealthWorker(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;



                return _db.HrHealthWorkerViews.FirstOrDefault(x => x.CNIC.Equals(cnic) && x.IsActive == true);
            }
        }

        public string GetSurgeries(string code, string filter)
        {
            string response = null;
            try
            {
                var client = new WebClient();
                response = client.DownloadString("http://125.209.111.70:88/dhis/api/get_surgeries.php?id=" + code + "&type=" + filter);
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public List<HrInquiryView> GetProfileInquiries(int profileId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.HrInquiryViews.Where(x => x.Profile_Id == profileId).ToList(); ;
            }
        }
        public List<ProfileAttachmentsView> GetVaccinationCertificate(int profileId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.ProfileAttachmentsViews.Where(x => x.Profile_Id == profileId && x.IsActive == true).ToList();
            }
        }
        public List<HrInquiryPenalty> GetInquiryPenalties(List<HrInquiryView> inquiries)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var inquiryIds = inquiries.Select(x => x.Id).ToList();
                return _db.HrInquiryPenalties.Where(x => inquiryIds.Contains((int)x.EmpInquiry_Id)).ToList(); ;
            }
        }
        public ProfileDetailsView GetAnyProfile(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
            }
        }
        public MobileResponse FPrintRegister(List<FPPrint> fprints, int profile_Id, string userId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var exist = db.HrFps.FirstOrDefault(x => x.Profile_Id == profile_Id);
                    log.Info($"Line No. 55.");
                    if (exist != null)
                    {
                        return new MobileResponse() { isException = false, message = "Already Registered!" };
                    }

                    HrFp fp = new HrFp();

                    for (int i = 0; i < fprints.Count; i++)
                    {
                        if (i < 5)
                        {
                            if (i == 0) fp.FP1 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprints[i].metaData));
                            if (i == 1) fp.FP2 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprints[i].metaData));
                            if (i == 2) fp.FP3 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprints[i].metaData));
                            if (i == 3) fp.FP4 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprints[i].metaData));
                            if (i == 4) fp.FP5 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprints[i].metaData));
                        }
                    }
                    log.Info($"Line No. 76.");
                    fp.Profile_Id = profile_Id;
                    fp.User_Id = userId;
                    fp.DateTime = DateTime.UtcNow.AddHours(5);

                    db.HrFps.Add(fp);
                    db.Configuration.ProxyCreationEnabled = false;
                    db.SaveChanges();
                    log.Info($"Line No. 84.");

                    return new MobileResponse() { isException = false, message = "Successfull!" };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }



        public bool AddProfileInquiry(InquiryDtoModel inquiryModel, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName;
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 939;

                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();

                        inquiryModel.hrInquiry.EntityLifecycle_Id = elc.Id;

                        _db.HrInquiries.Add(inquiryModel.hrInquiry);
                        _db.SaveChanges();

                        inquiryModel.hrInquiryPenalties.ForEach(x => x.EmpInquiry_Id = inquiryModel.hrInquiry.Id);

                        _db.HrInquiryPenalties.AddRange(inquiryModel.hrInquiryPenalties);
                        _db.SaveChanges();

                        return true;

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


        public HrProfile AddProfile(HrProfile hrProfile, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbHrProfile = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(hrProfile.CNIC));
                    try
                    {
                        if (dbHrProfile == null)
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

                            return savableHrProfile;
                        }
                        else
                        {
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

                            return editableHrProfile;
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ShiftProfileToPSH(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbHrProfile = _db.HrProfiles.FirstOrDefault(x => x.Id == profileId);
                    try
                    {
                        if (dbHrProfile != null)
                        {
                            dbHrProfile.HealthFacility_Id = 11606;
                            dbHrProfile.HfmisCode = "0350020010030010002";
                            if (dbHrProfile.EntityLifecycle_Id == null)
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Created_By = userName + " (added after migration)";
                                elc.Users_Id = userId;
                                elc.IsActive = true;
                                elc.Entity_Id = 9;
                                _db.Entity_Lifecycle.Add(elc);
                                _db.SaveChanges();
                                dbHrProfile.EntityLifecycle_Id = elc.Id;
                            }
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)dbHrProfile.EntityLifecycle_Id;
                            eml.Description = "Profile Shifted to PSH Department By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.Entry(dbHrProfile).State = EntityState.Modified;
                            _db.SaveChanges();

                            return true;
                        }
                        return false;
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

        public HrHealthWorker SaveHealthWorker(HrHealthWorker hrHealthWorker, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    try
                    {
                        if (hrHealthWorker.Id == 0)
                        {
                            hrHealthWorker.DateTime = DateTime.UtcNow.AddHours(5);
                            hrHealthWorker.CreatedBy = userName;
                            hrHealthWorker.UserId = userId;
                            hrHealthWorker.IsActive = true;
                            _db.HrHealthWorkers.Add(hrHealthWorker);
                            _db.SaveChanges();
                            return hrHealthWorker;
                        }
                        else
                        {
                            _db.Entry(hrHealthWorker).State = EntityState.Modified;
                            _db.SaveChanges();
                            return hrHealthWorker;
                        }
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

        public HrFocalPerson SaveFocalPerson(HrFocalPerson hrFocalPerson, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    try
                    {
                        var fp = _db.HrFocalPersons.FirstOrDefault(x => x.CNIC.Equals(hrFocalPerson.CNIC));
                        if (fp == null)
                        {
                            hrFocalPerson.Datetime = DateTime.UtcNow.AddHours(5);
                            hrFocalPerson.Username = userName;
                            hrFocalPerson.UserId = userId;
                            hrFocalPerson.IsActive = true;
                            _db.HrFocalPersons.Add(hrFocalPerson);
                            _db.SaveChanges();
                            return hrFocalPerson;
                        }
                        else
                        {

                            fp.Profile_Id = hrFocalPerson.Profile_Id;
                            fp.Name = hrFocalPerson.Name;
                            fp.CNIC = hrFocalPerson.CNIC;
                            fp.MobileNumber = hrFocalPerson.MobileNumber;
                            fp.HF_Id = hrFocalPerson.HF_Id;
                            fp.HFMISCode = hrFocalPerson.HFMISCode;
                            fp.IsActive = true;
                            _db.Entry(fp).State = EntityState.Modified;
                            _db.SaveChanges();
                            return hrFocalPerson;
                        }
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
        public HrProfile SaveShortProfile(HrProfile hrProfile, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbHrProfile = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(hrProfile.CNIC));
                    try
                    {
                        if (dbHrProfile == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();

                            elc.IsActive = true;
                            elc.Created_By = userName;
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Entity_Id = 9;
                            elc.Users_Id = userId;

                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();

                            hrProfile.EntityLifecycle_Id = elc.Id;

                            _db.HrProfiles.Add(hrProfile);
                            _db.SaveChanges();

                            return hrProfile;
                        }
                        else
                        {
                            if (hrProfile.EntityLifecycle_Id == null)
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Created_By = userName + " (added after migration)";
                                elc.Users_Id = userId;
                                elc.IsActive = true;
                                elc.Entity_Id = 9;
                                _db.Entity_Lifecycle.Add(elc);
                                _db.SaveChanges();
                                hrProfile.EntityLifecycle_Id = elc.Id;
                            }
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)hrProfile.EntityLifecycle_Id;
                            eml.Description = "Profile Updated By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();

                            _db.Entry(hrProfile).State = EntityState.Modified;
                            _db.SaveChanges();

                            return hrProfile;
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public bool LockServiceHistory(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == profileId);
                    try
                    {
                        if (profile != null)
                        {
                            if (!string.IsNullOrEmpty(profile.Tenure) && profile.Tenure.Equals("Completed"))
                            {
                                profile.Tenure = "Incomplete";
                                if (profile.EntityLifecycle_Id == null)
                                {
                                    Entity_Lifecycle elc = new Entity_Lifecycle();
                                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                    elc.Created_By = userName + " (added after migration)";
                                    elc.Users_Id = userId;
                                    elc.IsActive = true;
                                    elc.Entity_Id = 9;
                                    _db.Entity_Lifecycle.Add(elc);
                                    _db.SaveChanges();
                                    profile.EntityLifecycle_Id = elc.Id;
                                }
                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                                eml.Description = "Profile Updated: Service History Unlocked By " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();

                                _db.Entry(profile).State = EntityState.Modified;
                                _db.SaveChanges();
                                return false;
                            }
                            else
                            {
                                profile.Tenure = "Completed";
                                if (profile.EntityLifecycle_Id == null)
                                {
                                    Entity_Lifecycle elc = new Entity_Lifecycle();
                                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                    elc.Created_By = userName + " (added after migration)";
                                    elc.Users_Id = userId;
                                    elc.IsActive = true;
                                    elc.Entity_Id = 9;
                                    _db.Entity_Lifecycle.Add(elc);
                                    _db.SaveChanges();
                                    profile.EntityLifecycle_Id = elc.Id;
                                }
                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                                eml.Description = "Profile Updated: Service History Locked By " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();

                                _db.Entry(profile).State = EntityState.Modified;
                                _db.SaveChanges();
                                return true;
                            }
                        }
                        return false;
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
        public List<HrServiceHistoryView> GetServiceHistory(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var serviceHistory = _db.HrServiceHistoryViews.Where(x => x.Profile_Id == profileId && x.IsActive == true).OrderByDescending(k => k.From_Date).ToList();
                        return serviceHistory;
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

        public List<HrQualificationView> GetHrQualification(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var hrQualification = _db.HrQualificationViews.Where(x => x.Profile_Id == profileId && x.IsActive == true).OrderByDescending(k => k.DegreeFrom).ToList();
                        return hrQualification;
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
        // Service Function to get files master..
        public TableResponse<FilesMaster> GetFilesMaster(FilesACRsFilter filters, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var query = _db.FilesMasters.AsQueryable();

                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        query = query.Where(x => x.Name.ToLower().Contains(filters.Query.ToLower())
                        || x.Barcode.ToLower().Contains(filters.Query.ToLower())
                        || x.FileNumber.ToLower().Contains(filters.Query.ToLower())).AsQueryable();
                    }

                    query = query.Where(x => x.IsActive == true).AsQueryable();
                    var count = query.Count();
                    var list = query.OrderBy(x => Guid.NewGuid()).Skip(filters.Skip).Take(filters.PageSize).ToList();

                    return new TableResponse<FilesMaster>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public List<FilesMaster> GetFilesList()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var filesMaster = _db.FilesMasters.ToList();
                        return filesMaster;
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

        public bool RemoveServiceHistory(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var serviceHistory = _db.HrServiceHistories.FirstOrDefault(x => x.Id == Id);
                    try
                    {
                        if (serviceHistory != null)
                        {
                            var entity_Lifecycle = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == serviceHistory.EntityLifecycle_Id);
                            if (entity_Lifecycle != null)
                            {
                                entity_Lifecycle.IsActive = false;
                                _db.Entry(entity_Lifecycle).State = EntityState.Modified;
                                _db.SaveChanges();

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)serviceHistory.EntityLifecycle_Id;
                                eml.Description = "Service History Removed by " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();
                            }
                        }
                        return false;
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
        public HrServiceHistory SaveServiceHistory(HrServiceHistory hrServiceHistory, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hrServiceHistory.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.IsActive = true;
                            elc.Created_By = userName;
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Users_Id = userId;
                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();

                            hrServiceHistory.EntityLifecycle_Id = elc.Id;
                            if (hrServiceHistory.To_Date != null)
                            {
                                hrServiceHistory.TotalDays = Convert.ToDateTime(hrServiceHistory.To_Date).Subtract(Convert.ToDateTime(hrServiceHistory.From_Date)).Days;
                                hrServiceHistory.Continued = false;
                            }
                            _db.HrServiceHistories.Add(hrServiceHistory);
                            _db.SaveChanges();
                            var hfService = new HealthFacilityService();
                            var hrProfile = _db.HrProfiles.FirstOrDefault(x => x.Id == hrServiceHistory.Profile_Id);
                            //if (hrProfile != null)
                            //{
                            //    Thread thread = new Thread(() => new HealthFacilityService().HealthFacility_Distance(hrProfile.HealthFacility_Id, hrServiceHistory.HF_Id));
                            //    thread.Start();
                            //}

                            return hrServiceHistory;
                        }
                        else if (hrServiceHistory.Id > 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var serviceHistoryDb = _db.HrServiceHistories.FirstOrDefault(x => x.Id == hrServiceHistory.Id);
                            if (serviceHistoryDb != null)
                            {
                                if (hrServiceHistory.To_Date != null)
                                {
                                    serviceHistoryDb.TotalDays = Convert.ToDateTime(hrServiceHistory.To_Date).Subtract(Convert.ToDateTime(hrServiceHistory.From_Date)).Days;
                                    serviceHistoryDb.Continued = false;
                                    serviceHistoryDb.To_Date = hrServiceHistory.To_Date;
                                }
                                _db.Entry(serviceHistoryDb).State = EntityState.Modified;
                                _db.SaveChanges();

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)serviceHistoryDb.EntityLifecycle_Id;
                                eml.Description = "Service History Modified by " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();
                                return hrServiceHistory;
                            }
                            return null;
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public HrQualification SaveHrQualification(HrQualification hrQualification, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hrQualification.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;

                            hrQualification.IsActive = true;
                            hrQualification.CreatedBy = userName;
                            hrQualification.CreatedDate = DateTime.UtcNow.AddHours(5);
                            hrQualification.UserId = userId;
                            _db.HrQualifications.Add(hrQualification);
                            _db.SaveChanges();
                            return hrQualification;
                        }
                        return new HrQualification();
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool RemoveHrQualification(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hrQualification = _db.HrQualifications.FirstOrDefault(x => x.Id == Id);
                    try
                    {
                        if (hrQualification != null)
                        {
                            hrQualification.IsActive = false;
                            _db.SaveChanges();
                        }
                        return false;
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
        public ProfileQualified GetAttachedPersons(MapFilters mapFilters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        var profileQuery = _db.ProfileDetailsViews.Where(x => x.HfmisCode.StartsWith(mapFilters.HfmisCode)).AsQueryable();
                        var AttachedPerson = _db.HrAttachedPersonViews.Where(x => (x.Designation_Id == 802 || x.Designation_Id == 1320 || x.Designation_Id == 2404 || x.Designation_Id == 2490 || x.Designation_Id == 2491) && x.IsDiploma != null && x.OnTraining != null && x.HfmisCode.StartsWith(mapFilters.HfmisCode) && x.ProfileId != null && x.AttachedPersonId != null && x.IsActive == true).ToList();
                        var AttachedPersonMOWMO = _db.HrAttachedPersonViews.Where(x => (x.Designation_Id == 802 || x.Designation_Id == 1320 || x.Designation_Id == 2404 || x.Designation_Id == 2490 || x.Designation_Id == 2491) && x.IsDiploma == null && x.OnTraining == null && x.HfmisCode.StartsWith(mapFilters.HfmisCode) && x.ProfileId != null && x.AttachedPersonId != null && x.IsActive == true).ToList();

                        if (mapFilters.HFTypeCodes.Count > 0)
                        {
                            profileQuery = profileQuery.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                        }
                        var ConsultantAnaesthetists = profileQuery.Where(x => x.Designation_Id == 362 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).ToList();
                        var Technologists = profileQuery.Where(x => x.Designation_Id == 2171 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).ToList();
                        var CountOnlyMO = profileQuery.Where(x => x.WDesignation_Id == 802 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining"))).Count();
                        var CountOnlyWMO = profileQuery.Where(x => x.WDesignation_Id == 1320 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining"))).Count();
                        var CountMO = profileQuery.Where(x => x.WDesignation_Id == 2487 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).Count();
                        var CountWMO = profileQuery.Where(x => x.WDesignation_Id == 2489 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).Count();
                        var CountSMO = profileQuery.Where(x => x.WDesignation_Id == 2490 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).Count();
                        var CountSWMO = profileQuery.Where(x => x.WDesignation_Id == 2491 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).Count();
                        var CountTrainingCompletedMO = profileQuery.Where(x => x.Gender.ToLower() == "male" && x.AdditionalQualification.ToLower().Contains("da(") && x.AdditionalQualification.ToLower().Contains("training completed") && x.StatusName.Equals("Active")).Count();
                        var CountDiplomaMO = profileQuery.Where(x => x.Gender.ToLower() == "male" && x.AdditionalQualification.ToLower().Contains("diploma") && x.AdditionalQualification.ToLower().Contains("thesi") && !x.AdditionalQualification.ToLower().Contains("technician") && !x.AdditionalQualification.ToLower().Contains("assistant") && x.StatusName.Equals("Active")).Count();
                        var CountDiplomaWMO = profileQuery.Where(x => x.Gender.ToLower() == "female" && x.AdditionalQualification.ToLower().Contains("diploma") && x.AdditionalQualification.ToLower().Contains("thesi") && !x.AdditionalQualification.ToLower().Contains("technician") && !x.AdditionalQualification.ToLower().Contains("assistant") && x.StatusName.Equals("Active")).Count();
                        var CountTrainingCompletedWMO = profileQuery.Where(x => x.Gender.ToLower() == "female" && x.AdditionalQualification.ToLower().Contains("da(") && x.AdditionalQualification.ToLower().Contains("training completed") && x.StatusName.Equals("Active")).Count();
                        return new ProfileQualified
                        {
                            attachedPersons = AttachedPerson,
                            attachedPersonsMOWMO = AttachedPersonMOWMO,
                            CountMO = CountMO,
                            CountOnlyMOWMO = AttachedPersonMOWMO.Count(),
                            CountWMO = CountWMO,
                            CountSMO = CountSMO,
                            CountSWMO = CountSWMO,
                            CountDiplomaMO = CountDiplomaMO,
                            Technologists = Technologists,
                            ConsultantAnaesthetists = ConsultantAnaesthetists,
                            CountTrainingMO = CountTrainingCompletedMO,
                            CountDiplomaWMO = CountDiplomaWMO,
                            CountTrainingWMO = CountTrainingCompletedWMO
                        };
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

        public ProfileQualified GetEmployeePersons(MapFilters mapFilters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;

                        List<int?> vpProfileStatus = _db.VpProfileStatus.Where(x => x.IsActive == true).Select(k => k.ProfileStatus_Id).ToList();
                        var profileQuery = _db.ProfileDetailsViews.Where(x => vpProfileStatus.Contains((int)x.Status_Id) && x.HfmisCode.StartsWith(mapFilters.HfmisCode) && mapFilters.DesignationIds.Contains((int)x.Designation_Id)).AsQueryable();

                        if (mapFilters.HFTypeCodes.Count > 0)
                        {
                            profileQuery = profileQuery.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                        }
                        return new ProfileQualified
                        {
                            Persons = profileQuery.ToList()
                        };
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


        public Anaesthesia GetAnaesthesiaTree(MapFilters mapFilters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        var anaesthesia = new Anaesthesia();
                        anaesthesia.AttachedMOWMO = 0;
                        anaesthesia.AttachedPersons = 0;
                        List<AnaesthesiaTree> anaesthesiaTree = new List<AnaesthesiaTree>();
                        var profiles = _db.ProfileDetailsViews.Where(x => x.HfmisCode.StartsWith(mapFilters.HfmisCode) && x.Designation_Id == 362 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).ToList();
                        var technologists = _db.ProfileDetailsViews.Where(x => x.HfmisCode.StartsWith(mapFilters.HfmisCode) && x.Designation_Id == 2171 && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).ToList();

                        foreach (var profile in profiles)
                        {
                            var tree = new AnaesthesiaTree();
                            tree.Anaesthetist = profile;
                            var attachedPersons = _db.HrAttachedPersonViews.Where(x => (x.Designation_Id == 802 || x.Designation_Id == 1320 || x.Designation_Id == 2404 || x.Designation_Id == 1085 || x.Designation_Id == 1157) && x.IsDiploma != null && x.OnTraining != null && x.ProfileId == profile.Id && x.AttachedPersonId != null && x.IsActive == true).ToList();
                            anaesthesia.AttachedPersons += attachedPersons.Count();
                            List<MOAnaesthesiaTree> MOTree = new List<MOAnaesthesiaTree>();
                            foreach (var attachedPerson in attachedPersons)
                            {
                                var mowmoTree = _db.HrAttachedPersonViews
                                    .Where(x => (x.Designation_Id == 802
                                    || x.Designation_Id == 1320
                                    || x.Designation_Id == 2404
                                    || x.Designation_Id == 1085
                                    || x.Designation_Id == 1157)
                                    && x.IsDiploma == null
                                    && x.OnTraining == null
                                    && x.ProfileId == attachedPerson.AttachedPersonId
                                    && x.AttachedPersonId != null
                                    && x.IsActive == true).ToList();

                                var moWMOA = new MOAnaesthesiaTree();
                                moWMOA.MOWMOAnaesthesia = attachedPerson;
                                moWMOA.MOWMO = mowmoTree;
                                MOTree.Add(moWMOA);
                                anaesthesia.AttachedMOWMO += mowmoTree.Count();
                            }
                            tree.MOTree = MOTree;
                            anaesthesiaTree.Add(tree);
                        }
                        anaesthesia.anaesthesiaTree = anaesthesiaTree;
                        anaesthesia.Technologists = technologists;
                        return anaesthesia;
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
        public ProfileQualified GetAnesthesiaPersons(MapFilters mapFilters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        var profileQuery = _db.ProfileDetailsViews.Where(x => x.HfmisCode.StartsWith(mapFilters.HfmisCode) && (x.StatusName.Equals("Active") || x.StatusName.Equals("Pending Joining") || x.StatusName.Equals("On Deputation"))).AsQueryable();
                        if (mapFilters.HFTypeCodes.Count > 0)
                        {
                            profileQuery = profileQuery.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                        }
                        if (mapFilters.showProfileViewId == 1)
                        {
                            var MO = profileQuery.Where(x => x.WDesignation_Id == 2487).ToList();
                            return new ProfileQualified { MO = MO };
                        }
                        else if (mapFilters.showProfileViewId == 2)
                        {
                            var WMO = profileQuery.Where(x => x.WDesignation_Id == 2489).ToList();
                            return new ProfileQualified { WMO = WMO };
                        }
                        else if (mapFilters.showProfileViewId == 11)
                        {
                            var TrainingCompletedMO = profileQuery.Where(x => x.Gender.ToLower() == "male" && x.AdditionalQualification.ToLower().Contains("da(") && x.AdditionalQualification.ToLower().Contains("training completed")).ToList();
                            return new ProfileQualified { TrainingMO = TrainingCompletedMO };
                        }
                        else if (mapFilters.showProfileViewId == 12)
                        {
                            var DiplomaMO = profileQuery.Where(x => x.Gender.ToLower() == "male" && x.AdditionalQualification.ToLower().Contains("diploma") && x.AdditionalQualification.ToLower().Contains("thesi") && !x.AdditionalQualification.ToLower().Contains("assistant") && !x.AdditionalQualification.ToLower().Contains("technician")).ToList();
                            return new ProfileQualified { DiplomaMO = DiplomaMO };
                        }
                        else if (mapFilters.showProfileViewId == 21)
                        {
                            var TrainingCompletedWMO = profileQuery.Where(x => x.Gender.ToLower() == "female" && x.AdditionalQualification.ToLower().Contains("da(") && x.AdditionalQualification.ToLower().Contains("training completed")).ToList();
                            return new ProfileQualified { TrainingWMO = TrainingCompletedWMO };
                        }
                        else if (mapFilters.showProfileViewId == 22)
                        {
                            var DiplomaWMO = profileQuery.Where(x => x.Gender.ToLower() == "female" && x.AdditionalQualification.ToLower().Contains("diploma") && x.AdditionalQualification.ToLower().Contains("thesi") && !x.AdditionalQualification.ToLower().Contains("assistant") && !x.AdditionalQualification.ToLower().Contains("technician")).ToList();
                            return new ProfileQualified { DiplomaWMO = DiplomaWMO };
                        }
                        return new ProfileQualified();

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
        public List<HrAttachedPersonView> GetAttachedPerson(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var AttachedPerson = _db.HrAttachedPersonViews.Where(x => x.ProfileId == profileId && x.AttachedPersonId != null && x.IsActive == true).ToList();
                        return AttachedPerson;
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
        public bool RemoveAttachedPerson(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    try
                    {
                        var AttachedPerson = _db.HrAttachedPersons.FirstOrDefault(x => x.Id == Id);
                        AttachedPerson.IsActive = false;
                        _db.Entry(AttachedPerson).State = EntityState.Modified;
                        _db.SaveChanges();
                        return true;
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

        public TableResponse<uspSeniority_Result> GetSeniority(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    try
                    {
                        var query = _db.uspSeniority().ToList();
                        //var count = query.Count();
                        // var res = query.OrderBy(x => x.Id).ToList();
                        var count = query.Count();
                        return new TableResponse<uspSeniority_Result>() { List = query, Count = count };
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
        public bool RemoveFocalPerson(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    try
                    {
                        var focalPerson = _db.HrFocalPersons.FirstOrDefault(x => x.Id == Id);
                        if (focalPerson != null)
                        {
                            focalPerson.IsActive = false;
                            _db.Entry(focalPerson).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        return true;
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
        public IEnumerable<ProfileDetailsView> GetDuplication(string cnic)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                return _db.ProfileDetailsViews.Where(x => x.CNIC.Equals(cnic)).ToList();
            }
        }


        public bool DeleteDuplication(int Id, string cnic, string username, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profilelist = _db.HrProfiles.Where(x => x.CNIC.Equals(cnic)).ToList();
                    List<int> IdstoFind = new List<int>();

                    if (profilelist.Count() > 0)
                    {
                        foreach (var item in profilelist)
                        {
                            IdstoFind.Add(item.Id);
                        }
                    }

                    if (IdstoFind != null)
                    {
                        var servicelist = _db.HrServiceHistories.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var leaveRecords = _db.HrLeaveRecords.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var ESRlist = _db.ESRs.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var leaveORderlist = _db.LeaveOrders.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var ApplicationMasterlist = _db.ApplicationMasters.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var AppFileRecolist = _db.ApplicationFileRecositions.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var transferApplications = _db.TransferApplications.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var leaveApplications = _db.LeaveApplications.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();
                        var promotionApplications = _db.PromotionJobApplications.Where(x => IdstoFind.Contains((int)x.Profile_Id)).ToList();

                        if (servicelist.Count() > 0)
                        {
                            foreach (var item in servicelist)
                            {
                                HrServiceHistory leaveorder = _db.HrServiceHistories.FirstOrDefault(x => x.Id == item.Id);
                                leaveorder.Profile_Id = Id;
                                _db.Entry(leaveorder).State = EntityState.Modified;
                                //_db.SaveChanges();
                            }
                        }
                        if (leaveRecords.Count() > 0)
                        {
                            foreach (var item in leaveRecords)
                            {
                                HrLeaveRecord leaveRecord = _db.HrLeaveRecords.FirstOrDefault(x => x.Id == item.Id);
                                leaveRecord.Profile_Id = Id;
                                _db.Entry(leaveRecord).State = EntityState.Modified;
                                //_db.SaveChanges();
                            }
                        }

                        if (ESRlist.Count() > 0)
                        {
                            foreach (var item in ESRlist)
                            {
                                ESR esr = _db.ESRs.FirstOrDefault(x => x.Id == item.Id);
                                esr.Profile_Id = Id;
                                _db.Entry(esr).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        if (leaveORderlist.Count() > 0)
                        {
                            foreach (var item in leaveORderlist)
                            {
                                LeaveOrder leaveorder = _db.LeaveOrders.FirstOrDefault(x => x.Id == item.Id);
                                leaveorder.Profile_Id = Id;
                                _db.Entry(leaveorder).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        if (ApplicationMasterlist.Count() > 0)
                        {
                            foreach (var item in ApplicationMasterlist)
                            {
                                ApplicationMaster appMaster = _db.ApplicationMasters.FirstOrDefault(x => x.Id == item.Id);
                                appMaster.Profile_Id = Id;
                                _db.Entry(appMaster).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        if (AppFileRecolist.Count() > 0)
                        {
                            foreach (var item in AppFileRecolist)
                            {
                                ApplicationFileRecosition appFileRecosition = _db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == item.Id);
                                appFileRecosition.Profile_Id = Id;
                                _db.Entry(appFileRecosition).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                        if (transferApplications.Count() > 0)
                        {
                            foreach (var item in transferApplications)
                            {
                                var transferApplication = _db.TransferApplications.FirstOrDefault(x => x.Id == item.Id);
                                transferApplication.Profile_Id = Id;
                                _db.Entry(transferApplication).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                        if (leaveApplications.Count() > 0)
                        {
                            foreach (var item in leaveApplications)
                            {
                                var leaveApplication = _db.LeaveApplications.FirstOrDefault(x => x.Id == item.Id);
                                leaveApplication.Profile_Id = Id;
                                _db.Entry(leaveApplication).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        if (promotionApplications.Count() > 0)
                        {
                            foreach (var item in promotionApplications)
                            {
                                var promotionApplication = _db.PromotionJobApplications.FirstOrDefault(x => x.Id == item.Id);
                                promotionApplication.Profile_Id = Id;
                                _db.Entry(promotionApplication).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }

                    var list = profilelist.Where(x => x.Id != Id).ToList();

                    HrDuplicationLog duplicationlog = new HrDuplicationLog();
                    duplicationlog.ProfileId = Id;
                    duplicationlog.RemovedIds = list.ToString();
                    duplicationlog.UserId = userId;
                    duplicationlog.UserName = username;
                    _db.HrDuplicationLogs.Add(duplicationlog);

                    _db.HrProfiles.RemoveRange(list);
                    _db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                    throw;
                }

            }
        }
        public HrAttachedPerson SaveAttachedPerson(HrAttachedPerson hrAttachedPerson, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hrAttachedPerson.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var dbObj = _db.HrAttachedPersons.Where(x => x.ProfileId == hrAttachedPerson.ProfileId && x.AttachedPersonId == hrAttachedPerson.AttachedPersonId && x.IsActive == true).ToList();
                            if (dbObj.Count > 0)
                            {
                                return null;
                            }
                            hrAttachedPerson.IsActive = true;
                            hrAttachedPerson.UserName = userName;
                            hrAttachedPerson.DateTime = DateTime.UtcNow.AddHours(5);
                            hrAttachedPerson.UserId = userId;
                            _db.HrAttachedPersons.Add(hrAttachedPerson);
                            _db.SaveChanges();
                            return hrAttachedPerson;
                        }
                        else
                        {
                            var dbObj = _db.HrAttachedPersons.FirstOrDefault(x => x.Id == hrAttachedPerson.Id);
                            if (dbObj != null)
                            {
                                dbObj.IsDiploma = hrAttachedPerson.IsDiploma;
                                dbObj.OnTraining = hrAttachedPerson.OnTraining;
                                _db.Entry(dbObj).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<HrLeaveRecordView> GetLeaveRecord(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var leaveRecords = _db.HrLeaveRecordViews.Where(x => x.Profile_Id == profileId && x.IsActive == true).OrderByDescending(k => k.FromDate).ToList();
                        return leaveRecords;
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

        public HrReviewSubmission SubmitForReview(HrReviewSubmission reviewSubmission, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == reviewSubmission.ProfileId);
                    try
                    {
                        if (profile == null) return null;

                        var reviewSubmissionDb = _db.HrReviewSubmissions.FirstOrDefault(x => x.ProfileId == reviewSubmission.ProfileId);
                        if (reviewSubmissionDb != null) return reviewSubmissionDb;

                        reviewSubmission.SubmitBy = userName;
                        reviewSubmission.Status_Id = 1;
                        reviewSubmission.IsActive = true;
                        reviewSubmission.Username = userName;
                        reviewSubmission.DateTime = DateTime.UtcNow.AddHours(5);
                        reviewSubmission.UserId = userId;

                        _db.HrReviewSubmissions.Add(reviewSubmission);
                        _db.SaveChanges();
                        return reviewSubmission;
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

        public HrReview SubmitReview(HrReview review, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var reviewSubmission = _db.HrReviewSubmissions.FirstOrDefault(x => x.Id == review.ReviewSubmissionId);
                        if (reviewSubmission != null)
                        {
                            if (review.StatusId == 2)
                            {
                                reviewSubmission.Status_Id = 2;
                                _db.Entry(reviewSubmission).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        var officer = _db.PandSOfficerViews.FirstOrDefault(x => x.User_Id.Equals(userId));
                        if (officer != null)
                        {
                            review.OfficerId = officer.Id;
                        }
                        review.IsActive = true;
                        review.Username = userName;
                        review.DateTime = DateTime.UtcNow.AddHours(5);
                        review.UserId = userId;

                        _db.HrReviews.Add(review);
                        _db.SaveChanges();
                        return review;
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
        public bool RemoveLeaveRecord(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var leaveRecord = _db.HrLeaveRecords.FirstOrDefault(x => x.Id == Id);
                    try
                    {
                        if (leaveRecord != null)
                        {
                            var entity_Lifecycle = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == leaveRecord.EntityLifecycle_Id);
                            if (entity_Lifecycle != null)
                            {
                                entity_Lifecycle.IsActive = false;
                                _db.Entry(entity_Lifecycle).State = EntityState.Modified;
                                _db.SaveChanges();

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)leaveRecord.EntityLifecycle_Id;
                                eml.Description = "Leave Record Removed by " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();
                                return true;
                            }
                        }
                        return false;
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
        public HrLeaveRecord SaveLeaveRecord(HrLeaveRecord hrLeaveRecord, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hrLeaveRecord.Id == 0)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.IsActive = true;
                            elc.Created_By = userName;
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Users_Id = userId;
                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();

                            hrLeaveRecord.EntityLifecycle_Id = elc.Id;

                            if (hrLeaveRecord.OrderDate != null && hrLeaveRecord.OrderDate.Value.Hour == 19) { hrLeaveRecord.OrderDate = hrLeaveRecord.OrderDate.Value.AddHours(5); }

                            _db.HrLeaveRecords.Add(hrLeaveRecord);
                            _db.SaveChanges();

                            return hrLeaveRecord;
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public TableResponse<HrServiceHistoryView> GetServiceDetails(ProfileFilters filters, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.Id == filters.Id);
                    var list = _db.HrServiceHistoryViews.Where(x => x.Profile_Id == filters.Id && x.IsActive == true)
                        .OrderBy(x => x.From_Date).ToList();

                    if (profile == null) return null;
                    if (profile.DateOfFirstAppointment == null) return null;

                    var result = CalculateGapPeriod(profile, list);


                    return new TableResponse<HrServiceHistoryView>() { List = result };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public HrComplain SaveHrComplain(HrComplain hrComplain, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hrComplain.Id == 0)
                        {

                            hrComplain.IsActive = true;
                            hrComplain.CreatedBy = userName;
                            hrComplain.DateTime = DateTime.UtcNow.AddHours(5);
                            hrComplain.UserId = userId;
                            _db.HrComplains.Add(hrComplain);
                            _db.SaveChanges();
                            return hrComplain;
                        }
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public TableResponse<HrComplain> GetHrCompalins(string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var list = _db.HrComplains.Where(x => x.UserId.Equals(userId) && x.IsActive == true)
                        .OrderByDescending(x => x.DateTime).ToList();
                    return new TableResponse<HrComplain>() { List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrReviewView> GetHrReviews(int reviewSubmissionId, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var reviews = _db.HrReviewViews.Where(x => x.ReviewSubmissionId == reviewSubmissionId && x.IsActive == true).OrderByDescending(x => x.DateTime).ToList();
                    return reviews;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        private List<HrServiceHistoryView> CalculateGapPeriod(ProfileDetailsView profile, List<HrServiceHistoryView> serviceHistories)
        {
            List<HrServiceHistoryView> list = new List<HrServiceHistoryView>();
            int startYear = profile.DateOfFirstAppointment.Value.Year;
            int endYear = DateTime.Now.Year;
            if (profile.DateOfBirth.Value.AddYears(60) <= DateTime.Now)
            {
                endYear = profile.DateOfBirth.Value.AddYears(60).Year;
            }

            DateTime startDate = profile.DateOfFirstAppointment.Value, endDate;
            for (int i = startYear; i <= endYear; i++)
            {
                var currentYearService = serviceHistories.Where(x => x.From_Date.Value.Year == i && x.To_Date.Value.Year == i).ToList();

                if (currentYearService.Count == 0) // if no record found of this year
                {
                    list.Add(new HrServiceHistoryView() { From_Date = startDate, To_Date = new DateTime(i, 12, 31) });
                    startDate = new DateTime(i, 12, 31).AddDays(1);
                    continue;
                }

                var count = 0;
                foreach (var item in currentYearService) // if acr records exists of this year
                {
                    count++;
                    if (startDate < item.From_Date && item.To_Date.Value.Year == i) // if gap exists between From Period and Year Start Date
                    {
                        list.Add(new HrServiceHistoryView() { From_Date = startDate, To_Date = item.From_Date.Value.AddDays(-1) });
                    }
                    startDate = item.To_Date.Value.AddDays(1);
                    endDate = item.To_Date.Value.AddDays(1);
                    list.Add(item);
                }
                if (count == currentYearService.Count && (startDate < new DateTime(i, 12, 31))) // if gap exists in current Year and last ACR Record Date
                {
                    list.Add(new HrServiceHistoryView() { From_Date = startDate, To_Date = new DateTime(i, 12, 31) });
                    startDate = new DateTime(i, 12, 31).AddDays(1);
                }
            }

            return list;
        }
        public HrProfile AddToPool(int id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == id);
                try
                {
                    if (profile == null) return null;

                    profile.AddToEmployeePool = true;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName + " (added after migration)";
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        profile.EntityLifecycle_Id = elc.Id;
                    }

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile added to Pool By " + userName;
                    _db.Entity_Modified_Log.Add(eml);

                    _db.Entry(profile).State = EntityState.Modified;
                    _db.SaveChanges();
                    return profile;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ProfileListView> GetProfilesInPool(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => x.AddToEmployeePool == true).AsQueryable(); ;
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }
                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
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
                    var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<ProfileListView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public ProfileRemark PostProfileRemarks(ProfileRemark profileRemark, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (profileRemark.Id == 0)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName;
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 956;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        profileRemark.EntityLifecycle_Id = elc.Id;
                        _db.ProfileRemarks.Add(profileRemark);
                        _db.SaveChanges();

                        return profileRemark;
                    }
                    else
                    {
                        var profileRemarkDb = _db.ProfileRemarks.FirstOrDefault(x => x.Id == profileRemark.Id);

                        if (profileRemarkDb.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName;
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 956;
                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();
                            profileRemarkDb.EntityLifecycle_Id = elc.Id;
                            _db.SaveChanges();
                        }

                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)profileRemarkDb.EntityLifecycle_Id;
                        eml.Description = userName;
                        _db.Entity_Modified_Log.Add(eml);

                        profileRemarkDb.Title = profileRemark.Title;
                        profileRemarkDb.Remarks = profileRemark.Remarks;

                        _db.Entry(profileRemarkDb).State = EntityState.Modified;
                        _db.SaveChanges();
                        return profileRemarkDb;
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public HrSMSEmployee SendSMSToEmployee(HrSMSEmployee hrSMSEmployee, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (hrSMSEmployee.Id == 0)
                    {
                        var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == hrSMSEmployee.Profile_Id);
                        if (profile != null)
                        {
                            string message = $@"Dear {profile.EmployeeName}\nCNIC: {profile.CNIC}\nUnqiue Id: {profile.Id}\n
You are requested to apply for your Retirement Notification using online portal\nhttp://bit.ly/pshdretirement \n
Helpline: 042-99206173\n
Regards,\n
Primary and Secondary Healthcare Department";
                            //var sms = Common.Common.SendSMSTelenor(new SMS { Message = message, MobileNumber = profile.MobileNo });
                            var sms = Common.Common.SendSMSTelenor(new SMS { Message = message, MobileNumber = "03214677763", FKId = profile.Id });
                            if (sms != null)
                            {
                                hrSMSEmployee.UserId = userId;
                                hrSMSEmployee.Username = userName;
                                hrSMSEmployee.Datetime = DateTime.UtcNow.AddHours(5);
                                _db.HrSMSEmployees.Add(hrSMSEmployee);
                                _db.SaveChanges();
                                return hrSMSEmployee;
                            }
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<ProfileRemarksView> GetProfileRemarks(int profileId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var profileRemarks = _db.ProfileRemarksViews.Where(x => x.Profile_Id == profileId && x.IsActive == true).OrderByDescending(k => k.Created_Date).ToList();

                    return profileRemarks;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Entity_Log_View> GetProfileLogs(int profileId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profileLogs = _db.Entity_Log_View.Where(x => x.Entity_Id == 9 && x.FK_Id == profileId && x.IsActive == true && x.Username != "dpd").OrderBy(k => k.Datetime).ToList();
                    return profileLogs;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool RemoveProfileRemarks(int id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var profileRemarks = _db.ProfileRemarks.FirstOrDefault(x => x.Id == id);

                    var elcDb = _db.Entity_Lifecycle.FirstOrDefault(x => profileRemarks.EntityLifecycle_Id == x.Id);
                    elcDb.IsActive = false;
                    _db.Entry(elcDb).State = EntityState.Modified;
                    _db.SaveChanges();

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)elcDb.Id;
                    eml.Description = "Remarks Removed by " + userName;
                    _db.Entity_Modified_Log.Add(eml);
                    _db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public TableResponse<ProfileListView> GetProfilesInActive(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => x.Status_Id == 16).AsQueryable(); ;
                    if (filters.roleName == "South Punjab")
                    {
                        query = query.Where(x => Common.Common.southDistrictNames.Contains(x.District)).AsQueryable();
                    }
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }
                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
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
                    var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<ProfileListView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public HrProfile RemoveFromPool(int id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == id);
                try
                {
                    if (profile == null) return null;

                    profile.AddToEmployeePool = false;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName + " (added after migration)";
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        profile.EntityLifecycle_Id = elc.Id;
                    }

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile removed from Pool By " + userName;
                    _db.Entity_Modified_Log.Add(eml);

                    _db.Entry(profile).State = EntityState.Modified;
                    _db.SaveChanges();
                    return profile;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrSeniorityApplicationView> GenerateSeniorityList(int CategoryId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    ProfileFilters filters = new ProfileFilters();
                    filters.statuses = new List<int?>() { 1, 8, 11, 13, 16, 23, 24, 25, 27, 28, 29, 36 };
                    if (CategoryId == 1)
                    {
                        filters.designations = new List<int?>() { 362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313 };
                    }
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => !filters.statuses.Contains(x.Status_Id)).AsQueryable();
                    query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    query = query.Where(x => x.EmpMode_Id == 13).AsQueryable();

                    var list = query.OrderBy(x => x.FirstJoiningDate == null).ThenBy(x => x.FirstJoiningDate).ThenBy(x => x.PPSCMeritNumber == null).ThenBy(x => x.PPSCMeritNumber).ThenBy(x => x.DateOfBirth).ToList();
                    int c = 1;

                    foreach (var item in list)
                    {
                        HrSeniorityApplication hrSeniority = new HrSeniorityApplication();
                        hrSeniority.Profile_Id = item.Id;
                        hrSeniority.CategoryId = filters.CategoryId;

                        hrSeniority.EmployeeName = item.EmployeeName;
                        hrSeniority.FatherName = item.FatherName;
                        hrSeniority.CNIC = item.CNIC;
                        hrSeniority.DateOfBirth = item.DateOfBirth;
                        hrSeniority.Gender = item.Gender;
                        hrSeniority.Province = item.Province;
                        hrSeniority.MaritalStatus = item.MaritalStatus;
                        hrSeniority.BloodGroup = item.BloodGroup;

                        hrSeniority.SeniorityNo = item.SeniorityNo;

                        hrSeniority.PersonnelNo = item.PersonnelNo;
                        hrSeniority.JoiningGradeBPS = item.JoiningGradeBPS;
                        hrSeniority.CurrentGradeBPS = item.CurrentGradeBPS;
                        hrSeniority.PresentPostingOrderNo = item.PresentPostingOrderNo;
                        hrSeniority.PresentPostingDate = item.PresentPostingDate;
                        hrSeniority.AdditionalQualification = item.AdditionalQualification;
                        hrSeniority.Status = item.Status;
                        hrSeniority.DateOfFirstAppointment = item.DateOfFirstAppointment;
                        hrSeniority.SuperAnnuationDate = item.SuperAnnuationDate;
                        hrSeniority.ContractStartDate = item.ContractStartDate;
                        hrSeniority.ContractEndDate = item.ContractEndDate;
                        hrSeniority.LastPromotionDate = item.LastPromotionDate;
                        hrSeniority.PermanentAddress = item.PermanentAddress;
                        hrSeniority.CorrespondenceAddress = item.CorrespondenceAddress;
                        hrSeniority.LandlineNo = item.LandlineNo;
                        hrSeniority.MobileNo = item.MobileNo;
                        hrSeniority.EMaiL = item.EMaiL;
                        hrSeniority.PrivatePractice = item.PrivatePractice;
                        hrSeniority.PresentStationLengthOfService = item.PresentStationLengthOfService;
                        hrSeniority.Tenure = item.Tenure;
                        hrSeniority.AdditionalCharge = item.AdditionalCharge;
                        hrSeniority.Remarks = item.Remarks;
                        hrSeniority.Photo = item.Photo;
                        hrSeniority.HighestQualification = item.HighestQualification;
                        hrSeniority.MobileNoOfficial = item.MobileNoOfficial;
                        hrSeniority.Postaanctionedwithscale = item.Postaanctionedwithscale;
                        hrSeniority.Faxno = item.Faxno;
                        hrSeniority.HoD = item.HoD;
                        hrSeniority.Fp = item.Fp;
                        hrSeniority.Hfac = item.Hfac;
                        hrSeniority.DateOfRegularization = item.DateOfRegularization;
                        hrSeniority.Tbydeo = item.Tbydeo;
                        hrSeniority.DateOfCourse = item.DateOfCourse;
                        hrSeniority.RtmcNo = item.RtmcNo;
                        hrSeniority.PmdcNo = item.PmdcNo;
                        hrSeniority.CourseDuration = item.CourseDuration;
                        hrSeniority.PgSpecialization = item.PgSpecialization;
                        hrSeniority.Category = item.Category;
                        hrSeniority.RemunerationStatus = item.RemunerationStatus;
                        hrSeniority.PgFlag = item.PgFlag;
                        hrSeniority.CourseName = item.CourseName;
                        hrSeniority.AddToEmployeePool = item.AddToEmployeePool;
                        hrSeniority.Domicile_Id = item.Domicile_Id;
                        hrSeniority.Language_Id = item.Language_Id;
                        hrSeniority.Designation_Id = item.Designation_Id;
                        hrSeniority.WDesignation_Id = item.WDesignation_Id;
                        hrSeniority.Cadre_Id = item.Cadre_Id;
                        hrSeniority.EmpMode_Id = item.EmpMode_Id;
                        hrSeniority.HealthFacility_Id = item.HealthFacility_Id;
                        hrSeniority.Department_Id = item.Department_Id;
                        hrSeniority.Religion_Id = item.Religion_Id;
                        hrSeniority.Posttype_Id = item.Posttype_Id;
                        hrSeniority.HfmisCode = item.HfmisCode;
                        hrSeniority.HfmisCodeOld = item.HfmisCodeOld;
                        hrSeniority.Created_By = item.Created_By;
                        hrSeniority.Creation_Date = DateTime.UtcNow.AddHours(5);
                        hrSeniority.IsActive = true;
                        hrSeniority.EntityLifecycle_Id = item.EntityLifecycle_Id;
                        hrSeniority.Qualification_Id = item.Qualification_Id;
                        hrSeniority.Status_Id = item.Status_Id;
                        hrSeniority.Specialization_Id = item.Specialization_Id;
                        hrSeniority.ProfilePhoto = item.ProfilePhoto;
                        hrSeniority.WorkingHealthFacility_Id = item.WorkingHealthFacility_Id;
                        hrSeniority.WorkingHFMISCode = item.WorkingHFMISCode;
                        hrSeniority.Disability_Id = item.Disability_Id;
                        hrSeniority.Disability = item.Disability;
                        hrSeniority.PresentJoiningDate = item.PresentJoiningDate;
                        hrSeniority.AttachedWith = item.AttachedWith;
                        hrSeniority.AttachedWith_Id = item.AttachedWith_Id;
                        hrSeniority.FileNumber = item.FileNumber;
                        hrSeniority.VacCertificate = item.VacCertificate;
                        hrSeniority.PPSCMeritNumber = item.PPSCMeritNumber;
                        hrSeniority.ModeId = item.ModeId;
                        hrSeniority.FirstJoiningDate = item.FirstJoiningDate;
                        hrSeniority.SeniorityNumber = c++;

                        hrSeniority.IsVerified = item.IsVerified;
                        hrSeniority.FirstOrderDate = item.FirstOrderDate;
                        hrSeniority.FirstOrderNumber = item.FirstOrderNumber;
                        hrSeniority.RegularOrderNumber = item.RegularOrderNumber;
                        hrSeniority.PromotionOrderNumber = item.PromotionOrderNumber;
                        hrSeniority.ContractOrderNumber = item.ContractOrderNumber;
                        hrSeniority.ContractOrderDate = item.ContractOrderDate;
                        hrSeniority.PeriodofContract = item.PeriodofContract;
                        hrSeniority.OtherContract = item.OtherContract;
                        hrSeniority.PromotionJoiningDate = item.PromotionJoiningDate;

                        _db.HrSeniorityApplications.Add(hrSeniority);
                        _db.SaveChanges();
                    }
                    var cnics = _db.HrSeniorityApplications.Select(x => x.CNIC).ToList();


                    foreach (var item in _db.HrProfileSps.Where(x => !cnics.Contains(x.CNIC)
                    && x.EmploymentMode.ToLower().Equals("regular")).ToList())
                    {
                        HrSeniorityApplication hrSeniority = new HrSeniorityApplication();
                        hrSeniority.Profile_Id = item.Id;
                        hrSeniority.CategoryId = filters.CategoryId;
                        hrSeniority.EmployeeName = item.EmployeeName;
                        hrSeniority.FatherName = item.FatherName;
                        hrSeniority.CNIC = item.CNIC;
                        hrSeniority.DateOfBirth = item.DateOfBirth;
                        hrSeniority.Gender = item.Gender;
                        hrSeniority.Province = item.Province;
                        hrSeniority.MaritalStatus = item.MaritalStatus;
                        hrSeniority.BloodGroup = item.BloodGroup;

                        hrSeniority.SeniorityNo = item.SeniorityNo;

                        hrSeniority.PersonnelNo = item.PersonnelNo;
                        hrSeniority.JoiningGradeBPS = item.JoiningGradeBPS;
                        hrSeniority.CurrentGradeBPS = item.CurrentGradeBPS;
                        hrSeniority.PresentPostingOrderNo = item.PresentPostingOrderNo;
                        hrSeniority.PresentPostingDate = item.PresentPostingDate;
                        hrSeniority.AdditionalQualification = item.AdditionalQualification;
                        hrSeniority.Status = item.Status;
                        hrSeniority.DateOfFirstAppointment = item.DateOfFirstAppointment;
                        hrSeniority.SuperAnnuationDate = item.SuperAnnuationDate;
                        hrSeniority.ContractStartDate = item.ContractStartDate;
                        hrSeniority.ContractEndDate = item.ContractEndDate;
                        hrSeniority.LastPromotionDate = item.LastPromotionDate;
                        hrSeniority.PermanentAddress = item.PermanentAddress;
                        hrSeniority.CorrespondenceAddress = item.CorrespondenceAddress;
                        hrSeniority.LandlineNo = item.LandlineNo;
                        hrSeniority.MobileNo = item.MobileNo;
                        hrSeniority.EMaiL = item.EMaiL;
                        hrSeniority.PrivatePractice = item.PrivatePractice;
                        hrSeniority.PresentStationLengthOfService = item.PresentStationLengthOfService;
                        hrSeniority.Tenure = item.Tenure;
                        hrSeniority.AdditionalCharge = item.AdditionalCharge;
                        hrSeniority.Remarks = item.Remarks;
                        hrSeniority.Photo = item.Photo;
                        hrSeniority.HighestQualification = item.HighestQualification;
                        hrSeniority.MobileNoOfficial = item.MobileNoOfficial;
                        hrSeniority.Postaanctionedwithscale = item.Postaanctionedwithscale;
                        hrSeniority.Faxno = item.Faxno;
                        hrSeniority.HoD = item.HoD;
                        hrSeniority.Fp = item.Fp;
                        hrSeniority.Hfac = item.Hfac;
                        hrSeniority.DateOfRegularization = item.DateOfRegularization;
                        hrSeniority.Tbydeo = item.Tbydeo;
                        hrSeniority.DateOfCourse = item.DateOfCourse;
                        hrSeniority.RtmcNo = item.RtmcNo;
                        hrSeniority.PmdcNo = item.PmdcNo;
                        hrSeniority.CourseDuration = item.CourseDuration;
                        hrSeniority.PgSpecialization = item.PgSpecialization;
                        hrSeniority.Category = item.Category;
                        hrSeniority.RemunerationStatus = item.RemunerationStatus;
                        hrSeniority.PgFlag = item.PgFlag;
                        hrSeniority.CourseName = item.CourseName;
                        hrSeniority.AddToEmployeePool = item.AddToEmployeePool;
                        hrSeniority.Domicile_Id = item.Domicile_Id;
                        hrSeniority.Language_Id = item.Language_Id;
                        hrSeniority.Designation_Id = item.Designation_Id;
                        hrSeniority.WDesignation_Id = item.WDesignation_Id;
                        hrSeniority.Cadre_Id = item.Cadre_Id;
                        hrSeniority.EmpMode_Id = item.EmpMode_Id;
                        hrSeniority.HealthFacility_Id = item.HealthFacility_Id;
                        hrSeniority.Department_Id = item.Department_Id;
                        hrSeniority.Religion_Id = item.Religion_Id;
                        hrSeniority.Posttype_Id = item.Posttype_Id;
                        hrSeniority.HfmisCode = item.HfmisCode;
                        hrSeniority.HfmisCodeOld = item.HfmisCodeOld;
                        hrSeniority.Created_By = item.Created_By;
                        hrSeniority.Creation_Date = DateTime.UtcNow.AddHours(5);
                        hrSeniority.IsActive = true;
                        hrSeniority.EntityLifecycle_Id = item.EntityLifecycle_Id;
                        hrSeniority.Qualification_Id = item.Qualification_Id;
                        hrSeniority.Status_Id = item.Status_Id;
                        hrSeniority.Specialization_Id = item.Specialization_Id;
                        hrSeniority.ProfilePhoto = item.ProfilePhoto;
                        hrSeniority.WorkingHealthFacility_Id = item.WorkingHealthFacility_Id;
                        hrSeniority.WorkingHFMISCode = item.WorkingHFMISCode;
                        hrSeniority.Disability_Id = item.Disability_Id;
                        hrSeniority.Disability = item.Disability;
                        hrSeniority.PresentJoiningDate = item.PresentJoiningDate;
                        hrSeniority.AttachedWith = item.AttachedWith;
                        hrSeniority.AttachedWith_Id = item.AttachedWith_Id;
                        hrSeniority.FileNumber = item.FileNumber;
                        hrSeniority.VacCertificate = item.VacCertificate;
                        hrSeniority.PPSCMeritNumber = item.PPSCMeritNumber;
                        hrSeniority.ModeId = item.ModeId;
                        hrSeniority.FirstJoiningDate = item.FirstJoiningDate;
                        hrSeniority.HealthFacilityNameC = item.HealthFacilityName;
                        hrSeniority.DesignationNameC = item.DesignationName;
                        hrSeniority.EmploymentModeC = item.EmploymentMode;
                        hrSeniority.DistrictC = item.District;
                        hrSeniority.SeniorityNumber = c++;
                        _db.HrSeniorityApplications.Add(hrSeniority);
                        _db.SaveChanges();
                    }
                    return GetSeniorityList(filters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrSeniorityApplicationView_Fixed> GetSeniorityListFixed(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<HrSeniorityApplicationView_Fixed> query = _db.HrSeniorityApplicationView_Fixed.AsQueryable();
                    if (filters.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filters.Designation_Id).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filters.DivisionCode))
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.DivisionCode)).AsQueryable();
                    }
                    if (filters.PostingModeId > 0)
                    {
                        if (filters.PostingModeId == 1)
                        {
                            query = query.Where(x => x.PromotionJoiningDate != null || x.ModeId == 1).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.ModeId == filters.PostingModeId && x.PromotionJoiningDate == null).AsQueryable();
                        }
                    }
                    if (!string.IsNullOrEmpty(filters.searchTerm))
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower()) || x.FatherName.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    //var list = query.OrderBy(x => x.SeniorityNumber).ToList();
                    var list = query.OrderBy(x => x.JoiningDate == null).ThenBy(x => x.JoiningDate).ThenBy(x => x.PPSCMeritNumber == null).ThenBy(x => x.PPSCMeritNumber).ThenBy(x => x.DateOfBirth).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public SeniorityApplicantDto GetSeniorityApplicant(string cnic)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    SeniorityApplicantDto applicantDto = new SeniorityApplicantDto();

                    var user = _db.C_User.FirstOrDefault(x => x.UserName.Equals(cnic));
                    if (user != null)
                    {
                        applicantDto.Password = user.hashynoty;
                    }
                    applicantDto.profile = _db.ProfileSeniorityViews.FirstOrDefault(x => x.CNIC == cnic);
                    applicantDto.application = _db.HrSeniorityApplicationViews.FirstOrDefault(x => x.CNIC == cnic);
                    if (applicantDto.application != null)
                    {
                        applicantDto.logs = _db.HrSeniorityApplicationLogs.Where(x => x.HrSeniorityApplicationId == applicantDto.application.Id).ToList();
                    }
                    applicantDto.seniorityListData = _db.HrSeniorityApplicationView_Fixed.FirstOrDefault(x => x.CNIC == cnic);
                    return applicantDto;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrSeniorityApplicationView> GetSeniorityList(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<HrSeniorityApplicationView> query = _db.HrSeniorityApplicationViews.AsQueryable();
                    if (filters.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filters.Designation_Id).AsQueryable();
                    }
                    if (filters.ApplicationStatusId > 0)
                    {
                        if (filters.ApplicationStatusId == 1)
                        {
                            query = query.Where(x => x.ApplicationStatus_Id == 1).AsQueryable();
                        }
                        if (filters.ApplicationStatusId == 2)
                        {
                            query = query.Where(x => x.ApplicationStatus_Id == 2).AsQueryable();

                        }
                        if (filters.ApplicationStatusId == 3)
                        {
                            query = query.Where(x => x.ApplicationStatus_Id == 3).AsQueryable();
                        }
                        if (filters.ApplicationStatusId == 4)
                        {
                            query = query.Where(x => x.IsLocked == true && x.ApplicationStatus_Id == null).AsQueryable();
                        }
                        if (filters.ApplicationStatusId == 5)
                        {
                            query = query.Where(x => x.IsLocked == null).AsQueryable();
                        }
                    }
                    if (!string.IsNullOrEmpty(filters.DivisionCode))
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.DivisionCode)).AsQueryable();
                    }
                    if (filters.PostingModeId > 0)
                    {
                        if (filters.PostingModeId == 1)
                        {
                            query = query.Where(x => x.PromotionJoiningDate != null || x.ModeId == 1).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.ModeId == filters.PostingModeId && x.PromotionJoiningDate == null).AsQueryable();
                        }
                    }
                    if (!string.IsNullOrEmpty(filters.searchTerm))
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower()) || x.FatherName.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    //var list = query.OrderBy(x => x.SeniorityNumber).ToList();
                    var list = query.OrderBy(x => x.JoiningDate == null).ThenBy(x => x.JoiningDate).ThenBy(x => x.PPSCMeritNumber == null).ThenBy(x => x.PPSCMeritNumber).ThenBy(x => x.DateOfBirth).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool VerifyProfileForPromotion(int profileId, string username, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    var dbProfile = _db.HrProfiles.FirstOrDefault(x => x.Id == profileId);
                    if (dbProfile != null)
                    {
                        dbProfile.IsVerified = true;
                        dbProfile.VerifiedBy = username;
                        dbProfile.VerifiedUserId = userId;
                        dbProfile.VerifiedDatetime = DateTime.UtcNow.AddHours(5);
                        _db.Entry(dbProfile).State = EntityState.Modified;
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

        public List<HrSeniorityApplicationDTO> GetNewSeniorityList(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    List<HrSeniorityApplicationDTO> res = new List<HrSeniorityApplicationDTO>();

                    filters.statuses = new List<int?>() { 1, 8, 11, 13, 16, 23, 24, 25, 27, 28, 29, 36 };
                    if (filters.CategoryId == 1)
                    {
                        filters.designations = new List<int?>() { 362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313 };
                    }
                    DateTime fiveYears = DateTime.Now.AddYears(-5);
                    IQueryable<ProfileSeniorityView> query = _db.ProfileSeniorityViews.Where(x => !filters.statuses.Contains(x.Status_Id)).AsQueryable();

                    query = query.Where(x => x.EmpMode_Id == 13).AsQueryable();

                    var date60 = DateTime.Now.AddYears(-60);

                    //query = query.Where(x => DbFunctions.TruncateTime(x.DateOfBirth)
                    //          > DbFunctions.TruncateTime(date60));

                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
                    if (filters.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filters.Designation_Id).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filters.DivisionCode))
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.DivisionCode)).AsQueryable();
                    }
                    if (filters.PostingModeId > 0)
                    {
                        if (filters.PostingModeId == 1)
                        {
                            query = query.Where(x => x.PromotionJoiningDate != null || x.ModeId == 1).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.ModeId == filters.PostingModeId && x.PromotionJoiningDate == null).AsQueryable();
                        }
                    }
                    if (!string.IsNullOrEmpty(filters.searchTerm))
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower()) || x.FatherName.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();

                    var list = query.OrderBy(x => x.JoiningDate == null).ThenBy(x => x.JoiningDate).ThenBy(x => x.PPSCMeritNumber == null).ThenBy(x => x.PPSCMeritNumber).ThenBy(x => x.DateOfBirth).ToList();
                    int c = 1;

                    foreach (var item in list)
                    {
                        HrSeniorityApplicationDTO hrSeniority = new HrSeniorityApplicationDTO();
                        hrSeniority.Profile_Id = item.Id;
                        hrSeniority.CategoryId = filters.CategoryId;

                        hrSeniority.EmployeeName = item.EmployeeName;
                        hrSeniority.FatherName = item.FatherName;
                        hrSeniority.CNIC = item.CNIC;
                        hrSeniority.DateOfBirth = item.DateOfBirth;
                        hrSeniority.Gender = item.Gender;
                        hrSeniority.Province = item.Province;
                        hrSeniority.MaritalStatus = item.MaritalStatus;
                        hrSeniority.BloodGroup = item.BloodGroup;

                        hrSeniority.SeniorityNo = item.SeniorityNo;

                        hrSeniority.PersonnelNo = item.PersonnelNo;
                        hrSeniority.JoiningGradeBPS = item.JoiningGradeBPS;
                        hrSeniority.CurrentGradeBPS = item.CurrentGradeBPS;
                        hrSeniority.PresentPostingOrderNo = item.PresentPostingOrderNo;
                        hrSeniority.PresentPostingDate = item.PresentPostingDate;
                        hrSeniority.AdditionalQualification = item.AdditionalQualification;
                        hrSeniority.Status = item.Status;
                        hrSeniority.DateOfFirstAppointment = item.DateOfFirstAppointment;
                        hrSeniority.SuperAnnuationDate = item.SuperAnnuationDate;
                        hrSeniority.ContractStartDate = item.ContractStartDate;
                        hrSeniority.ContractEndDate = item.ContractEndDate;
                        hrSeniority.LastPromotionDate = item.LastPromotionDate;
                        hrSeniority.PermanentAddress = item.PermanentAddress;
                        hrSeniority.CorrespondenceAddress = item.CorrespondenceAddress;
                        hrSeniority.LandlineNo = item.LandlineNo;
                        hrSeniority.MobileNo = item.MobileNo;
                        hrSeniority.EMaiL = item.EMaiL;
                        hrSeniority.PrivatePractice = item.PrivatePractice;
                        hrSeniority.PresentStationLengthOfService = item.PresentStationLengthOfService;
                        hrSeniority.Tenure = item.Tenure;
                        hrSeniority.AdditionalCharge = item.AdditionalCharge;
                        hrSeniority.Remarks = item.Remarks;
                        hrSeniority.Photo = item.Photo;
                        hrSeniority.HighestQualification = item.HighestQualification;
                        hrSeniority.MobileNoOfficial = item.MobileNoOfficial;
                        if (item.Postaanctionedwithscale != null)
                        {
                            hrSeniority.Postaanctionedwithscale = item.Postaanctionedwithscale.Value.ToString();
                        }
                        hrSeniority.Faxno = item.Faxno;
                        hrSeniority.HoD = item.HoD;
                        hrSeniority.Fp = item.Fp;
                        hrSeniority.Hfac = item.Hfac;
                        hrSeniority.DateOfRegularization = item.DateOfRegularization;
                        hrSeniority.Tbydeo = item.Tbydeo;
                        hrSeniority.DateOfCourse = item.DateOfCourse;
                        hrSeniority.RtmcNo = item.RtmcNo;
                        hrSeniority.PmdcNo = item.PmdcNo;
                        hrSeniority.CourseDuration = item.CourseDuration;
                        hrSeniority.PgSpecialization = item.PgSpecialization;
                        hrSeniority.Category = item.Category;
                        hrSeniority.RemunerationStatus = item.RemunerationStatus;
                        hrSeniority.PgFlag = item.PgFlag;
                        hrSeniority.CourseName = item.CourseName;
                        hrSeniority.AddToEmployeePool = item.AddToEmployeePool;
                        hrSeniority.Domicile_Id = item.Domicile_Id;
                        hrSeniority.Language_Id = item.Language_Id;
                        hrSeniority.Designation_Id = item.Designation_Id;
                        hrSeniority.WDesignation_Id = item.WDesignation_Id;
                        hrSeniority.Cadre_Id = item.Cadre_Id;
                        hrSeniority.EmpMode_Id = item.EmpMode_Id;
                        hrSeniority.HealthFacility_Id = item.HealthFacility_Id;
                        hrSeniority.Department_Id = item.Department_Id;
                        hrSeniority.Religion_Id = item.Religion_Id;
                        hrSeniority.Posttype_Id = item.Posttype_Id;
                        hrSeniority.HfmisCode = item.HfmisCode;
                        hrSeniority.Creation_Date = DateTime.UtcNow.AddHours(5);
                        hrSeniority.IsActive = true;
                        hrSeniority.EntityLifecycle_Id = item.EntityLifecycle_Id;
                        hrSeniority.Qualification_Id = item.Qualification_Id;
                        hrSeniority.Status_Id = item.Status_Id;
                        hrSeniority.Specialization_Id = item.Specialization_Id;
                        hrSeniority.ProfilePhoto = item.ProfilePhoto;
                        hrSeniority.WorkingHealthFacility_Id = item.WorkingHealthFacility_Id;
                        hrSeniority.WorkingHFMISCode = item.WorkingHFMISCode;
                        hrSeniority.Disability_Id = item.Disability_Id;
                        hrSeniority.Disability = item.Disability;
                        hrSeniority.PresentJoiningDate = item.PresentJoiningDate;
                        hrSeniority.AttachedWith = item.AttachedWith;
                        hrSeniority.AttachedWith_Id = item.AttachedWith_Id;
                        hrSeniority.FileNumber = item.FileNumber;
                        hrSeniority.VacCertificate = item.VacCertificate;
                        hrSeniority.PPSCMeritNumber = item.PPSCMeritNumber;
                        hrSeniority.ModeId = item.ModeId;
                        hrSeniority.FirstJoiningDate = item.FirstJoiningDate;
                        hrSeniority.SeniorityNumber = c++;

                        hrSeniority.HealthFacility = item.HealthFacility;
                        hrSeniority.Tehsil = item.Tehsil;
                        hrSeniority.District = item.District;
                        hrSeniority.StatusName = item.StatusName;
                        hrSeniority.Designation_Name = item.Designation_Name;
                        hrSeniority.Domicile_Name = item.Domicile_Name;

                        hrSeniority.IsVerified = item.IsVerified;
                        hrSeniority.VerifiedBy = item.VerifiedBy;
                        hrSeniority.VerifiedDatetime = item.VerifiedDatetime;
                        hrSeniority.VerifiedUserId = item.VerifiedUserId;
                        hrSeniority.JoiningDate = item.JoiningDate;
                        hrSeniority.PromotionJoiningDate = item.PromotionJoiningDate;
                        hrSeniority.ModeName = item.ModeName;

                        res.Add(hrSeniority);
                    }
                    var cnics = _db.HrSeniorityApplications.Select(x => x.CNIC).ToList();

                    var query2 = _db.HrProfileSps.Where(x => !cnics.Contains(x.CNIC) && x.EmploymentMode.ToLower().Equals("regular")).AsQueryable();
                    if (filters.Designation_Id > 0)
                    {
                        query2 = query2.Where(x => x.Designation_Id == filters.Designation_Id).AsQueryable();
                    }

                    var d = query2.ToList();

                    foreach (var item in d)
                    {
                        HrSeniorityApplicationDTO hrSeniority = new HrSeniorityApplicationDTO();
                        hrSeniority.Profile_Id = item.Id;
                        hrSeniority.CategoryId = filters.CategoryId;
                        hrSeniority.EmployeeName = item.EmployeeName;
                        hrSeniority.FatherName = item.FatherName;
                        hrSeniority.CNIC = item.CNIC;
                        hrSeniority.DateOfBirth = item.DateOfBirth;
                        hrSeniority.Gender = item.Gender;
                        hrSeniority.Province = item.Province;
                        hrSeniority.MaritalStatus = item.MaritalStatus;
                        hrSeniority.BloodGroup = item.BloodGroup;

                        hrSeniority.SeniorityNo = item.SeniorityNo;

                        hrSeniority.PersonnelNo = item.PersonnelNo;
                        hrSeniority.JoiningGradeBPS = item.JoiningGradeBPS;
                        hrSeniority.CurrentGradeBPS = item.CurrentGradeBPS;
                        hrSeniority.PresentPostingOrderNo = item.PresentPostingOrderNo;
                        hrSeniority.PresentPostingDate = item.PresentPostingDate;
                        hrSeniority.AdditionalQualification = item.AdditionalQualification;
                        hrSeniority.Status = item.Status;
                        hrSeniority.DateOfFirstAppointment = item.DateOfFirstAppointment;
                        hrSeniority.SuperAnnuationDate = item.SuperAnnuationDate;
                        hrSeniority.ContractStartDate = item.ContractStartDate;
                        hrSeniority.ContractEndDate = item.ContractEndDate;
                        hrSeniority.LastPromotionDate = item.LastPromotionDate;
                        hrSeniority.PermanentAddress = item.PermanentAddress;
                        hrSeniority.CorrespondenceAddress = item.CorrespondenceAddress;
                        hrSeniority.LandlineNo = item.LandlineNo;
                        hrSeniority.MobileNo = item.MobileNo;
                        hrSeniority.EMaiL = item.EMaiL;
                        hrSeniority.PrivatePractice = item.PrivatePractice;
                        hrSeniority.PresentStationLengthOfService = item.PresentStationLengthOfService;
                        hrSeniority.Tenure = item.Tenure;
                        hrSeniority.AdditionalCharge = item.AdditionalCharge;
                        hrSeniority.Remarks = item.Remarks;
                        hrSeniority.Photo = item.Photo;
                        hrSeniority.HighestQualification = item.HighestQualification;
                        hrSeniority.MobileNoOfficial = item.MobileNoOfficial;
                        hrSeniority.Postaanctionedwithscale = item.Postaanctionedwithscale;
                        hrSeniority.Faxno = item.Faxno;
                        hrSeniority.HoD = item.HoD;
                        hrSeniority.Fp = item.Fp;
                        hrSeniority.Hfac = item.Hfac;
                        hrSeniority.DateOfRegularization = item.DateOfRegularization;
                        hrSeniority.Tbydeo = item.Tbydeo;
                        hrSeniority.DateOfCourse = item.DateOfCourse;
                        hrSeniority.RtmcNo = item.RtmcNo;
                        hrSeniority.PmdcNo = item.PmdcNo;
                        hrSeniority.CourseDuration = item.CourseDuration;
                        hrSeniority.PgSpecialization = item.PgSpecialization;
                        hrSeniority.Category = item.Category;
                        hrSeniority.RemunerationStatus = item.RemunerationStatus;
                        hrSeniority.PgFlag = item.PgFlag;
                        hrSeniority.CourseName = item.CourseName;
                        hrSeniority.AddToEmployeePool = item.AddToEmployeePool;
                        hrSeniority.Domicile_Id = item.Domicile_Id;
                        hrSeniority.Language_Id = item.Language_Id;
                        hrSeniority.Designation_Id = item.Designation_Id;
                        hrSeniority.WDesignation_Id = item.WDesignation_Id;
                        hrSeniority.Cadre_Id = item.Cadre_Id;
                        hrSeniority.EmpMode_Id = item.EmpMode_Id;
                        hrSeniority.HealthFacility_Id = item.HealthFacility_Id;
                        hrSeniority.Department_Id = item.Department_Id;
                        hrSeniority.Religion_Id = item.Religion_Id;
                        hrSeniority.Posttype_Id = item.Posttype_Id;
                        hrSeniority.HfmisCode = item.HfmisCode;
                        hrSeniority.HfmisCodeOld = item.HfmisCodeOld;
                        hrSeniority.Created_By = item.Created_By;
                        hrSeniority.Creation_Date = DateTime.UtcNow.AddHours(5);
                        hrSeniority.IsActive = true;
                        hrSeniority.EntityLifecycle_Id = item.EntityLifecycle_Id;
                        hrSeniority.Qualification_Id = item.Qualification_Id;
                        hrSeniority.Status_Id = item.Status_Id;
                        hrSeniority.Specialization_Id = item.Specialization_Id;
                        hrSeniority.ProfilePhoto = item.ProfilePhoto;
                        hrSeniority.WorkingHealthFacility_Id = item.WorkingHealthFacility_Id;
                        hrSeniority.WorkingHFMISCode = item.WorkingHFMISCode;
                        hrSeniority.Disability_Id = item.Disability_Id;
                        hrSeniority.Disability = item.Disability;
                        hrSeniority.PresentJoiningDate = item.PresentJoiningDate;
                        hrSeniority.AttachedWith = item.AttachedWith;
                        hrSeniority.AttachedWith_Id = item.AttachedWith_Id;
                        hrSeniority.FileNumber = item.FileNumber;
                        hrSeniority.VacCertificate = item.VacCertificate;
                        hrSeniority.PPSCMeritNumber = item.PPSCMeritNumber;
                        hrSeniority.ModeId = item.ModeId;
                        hrSeniority.FirstJoiningDate = item.FirstJoiningDate;
                        hrSeniority.HealthFacilityNameC = item.HealthFacilityName;
                        hrSeniority.DesignationNameC = item.DesignationName;
                        hrSeniority.EmploymentModeC = item.EmploymentMode;
                        hrSeniority.DistrictC = item.District;

                        hrSeniority.HealthFacility = item.HealthFacilityName;
                        hrSeniority.District = item.District;
                        hrSeniority.Designation_Name = item.DesignationName;

                        hrSeniority.SeniorityNumber = c++;
                        hrSeniority.JoiningDate = item.FirstJoiningDate;
                        hrSeniority.PromotionJoiningDate = null;
                        hrSeniority.ModeName = null;

                        //hrSeniority.IsVerified = item.IsVerified;
                        //hrSeniority.VerifiedBy = item.VerifiedBy;
                        //hrSeniority.VerifiedDatetime = item.VerifiedDatetime;
                        //hrSeniority.VerifiedUserId = item.VerifiedUserId;
                        res.Add(hrSeniority);
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public TableResponse<ProfileListView> GetProfiles(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => x.Status_Id != 16).AsQueryable();
                    if (filters.roleName == "PHFMC Admin")
                    {
                        var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.Id).ToList();
                        query = query.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (filters.roleName == "PACP")
                    {
                        var hfIds = _db.HFListPs.Where(x => x.HFTypeCode.Equals("063")).Select(k => k.Id).ToList();
                        query = query.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (filters.roleName == "South Punjab")
                    {
                        query = query.Where(x => Common.Common.southDistrictNames.Contains(x.District)).AsQueryable();
                    }
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }
                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.statuses != null && filters.statuses.Count > 0)
                    {
                        query = query.Where(x => filters.statuses.Contains(x.Status_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
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
                    if (filters.retirementInOneYear == true || filters.retirementAlerted == true)
                    {
                        var alertedProfileIds = _db.HrSMSEmployees.Where(x => x.Profile_Id > 0).Select(k => k.Profile_Id).ToList();
                        if (filters.retirementInOneYear == true)
                        {
                            DateTime mydate = DateTime.UtcNow.AddHours(5).AddYears(-3);
                            DateTime dateAfterOneYear = DateTime.UtcNow.AddHours(5).AddYears(1);
                            query = query.Where(x => DbFunctions.TruncateTime(x.SuperAnnuationDate) >= DbFunctions.TruncateTime(mydate) &&
                            DbFunctions.TruncateTime(x.SuperAnnuationDate) <= DbFunctions.TruncateTime(dateAfterOneYear)
                            && x.CurrentGradeBPS > 15 && x.EmpMode_Id == 13 && x.Status_Id != 25 && !alertedProfileIds.Contains(x.Id)
                            ).AsQueryable();
                        }
                        if (filters.retirementAlerted == true)
                        {
                            query = query.Where(x => alertedProfileIds.Contains(x.Id)).AsQueryable();
                        }
                    }


                    var count = query.Count();
                    if (filters.retirementInOneYear == true || filters.retirementAlerted == true)
                    {
                        var list = query.OrderBy(x => x.SuperAnnuationDate).Skip(filters.Skip).Take(filters.PageSize).ToList();
                        return new TableResponse<ProfileListView> { List = list, Count = count };
                    }
                    else
                    {
                        var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                        return new TableResponse<ProfileListView> { List = list, Count = count };
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public TableResponse<ProfileListView> GetProfiles(ProfileFilters filters)
        //{
        //    try
        //    {
        //        using (var _db = new HR_System())
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;
        //            _db.Database.CommandTimeout = 60 * 4;
        //            var query = _db.SP_Pagination_Employee_Profile(filters.Skip, filters.PageSize).ToList();
        //            if (filters.roleName == "PHFMC Admin")
        //            {
        //                var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.Id).ToList();
        //                query = query.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).ToList();
        //            }
        //            if (filters.hfmisCode != null)
        //            {
        //                query = query?.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).ToList();
        //            }
        //            if (filters.cadres != null && filters.cadres.Count > 0)
        //            {
        //                query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).ToList();
        //            }
        //            if (filters.designations != null && filters.designations.Count > 0)
        //            {
        //                query = query.Where(x => filters.designations.Contains(x.Designation_Id)).ToList();
        //            }
        //            if (filters.statuses != null && filters.statuses.Count > 0)
        //            {
        //                query = query.Where(x => filters.statuses.Contains(x.Status_Id)).ToList();
        //            }
        //            if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
        //            {
        //                if (new RootService().IsCNIC(filters.searchTerm))
        //                {
        //                    filters.searchTerm = filters.searchTerm.Replace("-", "");
        //                    query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).ToList();
        //                }
        //                else
        //                {
        //                    query = query.Where(x => x.EmployeeName.ToLower().Contains(filters.searchTerm.ToLower())).ToList();
        //                }
        //            }
        //            if (filters.retirementInOneYear == true || filters.retirementAlerted == true)
        //            {
        //                var alertedProfileIds = _db.HrSMSEmployees.Where(x => x.Profile_Id > 0).Select(k => k.Profile_Id).ToList();
        //                if (filters.retirementInOneYear == true)
        //                {
        //                    DateTime mydate = DateTime.UtcNow.AddHours(5).AddYears(-3);
        //                    DateTime dateAfterOneYear = DateTime.UtcNow.AddHours(5).AddYears(1);
        //                    query = query.Where(x => DbFunctions.TruncateTime(x.SuperAnnuationDate) >= DbFunctions.TruncateTime(mydate) &&
        //                    DbFunctions.TruncateTime(x.SuperAnnuationDate) <= DbFunctions.TruncateTime(dateAfterOneYear)
        //                    && x.CurrentGradeBPS > 15 && x.EmpMode_Id == 13 && x.Status_Id != 25 && !alertedProfileIds.Contains(x.Id)
        //                    ).ToList();
        //                }
        //                if (filters.retirementAlerted == true)
        //                {
        //                    query = query.Where(x => alertedProfileIds.Contains(x.Id)).ToList();
        //                }
        //            }
        //            var count = query.Count();
        //             if (filters.retirementInOneYear == true || filters.retirementAlerted == true)
        //             {
        //                 var list = query.Select(d => new ProfileListView()
        //                 {
        //                    Id = d.Id,
        //                    Srno_old = d.Srno_old,
        //                    EmployeeName = d.EmployeeName,
        //                    FatherName = d.FatherName,
        //                     CNIC = d.CNIC,
        //                     DateOfBirth = d.DateOfBirth,
        //                    Gender = d.Gender,
        //                    Province = d.Province,
        //                    MaritalStatus = d.MaritalStatus,
        //                    BloodGroup = d.BloodGroup,
        //                    SeniorityNo = d.SeniorityNo,
        //                    PersonnelNo = d.PersonnelNo,
        //                    JoiningGradeBPS = d.JoiningGradeBPS,
        //                     CurrentGradeBPS = d.CurrentGradeBPS,
        //                    PresentPostingOrderNo = d.PresentPostingOrderNo,
        //                     PresentPostingDate = d.PresentPostingDate,
        //                    AdditionalQualification = d.AdditionalQualification,
        //                    Status = d.Status,
        //                    DateOfFirstAppointment = d.DateOfFirstAppointment, 
        //                    SuperAnnuationDate = d.SuperAnnuationDate,
        //                    ContractStartDate = d.ContractStartDate,
        //                    ContractEndDate  = d.ContractEndDate ,
        //                    LastPromotionDate = d.LastPromotionDate ,
        //                    PermanentAddress = d.PermanentAddress,
        //                    CorrespondenceAddress  = d.CorrespondenceAddress,
        //                    LandlineNo = d.LandlineNo,
        //                    MobileNo  = d.MobileNo,
        //                    EMaiL  = d.EMaiL,
        //                    PrivatePractice = d.PrivatePractice,
        //                    PresentStationLengthOfService = d.PresentStationLengthOfService,
        //                    Tenure  = d.Tenure,
        //                    AdditionalCharge = d.AdditionalCharge,
        //                    Remarks  = d.Remarks,
        //                    Photo = d.Photo,
        //                    HighestQualification = d.HighestQualification, 
        //                    MobileNoOfficial = d.MobileNoOfficial ,
        //                    Postaanctionedwithscale  = d.Postaanctionedwithscale,
        //                    Faxno = d.Faxno,
        //                    HoD  = d.HoD,
        //                    Fp = d.Fp,
        //                    Hfac = d.Hfac,
        //                    DateOfRegularization = d.DateOfRegularization, 
        //                    Tbydeo = d.Tbydeo,
        //                    DateOfCourse = d.DateOfCourse,
        //                    RtmcNo = d.RtmcNo,
        //                    PmdcNo = d.PmdcNo,
        //                    CourseDuration = d.CourseDuration,
        //                    PgSpecialization = d.PgSpecialization,
        //                    Category = d.Category,
        //                    RemunerationStatus = d.RemunerationStatus,
        //                    PgFlag = d.PgFlag,
        //                    CourseName = d.CourseName,
        //                     AddToEmployeePool = d.AddToEmployeePool,
        //                    Domicile_Id = d.Domicile_Id,
        //                    Language_Id  = d.Language_Id,
        //                    Designation_Id = d.Designation_Id,
        //                    WDesignation_Id = d.WDesignation_Id,
        //                    Cadre_Id = d.Cadre_Id,
        //                    EmpMode_Id = d.EmpMode_Id,
        //                    HealthFacility_Id = d.HealthFacility_Id,
        //                    Department_Id = d.Department_Id,
        //                    Religion_Id = d.Religion_Id,
        //                    Posttype_Id = d.Posttype_Id,
        //                    HfmisCode = d.HfmisCode ,
        //                    HfmisCodeOld = d.HfmisCodeOld,
        //                    Created_By = d.Created_By,
        //                    Creation_Date  = d.Creation_Date,
        //                    IsActive = d.IsActive,
        //                    EntityLifecycle_Id = d.EntityLifecycle_Id,
        //                    Qualification_Id = d.Qualification_Id,
        //                    Status_Id = d.Status_Id,
        //                    Specialization_Id = d.Specialization_Id,
        //                    ProfilePhoto = d.ProfilePhoto,
        //                    WorkingHealthFacility_Id = d.WorkingHealthFacility_Id,
        //                    WorkingHFMISCode = d.WorkingHFMISCode,
        //                    Disability_Id = d.Disability_Id,
        //                    Disability  = d.Disability,
        //                    PresentJoiningDate = d.PresentJoiningDate,
        //                    AttachedWith = d.AttachedWith,
        //                    AttachedWith_Id  = d.AttachedWith_Id,
        //                    FileNumber  = d.FileNumber,
        //                    EmpMode_Name = d.EmployeeName,
        //                    Cadre_Name = d.Cadre_Name,
        //                    Designation_Name  = d.Designation_Name,
        //                    Designation_HrScale_Id = d.Designation_HrScale_Id,
        //                    StatusName = d.StatusName,
        //                    WDesignation_Name =d.WDesignation_Name,
        //                    Department_Name = d.Department_Name,
        //                    Division = d.Division,
        //                    District = d.District,
        //                    Tehsil = d.Tehsil,
        //                    HealthFacility = d.HealthFacility,
        //                    WorkingHealthFacility = d.WorkingHealthFacility,
        //                        }
        //                 ).OrderBy(x => x.SuperAnnuationDate).ToList();
        //                 return new TableResponse<ProfileListView> { List = list, Count = count };
        //             }
        //             else
        //             {
        //                 var list = query.Select(d=> new ProfileListView
        //                 {
        //                    Id = d.Id,
        //                    Srno_old = d.Srno_old,
        //                    EmployeeName = d.EmployeeName,
        //                    FatherName = d.FatherName,
        //                     CNIC = d.CNIC,
        //                     DateOfBirth = d.DateOfBirth,
        //                    Gender = d.Gender,
        //                    Province = d.Province,
        //                    MaritalStatus = d.MaritalStatus,
        //                    BloodGroup = d.BloodGroup,
        //                    SeniorityNo = d.SeniorityNo,
        //                    PersonnelNo = d.PersonnelNo,
        //                    JoiningGradeBPS = d.JoiningGradeBPS,
        //                     CurrentGradeBPS = d.CurrentGradeBPS,
        //                    PresentPostingOrderNo = d.PresentPostingOrderNo,
        //                     PresentPostingDate = d.PresentPostingDate,
        //                    AdditionalQualification = d.AdditionalQualification,
        //                    Status = d.Status,
        //                    DateOfFirstAppointment = d.DateOfFirstAppointment, 
        //                    SuperAnnuationDate = d.SuperAnnuationDate,
        //                    ContractStartDate = d.ContractStartDate,
        //                    ContractEndDate = d.ContractEndDate ,
        //                    LastPromotionDate = d.LastPromotionDate ,
        //                    PermanentAddress = d.PermanentAddress,
        //                    CorrespondenceAddress = d.CorrespondenceAddress,
        //                    LandlineNo = d.LandlineNo,
        //                    MobileNo = d.MobileNo,
        //                    EMaiL = d.EMaiL,
        //                    PrivatePractice = d.PrivatePractice,
        //                    PresentStationLengthOfService = d.PresentStationLengthOfService,
        //                    Tenure = d.Tenure,
        //                    AdditionalCharge = d.AdditionalCharge,
        //                    Remarks = d.Remarks,
        //                    Photo = d.Photo,
        //                    HighestQualification = d.HighestQualification, 
        //                    MobileNoOfficial = d.MobileNoOfficial ,
        //                    Postaanctionedwithscale = d.Postaanctionedwithscale,
        //                    Faxno = d.Faxno,
        //                    HoD = d.HoD,
        //                    Fp = d.Fp,
        //                    Hfac = d.Hfac,
        //                    DateOfRegularization = d.DateOfRegularization, 
        //                    Tbydeo = d.Tbydeo,
        //                    DateOfCourse = d.DateOfCourse,
        //                    RtmcNo = d.RtmcNo,
        //                    PmdcNo = d.PmdcNo,
        //                    CourseDuration = d.CourseDuration,
        //                    PgSpecialization = d.PgSpecialization,
        //                    Category = d.Category,
        //                    RemunerationStatus = d.RemunerationStatus,
        //                    PgFlag = d.PgFlag,
        //                    CourseName = d.CourseName,
        //                     AddToEmployeePool = d.AddToEmployeePool,
        //                    Domicile_Id = d.Domicile_Id,
        //                    Language_Id = d.Language_Id,
        //                    Designation_Id = d.Designation_Id,
        //                    WDesignation_Id = d.WDesignation_Id,
        //                    Cadre_Id = d.Cadre_Id,
        //                    EmpMode_Id = d.EmpMode_Id,
        //                    HealthFacility_Id = d.HealthFacility_Id,
        //                    Department_Id = d.Department_Id,
        //                    Religion_Id = d.Religion_Id,
        //                    Posttype_Id = d.Posttype_Id,
        //                    HfmisCode = d.HfmisCode ,
        //                    HfmisCodeOld = d.HfmisCodeOld,
        //                    Created_By = d.Created_By,
        //                    Creation_Date = d.Creation_Date,
        //                    IsActive = d.IsActive,
        //                    EntityLifecycle_Id = d.EntityLifecycle_Id,
        //                    Qualification_Id = d.Qualification_Id,
        //                    Status_Id = d.Status_Id,
        //                    Specialization_Id = d.Specialization_Id,
        //                    ProfilePhoto = d.ProfilePhoto,
        //                    WorkingHealthFacility_Id = d.WorkingHealthFacility_Id,
        //                    WorkingHFMISCode = d.WorkingHFMISCode,
        //                    Disability_Id = d.Disability_Id,
        //                    Disability = d.Disability,
        //                    PresentJoiningDate = d.PresentJoiningDate,
        //                    AttachedWith = d.AttachedWith,
        //                    AttachedWith_Id = d.AttachedWith_Id,
        //                    FileNumber = d.FileNumber,
        //                    EmpMode_Name = d.EmployeeName,
        //                    Cadre_Name = d.Cadre_Name,
        //                    Designation_Name = d.Designation_Name,
        //                    Designation_HrScale_Id = d.Designation_HrScale_Id,
        //                    StatusName = d.StatusName,
        //                    WDesignation_Name = d.WDesignation_Name,
        //                    Department_Name = d.Department_Name,
        //                    Division = d.Division,
        //                    District = d.District,
        //                    Tehsil = d.Tehsil,
        //                    HealthFacility = d.HealthFacility,
        //                    WorkingHealthFacility = d.WorkingHealthFacility,
        //                 }
        //                 ).OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).ToList();
        //                 return new TableResponse<ProfileListView> { List = list, Count = count };
        //             }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public bool ChangeSeniorityApplicationStatus(int Id, int StatusId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    var application = _db.HrSeniorityApplications.FirstOrDefault(x => x.Id == Id);
                    if (application != null)
                    {
                        if (StatusId == 2)
                        {
                            application.IsLocked = null;
                        }
                        application.ApplicationStatus_Id = StatusId;
                        application.StatusByUserId = userId;
                        application.StatusByUsername = userName;
                        application.StatusDateTime = DateTime.UtcNow.AddHours(5);
                        _db.Entry(application).State = EntityState.Modified;
                        _db.SaveChanges();



                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public TableResponse<HrReviewSubmissionView> GetProfileReviews(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<HrReviewSubmissionView> query = _db.HrReviewSubmissionViews.Where(x => x.IsActive == true).AsQueryable();
                    if (filters.roleName == "PHFMC Admin")
                    {
                        var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.Id).ToList();
                        query = query.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }
                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
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
                    var list = query.OrderBy(x => x.WDesignation_Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<HrReviewSubmissionView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public TableResponse<ProfileListView> GetProfilesForMobile(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => x.Status_Id != 16).AsQueryable();
                    if (filters.roleName == "PHFMC Admin")
                    {
                        var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.Id).ToList();
                        query = query.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }
                    if (filters.cadres != null && filters.cadres.Count > 0)
                    {
                        query = query.Where(x => filters.cadres.Contains(x.Cadre_Id)).AsQueryable();
                    }
                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.designations.Contains(x.Designation_Id)).AsQueryable();
                    }
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
                    var list = query.OrderBy(x => x.EmployeeName).ToList();
                    return new TableResponse<ProfileListView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DDSView GetFile(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.DDSViews.FirstOrDefault(x => (x.F_CNIC.Equals(cnic) || x.FileNIC.Equals(cnic) || x.FileType.Equals(cnic)) && x.FileType_Id == 1);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
        public List<ApplicationView> GetApplications(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.ApplicationViews.Where(x => x.CNIC.Equals(cnic) && x.IsActive == true).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
        public List<ESRLatestView> GetOrders(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.ESRLatestViews.Where(x => x.CNIC.Equals(cnic) && x.OrderHTML != null && x.elcIsActive == true).OrderByDescending(x => x.Created_Date).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
        public List<LeaveOrderView> GetLeaveOrders(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.LeaveOrderViews.Where(x => x.CNIC.Equals(cnic) && x.OrderHTML != null && x.IsActive == true).OrderByDescending(x => x.DateTime).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
        public bool ConfirmJoining(int id, string joiningDate, string userName, string userId)
        {
            using (var _db = new HR_System())
            {

                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == id);
                try
                {
                    if (profile == null) return false;
                    profile.Status_Id = 2;
                    profile.PresentStationLengthOfService = joiningDate;
                    profile.PresentJoiningDate = Convert.ToDateTime(joiningDate);

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName + " (added after migration)";
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        profile.EntityLifecycle_Id = elc.Id;
                    }
                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile confirmed joining to HF by " + userName;
                    _db.Entity_Modified_Log.Add(eml);
                    _db.SaveChanges();

                    var serviceHistory = _db.HrServiceHistories.OrderByDescending(k => k.Id).FirstOrDefault(x => x.Profile_Id == profile.Id && x.PendingJoining == true);
                    if (serviceHistory != null)
                    {
                        serviceHistory.PendingJoining = false;
                        serviceHistory.From_Date = profile.PresentJoiningDate;
                        serviceHistory.Continued = true;
                        _db.Entry(serviceHistory).State = EntityState.Modified;
                        _db.SaveChanges();

                        Entity_Modified_Log eml2 = new Entity_Modified_Log();
                        eml2.Modified_By = userId;
                        eml2.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml2.Entity_Lifecycle_Id = (long)serviceHistory.EntityLifecycle_Id;
                        eml2.Description = "Service History Modified: Confirm Joining by " + userName;
                        _db.Entity_Modified_Log.Add(eml2);
                        _db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
        public bool NotJoined(int id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {

                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.HrProfiles.FirstOrDefault(x => x.Id == id);
                try
                {
                    if (profile == null) return false;
                    profile.Status_Id = 16;
                    profile.HealthFacility_Id = 11606;
                    profile.WorkingHealthFacility_Id = 11606;
                    profile.HfmisCode = "0350020010030010002";
                    profile.WorkingHFMISCode = "0350020010030010002";
                    //profile.PresentStationLengthOfService = joiningDate;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = userName + " (added after migration)";
                        elc.Users_Id = userId;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        profile.EntityLifecycle_Id = elc.Id;
                    }
                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Employee not confirmed joining to HF by " + userName;
                    _db.Entity_Modified_Log.Add(eml);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
        public bool ProfileExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
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
            if (clientProfile.EmpMode_Id == null) { return null; }
            else { hrProfile.EmpMode_Id = clientProfile.EmpMode_Id; }
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
            hrProfile.PmdcNo = clientProfile.PmdcNo;
            hrProfile.HoD = clientProfile.HoD;
            hrProfile.ModeId = clientProfile.ModeId;
            hrProfile.FirstJoiningDate = clientProfile.FirstJoiningDate;
            hrProfile.PPSCMeritNumber = clientProfile.PPSCMeritNumber;
            hrProfile.PresentPostingOrderNo = clientProfile.PresentPostingOrderNo;
            if (clientProfile.PresentPostingDate != null && clientProfile.PresentPostingDate.Value.Hour == 19) { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate.Value.AddHours(5); }
            else { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate; }
            if (clientProfile.PresentJoiningDate != null && clientProfile.PresentJoiningDate.Value.Hour == 19) { hrProfile.PresentJoiningDate = clientProfile.PresentJoiningDate.Value.AddHours(5); }
            else { hrProfile.PresentJoiningDate = clientProfile.PresentJoiningDate; }

            hrProfile.Qualification_Id = clientProfile.Qualification_Id;
            hrProfile.Specialization_Id = clientProfile.Specialization_Id;
            hrProfile.AdditionalQualification = clientProfile.AdditionalQualification;
            hrProfile.AdditionalCharge = clientProfile.AdditionalCharge;
            hrProfile.AttachedWith = clientProfile.AttachedWith;
            hrProfile.Posttype_Id = clientProfile.Posttype_Id;
            hrProfile.Hfac = clientProfile.Hfac;
            hrProfile.FirstOrderDate = clientProfile.FirstOrderDate;
            hrProfile.FirstOrderNumber = clientProfile.FirstOrderNumber;
            hrProfile.RegularOrderNumber = clientProfile.RegularOrderNumber;
            hrProfile.PromotionOrderNumber = clientProfile.PromotionOrderNumber;
            hrProfile.JoiningDeisgnation_Id = clientProfile.JoiningDeisgnation_Id;
            hrProfile.ContractOrderNumber = clientProfile.ContractOrderNumber;
            hrProfile.ContractOrderDate = clientProfile.ContractOrderDate;
            hrProfile.PeriodofContract = clientProfile.PeriodofContract;
            hrProfile.OtherContract = clientProfile.OtherContract;
            hrProfile.PromotionJoiningDate = clientProfile.PromotionJoiningDate;
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

            hrProfile.PresentStationLengthOfService = clientProfile.PresentStationLengthOfService;
            hrProfile.PrivatePractice = clientProfile.PrivatePractice;
            hrProfile.PermanentAddress = clientProfile.PermanentAddress;
            hrProfile.CorrespondenceAddress = clientProfile.CorrespondenceAddress;
            hrProfile.LandlineNo = clientProfile.LandlineNo;
            hrProfile.Faxno = clientProfile.Faxno;
            hrProfile.EMaiL = clientProfile.EMaiL;

            hrProfile.Disability_Id = clientProfile.Disability_Id;
            hrProfile.Disability = hrProfile.Disability_Id == null ? null : clientProfile.Disability;

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
            if (clientProfile.EmpMode_Id == null) { return null; }
            else { hrProfile.EmpMode_Id = clientProfile.EmpMode_Id; }
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
            hrProfile.AttachedWith = clientProfile.AttachedWith;

            hrProfile.WorkingHealthFacility_Id = clientProfile.WorkingHealthFacility_Id;
            hrProfile.WorkingHFMISCode = clientProfile.WorkingHFMISCode;
            hrProfile.WDesignation_Id = clientProfile.WDesignation_Id;

            hrProfile.SeniorityNo = clientProfile.SeniorityNo;
            hrProfile.PersonnelNo = clientProfile.PersonnelNo;
            hrProfile.PmdcNo = clientProfile.PmdcNo;
            hrProfile.HoD = clientProfile.HoD;
            hrProfile.PresentPostingOrderNo = clientProfile.PresentPostingOrderNo;

            if (clientProfile.PresentPostingDate != null && clientProfile.PresentPostingDate.Value.Hour == 19) { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate.Value.AddHours(5); }
            else { hrProfile.PresentPostingDate = clientProfile.PresentPostingDate; }

            if (clientProfile.PresentJoiningDate != null && clientProfile.PresentJoiningDate.Value.Hour == 19) { hrProfile.PresentJoiningDate = clientProfile.PresentJoiningDate.Value.AddHours(5); }
            else { hrProfile.PresentJoiningDate = clientProfile.PresentJoiningDate; }

            hrProfile.Qualification_Id = clientProfile.Qualification_Id;
            hrProfile.Specialization_Id = clientProfile.Specialization_Id;
            hrProfile.AdditionalQualification = clientProfile.AdditionalQualification;
            hrProfile.AdditionalCharge = clientProfile.AdditionalCharge;
            hrProfile.Posttype_Id = clientProfile.Posttype_Id;
            hrProfile.Hfac = clientProfile.Hfac;
            hrProfile.ModeId = clientProfile.ModeId;
            hrProfile.PPSCMeritNumber = clientProfile.PPSCMeritNumber;
            hrProfile.FirstJoiningDate = clientProfile.FirstJoiningDate;
            hrProfile.FirstOrderDate = clientProfile.FirstOrderDate;
            hrProfile.FirstOrderNumber = clientProfile.FirstOrderNumber;
            hrProfile.RegularOrderNumber = clientProfile.RegularOrderNumber;
            hrProfile.PromotionOrderNumber = clientProfile.PromotionOrderNumber;
            hrProfile.JoiningDeisgnation_Id = clientProfile.JoiningDeisgnation_Id;
            hrProfile.ContractOrderNumber = clientProfile.ContractOrderNumber;
            hrProfile.ContractOrderDate = clientProfile.ContractOrderDate;
            hrProfile.PeriodofContract = clientProfile.PeriodofContract;
            hrProfile.OtherContract = clientProfile.OtherContract;
            hrProfile.PromotionJoiningDate = clientProfile.PromotionJoiningDate;
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

            hrProfile.PresentStationLengthOfService = clientProfile.PresentStationLengthOfService;
            hrProfile.PrivatePractice = clientProfile.PrivatePractice;
            hrProfile.PermanentAddress = clientProfile.PermanentAddress;
            hrProfile.CorrespondenceAddress = clientProfile.CorrespondenceAddress;
            hrProfile.LandlineNo = clientProfile.LandlineNo;
            hrProfile.Faxno = clientProfile.Faxno;
            hrProfile.EMaiL = clientProfile.EMaiL;

            hrProfile.Disability_Id = clientProfile.Disability_Id;
            hrProfile.Disability = hrProfile.Disability_Id == null ? null : clientProfile.Disability;
            return hrProfile;

        }
    }
    public class ProfileFilters : Paginator
    {
        public string hfmisCode { get; set; }
        public string DivisionCode { get; set; }
        public string searchTerm { get; set; }
        public int Id;
        public int CategoryId;
        public int Designation_Id;
        public string Designation;
        public string Category;

        public int ApplicationStatusId;
        public int PostingModeId;
        public bool retirementInOneYear;
        public bool retirementAlerted;
        public List<int?> cadres;
        public List<int?> designations;
        public List<int?> statuses;
        public string value;
        public string roleName { get; set; }
    }
    public class SeniorityApplicantDto
    {
        public string Password { get; set; }
        public ProfileSeniorityView profile { get; set; }
        public HrSeniorityApplicationView application { get; set; }
        public List<HrSeniorityApplicationLog> logs { get; set; }
        public HrSeniorityApplicationView_Fixed seniorityListData { get; set; }
    }
    public class InquiryDtoModel
    {
        public HrInquiry hrInquiry { get; set; }
        public List<HrInquiryPenalty> hrInquiryPenalties { get; set; }
    }
    public class ProfileDesignationSum
    {
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int Count { get; set; }
    }
    public class ProfileQualified
    {
        public List<ProfileDetailsView> ConsultantAnaesthetists { get; set; }
        public List<ProfileDetailsView> Technologists { get; set; }
        public List<ProfileDetailsView> DiplomaMO { get; set; }
        public List<ProfileDetailsView> TrainingMO { get; set; }
        public List<ProfileDetailsView> TrainingWMO { get; set; }
        public List<ProfileDetailsView> DiplomaWMO { get; set; }
        public List<ProfileDetailsView> MO { get; set; }
        public List<ProfileDetailsView> WMO { get; set; }
        public List<ProfileDetailsView> SMO { get; set; }
        public List<ProfileDetailsView> SWMO { get; set; }
        public List<ProfileDetailsView> Persons { get; set; }
        public int CountDiplomaMO { get; set; }
        public int CountTrainingMO { get; set; }
        public int CountTrainingWMO { get; set; }
        public int CountDiplomaWMO { get; set; }
        public int CountOnlyMO { get; set; }
        public int CountOnlyWMO { get; set; }
        public int CountOnlyMOWMO { get; set; }
        public int CountMO { get; set; }
        public int CountWMO { get; set; }
        public int CountSMO { get; set; }
        public int CountSWMO { get; set; }
        public int CountattachedPersons { get; set; }
        public int CountattachedPersonsMO { get; set; }
        public int CountattachedPersonsWMO { get; set; }
        public List<ProfileDesignationSum> ProfileSums { get; set; }
        public List<HrAttachedPersonView> attachedPersons { get; set; }
        public List<HrAttachedPersonView> attachedPersonsMO { get; set; }
        public List<HrAttachedPersonView> attachedPersonsWMO { get; set; }
        public List<HrAttachedPersonView> attachedPersonsMOWMO { get; set; }

    }

    public class Anaesthesia
    {
        public int AttachedPersons { get; set; }
        public int AttachedMOWMO { get; set; }
        public List<ProfileDetailsView> Technologists { get; set; }
        public List<AnaesthesiaTree> anaesthesiaTree { get; set; }
    }
    public class AnaesthesiaTree
    {
        public ProfileDetailsView Anaesthetist { get; set; }
        public int AttachedPersons { get; set; }
        public List<MOAnaesthesiaTree> MOTree { get; set; }
    }
    public class MOAnaesthesiaTree
    {
        public HrAttachedPersonView MOWMOAnaesthesia { get; set; }
        public List<HrAttachedPersonView> MOWMO { get; set; }
    }


}