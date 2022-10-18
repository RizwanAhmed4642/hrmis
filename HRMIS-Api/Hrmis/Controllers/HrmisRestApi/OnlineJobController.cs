using Hrmis.Models.Common;
using Hrmis.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/OnlineJob")]
    public class OnlineJobController : ApiController
    {
        private OnlineJobService _onlineJobService;
        public OnlineJobController()
        {
            _onlineJobService = new OnlineJobService();
        }
        [HttpPost]
        [Route("GetJobApplicantPreferences")]
        public IHttpActionResult GetJobApplicantPreferences([FromBody] JobFilter filter)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicantPreferences(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetHrApplications")]
        public IHttpActionResult GetHrApplications([FromBody] JobFilter filter)
        {
            try
            {
                return Ok(_onlineJobService.GetHrApplications(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHrApplication/{id}")]
        public IHttpActionResult GetHrApplication(int id)
        {
            try
            {
                return Ok(_onlineJobService.GetHrApplication(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetJobBatches/{designationId}")]
        public IHttpActionResult GetJobBatches(int designationId)
        {
            try
            {
                return Ok(_onlineJobService.GetJobBatches(designationId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetJobApplicants")]
        public IHttpActionResult GetJobApplicants([FromBody] JobFilter filter)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicants(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetJobApplicant/{applicantId}")]
        public IHttpActionResult GetJobApplicant(int applicantId)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicant(applicantId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetJobApplicantQualifications/{applicantId}")]
        public IHttpActionResult GetJobApplicantQualifications(int applicantId)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicantQualifications(applicantId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetJobApplicantExperiences/{applicantId}")]
        public IHttpActionResult GetJobApplicantExperiences(int applicantId)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicantExperiences(applicantId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetJobApplicantPreferences/{applicantId}/{jobId}")]
        public IHttpActionResult GetJobApplicantPreferences(int applicantId, int jobId)
        {
            try
            {
                return Ok(_onlineJobService.GetJobApplicantPrefs(applicantId, jobId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    public class JobFilter : Paginator
    {
        public int JobId { get; set; }
        public string BatchNo { get; set; }
    }
}