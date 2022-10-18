using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Hrmis.Models.Common;

namespace Hrmis.Controllers.PublicControllers
{
    [RoutePrefix("api/WMOs")]
    public class WMOsController : ApiController
    {
        private HR_System db = new HR_System();
        public WMOsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        [HttpPost]
        [Route("UpdateMeritProfile")]
        public IHttpActionResult UpdateMeritProfile(HrProfile hrProfile)
        {

            try
            {
                hrProfile.HealthFacility = null;
                hrProfile.CNIC = hrProfile.CNIC.Replace("-", "");
                hrProfile.MobileNo = hrProfile.MobileNo.Replace("-", "");
                hrProfile.Province = "Punjab";
                hrProfile.Status_Id = 16;
                hrProfile.Department_Id = 25;
                hrProfile.Designation_Id = 1320;
                hrProfile.WDesignation_Id = 1320;
                hrProfile.EmpMode_Id = 13;
                hrProfile.Postaanctionedwithscale = "17";
                hrProfile.CurrentGradeBPS = 17;
                hrProfile.Cadre_Id = 3;
                hrProfile.Hfac = "GOP";
                hrProfile.Posttype_Id = 3;
                hrProfile.HoD = "No";

                if (!ModelState.IsValid) return BadRequest(ModelState);
                db.Entry(hrProfile).State = EntityState.Modified;
                db.SaveChanges();


                var merit = db.Merits.FirstOrDefault(z => z.CNIC == hrProfile.CNIC);
                merit.Status = "ProfileBuilt";
                db.SaveChanges();



                Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = hrProfile.MobileNo,
                                    Message = "Your profile has been saved successfully for Posting of Women Medical Officers Through PPSC"
                                }
                            });
                string EmailMessage = "Your profile has been saved successfully for Posting of Women Medical Officers Through PPSC";
                Common.SendEmail(hrProfile.EMaiL, "Primary Secondary Healthcare Department", EmailMessage);

