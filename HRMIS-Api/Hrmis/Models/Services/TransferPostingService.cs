using Hrmis.Models.DbModel;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Services
{
    public class TransferPostingService
    {


        public bool UpdateVacancy(HR_System db, bool isAddition, string hfmisCode, int? designationId, int? empModeId, string userName, string userId)
        {
            try
            {
                VPMaster vPMaster = null;
                VPDetail vPDetail = null;
                db.Configuration.ProxyCreationEnabled = false;
                var vPMasterQ = db.VPMasters.AsQueryable();
                var vPDetailQ = db.VPDetails.AsQueryable();

                vPMaster = vPMasterQ.Where(x => x.HFMISCode.Equals(hfmisCode) && x.Desg_Id == designationId)?.FirstOrDefault();

                if (vPMaster == null) return false;
                if(vPMaster.TotalSanctioned - vPMaster.TotalWorking <= 0 && isAddition == true)
                {
                    return false;
                }
                vPDetail = vPDetailQ.Where(x => x.Master_Id == vPMaster.Id && x.EmpMode_Id == empModeId)?.FirstOrDefault();

                if (isAddition)
                {
                    vPMaster.TotalWorking += 1;
                    if (vPDetail != null)
                    {
                        vPDetail.TotalWorking += 1;
                    }
                    else
                    {
                        var vpDetailNew = new VPDetail();
                        vpDetailNew.Master_Id = vPMaster.Id;
                        vpDetailNew.EmpMode_Id = (int) empModeId;
                        vpDetailNew.TotalWorking = 1;
                        db.VPDetails.Add(vpDetailNew);
                        db.SaveChanges();

                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld.Created_By = userName;
                        eld.Users_Id = userId;
                        eld.IsActive = true;
                        eld.Entity_Id = 3;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();

                        vpDetailNew.EntityLifecycle_Id = eld.Id;
                        db.Entry(vpDetailNew).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    vPMaster.TotalWorking = vPMaster.TotalWorking == 0 ? vPMaster.TotalWorking : vPMaster.TotalWorking - 1;
                    vPMaster.TotalApprovals = vPMaster.TotalApprovals == null ? 0 : vPMaster.TotalApprovals;
                    vPMaster.TotalApprovals = vPMaster.TotalApprovals == 0 ? 0 : (vPMaster.TotalApprovals - 1);
                    if (vPDetail != null)
                    {
                        vPDetail.TotalWorking = vPDetail.TotalWorking == 0 ? vPDetail.TotalWorking : vPDetail.TotalWorking - 1;
                        vPDetail.TotalApprovals = vPDetail.TotalApprovals == null ? 0 : vPDetail.TotalApprovals;
                        vPDetail.TotalApprovals = vPDetail.TotalApprovals == 0 ? 0 : (vPDetail.TotalApprovals - 1);
                    }
                    else
                    {
                        var vpDetailNew = new VPDetail();
                        vpDetailNew.Master_Id = vPMaster.Id;
                        vpDetailNew.EmpMode_Id = (int)empModeId;
                        vpDetailNew.TotalWorking = 1;
                        db.VPDetails.Add(vpDetailNew);
                        db.SaveChanges();

                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld.Created_By = userName;
                        eld.Users_Id = userId;
                        eld.IsActive = true;
                        eld.Entity_Id = 3;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();

                        vpDetailNew.EntityLifecycle_Id = eld.Id;
                        db.Entry(vpDetailNew).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();
                Entity_Modified_Log eml = new Entity_Modified_Log();

                if (vPMaster.EntityLifecycle_Id == null)
                {
                    Entity_Lifecycle eldd = new Entity_Lifecycle();
                    eldd.Created_Date = DateTime.UtcNow.AddHours(5);
                    eldd.Created_By = HttpContext.Current.User.Identity.GetUserName();
                    eldd.Users_Id = HttpContext.Current.User.Identity.GetUserId();
                    eldd.IsActive = true;
                    eldd.Entity_Id = 3;
                    db.SaveChanges();
                    vPMaster.Entity_Lifecycle = eldd;
                    eml.Modified_By = "System";
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Description = "Order Generated";
                }
                else
                {
                    eml.Modified_By = "System";
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Description = "Order Generated";
                    eml.Entity_Lifecycle_Id = (long)vPMaster.EntityLifecycle_Id;
                }
                db.Entity_Modified_Log.Add(eml);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}