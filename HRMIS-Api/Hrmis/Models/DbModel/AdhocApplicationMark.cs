
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
    
public partial class AdhocApplicationMark
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public Nullable<int> ApplicantQualification_Id { get; set; }

    public Nullable<int> Marks_Id { get; set; }

    public Nullable<double> Marks { get; set; }

    public string Grade { get; set; }

    public Nullable<double> Percentage { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> Error { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<int> BatchApplicationId { get; set; }

    public Nullable<bool> IsVerified { get; set; }

    public Nullable<System.DateTime> VerifiedDatetime { get; set; }

    public string VerifiedBy { get; set; }

    public string VerifiedByUserId { get; set; }

}

}
