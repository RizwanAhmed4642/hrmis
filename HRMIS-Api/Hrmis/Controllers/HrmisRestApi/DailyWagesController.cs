using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Hrmis.Models.Services.DailyWagesService;
using Hrmis.Models.Dto;
using System.Security.Cryptography;


namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/DailyWagesProfile")]
    public class DailyWagesController : ApiController
    {

        public readonly DailyWagesService _dailyprofileService;
        public readonly UserLogsSErvice _userLogsSErvice;

        public DailyWagesController()
        {
            _dailyprofileService = new DailyWagesService();
            _userLogsSErvice = new UserLogsSErvice();

        }

        [HttpPost]
        [Route("SaveProfile")]
        public IHttpActionResult SaveProfile([FromBody] DailyWagesProfileClass hrProfile)
        {
            try
            {
                object profile;
                if (hrProfile.Id > 0)
                {
                    profile = _dailyprofileService.UpdateDailyWagerById(hrProfile, User.Identity.GetUserName(), User.Identity.GetUserId());
                }
                else
                {
                    profile = _dailyprofileService.AddDailyWagerProfile(hrProfile, User.Identity.GetUserName(), User.Identity.GetUserId());
                }
                if (profile == null) {
                    return Ok(new { Status = false, Message = "CNIC Alreay Exist", Data = "" });
                } 
                else {
                    return Ok(new { Status = true , Message = "Saved Successfully" , Data = profile });

                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                return Ok(new { Status = false, Message = ex.Message, Data = "" });

            }
        }

        [HttpPost]
        [Route("GetDailyWagesInPool")]
        public IHttpActionResult GetDailyWagesInPool(ProfileFilters filters)
        {
            try
            {
                
                var res =_dailyprofileService.GetDailyWagesInPool(filters);
                return Ok(new { Status = true , Message = "" , List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); 
                return Ok(new { Status = false, Message = ex.Message, List = "" });
                //return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetDailyWages")]
        public IHttpActionResult GetDailyWages(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                if (User.IsInRole("PACP"))
                {
                    filters.roleName = "PACP";
                }
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                var res = _dailyprofileService.GetDailyWages(filters);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }

        [HttpPost]
        [Route("GetDailyWagesCount")]
        public IHttpActionResult GetDailyWagesCount(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                if (User.IsInRole("PACP"))
                {
                    filters.roleName = "PACP";
                }
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                var res = _dailyprofileService.GetDailyWagesCount(filters);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }


        [HttpGet]
        [Route("GetDailyWagerbyId/{wagerId}")]
        public IHttpActionResult GetDailyWagerbyId(int wagerId)
        {
            try
            {
                var res = _dailyprofileService.GetDailyWagerbyId(wagerId);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }
        [HttpPost]
        [Route("AddContractFileById")]
        public IHttpActionResult AddContractFileById([FromBody] ContractFileDTO obj)
        {
            try
            {
                var res = _dailyprofileService.AddContractFileById(obj.Id, obj.imageFile);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }

        [HttpGet]
        [Route("GetCorrdinates/{name}/{Type}")]
        public IHttpActionResult GetCorrdinates(string name = null, string Type = null)
        {

            try
            {
                var res = _dailyprofileService.GetCooridinate(name, Type);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }





        }
        [HttpPost]
        //[AllowAnonymous]
        [Route("GetDailyWagesMapCount")]
        public IHttpActionResult GetDailyWagesMapCount(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                if (User.IsInRole("PACP"))
                {
                    filters.roleName = "PACP";
                }
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                var res = _dailyprofileService.GetDailyWagesForMap(filters);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("GetDailyWagesDistrictWise")]
        public IHttpActionResult GetDailyWagesDistrictWise(ProfileFilters filters)
        {
            try
            {


                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                if (User.IsInRole("PACP"))
                {
                    filters.roleName = "PACP";
                }
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                var res = _dailyprofileService.GetDailyWagesDistrict(filters);
                return Ok(new { Status = true, Message = "", List = res });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, List = "" });
            }
        }



       
        [HttpGet]
        [Route("GetProfileById/{Id}")]
        public IHttpActionResult GetProfileById(int Id)
        {
            try
            {

                var res = _dailyprofileService.GetProfileById(Id);
                return Ok(new { Status = true, Message = "", Profile = res });
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(new { Status = false, Message = ex.Message, Profile = "" });
            }





        }

		[HttpGet]
		[Route("GetDailyDesignationbyName/{name}")]
		public IHttpActionResult GetDailyDesignationbyName(string name)
		{
			try
			{

				var res = _dailyprofileService.GetDesignationList(name);
				return Ok(new { Status = true, Message = "", List = res });
			}
			catch (Exception ex)
			{

				Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
				return Ok(new { Status = false, Message = ex.Message, Profile = "" });
			}





		}

	}


}
