using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using Hrmis.Models.CustomModels;
using Microsoft.AspNet.Identity;
using Hrmis.Models.Dto;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Hrmis.Models;

namespace Hrmis.Controllers.MSSApiControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/MSSProfiles")]
    public class ProfilesController : ApiController
    {
        private HR_System db = new HR_System();

        [Route("GetProfiles")]
        [HttpGet]
        public IHttpActionResult Profiles()
        {
            try
            {
                return Ok(db.ProfileThumbViews);
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Profiles/{CNIC}")]
        [HttpGet]
        public IHttpActionResult Profile(string cnic)
        {
            try
            {
                return Ok(db.ProfileThumbViews.FirstOrDefault(x => x.CNIC.Equals(cnic)));
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetDbExMessage(DbEntityValidationException dbx)
        {
            return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                .Aggregate("",
                    (current, validationError) =>
                        current +
                        $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
