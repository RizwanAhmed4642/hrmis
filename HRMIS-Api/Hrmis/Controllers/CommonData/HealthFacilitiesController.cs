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

namespace Hrmis.Controllers.CommonData
{
    [RoutePrefix("HealthFacilities")]
    public class HealthFacilitiesController : ApiController
    {
        private static HR_System db = new HR_System();

        public HealthFacilitiesController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        [Route("DetailList/{HfmisCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilitiesDetailList(string HfmisCode)
        {
            return Ok(ReturnHealthfacilitiesDetailList(HfmisCode));
        }



        public static Result<List<HFList>> ReturnHealthfacilitiesDetailList(string HfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfListy = db.HFLists.Where(x => (x.HFMISCode.StartsWith(HfmisCode) || x.HFMISCode.Equals(HfmisCode)) && x.IsActive == true).ToList();
                return new Result<List<HFList>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = hfListy
                };
            }
            catch (Exception ex)
            {
                return new Result<List<HFList>>()
                {
                    Type = ResultType.Success.ToString(),
                    exception = ex
                };
            }

        }

    }
}
