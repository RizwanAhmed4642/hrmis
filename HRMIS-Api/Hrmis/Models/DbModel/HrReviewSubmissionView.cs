
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
    
public partial class HrReviewSubmissionView
{

    public int Id { get; set; }

    public Nullable<int> ProfileId { get; set; }

    public string SubmitBy { get; set; }

    public string Remarks { get; set; }

    public string EmployeeName { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string Designation_Name { get; set; }

    public Nullable<int> EmpMode_Id { get; set; }

    public string EmpMode_Name { get; set; }

    public Nullable<int> Cadre_Id { get; set; }

    public string Cadre_Name { get; set; }

    public string StatusName { get; set; }

    public string ReviewStatusName { get; set; }

    public string HfmisCode { get; set; }

    public Nullable<int> HealthFacility_Id { get; set; }

    public string HealthFacility { get; set; }

    public string District { get; set; }

    public string Tehsil { get; set; }

    public Nullable<int> WDesignation_Id { get; set; }

    public string WDesignation_Name { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string UserId { get; set; }

    public string Username { get; set; }

    public Nullable<bool> IsActive { get; set; }

}

}
