using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hrmis.Models.DbModel;

namespace Hrmis.Controllers
{
    public class PromotionJobApplicationsController : Controller
    {
        private HR_System db = new HR_System();

        // GET: PromotionJobApplications
        public ActionResult Index()
        {
            var promotionJobApplications = db.PromotionJobApplications.Include(p => p.HrDesignation).Include(p => p.HrProfile);
            return View(promotionJobApplications.ToList());
        }

        // GET: PromotionJobApplications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionJobApplication promotionJobApplication = db.PromotionJobApplications.Find(id);
            if (promotionJobApplication == null)
            {
                return HttpNotFound();
            }
            return View(promotionJobApplication);
        }

        // GET: PromotionJobApplications/Create
        public ActionResult Create()
        {
            ViewBag.AppliedForDesignation_Id = new SelectList(db.HrDesignations, "Id", "Name");
            ViewBag.Profile_Id = new SelectList(db.HrProfiles, "Id", "EmployeeName");
            return View();
        }

        // POST: PromotionJobApplications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RequestedAs,AppliedForDesignation_Id,Profile_Id,CNIC,Name,FatherName,Address,Email,MobileNumber,DistrictOfDomicile,HFMIS,PromotionToCurrentScale,SeniorityNo,CertificateOfWorkingFilepath,NoEnquiryCeritificateFilepath,MatricFScMbbsDegreeFilepath,PostgraduateDegreeFilepath,PmdcFilepath,NoEnquiryCertifcateAttestedFilepath,ServiceStatementFirst,ServiceStatementSecond,ServiceStatementThird,CreatedOn")] PromotionJobApplication promotionJobApplication)
        {
            if (ModelState.IsValid)
            {
                db.PromotionJobApplications.Add(promotionJobApplication);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppliedForDesignation_Id = new SelectList(db.HrDesignations, "Id", "Name", promotionJobApplication.AppliedForDesignation_Id);
            ViewBag.Profile_Id = new SelectList(db.HrProfiles, "Id", "EmployeeName", promotionJobApplication.Profile_Id);
            return View(promotionJobApplication);
        }

        // GET: PromotionJobApplications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionJobApplication promotionJobApplication = db.PromotionJobApplications.Find(id);
            if (promotionJobApplication == null)
            {
                return HttpNotFound();
            }
            ViewBag.AppliedForDesignation_Id = new SelectList(db.HrDesignations, "Id", "Name", promotionJobApplication.AppliedForDesignation_Id);
            ViewBag.Profile_Id = new SelectList(db.HrProfiles, "Id", "EmployeeName", promotionJobApplication.Profile_Id);
            return View(promotionJobApplication);
        }

        // POST: PromotionJobApplications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RequestedAs,AppliedForDesignation_Id,Profile_Id,CNIC,Name,FatherName,Address,Email,MobileNumber,DistrictOfDomicile,HFMIS,PromotionToCurrentScale,SeniorityNo,CertificateOfWorkingFilepath,NoEnquiryCeritificateFilepath,MatricFScMbbsDegreeFilepath,PostgraduateDegreeFilepath,PmdcFilepath,NoEnquiryCertifcateAttestedFilepath,ServiceStatementFirst,ServiceStatementSecond,ServiceStatementThird,CreatedOn")] PromotionJobApplication promotionJobApplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promotionJobApplication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppliedForDesignation_Id = new SelectList(db.HrDesignations, "Id", "Name", promotionJobApplication.AppliedForDesignation_Id);
            ViewBag.Profile_Id = new SelectList(db.HrProfiles, "Id", "EmployeeName", promotionJobApplication.Profile_Id);
            return View(promotionJobApplication);
        }

        // GET: PromotionJobApplications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionJobApplication promotionJobApplication = db.PromotionJobApplications.Find(id);
            if (promotionJobApplication == null)
            {
                return HttpNotFound();
            }
            return View(promotionJobApplication);
        }

        // POST: PromotionJobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PromotionJobApplication promotionJobApplication = db.PromotionJobApplications.Find(id);
            db.PromotionJobApplications.Remove(promotionJobApplication);
            db.SaveChanges();
            return RedirectToAction("Index");
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
