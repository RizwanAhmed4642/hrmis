
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
    
public partial class ApplicationForwardLog
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public Nullable<int> TrackingNo { get; set; }

    public string FromOfficerName { get; set; }

    public string ToOfficerName { get; set; }

    public Nullable<int> FromOfficer_Id { get; set; }

    public Nullable<int> ToOfficer_Id { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string User_Id { get; set; }

    public string Username { get; set; }

    public Nullable<bool> IsRecieved { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> RcvDateTime { get; set; }

}

}
