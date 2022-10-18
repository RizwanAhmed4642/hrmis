using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hrmis.Models.Services
{
    public class OnlineJobService
    {
        public List<uspJobApplicantPreferences_Result> GetJobApplicantPreferences(JobFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.uspJobApplicantPreferences(filter.JobId, filter.BatchNo).ToList();

                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrApplicationView> GetHrApplications(JobFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.HrApplicationViews.Where(x => x.DesignationId == filter.JobId && x.IsActive == true).ToList();

                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public HrApplicationView GetHrApplication(int id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.HrApplicationViews.FirstOrDefault(x => x.Id == id && x.IsActive == true);

                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<ApplicantView> GetJobs(JobFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.ApplicantViews.AsQueryable();

                    var count = res.Count();
                    var data = res.OrderBy(x => x.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();

                    return new TableResponse<ApplicantView>() { Count = count, List = data };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<JobBatchesView> GetJobBatches(int designationId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var job = db.JobsViews.FirstOrDefault(x => x.Designation_Id == designationId);
                    if (job != null)
                    {
                        var res = db.JobBatchesViews.Where(x => x.JobId == job.Id).ToList();
                        return res;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<JobApplicantsView> GetJobApplicants(JobFilter filter)
        {
            try
            {
                using (HR_System db = new HR_System())
                {
                    var query = db.JobApplicationsViews.Where(x => x.Job_Id == filter.JobId && x.IsActive == true).AsQueryable();

                    List<int> Ids = null;

                    //var job = db.JobswithDesignations.Where(x => x.Id == filter.JobId).FirstOrDefault();
                    //var reqExp = job.Req_Experience_Years;
                    //var applicantExp = db.ApplicantExperiences.Where(x => x.Applicant_Id == filter.ApplicantId).ToList();

                    if (!string.IsNullOrEmpty(filter.BatchNo))
                    {
                        Ids = query.Where(x => x.JobBatch_No == filter.BatchNo).Select(x => x.Id).ToList();
                    }
                    var applicants = db.JobApplicantsViews.AsQueryable();
                    applicants = applicants.Where(x => Ids.Contains(x.Id));
                    var total = applicants.Count();
                    var list = applicants.OrderBy(x => x.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<JobApplicantsView>() { List = list, Count = total };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ApplicantView GetJobApplicant(int applicantId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.ApplicantViews.FirstOrDefault(x => x.Id == applicantId);
                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<ApplicantQualificationview> GetJobApplicantQualifications(int applicantId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.ApplicantQualificationviews.Where(x => x.Applicant_Id == applicantId).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<ApplicantExperience> GetJobApplicantExperiences(int applicantId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.ApplicantExperiences.Where(x => x.Applicant_Id == applicantId).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<JobApplicantPrefView> GetJobApplicantPrefs(int applicantId, int jobId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var res = db.JobApplicantPrefViews.Where(x => x.Applicant_Id == applicantId && x.Job_Id == jobId).ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }

}