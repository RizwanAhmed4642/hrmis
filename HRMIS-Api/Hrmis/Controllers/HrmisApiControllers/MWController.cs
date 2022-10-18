using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using Hrmis.Models.Dto;
using System;
using System.Web.Http;
using static Hrmis.Models.Services.MWRouteService;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("api/MWRoute")]
    public class MWController : ApiController
    {
        private readonly MWRouteService _mwRouteService;

        public MWController()
        {
            _mwRouteService = new MWRouteService();
        }

        [HttpGet]
        [Route("GetCities")]
        public IHttpActionResult GetCities()
        {
            try
            {
                var result = _mwRouteService.GetCities();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }

        [HttpGet]
        [Route("GetPointTypes")]
        public IHttpActionResult GetPointTypes()
        {
            try
            {
                var result = _mwRouteService.GetPointTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpPost]
        [Route("SaveRoute")]
        public IHttpActionResult SaveRoute(MWRouteDto mwRouteDto)
        {
            try
            {
                var result = _mwRouteService.SaveRoutes(mwRouteDto, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpGet]
        [Route("RemoveRoute")]
        public IHttpActionResult RemoveRoute(int Id)
        {
            try
            {
                var result = _mwRouteService.RemoveRoute(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }

        [HttpPost]
        [Route("SaveRouteDetail")]
        public IHttpActionResult SaveRouteDetail(MWRouteDetail mwRouteDetail)
        {
            try
            {
                var result = _mwRouteService.SaveRouteDetail(mwRouteDetail, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpGet]
        [Route("RemoveRouteDetail")]
        public IHttpActionResult RemoveRouteDetail(int Id)
        {
            try
            {
                var result = _mwRouteService.RemoveRouteDetail(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpGet]
        [Route("GetTagLocationsCount")]
        public IHttpActionResult GetTagLocationsCount()
        {
            try
            {
                var result = _mwRouteService.GetTagLocationsCount();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpGet]
        [Route("GetRouteDetail/{SourceId}/{DestinationId}")]
        public IHttpActionResult GetRouteDetails(int SourceId, int DestinationId)
        {
            try
            {
                var result = _mwRouteService.GetRouteDetails(SourceId, DestinationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
        [HttpGet]
        [Route("GetRoutes")]
        public IHttpActionResult GetRoutes()
        {
            try
            {
                var result = _mwRouteService.GetRoutes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new AppResponse<Exception>() { Data = ex, IsException = true, Messages = "Error" });
            }
        }
    }
}
