
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
    
public partial class AdhocScrutiny
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public string DocName { get; set; }

    public Nullable<bool> IsAccepted { get; set; }

    public Nullable<bool> IsRejected { get; set; }

    public Nullable<int> Reason_Id { get; set; }

    public Nullable<int> Qualification_Id { get; set; }

    public Nullable<int> Experience_Id { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public string UploadPath { get; set; }

    public Nullable<bool> GrievanceAccepted { get; set; }

    public Nullable<System.DateTime> GrievanceAcceptedTime { get; set; }

    public string GrievanceRemarks { get; set; }

    public Nullable<int> IsCorrectedByApplicant { get; set; }

}

}