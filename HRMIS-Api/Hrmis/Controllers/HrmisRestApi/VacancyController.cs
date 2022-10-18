using Hrmis.Controllers.HrmisApiControllers;
using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Common;
using Hrmis.Models.ViewModels.Vacancy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Vacancy")]
    public class VacancyController : ApiController
    {
        public readonly VacancyService _vacancyService;

        public VacancyController()
        {
            _vacancyService = new VacancyService();
        }
        [HttpPost]
        [Route("SaveVacancy")]
        public IHttpActionResult SaveVacancy([FromBody] VPMaster vpm)
        {
            try
            {
                if (vpm.Id == 0 && _vacancyService.DuplicateVacancy(vpm, User.Identity.GetUserName(), User.Identity.GetUserId()))
                {
                    return Ok("Duplicate");
                }
                var result = _vacancyService.SaveVacancy(vpm, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("HoldVacancy")]
        public IHttpActionResult HoldVacancy([FromBody] VPHolder vpHolder)
        {
            try
            {
                var result = _vacancyService.SaveVacancyHolder(vpHolder, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("RemoveVacancy")]
        [HttpPost]
        public IHttpActionResult RemoveVacancy(int id)
        {
            try
            {
                return Ok(_vacancyService.RemoveVacancy(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetVpDProfileViews/{mId}")]
        public IHttpActionResult GetVpDProfileViews(int mId)
        {
            try
            {
                return Ok(_vacancyService.GetVpDProfileViews(mId, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetSuggestedProfile/{vpMaster_Id}/{hFId}/{scale}")]
        public IHttpActionResult GetSuggestedProfile(int vpMaster_Id, int hFId, int scale)
        {
            try
            {
                return Ok(_vacancyService.GetSuggestedProfile(vpMaster_Id, hFId, scale));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetVpProfiles/{vpMaster_Id}")]
        public IHttpActionResult GetVpProfiles(int vpMaster_Id)
        {
            try
            {
                return Ok(_vacancyService.GetVpProfiles(vpMaster_Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("Report/{hfmisCode}")]
        public IHttpActionResult Report(string hfmisCode)
        {
            try
            {
                string hftypecode = "";
                string userHfmisCode = "";
                string userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    using (var db = new HR_System())
                    {
                        var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var user = usermanger.FindById(userId);
                        hftypecode = user.HfTypeCode;
                        userHfmisCode = user.hfmiscode;
                    }
                }

                return Ok(_vacancyService.VPReport(userId, hftypecode, userHfmisCode, hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ReportDetail")]
        public IHttpActionResult ReportDetail(VacancyReportDetail vacancyReportDetail)
        {
            try
            {
                string hftypecode = "";
                string hfmisCode = "0";
                string userHfmisCode = "";
                string userName = "";
                string role = "";
                string userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    using (var db = new HR_System())
                    {
                        var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var user = usermanger.FindById(userId);
                        hftypecode = user.HfTypeCode;
                        userHfmisCode = user.hfmiscode;
                        userName = user.UserName;
                        role = User.IsInRole("Secondary") ? "Secondary" :
                           User.IsInRole("Primary") ? "Primary" :
                           User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC") ? "PHFMC" :
                           User.IsInRole("Deputy Secretary") ? "Deputy Secretary" :
                           User.IsInRole("Administrator") ? "Administrator" :
                           User.IsInRole("Chief Executive Officer") ? "Chief Executive Officer" :
                           // new role added by adnan 17/10/2022
                           User.IsInRole("Districts") ? "Districts" :
                           User.IsInRole("Order Generation") ? "Order Generation" :

                           User.IsInRole("Hisdu Order Team") ? "Hisdu Order Team" : "";
                    }
                }
                if (!string.IsNullOrEmpty(vacancyReportDetail.geoLevelName))
                {
                    using (var db = new HR_System())
                    {
                        if (vacancyReportDetail.column.Equals("Division"))
                        {
                            var division = db.Divisions.FirstOrDefault(x => x.Name.Equals(vacancyReportDetail.geoLevelName));
                            if (division != null)
                            {
                                hfmisCode = division.Code;
                            }
                        }
                        if (vacancyReportDetail.column.Equals("District"))
                        {
                            var district = db.Districts.FirstOrDefault(x => x.Name.Equals(vacancyReportDetail.geoLevelName));
                            if (district != null)
                            {
                                hfmisCode = district.Code;
                            }
                        }
                    }
                }
                return Ok(_vacancyService.VPReportDetail(userId, userName, role, hftypecode, vacancyReportDetail.type, userHfmisCode, hfmisCode, vacancyReportDetail.clickType, vacancyReportDetail.designationIds));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private string GetSql(VecancyReportViewModel model)
        {

            string sqlDimSelect = string.Join(",", model.Dimensions.Select(x => $"{x.value} as {x.Display.Replace(" ", "")}").ToArray());
            string sqlDimGroupBy = string.Join(",", model.Dimensions.Select(x => x.value).ToArray());
            string sqlMea = string.Join(",", model.Measures.Select(x => $"SUM({x.value}) {x.Display.Replace(" ", "")}").ToArray());
            string[] filterStrings = { null, null, null, null, null, null, null, null, null, null, null };
            var sql = "SELECT " + sqlDimSelect;
            if (model.Dimensions.Count > 0 && model.Measures.Count > 0)
            {
                sql += $", {sqlMea} ";
            }
            else
            {
                sql += $" {sqlMea} ";
            }

            sql += " FROM VpMProfileView ";

            if (model.Filters.Designations.Count > 0)
            {
                filterStrings[0] = $" Desg_Id in ({string.Join(",", model.Filters.Designations.Select(x => x).ToArray())})";
            }
            if (model.Filters.Divisions.Count > 0)
            {
                filterStrings[1] = $" DivisionName in ({string.Join(",", model.Filters.Divisions.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.Districts.Count > 0)
            {
                filterStrings[2] = $" DistrictName in ({string.Join(",", model.Filters.Districts.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.Tehsils.Count > 0)
            {
                filterStrings[3] = $" TehsilName in ({string.Join(",", model.Filters.Tehsils.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.HFTypes.Count > 0)
            {
                filterStrings[4] = $" HFTypeName in ({string.Join(",", model.Filters.HFTypes.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.HFs.Count > 0)
            {
                filterStrings[5] = $" HFName in ({string.Join(",", model.Filters.HFs.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.Scales.Count > 0)
            {
                filterStrings[6] = $" BPS in ({string.Join(",", model.Filters.Scales.Select(x => x).ToArray())})";
            }

            if (model.Filters.Cadres.Count > 0)
            {
                filterStrings[7] = $" CadreName in ({string.Join(",", model.Filters.Cadres.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.PostTypes.Count > 0)
            {
                filterStrings[8] = $" PostTypeName in ({string.Join(",", model.Filters.PostTypes.Select(x => "'" + x + "'").ToArray())})";
            }
            if (model.Filters.HFACs.Count > 0)
            {
                filterStrings[9] = $" HFACName in ({string.Join(",", model.Filters.HFACs.Select(x => "'" + x + "'").ToArray())})";
            }
            else
            {
                filterStrings[9] = " HFAC not in (4)";
            }
            if (User.IsInRole("South Punjab"))
            {
                filterStrings[10] = $" DistrictName in ({string.Join(",", Common.southDistrictNames.Select(x => "'" + x + "'").ToArray())})";
            }
            var filter = filterStrings.Where(x => x != null).ToArray();
            if (filter.Length > 0)
            {
                sql += $" WHERE {string.Join(" and ", filter)} ";
            }
            if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
            {
                sql += (filter.Length > 0 ? " and " : "") + " HFAC = 2 ";
            }
            //if (User.Identity.GetUserName().Equals("dpd"))
            //{
            //    sql += $" and HFAC != 2 ";
            //}
            if (model.Dimensions.Count > 0)
            {
                sql += " GROUP BY " + sqlDimGroupBy;
            }

            if (model.Dimensions.Count > 0 || model.Measures.Count > 0)
            {
                sql += " ORDER BY " + string.Join(",", Enumerable.Range(1, model.Dimensions.Count + model.Measures.Count).Select(x => x + ""));
            }
            return sql;
        }
        [Route("GetVacancyData")]
        [HttpPost]
        public IHttpActionResult GetVecancyData([FromBody] VecancyReportViewModel model)
        {
            try
            {
                var data = _vacancyService.GetDataFromDataTable(_vacancyService.GetData(GetSql(model)));
                return Ok(data);
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetVacancyMaster")]
        [HttpPost]
        public IHttpActionResult GetVacancyMaster([FromBody] VacancyMasterDtoViewModel model)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var vpMaster = db.VpMProfileViews.FirstOrDefault(x => x.HF_Id == model.HealthFacility_Id && x.Desg_Id == model.Designation_Id);
                    if (vpMaster != null)
                    {
                        var vpDetails = db.VpDProfileViews.Where(x => x.Master_Id == vpMaster.Id).ToList();
                        var vpMasterLog = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == vpMaster.EntityLifeCycle_Id);
                        var vpMasterLogs = db.Entity_Modified_Log.Where(x => x.Entity_Lifecycle_Id == vpMasterLog.Id).ToList();
                        return Ok(new { vpMaster, vpDetails, vpMasterLog, vpMasterLogs });
                    }
                    return Ok(false);
                }
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetVacancyHolder")]
        [HttpPost]
        public IHttpActionResult GetVacancyHolder([FromBody] VacancyMasterDtoViewModel model)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var vpHolder = db.VPHolderViews.FirstOrDefault(x => x.VpMaster_Id == model.Id && x.IsActive == true);
                    return Ok(vpHolder);
                }
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetVacancyHolders")]
        [HttpPost]
        public IHttpActionResult GetVacancyHolders([FromBody] VacancyMasterDtoViewModel model)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<VPHolderView> query = db.VPHolderViews.Where(x => x.IsActive == true).AsQueryable(); ;
                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.Created_Date).Skip(model.Skip).Take(model.PageSize).ToList();
                    return Ok(new TableResponse<VPHolderView> { List = list, Count = count });
                }
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("DownloadVacancyData")]
        [HttpPost]
        public IHttpActionResult DownloadVacancyData([FromBody] VecancyReportViewModel model)
        {
            try
            {
                var gv = new GridView();
                var dt = _vacancyService.GetData(GetSql(model));
                var dtSr = new DataTable();
                var dtColumn = new List<DataColumn>();
                dtColumn.Add(new DataColumn("Sr."));
                for (int count = 0; count < dt.Columns.Count; count++)
                {
                    dtColumn.Add(new DataColumn(dt.Columns[count].ColumnName));
                }
                dtSr.Columns.AddRange(dtColumn.ToArray());
                dtSr.Columns["Sr."].AutoIncrement = true;
                //Set the Starting or Seed value.
                dtSr.Columns["Sr."].AutoIncrementSeed = 1;
                //Set the Increment value.
                dtSr.Columns["Sr."].AutoIncrementStep = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    var itemArrayWithSr = new List<object>() { null};
                    itemArrayWithSr.AddRange(dr.ItemArray.ToList());
                    dtSr.Rows.Add(itemArrayWithSr.ToArray());
                }
                gv.DataSource = dtSr;
                gv.DataBind();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=VacancyData.xls");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                HttpContext.Current.Response.Output.Write(objStringWriter.ToString());
                HttpContext.Current.Response.End();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
