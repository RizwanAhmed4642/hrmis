using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using Hrmis.Models.ViewModels.HealthFacility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;

namespace Hrmis.Models.Services
{
    public class HealthFacilityService
    {
        public HealthFacility addHF(HFList hf, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (hf.Id == 0)
                        {
                            var savableHF = BindHFSaveModel(hf);
                            if (savableHF == null) return null;
                            Entity_Lifecycle elc = new Entity_Lifecycle();

                            elc.IsActive = true;
                            elc.Created_By = userName;
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Entity_Id = 1;
                            elc.Users_Id = userId;

                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();

                            savableHF.Entity_Lifecycle_Id = elc.Id;

                            _db.HealthFacilities.Add(savableHF);
                            _db.SaveChanges();

                            return savableHF;
                        }
                        else if (HFExists(hf.Id))
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == hf.Id);
                            var editableHF = BindHFEditModel(hf, dbHF);
                            if (editableHF == null) return null;

                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)editableHF.Entity_Lifecycle_Id;
                            eml.Description = "HF Modified By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();

                            _db.Entry(editableHF).State = EntityState.Modified;
                            _db.SaveChanges();

                            return editableHF;
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
        public HFMode addHFMode(HFMode hfMode, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        if (hfMode.Id == 0)
                        {
                            if (hfMode.HF_Id == null || hfMode.HF_Id == 0 || hfMode.Mode_Id == null || hfMode.Mode_Id == 0 || string.IsNullOrEmpty(hfMode.ModeName)) return null;
                            var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == hfMode.HF_Id);

                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)dbHF.Entity_Lifecycle_Id;
                            eml.Description = "HF Mode added By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();

                            _db.HFModes.Add(hfMode);
                            _db.SaveChanges();

