using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hrmis.Models.Dto
{
    public class HrSeniorityApplicationDTO
    {
        public int Id { get; set; }
        public Nullable<long> Srno_old { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Province { get; set; }
        public string MaritalStatus { get; set; }
        public string BloodGroup { get; set; }
        public string SeniorityNo { get; set; }
        public string PersonnelNo { get; set; }
        public Nullable<int> JoiningGradeBPS { get; set; }
        public Nullable<int> CurrentGradeBPS { get; set; }
        public string PresentPostingOrderNo { get; set; }
        public Nullable<System.DateTime> PresentPostingDate { get; set; }
        public string AdditionalQualification { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateOfFirstAppointment { get; set; }
        public Nullable<System.DateTime> SuperAnnuationDate { get; set; }
        public Nullable<System.DateTime> ContractStartDate { get; set; }
        public Nullable<System.DateTime> ContractEndDate { get; set; }
        public Nullable<System.DateTime> LastPromotionDate { get; set; }
        public string PermanentAddress { get; set; }
        public string CorrespondenceAddress { get; set; }
        public string LandlineNo { get; set; }
        public string MobileNo { get; set; }
        public string EMaiL { get; set; }
        public string PrivatePractice { get; set; }
        public string PresentStationLengthOfService { get; set; }
        public string Tenure { get; set; }
        public string AdditionalCharge { get; set; }
        public string Remarks { get; set; }
        public byte[] Photo { get; set; }
        public string HighestQualification { get; set; }
        public string MobileNoOfficial { get; set; }
        public string Postaanctionedwithscale { get; set; }
        public string Faxno { get; set; }
        public string HoD { get; set; }
        public string Fp { get; set; }
        public string Hfac { get; set; }
        public Nullable<System.DateTime> DateOfRegularization { get; set; }
        public Nullable<System.DateTime> PromotionJoiningDate { get; set; }
        public string Tbydeo { get; set; }
        public string DateOfCourse { get; set; }
        public string RtmcNo { get; set; }
        public string PmdcNo { get; set; }
        public string CourseDuration { get; set; }
        public string PgSpecialization { get; set; }
        public string Category { get; set; }
        public string RemunerationStatus { get; set; }
        public string PgFlag { get; set; }
        public string CourseName { get; set; }
        public Nullable<bool> AddToEmployeePool { get; set; }
        public Nullable<int> Domicile_Id { get; set; }
        public Nullable<int> Language_Id { get; set; }
        public Nullable<int> Designation_Id { get; set; }
        public Nullable<int> WDesignation_Id { get; set; }
        public Nullable<int> Cadre_Id { get; set; }
        public Nullable<int> EmpMode_Id { get; set; }
        public Nullable<int> HealthFacility_Id { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public Nullable<int> Religion_Id { get; set; }
        public Nullable<int> Posttype_Id { get; set; }
        public string HfmisCode { get; set; }
        public string HfmisCodeOld { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> EntityLifecycle_Id { get; set; }
        public Nullable<int> Qualification_Id { get; set; }
        public Nullable<int> Status_Id { get; set; }
        public Nullable<int> Specialization_Id { get; set; }
        public string ProfilePhoto { get; set; }
        public Nullable<int> WorkingHealthFacility_Id { get; set; }
        public string WorkingHFMISCode { get; set; }
        public Nullable<int> Disability_Id { get; set; }
        public string Disability { get; set; }
        public Nullable<System.DateTime> PresentJoiningDate { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public string AttachedWith { get; set; }
        public Nullable<int> AttachedWith_Id { get; set; }
        public string FileNumber { get; set; }
        public string VacCertificate { get; set; }
        public Nullable<int> PPSCMeritNumber { get; set; }
        public Nullable<int> ModeId { get; set; }
        public Nullable<System.DateTime> FirstJoiningDate { get; set; }
        public Nullable<int> Profile_Id { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> SeniorityNumber { get; set; }
        public string DesignationNameC { get; set; }
        public string EmploymentModeC { get; set; }
        public string HealthFacilityNameC { get; set; }
        public string DistrictC { get; set; }
        public string HealthFacility { get; set; }
        public string Tehsil { get; set; }
        public string District { get; set; }
        public string StatusName { get; set; }
        public string Designation_Name { get; set; }
        public string Domicile_Name { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public string VerifiedBy { get; set; }
        public Nullable<System.DateTime> VerifiedDatetime { get; set; }
        public string VerifiedUserId { get; set; }
        public string ModeName { get; set; }
    }
}