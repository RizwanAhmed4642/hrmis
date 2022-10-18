using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Hrmis.Models.Common;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using Phfmc.Models.Common;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/PER")]
    public class PERsController : ApiController
    {
        private HR_System db = new HR_System();

        public PERsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        [HttpGet]
        [Route("Get/{CurrentPage}")]
        // GET: PERs
        public async Task<IHttpActionResult> GetPERs(int CurrentPage)
        {

            int ItemsPerPage = 100;
            int ItemsToSkip = (CurrentPage - 1) * ItemsPerPage;

            var pers = db.PERs.Include(x => x.HrProfile).Select(x => new
            {
                x.Year,
                x.Id,
                x.HrProfile.EmployeeName,
                x.HrProfile.CNIC,
                DocDesignationName = x.HrProfile.HrDesignation.Name,
            }).OrderByDescending(x => x.Id).Skip(ItemsToSkip).Take(ItemsPerPage).ToList();


            return Ok(pers);
        }


        [HttpGet]
        [Route("QuantificationReport/{ProfileID}/{From}/{To}")]
        // GET: PERs
        public async Task<IHttpActionResult> GetQuantificationReport(int ProfileID, int From, int To)
        {
            var pers = db.PerViews.Where(x => x.HrProfile_Id == ProfileID).Where(x=>x.Scale == From || x.Scale == To).OrderBy(x => x.Scale).ToList();
            foreach (var item in pers)
            {
                item.Score = CalculatePerScore(item);
            }
            var result = pers.GroupBy(x => x.Scale).Select(x => new QunatificationVM { AggregateScore = 0, Scale = x.Key, Data = x.Select(y => y).ToList() }).ToList();
            foreach (var item in result)
            {
                decimal TotalScaleScore = 0;
                double TotalScalePerDuration = 0;
                foreach (var dataItem in item.Data)
                {
                    TotalScaleScore += dataItem.Score;
                    TotalScalePerDuration += (dataItem.ToPeriod - dataItem.FromPeriod).TotalDays;
                }
                TotalScalePerDuration = TotalScalePerDuration / 365; // converting days to years

                item.AggregateScore = ((TotalScaleScore * 10) / Convert.ToDecimal(TotalScalePerDuration));
                item.ScaleScoreTotal = TotalScaleScore;
                item.ScalePerTimePeriodTotal = TotalScalePerDuration;
            }

            return Ok(result);
            
        }

        [HttpGet]
        [Route("SynopsisReport/{ProfileID}")]
        // GET: PERs
        public async Task<IHttpActionResult> GetSynopsisReport(int ProfileID)
        {
            var synops = db.PerViews.Where(x => x.HrProfile_Id == ProfileID).OrderBy(x => x.Year).ThenBy(x => x.FromPeriod).ToList();
            return Ok(synops);
        }


        [HttpGet]
        [Route("WorkingPaper/{ProfileID}")]
        // GET: PERs
        public async Task<IHttpActionResult> GetWorkingPaper(int ProfileID)
        {

            var WorkingPaper = db.PerWrkngPprGrdsCnts.AsNoTracking().Where(x => x.HrProfile_Id == ProfileID).ToList();
            var WorkingPaperJSON = JsonConvert.SerializeObject(WorkingPaper);


            var esrs = db.PromotionEsrViews.Where(x => x.Profile_Id == ProfileID).ToList();
            int CurrentScaleESR = (int)esrs.OrderByDescending(x => x.BPSTo).FirstOrDefault().BPSTo;
            int PrevoiseScaleESR = CurrentScaleESR - 1;
            DateTime PromotedToCurrentScale = (DateTime)esrs.Where(x => x.BPSTo == CurrentScaleESR).FirstOrDefault().CreatedOn;
            DateTime PromotedToPrevoiseScale = (DateTime)esrs.Where(x => x.BPSTo == PrevoiseScaleESR).FirstOrDefault().CreatedOn;
            int InThePresentGrade = ((DateTime.UtcNow.AddHours(5) - PromotedToCurrentScale).Days / 365);
            var deferments = db.PerDeferments.Where(x => x.isDeleted == false && x.isResolved == false).ToList();
            var inquiries = db.PerInquiries.Where(x => x.isDeleted == false && x.isResolved == false).ToList();

            
            return Ok(new
            {
                WorkingPaper = WorkingPaperJSON,
                CurrentScaleESR = CurrentScaleESR,
                PrevoiseScaleESR = PrevoiseScaleESR,
                PromotedToCurrentScale = PromotedToCurrentScale,
                PromotedToPrevoiseScale = PromotedToPrevoiseScale,
                InThePresentGrade = InThePresentGrade,
                deferments = deferments,
                inquiries = inquiries
            });

        }


        [Route("Add")]
        [HttpPost]
        public async Task<IHttpActionResult> Add([FromBody] PER per)
        {
            try
            {
                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                eld.Created_By = User.Identity.GetUserName();
                eld.Users_Id = User.Identity.GetUserId();
                eld.IsActive = true;
                eld.Entity_Id = 23;
                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();

                per.EntityLifecycle_Id = (int)eld.Id;

                if (per.PerType_Id != 1)
                {
                    per.PerGradings = null;
                    per.PerPrfmAssments = null;
                    per.PerTrainings = null;
                    per.PerTargetAchievements = null;
                }

                db.PERs.Add(per);
                db.SaveChanges();
                return Ok(new { result = true, Id = per.Id });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }


        }

        //[Route("Update")]
        //[HttpPost]
        //public async Task<IHttpActionResult> Update([FromBody] PER per)
        //{

        //    try
        //    {


        //        //HR_System ctx = new HR_System();
        //        //var PerGradingslist = ctx.PerGradings.Where(x => x.Per_Id == per.Id).ToList();
        //        //var PerPrfmAssmentslist = ctx.PerPrfmAssments.Where(x => x.Per_Id == per.Id).ToList();
        //        //var PerTargetAchievementslist = ctx.PerTargetAchievements.Where(x => x.PER_Id == per.Id).ToList();
        //        //var PerTrainingslist = ctx.PerTrainings.Where(x => x.PER_Id == per.Id).ToList();
        //        //ctx.PerGradings.RemoveRange(PerGradingslist);
        //        //ctx.PerPrfmAssments.RemoveRange(PerPrfmAssmentslist);
        //        //ctx.PerTargetAchievements.RemoveRange(PerTargetAchievementslist);
        //        //ctx.PerTrainings.RemoveRange(PerTrainingslist);
        //        //ctx.SaveChanges();

        //        db.Entry(per).State = EntityState.Modified;
        //        db.SaveChanges();


        //        //db.PerGradings.RemoveRange(per.PerGradings);
        //        //db.PerPrfmAssments.RemoveRange(per.PerPrfmAssments);
        //        //db.PerTargetAchievements.RemoveRange(per.PerTargetAchievements);
        //        //db.PerTrainings.RemoveRange(per.PerTrainings);
        //        //db.SaveChanges();

        //        //db.PerGradings.AddRange(tempVar1.Select(c => { c.Per_Id = per.Id; return c; }).ToList());
        //        //db.PerPrfmAssments.AddRange(tempVar2.Select(c => { c.Per_Id = per.Id; return c; }).ToList());
        //        //db.PerTargetAchievements.AddRange(tempVar3.Select(c => { c.PER_Id = per.Id; return c; }).ToList());
        //        //db.PerTrainings.AddRange(per.PerTrainings);
        //        //db.SaveChanges();

        //        UpdateChildRelations(per);

        //        return Ok(new { result = true, Id = per.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(false);
        //    }


        //}
        [Route("Update")]
        [HttpPost]
        public async Task<IHttpActionResult> Update([FromBody] PER model)
        {

            try
            {
                var existingParent = db.PERs
        .Where(p => p.Id == model.Id)
        .Include(p => p.PerTrainings)
        .Include(p => p.PerPrfmAssments)
        .Include(p => p.PerTargetAchievements)
        .SingleOrDefault();

                if (existingParent != null)
                {
                    // Update parent
                    db.Entry(existingParent).CurrentValues.SetValues(model);

                    // Delete children
                    foreach (var existingChild in existingParent.PerTrainings.ToList())
                    {
                        if (!model.PerTrainings.Any(c => c.Id == existingChild.Id))
                            db.PerTrainings.Remove(existingChild);
                    }
                    foreach (var existingChild in existingParent.PerPrfmAssments.ToList())
                    {
                        if (!model.PerPrfmAssments.Any(c => c.Per_Id == existingChild.Per_Id && c.PA_Id == existingChild.PA_Id))
                            db.PerPrfmAssments.Remove(existingChild);
                    }
                    foreach (var existingChild in existingParent.PerTargetAchievements.ToList())
                    {
                        if (!model.PerTargetAchievements.Any(c => c.Id == existingChild.Id))
                            db.PerTargetAchievements.Remove(existingChild);
                    }


                    // Update and Insert children
                    foreach (var childModel in model.PerTrainings)
                    {
                        var existingChild = existingParent.PerTrainings
                            .Where(c => c.Id == childModel.Id && c.Id != 0)
                            .SingleOrDefault();

                        if (existingChild != null)
                            // Update child
                            db.Entry(existingChild).CurrentValues.SetValues(childModel);
                        else
                        {
                            // Insert child
                            var newChild = new PerTraining
                            {
                                FromPeriod = childModel.FromPeriod,
                                ToPeriod = childModel.ToPeriod,
                                Trainings_Id = childModel.Trainings_Id,
                                InstituteAndCountry = childModel.InstituteAndCountry,
                                PER_Id = childModel.PER_Id

                                //...
                            };
                            existingParent.PerTrainings.Add(newChild);
                        }
                    }

                    // Update and Insert children
                    foreach (var childModel in model.PerTargetAchievements)
                    {
                        var existingChild = existingParent.PerTargetAchievements
                            .Where(c => c.Id == childModel.Id && c.Id != 0)
                            .SingleOrDefault();

                        if (existingChild != null)
                            // Update child
                            db.Entry(existingChild).CurrentValues.SetValues(childModel);
                        else
                        {
                            // Insert child
                            var newChild = new PerTargetAchievement
                            {
                                TargetAchieved = childModel.TargetAchieved,
                                TargetFixed = childModel.TargetFixed,
                                FailureReason = childModel.FailureReason
                            };
                            existingParent.PerTargetAchievements.Add(newChild);
                        }
                    }

                    // Update and Insert children
                    foreach (var childModel in model.PerPrfmAssments)
                    {
                        var existingChild = existingParent.PerPrfmAssments
                            .Where(c => c.Per_Id == childModel.Per_Id && c.PA_Id == childModel.PA_Id)
                            .SingleOrDefault();

                        if (existingChild != null)
                            // Update child
                            db.Entry(existingChild).CurrentValues.SetValues(childModel);
                        else
                        {
                            // Insert child
                            var newChild = new PerPrfmAssment
                            {
                                Answer = childModel.Answer,
                                PA_Id = childModel.PA_Id,
                                Per_Id = childModel.Per_Id
                            };
                            existingParent.PerPrfmAssments.Add(newChild);
                        }
                    }

                    db.SaveChanges();
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        private void UpdateChildRelations(PER per)
        {


            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existing = db.PERs.FirstOrDefault(x => x.Id == per.Id);
                        var deletedTrng = existing.PerTrainings.Except(per.PerTrainings,
                                trng => trng.Id).ToList();
                        var addedTrng = per.PerTrainings.Except(existing.PerTrainings,
                                trng => trng.Id).ToList();
                        var updatedTeachers = existing.PerTrainings.Except(deletedTrng, tng => per.Id);

                        db.SaveChanges();
                        transc.Commit();
                    }
                    catch (Exception)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }

        }


        [Route("Detail/{id}")]
        [HttpGet]
        // GET: PERs/Details/5
        public async Task<IHttpActionResult> Detail(int? id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = true;
                var per = db.PerViews.Where(x => x.Id == id).FirstOrDefault();
                return Ok(per);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }


        //// POST: PERs/Delete/5
        //[HttpPost]
        //[Route("PER/Delete/{id}")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    PER pER = db.PERs.Find(id);
        //    db.PERs.Remove(pER);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        [Route("DoctorExists/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> DoctorExists(string CNIC)
        {
            try
            {
                int[] dArray = new int[] { 1320, 802, 1085, 21, 22 };
                int? profileID = db.HrProfiles.Where(x => x.CNIC == CNIC && dArray.Contains((int)x.Designation_Id)).Select(x=>x.Id).FirstOrDefault();

                if(profileID == null || profileID == 0)
                return Ok(false);
                else return Ok(profileID);
            }
            catch
            {
                return Ok(false);
            }
        }

        [Route("Doctor/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDoctor(string CNIC)
        {
            try
            {
                string query = @"SELECT * FROM ProfileDetailsView where Designation_Id in (1320,802,1085,21,22) and CNIC=@param";

                ProfileDetailsView profile = db.Database.SqlQuery<ProfileDetailsView>(query, new SqlParameter("@param", CNIC)).FirstOrDefault();

                return Ok(profile);
            }
            catch
            {
                return Ok(false);
            }
        }

        [Route("DoctorByID/{DoctorID}")]
        [HttpGet]
        public async Task<IHttpActionResult> DoctorByID(int DoctorID)
        {
            try
            {
                var profile = db.ProfileDetailsViews.Where(x => x.Id == DoctorID).FirstOrDefault();

                return Ok(profile);
            }
            catch
            {
                return Ok(false);
            }
        }

        [Route("Grading")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGrading()
        {

            var grading = db.Gradings.Select(x => new
            {
                x.Id,
                x.Name,
                x.SpecificSection,
                Gradings_Id = x.Id,
                GradingsVals_Id = 0

            }).ToList();

            return Ok(grading);
        }


        [Route("PrfmAssment")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPrfmAssment()
        {

            var prfmAssment = db.PrfmAssments.Select(x => new
            {
                x.Id,
                x.Name,
                x.PerPrfmAssments,
                PA_Id = x.Id,
                Answer = ""

            }).ToList();

            return Ok(prfmAssment);
        }

        [Route("Trainings")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTrainings()
        {

            var trainings = db.Trainings.ToList();

            return Ok(trainings);
        }

        [Route("PerTrainings/{perID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPerTrainings(int perID)
        {

            var trainings = db.PerTrainings.Include(x => x.Training).Where(x => x.PER_Id == perID).ToList();

            return Ok(trainings);
        }


        [Route("PerTargetAchievements/{perID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPerTargetAchievements(int perID)
        {

            var perTargetAchievements = db.PerTargetAchievements.Where(x => x.PER_Id == perID).ToList();

            return Ok(perTargetAchievements);
        }

        [Route("PerPrfmAssment/{perID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPerPrfmAssment(int perID)
        {

            var perPrfmAssment = db.PerPrfmAssments.Include(x => x.PrfmAssment).Where(x => x.Per_Id == perID).ToList();

            return Ok(perPrfmAssment);
        }
        [Route("PerGradings/{perID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPerGradings(int perID)
        {

            var perGradings = db.PerGradings.Include(x => x.Grading).Include(x => x.GradingVal).Where(x => x.Per_Id == perID)
                .Select(x => new
                {
                    x.Per_Id,
                    x.Gradings_Id,
                    x.GradingsVals_Id,
                    x.Grading.Name,
                    x.Grading.SpecificSection,
                    Value = x.GradingVal.Name

                }).ToList();

            return Ok(perGradings);
        }

        [Route("PromotionProfileBinidngData/{DoctorID}")]
        [HttpGet]
        public async Task<IHttpActionResult> PromotionProfileBinidngData(int DoctorID)
        {
            try
            {
                var data = new
                {
                    PERs = db.PerViews.Where(x => x.HrProfile_Id == DoctorID)
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.FromPeriod).ThenByDescending(x => x.ToPeriod)
                    .ToList(),
                    Promotions = db.PromotionEsrViews.Where(x => x.Profile_Id == DoctorID).OrderBy(x => x.CreatedOn).ToList()
                    
                };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }

        }


        [Route("LeaveTypes")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLeaveTypes()
        {
            try
            {
                return Ok(db.LeaveTypes.OrderBy(x=>x.LeaveType1).ToList());
            }
            catch (Exception ex)
            {
                return Ok(false);
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private decimal CalculatePerScore(PerView item)
        {
            var TotalMonths = item.ToPeriod.Subtract(item.FromPeriod).Days / (365.25 / 12);

            if (TotalMonths < 3) return 0;
            else if (TotalMonths > 11) return GetScoreByFullYear(item);
            else if (TotalMonths < 11 && TotalMonths >= 3)
            {
                return ((GetScoreByFullYear(item) * (Convert.ToDecimal((item.ToPeriod - item.FromPeriod).TotalDays))) / 365);
            }
            else return 0;
        }

        private decimal GetScoreByFullYear(PerView item)
        {


            if (item.OgByCoID == 1 && item.FitnessGradingValsID == 7) return 8; //VeryGood and Fit
            if (item.OgByCoID == 1 && item.FitnessGradingValsID == 8) return 0; //VeryGood and Un-Fit
            if (item.OgByCoID == 1 && item.FitnessGradingValsID == 9) return 10; //VeryGood and Fit for accelerated promotion
            if (item.OgByCoID == 1 && item.FitnessGradingValsID == 10) return 5; //VeryGood and Not yet fit for promotion

            if (item.OgByCoID == 11 && item.FitnessGradingValsID == 7) return 7; //Good and Fit
            if (item.OgByCoID == 11 && item.FitnessGradingValsID == 8) return 0; //Good and Un-Fit
            if (item.OgByCoID == 11 && item.FitnessGradingValsID == 9) return 7; //Good and Fit for accelerated promotion
            if (item.OgByCoID == 11 && item.FitnessGradingValsID == 10) return 5; //Good and Not yet fit for promotion

            if (item.OgByCoID == 2 && item.FitnessGradingValsID == 7) return 5; //Satisfactory and Fit
            if (item.OgByCoID == 2 && item.FitnessGradingValsID == 8) return 0; //Satisfactory and Un-Fit
            if (item.OgByCoID == 2 && item.FitnessGradingValsID == 9) return 5; //Satisfactory and Fit for accelerated promotion
            if (item.OgByCoID == 2 && item.FitnessGradingValsID == 10) return 5; //Satisfactory and Not yet fit for promotion

            if (item.OgByCoID == 3 && item.FitnessGradingValsID == 7) return 0; //Un-Satisfactory and Fit
            if (item.OgByCoID == 3 && item.FitnessGradingValsID == 8) return 0; //Un-Satisfactory and Un-Fit
            if (item.OgByCoID == 3 && item.FitnessGradingValsID == 9) return 0; //Un-Satisfactory and Fit for accelerated promotion
            if (item.OgByCoID == 3 && item.FitnessGradingValsID == 10) return 0; //Un-Satisfactory and Not yet fit for promotion

            if (item.OgByCoID == 12 && item.FitnessGradingValsID == 7) return 5; //Average and Fit
            if (item.OgByCoID == 12 && item.FitnessGradingValsID == 8) return 0; //Average and Un-Fit
            if (item.OgByCoID == 12 && item.FitnessGradingValsID == 9) return 5; //Average and Fit for accelerated promotion
            if (item.OgByCoID == 12 && item.FitnessGradingValsID == 10) return 5; //Average and Not yet fit for promotion
            return 0;
        }


        #region Inquiry Logic

        [Route("Inquiry/List/{DoctorID}")]
        [HttpGet]
        public async Task<IHttpActionResult> AddInquiry(int DoctorID)
        {
            return Ok(db.PerInquiries.Where(x => x.Profile_Id == DoctorID && x.isDeleted == false).OrderByDescending(x => x.Id).ToList());
        }

        [Route("Inquiry/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddInquiry(PerInquiry perInquiry)
        {
            try
            {
                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                eld.Created_By = User.Identity.GetUserName();
                eld.Users_Id = User.Identity.GetUserId();
                eld.IsActive = true;
                eld.Entity_Id = 466;
                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();

                perInquiry.Entity_Lifecycle = (int)eld.Id;

                db.PerInquiries.Add(perInquiry);
                db.SaveChanges();

                return Ok(new Result<List<PerInquiry>>()
                {

                    Type = ResultType.Success.ToString(),
                    Data = db.PerInquiries.Where(x => x.isDeleted == false).OrderByDescending(x => x.Id).ToList()

                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<string>()
                {
                    Type = ResultType.Exception.ToString(),
                    Message = "Oops!! something went wrong. Please try again later. Thank you.",
                    exception = ex
                });
            }
        }

        [Route("Inquiry/Update")]
        [HttpPost]
        public async Task<IHttpActionResult> Updateinquiry(PerInquiry perInquiry)
        {
            try
            {
                db.PerInquiries.Attach(perInquiry);
                db.Entry(perInquiry).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new Result<List<PerInquiry>>()
                {
                    Type = ResultType.Success.ToString()
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<string>()
                {
                    Type = ResultType.Exception.ToString(),
                    Message = "Oops!! something went wrong. Please try again later. Thank you.",
                    exception = ex
                });
            }
        }

        [Route("Inquiry/SetResolved/{Status}/{InquiryID}")]
        [HttpGet]
        public async Task<IHttpActionResult> SetResolved(Boolean Status, int InquiryID)
        {
            try
            {
                var inquiry = db.PerInquiries.Where(x => x.Id == InquiryID).FirstOrDefault();
                inquiry.isResolved = Status;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        [Route("Inquiry/Delete/{InquiryID}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteInquiry(int InquiryID)
        {
            try
            {
                var inquiry = db.PerInquiries.Where(x => x.Id == InquiryID).FirstOrDefault();
                inquiry.isDeleted = true;
                inquiry.DeletedOn = DateTime.UtcNow.AddHours(5);
                inquiry.DeletedBy = User.Identity.GetUserName();
                db.SaveChanges();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        #endregion

        #region Deferment Logic

        [Route("Deferment/List/{DoctorID}")]
        [HttpGet]
        public async Task<IHttpActionResult> AddDeferment(int DoctorID)
        {
            return Ok(db.PerDeferments.Where(x => x.Profile_Id == DoctorID && x.isDeleted == false).OrderByDescending(x => x.Id).ToList());
        }

        [Route("Deferment/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> AddDeferment(PerDeferment perDeferment)
        {
            try
            {
                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.UtcNow.AddHours(5);
                eld.Created_By = User.Identity.GetUserName();
                eld.Users_Id = User.Identity.GetUserId();
                eld.IsActive = true;
                eld.Entity_Id = 466;
                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();

                perDeferment.Entity_Lifecycle = (int)eld.Id;

                db.PerDeferments.Add(perDeferment);
                db.SaveChanges();

                return Ok(new Result<List<PerDeferment>>()
                {

                    Type = ResultType.Success.ToString(),
                    Data = db.PerDeferments.Where(x => x.isDeleted == false).OrderByDescending(x => x.Id).ToList()

                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<string>()
                {
                    Type = ResultType.Exception.ToString(),
                    Message = "Oops!! something went wrong. Please try again later. Thank you.",
                    exception = ex
                });
            }
        }

        [Route("Deferment/Update")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateDeferment(PerDeferment perDeferment)
        {
            try
            {
                db.PerDeferments.Attach(perDeferment);
                db.Entry(perDeferment).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new Result<List<PerDeferment>>()
                { 
                    Type = ResultType.Success.ToString()
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<string>()
                {
                    Type = ResultType.Exception.ToString(),
                    Message = "Oops!! something went wrong. Please try again later. Thank you.",
                    exception = ex
                });
            }
        }

        [Route("Deferment/SetResolved/{Status}/{DefermentID}")]
        [HttpGet]
        public async Task<IHttpActionResult> SetResolvedDeferment(Boolean Status, int DefermentID)
        {
            try
            {
                var Deferment = db.PerDeferments.Where(x => x.Id == DefermentID).FirstOrDefault();
                Deferment.isResolved = Status;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        [Route("Deferment/Delete/{DefermentID}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteDeferment(int DefermentID)
        {
            try
            {
                var Deferment = db.PerDeferments.Where(x => x.Id == DefermentID).FirstOrDefault();
                Deferment.isDeleted = true;
                Deferment.DeletedOn = DateTime.UtcNow.AddHours(5);
                Deferment.DeletedBy = User.Identity.GetUserName();
                db.SaveChanges();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        #endregion

    }

    public class QunatificationVM
    {


        public List<PerView> Data { get; set; }
        public decimal Scale { get; set; }
        public decimal AggregateScore { get; set; }
        public decimal ScaleScoreTotal { get; set; }
        public double ScalePerTimePeriodTotal { get; set; }
    }
}