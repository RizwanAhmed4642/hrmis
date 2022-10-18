
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
    
public partial class ApplicationLog
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public string Remarks { get; set; }

    public Nullable<int> Action_Id { get; set; }

    public Nullable<bool> SMS_SentToApplicant { get; set; }

    public Nullable<bool> SMS_SentToOfficer { get; set; }

    public Nullable<int> StatusByOfficer_Id { get; set; }

    public string StatusByOfficer { get; set; }

    public string FromStatus { get; set; }

    public string ToStatus { get; set; }

    public Nullable<int> FromStatus_Id { get; set; }

    public Nullable<int> ToStatus_Id { get; set; }

    public string FromOfficerName { get; set; }

    public string ToOfficerName { get; set; }

    public Nullable<bool> IsReceived { get; set; }

    public Nullable<System.DateTime> ReceivedTime { get; set; }

    public Nullable<int> FromOfficer_Id { get; set; }

    public Nullable<int> ToOfficer_Id { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string User_Id { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> FileRequestTime { get; set; }

    public Nullable<int> FileRequest_Id { get; set; }

    public Nullable<int> FileRequestByOfficer_Id { get; set; }

    public Nullable<int> FileRequestLog_Id { get; set; }

    public Nullable<int> RemarksByOfficer_Id { get; set; }

    public string RemarksByOfficer { get; set; }

    public Nullable<int> Purpose_Id { get; set; }

    public Nullable<System.DateTime> DueDate { get; set; }

}

}