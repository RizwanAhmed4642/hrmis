using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using System.Data.Entity.Validation;
using Hrmis.Models.Dto;
using System;
using Microsoft.AspNet.Identity;

namespace Hrmis.Controllers.HrmisApiControllers
{
  
    [RoutePrefix("api/Central")]
    public class CentralController : ApiController
    {
        private HR_System db = new HR_System();

        public CentralController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        [Route("GetPrograms")]
        [HttpGet]
        public IHttpActionResult GetPrograms()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var prog = db.PSHealthPrograms.ToList().OrderBy(x => x.ProgramName);
                return Ok(prog);
            }
        }

        [Route("GetProjects")]
        [HttpGet]
        public IHttpActionResult GetProjects()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var proj = db.PSHealthProjects.ToList().OrderBy(x => x.ProjectName);
                return Ok(proj);
            }
        }

        [Route("GetProjects_ProgramView")]
        [HttpGet]
        public IHttpActionResult GetProjects_ProgramView()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var proj = db.Project_ProgramView.ToList().OrderBy(x => x.ProjectName);
                return Ok(proj);
            }
        }

        [Route("SubmitProject")]
        [HttpPost]
        public IHttpActionResult SubmitProject(PSHealthProject project)
        {
            using (var db = new HR_System())
            {
                try
                {
                    string userID = User.Identity.GetUserId();
                    string userName = User.Identity.GetUserName();

                    project.IsActive = true;
                    project.CreatedBy = userName;
                    project.CreatedBy_UserId = userID;
                    project.CreatedOn = DateTime.UtcNow.AddHours(5);

                    db.PSHealthProjects.Add(project);
                    db.SaveChanges();
                    return Ok(project);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("SubmitMHF/{projectId}/{typecode}")]
        [HttpGet]
        public IHttpActionResult SubmitMHF(int projectId, string typecode)
        {
            using (var db = new HR_System())
            {
                try
                {
                    string userID = User.Identity.GetUserId();
                    string userName = User.Identity.GetUserName();

                    var hfs = db.HealthFacilityDetails.Where(x => x.HFTypeCode.Equals(typecode)).ToList();
                    foreach (var hf in hfs)
                    {
                        PSHealthFacility phfnew = new PSHealthFacility();
                        phfnew.Project_Id = projectId;
                        phfnew.HealthFacility_Id = hf.Id;
                        phfnew.IsActive = true;
                        phfnew.CreatedBy = userName;
                        phfnew.CreatedBy_UserId = userID;
                        phfnew.CreatedOn = DateTime.UtcNow.AddHours(5);

                        db.PSHealthFacilities.Add(phfnew);
                    }
                    db.SaveChanges();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        [Route("GetProjectsCodewise/{code}")]
        [HttpGet]
        public IHttpActionResult GetProjectsCodewise(int code)
        {
            using (var db = new HR_System())
            {
                try
                {
                   db.Configuration.ProxyCreationEnabled = false;
                    var proj = db.PSHealthProjects.Where(x => x.Program_Id == code).ToList().OrderBy(x => x.ProjectName);
                    return Ok(proj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        [Route("submitProgram")]
        [HttpPost]
        public IHttpActionResult submitProgram(PSHealthProgram program)
        {
            using (var db = new HR_System())
            {
                try
                {
                    string userID = User.Identity.GetUserId();
                    string userName = User.Identity.GetUserName();

                    program.IsActive = true;
                    program.CreatedBy = userName;
                    program.CreatedBy_UserId = userID;
                    program.CreatedOn = DateTime.UtcNow.AddHours(5);
                    
                    db.PSHealthPrograms.Add(program);
                    db.SaveChanges();
                    return Ok(program);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }



    }
}