
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
    
public partial class ApplicantExperience
{

    public int Id { get; set; }

    public int Applicant_Id { get; set; }

    public int Applied_Job_Id { get; set; }

    public string Organization { get; set; }

    public string JobTitle { get; set; }

    public System.DateTime FromDate { get; set; }

    public Nullable<System.DateTime> ToDate { get; set; }

    public Nullable<bool> IsContinue { get; set; }

    public string ExperienceType { get; set; }

    public string UploadPath { get; set; }

    public bool IsActive { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

}

}
