using Hrmis.Models.Common;
using Hrmis.Models.CustomModels;
using Hrmis.Models.DbModel;
using Hrmis.Models.ImageProcessor;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Zen.Barcode;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [RoutePrefix("api/TransferAndPosting")]
    public class TransferAndPostingController : ApiController
    {
        private HR_System db;
        private TransferPostingService _transferPostingService;

        public TransferAndPostingController()
        {
            db = new HR_System();
            _transferPostingService = new TransferPostingService();
        }

        [Route("GetESR/{district}/{transferTypeId}")]
        [HttpGet]
        public IHttpActionResult GetESR(string district, int transferTypeId)
        {

            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            db.Configuration.ProxyCreationEnabled = false;
            List<ESRView> list = new List<ESRView>();
            if (userName.Equals("dpd"))
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    list = db.ESRViews.Where(x => x.elcIsActive == true && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    list = db.ESRViews.Where(x => x.Created_By.Equals(district) && x.elcIsActive == true && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    list = db.ESRViews.Where(x => x.Created_By.Equals(district) && x.TransferTypeID == transferTypeId && x.elcIsActive == true && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    list = db.ESRViews.Where(x => x.elcIsActive == true && x.Users_Id.Equals(userId) && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    list = db.ESRViews.Where(x => x.Created_By.Equals(district) && x.elcIsActive == true && x.Users_Id.Equals(userId) && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    list = db.ESRViews.Where(x => x.Created_By.Equals(district) && x.TransferTypeID == transferTypeId && x.elcIsActive == true && x.Users_Id.Equals(userId) && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.Created_Date).ToList();
                }
                else
                {
                    return Ok(false);
                }
            }
            return Ok(list);
        }
        [Route("GetESRReport/{district}/{transferTypeId}/{currentPage}/{itemsPerPage}")]
        [HttpPost]
        public IHttpActionResult GetESRReport([FromBody]ProfileQuery query, string district, int transferTypeId, int currentPage = 1, int itemsPerPage = 100)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            db.Configuration.ProxyCreationEnabled = false;

            List<OrderReportOverallView> report = new List<OrderReportOverallView>();
            IQueryable<OrderReportOverallView> reportQuery = null;
            int totalRecords = 0;


            if (currentPage <= 0) currentPage = 1;
            if (itemsPerPage <= 0) currentPage = 100;


            if (userName.Equals("dpd") || User.IsInRole("Deputy Secretary"))
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else
                {
                    return Ok(false);
                }
            }

            if (!string.IsNullOrEmpty(query?.Query))
            {
                reportQuery = reportQuery.Where(x => x.EmployeeName.ToLower().Contains(query.Query)
    || x.CNIC.Equals(query.Query)
    || x.Designation.Contains(query.Query));
            }

            totalRecords = reportQuery.Count();
            report = reportQuery.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).OrderByDescending(x => x.DateTime).ToList();

            return Ok(new
            {
                esrlist = report,
                totalRecords
            });
        }
        [Route("GetESRReportDropsDowns/{district}/{transferTypeId}")]
        [HttpGet]
        public IHttpActionResult GetESRReportDropsDowns(string district, int transferTypeId)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            db.Configuration.ProxyCreationEnabled = false;
            List<string> transferTypes = new List<string>();

            List<OrderReportOverallView> report = new List<OrderReportOverallView>();
            IQueryable<OrderReportOverallView> reportQuery = null;


            List<string> districtsList = new List<string>();
            if (userName.Equals("dpd") || User.IsInRole("Deputy Secretary"))
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                if (district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId == 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else if (!district.Equals("0") && transferTypeId != 0)
                {
                    reportQuery = db.OrderReportOverallViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                }
                else
                {
                    return Ok(false);
                }
            }
            List<ItemCountViewModel> typeCount = reportQuery.GroupBy(c => c.OrderType).Select(x => new ItemCountViewModel { Name = x.Key, Count = x.Count() }).Distinct().ToList();
            typeCount.ForEach(u =>
            {
                transferTypes.Add(u.Name + " (" + u.Count + ")");
            });

            List<ItemCountViewModel> districtsCount = reportQuery.GroupBy(c => c.DistrictName).Select(x => new ItemCountViewModel { Name = x.Key, Count = x.Count() }).Distinct().ToList();
            districtsCount.ForEach(u =>
            {
                districtsList.Add(u.Name + " (" + u.Count + ")");
            });
            return Ok(new
            {
                transferTypes,
                districtsList
            });

        }
        [Route("SearchEsrReport/{district}")]
        [HttpPost]
        public IHttpActionResult SearchEsrReport(string district, [FromBody] ProfileQuery query)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                string userName = User.Identity.GetUserName();
                db.Configuration.ProxyCreationEnabled = false;
                IQueryable<ESRReportView> records = db.ESRReportViews.Where(x => x.EmployeeName.ToLower().Contains(query.Query)
               || x.CNIC.Equals(query.Query)
               || x.Designation.Contains(query.Query));
                List<ESRReportView> results = new List<ESRReportView>();
                if (userName.Equals("dpd"))
                {
                    if (district.Equals("0"))
                    {
                        results = records.Where(x => x.IsActive == true && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.DateTime).ToList();
                    }
                    else if (!district.Equals("0"))
                    {
                        results = records.Where(x => x.UserName.Equals(district) && x.IsActive == true && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.DateTime).ToList();
                    }
                }
                else
                {
                    if (district.Equals("0"))
                    {
                        results = records.Where(x => x.IsActive == true && x.UserId.Equals(userId) && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.DateTime).ToList();
                    }
                    else if (!district.Equals("0"))
                    {
                        results = records.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId) && !string.IsNullOrEmpty(x.OrderHTML) && !x.CNIC.Equals("3520189133751") && !x.CNIC.Equals("3520111111112")).OrderByDescending(x => x.DateTime).ToList();
                    }
                }
                return Ok(new { result = true, results = results });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetESRSummaryTotal")]
        [HttpGet]
        public IHttpActionResult GetESRSummaryTotal()
        {

            return Ok(db.ViewESRSummaryTotals.ToList());
        }
        [Route("RemoveESR/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveESR(int Id)
        {
            ESR esr = db.ESRs.FirstOrDefault(x => x.Id == Id);
            Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == esr.EntityLifecycle_Id);
            elc.IsActive = false;
            db.Entry(elc).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(true);
        }

        [Route("AddProfile")]
        [HttpPost]
        public IHttpActionResult AddProfile(HrProfile profile)
        {
            try
            {
                profile.HealthFacility = null;
                profile.CNIC = profile.CNIC.Replace("-", "");
                if (profile.MobileNo != null)
                {
                    profile.MobileNo = profile.MobileNo.Replace("-", "");
                }

                bool profileExisit = db.ProfileDetailsViews.Count(e => e.Id == profile.Id) > 0;
                if (profileExisit)
                {
                    HrProfile tempProfile = db.HrProfiles.FirstOrDefault(x => x.Id == profile.Id);
                    tempProfile.EmployeeName = profile.EmployeeName;
                    tempProfile.FatherName = profile.FatherName;
                    tempProfile.CNIC = profile.CNIC;
                    tempProfile.Designation_Id = profile.Designation_Id;
                    tempProfile.CurrentGradeBPS = db.HrDesignations.FirstOrDefault(x => x.Id == tempProfile.Designation_Id).HrScale_Id;
                    tempProfile.HealthFacility_Id = profile.HealthFacility_Id;
                    tempProfile.HfmisCode = profile.HfmisCode;
                    tempProfile.MobileNo = profile.MobileNo;
                    tempProfile.EMaiL = profile.EMaiL;
                    db.Entry(tempProfile).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { result = true, cnic = profile.CNIC });
                }
                else
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 9;

                    db.Entity_Lifecycle.Add(eld);
                    profile.EntityLifecycle_Id = eld.Id;
                    db.HrProfiles.Add(profile);
                    db.SaveChanges();
                    return Ok(new { result = true, cnic = profile.CNIC });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("PostESR/{hfmisCode}")]
        public IHttpActionResult PostESR([FromBody] List<ESR> listEsr, string hfmisCode)
        {
            if (!ModelState.IsValid || !listEsr.Any())
            {
                return BadRequest(ModelState);
            }
            var esr = listEsr.FirstOrDefault();

            try
            {
                HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == esr.Profile_Id);

                if (esr.TransferTypeID != null)
                {
                    //Mutual Transfer
                    if (esr.TransferTypeID == 1)
                    {
                        var esr2 = listEsr[1];

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
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile1 = new Entity_Modified_Log();
                        emlProfile1.Modified_By = User.Identity.GetUserId();
                        emlProfile1.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile1.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile1.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
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
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile2.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile2 = new Entity_Modified_Log();
                        emlProfile2.Modified_By = User.Identity.GetUserId();
                        emlProfile2.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile2.Entity_Lifecycle_Id = (long)profile2.EntityLifecycle_Id;
                        emlProfile2.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile2);
                        profile2.AddToEmployeePool = false;
                        db.Entry(profile2).State = EntityState.Modified;
                        db.SaveChanges();

                        //save esr2
                        Entity_Lifecycle eld2 = new Entity_Lifecycle();
                        eld2.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld2.Created_By = User.Identity.GetUserName();
                        eld2.Users_Id = User.Identity.GetUserId();
                        eld2.IsActive = true;
                        eld2.Entity_Id = 5;

                        db.Entity_Lifecycle.Add(eld2);
                        db.SaveChanges();


                        esr2.EntityLifecycle_Id = eld2.Id;
                        esr2.ResponsibleUser = eld2.Created_By;
                        db.ESRs.Add(esr2);
                        db.SaveChanges();

                    }
                    //At Disposal
                    if (esr.TransferTypeID == 2)
                    {

                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());

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
                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Suspend
                    if (esr.TransferTypeID == 3)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                        profile.HfmisCode = "0350020010030010002";
                        profile.HealthFacility_Id = 11606;

                        profile.WorkingHFMISCode = "0350020010030010002";
                        profile.WorkingHealthFacility_Id = 11606;
                        profile.Status_Id = 27;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // General Transfer
                    if (esr.TransferTypeID == 4)
                    {


                        _transferPostingService.UpdateVacancy(db, false, esr.HfmisCodeFrom, esr.DesignationFrom, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());

                        _transferPostingService.UpdateVacancy(db, true, esr.HfmisCodeTo, esr.DesignationTo, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());


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
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // Adhoc Appointment
                    if (esr.TransferTypeID == 8)
                    {

                        _transferPostingService.UpdateVacancy(db, true, esr.HfmisCodeTo, esr.DesignationTo, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());


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
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Termination
                    if (esr.TransferTypeID == 10)
                    {
                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());

                        profile.Status_Id = 28;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }

                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.Entry(profile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                eld.Created_By = User.Identity.GetUserName();
                eld.Users_Id = User.Identity.GetUserId();
                eld.IsActive = true;
                eld.Entity_Id = 5;

                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();


                esr.EntityLifecycle_Id = eld.Id;
                esr.ResponsibleUser = eld.Created_By;
                db.ESRs.Add(esr);
                profile.AddToEmployeePool = false;
                db.SaveChanges();

                var profileView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esr.Profile_Id);
                var EsrProfile = db.HrProfiles.FirstOrDefault(x => x.Id == esr.Profile_Id);
                if (EsrProfile != null)
                {

                    //EsrProfile.Status_Id = 30; // Setting Profile status to Pending Transfer
                    //db.SaveChanges();


                    SendSMS(profileView, hfmisCode);


                }
                Image barCode = barCodeZ(esr.Id);
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                return Ok(new { esr, imgSrc });

            }
            catch (DbEntityValidationException dbEx)
            {
                if (ESRExists(esr.Id))
                    return Conflict();
                else if (!ESRExists(esr.Id))
                    return NotFound();
                else
                {
                    return Ok(GetDbExMessage(dbEx));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("PostESRDetail")]
        public IHttpActionResult PostESRDetail([FromBody] ESRDetail esrDetail)
        {
            if (!ModelState.IsValid || esrDetail == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                db.ESRDetails.Add(esrDetail);
                db.SaveChanges();
                return Ok(new { esrDetail });
            }
            catch (DbEntityValidationException dbEx)
            {
                return Ok(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private void SendSMS(ProfileDetailsView EsrProfile, string hfmisCode)
        {
            if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Punjab Health Facilities and Management Company. Please visit http://pshealth.punjab.gov.pk/ for further details.";

                    List<SMS> smsy = new List<SMS>();
                    SMS sms = new SMS()
                    {
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody

                    };
                    smsy.Add(sms);

                    Common.SMS_Send(smsy);
                }
            }
            else
            {
                if (hfmisCode.Length == 1)
                {
                    if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                    {
                        string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";

                        List<SMS> smsy = new List<SMS>();
                        SMS sms = new SMS()
                        {
                            MobileNumber = EsrProfile.MobileNo,
                            Message = MessageBody

                        };
                        smsy.Add(sms);

                        Common.SMS_Send(smsy);
                    }
                }
                else if (hfmisCode.Length == 6)
                {
                    if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "" && EsrProfile.District != null)
                    {
                        string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at District Health Authority, " + EsrProfile.District + ".\nPlease visit http://pshealth.punjab.gov.pk/ for further details.";

                        List<SMS> smsy = new List<SMS>();
                        SMS sms = new SMS()
                        {
                            MobileNumber = EsrProfile.MobileNo,
                            Message = MessageBody

                        };
                        smsy.Add(sms);

                        Common.SMS_Send(smsy);
                    }
                }
            }
        }

        [Route("UploadNotingFile/{cnic}/{esrId}")]
        public async Task<IHttpActionResult> FileUpload(string cnic, int esrId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\NotingFiles\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";

                foreach (var file in provider.Contents)
                {
                    filename = Common.RandomString(7) + "_" + cnic + "_" + esrId + "_" + file.Headers.ContentDisposition.FileName.Trim('\"');
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


                }
                db.ESRs.Find(esrId).NotingFile = RootPath + @"\" + filename;
                db.SaveChanges();
                return Ok(new { result = true, src = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public Image barCodeZ(long Id)
        {
            Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
            BarcodeSymbology s = BarcodeSymbology.Code39NC;
            BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
            var metrics = drawObject.GetDefaultMetrics(50);
            metrics.Scale = 1;
            return drawObject.Draw("ESR-" + Id, metrics);
        }
        private string GetDbExMessage(DbEntityValidationException dbx) { return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors).Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}"); }
        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }
        protected override void Dispose(bool disposing) { if (disposing) { db.Dispose(); } base.Dispose(disposing); }
        private bool ESRExists(long id)
        {
            return db.ESRs.Count(e => e.Id == id) > 0;
        }
    }

    public class ItemCountViewModel
    {
        public string Name { get; set; }
        public int? Count { get; set; }
    }
}