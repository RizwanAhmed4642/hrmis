
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
    
public partial class JobApplicantDocumentView
{

    public int Id { get; set; }

    public Nullable<int> Applicant_Id { get; set; }

    public Nullable<int> JobDocument_Id { get; set; }

    public Nullable<double> TotalMarks { get; set; }

    public Nullable<double> ObtainedMarks { get; set; }

    public string Degree { get; set; }

    public Nullable<System.DateTime> PassingYear { get; set; }

    public Nullable<int> Division { get; set; }

    public string Grade { get; set; }

    public string DocumentName { get; set; }

    public string UploadPath { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<int> OrderBy { get; set; }

}

}
