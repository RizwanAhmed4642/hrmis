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
    [RoutePrefix("Common")]
    public class LocationController : ApiController
    {

        private static HR_System db = new HR_System();

        public LocationController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        [Route("GetDivisions/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDivisions(string code)
        {
            return Ok(ReturnDivisions(code));
        }

        [Route("GetDistricts/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDistricts(string code)
        {
            return Ok(ReturnDistricts(code));
        }

        [Route("GetTehsils/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTehsils(string code)
        {
            return Ok(ReturnTehsils(code));
        }

        [Route("DetailList/{HfmisCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilitiesDetailList(string HfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfListy = db.HFLists.Where(x => (x.HFMISCode.StartsWith(HfmisCode) || x.HFMISCode.Equals(HfmisCode)) && x.IsActive == true).ToList();
                return Ok(new Result<List<HFList>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = hfListy
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<List<HFList>>()
                {
                    Type = ResultType.Success.ToString(),
                    exception = ex
                });
            }
        }


        public static Result<List<Division>> ReturnDivisions(string code)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var divisions = db.Divisions.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToList();
                return new Result<List<Division>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = divisions
                };
            }
            catch (Exception ex)
            {
                return new Result<List<Division>>()
                {
                    Type = ResultType.Success.ToString(),
                    exception = ex
                };
            }

        }

        public static Result<List<District>> ReturnDistricts(string code)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var districts = db.Districts.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToList();

                return new Result<List<District>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = districts
                };
            }
            catch (Exception ex)
            {
                return new Result<List<District>>()
                {
                    Type = ResultType.Success.ToString(),
                    exception = ex
                };
            }
        }

        public static Result<List<Tehsil>> ReturnTehsils(string code)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var tehsils = db.Tehsils.Where(x => x.Code.StartsWith(code) || x.Code.Equals(code)).ToList();
                return new Result<List<Tehsil>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = tehsils
                };
            }
            catch (Exception ex)
            {
                return new Result<List<Tehsil>>()
                {
                    Type = ResultType.Success.ToString(),
                    exception = ex
                };
            }
        }


    }


}
