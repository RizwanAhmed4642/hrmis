
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
    
public partial class PromoJobApplicationVM
{

    public int Id { get; set; }

    public Nullable<int> RequestedAs { get; set; }

    public Nullable<int> AppliedForDesignation_Id { get; set; }

    public Nullable<int> AppliedForDesignationSR_Id { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public string CNIC { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string Address { get; set; }

    public string Email { get; set; }

    public string MobileNumber { get; set; }

    public Nullable<int> DistrictOfDomicile { get; set; }

    public string HealthFacilityName { get; set; }

    public string HFMIS { get; set; }

    public Nullable<System.DateTime> PromotionToCurrentScale { get; set; }

    public string SeniorityNo { get; set; }

    public string SeniorityNoFilepath { get; set; }

    public string CertificateOfWorkingFilepath { get; set; }

    public string NoEnquiryCeritificateFilepath { get; set; }

    public string MatricFScMbbsDegreeFilepath { get; set; }

    public string PostgraduateDegreeFilepath { get; set; }

    public string PmdcFilepath { get; set; }

    public string NoEnquiryCertifcateAttestedFilepath { get; set; }

    public string ExperienceCertFilePath { get; set; }

    public string SignedCopyFielpath { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public string AdditionalInfo { get; set; }

    public string AdditionalInfoWorkingCertificate { get; set; }

    public string PresentPostingStatus { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> DateOfBirth { get; set; }

    public string Status { get; set; }

    public string StatusRemarks { get; set; }

    public string Speciality { get; set; }

    public string AppliedForDesignationName { get; set; }

    public string AppliedForDesignationSrName { get; set; }

    public string CurrentDesignationName { get; set; }

    public string DistrictName { get; set; }

    public string HfName { get; set; }

}

}
