using System;
using System.Collections.Generic;
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
using Hrmis.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Net.Http;
using Hrmis.Models.CustomModels;
using System.Data;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/VP")]
    public class VacancyPositionController : ApiController
    {
        private HR_System db = new HR_System();

        [Route("GetVPMasters/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetVPMasters(string hfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<VpMProfileView> vps = db.VpMProfileViews.Where(x => x.HFMISCode.Equals(hfmisCode)).ToList();

                //List<VpDView> vds = db.VpDViews.Where(x => x.HFMISCode.Equals(hfmisCode)).ToList();
                return Ok(vps);
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage} {Environment.NewLine}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("GetVPMasters2")]
        [HttpPost]
        public IHttpActionResult GetVPMasters2([FromBody] VpMasterDto dto)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                List<int> dsgs = GetDesignationsIds(dto.HfmisCode, dto.HfTypeCode);

                List<VpMProfileView> vp = new List<VpMProfileView>();

                foreach (int dsg in dsgs)
                {
                    VpMProfileView VpMProfileView = db.VpMProfileViews.FirstOrDefault(x => x.Desg_Id == dsg);
                    int sumSanctioned = 0;
                    int? sumWorking = 0;
                    var vpmList = GetDesignationVps(dsg, dto.HfmisCode, dto.HfTypeCode);
                    foreach (VpMProfileView vpm in vpmList)
                    {
                        sumSanctioned += vpm.TotalSanctioned;
                        sumWorking += vpm.TotalWorking;
                    }
                    VpMProfileView.TotalSanctioned = sumSanctioned;
                    VpMProfileView.TotalWorking = (int)sumWorking;
                    vp.Add(VpMProfileView);
                }
                vp = vp.OrderByDescending(x => x.TotalSanctioned).ToList();
                return Ok(vp);
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

        private List<int> GetDesignationsIds(string hfmisCode, string hfTypeCode)
        {
            List<int> views = new List<int>();
            if (!string.IsNullOrWhiteSpace(hfmisCode) && !string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.StartsWith(hfmisCode) && x.HFMISCode.Substring(12, 3) == hfTypeCode)
                        .Select(x => x.Desg_Id).Distinct().ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfmisCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.StartsWith(hfmisCode)).Select(x => x.Desg_Id).Distinct().ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.Substring(12, 3) == hfTypeCode).
                        Select(x => x.Desg_Id).Distinct().ToList();
            }
            else
            {
                views =
                    db.VpMProfileViews.
                        Select(x => x.Desg_Id).Distinct().ToList();
            }
            return views;
        }
        private List<int> GetDesignationsIds(string hfmisCode, string hfTypeCode, int currentPage, int itemsPerPage)
        {
            List<int> views = new List<int>();
            if (!string.IsNullOrWhiteSpace(hfmisCode) && !string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.StartsWith(hfmisCode) && x.HFMISCode.Substring(12, 3) == hfTypeCode)
                        .Select(x => x.Desg_Id).Distinct().Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfmisCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.StartsWith(hfmisCode)).Select(x => x.Desg_Id).Distinct().Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.HFMISCode.Substring(12, 3) == hfTypeCode).
                        Select(x => x.Desg_Id).Distinct().Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            }
            else
            {
                views =
                    db.VpMProfileViews.
                        Select(x => x.Desg_Id).Distinct().Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            }
            return views;
        }

        private List<VpMProfileView> GetDesignationVps(int desgId, string hfmisCode, string hfTypeCode)
        {
            List<VpMProfileView> views = new List<VpMProfileView>();
            if (!string.IsNullOrWhiteSpace(hfmisCode) && !string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(
                        x => x.Desg_Id == desgId
                        && x.HFMISCode.StartsWith(hfmisCode)
                        && x.HFMISCode.Substring(12, 3) == hfTypeCode).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfmisCode))
            {
                views =
                    db.VpMProfileViews.Where(x => x.Desg_Id == desgId &&
                         x.HFMISCode.StartsWith(hfmisCode)).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(hfTypeCode))
            {
                views =
                    db.VpMProfileViews.Where(x => x.Desg_Id == desgId
                   && x.HFMISCode.Substring(12, 3) == hfTypeCode).ToList();
            }
            else
            {
                views =
                    db.VpMProfileViews.Where(x => x.Desg_Id == desgId).ToList();
            }
            return views;
        }

        [Route("GetVPDetails/{mId}")]
        [HttpGet]
        public IHttpActionResult GetVPMasters(int mId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<VpDProfileView> vps = db.VpDProfileViews.Where(x => x.Master_Id == mId).ToList();
                return Ok(vps);
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
        [Route("PostVP/{userName}/{userId}")]
        [HttpPost]
        public IHttpActionResult PostVP([FromBody] VPMaster vpm, string userName, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Entity_Lifecycle el = new Entity_Lifecycle();
                el.Created_Date = DateTime.UtcNow.AddHours(5);
                el.Created_By = userName;
                el.Users_Id = userId;
                el.IsActive = true;
                el.Entity_Id = 3;
                vpm.Entity_Lifecycle = el;

                List<VPDetail> vpdetailsList = vpm.VPDetails.ToList();
                vpm.VPDetails = null;
                db.VPMasters.Add(vpm);
                db.SaveChanges();
                int? totalWorking = 0;
                foreach (VPDetail vpdetail in vpdetailsList)
                {
                    vpdetail.Master_Id = vpm.Id;
                    totalWorking += vpdetail.TotalWorking;
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 3;
                    vpdetail.Entity_Lifecycle = eld;
                }
                vpm.TotalWorking = totalWorking;
                db.Entry(vpm).State = EntityState.Modified;
                db.VPDetails.AddRange(vpdetailsList);
                db.SaveChanges();
                return Ok(new { result = true });
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
        [Route("EditVP/{userName}/{userId}")]
        [HttpPost]
        public IHttpActionResult EditVP([FromBody] VPMaster vpm, string userName, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                VPMaster vpMaster = db.VPMasters.Find(vpm.Id);
                vpMaster.PostType_Id = vpm.PostType_Id;
                vpMaster.TotalSanctioned = vpm.TotalSanctioned;
                vpMaster.TotalWorking = vpm.TotalWorking;

                Entity_Modified_Log eml = new Entity_Modified_Log();

                if (vpMaster.EntityLifecycle_Id == null)
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 3;


                    db.SaveChanges();
                    vpMaster.Entity_Lifecycle = eld;
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);
                }
                else
                {
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);
                }

                //eml.Modified_By = userId;
                //eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                ////eml.Description = JsonConvert.SerializeObject(vpMaster);
                //vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);

                int? totalWorking = 0;
                foreach (VPDetail vpdetail in vpm.VPDetails)
                {
                    vpdetail.Master_Id = vpMaster.Id;
                    totalWorking += vpdetail.TotalWorking;
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 3;
                    vpdetail.Entity_Lifecycle = eld;
                    db.VPDetails.Add(vpdetail);
                }
                vpMaster.TotalWorking += totalWorking;
                db.Entry(vpMaster).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(new { result = true });
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
        [Route("EditVPEmpModes/{userName}/{userId}")]
        [HttpPost]
        public IHttpActionResult EditVPEmpModes([FromBody] VPMaster vpm, string userName, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                VPMaster vpMaster = db.VPMasters.Find(vpm.Id);
                vpMaster.PostType_Id = vpm.PostType_Id;
                vpMaster.TotalSanctioned = vpm.TotalSanctioned;
                vpMaster.TotalWorking = vpm.TotalWorking;

                Entity_Modified_Log eml = new Entity_Modified_Log();

                if (vpMaster.EntityLifecycle_Id == null)
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 3;
                   
                    
                    db.SaveChanges();
                    vpMaster.Entity_Lifecycle = eld;
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);
                }
                else
                {          
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);                 
                    vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);
                }
                //Entity_Modified_Log eml = new Entity_Modified_Log();
                //eml.Modified_By = userId;
                //eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                ////eml.Description = JsonConvert.SerializeObject(vpMaster);
                //vpMaster.Entity_Lifecycle.Entity_Modified_Log.Add(eml);

                foreach (VPDetail vpdetail in vpm.VPDetails)
                {
                    VPDetail vpd = db.VPDetails.Find(vpdetail.Id);
                    vpd.EmpMode_Id = vpdetail.EmpMode_Id;
                    vpd.TotalWorking = vpdetail.TotalWorking;
                    if (vpd.Entity_Lifecycle != null)
                    {
                        Entity_Modified_Log emld = new Entity_Modified_Log();
                        emld.Modified_By = userId;
                        emld.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Description = "EmpMode_Id : " + vpdetail.EmpMode_Id;
                        vpd.Entity_Lifecycle.Entity_Modified_Log.Add(emld);
                    }
                    db.Entry(vpd).State = EntityState.Modified;
                }
                int? totalWorking = 0;
                foreach (VPDetail vpdetail in db.VPDetails.Where(x => x.Master_Id == vpMaster.Id).ToList())
                {
                    totalWorking += vpdetail.TotalWorking;
                }
                vpMaster.TotalWorking = totalWorking;
                db.Entry(vpMaster).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(new { result = true });
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




        [Route("GetVPDetials/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetVPDetials(string hfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var vps = db.VpDetailViews.Where(x => x.HFMIS_Code.Equals(hfmisCode)).OrderBy(x => x.DsgName).ToList();
                return Ok(vps);
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

        private string GetSql(VecancyReportViewModel model) {

            string sqlDim = string.Join(",", model.Dimensions.Select(x => x.value).ToArray());
            string sqlMea = string.Join(",", model.Measures.Select(x => $"SUM({x.value}) {x.value}").ToArray());
            string[] filterStrings = { null, null, null, null, null,null ,null};
            var sql = "SELECT " + sqlDim;
            if (model.Dimensions.Count > 0 && model.Measures.Count > 0)
            {
                sql += $", {sqlMea} ";
            }
            else
            {
                sql += $" {sqlMea} ";
            }

            sql += " FROM VpMDView ";

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
            if (model.Filters.Scales.Count > 0)
            {
                filterStrings[5] = $" Scale in ({string.Join(",", model.Filters.Scales.Select(x =>  x ).ToArray())})";
            }

            if (model.Filters.Cadres.Count > 0)
            {
                filterStrings[6] = $" CadreName in ({string.Join(",", model.Filters.Cadres.Select(x => "'" + x + "'").ToArray())})";
            }

            var filter = filterStrings.Where(x => x != null).ToArray();
            if (filter.Length > 0)
            {
                sql += $" WHERE {string.Join(" and ", filter)} ";
            }

            if (model.Dimensions.Count > 0)
            {
                sql += " GROUP BY " + sqlDim;
            }
            return sql;
        }

        [Route("GetVecancyData")]
        [HttpPost]
        public IHttpActionResult GetVecancyData([FromBody] VecancyReportViewModel model)
        {
            try
            {
                var data = GetDataFromDataTable(GetData(GetSql(model)));
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

        [Route("DownloadVacancyData")]
        [HttpPost]
        public IHttpActionResult DownloadVacancyData([FromBody] VecancyReportViewModel model)
        {
            try
            {
                var gv = new GridView();
                gv.DataSource = GetData(GetSql(model));
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

        private List<object> GetDataFromDataTable(DataTable table) {
            List<object> data = new List<object>();
            foreach (DataRow item in table.Rows)
            {
                List<object> row = new List<object>();
                foreach (DataColumn column in table.Columns)
                {
                    row.Add(item[column].ToString());
                }
                data.Add(row);
            }
            return data;
        }

        private DataTable GetData(string sql) {
            var table = new DataTable();
            using (System.Data.IDbCommand command = db.Database.Connection.CreateCommand())
            {
                try
                {
                    db.Database.Connection.Open();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;

                    using (System.Data.IDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
            return table;
        }
       


        //------------------------------------------------------------------







        //[Route("GetVPs/{hfmisCode}")]
        //[HttpGet]
        //public IHttpActionResult GetVPs(string hfmisCode)
        //{
        //    try
        //    {
        //        db.Configuration.ProxyCreationEnabled = false;
        //        var vps = db.VPSPs.Where(x => x.HFMIS_Code.Equals(hfmisCode)).ToList();
        //        return Ok(vps);
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        string message = dbex.EntityValidationErrors.
        //            SelectMany(validationErrors => validationErrors.ValidationErrors).
        //            Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        //        return InternalServerError(dbex);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[Route("PostVP/{userName}/{userId}")]
        //[HttpPost]
        //public IHttpActionResult PostVP([FromBody] List<VPSP> vP, string userName, string userId)
        //{
        //    string hfmisCode = vP[0].HFMIS_Code;
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        foreach (var vJob in vP)
        //        {
        //            Entity_Lifecycle el = new Entity_Lifecycle();
        //            el.Created_Date = DateTime.UtcNow.AddHours(5);
        //            el.Created_By = userName;
        //            el.Users_Id = userId;
        //            el.IsActive = true;
        //            el.Entity_Id = 3;
        //            vJob.Entity_Lifecycle = el;
        //        }
        //        db.VPSPs.AddRange(vP);
        //        db.SaveChanges();
        //        return Ok(new { result = true });
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        string message = dbex.EntityValidationErrors.
        //            SelectMany(validationErrors => validationErrors.ValidationErrors).
        //            Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        //        return InternalServerError(dbex);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [Route("UpdateVP/{userId}")]
        [HttpPost]
        public IHttpActionResult UpdateVP(VPSP v, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (v.EmpMode_Id != null)
                {
                    Entity_Lifecycle el = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == v.EntityLifeCycle_Id);
                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = userId;
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Description = "Updated Post : " + v.Designation_Id + " &  Employment Mode : " + v.EmpMode_Id;
                    el.Entity_Modified_Log.Add(eml);
                }

                if (v.EmpMode_Id != null)
                {
                    v.VPType = 2;
                }
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(new { result = true });
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

        [Route("RmvVPs")]
        [HttpPost]
        public async Task<IHttpActionResult> RmvVPs(List<int> ids)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    foreach (var id in ids)
                    {
                        VPMaster vpMaster = await db.VPMasters.FindAsync(id);
                        if (vpMaster == null)
                        {
                            return NotFound();
                        }
                        List<VPDetail> vpDetails = await db.VPDetails.Where(x => x.Master_Id == vpMaster.Id).ToListAsync();
                        foreach (VPDetail vpd in vpDetails)
                        {
                            db.VPDetails.Remove(vpd);
                        }

                        db.VPMasters.Remove(vpMaster);
                        await db.SaveChangesAsync();
                    }
                    return Ok(new { result = true });
                }
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        #region Dashboards

        [HttpGet]
        [Route("ReportByCadre/{cadreId}/{hfmisCode}")]
        public IHttpActionResult ReportByCadre(int cadreId, string hfmisCode)
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var cadre = db.Cadres.FirstOrDefault(x => x.Id == cadreId);
                string cadreName = cadre?.Name ?? string.Empty;
                var result = db.VpMProfileViews.Where(x => x.CadreName.StartsWith(cadreName) && x.HFMISCode.StartsWith(hfmisCode)).GroupBy(c => c.CadreName).
                    Select(s => new
                    {
                        Name = s.Key,
                        Sanctioned = s.Sum(a => a.TotalSanctioned),
                        Filled = s.Sum(a => a.TotalWorking),
                        Vacant = s.Sum(a => a.TotalSanctioned) - s.Sum(a => a.TotalWorking)
                    }).OrderByDescending(x => x.Sanctioned).ToList();
                return Ok(result);
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ReportByHF/{desigId}/{hfmisCode}/{hftypecode}")]
        public IHttpActionResult ReportByDesignation(int desigId, string hfmisCode, string hftypecode = "0")
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var result = db.VpMProfileViews.Where(x => x.Desg_Id == desigId && x.HFMISCode.StartsWith(hfmisCode) && (x.HFMISCode.Substring(12, 3).StartsWith(hftypecode) || x.HFMISCode.Substring(12, 3).Equals(hftypecode))).GroupBy(c => c.HFName).
                    Select(s => new
                    {
                        Name = s.Key,
                        Sanctioned = s.Sum(a => a.TotalSanctioned),
                        Filled = s.Sum(a => a.TotalWorking),
                        Vacant = s.Sum(a => a.TotalSanctioned) - s.Sum(a => a.TotalWorking)
                    }).OrderByDescending(x => x.Sanctioned).ToList();
                return Ok(result);
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ReportByHFType/{hfTypeId}/{hfmisCode}")]
        public IHttpActionResult ReportByHealthFacility(int hfTypeId, string hfmisCode)
        {

            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hf = db.HFTypes.FirstOrDefault(x => x.Id == hfTypeId);
                string hfTypeName = hf?.Name ?? string.Empty;
                var result = db.VpMProfileViews.Where(x => x.HFTypeName.StartsWith(hfTypeName) && x.HFMISCode.StartsWith(hfmisCode)).GroupBy(c => c.HFTypeName).
                    Select(s => new
                    {
                        Name = s.Key,
                        Sanctioned = s.Sum(a => a.TotalSanctioned),
                        Filled = s.Sum(a => a.TotalWorking),
                        Vacant = s.Sum(a => a.TotalSanctioned) - s.Sum(a => a.TotalWorking)
                    }).OrderByDescending(x => x.Sanctioned).ToList();
                return Ok(result);
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion


        private string CreateCodeWithHfType(string hfmisCode, string hfTypeCode)
        {
            string code = string.Empty;
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hfType = db.HFTypes.Where(x => x.Code == hfTypeCode).Include(x => x.HFCat_Id).FirstOrDefault();
                if (hfType != null) code = $"{hfType.Code}{hfType.HFCategory.Code}";
            }
            return code;
        }

        private bool VPExists(int id)
        {
            return db.VPs.Count(e => e.Id == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [Route("GetVacancyPositionsExcel/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyPositionsExcel(string code = "")
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
                string query;
                if (userHfmisCode == "0" && hftypecode != null)
                {
                   
                    query = $@" select DsgName as DesignationName, Scale, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular,Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract,CadreName FROM VpMDView where HFMISCode Like @param and substring(HFMISCode,13,3) = '{hftypecode}' group by DsgName,Scale,
CadreName order by TotalSanctioned desc";

                }
                else
                {
                   
                    query = @" select DsgName as DesignationName, Scale, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular,Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract,CadreName FROM VpMDView where HFMISCode Like @param group by DsgName,Scale,
CadreName order by TotalSanctioned desc";
                }
                //                 

                string CodeParam = string.Format("{0}%", code);
              
                var data = db.Database.SqlQuery<VpReportExcel>(query,
                   new SqlParameter("@param", CodeParam)
                   ).ToList();
                return Ok(data);
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetVacancyPositions/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyPositions(string code = "")
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
                string query;
                if (userHfmisCode == "0" && hftypecode != null)
                {
                    //query = $@"
                    //select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned, 
                    //sum(TotalWorking) as TotalWorking
                    //,HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
                    //from VPMaster
                    //left join HrDesignationView as HRDV on desg_Id = HRDV.Id
                    //where VPMaster.HFMISCode Like '{userHfmisCode}%' and substring(VPMaster.HFMISCode,13,3) = '{hftypecode}'
                    //group by desg_Id, HRDV.Name ,HRDV.Cadre_Name,HRDV.Scale order by TotalSanctioned desc ";


                    //query = $@"
                    //select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned, 
                    //sum(TotalWorking) as TotalWorking
                    //,HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
                    //from VPMaster
                    //left join HrDesignationView as HRDV on desg_Id = HRDV.Id
                    //where VPMaster.HFMISCode Like @param and substring(VPMaster.HFMISCode,13,3) = '{hftypecode}'
                    //group by desg_Id, HRDV.Name ,HRDV.Cadre_Name,HRDV.Scale order by TotalSanctioned desc ";

                    // last query
                  //query = $@" select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned,
                  //sum(TotalWorking) as TotalWorking, sum(Regular) as TotalRegular, sum(Contract) as TotalContract, sum(Adhoc) as TotalAdhoc
                  //, HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
                  //from VpMDView left join HrDesignationView as HRDV on desg_Id = HRDV.Id
                  //where VPMaster.HFMISCode Like @param and substring(VPMaster.HFMISCode,13,3) = '{hftypecode}'
                  //group by desg_Id, HRDV.Name, HRDV.Cadre_Name, HRDV.Scale order by TotalSanctioned desc";


query = $@" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, Scale, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular,Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract,CadreName FROM VpMDView where HFMISCode Like @param and substring(HFMISCode,13,3) = '{hftypecode}' group by DsgName,Scale,desg_Id,
CadreName order by TotalSanctioned desc";
                }
                else {
                    //earlier
                    //query = @"
                    //select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned, 
                    //sum(TotalWorking) as TotalWorking
                    //,HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
                    //from VPMaster
                    //left join HrDesignationView as HRDV on desg_Id = HRDV.Id
                    //where VPMaster.HFMISCode Like @param
                    //group by desg_Id, HRDV.Name ,HRDV.Cadre_Name,HRDV.Scale order by TotalSanctioned desc ";


                    //latest change comment for filled distionction // last query
                    //  query = @" select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned,
                    //sum(TotalWorking) as TotalWorking, sum(Regular) as TotalRegular, sum(Contract) as TotalContract, sum(Adhoc) as TotalAdhoc
                    //, HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
                    //from VpMDView left join HrDesignationView as HRDV on desg_Id = HRDV.Id
                    //where VPMaster.HFMISCode Like @param  group by desg_Id, HRDV.Name, HRDV.Cadre_Name, HRDV.Scale order by TotalSanctioned desc";


                    //query = @" select DsgName as DesignationName, Scale, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant, sum(Regular) as TotalRegular,
                    //Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract,CadreName as CadreName FROM VpMDView where HFMISCode Like @param group by DsgName  order by TotalSanctioned desc";
                    query = @" select distinct(desg_Id) as DesignationID,DsgName as DesignationName, Scale, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular,Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract,CadreName FROM VpMDView where HFMISCode Like @param group by DsgName,Scale,desg_Id,
CadreName order by TotalSanctioned desc";
                }
 //                   string query = @"
 // select distinct(desg_Id) as DesignationID, sum(TotalSanctioned) as TotalSanctioned, 
 //   sum(TotalWorking) as TotalWorking
	//,HRDV.Name as DesignationName, HRDV.Cadre_Name as CadreName, HRDV.Scale
 //   from VPMaster
 //   left join HrDesignationView as HRDV on desg_Id = HRDV.Id
	//where VPMaster.HFMISCode Like @param
 //   group by desg_Id, HRDV.Name ,HRDV.Cadre_Name,HRDV.Scale order by TotalSanctioned desc ";

                string CodeParam = string.Format("{0}%", code);
                //       string CodeParam1 = string.Format("{0}", userHfmisCode);

                //      VpReport data = db.Database.SqlQuery<VpReport>(query, new SqlParameter("@param", CodeParam)).FirstOrDefault();

                // var data = db.Database.SqlQuery<VpReport>(query).ToList();

                var data = db.Database.SqlQuery<VpReport>(query,
                   new SqlParameter("@param", CodeParam)
                   ).ToList();



                return Ok(data);
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetVacancyPositionsHFExcel/{hfmis}/{hftype}/{desgid}/{currentpg}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyPositionsHFExcel(string hfmis = "0", string hftype = "0", string desgid = "", int currentpg = 1)
        {

            try
            {
                string hftypecode = "";
                string userHfmisCode = "";
                string BaseQuery = "";
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

                string CodeParam = string.Format("{0}%", hfmis);
                List<SqlParameter> Params = new List<SqlParameter>() {

                    new SqlParameter("@did", desgid),
                    new SqlParameter("@code", CodeParam)
                   //  new SqlParameter("@hftcode", hftypecode)
                };


                string typeClause = "";
                if (hftype != "" && hftype != "0")
                {
                    typeClause = "and Substring(HFMISCode,13,3)=@type";
                    Params.Add(new SqlParameter("@type", hftype));
                }

                int ItemsPerPage = 100;
                int Skip = (currentpg - 1) * ItemsPerPage;
                int Take = Skip + ItemsPerPage;
                Params.Add(new SqlParameter("@skipy", Skip));
                Params.Add(new SqlParameter("@tako", Take));



                if (userHfmisCode == "0" && hftypecode != null)
                {
                    
                    BaseQuery = $@"select distinct(FullName) as HfFullName, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular, Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract FROM VpMDView where Substring(HFMISCode,13,3)='{hftypecode}' and Desg_Id =@did and HFMISCode Like @code " + typeClause + @"
group by FullName";

                }
                else
                {
                    BaseQuery = @"select distinct(FullName) as HfFullName, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular, Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract FROM VpMDView where Desg_Id =@did and HFMISCode Like @code " + typeClause + @"
group by FullName";
                }

                string query = @"select * from (SELECT *, ROW_NUMBER() OVER (ORDER BY TotalSanctioned) AS RowNum FROM(" + BaseQuery + ") Base) as abc WHERE abc.RowNum > @skipy AND abc.RowNum <= @tako";


                var data = db.Database.SqlQuery<VpReportHFExcel>(query, Params.ToArray()).ToList();

                var totalRecords = GetVpRecordsCount(desgid, CodeParam, hftype);

                return Ok(new { vp = data, totalRecords = totalRecords });
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetVacancyPositionsByDesg/{hfmis}/{hftype}/{desgid}/{currentpg}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyPositionsByDesg(string hfmis = "0", string hftype = "0", string desgid = "", int currentpg = 1)
        {

            try
            {
                string hftypecode = "";
                string userHfmisCode = "";
                string BaseQuery = "";
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

                string CodeParam = string.Format("{0}%", hfmis);
                List<SqlParameter> Params = new List<SqlParameter>() {

                    new SqlParameter("@did", desgid),
                    new SqlParameter("@code", CodeParam)
                   //  new SqlParameter("@hftcode", hftypecode)
                };


                string typeClause = "";
                if (hftype != "" && hftype != "0")
                {
                    typeClause = "and Substring(HFMISCode,13,3)=@type";
                    Params.Add(new SqlParameter("@type", hftype));
                }

                int ItemsPerPage = 100;
                int Skip = (currentpg - 1) * ItemsPerPage;
                int Take = Skip + ItemsPerPage;
                Params.Add(new SqlParameter("@skipy", Skip));
                Params.Add(new SqlParameter("@tako", Take));


               
                if (userHfmisCode == "0" && hftypecode != null)
                {
                    // BaseQuery = $@"select  
                    //distinct(tbl.HFMISCode), 
                    //sum(TotalSanctioned) as TotalSanctioned, 
                    //sum(TotalWorking) as TotalWorking,hf.FullName as HfFullName
                    //from VPMaster as tbl
                    //left join HealthFacilityDetail hf on hf.HFMISCode =  tbl.HFMISCode
                    //where Substring(tbl.HFMISCode,13,3)='{hftypecode}' and Desg_Id =@did and tbl.HFMISCode Like @code " + typeClause + @"
                    //group by tbl.HFMISCode, hf.FullName";


BaseQuery = $@"select distinct(FullName) as HfFullName, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular, Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract FROM VpMDView where Substring(HFMISCode,13,3)='{hftypecode}' and Desg_Id =@did and HFMISCode Like @code " + typeClause + @"
group by FullName";

                }
                else
                {
                    //BaseQuery = @"select  
                    //distinct(tbl.HFMISCode), 
                    //sum(TotalSanctioned) as TotalSanctioned, 
                    //sum(TotalWorking) as TotalWorking,hf.FullName as HfFullName
                    //from VPMaster as tbl
                    //left join HealthFacilityDetail hf on hf.HFMISCode =  tbl.HFMISCode
                    //where Desg_Id =@did and tbl.HFMISCode Like @code " + typeClause + @"
                    //group by tbl.HFMISCode, hf.FullName";

BaseQuery = @"select distinct(FullName) as HfFullName, sum(TotalSanctioned) as TotalSanctioned, sum(TotalWorking) as TotalWorking, sum(Vacant) as TotalVacant,
sum(Regular) as TotalRegular, Sum(Adhoc) as TotalAdhoc, Sum(Contract) as TotalContract FROM VpMDView where Desg_Id =@did and HFMISCode Like @code " + typeClause + @"
group by FullName";
                }

                string query = @"select * from (SELECT *, ROW_NUMBER() OVER (ORDER BY TotalSanctioned) AS RowNum FROM(" + BaseQuery + ") Base) as abc WHERE abc.RowNum > @skipy AND abc.RowNum <= @tako";


                var data = db.Database.SqlQuery<VpReport>(query, Params.ToArray()).ToList();

                var totalRecords = GetVpRecordsCount(desgid, CodeParam, hftype);

                return Ok(new { vp = data, totalRecords = totalRecords });
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        private int GetVpRecordsCount(string desgid, string CodeParam, string hftype)
        {

            List<SqlParameter> ParamsCount = new List<SqlParameter>() {

                    new SqlParameter("@did", desgid),
                    new SqlParameter("@code", CodeParam)
                };

            string typeClause = "";
            if (hftype != "" && hftype != "0")
            {
                typeClause = "and Substring(HFMISCode,13,3)=@type";
                ParamsCount.Add(new SqlParameter("@type", hftype));
            }

            string CountQuery = @"select  
	                            Count(distinct(HFMISCode)) 
	                            from VPMaster
	                            where Desg_Id=@did and HFMISCode Like @code " + typeClause + @"
	                            ";

            HR_System ctx = new HR_System();
            int recordsCount = ctx.Database.SqlQuery<int>(CountQuery, ParamsCount.Clone().ToArray()).SingleOrDefault();
            return recordsCount;
        }

        [Route("GetVacancyPositionsByScale/{hfmis}/{hftype}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVacancyPositionsByScale(string hfmis = "0", string hftype = "0")
        {

            try
            {
                string CodeParam = string.Format("{0}%", hfmis);
                List<SqlParameter> Params = new List<SqlParameter>() {
                    new SqlParameter("@code", CodeParam)
                };


                string typeClause = "";
                if (hftype != "" && hftype != "0")
                {
                    typeClause = "and Substring(HFMISCode,13,3)=@type";
                    Params.Add(new SqlParameter("@type", hftype));
                }


                string query = @" 
                                    select distinct(HrScale_Id) as Scale, sum(TotalSanctioned) as TotalSanctioned, 
                                    sum(TotalWorking) as TotalWorking
                                    from VPMaster
                                    left join HrDesignation as HRD on desg_Id = HRD.Id
	                                where VPMaster.HFMISCode Like @code "+ typeClause + @" 
                                    group by HrScale_Id order by TotalSanctioned desc";

              

                var data = db.Database.SqlQuery<VpReport>(query, Params.ToArray()).ToList();

              

                return Ok(data);
            }
            catch (DbEntityValidationException dbex)
            {
                string message = dbex.EntityValidationErrors.
                    SelectMany(validationErrors => validationErrors.ValidationErrors).
                    Aggregate("",
                        (current, validationError) =>
                            current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return InternalServerError(dbex);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        //[Route("GetVacancyPositionsByHfType")]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetVacancyPositionsByHfType()
        //{
        //    try
        //    {
        //        string query = @" 
        //                        select distinct(HfTypeCode), sum(TotalSanctioned) as TotalSanctioned, 
        //                        sum(TotalWorking) as TotalWorking,
        //                        HFTypes.Name as HfTypeName
        //                        from (
	       //                     select *, Substring(HFMISCode,13,3) as HfTypeCode from VPMaster 
	       //                     ) Base
	       //                     left join HFTypes on HFTypes.Code =  HfTypeCode
        //                        group by HfTypeCode, HFTypes.Name order by TotalSanctioned desc
        //                        ";

        //        var data = db.Database.SqlQuery<VpReport>(query).ToList();

        //        return Ok(data);
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        string message = dbex.EntityValidationErrors.
        //            SelectMany(validationErrors => validationErrors.ValidationErrors).
        //            Aggregate("",
        //                (current, validationError) =>
        //                    current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        //        return InternalServerError(dbex);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
    public class VacancyMasterDtoViewModel : Paginator
    {

        public VacancyMasterDtoViewModel()
        {
        }
        public int Id { get; set; }
        public int HealthFacility_Id { get; set; }
        public int Designation_Id { get; set; }
    }
    public class VecancyReportViewModel
    {

        public VecancyReportViewModel()
        {
            Dimensions = new List<VecancyDataViewModel>();
            Measures = new List<VecancyDataViewModel>();
            Filters = new FilterViewModel();
        }
        public List<VecancyDataViewModel> Dimensions { get; set; }
        public List<VecancyDataViewModel> Measures { get; set; }
        public FilterViewModel Filters { get; set; }
    }
public class VecancyDataViewModel
    {
        public string Display { get; set; }
        public string value { get; set; }
    }

}

public class FilterViewModel
{

    public FilterViewModel()
    {
        Designations = new List<int>();
        Divisions = new List<string>();
        Districts = new List<string>();
        Tehsils = new List<string>();
        HFTypes = new List<string>();
        HFs = new List<string>();
        Scales = new List<int>();
        Cadres = new List<string>();
        PostTypes = new List<string>();
        HFACs = new List<string>();
    }
    public List<int> Designations { get; set; }
    public List<string> Divisions { get; set; }
    public List<string> Districts { get; set; }
    public List<string> Tehsils { get; set; }
    public List<string> HFTypes  { get; set; }
    public List<string> HFs  { get; set; }
    public List<string> Cadres  { get; set; }
    public List<string> PostTypes { get; set; }
    public List<string> HFACs { get; set; }
    public List<int>  Scales  { get; set; }
}



public class VpReport {

    public Nullable<Int32> DesignationID { get; set; }
    public string DesignationName { get; set; }
    public Nullable<Int32> TotalSanctioned { get; set; }
    public Nullable<Int32> TotalWorking { get; set; }
    public string CadreName { get; set; }
    public Nullable<Int32> Scale { get; set; }
    public string HFMISCode { get; set; }
    public string HfFullName { get; set; }
    public string HfTypeName { get; set; }
    public Nullable<Int32> TotalVacant { get; set; }
    public Nullable<Int32> TotalRegular { get; set; }
    public Nullable<Int32> TotalAdhoc { get; set; }
    public Nullable<Int32> TotalContract { get; set; }
    
}

public class VpReportExcel
{
    public string DesignationName { get; set; }
    public string CadreName { get; set; }
    public Nullable<Int32> Scale { get; set; }
    public Nullable<Int32> TotalSanctioned { get; set; }
    public Nullable<Int32> TotalWorking { get; set; }   
    public Nullable<Int32> TotalRegular { get; set; }
    public Nullable<Int32> TotalContract { get; set; }
    public Nullable<Int32> TotalAdhoc { get; set; }
    public Nullable<Int32> TotalVacant { get; set; }

}

public class VpReportHFExcel
{
    public string HfFullName { get; set; }
    public Nullable<Int32> TotalSanctioned { get; set; }
    public Nullable<Int32> TotalWorking { get; set; }
    public Nullable<Int32> TotalRegular { get; set; }
    public Nullable<Int32> TotalContract { get; set; }
    public Nullable<Int32> TotalAdhoc { get; set; }
    public Nullable<Int32> TotalVacant { get; set; }

}



