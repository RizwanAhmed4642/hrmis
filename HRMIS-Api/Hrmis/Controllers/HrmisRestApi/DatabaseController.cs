using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Database")]
    public class DatabaseController : ApiController
    {
        private readonly DatabaseService _databaseService;

        public DatabaseController()
        {
            _databaseService = new DatabaseService();
        }

        [HttpPost]
        [Route("GetVacancyMapHrmis")]
        public IHttpActionResult GetVacancyMapHrmis([FromBody] MapFilters mapFilters)
        {
            try
            {
                //using (var _db = new HR_System())
                //{
                //    _db.Configuration.ProxyCreationEnabled = false;

                //    var vpQuery = _db.VpGeoViews.Where(x => x.HFMISCode.StartsWith(mapFilters.HfmisCode) && mapFilters.DesignationIds.Contains(x.Desg_Id)).AsQueryable();
                //    if (mapFilters.HFTypeCodes.Count > 0)
                //    {
                //        vpQuery = vpQuery.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                //    }

                //    var vp = new List<VpGeoDto>();
                //        vp = vpQuery.GroupBy(x => new { x.HFMISCode, x.HFName, x.Desg_Id, x.DsgName, x.ImagePath }).Select(k => new VpGeoDto
                //        {
                //            Code = k.Key.HFMISCode,
                //            Name = k.Key.HFName,
                //            DsgName = k.Key.DsgName,
                //            //Latitude = k.Key.HFLatitude,
                //            //Longitude = k.Key.HFLongitude,
                //            ImagePath = k.Key.ImagePath,
                //            TotalSanctioned = k.Sum(s => s.TotalSanctioned),
                //            TotalWorking = k.Sum(s => s.TotalWorking),
                //            Vacant = k.Sum(s => s.Vacant),
                //            Profiles = k.Sum(s => s.Profiles),
                //            WorkingProfiles = k.Sum(s => s.WorkingProfiles)
                //        }).ToList();
                //    return Ok(vp);
                //}
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //var vpQuery = db.VpGeoViews.Where(x => x.HFMISCode.StartsWith(mapFilters.HfmisCode) && mapFilters.DesignationIds.Contains(x.Desg_Id) && !x.DistrictName.Equals("Islamabad")).AsQueryable();

                    //if (mapFilters.HFTypeCodes.Count > 0)
                    //{
                    //    vpQuery = vpQuery.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                    //}
                    //var districts = vpQuery.GroupBy(x => new { x.DistrictCode, x.DistrictName }).Select(x => new DistrictsVacancyModel
                    //{
                    //    Code = x.Key.DistrictCode,
                    //    Name = x.Key.DistrictName,
                    //    Count = x.Sum(l => l.Vacant)
                    //}).ToList();

                    var vpQuery2 = db.VpGeoViews.Where(x => x.HFMISCode.StartsWith(mapFilters.HfmisCode) && mapFilters.DesignationIds.Contains(x.Desg_Id)).AsQueryable();
                    if (mapFilters.HFTypeCodes.Count > 0)
                    {
                        vpQuery2 = vpQuery2.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                    }

                    var vp = new List<VpGeoDto>();
                    vp = vpQuery2.GroupBy(x => new { x.HF_Id, x.HFMISCode, x.HFName, x.HFLatitude, x.HFLongitude, x.Desg_Id, x.DsgName, x.ImagePath }).Select(k => new VpGeoDto
                    {
                        HF_Id = k.Key.HF_Id,
                        Code = k.Key.HFMISCode,
                        Name = k.Key.HFName,
                        DsgName = k.Key.DsgName,
                        Latitude = k.Key.HFLatitude,
                        Longitude = k.Key.HFLongitude,
                        ImagePath = k.Key.ImagePath,
                        TotalSanctioned = k.Sum(s => s.TotalSanctioned),
                        TotalWorking = k.Sum(s => s.TotalWorking),
                        Vacant = k.Sum(s => s.Vacant),
                        Percent = (k.Sum(s => s.TotalWorking) / k.Sum(s => s.TotalSanctioned)) * 100,
                        Profiles = k.Sum(s => s.Profiles),
                        Adhoc = k.Sum(s => s.Adhoc),
                        Regular = k.Sum(s => s.Regular),
                        OPS = k.Sum(s => s.OnPayScale),
                        WorkingProfiles = k.Sum(s => s.WorkingProfiles)
                    }).OrderBy(x => x.DsgName).ToList();
                    return Ok(new {/* districts, */ vp });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetVacancyChartHrmis")]
        public IHttpActionResult GetVacancyChartHrmis([FromBody] MapFilters mapFilters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var vpQuery2 = db.VpGeoViews.Where(x => x.HFMISCode.StartsWith(mapFilters.HfmisCode)).AsQueryable();

                    if (mapFilters.DesignationIds.Count == 0)
                    {
                        return Ok(false);
                    }
                    if (mapFilters.DesignationIds.Count > 0)
                    {
                        vpQuery2 = vpQuery2.Where(x => mapFilters.DesignationIds.Contains(x.Desg_Id)).AsQueryable();
                    }
                    if (mapFilters.HFTypeCodes.Count > 0)
                    {
                        vpQuery2 = vpQuery2.Where(x => mapFilters.HFTypeCodes.Contains(x.HFTypeCode)).AsQueryable();
                    }
                    var vp = new List<VpGeoDto>();
                    vp = vpQuery2.GroupBy(x => new { x.HF_Id, x.HFMISCode, x.HFName, x.Desg_Id, x.DsgName, x.ImagePath }).Select(k => new VpGeoDto
                    {
                        HF_Id = k.Key.HF_Id,
                        Code = k.Key.HFMISCode,
                        Name = k.Key.HFName,
                        DsgName = k.Key.DsgName,
                        ImagePath = k.Key.ImagePath,
                        TotalSanctioned = k.Sum(s => s.TotalSanctioned),
                        TotalWorking = k.Sum(s => s.TotalWorking),
                        Vacant = k.Sum(s => s.Vacant),
                        Percent = (k.Sum(s => s.TotalWorking) / k.Sum(s => s.TotalSanctioned)) * 100,
                        Profiles = k.Sum(s => s.Profiles),
                        Adhoc = k.Sum(s => s.Adhoc),
                        Regular = k.Sum(s => s.Regular),
                        OPS = k.Sum(s => s.OnPayScale),
                        WorkingProfiles = k.Sum(s => s.WorkingProfiles)
                    }).OrderBy(x => x.DsgName).ToList();
                    return Ok(new { vp });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetVPChartCount")]
        public IHttpActionResult GetVPChartCount([FromBody] MapFilters mapFilters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var vpChartCount = db.VPChartsCounts.FirstOrDefault(x => x.HFMISCode.StartsWith(mapFilters.HfmisCode) && x.DesignationId == mapFilters.DesignationId && x.IsActive == true);
                    return Ok(vpChartCount);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveVPChartCount")]
        public IHttpActionResult SaveVPChartCount([FromBody] VPChartsCount vPChartsCount)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var vpChartCountDb = db.VPChartsCounts.FirstOrDefault(x => x.HFMISCode.StartsWith(vPChartsCount.HFMISCode) && x.DesignationId == vPChartsCount.DesignationId && x.IsActive == true);
                    if (vpChartCountDb != null)
                    {
                        vpChartCountDb.IsActive = false;
                        vpChartCountDb.CreatedBy = User.Identity.GetUserName();
                        db.Entry(vpChartCountDb).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    vPChartsCount.IsActive = true;
                    vPChartsCount.CreatedBy = User.Identity.GetUserName();
                    vPChartsCount.UserId = User.Identity.GetUserId();
                    vPChartsCount.Datetime = DateTime.UtcNow.AddHours(5);
                    db.VPChartsCounts.Add(vPChartsCount);
                    db.SaveChanges();
                    return Ok(vPChartsCount);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetServiceList")]
        public IHttpActionResult GetServiceList([FromBody] DatabaseService.DatabaseFilter filter)
        {
            try
            {
                return Ok(_databaseService.GetServiceList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCords")]
        public IHttpActionResult GetCords([FromBody] AddressDetails filters)
        {
            try
            {

                if (!string.IsNullOrEmpty(filters.Address))
                {

                    List<Example> result = new List<Example>();
                    string url = @"https://maps.googleapis.com/maps/api/geocode/json?address= " + filters.Address + "  &key=AIzaSyBfi4jgjhyEOI_OWskLmc51XhTI7hV-SjU";

                    WebRequest request = WebRequest.Create(url);
                    request.Credentials = CredentialCache.DefaultCredentials;

                    WebResponse response = request.GetResponse();
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<List<Example>>(responseFromServer);
                    }
                    response.Close();
                }
                return null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        //get designation
        [HttpPost]
        [Route("GetDesignationList")]
        public IHttpActionResult GetDesignationList([FromBody] DatabaseService.DatabaseFilter filter)
        {
            try
            {
                return Ok(_databaseService.GetDesignationList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        //end get designation

        //get designationlist for dropdown
        [HttpGet]
        [Route("GetDesignationsDDL")]
        public IHttpActionResult GetDesignationsDDL()
        {
            try
            {
                return Ok(_databaseService.GetDesignations());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }



        //get designation search
        [HttpPost]
        [Route("GetDesigSearchList")]
        public IHttpActionResult GetDesigSearchList([FromBody] DatabaseService.DatabaseFilter filter)
        {
            try
            {
                return Ok(_databaseService.GetDesigSearchList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        //end get designation

        //getHfTypesList
        //[HttpPost]
        //[Route("GetHfTypeList")]
        //public IHttpActionResult GetHfTypeList([FromBody] DatabaseService.DatabaseFilter filter)
        //{
        //    try
        //    {
        //        return Ok(_databaseService.GetHfTypeList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        //getHfCategoryList
        [HttpPost]
        [Route("GetHfCategoryList")]
        public IHttpActionResult GetHfCategoryList([FromBody] DatabaseService.DatabaseFilter filter)
        {
            try
            {
                return Ok(_databaseService.GetHfCategoryList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveService")]
        public IHttpActionResult SaveService([FromBody] Service service)
        {
            try
            {
                var s = _databaseService.addService(service, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (s == null) return Ok("Invalid");
                return Ok(s);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetVpProfileStatus/{type}")]
        public IHttpActionResult GetVpProfileStatus(string type)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (type.Equals("HrProfileStatus"))
                    {
                        var hrProfileStatus = _db.HrProfileStatus.Where(x => !string.IsNullOrEmpty(x.Name)).OrderBy(k => k.Name).ToList();
                        return Ok(hrProfileStatus);
                    }
                    if (type.Equals("VpProfileStatus"))
                    {
                        var hrProfileStatus = _db.HrProfileStatus.Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
                        var vpProfileStatus = _db.VpProfileStatus.ToList();
                        return Ok(vpProfileStatus);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("AddVpProfileStatus/{status_Id}")]
        public IHttpActionResult AddVpProfileStatus(int status_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbObj = _db.VpProfileStatus.FirstOrDefault(x => x.ProfileStatus_Id == status_Id);
                    if (dbObj != null)
                    {
                        dbObj.ProfileStatus_Id = status_Id;
                        dbObj.DateTime = DateTime.UtcNow.AddHours(5);
                        dbObj.CreatedBy = User.Identity.GetUserName();
                        dbObj.User_Id = User.Identity.GetUserId();
                        dbObj.IsActive = true;
                        _db.SaveChanges();
                        return Ok("Already Exist");
                    }
                    VpProfileStatu vpProfileStatu = new VpProfileStatu();

                    vpProfileStatu.ProfileStatus_Id = status_Id;
                    vpProfileStatu.DateTime = DateTime.UtcNow.AddHours(5);
                    vpProfileStatu.CreatedBy = User.Identity.GetUserName();
                    vpProfileStatu.User_Id = User.Identity.GetUserId();
                    vpProfileStatu.IsActive = true;

                    _db.VpProfileStatus.Add(vpProfileStatu);
                    _db.SaveChanges();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHFOpen")]
        public IHttpActionResult SaveHFOpen([FromBody] HFOpenedPosting hFOpenedPosting)
        {
            try
            {
                var res = _databaseService.SaveOpenedHF(hFOpenedPosting, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetOpenHF")]
        public IHttpActionResult GetOpenHF()
        {
            try
            {
                var res = _databaseService.GetOpenHF(User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAdhocOpenHF")]
        public IHttpActionResult GetAdhocOpenHF()
        {
            try
            {
                var res = _databaseService.GetAdhocOpenHF(User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveOpenHF/{Id}")]
        public IHttpActionResult RemoveLeaveRecord(int Id)
        {
            try
            {
                var res = _databaseService.RemoveOpenHF(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveAdhocOpenHF/{Id}")]
        public IHttpActionResult RemoveAdhocOpenHF(int Id)
        {
            try
            {
                var res = _databaseService.RemoveAdhocOpenHF(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveVpProfileStatus/{status_Id}")]
        public IHttpActionResult RemoveVpProfileStatus(int status_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var vpProfileStatus = _db.VpProfileStatus.FirstOrDefault(x => x.ProfileStatus_Id == status_Id);
                    if (vpProfileStatus != null)
                    {
                        vpProfileStatus.IsActive = false;
                        _db.SaveChanges();
                        return Ok(true);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHfType")]
        public IHttpActionResult SaveHfType([FromBody] HFType hftype)
        {
            try
            {

                var hft = _databaseService.addHfType(hftype, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (hft == null) return BadRequest("Invalid");
                return Ok(hft);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("SaveMeritActiveDesignation")]
        public IHttpActionResult SaveMeritActiveDesignation([FromBody] MeritActiveDesignation MADesignation)
        {
            try
            {

                var mactivedesignation = _databaseService.addmeritActiveDesignation(MADesignation, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (mactivedesignation == null) return BadRequest("Invalid");
                return Ok(mactivedesignation);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("SaveDesignation")]
        public IHttpActionResult SaveDesignation([FromBody] HrDesignation desig)
        {
            try
            {
                if (_databaseService.DesigExistsByName(desig.Name))
                {
                    return Ok("Duplicate");
                }
                var dsg = _databaseService.addDesig(desig, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (dsg == null) return Ok("Invalid");
                return Ok(dsg);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("RemoveDesig")]
        public IHttpActionResult RemoveDesig([FromBody] HrDesignation desig)
        {
            try
            {
                var dsg = _databaseService.RemoveDesig(desig, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (dsg == null) return BadRequest("Invalid");
                return Ok(dsg);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }
    }

    public class MapFilters
    {
        public List<int> DesignationIds { get; set; }
        public string HfmisCode { get; set; }
        public List<string> HFTypeCodes { get; set; }
        public int showProfileViewId { get; set; }
        public int DesignationId { get; set; }
    }
    public class DesignationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HrScale_Id { get; set; }
        public int Cadre_Id { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IList<string> types { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Hrmis.Controllers.HrmisRestApi.Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Example
    {
        public IList<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public bool partial_match { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }
    }


    public class AddressDetails
    {
        public string Address { get; set; }
    }
}