
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
    
public partial class JobApplicantsView
{

    public int Id { get; set; }

    public int Applicant_Id { get; set; }

    public int Job_Id { get; set; }

    public int Applicant_Status_Id { get; set; }

    public bool IsActive { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public Nullable<int> JobFacility_Id { get; set; }

    public bool IsLocked { get; set; }

    public bool IsDeleted { get; set; }

    public string JobBatch_No { get; set; }

    public Nullable<int> Department_Id { get; set; }

    public Nullable<bool> ShortList { get; set; }

    public string Remarks { get; set; }

    public Nullable<double> InterMarks { get; set; }

    public Nullable<double> GradMarks { get; set; }

    public Nullable<double> MastMarks { get; set; }

    public Nullable<double> HigherStageMarks { get; set; }

    public Nullable<double> MatricMarks { get; set; }

    public Nullable<double> Total { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public Nullable<bool> ShorlistStatus { get; set; }

    public string shorlistRemarks { get; set; }

    public string Address { get; set; }

    public string CurrentlyWorkingFacility { get; set; }

    public Nullable<System.DateTime> DOB { get; set; }

    public string Email { get; set; }

    public string Gender { get; set; }

    public string PhotoPath { get; set; }

    public string DesignationName { get; set; }

    public Nullable<System.DateTime> jobclosingDate { get; set; }

    public Nullable<int> joblevelId { get; set; }

    public string jobName { get; set; }

    public Nullable<bool> PrefrencesAvailable { get; set; }

}

}