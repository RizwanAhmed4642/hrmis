
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Hrmis.Models.DbModel
{

using System;
    using System.Collections.Generic;
    
public partial class ProfileDetailsView
{

    public int Id { get; set; }

    public Nullable<long> Srno { get; set; }

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

    public Nullable<int> LengthOfService { get; set; }

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

    public string HfmisCode { get; set; }

    public string Tenure { get; set; }

    public string AdditionalCharge { get; set; }

    public string Remarks { get; set; }

    public byte[] Photo { get; set; }

    public Nullable<int> Cadre_Id { get; set; }

    public string HighestQualification { get; set; }

    public string MobileNoOfficial { get; set; }

    public Nullable<int> Postaanctionedwithscale { get; set; }

    public string Faxno { get; set; }

    public string HoD { get; set; }

    public string Fp { get; set; }

    public string Hfac { get; set; }

    public Nullable<System.DateTime> DateOfRegularization { get; set; }

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

    public Nullable<long> EntityLifecycle_Id { get; set; }

    public string ProfilePhoto { get; set; }

    public Nullable<int> Disability_Id { get; set; }

    public string DisablityType { get; set; }

    public string Disability { get; set; }

    public string FileNumber { get; set; }

    public string AttachedWith { get; set; }

    public Nullable<int> AttachedWith_Id { get; set; }

    public Nullable<System.DateTime> PresentJoiningDate { get; set; }

    public string VacCertificate { get; set; }

    public Nullable<int> PPSCMeritNumber { get; set; }

    public Nullable<System.DateTime> FirstJoiningDate { get; set; }

    public Nullable<int> ModeId { get; set; }

    public string ModeName { get; set; }

    public Nullable<bool> IsVerified { get; set; }

    public string VerifiedBy { get; set; }

    public Nullable<System.DateTime> VerifiedDatetime { get; set; }

    public string VerifiedUserId { get; set; }

    public Nullable<System.DateTime> FirstOrderDate { get; set; }

    public string FirstOrderNumber { get; set; }

    public string RegularOrderNumber { get; set; }

    public string PromotionOrderNumber { get; set; }

    public string ContractOrderNumber { get; set; }

    public Nullable<System.DateTime> ContractOrderDate { get; set; }

    public string OtherContract { get; set; }

    public Nullable<int> PeriodofContract { get; set; }

    public Nullable<System.DateTime> PromotionJoiningDate { get; set; }

    public string Division { get; set; }

    public string District { get; set; }

    public string Tehsil { get; set; }

    public string WorkingDivision { get; set; }

    public string WorkingDistrict { get; set; }

    public string WorkingTehsil { get; set; }

    public string DivisionOld { get; set; }

    public string DistrictOld { get; set; }

    public string TehsilOld { get; set; }

    public string HealthFacility { get; set; }

    public string HFTypeCode { get; set; }

    public string HFTypeName { get; set; }

    public Nullable<int> HealthFacility_Id { get; set; }

    public string WorkingHealthFacility { get; set; }

    public string WHFTypeCode { get; set; }

    public string WHFTypeName { get; set; }

    public Nullable<int> WorkingHealthFacility_Id { get; set; }

    public string WorkingHFMISCode { get; set; }

    public string StatusName { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public string QualificationName { get; set; }

    public Nullable<int> Qualification_Id { get; set; }

    public string SpecializationName { get; set; }

    public Nullable<int> Specialization_Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string Designation_Name { get; set; }

    public Nullable<int> Designation_HrScale_Id { get; set; }

    public Nullable<int> JDesignation_Id { get; set; }

    public string JDesignation_Name { get; set; }

    public Nullable<int> JDesignation_HrScale_Id { get; set; }

    public string Cadre_Name { get; set; }

    public Nullable<int> WDesignation_Id { get; set; }

    public string WDesignation_Name { get; set; }

    public Nullable<int> WDesignation_HrScale_Id { get; set; }

    public Nullable<int> Department_Id { get; set; }

    public string Department_Name { get; set; }

    public Nullable<int> EmpMode_Id { get; set; }

    public string EmpMode_Name { get; set; }

    public Nullable<int> Domicile_Id { get; set; }

    public string Domicile_Name { get; set; }

    public Nullable<int> Posttype_Id { get; set; }

    public string PostType_Name { get; set; }

    public Nullable<int> Religion_Id { get; set; }

    public string Religion_Name { get; set; }

    public Nullable<int> Language_Id { get; set; }

    public string Language_Name { get; set; }

}

}
