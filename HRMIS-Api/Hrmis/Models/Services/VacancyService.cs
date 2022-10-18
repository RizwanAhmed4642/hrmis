using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.Vacancy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hrmis.Models.Services
{
    public class VacancyService
    {
        public List<ProfileDetailsView> GetSuggestedProfile(int vpMaster_Id, int hf_Id, int scale)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<ProfileDetailsView> emoloyeeProfiles = new List<ProfileDetailsView>();
                    List<int?> scales = new List<int?>();
                    if (scale >= 17)
                    {
                        scales.Add(17); scales.Add(18); scales.Add(19); scales.Add(20); scales.Add(21);
                    }
                    else
                    {
                        for (int i = 1; i < 17; i++)
                        {
                            scales.Add(i);
                        }
                    }
                    List<int?> vpProfileStatus = _db.VpProfileStatus.Where(x => x.IsActive == true).Select(k => k.ProfileStatus_Id).ToList();
                    List<int?> vpProfileIds = _db.VPProfileViews.Where(x => x.VPMaster_Id == vpMaster_Id && x.IsActive == true).Select(k => k.Profile_Id).ToList();

                    emoloyeeProfiles = _db.ProfileDetailsViews.Where(x => x.HealthFacility_Id == hf_Id
                    && scales.Contains(x.CurrentGradeBPS)
                    && !vpProfileIds.Contains(x.Id)
                    && vpProfileStatus.Contains(x.Status_Id)).OrderBy(x => x.EmployeeName).ToList();

                    return emoloyeeProfiles;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VPProfileView> GetVpProfiles(int vpMaster_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.VPProfileViews.Where(x => x.Id == vpMaster_Id && x.IsActive == true).ToList();
                    return profile;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public VacancyMasDtlViewModel GetVpDProfileViews(int mId, string username, string hfmisCode)
        {
            using (var _db = new HR_System())
            {

                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var vpMaster = _db.VpMProfileViews.FirstOrDefault(x => x.Id == mId);
                    var vpDetails = _db.VpDProfileViews.Where(x => x.Master_Id == vpMaster.Id).ToList();
                    var vpProfiles = _db.VPProfileViews.Where(x => x.VPMaster_Id == vpMaster.Id).ToList();
                    if (username.Equals("dpd") || username.Equals("so.toqeer"))
                    {
                        var vpMasterLogs = _db.VpMasterLogs.Where(x => x.VpMaster_Id == vpMaster.Id).OrderBy(k => k.DateTime).ToList();
                        var vpDetailLogs = _db.VpDetailLogs.Where(x => x.VpMaster_Id == vpMaster.Id).OrderBy(k => k.Id).ToList();
                        var emls = _db.Entity_Modified_Log_View.Where(x => x.Entity_Lifecycle_Id == vpMaster.EntityLifeCycle_Id).OrderBy(k => k.Modified_Date).ToList();
                        return new VacancyMasDtlViewModel()
                        {
                            vpMaster = vpMaster,
                            vpDetails = vpDetails,
                            vpProfiles = vpProfiles,
                            vpMasterLogs = vpMasterLogs,
                            vpDetailLogs = vpDetailLogs,
                            emls = emls
                        };
                    }
                    return new VacancyMasDtlViewModel() { vpMaster = vpMaster, vpDetails = vpDetails };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public List<VacancyViewModel> VPReport(string userId, string userHftypecode, string userHfmisCode, string hfmisCode)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    string query;
                    string southUserId = "028654f4-ee5a-4016-a600-f5579705e310";
                    bool southUser = false;
                    
                    _db.Configuration.ProxyCreationEnabled = false;

                    if (userHfmisCode == "0" && userHftypecode != null)
                    {
                        query = $@" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like @param and substring(HFMISCode,13,3) = '{userHftypecode}' group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc";
                    }
                    else if (userId.Equals(southUserId))
                    {
                            query = $@" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like '031%' or HFMISCode Like '032%'  or HFMISCode Like '036%'
                                     group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc";
                            var report1 = _db.Database.SqlQuery<VacancyViewModel>(query).ToList();
                            return report1;
                    }
                    else
                    {
                        query = @" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like @param group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc";
                    }
                    string CodeParam = string.Format("{0}%", hfmisCode);
                    var report = _db.Database.SqlQuery<VacancyViewModel>(query, new SqlParameter("@param", CodeParam)).ToList();
                    return report;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<VacancyViewModel> VPReportDetail(string userId, string userName, string roleName, string userHftypecode, string type, string userHfmisCode, string hfmisCode, string clickType, List<int> designationIds)
        {
            using (var _db = new HR_System())
            {
                try
                {

                    string query = "";
                    _db.Configuration.ProxyCreationEnabled = false;

                    var districtClause = string.Empty;
                    var desigsClause = string.Empty;
                    var phfmcClause = string.Empty;
                    if (designationIds.Count > 0)
                    {
                        desigsClause = $" and desg_Id in ({ string.Join(",", designationIds)}) ";
                    }
                    //else if (roleName.Equals("PHFMC"))
                    //{
                    //    var phfmcDesignationIds = _db.PHFMC_Designations.Select(k => k.Id).ToList();
                    //    desigsClause = $" and desg_Id in ({ string.Join(",", phfmcDesignationIds)}) ";
                    //}
                    if (roleName.Equals("PHFMC"))
                    {
                        var hfmisCodes = _db.HFListPs.Where(x => x.HFAC == 2 && x.HFMISCode.StartsWith(hfmisCode)).Select(k => k.HFMISCode).ToList();
                        phfmcClause = $" and HFMISCode in ('{ string.Join("','", hfmisCodes)}') ";
                    }
                    if (clickType.Equals("s"))
                    {
                        clickType = string.Empty;
                    }
                    else if (clickType.Equals("f"))
                    {
                        clickType = $" and TotalWorking > 0 ";
                    }
                    else if (clickType.Equals("v"))
                    {
                        clickType = $" and Vacant > 0 ";
                    }
                    if (type.Equals("Designation"))
                    {
                        if (userHfmisCode == "0" && userHftypecode != null)
                        {
                            query = $@" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,sum(Adhoc) as Adhoc,sum(Contract) as Contract,sum(DailyWages) as DailyWages,sum(Regular) as Regular,sum(PHFMC) as PHFMC,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like @param and substring(HFMISCode,13,3) = '{userHftypecode}' {desigsClause} {clickType} {phfmcClause} group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc, DsgName";
                        }
                        else
                        {
                            query = $@" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, CadreName, BPS, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,sum(Adhoc) as Adhoc,sum(Contract) as Contract,sum(DailyWages) as DailyWages,sum(Regular) as Regular,sum(PHFMC) as PHFMC,
                                    sum(Profiles) as TotalProfile,CadreName FROM VpMProfileView where HFMISCode Like @param {desigsClause} {clickType} {phfmcClause} group by DsgName,BPS,desg_Id,
                                    CadreName order by TotalSanctioned desc, DsgName";
                        }
                    }
                    if (type.Equals("Facility"))
                    {
                        if (userHfmisCode == "0" && userHftypecode != null)
                        {
                            query = $@" select distinct(HF_Id) as HfId,HFName as HfFullName,HFMISCode, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,sum(Adhoc) as Adhoc,sum(Contract) as Contract,sum(DailyWages) as DailyWages,sum(Regular) as Regular,sum(PHFMC) as PHFMC,
                                    sum(Profiles) as TotalProfile FROM VpMProfileView where HFMISCode Like @param and substring(HFMISCode,13,3) = '{userHftypecode}'  {desigsClause} {clickType} {phfmcClause} group by HF_Id,HFName,HFMISCode
                                    order by HFName";
                        }
                        else
                        {
                            query = $@" select distinct(HF_Id) as HfId,HFName as HfFullName,HFMISCode, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,sum(Adhoc) as Adhoc,sum(Contract) as Contract,sum(DailyWages) as DailyWages,sum(Regular) as Regular,sum(PHFMC) as PHFMC,
                                    sum(Profiles) as TotalProfile FROM VpMProfileView where HFMISCode Like @param  {desigsClause} {clickType} {phfmcClause} group by HF_Id,HFName,HFMISCode
                                    order by HFName";
                        }
                    }

                    if (type.Equals("DesignationFacility"))
                    {
                        if (userHfmisCode == "0" && userHftypecode != null)
                        {
                            query = $@" select desg_Id as DesignationID,DsgName as DesignationName, CadreName, BPS, HF_Id as HfId,HFName as HfFullName,HFMISCode, TotalSanctioned, TotalWorking, Vacant as  TotalVacant,Adhoc,Contract,DailyWages,Regular,PHFMC,
                                    sum(Profiles) as TotalProfile FROM VpMProfileView where HFMISCode Like @param {desigsClause} {clickType} and substring(HFMISCode,13,3) = '{userHftypecode}' {phfmcClause} order by HFName, DsgName";
                        }
                        else
                        {
                            query = $@" select  desg_Id as DesignationID,DsgName as DesignationName, CadreName, BPS, HF_Id as HfId,HFName as HfFullName,HFMISCode, TotalSanctioned, TotalWorking, Vacant as TotalVacant,Adhoc,Contract,DailyWages,Regular,PHFMC,
                                    Profiles as TotalProfile FROM VpMProfileView where HFMISCode Like @param {desigsClause} {clickType} {phfmcClause} order by HFName, DsgName";
                        }
                    }
                    string CodeParam = string.Format("{0}%", hfmisCode);
                    var report = _db.Database.SqlQuery<VacancyViewModel>(query, new SqlParameter("@param", CodeParam)).ToList();

                    return report;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public async Task<VpMProfileView> SaveVacancy(VPMaster vpm, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                using (var transc = _db.Database.BeginTransaction())
                {

                    try
                    {
                        if (vpm.Id != 0)
                        {
                            var changed = false;
                            string description = "";
                            VpMasterLog vpMasterLog = new VpMasterLog();
                            _db.Configuration.ProxyCreationEnabled = false;
                            var vpmDb = _db.VPMasters.FirstOrDefault(x => x.Id == vpm.Id);
                            vpMasterLog.VpMaster_Id = vpmDb.Id;
                            vpMasterLog.Action_Id = 2;
                            int? TotalWorkingAllBefore = _db.VPDetails.Where(x => x.Master_Id == vpmDb.Id).Sum(x => x.TotalWorking);

                            if (vpmDb.TotalSanctioned != vpm.TotalSanctioned)
                            {
                                int TotalSanctionedBefore = vpmDb.TotalSanctioned;
                                vpmDb.TotalSanctioned = vpm.TotalSanctioned;
                                _db.Entry(vpmDb).State = EntityState.Modified;
                                _db.SaveChanges();

                                description = "TotalSanctionedBefore: " + TotalSanctionedBefore + ",TotalSanctionedAfter: " + vpmDb.TotalSanctioned;

                                vpMasterLog.SanctionedBefore = TotalSanctionedBefore;
                                vpMasterLog.SanctionedAfter = vpmDb.TotalSanctioned;

                                changed = true;
                            }
                            foreach (var vpDetail in vpm.VPDetails.ToList())
                            {
                                var vpDetailDb = _db.VPDetails.FirstOrDefault(x => x.Master_Id == vpmDb.Id && x.EmpMode_Id == vpDetail.EmpMode_Id);

                                if (vpDetailDb == null)
                                {
                                    var vpDetailNew = new VPDetail();
                                    vpDetailNew.Master_Id = vpmDb.Id;
                                    vpDetailNew.EmpMode_Id = vpDetail.EmpMode_Id;
                                    vpDetailNew.TotalWorking = vpDetail.TotalWorking;
                                    _db.VPDetails.Add(vpDetailNew);
                                    _db.SaveChanges();

                                    Entity_Lifecycle eld = new Entity_Lifecycle();
                                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                                    eld.Created_By = userName;
                                    eld.Users_Id = userId;
                                    eld.IsActive = true;
                                    eld.Entity_Id = 3;
                                    _db.Entity_Lifecycle.Add(eld);
                                    _db.SaveChanges();

                                    vpDetailNew.EntityLifecycle_Id = eld.Id;
                                    _db.Entry(vpDetailNew).State = EntityState.Modified;
                                    _db.SaveChanges();
                                    changed = true;
                                }
                                else
                                {
                                    VpDetailLog vpDetailLog = new VpDetailLog();
                                    vpDetailLog.VpMaster_Id = vpmDb.Id;
                                    vpDetailLog.VpDetail_Id = vpDetailDb.Id;
                                    vpDetailLog.Action_Id = 2;
                                    vpDetailLog.EmpMode_Id = vpDetailDb.EmpMode_Id;

                                    int? TotalWorkingBefore = vpDetailDb.TotalWorking;
                                    if (TotalWorkingBefore != vpDetail.TotalWorking)
                                    {
                                        vpDetailDb.TotalWorking = vpDetail.TotalWorking;
                                        _db.Entry(vpDetailDb).State = EntityState.Modified;
                                        _db.SaveChanges();

                                        if (vpDetailDb.EntityLifecycle_Id == null)
                                        {
                                            Entity_Lifecycle elc = new Entity_Lifecycle();
                                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                            elc.Created_By = userName;
                                            elc.Users_Id = userId;
                                            elc.IsActive = true;
                                            elc.Entity_Id = 3;
                                            _db.Entity_Lifecycle.Add(elc);
                                            _db.SaveChanges();
                                            vpDetailDb.EntityLifecycle_Id = elc.Id;
                                        }

                                        Entity_Modified_Log eml = new Entity_Modified_Log();
                                        eml.Modified_By = userId;
                                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                        eml.Entity_Lifecycle_Id = (long)vpDetailDb.EntityLifecycle_Id;
                                        eml.Description = "TotalWorkingBefore: " + TotalWorkingBefore + ",TotalWorkingAfter: " + vpDetail.TotalWorking;
                                        _db.Entity_Modified_Log.Add(eml);
                                        _db.SaveChanges();


                                        vpDetailLog.TotalWorkingBefore = TotalWorkingBefore;
                                        vpDetailLog.TotalWorkingAfter = vpDetail.TotalWorking;
                                        _db.VpDetailLogs.Add(vpDetailLog);
                                        _db.SaveChanges();
                                        changed = true;
                                    }
                                }
                            }

                            int? TotalWorkingAfter = _db.VPDetails.Where(x => x.Master_Id == vpmDb.Id).Sum(x => x.TotalWorking);
                            TotalWorkingAfter = TotalWorkingAfter == null ? 0 : TotalWorkingAfter;
                            vpmDb.TotalWorking = vpmDb.TotalWorking == null ? 0 : vpmDb.TotalWorking;
                            int vacantNow = 0;
                            int vacantBefore = 1;
                            if (vpmDb.TotalWorking != TotalWorkingAfter)
                            {
                                vacantBefore = (int)(vpmDb.TotalSanctioned - vpmDb.TotalWorking);
                                vpmDb.TotalWorking = TotalWorkingAfter;
                                _db.SaveChanges();
                                vacantNow = (int)(vpmDb.TotalSanctioned - vpmDb.TotalWorking);

                                if (!string.IsNullOrEmpty(description))
                                {
                                    description += "::";
                                    description = "TotalWorkingAllBefore: " + TotalWorkingAllBefore + ",TotalWorkingAllAfter: " + vpmDb.TotalWorking;
                                }
                                else
                                {
                                    description = "TotalWorkingAllBefore: " + TotalWorkingAllBefore + ",TotalWorkingAllAfter: " + vpmDb.TotalWorking;
                                }
                                vpMasterLog.FilledBefore = TotalWorkingAllBefore;
                                vpMasterLog.FilledAfter = vpmDb.TotalWorking;


                                if (vacantBefore <= 0 && vacantNow > 0)
                                {

                                    var subscriptions = _db.VpSubscribers.Where(x => x.VPMaster_Id == vpmDb.Id).ToList();

                                    foreach (var subs in subscriptions)
                                    {
                                        var profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.Id == subs.Profile_Id);

                                        var vpMaster = _db.VpMastProfileViews.FirstOrDefault(x => x.Id == subs.VPMaster_Id);



                                        string emailBody = @"
                                <p>Vacant Seat Alert</p>
                                <p><strong> " + vpMaster.DsgName + " - " + vpMaster.HFName + @"</strong >:-</p>
                                        <p>Dear, <strong>" + profile.EmployeeName + @"</strong></p>
                                                <p><strong>You can now apply for " + vpMaster.DsgName + " at " + vpMaster.HFName + @" </p>
                                                         <p></p>
                                                               <p><a href =""https://hrmis.pshealthpunjab.gov.pk"" target=""_blank"">Click Here To Apply for Transfer </a></p>
                                        
                                                                        <p> &nbsp;</p>
                                           
                                                                           <p><strong> Regards,</strong ></p>
                                                
                                                                                <p><span style ='text-decoration: underline;'><em>Health Information and Service Delivery Unit </em></span></p>
                                                         
                                                                                            <p><span style ='text-decoration: underline;'><em>Primary & Secondary Healthcare Department </em></span></p>
                                                                             ";
                                        Common.Common.SendEmail("belalmughal@gmail.com", "Vacant Seat Alert - " + vpMaster.HFName + " - " + vpMaster.DsgName, emailBody);

                                        string MessageBody = @"Vacant Seat Alert\n" + vpMaster.DsgName + "\n" + vpMaster.HFName + "\nYou can now apply for " + vpMaster.DsgName + " at " + vpMaster.HFName + "\n\nRegards,\nHealth Information and Services Delivery Unit.\nPrimary and Secondary Healthcare Department.";

                                        SMS sms = new SMS()
                                        {
                                            UserId = userId,
                                            FKId = (int)vpMaster.Id,
                                            MobileNumber = profile.MobileNo,
                                            Message = MessageBody
                                        };
                                        Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                                        t.Start();
                                        //await Common.Common.SendSMSTelenor(sms);
                                    }
                                }



                                changed = true;
                            }
                            else if (TotalWorkingAfter == null)
                            {
                                vpmDb.TotalWorking = 0;
                                _db.SaveChanges();
                                changed = true;
                            }

                            int countProfiles = 0;

                            foreach (var vpProfile in vpm.VPProfiles.ToList())
                            {
                                Entity_Lifecycle eld = new Entity_Lifecycle();
                                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                                eld.Created_By = userName;
                                eld.Users_Id = userId;
                                eld.IsActive = true;
                                eld.Entity_Id = 3;
                                _db.Entity_Lifecycle.Add(eld);
                                _db.SaveChanges();

                                vpProfile.EntityLifecycle_Id = eld.Id;
                                _db.VPProfiles.Add(vpProfile);
                                _db.SaveChanges();
                                changed = true;
                                countProfiles++;
                                vpMasterLog.Remarks = countProfiles + " Profile Filled";
                            }


                            if (changed)
                            {
                                var elcDb = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == vpmDb.EntityLifecycle_Id);
                                elcDb.Last_Modified_By = userName;
                                _db.Entry(elcDb).State = EntityState.Modified;
                                _db.SaveChanges();

                                if (vpmDb.EntityLifecycle_Id == null)
                                {
                                    Entity_Lifecycle elc = new Entity_Lifecycle();
                                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                    elc.Created_By = userName;
                                    elc.Users_Id = userId;
                                    elc.IsActive = true;
                                    elc.Entity_Id = 3;
                                    _db.Entity_Lifecycle.Add(elc);
                                    _db.SaveChanges();
                                    vpmDb.EntityLifecycle_Id = elc.Id;
                                }

                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)vpmDb.EntityLifecycle_Id;
                                eml.Description = description;
                                _db.Entity_Modified_Log.Add(eml);

                                vpMasterLog.DateTime = DateTime.UtcNow.AddHours(5);
                                vpMasterLog.CreatedBy = userName;
                                vpMasterLog.User_Id = userId;
                                vpMasterLog.IsActive = true;
                                _db.VpMasterLogs.Add(vpMasterLog);
                                _db.SaveChanges();

                            }
                            var res = _db.VpMProfileViews.FirstOrDefault(x => x.Id == vpmDb.Id);
                            if (res.TotalWorking > res.TotalSanctioned)
                            {
                                Common.Common.EmailToMe(userName, userId, "Vacancy Anamoly Occured");
                            }
                            transc.Commit();
                            return res;
                        }
                        else
                        {
                            Entity_Lifecycle el = new Entity_Lifecycle();
                            el.Created_Date = DateTime.UtcNow.AddHours(5);
                            el.Created_By = userName;
                            el.Users_Id = userId;
                            el.IsActive = true;
                            el.Entity_Id = 3;
                            vpm.Entity_Lifecycle = el;

                            List<VPDetail> vpdetailsList = vpm.VPDetails.ToList();
                            vpm.VPDetails = null;

                            _db.VPMasters.Add(vpm);
                            _db.SaveChanges();

                            VpMasterLog vpMasterLog = new VpMasterLog();
                            vpMasterLog.VpMaster_Id = vpm.Id;
                            vpMasterLog.Action_Id = 1;


                            foreach (VPDetail vpdetail in vpdetailsList)
                            {
                                vpdetail.Master_Id = vpm.Id;
                                Entity_Lifecycle eld = new Entity_Lifecycle();
                                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                                eld.Created_By = userName;
                                eld.Users_Id = userId;
                                eld.IsActive = true;
                                eld.Entity_Id = 3;
                                vpdetail.Entity_Lifecycle = eld;

                                //VpDetailLog vpDetailLog = new VpDetailLog();
                                //vpDetailLog.VpMaster_Id = vpm.Id;
                                //vpDetailLog.Action_Id = 1;
                                //vpDetailLog.EmpMode_Id = vpdetail.EmpMode_Id;
                            }
                            vpm.TotalWorking = vpdetailsList.Sum(x => x.TotalWorking);
                            _db.Entry(vpm).State = EntityState.Modified;
                            _db.VPDetails.AddRange(vpdetailsList);
                            _db.SaveChanges();

                            vpMasterLog.SanctionedAfter = vpm.TotalSanctioned;
                            vpMasterLog.FilledAfter = vpm.TotalWorking;
                            vpMasterLog.DateTime = DateTime.UtcNow.AddHours(5);
                            vpMasterLog.CreatedBy = userName;
                            vpMasterLog.User_Id = userId;
                            vpMasterLog.IsActive = true;

                            _db.VpMasterLogs.Add(vpMasterLog);
                            _db.SaveChanges();



                            var res = _db.VpMProfileViews.FirstOrDefault(x => x.Id == vpm.Id);
                            if (res.TotalWorking > res.TotalSanctioned)
                            {
                                Common.Common.EmailToMe(userName, userId, "Vacancy Anamoly Occured @ " + res.Id);
                            }

                            transc.Commit();

                            return res;
                        }
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }

        public VPHolder SaveVacancyHolder(VPHolder vpHolder, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (vpHolder.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;

                            Entity_Lifecycle eld = new Entity_Lifecycle();

                            eld.Created_Date = DateTime.UtcNow.AddHours(5);
                            eld.Created_By = userName;
                            eld.Users_Id = userId;
                            eld.IsActive = true;
                            eld.Entity_Id = 5967;

                            _db.Entity_Lifecycle.Add(eld);
                            _db.SaveChanges();

                            vpHolder.Elc_Id = eld.Id;

                            _db.VPHolders.Add(vpHolder);

                            _db.SaveChanges();

                            transc.Commit();

                            return vpHolder;
                        }
                        else
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var vpHolderDb = _db.VPHolders.FirstOrDefault(x => x.Id == vpHolder.Id);
                            if (vpHolderDb == null) { return null; }
                            vpHolderDb.ESR = vpHolder.ESR;
                            vpHolderDb.ELR = vpHolder.ELR;
                            vpHolderDb.OrderNumber = vpHolder.OrderNumber;

                            _db.Entry(vpHolderDb).State = EntityState.Modified;
                            _db.SaveChanges();

                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = userId;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)vpHolderDb.Elc_Id;
                            eml.Description = "Order Generated on Approved Vacancy Or Vacancy Status Modified";
                            _db.Entity_Modified_Log.Add(eml);
                            _db.SaveChanges();
                            transc.Commit();
                            return vpHolderDb;
                        }
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }


        public bool RemoveVacancy(int id, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    VPMaster vpMaster = db.VPMasters.Find(id);
                    if (vpMaster == null)
                    {
                        return false;
                    }
                    var applicaitons = db.ApplicationMasters.Where(x => x.VpMaster_Id == id).Count();
                    var holders = db.VPHolders.Where(x => x.VpMaster_Id == id).Count();
                    if (applicaitons > 0 || holders > 0)
                    {
                        return false;
                    }
                    List<VPDetail> vpDetails = db.VPDetails.Where(x => x.Master_Id == vpMaster.Id).ToList();
                    foreach (VPDetail vpd in vpDetails)
                    {
                        db.VPDetails.Remove(vpd);
                    }
                    db.VPMasters.Remove(vpMaster);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
        public bool DuplicateVacancy(VPMaster vpm, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    int vpmCount = db.VPMasters.Where(x => x.HF_Id == vpm.HF_Id && x.Desg_Id == vpm.Desg_Id && x.PostType_Id == vpm.PostType_Id).Count();
                    if (vpmCount > 0)
                    {
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
        public DataTable GetData(string sql)
        {
            var table = new DataTable();
            using (var db = new HR_System())
            {
                using (IDbCommand command = db.Database.Connection.CreateCommand())
                {
                    try
                    {
                        db.Database.Connection.Open();
                        command.CommandText = sql;
                        command.CommandTimeout = command.Connection.ConnectionTimeout;

                        using (System.Data.IDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                    finally
                    {
                        db.Database.Connection.Close();
                    }
                }
            }

            return table;
        }
        public List<object> GetDataFromDataTable(DataTable table)
        {
            List<object> data = new List<object>();
            foreach (DataRow item in table.Rows)
            {
                List<object> row = new List<object>();
                foreach (DataColumn column in table.Columns)
                {
                    row.Add(item[column].ToString());
                }
                data.Add(row);
            }
            return data;
        }
    }

    public class VpGeoDto
    {
        public long Id { get; set; }
        public int TotalSanctioned { get; set; }
        public int TotalWorking { get; set; }
        public int Vacant { get; set; }
        public double Percent { get; set; }
        public int TotalApprovals { get; set; }
        public Nullable<bool> Locked { get; set; }
        public int Profiles { get; set; }
        public int WorkingProfiles { get; set; }
        public string HFMISCode { get; set; }
        public int HF_Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string HFName { get; set; }
        public Nullable<int> HFAC { get; set; }
        public int PostType_Id { get; set; }
        public string PostTypeName { get; set; }
        public int Desg_Id { get; set; }
        public string DsgName { get; set; }
        public Nullable<int> BPSWorking { get; set; }
        public Nullable<int> Cadre_Id { get; set; }
        public string CadreName { get; set; }
        public string ImagePath { get; set; }
        public Nullable<int> BPS { get; set; }
        public Nullable<int> BPS2 { get; set; }
        public Nullable<int> HFTypeId { get; set; }
        public string HFTypeCode { get; set; }
        public string HFTypeName { get; set; }
        public Nullable<long> EntityLifeCycle_Id { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Last_Modified_By { get; set; }
        public string Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public string Users_Id { get; set; }
        public Nullable<int> Adhoc { get; set; }
        public Nullable<int> Contract { get; set; }
        public Nullable<int> Regular { get; set; }
        public Nullable<int> PHFMC { get; set; }
        public Nullable<int> OPS { get; set; }
    }
}