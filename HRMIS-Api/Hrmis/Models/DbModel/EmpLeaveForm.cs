
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
    
public partial class EmpLeaveForm
{

    public int ID { get; set; }

    public Nullable<int> ProfileID { get; set; }

    public Nullable<System.DateTime> LeaveEnterDate { get; set; }

    public Nullable<int> LeaveTypeID { get; set; }

    public Nullable<System.DateTime> LeaveFrom { get; set; }

    public Nullable<System.DateTime> LeaveTo { get; set; }

    public Nullable<decimal> TotalDays { get; set; }

    public string Reason { get; set; }

    public string ContactInfo { get; set; }

    public Nullable<int> ForwardedByID { get; set; }

    public Nullable<int> ApprovalByID { get; set; }

    public Nullable<int> CreatedBy { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public Nullable<int> LeaveStatusID { get; set; }

    public string Cnic { get; set; }

    public Nullable<decimal> CBalance { get; set; }

}

}