                            return hfMode;
                        }
                        else if (hfMode.Id > 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;

                            if (hfMode.HF_Id == null || hfMode.HF_Id == 0 || hfMode.Mode_Id == null || hfMode.Mode_Id == 0 || string.IsNullOrEmpty(hfMode.ModeName)) return null;

                            var dbHFMode = _db.HFModes.FirstOrDefault(x => x.Id == hfMode.Id);
                            if (dbHFMode == null) return null;

                            dbHFMode.HF_Id = hfMode.HF_Id;
                            dbHFMode.Mode_Id = hfMode.Mode_Id;
                            dbHFMode.ModeName = hfMode.ModeName;

                            var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == dbHFMode.HF_Id);
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)dbHF.Entity_Lifecycle_Id;
                            eml.Description = "HF Mode Modified By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();

                            _db.Entry(dbHFMode).State = EntityState.Modified;
                            _db.SaveChanges();

                            return dbHFMode;
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
        public HFUCM addHFUCM(HFUCM hfUc, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;

                        if (hfUc.HF_Id == null || hfUc.HF_Id == 0) return null;

                        var dbUC = _db.HFUCMs.FirstOrDefault(x => x.HF_Id == hfUc.HF_Id);
                        if (dbUC != null)
                        {
                            dbUC.UC_Id = hfUc.UC_Id;
                            dbUC.EPI_Code = hfUc.EPI_Code;

                            dbUC.IsActive = true;
                            dbUC.Datetime = DateTime.UtcNow.AddHours(5);
                            dbUC.UserId = userId;
                            dbUC.Username = userName;

                            _db.Entry(dbUC).State = EntityState.Modified;
                            _db.SaveChanges();
                            return hfUc;
                        }

                        var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == hfUc.HF_Id);

                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)dbHF.Entity_Lifecycle_Id;
                        eml.Description = "HF UC added By " + userName;
                        _db.Entity_Modified_Log.Add(eml);
                        _db.SaveChanges();

                        hfUc.IsActive = true;
                        hfUc.Datetime = eml.Modified_Date;
                        hfUc.UserId = userId;
                        hfUc.Username = userName;

                        _db.HFUCMs.Add(hfUc);
                        _db.SaveChanges();

                        return hfUc;
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

        public int GetHFId(string hfmisCode)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hf = _db.HFLists.FirstOrDefault(x => x.HFMISCode.Equals(hfmisCode) && x.IsActive == true);
                    if (hf != null)
                    {
                        return hf.Id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public HFListP GetHF(int hfId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hf = _db.HFListPs.FirstOrDefault(x => x.Id == hfId && x.IsActive == true);
                    return hf;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public HFService AddHFService(int service_Id, int HF_Id, string hfmisCode, string userId, string userName)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (HFExists(HF_Id))
                    {
                        if (!HFServieExists(HF_Id, service_Id))
                        {
                            HFService hfService = new HFService();
                            hfService.HF_Id = HF_Id;
                            hfService.HfmisCode = hfmisCode;
                            hfService.Services_Id = service_Id;
                            _db.HFServices.Add(hfService);
                            _db.SaveChanges();

                            var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == hfService.HF_Id);

                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)dbHF.Entity_Lifecycle_Id;
                            eml.Description = "HF Service added By " + userName;
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();

                            return hfService;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public HFWardBed AddHFWardBed(HFWardBed hFWardBed, string userId, string userName)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (HFExists(hFWardBed.HF_Id))
                    {
                        if (hFWardBed.Id > 0)
                        {
                            var ward = _db.HFWardBeds.FirstOrDefault(x => x.Id == hFWardBed.Id);
                            if (ward != null)
                            {
                                string editLogMessage = "";
                                if (ward.TotalGB != hFWardBed.TotalGB)
                                {
                                    editLogMessage = "TotalGBBefore:" + ward.TotalGB + ",TotalGBAfter:" + hFWardBed.TotalGB;
                                }
                                if (ward.TotalSB != hFWardBed.TotalSB)
                                {
                                    editLogMessage += "TotalSBBefore:" + ward.TotalSB + ",TotalSBAfter:" + hFWardBed.TotalSB;
                                }
                                if (ward.TotalSanctioned != hFWardBed.TotalSanctioned)
                                {
                                    editLogMessage += "TotalSanctionedBefore:" + ward.TotalSanctioned + ",TotalSanctionedAfter:" + hFWardBed.TotalSanctioned;
                                }
                                if (ward.TotalDonated != hFWardBed.TotalDonated)
                                {
                                    editLogMessage += "TotalDonatedBefore:" + ward.TotalDonated + ",TotalDonatedAfter:" + hFWardBed.TotalDonated;
                                }
                                ward.TotalGB = hFWardBed.TotalGB;
                                ward.TotalSB = hFWardBed.TotalSB;
                                ward.TotalSanctioned = hFWardBed.TotalSanctioned;
                                ward.TotalDonated = hFWardBed.TotalDonated;
                                _db.Entry(ward).State = EntityState.Modified;
                                _db.SaveChanges();

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)ward.EntityLifecycle_Id;
                                eml.Description = "HF Ward & Bed Info added By " + userName + "::" + editLogMessage;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();
                                return ward;
                            }
                        }
                        else
                        {
                            if (!HFWardExists(hFWardBed.HF_Id, hFWardBed.Ward_Id))
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Entity_Id = 847;
                                elc.IsActive = true;
                                elc.Created_By = userName;
                                elc.Users_Id = userId;
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                _db.Entity_Lifecycle.Add(elc);
                                _db.SaveChanges();

                                hFWardBed.EntityLifecycle_Id = elc.Id;
                                _db.HFWardBeds.Add(hFWardBed);
                                _db.SaveChanges();

                                var dbHF = _db.HealthFacilities.FirstOrDefault(x => x.Id == hFWardBed.HF_Id);

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)dbHF.Entity_Lifecycle_Id;
                                eml.Description = "HF Ward & Bed Info added By " + userName;
                                _db.Entity_Modified_Log.Add(eml);
                                _db.SaveChanges();

                                return hFWardBed;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public HFListP GetHF(string hfmisCode)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    return _db.HFListPs.FirstOrDefault(x => x.HFMISCode.Equals(hfmisCode) && x.IsActive == true);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool HFExists(int? Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hf = _db.HFListPs.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                    if (hf == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool HFServieExists(int HF_Id, int service_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hf = _db.HFServices.FirstOrDefault(x => x.Services_Id == service_Id && x.HF_Id == HF_Id);
                    if (hf == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool HFWardExists(int? HF_Id, int? ward_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hfWardBed = _db.HFWardBedsViews.FirstOrDefault(x => x.Ward_Id == ward_Id && x.HF_Id == HF_Id && x.IsActive == true);
                    if (hfWardBed == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool RemoveHealthFacility(int HF_Id, string userhfmisCode, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    HealthFacility hf = _db.HealthFacilities.FirstOrDefault(x => x.Id == HF_Id && x.HfmisCode.StartsWith(userhfmisCode));
                    if (hf == null)
                    {
                        return false;
                    }
                    Entity_Lifecycle elc = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hf.Entity_Lifecycle_Id);
                    if (elc == null) return false;
                    elc.IsActive = false;
                    _db.Entry(elc).State = EntityState.Modified;
                    _db.SaveChanges();

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = elc.Id;
                    eml.Description = "HF Removed By " + userName;
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
        public bool RmvHFWard(int id, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var hFWard = db.HFWards.Find(id);
                        if (hFWard == null)
                        {
                            return false;
                        }
                        var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == hFWard.HF_Id);

                        if (hf.Entity_Lifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 1;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            hf.Entity_Lifecycle_Id = elc.Id;
                        }
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hf.Entity_Lifecycle_Id;
                        eml.Description = "HF Ward removed by " + userName + ":Name=" + hFWard.Name + ",Beds=" + hFWard.NoB;
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();

                        db.HFWards.Remove(hFWard);
                        db.SaveChanges();
                        transc.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public bool RmvHFWardBed(int id, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var hFWard = db.HFWardBeds.Find(id);
                        if (hFWard == null)
                        {
                            return false;
                        }
                        var elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hFWard.EntityLifecycle_Id);
                        if (elc == null)
                        {
                            return false;
                        }
                        elc.IsActive = false;
                        db.Entry(elc).State = EntityState.Modified;
                        db.SaveChanges();
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hFWard.EntityLifecycle_Id;
                        eml.Description = "HF Ward & Bed Info Removed By " + userName;
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();
                        transc.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public bool RmvHFService(int hf_Id, int serviceId, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var hfService = db.HFServices.FirstOrDefault(x => x.HF_Id == hf_Id && x.Services_Id == serviceId);
                        var tempHfService = hfService;
                        if (hfService == null)
                        {
                            return false;
                        }
                        db.HFServices.Remove(hfService);
                        db.SaveChanges();
                        var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == hfService.HF_Id);

                        if (hf.Entity_Lifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 1;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            hf.Entity_Lifecycle_Id = elc.Id;
                        }
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hf.Entity_Lifecycle_Id;
                        eml.Description = "HF Service removed by " + userName + ":Id=" + tempHfService.Services_Id + ":Name=" + tempHfService.Service;
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();
                        transc.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public bool RmvHFVacancy(int id, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var vpMaster = db.VPMasters.Find(id);
                        if (vpMaster == null)
                        {
                            return false;
                        }
                        var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == vpMaster.HF_Id);

                        if (hf.Entity_Lifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = userName + " (added after migration)";
                            elc.Users_Id = userId;
                            elc.IsActive = true;
                            elc.Entity_Id = 1;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            hf.Entity_Lifecycle_Id = elc.Id;
                        }
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = userId;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hf.Entity_Lifecycle_Id;
                        eml.Description = "HF Vacancy removed by " + userName + ":DesignationId=" + vpMaster.Desg_Id + ":PosttypeId=" + vpMaster.PostType_Id + ":S=" + vpMaster.TotalSanctioned + ":F=" + vpMaster.TotalWorking + ":ELC_Id=" + vpMaster.EntityLifecycle_Id;
                        foreach (var vpDetail in db.VPDetails.Where(x => x.Master_Id == vpMaster.Id).ToList())
                        {
                            eml.Description += "Child:" + vpDetail.Id + ":Master_Id =" + vpDetail.Master_Id + ":EPM_MODE_Id=" + vpDetail.EmpMode_Id + ":FILLED=" + vpDetail.TotalWorking;
                            db.VPDetails.Remove(vpDetail);
                        }
                        db.Entity_Modified_Log.Add(eml);
                        db.VPMasters.Remove(vpMaster);
                        db.SaveChanges();
                        transc.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public HFDashboardViewModel GetHFDashboardInfo(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                HFListP healthFacility = null;
                List<HFPhoto> hfPhotos = null;
                HFPhoto hfphoto = null;
                HFUCMView hFUCM = null;
                List<HFWard> hFWards = null;
                List<HFServicesViewModel> hFServices = null;
                List<VpMasterProfileView> hFVacancy = null;
                List<ProfileDetailsView> heads = null;
                ProfileDetailsView hodProfile = null;
                List<ProfileDetailsView> emoloyeeProfiles = null;

                int hf_Id = GetHFId(hfmisCode);
                healthFacility = GetHF(hfmisCode);
                hFUCM = HealthFacility_UCInfo(hf_Id);
                hfPhotos = HealthFacility_Photos(hf_Id);
                hfphoto = hfPhotos.Count > 0 ? hfPhotos.First() : new HFPhoto();
                //hFVacancy = HealthFacility_Vacancy(hf_Id);
                //hFWards = HealthFacility_Wards(hf_Id);
                //hFServices = HealthFacility_Services(hf_Id);
                //hodProfile = HealthFacility_HeadProfile(hfmisCode, hf_Id);
                //heads = HealthFacility_Heads_Profiles(hfmisCode);
                return BindDashboardModel(healthFacility, hfphoto, hfPhotos, hFWards, hFServices, hFVacancy, hodProfile, emoloyeeProfiles, heads, hFUCM);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public HFDashboardViewModel GetHFHOD(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;


                List<ProfileDetailsView> heads = null;
                ProfileDetailsView hodProfile = null;

                int hf_Id = GetHFId(hfmisCode);
                hodProfile = HealthFacility_HeadProfile(hfmisCode, hf_Id);
                heads = HealthFacility_Heads_Profiles(hfmisCode);
                return new HFDashboardViewModel() { HeadOfDepartment = hodProfile, Heads = heads };
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public VpMastProfileView GetHFDesignationVacancy(int hfId, int dsgId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    VpMastProfileView vpMast = _db.VpMastProfileViews.FirstOrDefault(x => x.HF_Id == hfId && x.Desg_Id == dsgId);
                    return vpMast;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public List<VpMastProfileView> GetHFVacancy(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                List<VpMastProfileView> hFVacancy = null;

                int hf_Id = GetHFId(hfmisCode);

                hFVacancy = HealthFacility_Vacancy(hf_Id);

                return hFVacancy;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public HFMode GetHFMode(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                HFMode hFMode = null;

                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    hFMode = HealthFacility_Mode(hf_Id);
                    return hFMode;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public HFUCMView GetHFUCInfo(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                HFUCMView hFUc = null;

                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    hFUc = HealthFacility_UCInfo(hf_Id);
                    return hFUc;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public List<HFWard> GetHFWards(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                List<HFWard> hFWards = null;

                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    hFWards = HealthFacility_Wards(hf_Id);
                    return hFWards;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public List<HFWardBedsView> GetHFWardsBeds(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                List<HFWardBedsView> hFWardBedsViews = null;

                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    hFWardBedsViews = HealthFacility_WardsBeds(hf_Id);
                    return hFWardBedsViews;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public List<HFServicesViewModel> GetHFServices(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                List<HFServicesViewModel> hFServices = null;

                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    hFServices = HealthFacility_Services(hf_Id);
                    return hFServices;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<HFListP> GetCachedHFByHfmisCode(string hfmisCode)
        {
            var listDto = new List<HFListP>();
            if (CachedData.CachedHFListP == null || CachedData.CachedHFListP.Count == 0)
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        _db.Configuration.ProxyCreationEnabled = false;
                        var query = _db.HFListPs.Where(x => x.IsActive == true).AsQueryable();
                        var count = query.Count();
                        var list = query.OrderBy(x => x.OrderBy).ToList();
                        CachedData.CachedHFListP = list;
                        return CachedData.CachedHFListP.Where(x => x.HFMISCode.StartsWith(hfmisCode)).ToList();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            else
            {
                return CachedData.CachedHFListP.Where(x => x.HFMISCode.StartsWith(hfmisCode)).ToList();
            }
        }

        public TableResponse<HFListP> GetHFList(HealthFacilityFilter filter, string userName, string userId, string role)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.HFListPs.Where(x => x.IsActive == true).AsQueryable();


                    //if (role.Equals("PHFMC Admin") || role.Equals("PHFMC"))
                    //{
                    //    query = _db.HFListPs.Where(x => x.IsActive == true && x.HFAC==2).AsQueryable();
                    //}

                    if (role.Equals("Secondary"))
                    {
                        var userHfTypes = new List<string>() { "011", "012", "068" };
                        query = query.Where(x => userHfTypes.Contains(x.HFTypeCode)).AsQueryable();

                        if (userId == "28ea00b0-d34d-4544-9b0a-2284f123bf33")
                        {
                            userHfTypes = new List<string>() {"068"};
                            query = query.Where(x => userHfTypes.Contains(x.HFTypeCode)).AsQueryable();
                        }
                    }
                    else if (role.Equals("Primary"))
                    {
                        var userHfTypes = _db.HFTypes.Where(x => x.HFCat_Id == 3).Select(k => k.Code).ToList();
                        query = query.Where(x => userHfTypes.Contains(x.HFTypeCode)).AsQueryable();
                    }
                    else if (role.Equals("South Punjab"))
                    {
                        query = query.Where(x => Common.Common.southDistricts.Contains(x.DistrictCode)).AsQueryable();
                    }
                    if (role.Equals("PHFMC Admin") || role.Equals("PHFMC"))
                    {
                        query = query.Where(x => x.HFAC == 2).AsQueryable();
                    }
                    if (role.Equals("PACP"))
                    {
                        query = query.Where(x => x.HFTypeCode.Equals("063")).AsQueryable();
                    }
                    query = query.Where(x => x.HFMISCode.StartsWith(filter.HFMISCode)).AsQueryable();
                    if (filter.HFTypes.Count != 0)
                    {
                        query = query.Where(x => filter.HFTypes.Contains(x.HFTypeCode)).AsQueryable();
                    }
                    //if (filter.HFCategories.Count != 0)
                    //{
                    //    var hfTypes = _db.HFTypes.Where(x => filter.HFCategories.Contains(x.Code)).Select(k => k.Code).ToList();
                    //    query = query.Where(x => hfTypes.Contains(x.HFTypeCode)).AsQueryable();
                    //}
                    if (filter.HFACs.Count != 0)
                    {
                        query = query.Where(x => filter.HFACs.Contains(x.HFAC)).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filter.HFStatus) && !filter.HFStatus.Equals("Select Status"))
                    {
                        if (filter.HFStatus.Equals("Functional"))
                        {
                            query = query.Where(x => x.Status.Equals(filter.HFStatus)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => !x.Status.Equals("Functional")).AsQueryable();
                        }
                    }
                    //if (filter.Divisions.Count != 0)
                    //{
                    //    query = _db.HFLists.Where(x => filter.Divisions.Contains(x.DivisionCode)).AsQueryable();
                    //}
                    //if (filter.Districts.Count != 0)
                    //{
                    //    query = _db.HFLists.Where(x => filter.Districts.Contains(x.DistrictCode)).AsQueryable();
                    //}
                    //if (filter.Tehsils.Count != 0)
                    //{
                    //    query = _db.HFLists.Where(x => filter.Tehsils.Contains(x.TehsilCode)).AsQueryable();
                    //}
                    var count = query.Count();
                    var list = query.OrderBy(x => x.OrderBy).Skip(filter.Skip).Take(filter.PageSize).ToList();

                    return new TableResponse<HFListP>() { Count = count, List = list };
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //Health Facility Related Data
        public List<HFPhoto> HealthFacility_Photos(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<HFPhoto> hFPhotos = _db.HFPhotos.Where(x => x.HF_Id == HF_Id).OrderByDescending(x => x.Id).ToList();
                    return hFPhotos;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HFWard> HealthFacility_Wards(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<HFWard> hfWards = _db.HFWards.Where(x => x.HF_Id == HF_Id).ToList();
                    return hfWards;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HFWardBedsView> HealthFacility_WardsBeds(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<HFWardBedsView> hFWardsBeds = _db.HFWardBedsViews.Where(x => x.HF_Id == HF_Id && x.IsActive == true).ToList();
                    return hFWardsBeds;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HFBlock> HealthFacility_Blocks(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<HFBlock> hfBlocks = _db.HFBlocks.Where(x => x.HF_Id == HF_Id).ToList();
                    return hfBlocks;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HFServicesViewModel> HealthFacility_Services(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<HFServicesViewModel> hfservices = _db.HFServices.Where(x => x.HF_Id == HF_Id).Include(x => x.Service).Select(x => new HFServicesViewModel { Name = x.Service.Name, HF_Id = x.HF_Id, HfmisCode = x.HfmisCode, Services_Id = x.Services_Id }).ToList();
                    return hfservices;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<VpMastProfileView> HealthFacility_Vacancy(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    List<VpMastProfileView> hfVacancies = _db.VpMastProfileViews.Where(x => x.HF_Id == HF_Id).OrderByDescending(x => x.BPS).ToList();
                    return hfVacancies;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public HFMode HealthFacility_Mode(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    HFMode hfMode = _db.HFModes.FirstOrDefault(x => x.HF_Id == HF_Id);
                    return hfMode;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public HFUCMView HealthFacility_UCInfo(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    HFUCMView hfUc = _db.HFUCMViews.FirstOrDefault(x => x.HF_Id == HF_Id);
                    return hfUc;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string HFPhoto(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var photo = _db.HFPhotos.OrderByDescending(x => x.Id).FirstOrDefault(x => x.HF_Id == HF_Id);
                    return photo.ImagePath;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ProfileDetailsView HealthFacility_HeadProfile(string hfmisCode, int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ProfileDetailsView hodProfile = _db.ProfileDetailsViews.FirstOrDefault(x => (x.HfmisCode.Equals(hfmisCode) || x.HealthFacility_Id == HF_Id) && (x.HoD == "1") && x.Status_Id != 16);
                    return hodProfile;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<ProfileDetailsView> HealthFacility_Profiles(string hfmisCode, int type, int id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    int HF_Id = GetHFId(hfmisCode);
                    if (type == 1)
                    {
                        emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && x.Status_Id == id && x.Status_Id != 16).OrderByDescending(x => x.CurrentGradeBPS).ThenBy(x => x.EmployeeName)
                    .ToList();
                    }
                    if (type == 2)
                    {
                        emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && x.WDesignation_Id == id && (x.Status_Id == 2 ||
                    x.Status_Id == 3 ||
                    x.Status_Id == 9 ||
                    x.Status_Id == 31 ||
                    x.Status_Id == 15 ||
                    x.Status_Id == 17 ||
                    x.Status_Id == 34 ||
                    x.Status_Id == 38 ||
                    x.Status_Id == 30)).OrderByDescending(x => x.CurrentGradeBPS).ThenBy(x => x.EmployeeName)
                    .ToList();
                    }
                    if (type == 3)
                    {
                        emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && x.EmpMode_Id == id && (x.Status_Id == 2 ||
                    x.Status_Id == 3 ||
                    x.Status_Id == 9 ||
                    x.Status_Id == 31 ||
                    x.Status_Id == 15 ||
                    x.Status_Id == 17 ||
                    x.Status_Id == 38 ||
                    x.Status_Id == 34 ||
                    x.Status_Id == 30)).OrderByDescending(x => x.CurrentGradeBPS).ThenBy(x => x.EmployeeName)
                    .ToList();
                    }
                    return emoloyeeProfiles;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrServiceHistoryView> HealthFacility_ProfilePJHistory(List<int> ids)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var srs = _db.HrServiceHistoryViews.Where(x => ids.Contains((int)x.Profile_Id) && x.PendingJoining == true && x.IsActive == true).ToList();
                    return srs;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<ProfileDetailsView> HealthFacility_Heads_Profiles(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    int HF_Id = GetHFId(hfmisCode);

                    emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && x.HoD == "1" && x.Status_Id != 16).OrderByDescending(x => x.CurrentGradeBPS).ThenBy(x => x.EmployeeName)
                .ToList();
                    return emoloyeeProfiles;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<HFApplication> HealthFacility_Applications(int HF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hfApplications = new List<HFApplication>();
                    var query = _db.ApplicationViews.Where(x => x.ApplicationSource_Id == 3 && x.IsActive == true).AsQueryable();
                    if (HF_Id == 0)
                    {
                        query = query.Where(x => x.ApplicationType_Id == 2).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => x.ApplicationType_Id == 2 && x.ToHF_Id == HF_Id).AsQueryable();
                    }
                    //var profileIds = fAp.ApplicationViews.Where(x => x.Profile_Id > 0).Select(k => k.Profile_Id).ToList();
                    //fAp.ProfileDetailsViews = _db.ProfileDetailsViews.Where(x => profileIds.Contains(x.Id)).OrderBy(o => o.PresentPostingDate).ToList();
                    //fAp.FacilityDistances = new List<FacilityDistance>();
                    //fAp.FacilityVacancyPercentage = new List<FacilityVacancyPercentage>();
                    var applications = query.OrderBy(k => k.Created_Date).ToList();
                    foreach (var app in applications)
                    {
                        var hfApplication = new HFApplication();
                        hfApplication.application = app;
                        hfApplication.profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.Id == app.Profile_Id);
                        //hfApplication.facilityDistance = HealthFacility_Distance(app.HealthFacility_Id, app.ToHF_Id);
                        hfApplication.facilityVacancyPercentage = HealthFacility_CalculateVacancy(app.FromHF_Id, (int)app.ToHF_Id, (int)app.Designation_Id);
                        hfApplications.Add(hfApplication);
                    }
                    return hfApplications;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<HFApplication> GetHFApplications(string hfmisCode = "")
        {
            try
            {
                if (string.IsNullOrEmpty(hfmisCode)) return null;

                List<HFApplication> facilityApplications = null;
                int hf_Id = GetHFId(hfmisCode);
                if (hf_Id != 0)
                {
                    facilityApplications = HealthFacility_Applications(hf_Id);
                }
                return facilityApplications;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public List<FacilityApplicationListModel> ApplicationHealthFacilityDesig(int hfId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.ApplicationViews.Where(x => x.ToHF_Id == hfId && x.ApplicationSource_Id == 3 && x.IsActive == true).AsQueryable();
                    query = query.Where(x => x.ApplicationType_Id == 2).AsQueryable();
                    var hfs = query.GroupBy(x => new { x.Designation_Id, x.designationName }).Select(x => new FacilityApplicationListModel
                    {
                        Id = x.Key.Designation_Id,
                        Name = x.Key.designationName,
                        Count = x.Count()
                    }).OrderByDescending(c => c.Count).ToList();
                    return hfs;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<uspGetHFApps_Result> GetHFApps(int hfId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var d = _db.uspGetHFApps(hfId).ToList();
                    return d;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<FacilityApplicationListModel> ApplicationHealthFacility()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.ApplicationViews.Where(x => x.ApplicationSource_Id == 3 && x.IsActive == true).AsQueryable();
                    query = query.Where(x => x.ApplicationType_Id == 2).AsQueryable();
                    var hfs = query.GroupBy(x => new { x.ToHF_Id, x.toHealthFacility, x.ToHFCode }).Select(x => new FacilityApplicationListModel
                    {
                        Id = x.Key.ToHF_Id,
                        Name = x.Key.toHealthFacility,
                        HfmisCode = x.Key.ToHFCode,
                        Count = x.Count()
                    }).OrderByDescending(c => c.Count).ToList();
                    return hfs;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HFApplication> HealthFacility_Apps(int HF_Id, int DesigId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hfApplications = new List<HFApplication>();
                    VpMastProfileView vpMaster = null;
                    var query = _db.ApplicationViews.Where(x => x.ApplicationSource_Id == 3 && x.IsActive == true).AsQueryable();
                    List<int?> designationIds = new List<int?>();
                    if (DesigId == 0)
                    {
                        designationIds.Add(2404);
                        designationIds.Add(802);
                        designationIds.Add(1320);
                        vpMaster = _db.VpMastProfileViews.FirstOrDefault(x => x.Desg_Id == 2404 && x.HF_Id == HF_Id);
                    }
                    else
                    {
                        designationIds.Add(DesigId);
                        vpMaster = _db.VpMastProfileViews.FirstOrDefault(x => x.Desg_Id == DesigId && x.HF_Id == HF_Id);
                        if (vpMaster == null)
                        {
                            vpMaster = _db.VpMastProfileViews.FirstOrDefault(x => x.Desg_Id == 2404 && x.HF_Id == HF_Id);
                        }
                    }
                    if (DesigId == 2404)
                    {
                        designationIds.Add(802);
                        designationIds.Add(1320);
                    }
                    query = query.Where(x => x.ApplicationType_Id == 2 && x.ToHF_Id == HF_Id && designationIds.Contains(x.Designation_Id)).AsQueryable();
                    //var profileIds = fAp.ApplicationViews.Where(x => x.Profile_Id > 0).Select(k => k.Profile_Id).ToList();
                    //fAp.ProfileDetailsViews = _db.ProfileDetailsViews.Where(x => profileIds.Contains(x.Id)).OrderBy(o => o.PresentPostingDate).ToList();
                    //fAp.FacilityDistances = new List<FacilityDistance>();
                    //fAp.FacilityVacancyPercentage = new List<FacilityVacancyPercentage>();
                    var applications = query.OrderBy(k => k.Created_Date).ToList();
                    foreach (var app in applications)
                    {
                        var hfApplication = new HFApplication();
                        hfApplication.application = app;
                        hfApplication.profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.Id == app.Profile_Id);
                        //hfApplication.facilityDistance = HealthFacility_Distance(app.HealthFacility_Id, app.ToHF_Id);
                        //hfApplication.facilityDistance.HFId = (int) app.HealthFacility_Id;
                        //hfApplication.facilityDistance.HFName = app.HealthFacilityName;
                        hfApplication.facilityDistances = new List<FacilityDistance>();
                        DateTime barDate = DateTime.UtcNow.AddHours(5).AddYears(-3);

                         //&& ((x.From_Date >= barDate) || (x.From_Date < barDate && (x.To_Date > barDate || x.Continued == true)))
                        var serviceHistory = _db.HrServiceHistoryViews.Where(x => x.Profile_Id == app.Profile_Id && x.IsActive == true).OrderByDescending(k => k.From_Date).ToList();
                        if (serviceHistory != null && serviceHistory.Count > 0)
                        {
                            foreach (var item in serviceHistory)
                            {
                                var hfDistance = HealthFacility_Distance(_db, item.HF_Id, app.ToHF_Id, item);
                                //var hfDirection = HealthFacility_Directions(item.HF_Id, app.ToHF_Id);
                                int totalDays = 0;
                                int leaveDays = 0;

                                if (item.From_Date != null)
                                {
                                    DateTime fromDate = (DateTime)item.From_Date;
                                    if (item.From_Date < barDate)
                                    {
                                        fromDate = barDate;
                                    }
                                    var leaves = _db.HrLeaveRecordViews.Where(x => x.Profile_Id == item.Profile_Id && x.FromDate >= fromDate && x.ToDate <= item.To_Date && x.IsActive == true).ToList();
                                    if (leaves.Count > 0)
                                    {
                                        leaveDays = leaves.Sum(x => x.TotalDays).Value;
                                    }
                                    if (item.Continued == true)
                                    {
                                        totalDays = Convert.ToDateTime(DateTime.UtcNow.AddHours(5)).Subtract(fromDate).Days;
                                    }
                                    else if (item.To_Date != null)
                                    {
                                        totalDays = Convert.ToDateTime(item.To_Date).Subtract(fromDate).Days;
                                    }
                                }
                                hfDistance.ServiceHistory = item;
                                //hfDirection.ServiceHistory = item;
                                int activeServiceDays = totalDays - leaveDays;
                                hfDistance.TotalDays = activeServiceDays >= 0 ? activeServiceDays : 0;
                                //hfDirection.TotalDays = activeServiceDays >= 0 ? activeServiceDays : 0;
                                hfApplication.facilityDistances.Add(hfDistance);
                                //hfApplication.facilityDirections.Add(hfDirection);
                            }
                        }
                        hfApplication.winner = new WinnerModel()
                        {
                            Adhoc = vpMaster.Adhoc,
                            Filled = vpMaster.TotalWorking,
                            Regular = vpMaster.Regular,
                            Vacant = vpMaster.Vacant,
                            AVacant = vpMaster.Vacant + vpMaster.Adhoc
                        };
                        hfApplication.facilityVacancyPercentage = HealthFacility_CalculateVacancy(app.FromHF_Id, (int)app.ToHF_Id, (int)app.Designation_Id);
                        hfApplications.Add(hfApplication);
                    }
                    return hfApplications;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FacilityDistance HealthFacility_Distance(HR_System _db, int? HF_From_Id, int? ToHF_Id, HrServiceHistoryView item)
        {
            try
            {
                _db.Configuration.ProxyCreationEnabled = false;
                #region Properties
                List<HFDistance> hFDistances = new List<HFDistance>();
                HFDistance hfDistanceNew;
                var facilityDist = new FacilityDistance();
                facilityDist.minimumDistances = new List<MinimumDistanceCity>();
                facilityDist.ServiceHistory = item;
                bool prefDist = false, p2p = false, nDhq = false, nThq = false, lhr = false,
                    mltn = false,
                    ryk = false,
                    rwp = false,
                    fsd = false,
                    gjr = false,
                    srg = false,
                    sahiwal = false,
                    bwp = false,
                    dgk = false,
                    sialkot = false;
                int distancesAvailable = 0;
                #endregion

                var hfFrom = _db.HFListPs.FirstOrDefault(x => x.Id == HF_From_Id);
                if (hfFrom != null)
                {
                    if (hfFrom.Latitude != null && hfFrom.Longitude != null)
                    {
                        var hfTo = _db.HFListPs.FirstOrDefault(x => x.Id == ToHF_Id);

                        #region AddOrigin
                        string origin = hfFrom.Latitude + "," + hfFrom.Longitude;
                        string destinations = "";
                        #endregion

                        #region Near DHQ Bird Eye Distance
                        var dhqs = _db.HFListPs.Where(x => x.HFTypeCode.Equals("011") && x.IsActive == true);
                        List<DistanceAndId> dhqsDistanceAndIds = new List<DistanceAndId>();
                        foreach (var dhq in dhqs)
                        {
                            if (dhq.Latitude != null && dhq.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hfFrom.Latitude),
                                Convert.ToDouble(hfFrom.Longitude),
                                Convert.ToDouble(dhq.Latitude),
                                Convert.ToDouble(dhq.Longitude));
                                if (distance >= 0)
                                {
                                    dhqsDistanceAndIds.Add(new DistanceAndId() { Id = dhq.Id, Distance = distance, FacilityName = dhq.FullName });
                                }
                            }
                        }
                        dhqsDistanceAndIds = dhqsDistanceAndIds.OrderBy(x => x.Distance).ToList();
                        HFListP newrestDHQ = null;
                        if (dhqsDistanceAndIds.Count > 0)
                        {
                            int dhqsDistanceAndId = dhqsDistanceAndIds[0].Id;
                            newrestDHQ = _db.HFListPs.FirstOrDefault(x => x.Id == dhqsDistanceAndId);
                            if (newrestDHQ.DistrictCode != hfFrom.DistrictCode)
                            {
                                facilityDist.FromDHQName = newrestDHQ.DistrictName;
                            }
                        }

                        #endregion

                        #region Near THQ Bird Eye Distance
                        
                        var thqs = _db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfFrom.DistrictCode) && x.HFTypeCode.Equals("012") && x.IsActive == true);
                        List<DistanceAndId> thqsDistanceAndIds = new List<DistanceAndId>();
                        foreach (var thq in thqs)
                        {
                            if (thq.Latitude != null && thq.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hfFrom.Latitude),
                                Convert.ToDouble(hfFrom.Longitude),
                                Convert.ToDouble(thq.Latitude),
                                Convert.ToDouble(thq.Longitude));
                                if (distance >= 0)
                                {
                                    thqsDistanceAndIds.Add(new DistanceAndId() { Id = thq.Id, Distance = distance, FacilityName = thq.FullName });
                                }
                            }
                        }
                        thqsDistanceAndIds = thqsDistanceAndIds.OrderBy(x => x.Distance).ToList();
                        HFListP newrestTHQ = null;
                        if (thqsDistanceAndIds.Count > 0)
                        {
                            int thqsDistanceAndId = thqsDistanceAndIds[0].Id;
                            newrestTHQ = _db.HFListPs.FirstOrDefault(x => x.Id == thqsDistanceAndId);
                            if (newrestTHQ.Id != hfFrom.Id)
                            {
                                facilityDist.FromTHQName = newrestTHQ.TehsilName;
                            }
                        }

                        #endregion

                        #region P2P
                        if (hfTo != null)
                        {
                            if (hfTo.Latitude != null && hfTo.Longitude != null)
                            {
                                var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == hfTo.Id);
                                //if (hfDistance == null)
                                //{
                                //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfTo.Id && x.ToHF_Id == hfFrom.Id);
                                //}
                                if (hfDistance == null)
                                {
                                    hfDistanceNew = new HFDistance();
                                    hfDistanceNew.FromHF_Id = hfFrom.Id;
                                    hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                    hfDistanceNew.OriginName = hfFrom.FullName;
                                    hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                    hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                    hfDistanceNew.DestinationName = hfTo.FullName;
                                    hfDistanceNew.ToHF_Id = hfTo.Id;
                                    hfDistanceNew.ToHFMISCode = hfTo.HFMISCode;
                                    hfDistanceNew.DestinationLatitude = hfTo.Latitude;
                                    hfDistanceNew.DestinationLongitude = hfTo.Longitude;
                                    hFDistances.Add(hfDistanceNew);
                                    destinations += hfTo.Latitude + "%2C" + hfTo.Longitude + "%7C";
                                    p2p = true;
                                }
                                else
                                {
                                    facilityDist.ByRoad = hfDistance.DistanceText;
                                    facilityDist.ByRoadValue = (float)hfDistance.DistanceValue;
                                    facilityDist.ByRoadTime = hfDistance.DurationText;
                                    facilityDist.ByRoadValueTime = (float)hfDistance.DurationValue;
                                }
                            }
                            else
                            {
                                facilityDist.Error += "To Health Facility Missing Coordinates: " + hfTo.FullName + "/n";
                            }
                        }
                        else
                        {
                            facilityDist.Error += "To Health Facility Missing/n";
                        }

                        #endregion

                        #region Near DHQ Road Distance
                        if (newrestDHQ != null)
                        {
                            if (newrestDHQ.Latitude != null && newrestDHQ.Longitude != null)
                            {
                                var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == newrestDHQ.Id);
                                //if (hfDistance == null)
                                //{
                                //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == newrestDHQ.Id && x.ToHF_Id == hfFrom.Id);
                                //}
                                if (hfDistance == null)
                                {
                                    hfDistanceNew = new HFDistance();
                                    hfDistanceNew.FromHF_Id = hfFrom.Id;
                                    hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                    hfDistanceNew.OriginName = hfFrom.FullName;
                                    hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                    hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                    hfDistanceNew.ToHF_Id = newrestDHQ.Id;
                                    hfDistanceNew.ToHFMISCode = newrestDHQ.HFMISCode;
                                    hfDistanceNew.DestinationName = newrestDHQ.FullName;
                                    hfDistanceNew.DestinationLatitude = newrestDHQ.Latitude;
                                    hfDistanceNew.DestinationLongitude = newrestDHQ.Longitude;
                                    hFDistances.Add(hfDistanceNew);
                                    destinations += newrestDHQ.Latitude + "%2C" + newrestDHQ.Longitude + "%7C";
                                    nDhq = true;
                                }
                                else
                                {
                                    facilityDist.FromDHQ = hfDistance.DistanceText;
                                    facilityDist.FromDHQValue = (float)hfDistance.DistanceValue;
                                    facilityDist.FromDHQTime = hfDistance.DurationText;
                                    facilityDist.FromDHQValueTime = (float)hfDistance.DurationValue;
                                }
                            }
                            else
                            {
                                facilityDist.Error += "Nearest DHQ Missing Coordinates: " + newrestDHQ.FullName + "/n";
                            }
                        }
                        else
                        {
                            facilityDist.Error += "Nearest DHQ Missing/n";
                        }
                        #endregion

                        #region Near THQ Distance
                        if (newrestTHQ != null)
                        {
                            if (newrestTHQ.Latitude != null && newrestTHQ.Longitude != null)
                            {
                                var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == newrestTHQ.Id);
                                //if (hfDistance == null)
                                //{
                                //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == newrestTHQ.Id && x.ToHF_Id == hfFrom.Id);
                                //}
                                if (hfDistance == null)
                                {
                                    hfDistanceNew = new HFDistance();
                                    hfDistanceNew.DistanceTo = "THQ";
                                    hfDistanceNew.FromHF_Id = hfFrom.Id;
                                    hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                    hfDistanceNew.OriginName = hfFrom.FullName;
                                    hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                    hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                    hfDistanceNew.ToHF_Id = newrestTHQ.Id;
                                    hfDistanceNew.ToHFMISCode = newrestTHQ.HFMISCode;
                                    hfDistanceNew.DestinationName = newrestTHQ.FullName;
                                    hfDistanceNew.DestinationLatitude = newrestTHQ.Latitude;
                                    hfDistanceNew.DestinationLongitude = newrestTHQ.Longitude;
                                    hFDistances.Add(hfDistanceNew);
                                    destinations += newrestTHQ.Latitude + "%2C" + newrestTHQ.Longitude + "%7C";
                                    nThq = true;
                                }
                                else
                                {
                                    facilityDist.FromTHQ = hfDistance.DistanceText;
                                    facilityDist.FromTHQValue = (float)hfDistance.DistanceValue;
                                    facilityDist.FromTHQTime = hfDistance.DurationText;
                                    facilityDist.FromTHQValueTime = (float)hfDistance.DurationValue;
                                }
                            }
                            else
                            {
                                facilityDist.Error += "Nearest THQ Missing Coordinates: " + newrestTHQ.FullName + "/n";
                            }
                        }
                        else
                        {
                            facilityDist.Error += "Nearest THQ Missing/n";
                        }

                        #endregion

                        #region From Lahore
                        //Lahore City
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("035002001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "035002001";
                            hfDistanceNew.DestinationName = "Lahore City";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "31.5204%2C74.3587" + "%7C";
                            lhr = true;
                        }
                        else
                        {
                            facilityDist.FromLHR = hfDistanceNew.DistanceText;
                            facilityDist.FromLHRValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.FromLHRTime = hfDistanceNew.DurationText;
                            facilityDist.FromLHRValueTime = (float)hfDistanceNew.DurationValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Lahore City",
                                lowestLabel = facilityDist.FromLHR,
                                lowestValue = facilityDist.FromLHRValue,
                                lowestDurationLabel = facilityDist.FromLHRTime,
                                lowestDurationValue = facilityDist.FromLHRValueTime
                            });
                        }
                        //destinations += "31.5204%2C74.3587" + "%7C";

                        #endregion

                        #region MainCity
                        #region Multan
                        //Multan City
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("036001007"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "036001007";
                            hfDistanceNew.DestinationName = "Multan City";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "30.1756%2C71.4708" + "%7C";
                            mltn = true;
                        }
                        else
                        {
                            facilityDist.FromMultan = hfDistanceNew.DistanceText;
                            facilityDist.FromMultanValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Multan City",
                                lowestLabel = facilityDist.FromMultan,
                                lowestValue = facilityDist.FromMultanValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        //string multanLatLong = "30.1756%2C71.4708";

                        #endregion

                        #region Rawalpindi
                        //Rawalpindi
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("037003005"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "037003005";
                            hfDistanceNew.DestinationName = "Rawalpindi";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "33.5894%2C73.0664" + "%7C";
                            rwp = true;
                        }
                        else
                        {
                            facilityDist.FromRawalpindi = hfDistanceNew.DistanceText;
                            facilityDist.FromRawalpindiValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Rawalpindi",
                                lowestLabel = facilityDist.FromRawalpindi,
                                lowestValue = facilityDist.FromRawalpindiValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        //string rawalpindiLatLong = "33.5894%2C73.0664";

                        #endregion

                        #region Rahim Yar Khan
                        //Rahim Yar Khan
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("031003001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "031003001";
                            hfDistanceNew.DestinationName = "Rahim Yar Khan";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "28.4193%2C70.3139" + "%7C";
                            ryk = true;
                        }
                        else
                        {
                            facilityDist.FromRahimYarKhan = hfDistanceNew.DistanceText;
                            facilityDist.FromRahimYarKhanValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Rahim Yar Khan",
                                lowestLabel = facilityDist.FromRahimYarKhan,
                                lowestValue = facilityDist.FromRahimYarKhanValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        //string rahimYarKhanLatLong = "28.4193%2C70.3139";

                        #endregion

                        #region Faisalabad
                        //Faisalabad City
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("033001001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "033001001";
                            hfDistanceNew.DestinationName = "Faisalabad City";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "31.4251%2C73.0893" + "%7C";
                            fsd = true;
                        }
                        else
                        {
                            facilityDist.FromFaisalabad = hfDistanceNew.DistanceText;
                            facilityDist.FromFaisalabadValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Faisalabad City",
                                lowestLabel = facilityDist.FromFaisalabad,
                                lowestValue = facilityDist.FromFaisalabadValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        //string faisalabadLatLong = "31.4251%2C73.0893";

                        #endregion

                        #region Gujranwala
                        //Gujranwala
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("034001001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "034001001";
                            hfDistanceNew.DestinationName = "Gujranwala";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "32.1481%2C74.183" + "%7C";
                            gjr = true;
                        }
                        else
                        {
                            facilityDist.FromGujranwala = hfDistanceNew.DistanceText;
                            facilityDist.FromGujranwalaValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Gujranwala",
                                lowestLabel = facilityDist.FromGujranwala,
                                lowestValue = facilityDist.FromGujranwalaValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        //string gujranwalaLatLong = "32.1481%2C74.183";

                        #endregion

                        #region Bahawalpur
                        //Bahawalpur
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("031002001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "031002001";
                            hfDistanceNew.DestinationName = "Bahawalpur City";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "29.4117%2C71.6938" + "%7C";
                            bwp = true;
                        }
                        else
                        {
                            facilityDist.FromBahawalpur = hfDistanceNew.DistanceText;
                            facilityDist.FromBahawalpurValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Bahawalpur City",
                                lowestLabel = facilityDist.FromBahawalpur,
                                lowestValue = facilityDist.FromBahawalpurValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        #endregion

                        #region DeraGhaziKhan
                        //DeraGhaziKhan
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("032001001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "032001001";
                            hfDistanceNew.DestinationName = "Dera Ghazi Khan";
                            hfDistanceNew.DestinationLatitude = 31.5204;
                            hfDistanceNew.DestinationLongitude = 74.3587;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "30.0491%2C70.6389" + "%7C";
                            dgk = true;
                        }
                        else
                        {
                            facilityDist.FromDeraGhaziKhan = hfDistanceNew.DistanceText;
                            facilityDist.FromDeraGhaziKhanValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Dera Ghazi Khan",
                                lowestLabel = facilityDist.FromDeraGhaziKhan,
                                lowestValue = facilityDist.FromDeraGhaziKhanValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        #endregion

                        #region Sargodha
                        //Sargodha
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("038004001"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "038004001";
                            hfDistanceNew.DestinationName = "Sargodha";
                            hfDistanceNew.DestinationLatitude = 32.0676;
                            hfDistanceNew.DestinationLongitude = 72.6796;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "32.0676% 2C72.6796" + "%7C";
                            srg = true;
                        }
                        else
                        {
                            facilityDist.FromSargodha = hfDistanceNew.DistanceText;
                            facilityDist.FromSargodhaValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Sargodha",
                                lowestLabel = facilityDist.FromSargodha,
                                lowestValue = facilityDist.FromSargodhaValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        #endregion

                        #region Sahiwal
                        //Sahiwal
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("038004003"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "038004003";
                            hfDistanceNew.DestinationName = "Sahiwal";
                            hfDistanceNew.DestinationLatitude = 30.6562;
                            hfDistanceNew.DestinationLongitude = 73.0872;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "30.6562% 2C73.0872" + "%7C";
                            sahiwal = true;
                        }
                        else
                        {
                            facilityDist.FromSahiwal = hfDistanceNew.DistanceText;
                            facilityDist.FromSahiwalValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Sahiwal",
                                lowestLabel = facilityDist.FromSahiwal,
                                lowestValue = facilityDist.FromSahiwalValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        #endregion

                        #region Sialkot
                        //Sialkot
                        hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("034003002"));
                        if (hfDistanceNew == null)
                        {
                            hfDistanceNew = new HFDistance();
                            hfDistanceNew.FromHF_Id = hfFrom.Id;
                            hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                            hfDistanceNew.OriginName = hfFrom.FullName;
                            hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                            hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                            hfDistanceNew.ToHFMISCode = "034003002";
                            hfDistanceNew.DestinationName = "Sialkot";
                            hfDistanceNew.DestinationLatitude = 32.4996;
                            hfDistanceNew.DestinationLongitude = 74.5346;
                            hFDistances.Add(hfDistanceNew);
                            destinations += "32.4996% 2C74.5346" + "%7C";
                            sialkot = true;
                        }
                        else
                        {
                            facilityDist.FromSialkot = hfDistanceNew.DistanceText;
                            facilityDist.FromSialkotValue = (float)hfDistanceNew.DistanceValue;
                            facilityDist.minimumDistances.Add(new MinimumDistanceCity
                            {
                                cityName = "Sialkot",
                                lowestLabel = facilityDist.FromSialkot,
                                lowestValue = facilityDist.FromSialkotValue,
                                lowestDurationLabel = hfDistanceNew.DurationText,
                                lowestDurationValue = (float)hfDistanceNew.DurationValue
                            });
                        }
                        #endregion

                        #endregion

                        #region Preferred District
                        if (hfTo != null)
                        {
                            var preferredDistrict = _db.DistrictLatLongs.FirstOrDefault(x => x.Code == hfTo.DistrictCode);
                            if (preferredDistrict != null && preferredDistrict.Latitude != null && preferredDistrict.Longitude != null)
                            {
                                var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHFMISCode == hfTo.DistrictCode);
                                if (hfDistance == null)
                                {
                                    hfDistanceNew = new HFDistance();
                                    hfDistanceNew.FromHF_Id = hfFrom.Id;
                                    hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                    hfDistanceNew.OriginName = hfFrom.FullName;
                                    hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                    hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                    hfDistanceNew.DestinationName = preferredDistrict.Name;
                                    hfDistanceNew.ToHFMISCode = preferredDistrict.Code;
                                    hfDistanceNew.DestinationLatitude = preferredDistrict.Latitude;
                                    hfDistanceNew.DestinationLongitude = preferredDistrict.Longitude;
                                    hFDistances.Add(hfDistanceNew);
                                    destinations += preferredDistrict.Latitude + "%2C" + preferredDistrict.Longitude + "%7C";
                                    prefDist = true;
                                }
                                else
                                {
                                    facilityDist.PreferredDistrict = hfDistance.DistanceText;
                                    facilityDist.PreferredDistrictValue = (float)hfDistance.DistanceValue;
                                    facilityDist.PreferredDistrictTime = hfDistance.DurationText;
                                    facilityDist.PreferredDistrictTimeValue = (float)hfDistance.DurationValue;
                                }
                            }
                        }
                        #endregion

                        #region Api Process
                        if (!string.IsNullOrEmpty(destinations))
                        {
                            if (destinations.EndsWith("%7C"))
                            {
                                destinations = destinations.Substring(0, destinations.Length - 3);
                            }
                            RootObject result = new RootObject();
                            string url = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins=" + origin + "&destinations=" + destinations + "&key=AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI";///AIzaSyCKn6GNT8nJhIARmSWbiOqvvUxtsziZjzc";

                            WebRequest request = WebRequest.Create(url);
                            request.Credentials = CredentialCache.DefaultCredentials;

                            WebResponse response = request.GetResponse();
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(dataStream);
                                string responseFromServer = reader.ReadToEnd();
                                result = JsonConvert.DeserializeObject<RootObject>(responseFromServer);

                                //for (int i = 0; i < hFDistances.Count; i++)
                                //{
                                //    var hfDistance = hFDistances[i];
                                //    var dst = result.rows[0].elements[i];
                                //    var name = result.destination_addresses[i];
                                //}
                                #region Response
                                if (result.rows.Count > 0)
                                {
                                    #region P2P
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && p2p == true)
                                    {
                                        facilityDist.ByRoad = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.ByRoadValue = result.rows[0].elements[distancesAvailable].distance.value;

                                        facilityDist.ByRoadTime = result.rows[0].elements[distancesAvailable].duration.text;
                                        facilityDist.ByRoadValueTime = result.rows[0].elements[distancesAvailable].duration.value;

                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == ToHF_Id);
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.ByRoad;
                                            hfDistance.DistanceValue = facilityDist.ByRoadValue;
                                            hfDistance.DurationText = facilityDist.ByRoadTime;
                                            hfDistance.DurationValue = facilityDist.ByRoadValueTime;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Near DHQ
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && nDhq == true)
                                    {
                                        facilityDist.FromDHQ = result.rows[0].elements[1].distance.text;
                                        facilityDist.FromDHQValue = result.rows[0].elements[distancesAvailable].distance.value;

                                        facilityDist.FromDHQTime = result.rows[0].elements[distancesAvailable].duration.text;
                                        facilityDist.FromDHQValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == newrestDHQ.Id);
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromDHQ;
                                            hfDistance.DistanceValue = facilityDist.FromDHQValue;
                                            hfDistance.DurationText = facilityDist.FromDHQTime;
                                            hfDistance.DurationValue = facilityDist.FromDHQValueTime;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Near THQ
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && nThq == true)
                                    {
                                        facilityDist.FromTHQ = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromTHQValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.FromTHQTime = result.rows[0].elements[distancesAvailable].duration.text;
                                        facilityDist.FromTHQValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == newrestTHQ.Id);
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromTHQ;
                                            hfDistance.DistanceValue = facilityDist.FromTHQValue;
                                            hfDistance.DurationText = facilityDist.FromTHQTime;
                                            hfDistance.DurationValue = facilityDist.FromTHQValueTime;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region From Lahore
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && lhr == true)
                                    {
                                        facilityDist.FromLHR = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromLHRValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.FromLHRTime = result.rows[0].elements[distancesAvailable].duration.text;
                                        facilityDist.FromLHRValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Lahore",
                                            lowestLabel = facilityDist.FromLHR,
                                            lowestValue = facilityDist.FromLHRValue,
                                            lowestDurationLabel = facilityDist.FromLHRTime,
                                            lowestDurationValue = facilityDist.FromLHRValueTime
                                        });
                                        distancesAvailable++;
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "035002001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromLHR;
                                            hfDistance.DistanceValue = facilityDist.FromLHRValue;
                                            hfDistance.DurationText = facilityDist.FromLHRTime;
                                            hfDistance.DurationValue = facilityDist.FromLHRValueTime;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                    }
                                    #endregion

                                    #region MainCity

                                    #region Multan
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && mltn == true)
                                    {
                                        facilityDist.FromMultan = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromMultanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Multan",
                                            lowestLabel = facilityDist.FromMultan,
                                            lowestValue = facilityDist.FromMultanValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "036001007");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromMultan;
                                            hfDistance.DistanceValue = facilityDist.FromMultanValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Rawalpindi
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && rwp == true)
                                    {
                                        facilityDist.FromRawalpindi = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromRawalpindiValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Rawalpindi",
                                            lowestLabel = facilityDist.FromRawalpindi,
                                            lowestValue = facilityDist.FromRawalpindiValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "037003005");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromRawalpindi;
                                            hfDistance.DistanceValue = facilityDist.FromRawalpindiValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region RahimYarKhan
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && ryk == true)
                                    {
                                        facilityDist.FromRahimYarKhan = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromRahimYarKhanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "RahimYarKhan",
                                            lowestLabel = facilityDist.FromRahimYarKhan,
                                            lowestValue = facilityDist.FromRahimYarKhanValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "031003001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromRahimYarKhan;
                                            hfDistance.DistanceValue = facilityDist.FromRahimYarKhanValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }

                                    #endregion

                                    #region Faisalabad
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && fsd == true)
                                    {
                                        facilityDist.FromFaisalabad = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromFaisalabadValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Faisalabad",
                                            lowestLabel = facilityDist.FromFaisalabad,
                                            lowestValue = facilityDist.FromFaisalabadValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "033001001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromFaisalabad;
                                            hfDistance.DistanceValue = facilityDist.FromFaisalabadValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }

                                    #endregion

                                    #region Gujranwala
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && gjr == true)
                                    {
                                        facilityDist.FromGujranwala = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromGujranwalaValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Gujranwala",
                                            lowestLabel = facilityDist.FromGujranwala,
                                            lowestValue = facilityDist.FromGujranwalaValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "034001001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromGujranwala;
                                            hfDistance.DistanceValue = facilityDist.FromGujranwalaValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Bahawalpur
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && bwp == true)
                                    {
                                        facilityDist.FromBahawalpur = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromBahawalpurValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Bahawalpur",
                                            lowestLabel = facilityDist.FromBahawalpur,
                                            lowestValue = facilityDist.FromBahawalpurValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "031002001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromBahawalpur;
                                            hfDistance.DistanceValue = facilityDist.FromBahawalpurValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Dera Ghazi Khan
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && dgk == true)
                                    {
                                        facilityDist.FromDeraGhaziKhan = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromDeraGhaziKhanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Bahawalpur",
                                            lowestLabel = facilityDist.FromDeraGhaziKhan,
                                            lowestValue = facilityDist.FromDeraGhaziKhanValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "032001001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromDeraGhaziKhan;
                                            hfDistance.DistanceValue = facilityDist.FromDeraGhaziKhanValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Sargodha
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && srg == true)
                                    {
                                        facilityDist.FromSargodha = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromSargodhaValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Sargodha",
                                            lowestLabel = facilityDist.FromSargodha,
                                            lowestValue = facilityDist.FromSargodhaValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "038004001");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromSargodha;
                                            hfDistance.DistanceValue = facilityDist.FromSargodhaValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Sahiwal
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && sahiwal == true)
                                    {
                                        facilityDist.FromSahiwal = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromSahiwalValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Sahiwal",
                                            lowestLabel = facilityDist.FromSahiwal,
                                            lowestValue = facilityDist.FromSahiwalValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "038004003");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromSahiwal;
                                            hfDistance.DistanceValue = facilityDist.FromSahiwalValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #region Sialkot
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && sialkot == true)
                                    {
                                        facilityDist.FromSialkot = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.FromSialkotValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                        {
                                            cityName = "Sialkot",
                                            lowestLabel = facilityDist.FromSialkot,
                                            lowestValue = facilityDist.FromSialkotValue,
                                            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                        });
                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "034003002");
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.FromSialkot;
                                            hfDistance.DistanceValue = facilityDist.FromSialkotValue;
                                            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion

                                    #endregion

                                    #region Preferred District
                                    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && prefDist == true)
                                    {
                                        facilityDist.PreferredDistrict = result.rows[0].elements[distancesAvailable].distance.text;
                                        facilityDist.PreferredDistrictValue = result.rows[0].elements[distancesAvailable].distance.value;
                                        facilityDist.PreferredDistrictTime = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                        facilityDist.PreferredDistrictTimeValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;

                                        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == hfTo.DistrictCode);
                                        if (hfDistance != null)
                                        {
                                            hfDistance.DistanceText = facilityDist.PreferredDistrict;
                                            hfDistance.DistanceValue = facilityDist.PreferredDistrictValue;
                                            hfDistance.DurationText = facilityDist.PreferredDistrictTime;
                                            hfDistance.DurationValue = facilityDist.PreferredDistrictTimeValue;
                                            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                            _db.HFDistances.Add(hfDistance);
                                            _db.SaveChanges();
                                        }
                                        distancesAvailable++;
                                    }
                                    #endregion
                                }

                                #endregion

                            }
                            response.Close();
                        }

                        #endregion

                        //destinations += multanLatLong + "%7C" + rawalpindiLatLong + "%7C"
                        //     + rahimYarKhanLatLong + "%7C" + faisalabadLatLong + "%7C" + gujranwalaLatLong;

                        //var city = _db.Distr.FirstOrDefault(x => x.HFMISCode.StartsWith(hfFrom.DistrictCode) && x.HFTypeCode.Equals("012") && x.IsActive == true);
                        //if (city != null && city.Latitude != null)
                        //{
                        //    facilityDist.FromCity = CalculateDistance(newrestTHQ.Latitude, newrestTHQ.Longitude, hfFrom.Latitude, hfFrom.Longitude);
                        //}

                        facilityDist.HFId = hfFrom.Id;
                        facilityDist.HFName = hfFrom.FullName;

                        if (hfFrom.HFTypeCode.Equals("011"))
                        {
                            facilityDist.FromDHQ = "0";
                            facilityDist.FromDHQValue = 0;
                            facilityDist.FromDHQName = "";
                        }

                        if (hfFrom.HFTypeCode.Equals("012"))
                        {
                            facilityDist.FromTHQ = "0";
                            facilityDist.FromTHQValue = 0;
                            facilityDist.FromTHQName = "";
                        }
                        if (hfFrom.HFTypeCode.Equals("068"))
                        {
                            facilityDist.FromTHQ = "0";
                            facilityDist.FromTHQValue = 0;
                            facilityDist.FromTHQName = "";
                        }
                        facilityDist.minimumDistance = facilityDist.minimumDistances.OrderBy(x => x.lowestValue).FirstOrDefault();
                        if (facilityDist.minimumDistance != null)
                        {
                            if (facilityDist.minimumDistance.cityName.Equals(hfFrom.TehsilName))
                            {
                                facilityDist.FromCity = "0";
                                facilityDist.FromCityValue = 0;
                                facilityDist.FromCityTime = "0";
                                facilityDist.FromCityValueTime = 0;
                                facilityDist.FromCityName = facilityDist.minimumDistance.cityName;
                            }
                            else
                            {
                                facilityDist.FromCity = facilityDist.minimumDistance.lowestLabel;
                                facilityDist.FromCityValue = facilityDist.minimumDistance.lowestValue;
                                facilityDist.FromCityTime = facilityDist.minimumDistance.lowestDurationLabel;
                                facilityDist.FromCityValueTime = facilityDist.minimumDistance.lowestDurationValue;
                                facilityDist.FromCityName = facilityDist.minimumDistance.cityName;
                            }

                        }
                        return facilityDist;
                    }
                    else
                    {
                        facilityDist.Error += "Missing Coordinates: " + hfFrom.FullName + "/n";
                    }
                }

                return facilityDist;

                //switch ('K')
                //{
                //    case 'K': //Kilometers -> default
                //        return dist * 1.609344;
                //    case 'N': //Nautical Miles 
                //        return dist * 0.8684;
                //    case 'M': //Miles
                //        return dist;
                //}

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public HrMarking GetMarking(int designationId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var markings = _db.HrMarkings.FirstOrDefault(x => x.Designation_Id == designationId);
                    return markings;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public List<HFApplication> CalculateFinalScore(List<HFApplication> recentApplications, int designationId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var markings = _db.HrMarkings.FirstOrDefault(x => x.Designation_Id == designationId);
                    foreach (var item in recentApplications)
                    {

                        //if (item.profile.CNIC == "3520136609741")
                        //{
                        //    item.profile.disability = 'Vision (one eye)';
                        //}



                        item.finalScore = new FinalScoreModel();
                        item.finalScore.daysFromFirstAppointment = CalculateDays(item.profile.DateOfFirstAppointment);
                        item.finalScore.totalService = item.finalScore.daysFromFirstAppointment * markings.ServiceTotal;
                        item.finalScore.totalService = item.finalScore.totalService == null ? 0 : Math.Round(item.finalScore.totalService.Value, 1);
                        if (item.profile.PresentJoiningDate != null) { item.finalScore.daysFromPresentJoiningDate = CalculateDays(item.profile.PresentJoiningDate); }
                        else { item.finalScore.daysFromPresentJoiningDate = CalculateDays(item.profile.PresentPostingDate); }
                        item.finalScore.daysFromPresentJoiningDate = item.finalScore.daysFromPresentJoiningDate == null ? 0 : Math.Round(item.finalScore.daysFromPresentJoiningDate.Value, 1);
                        item.finalScore.posting = item.finalScore.daysFromPresentJoiningDate * markings.ServiceCurrent;
                        item.finalScore.serviceScore = item.finalScore.totalService + item.finalScore.posting;
                        item.finalScore.service = item.finalScore.serviceScore * markings.Service;
                        item.finalScore.service = item.finalScore.service == null ? 0 : Math.Round(item.finalScore.service.Value, 1);
                        item.finalScore.totalDays = item.facilityDistances.Sum(x => x.TotalDays);
                        item.finalScore.totalServiceDays = 1095;
                        item.finalScore.lhrDistance = 0;
                        item.finalScore.roadDistance = 0;
                        item.finalScore.preferredDistrict = 0;
                        item.finalScore.dhqDistance = 0;
                        item.finalScore.thqDistance = 0;
                        item.finalScore.mainCityDistance = 0;
                        item.finalScore.totalDistances = new List<double?>();
                        // for health facilities
                        for (int index = 0; index < item.facilityDistances.Count; index++)
                        {
                            var val = item.facilityDistances[index];
                            //item.finalScore.totalServiceDays += item.facilityDistances[index].TotalDays;

                            val.lhrDistanceKM = val.FromLHRValue / 1000;
                            val.lhrDistance = val.TotalDays * val.lhrDistanceKM;
                            val.lhrDistance = val.lhrDistance == null ? 0 : Math.Round(val.lhrDistance.Value, 1);
                            item.finalScore.lhrDistance += val.lhrDistance;

                            val.preferredDistrictLol = val.TotalDays * (val.PreferredDistrictValue / 1000);
                            val.preferredDistrictLol = val.preferredDistrictLol == null ? 0 : Math.Round(val.preferredDistrictLol.Value, 1);
                            item.finalScore.preferredDistrict += val.preferredDistrictLol == null ? 0 : val.preferredDistrictLol;

                            //item.finalScore.roadDistance += item.facilityDistances[index].TotalDays * (item.facilityDistances[index].ByRoadValue / 1000); 

                            val.dhqDistanceKM = val.dhqDistance / 1000;
                            val.dhqDistance = val.TotalDays * val.dhqDistanceKM;
                            val.dhqDistance = val.dhqDistance == null ? 0 : Math.Round(val.dhqDistance.Value, 1);
                            item.finalScore.dhqDistance += val.dhqDistance == null ? 0 : val.dhqDistance;

                            val.thqDistanceKM = val.FromTHQValue / 1000;
                            val.thqDistance = val.TotalDays * val.thqDistanceKM;
                            val.thqDistance = val.thqDistance == null ? 0 : Math.Round(val.thqDistance.Value, 1);
                            item.finalScore.thqDistance += val.thqDistance == null ? 0 : val.thqDistance;

                            val.mainCityDistanceKM = val.FromCityValue / 1000;
                            val.mainCityDistance = val.TotalDays * val.mainCityDistanceKM;
                            val.mainCityDistance = val.mainCityDistance == null ? 0 : Math.Round(val.mainCityDistance.Value, 1);
                            item.finalScore.mainCityDistance += val.mainCityDistance == null ? 0 : val.mainCityDistance;
                        }
                        item.finalScore.totalDistancesString += "(";

                        for (int index = 0; index < item.facilityDistances.Count; index++)
                        {
                            var val = item.facilityDistances[index];
                            val.totalDistance = (val.TotalDays * (val.FromLHRValue / 1000) * markings.FromLahoreCity) +
                              (val.TotalDays * (val.FromDHQValue / 1000) * markings.NearDHQ) +
                              (val.TotalDays * (val.FromTHQValue / 1000) * markings.NearTHQ) +
                              (val.TotalDays * (val.PreferredDistrictValue / 1000) * markings.PreferredDistrict) +
                              (val.TotalDays * (val.FromCityValue / 1000) * markings.MainCity);
                            val.totalDistance = val.totalDistance == null ? 0 : Math.Round(val.totalDistance.Value, 1);
                            item.finalScore.totalDistances.Add(val.totalDistance == null ? 0 : val.totalDistance);
                            item.finalScore.totalDistancesString += (val.totalDistance == null ? 0 : val.totalDistance) + "";

                            item.finalScore.totalDistancesString += ((index + 1) < item.facilityDistances.Count) ? " + " : ") / 1095";
                        }

                        item.finalScore.subScoreSum = (item.finalScore.lhrDistance * markings.FromLahoreCity) + (item.finalScore.dhqDistance * markings.NearDHQ)
                          + (item.finalScore.thqDistance * markings.NearTHQ) + (item.finalScore.preferredDistrict * markings.PreferredDistrict) + (item.finalScore.mainCityDistance * markings.MainCity);
                        item.finalScore.subScore = item.finalScore.subScoreSum / item.finalScore.totalServiceDays;
                        item.finalScore.subScore = Math.Round(item.finalScore.subScore == null ? 0 : Math.Round(item.finalScore.subScore.Value, 2));
                        /*    item.facilityDistances.push({
                           HFName: 'Total',
                           TotalDays: totoalDays,
                           FromLHR: (+((lhrDistance / totoalServiceDays) * markings.distanceDetail.lhr) as number).toFixed(),
                           FromCity: (+((mainCityDistance / totoalServiceDays) * markings.distanceDetail.city) as number).toFixed(),
                           FromDHQ: (+((dhqDistance / totoalServiceDays) * markings.distanceDetail.dhq) as number).toFixed(),
                           FromTHQ: (+((thqDistance / totoalServiceDays) * markings.distanceDetail.thq) as number).toFixed()
                         }); */
                        /* item.facilityDistances.push({
                          HFName: 'Sub Total',
                          lhrDistance: +lhrDistance,
                          dhqDistance: +dhqDistance,
                          thqDistance: +thqDistance,
                          mainCityDistance: +mainCityDistance,
                          totoalServiceDays: +totoalServiceDays,
                          TotalDays: +item.subScore
                        }); */

                        item.finalScore.distance = item.finalScore.subScore * markings.Distance;
                        item.finalScore.distance = Math.Round(item.finalScore.distance == null ? 0 : Math.Round(item.finalScore.distance.Value, 2));
                        //        item.facilityDistances.Add(new FacilityDistance() {
                        //        HFName =  "Total",
                        //        finalScore.preferredDistrict = finalScore.preferredDistrict / finalScore.totalServiceDays,
                        //        finalScore.lhrDistance = finalScore.lhrDistance / finalScore.totalServiceDays,
                        //            finalScore.dhqDistance= finalScore.dhqDistance / finalScore.totalServiceDays,
                        //            finalScore.thqDistance= finalScore.thqDistance / finalScore.totalServiceDays,
                        //            finalScore.mainCityDistance= finalScore.mainCityDistance / finalScore.totalServiceDays,
                        //            finalScore.totalDistances= finalScore.totalDistances,
                        //            finalScore.TotalDays = finalScore.subScore
                        //        });
                        //        item.facilityDistances.push({
                        //        HFName: 'Score',
                        //TotalDays: +item.subScore
                        //        });
                        //        finalScore.fromVacancy = item.facilityVacancyPercentage.PercentageFrom * markings.vacancyDetail.from;
                        //        item.toVacancy = item.facilityVacancyPercentage.PercentageTo * markings.vacancyDetail.to;
                        //        item.vacancyScore = (((item.fromVacancy + item.toVacancy)) as number).toFixed();
                        //        item.vacancy = (((item.fromVacancy + item.toVacancy) * markings.vacancy) as number).toFixed();

                        item.finalScore.finalScore = item.finalScore.service + item.finalScore.distance;
                        //if (item.profile.disability)
                        //{
                        //    item.finalScore = (+item.finalScore + +markings.disability) as number;
                        //}

                        //    if (this.isBHU)
                        //    {
                        //        if (item.profile.Gender == 'Female')
                        //        {
                        //            item.bhuVacant = 93;
                        //            item.finalScore = +item.finalScore + +markings.genderFemale as number;
                        //        }
                        //        else if (item.profile.Gender == 'Male')
                        //        {
                        //            item.bhuVacant = 23;
                        //            item.finalScore = +item.finalScore + +markings.genderMale as number;
                        //        }
                        //        if (item.bhuVacant > this.hfTenure.percentage)
                        //        {
                        //            item.finalScore = (+item.finalScore * markings.bhuVacant) as number;
                        //        }
                        //        item.finalScore = item.finalScore ? +item.finalScore as number : 0;
                        //        item.finalScore = +item.finalScore.toFixed();
                        //    }
                        //    item.finalScore = +item.finalScore as number;
                    }
                    recentApplications = recentApplications.OrderByDescending(k => k.finalScore.finalScore).ToList();
                    return recentApplications;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public FacilityDistance GetDistance(string orgin, string destinations)
        {
            var facilityDist = new FacilityDistance();

            RootObject result = new RootObject();
            string url = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins=" + orgin + "&destinations=" + destinations + "&key=AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI";

            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<RootObject>(responseFromServer);
                if (result.rows.Count > 0)
                {
                    facilityDist.ByRoad = result.rows[0].elements[0].distance.text;
                    facilityDist.FromDHQ = result.rows[0].elements[0].distance.text;
                    facilityDist.FromTHQ = result.rows[0].elements[0].distance.text;
                }
            }
            response.Close();
            return facilityDist;
        }
        public double CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double? rlat1 = Math.PI * latitude1 / 180;
            double? rlat2 = Math.PI * latitude2 / 180;
            double? theta = longitude1 - longitude2;
            double? rtheta = Math.PI * theta / 180;
            double? dist =
                Math.Sin(rlat1.Value) * Math.Sin(rlat2.Value) + Math.Cos(rlat1.Value) *
                Math.Cos(rlat2.Value) * Math.Cos(rtheta.Value);
            dist = Math.Acos(dist.Value);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            return Math.Round((double)dist, 1);
        }
        public double CalculateDays(DateTime? dateTime)
        {
            if (dateTime == null) return 0;
            DateTime dateNow = DateTime.UtcNow.AddHours(5);
            return (dateNow - dateTime.Value).TotalDays;
        }
        public FacilityVacancyPercentage HealthFacility_CalculateVacancy(int? HF_From_Id, int? HF_Id, int? designationId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var facilityVacancyPercentage = new FacilityVacancyPercentage();
                    facilityVacancyPercentage.HFId = (int)HF_From_Id;
                    List<int> designationIds = new List<int>();
                    if (designationId > 0)
                    {
                        designationIds.Add((int)designationId);
                        if (designationId == 802 || designationId == 1320)
                        {
                            designationIds.Add(2404);
                        }
                    }
                    var vpmTo = _db.VPMasters.FirstOrDefault(x => x.HF_Id == HF_Id && designationIds.Contains(x.Desg_Id));
                    var vpmFrom = _db.VPMasters.FirstOrDefault(x => x.HF_Id == HF_From_Id && designationIds.Contains(x.Desg_Id));
                    if (vpmFrom != null)
                    {
                        int? SanctionedFrom = vpmFrom.TotalSanctioned;
                        int? filledNowFrom = vpmFrom.TotalWorking;
                        int filledAfterFrom = filledNowFrom.Value - 1;
                        decimal percentageFrom = ((decimal)filledAfterFrom) / ((decimal)SanctionedFrom.Value);
                        percentageFrom = percentageFrom * 100;
                        facilityVacancyPercentage.PercentageFrom = Math.Round(percentageFrom, 1);
                    }
                    if (vpmTo != null)
                    {
                        int? SanctionedTo = vpmTo.TotalSanctioned;
                        int? filledNowTo = vpmTo.TotalWorking;
                        int filledAfterTo = filledNowTo.Value + 1;
                        decimal percentageTo = ((decimal)filledAfterTo) / ((decimal)SanctionedTo.Value);
                        percentageTo = percentageTo * 100;
                        facilityVacancyPercentage.PercentageTo = Math.Round(percentageTo, 1);
                    }
                    return facilityVacancyPercentage;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrProfileStatusWiseTab> HealthFacility_Profiles_Status(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int HF_Id = GetHFId(hfmisCode);
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    IQueryable<ProfileDetailsView> query = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && x.Status_Id != 16).AsQueryable();

                    var employeeStatuses = query.GroupBy(x => new { x.Status_Id, x.StatusName }).Select(x => new HrProfileStatusWiseTab
                    {
                        Id = x.Key.Status_Id,
                        Name = x.Key.StatusName,
                        Count = x.Count()
                    }).OrderBy(x => x.Name).ToList();
                    return employeeStatuses;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrProfileStatusWiseTab> HealthFacility_Profiles_EmpModes(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int HF_Id = GetHFId(hfmisCode);
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    IQueryable<ProfileDetailsView> query = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id && (x.Status_Id == 2 ||
                    x.Status_Id == 3 ||
                    x.Status_Id == 9 ||
                    x.Status_Id == 31 ||
                    x.Status_Id == 17 ||
                    x.Status_Id == 34 ||
                    x.Status_Id == 30)).AsQueryable();

                    var employeeStatuses = query.GroupBy(x => new { x.EmpMode_Id, x.EmpMode_Name }).Select(x => new HrProfileStatusWiseTab
                    {
                        Id = x.Key.EmpMode_Id,
                        Name = x.Key.EmpMode_Name,
                        Count = x.Count()
                    }).OrderBy(x => x.Name).ToList();
                    return employeeStatuses;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrProfileStatusWiseTab> HealthFacility_Profiles_Designations(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int HF_Id = GetHFId(hfmisCode);
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    IQueryable<ProfileDetailsView> query = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == HF_Id
                    && (x.Status_Id == 2 ||
                    x.Status_Id == 3 ||
                    x.Status_Id == 9 ||
                    x.Status_Id == 31 ||
                    x.Status_Id == 17 ||
                    x.Status_Id == 34 ||
                    x.Status_Id == 30)).AsQueryable();

                    var employeeStatuses = query.GroupBy(x => new { x.WDesignation_Id, x.WDesignation_Name }).Select(x => new HrProfileStatusWiseTab
                    {
                        Id = x.Key.WDesignation_Id,
                        Name = x.Key.WDesignation_Name,
                        Count = x.Count()
                    }).OrderBy(x => x.Name).ToList();
                    return employeeStatuses;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<ProfileDetailsView> HealthFacility_ProfilesAgainstVacancy(int hf_Id, int designation_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    List<int?> vpProfileStatus = _db.VpProfileStatus.Where(x => x.IsActive == true).Select(k => k.ProfileStatus_Id).ToList();
                    emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == hf_Id && x.WDesignation_Id == designation_Id
                    && vpProfileStatus.Contains(x.Status_Id)).OrderByDescending(x => x.EmployeeName).ToList();
                    return emoloyeeProfiles;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<HrProfileStatusWiseTab> HealthFacility_PPSC_Designations(string hfmisCode)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int HF_Id = GetHFId(hfmisCode);
                    IQueryable<MeritPostingView> query = _db.MeritPostingViews.Where(x => x.PostingHF_Id == HF_Id).AsQueryable();

                    var postedDesignations = query.GroupBy(x => new { x.Designation_Id, x.DesignationName }).Select(x => new HrProfileStatusWiseTab
                    {
                        Id = x.Key.Designation_Id,
                        Name = x.Key.DesignationName,
                        Count = x.Count()
                    }).OrderBy(x => x.Name).ToList();
                    return postedDesignations;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<MeritPostingView> HealthFacility_PPSC_Candidates(string hfmisCode, int designationId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int HF_Id = GetHFId(hfmisCode);
                    List<MeritPostingView> posted = _db.MeritPostingViews.Where(x => x.PostingHF_Id == HF_Id && x.Designation_Id == designationId).OrderBy(k => k.MeritNumber).ToList();
                    return posted;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private HFDashboardViewModel BindDashboardModel(HFListP healthFacility, HFPhoto hfphoto, List<HFPhoto> hfPhotos, List<HFWard> hFWards, List<HFServicesViewModel> hFServices, List<VpMasterProfileView> hfVacancy, ProfileDetailsView hodProfile, List<ProfileDetailsView> emoloyeeProfiles, List<ProfileDetailsView> heads, HFUCMView hFUCM)
        {
            return new HFDashboardViewModel
            {
                Id = healthFacility.Id,
                HFMISCode = healthFacility.HFMISCode,
                FullName = healthFacility.FullName,
                HFAC = healthFacility.HFAC,
                Name = healthFacility.Name,
                DivisionCode = healthFacility.DivisionCode,
                DivisionName = healthFacility.DivisionName,
                DistrictCode = healthFacility.DistrictCode,
                DistrictName = healthFacility.DistrictName,
                TehsilCode = healthFacility.TehsilCode,
                TehsilName = healthFacility.TehsilName,
                HFCategoryName = healthFacility.HFCategoryName,
                ImagePath = healthFacility.ImagePath,
                CategoryCode = healthFacility.CategoryCode,
                OrderBy = healthFacility.OrderBy,
                Entity_Lifecycle_Id = healthFacility.Entity_Lifecycle_Id,
                IsActive = healthFacility.IsActive,
                Created_Date = healthFacility.Created_Date,
                Created_By = healthFacility.Created_By,
                Last_Modified_By = healthFacility.Last_Modified_By,
                Users_Id = healthFacility.Users_Id,
                HfmisOldCode = healthFacility.HfmisOldCode,
                HFTypeName = healthFacility.HFTypeName,
                HFTypeCode = healthFacility.HFTypeCode,
                PhoneNo = healthFacility.PhoneNo,
                FaxNo = healthFacility.FaxNo,
                Email = healthFacility.Email,
                Address = healthFacility.Address,
                Status = healthFacility.Status,
                CoveredArea = healthFacility.CoveredArea,
                UnCoveredArea = healthFacility.UnCoveredArea,
                ResidentialArea = healthFacility.ResidentialArea,
                NonResidentialArea = healthFacility.NonResidentialArea,
                NA = healthFacility.NA,
                PP = healthFacility.PP,
                Mauza = healthFacility.Mauza,
                UcName = healthFacility.UcName,
                UcNo = healthFacility.UcNo,
                Latitude = healthFacility.Latitude,
                Longitude = healthFacility.Longitude,
                HFPhoto = hfphoto,
                HFPhotos = hfPhotos,
                HFWardsList = hFWards,
                HFServicesList = hFServices,
                Vacancies = hfVacancy,
                HeadOfDepartment = hodProfile,
                EmoloyeeProfiles = emoloyeeProfiles,
                Heads = heads,
                HFUCMView = hFUCM
            };
        }
        private HealthFacility BindHFSaveModel(HFList clientHf)
        {
            var healthFacility = new HealthFacility();

            //check if new or edit
            if (clientHf.Id != 0) { return null; }

            //Required
            if (clientHf.Name == null) { return null; }
            else { healthFacility.Name = clientHf.Name; }

            //Required
            if (clientHf.HFMISCode == null) { return null; }
            else { healthFacility.HfmisCode = clientHf.HFMISCode; }

            //Required
            if (clientHf.HFAC == 0) { return null; }
            else { healthFacility.HFAC = clientHf.HFAC; }

            healthFacility.PhoneNo = clientHf.PhoneNo;

            healthFacility.FaxNo = clientHf.FaxNo;
            healthFacility.Email = clientHf.Email;

            healthFacility.Address = clientHf.Address;

            healthFacility.Status = clientHf.Status;

            healthFacility.CoveredArea = clientHf.CoveredArea;
            healthFacility.UnCoveredArea = clientHf.UnCoveredArea;
            healthFacility.ResidentialArea = clientHf.ResidentialArea;
            healthFacility.NonResidentialArea = clientHf.NonResidentialArea;

            healthFacility.UcName = clientHf.UcName;
            healthFacility.UcNo = clientHf.UcNo;
            healthFacility.NA = clientHf.NA;
            healthFacility.PP = clientHf.PP;
            healthFacility.Mauza = clientHf.Mauza;

            healthFacility.Latitude = clientHf.Latitude;
            healthFacility.Longitude = clientHf.Longitude;

            healthFacility.HfmisOldCode = healthFacility.HfmisOldCode;
            healthFacility.GEOM = healthFacility.GEOM;

            return healthFacility;

        }
        private HealthFacility BindHFEditModel(HFList clientHf, HealthFacility healthFacility)
        {
            //check if new or edit
            if (clientHf.Id == 0) { return null; }
            if (healthFacility.Id == 0) { return null; }
            if (healthFacility.HfmisCode.Length != 19) { return null; }

            //Required
            //if (clientHf.Name == null) { return null; }
            //else { healthFacility.Name = clientHf.Name; }

            //Required
            //if (clientHf.HFMISCode == null) { return null; }
            //else { healthFacility.HfmisCode = clientHf.HFMISCode; }

            //Required
            //if (clientHf.HFAC == 0) { return null; }
            //else { healthFacility.HFAC = clientHf.HFAC; }

            //Required
            //if (clientHf.PhoneNo == null) { return null; }
            //else { healthFacility.PhoneNo = clientHf.PhoneNo; }

            healthFacility.FaxNo = clientHf.FaxNo;
            healthFacility.Email = clientHf.Email;

            //Required
            //if (clientHf.Address == null) { return null; }
            //else { healthFacility.Address = clientHf.Address; }

            //Required
            //if (clientHf.Status == null) { return null; }
            //else { healthFacility.Status = clientHf.Status; }

            healthFacility.HfmisCode = clientHf.HFMISCode;

            healthFacility.Name = clientHf.Name;

            healthFacility.HFAC = clientHf.HFAC;
            healthFacility.PhoneNo = clientHf.PhoneNo;

            healthFacility.Address = clientHf.Address;
            healthFacility.Status = clientHf.Status;

            healthFacility.CoveredArea = clientHf.CoveredArea;
            healthFacility.UnCoveredArea = clientHf.UnCoveredArea;
            healthFacility.ResidentialArea = clientHf.ResidentialArea;
            healthFacility.NonResidentialArea = clientHf.NonResidentialArea;

            healthFacility.UcName = clientHf.UcName;
            healthFacility.UcNo = clientHf.UcNo;
            healthFacility.NA = clientHf.NA;
            healthFacility.PP = clientHf.PP;
            healthFacility.Mauza = clientHf.Mauza;

            healthFacility.Latitude = clientHf.Latitude;
            healthFacility.Longitude = clientHf.Longitude;

            return healthFacility;

        }
        public FacilityDistance HealthFacility_Directions(int? HF_From_Id, int? ToHF_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    #region Properties
                    List<HFDistance> hFDistances = new List<HFDistance>();
                    HFDistance hfDistanceNew;
                    var facilityDist = new FacilityDistance();
                    facilityDist.minimumDistances = new List<MinimumDistanceCity>();
                    bool prefDist = false, p2p = false, nDhq = false, nThq = false, lhr = false,
                        mltn = false,
                        ryk = false,
                        rwp = false,
                        fsd = false,
                        gjr = false,
                        srg = false,
                        sahiwal = false,
                        bwp = false,
                        dgk = false,
                        sialkot = false;
                    int distancesAvailable = 0;
                    #endregion

                    var hfFrom = _db.HFListPs.FirstOrDefault(x => x.Id == HF_From_Id);
                    if (hfFrom != null)
                    {
                        if (hfFrom.Latitude != null && hfFrom.Longitude != null)
                        {
                            var hfTo = _db.HFListPs.FirstOrDefault(x => x.Id == ToHF_Id);

                            #region Destinations
                            #region AddOrigin
                            string origin = hfFrom.Latitude + "," + hfFrom.Longitude;
                            string destinations = "";
                            #endregion

                            #region Near DHQ Bird Eye Distance
                            var dhqs = _db.HFListPs.Where(x => x.HFTypeCode.Equals("011") && x.IsActive == true);
                            List<DistanceAndId> dhqsDistanceAndIds = new List<DistanceAndId>();
                            foreach (var dhq in dhqs)
                            {
                                if (dhq.Latitude != null && dhq.Longitude != null)
                                {
                                    double distance = CalculateDistance(Convert.ToDouble(hfFrom.Latitude),
                                    Convert.ToDouble(hfFrom.Longitude),
                                    Convert.ToDouble(dhq.Latitude),
                                    Convert.ToDouble(dhq.Longitude));
                                    if (distance >= 0)
                                    {
                                        dhqsDistanceAndIds.Add(new DistanceAndId() { Id = dhq.Id, Distance = distance, FacilityName = dhq.FullName });
                                    }
                                }
                            }
                            dhqsDistanceAndIds = dhqsDistanceAndIds.OrderBy(x => x.Distance).ToList();
                            HFListP newrestDHQ = null;
                            if (dhqsDistanceAndIds.Count > 0)
                            {
                                int dhqsDistanceAndId = dhqsDistanceAndIds[0].Id;
                                newrestDHQ = _db.HFListPs.FirstOrDefault(x => x.Id == dhqsDistanceAndId);
                                if (newrestDHQ.DistrictCode != hfFrom.DistrictCode)
                                {
                                    facilityDist.FromDHQName = newrestDHQ.DistrictName;
                                }
                            }

                            #endregion

                            #region Near THQ Bird Eye Distance
                            var thqs = _db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfFrom.DistrictCode) && x.HFTypeCode.Equals("012") && x.IsActive == true);
                            List<DistanceAndId> thqsDistanceAndIds = new List<DistanceAndId>();
                            foreach (var thq in thqs)
                            {
                                if (thq.Latitude != null && thq.Longitude != null)
                                {
                                    double distance = CalculateDistance(Convert.ToDouble(hfFrom.Latitude),
                                    Convert.ToDouble(hfFrom.Longitude),
                                    Convert.ToDouble(thq.Latitude),
                                    Convert.ToDouble(thq.Longitude));
                                    if (distance >= 0)
                                    {
                                        thqsDistanceAndIds.Add(new DistanceAndId() { Id = thq.Id, Distance = distance, FacilityName = thq.FullName });
                                    }
                                }
                            }
                            thqsDistanceAndIds = thqsDistanceAndIds.OrderBy(x => x.Distance).ToList();
                            HFListP newrestTHQ = null;
                            if (thqsDistanceAndIds.Count > 0)
                            {
                                int thqsDistanceAndId = thqsDistanceAndIds[0].Id;
                                newrestTHQ = _db.HFListPs.FirstOrDefault(x => x.Id == thqsDistanceAndId);
                                if (newrestTHQ.Id != hfFrom.Id)
                                {
                                    facilityDist.FromTHQName = newrestTHQ.TehsilName;
                                }
                            }

                            #endregion

                            #region P2P
                            if (hfTo != null)
                            {
                                if (hfTo.Latitude != null && hfTo.Longitude != null)
                                {
                                    var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == hfTo.Id);
                                    //if (hfDistance == null)
                                    //{
                                    //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfTo.Id && x.ToHF_Id == hfFrom.Id);
                                    //}
                                    if (hfDistance == null)
                                    {
                                        hfDistanceNew = new HFDistance();
                                        hfDistanceNew.FromHF_Id = hfFrom.Id;
                                        hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                        hfDistanceNew.OriginName = hfFrom.FullName;
                                        hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                        hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                        hfDistanceNew.DestinationName = hfTo.FullName;
                                        hfDistanceNew.ToHF_Id = hfTo.Id;
                                        hfDistanceNew.ToHFMISCode = hfTo.HFMISCode;
                                        hfDistanceNew.DestinationLatitude = hfTo.Latitude;
                                        hfDistanceNew.DestinationLongitude = hfTo.Longitude;
                                        hFDistances.Add(hfDistanceNew);
                                        destinations += hfTo.Latitude + "%2C" + hfTo.Longitude + "%7C";
                                        p2p = true;
                                    }
                                    else
                                    {
                                        facilityDist.ByRoad = hfDistance.DistanceText;
                                        facilityDist.ByRoadValue = (float)hfDistance.DistanceValue;
                                        facilityDist.ByRoadTime = hfDistance.DurationText;
                                        facilityDist.ByRoadValueTime = (float)hfDistance.DurationValue;
                                    }
                                }
                                else
                                {
                                    facilityDist.Error += "To Health Facility Missing Coordinates: " + hfTo.FullName + "/n";
                                }
                            }
                            else
                            {
                                facilityDist.Error += "To Health Facility Missing/n";
                            }

                            #endregion

                            #region Near DHQ Road Distance
                            if (newrestDHQ != null)
                            {
                                if (newrestDHQ.Latitude != null && newrestDHQ.Longitude != null)
                                {
                                    var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == newrestDHQ.Id);
                                    //if (hfDistance == null)
                                    //{
                                    //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == newrestDHQ.Id && x.ToHF_Id == hfFrom.Id);
                                    //}
                                    if (hfDistance == null)
                                    {
                                        hfDistanceNew = new HFDistance();
                                        hfDistanceNew.FromHF_Id = hfFrom.Id;
                                        hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                        hfDistanceNew.OriginName = hfFrom.FullName;
                                        hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                        hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                        hfDistanceNew.ToHF_Id = newrestDHQ.Id;
                                        hfDistanceNew.ToHFMISCode = newrestDHQ.HFMISCode;
                                        hfDistanceNew.DestinationName = newrestDHQ.FullName;
                                        hfDistanceNew.DestinationLatitude = newrestDHQ.Latitude;
                                        hfDistanceNew.DestinationLongitude = newrestDHQ.Longitude;
                                        hFDistances.Add(hfDistanceNew);
                                        destinations += newrestDHQ.Latitude + "%2C" + newrestDHQ.Longitude + "%7C";
                                        nDhq = true;
                                    }
                                    else
                                    {
                                        facilityDist.FromDHQ = hfDistance.DistanceText;
                                        facilityDist.FromDHQValue = (float)hfDistance.DistanceValue;
                                        facilityDist.FromDHQTime = hfDistance.DurationText;
                                        facilityDist.FromDHQValueTime = (float)hfDistance.DurationValue;
                                    }
                                }
                                else
                                {
                                    facilityDist.Error += "Nearest DHQ Missing Coordinates: " + newrestDHQ.FullName + "/n";
                                }
                            }
                            else
                            {
                                facilityDist.Error += "Nearest DHQ Missing/n";
                            }
                            #endregion

                            #region Near THQ Distance
                            if (newrestTHQ != null)
                            {
                                if (newrestTHQ.Latitude != null && newrestTHQ.Longitude != null)
                                {
                                    var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHF_Id == newrestTHQ.Id);
                                    //if (hfDistance == null)
                                    //{
                                    //    hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == newrestTHQ.Id && x.ToHF_Id == hfFrom.Id);
                                    //}
                                    if (hfDistance == null)
                                    {
                                        hfDistanceNew = new HFDistance();
                                        hfDistanceNew.DistanceTo = "THQ";
                                        hfDistanceNew.FromHF_Id = hfFrom.Id;
                                        hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                        hfDistanceNew.OriginName = hfFrom.FullName;
                                        hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                        hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                        hfDistanceNew.ToHF_Id = newrestTHQ.Id;
                                        hfDistanceNew.ToHFMISCode = newrestTHQ.HFMISCode;
                                        hfDistanceNew.DestinationName = newrestTHQ.FullName;
                                        hfDistanceNew.DestinationLatitude = newrestTHQ.Latitude;
                                        hfDistanceNew.DestinationLongitude = newrestTHQ.Longitude;
                                        hFDistances.Add(hfDistanceNew);
                                        destinations += newrestTHQ.Latitude + "%2C" + newrestTHQ.Longitude + "%7C";
                                        nThq = true;
                                    }
                                    else
                                    {
                                        facilityDist.FromTHQ = hfDistance.DistanceText;
                                        facilityDist.FromTHQValue = (float)hfDistance.DistanceValue;
                                        facilityDist.FromTHQTime = hfDistance.DurationText;
                                        facilityDist.FromTHQValueTime = (float)hfDistance.DurationValue;
                                    }
                                }
                                else
                                {
                                    facilityDist.Error += "Nearest THQ Missing Coordinates: " + newrestTHQ.FullName + "/n";
                                }
                            }
                            else
                            {
                                facilityDist.Error += "Nearest THQ Missing/n";
                            }

                            #endregion

                            #region From Lahore
                            //Lahore City
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("035002001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "035002001";
                                hfDistanceNew.DestinationName = "Lahore City";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "31.5204%2C74.3587" + "%7C";
                                lhr = true;
                            }
                            else
                            {
                                facilityDist.FromLHR = hfDistanceNew.DistanceText;
                                facilityDist.FromLHRValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.FromLHRTime = hfDistanceNew.DurationText;
                                facilityDist.FromLHRValueTime = (float)hfDistanceNew.DurationValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Lahore City",
                                    lowestLabel = facilityDist.FromLHR,
                                    lowestValue = facilityDist.FromLHRValue,
                                    lowestDurationLabel = facilityDist.FromLHRTime,
                                    lowestDurationValue = facilityDist.FromLHRValueTime
                                });
                            }
                            //destinations += "31.5204%2C74.3587" + "%7C";

                            #endregion

                            #region MainCity
                            #region Multan
                            //Multan City
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("036001007"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "036001007";
                                hfDistanceNew.DestinationName = "Multan City";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "30.1756%2C71.4708" + "%7C";
                                mltn = true;
                            }
                            else
                            {
                                facilityDist.FromMultan = hfDistanceNew.DistanceText;
                                facilityDist.FromMultanValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Multan City",
                                    lowestLabel = facilityDist.FromMultan,
                                    lowestValue = facilityDist.FromMultanValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            //string multanLatLong = "30.1756%2C71.4708";

                            #endregion

                            #region Rawalpindi
                            //Rawalpindi
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("037003005"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "037003005";
                                hfDistanceNew.DestinationName = "Rawalpindi";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "33.5894%2C73.0664" + "%7C";
                                rwp = true;
                            }
                            else
                            {
                                facilityDist.FromRawalpindi = hfDistanceNew.DistanceText;
                                facilityDist.FromRawalpindiValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Rawalpindi",
                                    lowestLabel = facilityDist.FromRawalpindi,
                                    lowestValue = facilityDist.FromRawalpindiValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            //string rawalpindiLatLong = "33.5894%2C73.0664";

                            #endregion

                            #region Rahim Yar Khan
                            //Rahim Yar Khan
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("031003001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "031003001";
                                hfDistanceNew.DestinationName = "Rahim Yar Khan";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "28.4193%2C70.3139" + "%7C";
                                ryk = true;
                            }
                            else
                            {
                                facilityDist.FromRahimYarKhan = hfDistanceNew.DistanceText;
                                facilityDist.FromRahimYarKhanValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Rahim Yar Khan",
                                    lowestLabel = facilityDist.FromRahimYarKhan,
                                    lowestValue = facilityDist.FromRahimYarKhanValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            //string rahimYarKhanLatLong = "28.4193%2C70.3139";

                            #endregion

                            #region Faisalabad
                            //Faisalabad City
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("033001001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "033001001";
                                hfDistanceNew.DestinationName = "Faisalabad City";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "31.4251%2C73.0893" + "%7C";
                                fsd = true;
                            }
                            else
                            {
                                facilityDist.FromFaisalabad = hfDistanceNew.DistanceText;
                                facilityDist.FromFaisalabadValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Faisalabad City",
                                    lowestLabel = facilityDist.FromFaisalabad,
                                    lowestValue = facilityDist.FromFaisalabadValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            //string faisalabadLatLong = "31.4251%2C73.0893";

                            #endregion

                            #region Gujranwala
                            //Gujranwala
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("034001001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "034001001";
                                hfDistanceNew.DestinationName = "Gujranwala";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "32.1481%2C74.183" + "%7C";
                                gjr = true;
                            }
                            else
                            {
                                facilityDist.FromGujranwala = hfDistanceNew.DistanceText;
                                facilityDist.FromGujranwalaValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Gujranwala",
                                    lowestLabel = facilityDist.FromGujranwala,
                                    lowestValue = facilityDist.FromGujranwalaValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            //string gujranwalaLatLong = "32.1481%2C74.183";

                            #endregion

                            #region Bahawalpur
                            //Bahawalpur
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("031002001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "031002001";
                                hfDistanceNew.DestinationName = "Bahawalpur City";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "29.4117%2C71.6938" + "%7C";
                                bwp = true;
                            }
                            else
                            {
                                facilityDist.FromBahawalpur = hfDistanceNew.DistanceText;
                                facilityDist.FromBahawalpurValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Bahawalpur City",
                                    lowestLabel = facilityDist.FromBahawalpur,
                                    lowestValue = facilityDist.FromBahawalpurValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            #endregion

                            #region DeraGhaziKhan
                            //DeraGhaziKhan
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("032001001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "032001001";
                                hfDistanceNew.DestinationName = "Dera Ghazi Khan";
                                hfDistanceNew.DestinationLatitude = 31.5204;
                                hfDistanceNew.DestinationLongitude = 74.3587;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "30.0491%2C70.6389" + "%7C";
                                dgk = true;
                            }
                            else
                            {
                                facilityDist.FromDeraGhaziKhan = hfDistanceNew.DistanceText;
                                facilityDist.FromDeraGhaziKhanValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Dera Ghazi Khan",
                                    lowestLabel = facilityDist.FromDeraGhaziKhan,
                                    lowestValue = facilityDist.FromDeraGhaziKhanValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            #endregion

                            #region Sargodha
                            //Sargodha
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("038004001"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "038004001";
                                hfDistanceNew.DestinationName = "Sargodha";
                                hfDistanceNew.DestinationLatitude = 32.0676;
                                hfDistanceNew.DestinationLongitude = 72.6796;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "32.0676% 2C72.6796" + "%7C";
                                srg = true;
                            }
                            else
                            {
                                facilityDist.FromSargodha = hfDistanceNew.DistanceText;
                                facilityDist.FromSargodhaValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Sargodha",
                                    lowestLabel = facilityDist.FromSargodha,
                                    lowestValue = facilityDist.FromSargodhaValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            #endregion

                            #region Sahiwal
                            //Sahiwal
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("038004003"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "038004003";
                                hfDistanceNew.DestinationName = "Sahiwal";
                                hfDistanceNew.DestinationLatitude = 30.6562;
                                hfDistanceNew.DestinationLongitude = 73.0872;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "30.6562% 2C73.0872" + "%7C";
                                sahiwal = true;
                            }
                            else
                            {
                                facilityDist.FromSahiwal = hfDistanceNew.DistanceText;
                                facilityDist.FromSahiwalValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Sahiwal",
                                    lowestLabel = facilityDist.FromSahiwal,
                                    lowestValue = facilityDist.FromSahiwalValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            #endregion

                            #region Sialkot
                            //Sialkot
                            hfDistanceNew = _db.HFDistances.FirstOrDefault(x => (x.FromHF_Id == hfFrom.Id) && x.ToHFMISCode.Equals("034003002"));
                            if (hfDistanceNew == null)
                            {
                                hfDistanceNew = new HFDistance();
                                hfDistanceNew.FromHF_Id = hfFrom.Id;
                                hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                hfDistanceNew.OriginName = hfFrom.FullName;
                                hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                hfDistanceNew.ToHFMISCode = "034003002";
                                hfDistanceNew.DestinationName = "Sialkot";
                                hfDistanceNew.DestinationLatitude = 32.4996;
                                hfDistanceNew.DestinationLongitude = 74.5346;
                                hFDistances.Add(hfDistanceNew);
                                destinations += "32.4996% 2C74.5346" + "%7C";
                                sialkot = true;
                            }
                            else
                            {
                                facilityDist.FromSialkot = hfDistanceNew.DistanceText;
                                facilityDist.FromSialkotValue = (float)hfDistanceNew.DistanceValue;
                                facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                {
                                    cityName = "Sialkot",
                                    lowestLabel = facilityDist.FromSialkot,
                                    lowestValue = facilityDist.FromSialkotValue,
                                    lowestDurationLabel = hfDistanceNew.DurationText,
                                    lowestDurationValue = (float)hfDistanceNew.DurationValue
                                });
                            }
                            #endregion

                            #endregion

                            #region Preferred District
                            if (hfTo != null)
                            {
                                var preferredDistrict = _db.DistrictLatLongs.FirstOrDefault(x => x.Code == hfTo.DistrictCode);
                                if (preferredDistrict != null && preferredDistrict.Latitude != null && preferredDistrict.Longitude != null)
                                {
                                    var hfDistance = _db.HFDistances.FirstOrDefault(x => x.FromHF_Id == hfFrom.Id && x.ToHFMISCode == hfTo.DistrictCode);
                                    if (hfDistance == null)
                                    {
                                        hfDistanceNew = new HFDistance();
                                        hfDistanceNew.FromHF_Id = hfFrom.Id;
                                        hfDistanceNew.FromHFMISCode = hfFrom.HFMISCode;
                                        hfDistanceNew.OriginName = hfFrom.FullName;
                                        hfDistanceNew.OriginLatitude = hfFrom.Latitude;
                                        hfDistanceNew.OriginLongitude = hfFrom.Longitude;
                                        hfDistanceNew.DestinationName = preferredDistrict.Name;
                                        hfDistanceNew.ToHFMISCode = preferredDistrict.Code;
                                        hfDistanceNew.DestinationLatitude = preferredDistrict.Latitude;
                                        hfDistanceNew.DestinationLongitude = preferredDistrict.Longitude;
                                        hFDistances.Add(hfDistanceNew);
                                        destinations += preferredDistrict.Latitude + "%2C" + preferredDistrict.Longitude + "%7C";
                                        prefDist = true;
                                    }
                                    else
                                    {
                                        facilityDist.PreferredDistrict = hfDistance.DistanceText;
                                        facilityDist.PreferredDistrictValue = (float)hfDistance.DistanceValue;
                                        facilityDist.PreferredDistrictTime = hfDistance.DurationText;
                                        facilityDist.PreferredDistrictTimeValue = (float)hfDistance.DurationValue;
                                    }
                                }
                            }
                            #endregion
                            #endregion

                            #region Api Process
                            if (!string.IsNullOrEmpty(destinations))
                            {
                                if (destinations.EndsWith("%7C"))
                                {
                                    destinations = destinations.Substring(0, destinations.Length - 3);
                                }
                                DirectionResponse result = new DirectionResponse();
                                string url = @"https://maps.googleapis.com/maps/api/directions/json?units=metric&alternatives=true&origin=" + origin + "&destination=" + destinations + "&key=AIzaSyCKn6GNT8nJhIARmSWbiOqvvUxtsziZjzc";

                                WebRequest request = WebRequest.Create(url);
                                request.Credentials = CredentialCache.DefaultCredentials;

                                WebResponse response = request.GetResponse();
                                using (Stream dataStream = response.GetResponseStream())
                                {
                                    StreamReader reader = new StreamReader(dataStream);
                                    string responseFromServer = reader.ReadToEnd();
                                    result = JsonConvert.DeserializeObject<DirectionResponse>(responseFromServer);

                                    //for (int i = 0; i < hFDistances.Count; i++)
                                    //{
                                    //    var hfDistance = hFDistances[i];
                                    //    var dst = result.rows[0].elements[i];
                                    //    var name = result.destination_addresses[i];
                                    //}
                                    //#region Response
                                    //if (result.rows.Count > 0)
                                    //{
                                    //    #region P2P
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && p2p == true)
                                    //    {
                                    //        facilityDist.ByRoad = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.ByRoadValue = result.rows[0].elements[distancesAvailable].distance.value;

                                    //        facilityDist.ByRoadTime = result.rows[0].elements[distancesAvailable].duration.text;
                                    //        facilityDist.ByRoadValueTime = result.rows[0].elements[distancesAvailable].duration.value;

                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == ToHF_Id);
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.ByRoad;
                                    //            hfDistance.DistanceValue = facilityDist.ByRoadValue;
                                    //            hfDistance.DurationText = facilityDist.ByRoadTime;
                                    //            hfDistance.DurationValue = facilityDist.ByRoadValueTime;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Near DHQ
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && nDhq == true)
                                    //    {
                                    //        facilityDist.FromDHQ = result.rows[0].elements[1].distance.text;
                                    //        facilityDist.FromDHQValue = result.rows[0].elements[distancesAvailable].distance.value;

                                    //        facilityDist.FromDHQTime = result.rows[0].elements[distancesAvailable].duration.text;
                                    //        facilityDist.FromDHQValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == newrestDHQ.Id);
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromDHQ;
                                    //            hfDistance.DistanceValue = facilityDist.FromDHQValue;
                                    //            hfDistance.DurationText = facilityDist.FromDHQTime;
                                    //            hfDistance.DurationValue = facilityDist.FromDHQValueTime;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Near THQ
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && nThq == true)
                                    //    {
                                    //        facilityDist.FromTHQ = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromTHQValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.FromTHQTime = result.rows[0].elements[distancesAvailable].duration.text;
                                    //        facilityDist.FromTHQValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHF_Id == newrestTHQ.Id);
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromTHQ;
                                    //            hfDistance.DistanceValue = facilityDist.FromTHQValue;
                                    //            hfDistance.DurationText = facilityDist.FromTHQTime;
                                    //            hfDistance.DurationValue = facilityDist.FromTHQValueTime;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region From Lahore
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && lhr == true)
                                    //    {
                                    //        facilityDist.FromLHR = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromLHRValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.FromLHRTime = result.rows[0].elements[distancesAvailable].duration.text;
                                    //        facilityDist.FromLHRValueTime = result.rows[0].elements[distancesAvailable].duration.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Lahore",
                                    //            lowestLabel = facilityDist.FromLHR,
                                    //            lowestValue = facilityDist.FromLHRValue,
                                    //            lowestDurationLabel = facilityDist.FromLHRTime,
                                    //            lowestDurationValue = facilityDist.FromLHRValueTime
                                    //        });
                                    //        distancesAvailable++;
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "035002001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromLHR;
                                    //            hfDistance.DistanceValue = facilityDist.FromLHRValue;
                                    //            hfDistance.DurationText = facilityDist.FromLHRTime;
                                    //            hfDistance.DurationValue = facilityDist.FromLHRValueTime;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //    }
                                    //    #endregion

                                    //    #region MainCity

                                    //    #region Multan
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && mltn == true)
                                    //    {
                                    //        facilityDist.FromMultan = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromMultanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Multan",
                                    //            lowestLabel = facilityDist.FromMultan,
                                    //            lowestValue = facilityDist.FromMultanValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "036001007");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromMultan;
                                    //            hfDistance.DistanceValue = facilityDist.FromMultanValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Rawalpindi
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && rwp == true)
                                    //    {
                                    //        facilityDist.FromRawalpindi = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromRawalpindiValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Rawalpindi",
                                    //            lowestLabel = facilityDist.FromRawalpindi,
                                    //            lowestValue = facilityDist.FromRawalpindiValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "037003005");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromRawalpindi;
                                    //            hfDistance.DistanceValue = facilityDist.FromRawalpindiValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region RahimYarKhan
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && ryk == true)
                                    //    {
                                    //        facilityDist.FromRahimYarKhan = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromRahimYarKhanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "RahimYarKhan",
                                    //            lowestLabel = facilityDist.FromRahimYarKhan,
                                    //            lowestValue = facilityDist.FromRahimYarKhanValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "031003001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromRahimYarKhan;
                                    //            hfDistance.DistanceValue = facilityDist.FromRahimYarKhanValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }

                                    //    #endregion

                                    //    #region Faisalabad
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && fsd == true)
                                    //    {
                                    //        facilityDist.FromFaisalabad = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromFaisalabadValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Faisalabad",
                                    //            lowestLabel = facilityDist.FromFaisalabad,
                                    //            lowestValue = facilityDist.FromFaisalabadValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "033001001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromFaisalabad;
                                    //            hfDistance.DistanceValue = facilityDist.FromFaisalabadValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }

                                    //    #endregion

                                    //    #region Gujranwala
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && gjr == true)
                                    //    {
                                    //        facilityDist.FromGujranwala = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromGujranwalaValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Gujranwala",
                                    //            lowestLabel = facilityDist.FromGujranwala,
                                    //            lowestValue = facilityDist.FromGujranwalaValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "034001001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromGujranwala;
                                    //            hfDistance.DistanceValue = facilityDist.FromGujranwalaValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Bahawalpur
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && bwp == true)
                                    //    {
                                    //        facilityDist.FromBahawalpur = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromBahawalpurValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Bahawalpur",
                                    //            lowestLabel = facilityDist.FromBahawalpur,
                                    //            lowestValue = facilityDist.FromBahawalpurValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "031002001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromBahawalpur;
                                    //            hfDistance.DistanceValue = facilityDist.FromBahawalpurValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Dera Ghazi Khan
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && dgk == true)
                                    //    {
                                    //        facilityDist.FromDeraGhaziKhan = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromDeraGhaziKhanValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Bahawalpur",
                                    //            lowestLabel = facilityDist.FromDeraGhaziKhan,
                                    //            lowestValue = facilityDist.FromDeraGhaziKhanValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "032001001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromDeraGhaziKhan;
                                    //            hfDistance.DistanceValue = facilityDist.FromDeraGhaziKhanValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Sargodha
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && srg == true)
                                    //    {
                                    //        facilityDist.FromSargodha = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromSargodhaValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Sargodha",
                                    //            lowestLabel = facilityDist.FromSargodha,
                                    //            lowestValue = facilityDist.FromSargodhaValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "038004001");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromSargodha;
                                    //            hfDistance.DistanceValue = facilityDist.FromSargodhaValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Sahiwal
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && sahiwal == true)
                                    //    {
                                    //        facilityDist.FromSahiwal = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromSahiwalValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Sahiwal",
                                    //            lowestLabel = facilityDist.FromSahiwal,
                                    //            lowestValue = facilityDist.FromSahiwalValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "038004003");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromSahiwal;
                                    //            hfDistance.DistanceValue = facilityDist.FromSahiwalValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #region Sialkot
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && sialkot == true)
                                    //    {
                                    //        facilityDist.FromSialkot = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.FromSialkotValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.minimumDistances.Add(new MinimumDistanceCity
                                    //        {
                                    //            cityName = "Sialkot",
                                    //            lowestLabel = facilityDist.FromSialkot,
                                    //            lowestValue = facilityDist.FromSialkotValue,
                                    //            lowestDurationLabel = result.rows[0].elements[distancesAvailable]?.duration?.text,
                                    //            lowestDurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value
                                    //        });
                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == "034003002");
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.FromSialkot;
                                    //            hfDistance.DistanceValue = facilityDist.FromSialkotValue;
                                    //            hfDistance.DurationText = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //            hfDistance.DurationValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion

                                    //    #endregion

                                    //    #region Preferred District
                                    //    if (result.rows[0].elements.Count > distancesAvailable && result.rows[0].elements[distancesAvailable].distance != null && prefDist == true)
                                    //    {
                                    //        facilityDist.PreferredDistrict = result.rows[0].elements[distancesAvailable].distance.text;
                                    //        facilityDist.PreferredDistrictValue = result.rows[0].elements[distancesAvailable].distance.value;
                                    //        facilityDist.PreferredDistrictTime = result.rows[0].elements[distancesAvailable]?.duration?.text;
                                    //        facilityDist.PreferredDistrictTimeValue = (float)result.rows[0].elements[distancesAvailable].duration?.value;

                                    //        var hfDistance = hFDistances.FirstOrDefault(x => x.FromHF_Id == HF_From_Id && x.ToHFMISCode == hfTo.DistrictCode);
                                    //        if (hfDistance != null)
                                    //        {
                                    //            hfDistance.DistanceText = facilityDist.PreferredDistrict;
                                    //            hfDistance.DistanceValue = facilityDist.PreferredDistrictValue;
                                    //            hfDistance.DurationText = facilityDist.PreferredDistrictTime;
                                    //            hfDistance.DurationValue = facilityDist.PreferredDistrictTimeValue;
                                    //            hfDistance.UpdatedOn = DateTime.UtcNow.AddHours(5);
                                    //            _db.HFDistances.Add(hfDistance);
                                    //            _db.SaveChanges();
                                    //        }
                                    //        distancesAvailable++;
                                    //    }
                                    //    #endregion
                                    //}

                                    //#endregion

                                }
                                response.Close();
                            }

                            #endregion

                            //destinations += multanLatLong + "%7C" + rawalpindiLatLong + "%7C"
                            //     + rahimYarKhanLatLong + "%7C" + faisalabadLatLong + "%7C" + gujranwalaLatLong;

                            //var city = _db.Distr.FirstOrDefault(x => x.HFMISCode.StartsWith(hfFrom.DistrictCode) && x.HFTypeCode.Equals("012") && x.IsActive == true);
                            //if (city != null && city.Latitude != null)
                            //{
                            //    facilityDist.FromCity = CalculateDistance(newrestTHQ.Latitude, newrestTHQ.Longitude, hfFrom.Latitude, hfFrom.Longitude);
                            //}

                            facilityDist.HFId = hfFrom.Id;
                            facilityDist.HFName = hfFrom.FullName;

                            if (hfFrom.HFTypeCode.Equals("011"))
                            {
                                facilityDist.FromDHQ = "0";
                                facilityDist.FromDHQValue = 0;
                                facilityDist.FromDHQName = "";
                            }

                            if (hfFrom.HFTypeCode.Equals("012"))
                            {
                                facilityDist.FromTHQ = "0";
                                facilityDist.FromTHQValue = 0;
                                facilityDist.FromTHQName = "";
                            }
                            if (hfFrom.HFTypeCode.Equals("068"))
                            {
                                facilityDist.FromTHQ = "0";
                                facilityDist.FromTHQValue = 0;
                                facilityDist.FromTHQName = "";
                            }
                            facilityDist.minimumDistance = facilityDist.minimumDistances.OrderBy(x => x.lowestValue).FirstOrDefault();
                            if (facilityDist.minimumDistance != null)
                            {
                                if (facilityDist.minimumDistance.cityName.Equals(hfFrom.TehsilName))
                                {
                                    facilityDist.FromCity = "0";
                                    facilityDist.FromCityValue = 0;
                                    facilityDist.FromCityTime = "0";
                                    facilityDist.FromCityValueTime = 0;
                                    facilityDist.FromCityName = facilityDist.minimumDistance.cityName;
                                }
                                else
                                {
                                    facilityDist.FromCity = facilityDist.minimumDistance.lowestLabel;
                                    facilityDist.FromCityValue = facilityDist.minimumDistance.lowestValue;
                                    facilityDist.FromCityTime = facilityDist.minimumDistance.lowestDurationLabel;
                                    facilityDist.FromCityValueTime = facilityDist.minimumDistance.lowestDurationValue;
                                    facilityDist.FromCityName = facilityDist.minimumDistance.cityName;
                                }

                            }
                            return facilityDist;
                        }
                        else
                        {
                            facilityDist.Error += "Missing Coordinates: " + hfFrom.FullName + "/n";
                        }
                    }

                    return facilityDist;

                    //switch ('K')
                    //{
                    //    case 'K': //Kilometers -> default
                    //        return dist * 1.609344;
                    //    case 'N': //Nautical Miles 
                    //        return dist * 0.8684;
                    //    case 'M': //Miles
                    //        return dist;
                    //}

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
    public class HealthFacilityFilter : Paginator
    {
        public List<string> Divisions { get; set; }
        public List<string> Districts { get; set; }
        public List<string> Tehsils { get; set; }
        public List<string> HFTypes { get; set; }
        public List<string> HFCategories { get; set; }
        public List<int> HFACs { get; set; }
        public string HFMISCode { get; set; }
        public string HFStatus { get; set; }
    }
    public class HrProfileStatusWiseTab
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Count { get; set; }
    }
    public class HFApplication
    {
        public ApplicationView application { get; set; }
        public ProfileDetailsView profile { get; set; }
        public VpMasterProfileView vpMaster { get; set; }
        public FacilityDistance facilityDistance { get; set; }
        public FinalScoreModel finalScore { get; set; }
        public WinnerModel winner { get; set; }
        public List<FacilityDistance> facilityDistances { get; set; }
        public List<FacilityDistance> facilityDirections { get; set; }
        public FacilityVacancyPercentage facilityVacancyPercentage { get; set; }
        public RootObject Distance { get; set; }
    }

    public class FacilityDistance
    {
        public int HFId { get; set; }
        public string HFName { get; set; }
        public string ByRoad { get; set; }
        public float ByRoadValue { get; set; }
        public string FromDHQName { get; set; }
        public string FromDHQ { get; set; }
        public float FromDHQValue { get; set; }
        public string FromTHQName { get; set; }
        public string FromTHQ { get; set; }
        public float FromTHQValue { get; set; }
        public string FromLHR { get; set; }
        public float FromLHRValue { get; set; }
        public string FromCity { get; set; }
        public float FromCityValue { get; set; }
        public string FromCityName { get; set; }
        public string ByRoadTime { get; set; }
        public float ByRoadValueTime { get; set; }
        public string FromDHQTime { get; set; }
        public float FromDHQValueTime { get; set; }
        public string FromTHQTime { get; set; }
        public float FromTHQValueTime { get; set; }
        public string FromLHRTime { get; set; }
        public float FromLHRValueTime { get; set; }
        public string FromCityTime { get; set; }
        public float FromCityValueTime { get; set; }


        public string FromMultan { get; set; }
        public float FromMultanValue { get; set; }

        public string FromRahimYarKhan { get; set; }
        public float FromRahimYarKhanValue { get; set; }

        public string FromRawalpindi { get; set; }
        public float FromRawalpindiValue { get; set; }

        public string FromFaisalabad { get; set; }
        public float FromFaisalabadValue { get; set; }

        public string FromGujranwala { get; set; }
        public float FromGujranwalaValue { get; set; }

        public string FromBahawalpur { get; set; }
        public float FromBahawalpurValue { get; set; }

        public string FromDeraGhaziKhan { get; set; }
        public float FromDeraGhaziKhanValue { get; set; }

        public string FromSargodha { get; set; }
        public float FromSargodhaValue { get; set; }

        public string FromSahiwal { get; set; }
        public float FromSahiwalValue { get; set; }


        public string FromSialkot { get; set; }
        public float FromSialkotValue { get; set; }

        public string PreferredDistrict { get; set; }
        public float PreferredDistrictValue { get; set; }
        public string PreferredDistrictTime { get; set; }
        public float PreferredDistrictTimeValue { get; set; }

        public MinimumDistanceCity minimumDistance { get; set; }
        public HrServiceHistoryView ServiceHistory { get; set; }

        public List<MinimumDistanceCity> minimumDistances { get; set; }
        public int TotalDays { get; set; }

        public double? lhrDistanceKM { get; set; }
        public double? roadDistanceKM { get; set; }
        public double? preferredDistrictLolKM { get; set; }
        public double? dhqDistanceKM { get; set; }
        public double? thqDistanceKM { get; set; }
        public double? mainCityDistanceKM { get; set; }
        public double? totalDistanceKM { get; set; }
        public double? lhrDistance { get; set; }
        public double? roadDistance { get; set; }
        public double? preferredDistrictLol { get; set; }
        public double? dhqDistance { get; set; }
        public double? thqDistance { get; set; }
        public double? mainCityDistance { get; set; }
        public double? totalDistance { get; set; }

        public string Error { get; set; }
    }

    public class MinimumDistanceCity
    {
        public string cityName { get; set; }
        public string lowestLabel { get; set; }
        public float lowestValue { get; set; }
        public string lowestDurationLabel { get; set; }
        public float lowestDurationValue { get; set; }
    }
    public class FacilityVacancyPercentage
    {
        public int HFId { get; set; }
        public decimal PercentageFrom { get; set; }
        public decimal PercentageTo { get; set; }
    }
    public class FacilityApplicationListModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string HfmisCode { get; set; }
        public int Count { get; set; }
        public HFListP HealthFacility { get; set; }
    }
    public class FacilityApplicationTime
    {
        public int ProfileId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class RootObject
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
    }
    public class DistanceAndId
    {
        public int Id { get; set; }
        public double Distance { get; set; }
        public string FacilityName { get; set; }
    }
    public class WinnerModel
    {
        public int? Adhoc { get; set; }
        public int? Filled { get; set; }
        public int? Vacant { get; set; }
        public int? AVacant { get; set; }
        public int? Regular { get; set; }
    }
    public class FinalScoreModel
    {
        public double? daysFromFirstAppointment { get; set; }
        public double? daysFromPresentJoiningDate { get; set; }
        public double? totalService { get; set; }
        public double? posting { get; set; }
        public double? serviceScore { get; set; }
        public double? service { get; set; }
        public double? totalDays { get; set; }
        public double? totalServiceDays { get; set; }
        public double? lhrDistance { get; set; }
        public double? roadDistance { get; set; }
        public double? preferredDistrict { get; set; }
        public double? dhqDistance { get; set; }
        public double? thqDistance { get; set; }
        public double? mainCityDistance { get; set; }
        public List<double?> totalDistances { get; set; }
        public string totalDistancesString { get; set; }
        public double? subScoreSum { get; set; }
        public double? subScore { get; set; }
        public double? distance { get; set; }
        public double? fromVacancy { get; set; }
        public double? toVacancy { get; set; }
        public double? vacancyScore { get; set; }
        public double? vacancy { get; set; }
        public double? finalScore { get; set; }
    }

    public class DirectionResponse
    {
        List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        List<Routes> routes { get; set; }
        public string status { get; set; }
    }

    public class GeocodedWaypoint
    {
        public string geocoder_status { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }
    public class Routes
    {
        public Bounds bounds { get; set; }
        public string copyrights { get; set; }
        public List<Legs> legs { get; set; }
        public OverviewPolyline overview_polyline { get; set; }
        public string summary { get; set; }
    }
    public class Steps
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public EndLocation end_location { get; set; }
        public string html_instructions { get; set; }
        public Polyline polyline { get; set; }
        public StartLocation start_location { get; set; }
        public string travel_mode { get; set; }
    }
    public class Legs
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string end_address { get; set; }
        public EndLocation end_location { get; set; }
        public string start_address { get; set; }
        public StartLocation start_location { get; set; }
        public IList<object> traffic_speed_entry { get; set; }
        public IList<object> via_waypoint { get; set; }
        List<Routes> steps { get; set; }
    }
    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }
    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class OverviewPolyline
    {
        public string points { get; set; }
    }
    public class EndLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class StartLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public class Polyline
    {
        public string points { get; set; }
    }

}