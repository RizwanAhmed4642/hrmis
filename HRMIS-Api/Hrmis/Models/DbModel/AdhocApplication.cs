
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
    
public partial class AdhocApplication
{

    public int Id { get; set; }

    public Nullable<int> Applicant_Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> HF_Id { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public Nullable<int> Job_Id { get; set; }

    public Nullable<int> InterviewMarks { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public string StatusChangedBy { get; set; }

    public string StatusChangedByUserId { get; set; }

    public Nullable<System.DateTime> StatusChangedDateTime { get; set; }

    public string StatusChangedByDistrict { get; set; }

    public Nullable<int> GrievanceStatus { get; set; }

    public Nullable<System.DateTime> GrievanceDateTime { get; set; }

    public Nullable<bool> AcceptedByGrievance { get; set; }

    public string GrievanceRemarks { get; set; }

    public Nullable<bool> IsPostedOnAdhoc { get; set; }

    public Nullable<System.DateTime> PostingDatetime { get; set; }

}

}