                return Ok(new { Id = hrProfile.Id, CNIC = hrProfile.CNIC });
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Route("PostMeritProfile/{MeritNumber}/{ApplicationNumber}/{PMDCNumber}")]
        [HttpPost]
        public IHttpActionResult PostMeritProfile([FromBody] HrProfile hrProfile, int MeritNumber, int ApplicationNumber, string PMDCNumber)
        {



            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                Merit meritExists = db.Merits.FirstOrDefault(x => x.CNIC.Equals(hrProfile.CNIC));
                if (meritExists != null)
                {
                    return Ok("Profile Merit Already Exist");
                }
                HrProfile hrProfileExists = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(hrProfile.CNIC));
                if (hrProfileExists != null)
                {
                    hrProfileExists.EmployeeName = hrProfile.EmployeeName;
                    hrProfileExists.FatherName = hrProfile.FatherName;
                    hrProfileExists.DateOfBirth = hrProfile.DateOfBirth;
                    hrProfileExists.Gender = hrProfile.Gender;
                    hrProfileExists.Domicile_Id = hrProfile.Domicile_Id;
                    hrProfileExists.PermanentAddress = hrProfile.PermanentAddress;
                    hrProfileExists.MobileNo = hrProfile.MobileNo;
                    hrProfileExists.EMaiL = hrProfile.EMaiL;
                    hrProfileExists.HealthFacility = null;
                    hrProfileExists.CNIC = hrProfile.CNIC.Replace("-", "");
                    hrProfileExists.MobileNo = hrProfile.MobileNo.Replace("-", "");
                    hrProfileExists.Province = "Punjab";
                    hrProfileExists.Status_Id = 16;
                    hrProfileExists.Department_Id = 25;
                    hrProfileExists.Designation_Id = 1320;
                    hrProfileExists.WDesignation_Id = 1320;
                    hrProfileExists.EmpMode_Id = 13;
                    hrProfileExists.Postaanctionedwithscale = "17";
                    hrProfileExists.CurrentGradeBPS = 17;
                    hrProfileExists.Cadre_Id = 3;
                    hrProfileExists.Hfac = "GOP";
                    hrProfileExists.Posttype_Id = 3;
                    hrProfileExists.HoD = "No";
                    hrProfile = hrProfileExists;
                    db.Entry(hrProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    hrProfile.HealthFacility = null;
                    hrProfile.CNIC = hrProfile.CNIC.Replace("-", "");
                    hrProfile.MobileNo = hrProfile.MobileNo.Replace("-", "");
                    hrProfile.Province = "Punjab";
                    hrProfile.Status_Id = 16;
                    hrProfile.Department_Id = 25;
                    hrProfile.Designation_Id = 1320;
                    hrProfile.WDesignation_Id = 1320;
                    hrProfile.EmpMode_Id = 13;
                    hrProfile.Gender = "Female";
                    hrProfile.Postaanctionedwithscale = "17";
                    hrProfile.CurrentGradeBPS = 17;
                    hrProfile.Cadre_Id = 3;
                    hrProfile.Hfac = "GOP";
                    hrProfile.Posttype_Id = 3;
                    hrProfile.HoD = "No";


                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 9;

                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();
                    hrProfile.EntityLifecycle_Id = eld.Id;
                    db.HrProfiles.Add(hrProfile);
                    db.SaveChanges();
                }


                var merit = new Merit()
                {
                    Address = hrProfile.PermanentAddress,
                    ApplicationNumber = ApplicationNumber,
                    MeritNumber = MeritNumber,
                    PMDCNumber = PMDCNumber,
                    CNIC = hrProfile.CNIC,
                    Designation_Id = hrProfile.Designation_Id,
                    Email = hrProfile.EMaiL,
                    FatherName = hrProfile.FatherName,
                    MobileNumber = hrProfile.MobileNo,
                    Name = hrProfile.EmployeeName,
                    Status = "New"
                };

                db.Merits.Add(merit);
                db.SaveChanges();

                return Ok(new { Id = hrProfile.Id, CNIC = hrProfile.CNIC });
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
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
    }

    //public partial class HrProfileMerit
    //{
    //    public int Id { get; set; }
    //    public Nullable<long> Srno_old { get; set; }
    //    public string EmployeeName { get; set; }
    //    public string FatherName { get; set; }
    //    public string CNIC { get; set; }
    //    public Nullable<System.DateTime> DateOfBirth { get; set; }
    //    public string Gender { get; set; }
    //    public string Province { get; set; }
    //    public string MaritalStatus { get; set; }
    //    public string BloodGroup { get; set; }
    //    public string SeniorityNo { get; set; }
    //    public string PersonnelNo { get; set; }
    //    public Nullable<int> JoiningGradeBPS { get; set; }
    //    public Nullable<int> CurrentGradeBPS { get; set; }
    //    public string PresentPostingOrderNo { get; set; }
    //    public Nullable<System.DateTime> PresentPostingDate { get; set; }
    //    public string AdditionalQualification { get; set; }
    //    public string Status { get; set; }
    //    public Nullable<System.DateTime> DateOfFirstAppointment { get; set; }
    //    public Nullable<System.DateTime> SuperAnnuationDate { get; set; }
    //    public Nullable<System.DateTime> ContractStartDate { get; set; }
    //    public Nullable<System.DateTime> ContractEndDate { get; set; }
    //    public Nullable<System.DateTime> LastPromotionDate { get; set; }
    //    public string PermanentAddress { get; set; }
    //    public string CorrespondenceAddress { get; set; }
    //    public string LandlineNo { get; set; }
    //    public string MobileNo { get; set; }
    //    public string EMaiL { get; set; }
    //    public string PrivatePractice { get; set; }
    //    public string PresentStationLengthOfService { get; set; }
    //    public string Tenure { get; set; }
    //    public string AdditionalCharge { get; set; }
    //    public string Remarks { get; set; }
    //    public byte[] Photo { get; set; }
    //    public string HighestQualification { get; set; }
    //    public string MobileNoOfficial { get; set; }
    //    public string Postaanctionedwithscale { get; set; }
    //    public string Faxno { get; set; }
    //    public string HoD { get; set; }
    //    public string Fp { get; set; }
    //    public string Hfac { get; set; }
    //    public Nullable<System.DateTime> DateOfRegularization { get; set; }
    //    public string Tbydeo { get; set; }
    //    public string DateOfCourse { get; set; }
    //    public string RtmcNo { get; set; }
    //    public string PmdcNo { get; set; }
    //    public string CourseDuration { get; set; }
    //    public string PgSpecialization { get; set; }
    //    public string Category { get; set; }
    //    public string RemunerationStatus { get; set; }
    //    public string PgFlag { get; set; }
    //    public string CourseName { get; set; }
    //    public Nullable<bool> AddToEmployeePool { get; set; }
    //    public Nullable<int> Domicile_Id { get; set; }
    //    public Nullable<int> Language_Id { get; set; }
    //    public Nullable<int> Designation_Id { get; set; }
    //    public Nullable<int> WDesignation_Id { get; set; }
    //    public Nullable<int> Cadre_Id { get; set; }
    //    public Nullable<int> EmpMode_Id { get; set; }
    //    public Nullable<int> HealthFacility_Id { get; set; }
    //    public Nullable<int> Department_Id { get; set; }
    //    public Nullable<int> Religion_Id { get; set; }
    //    public Nullable<int> Posttype_Id { get; set; }
    //    public string HfmisCode { get; set; }
    //    public string HfmisCodeOld { get; set; }
    //    public string Created_By { get; set; }
    //    public Nullable<System.DateTime> Creation_Date { get; set; }
    //    public Nullable<bool> IsActive { get; set; }
    //    public Nullable<long> EntityLifecycle_Id { get; set; }
    //    public Nullable<int> Qualification_Id { get; set; }
    //    public Nullable<int> Status_Id { get; set; }
    //    public Nullable<int> Specialization_Id { get; set; }

    //    public Nullable<string> MeritNumber { get; set; }
    //    public Nullable<int> ApplicationNumber { get; set; }

    //}
}
