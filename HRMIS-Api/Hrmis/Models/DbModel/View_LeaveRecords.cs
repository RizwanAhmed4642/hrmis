
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
    
public partial class View_LeaveRecords
{

    public int ID { get; set; }

    public Nullable<int> ProfileID { get; set; }

    public Nullable<System.DateTime> LeaveEnterDate { get; set; }

    public Nullable<int> LeaveTypeID { get; set; }

    public string LeaveFrom { get; set; }

    public string LeaveTo { get; set; }

    public Nullable<decimal> TotalDays { get; set; }

    public string Reason { get; set; }

    public string ContactInfo { get; set; }

    public Nullable<int> ForwardedByID { get; set; }

    public Nullable<int> LeaveStatusID { get; set; }

    public string StatusName { get; set; }

    public string LeaveType { get; set; }

    public Nullable<decimal> CBalance { get; set; }

    public Nullable<System.DateTime> LeaveStart { get; set; }

    public Nullable<System.DateTime> LeaveEnd { get; set; }

    public string EmployeeName { get; set; }

    public string Designation_Name { get; set; }

    public Nullable<int> SubDept_ID { get; set; }

    public int IsHOD { get; set; }

    public string DName { get; set; }

    public Nullable<int> IndRegID { get; set; }

}

}
