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

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("PromotionESR")]
    public class PromotionESRController : ApiController
    {
        private HR_System db = new HR_System();

        public PromotionESRController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: PromotionESR
        //public async Task<IHttpActionResult> Index()
        //{
        //    var eSRs = db.ESRs.Include(e => e.EsrSectionOfficer);
        //    return Ok(await eSRs.ToListAsync());
        //}



        //// GET: PromotionESR/Create
        //public ActionResult Create()
        //{
        //    ViewBag.EsrSectionOfficerID = new SelectList(db.EsrSectionOfficers, "Id", "Name");
        //    return Ok();
        //}

        //// POST: PromotionESR/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpGet]
        [Route("BindingData")]
        public async Task<IHttpActionResult> BindingData()
        {
            try
            {
                var data = new
                {
                    Designations = db.HrDesignations.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList(),
                    Departments = db.HrDepartments.Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList()
                };

                return Ok(new Result<Object>
                {
                    Type = ResultType.Success.ToString(),
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<Object>
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong please try again later. Thank you"
                });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create([FromBody]ESR eSR)
        {
            try
            {
                Entity_Lifecycle eld = new Entity_Lifecycle();
                eld.Created_Date = DateTime.Parse(eSR.COMMENTS);
                eld.Created_By = User.Identity.GetUserName();
                eld.Users_Id = User.Identity.GetUserId();
                eld.IsActive = true;
                eld.Entity_Id = 465;
                db.Entity_Lifecycle.Add(eld);
                db.SaveChanges();

                eSR.EntityLifecycle_Id = (int)eld.Id;

                eSR.ModuleSource = "PromotionManagementSystem";
                eSR.COMMENTS = null;
                db.ESRs.Add(eSR);
                db.SaveChanges();

                return Ok(new Result<string>
                {
                    Type = ResultType.Success.ToString()
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<PromotionEsrView>()
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong please try again later. Thank you"
                });
            }
        }

        [HttpGet]
        [Route("Details/{EsrID}")]
        public async Task<IHttpActionResult> Details(long EsrID)
        {
            try
            {
                var esr = db.PromotionEsrViews.Where(x => x.Id == EsrID).FirstOrDefault();

                return Ok(new Result<PromotionEsrView>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = esr
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<PromotionEsrView>()
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong please try again later. Thank you"
                });
            }
        }

        //// GET: PromotionESR/Edit/5
        //public async Task<IHttpActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ESR eSR = await db.ESRs.FindAsync(id);
        //    if (eSR == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.EsrSectionOfficerID = new SelectList(db.EsrSectionOfficers, "Id", "Name", eSR.EsrSectionOfficerID);
        //    return Ok(eSR);
        //}

        //// POST: PromotionESR/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IHttpActionResult> Edit([Bind(Include = "Id,VDT,Profile_Id,CNIC,BPSFrom,BPSTo,CurrentDoJ,CurrentDoT,DesignationFrom,DesignationTo,DepartmentFrom,DepartmentTo,HF_Id_From,HF_Id_To,HfmisCodeFrom,HfmisCodeTo,PostingOrderNo,PostingOrderDate,LengthOfService,JoiningStatus,EmployeeFileNO,IsActive,EntityLifecycle_Id,Remarks,COMMENTS,PostingOrderPhoto,TargetUser,VPot,CurrentUser,WDesigFrom,OrderDetail,OrderNumer,TransferOrderType,TranferOrderStatus,VerbalOrderByDesignation,VerbalOrderByName,NotingFile,EmbossedFile,SectionOfficer,ResponsibleUser,TransferTypeID,DisposalofID,Disposalof,EsrSectionOfficerID,MutualESR_Id,OrderHTML")] ESR eSR)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(eSR).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.EsrSectionOfficerID = new SelectList(db.EsrSectionOfficers, "Id", "Name", eSR.EsrSectionOfficerID);
        //    return Ok(eSR);
        //}

        //// GET: PromotionESR/Delete/5
        //public async Task<IHttpActionResult> Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ESR eSR = await db.ESRs.FindAsync(id);
        //    if (eSR == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return Ok(eSR);
        //}

        //// POST: PromotionESR/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IHttpActionResult> DeleteConfirmed(long id)
        //{
        //    ESR eSR = await db.ESRs.FindAsync(id);
        //    db.ESRs.Remove(eSR);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
