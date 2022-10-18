using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private HR_System db = new HR_System();

        [Route("GetHFDashboardInfo/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetHFDashboardInfo(string hfmisCode = "")
        {
            try
            {
                HFDetail hfDetail = db.HFDetails.FirstOrDefault(x => x.HFMISCode.Equals(hfmisCode) && x.IsActive == true);
                //ProfileDetailsView hodProfile = db.ProfileDetailsViews.FirstOrDefault(x => (x.HfmisCode.Equals(hfmisCode) || x.HealthFacility_Id == hfDetail.Id) && (x.Designation_Id == 812 || x.WDesignation_Id == 812));
                ProfileDetailsView hodProfile = db.ProfileDetailsViews.FirstOrDefault(x => (x.HfmisCode.Equals(hfmisCode) || x.HealthFacility_Id == hfDetail.Id) && (x.HoD == "1"));
                List<VpMView> vpMViewsList = db.VpMViews.Where(x => x.HFMISCode.Equals(hfmisCode)).ToList();

                return Ok(new { hfDetail, hodProfile });
            }
            catch (DbEntityValidationException dbx)
            {
                return BadRequest(GetDbExMessage(dbx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private class HFVPStatus
        {
            public int Sanctioned { get; set; }
            public int Filled { get; set; }
            public int? Vacant { get; set; }
        }
        private class HFTypeCounts
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public int Sanctioned { get; set; }
            public int? Filled { get; set; }
            public int? Vacant { get; set; }
        }
        [Route("GetUserHFTypes/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetHealthFacilityDetails(string hfmisCode = "")
        {
            try
            {
                List<string> hftypes = new List<string>();
                List<HFTypeCounts> hftypesCountWise = new List<HFTypeCounts>();
                List<string> hftypesAvailable = new List<string>();

                hftypes = db.HFDetails.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).Select(x => x.HFTypeName).Distinct().ToList();

                foreach (string hftype in hftypes)
                {
                    HFTypeCounts hfTypeCounts = new HFTypeCounts();
                    hfTypeCounts.Name = hftype;
                    hfTypeCounts.Count = db.HFDetails.Where(x => x.HFTypeName.Equals(hftype) && x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).Count();
                    hfTypeCounts.Sanctioned = 0;
                    hfTypeCounts.Filled = 0;
                    hfTypeCounts.Vacant = 0;
                    foreach (VpMView vpmView in db.VpMViews.Where(x => x.HFTypeName.Equals(hftype) && x.HFMISCode.StartsWith(hfmisCode)).ToList())
                    {
                        hfTypeCounts.Sanctioned += vpmView.TotalSanctioned;
                        hfTypeCounts.Filled += vpmView.TotalWorking;

                    }
                    hfTypeCounts.Vacant = hfTypeCounts.Sanctioned - hfTypeCounts.Filled;
                    hftypesCountWise.Add(hfTypeCounts);
                }
                return Ok(hftypesCountWise);
            }
            catch (DbEntityValidationException dbx)
            {
                return BadRequest(GetDbExMessage(dbx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetDSummary/{type}/{code}")]
        [HttpGet]
        public IHttpActionResult GetDistrictsD(int type, string code)
        {
            try
            {
                List<DashboardSummaryDTO> dtos = new List<DashboardSummaryDTO>();

                if (type == 1)
                {
                    foreach (var district in db.Districts.Where(x => x.Code.StartsWith(code)).OrderBy(x => x.Name).ToList())
                    {
                        DashboardSummaryDTO dto = new DashboardSummaryDTO();
                        dto.Id = district.Id;
                        dto.Code = district.Code;
                        dto.Name = district.Name;
                        dto.Count = db.HFLists.Where(x => x.DistrictCode.Equals(district.Code) && (x.HFTypeCode.Equals("013") || x.HFTypeCode.Equals("014"))).Count();
                        dtos.Add(dto);
                    }
                }
                if (type == 2)
                {
                    foreach (var hf in db.HFLists.Where(x => x.DistrictCode.Equals(code) && (x.HFTypeCode.Equals("013") || x.HFTypeCode.Equals("014"))))
                    {
                        DashboardSummaryDTO dto = new DashboardSummaryDTO();
                        dto.Id = hf.Id;
                        dto.Code = hf.HFMISCode;
                        dto.Name = hf.FullName;
                        dto.DistCode = hf.DistrictCode;
                        dto.Count = db.HrProfiles.Where(x => x.HfmisCode.Equals(hf.HFMISCode) || x.HealthFacility_Id == hf.Id && x.IsActive == true).Count();
                        dtos.Add(dto);
                    }
                }
                return Ok(dtos);
            }
            catch (DbEntityValidationException dbx)
            {
                return BadRequest(GetDbExMessage(dbx));
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
    public class DashboardSummaryDTO
    {
        public int Id { get; set; }
        public string DistCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
