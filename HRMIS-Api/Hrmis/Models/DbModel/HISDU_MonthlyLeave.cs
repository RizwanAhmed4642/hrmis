
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
    
public partial class HISDU_MonthlyLeave
{

    public int Id { get; set; }

    public Nullable<int> ProfileID { get; set; }

    public Nullable<int> Year { get; set; }

    public Nullable<int> Month { get; set; }

    public Nullable<decimal> Lates { get; set; }

    public Nullable<decimal> ShortLeave { get; set; }

    public Nullable<decimal> SickLeave { get; set; }

    public Nullable<decimal> CasualLeave { get; set; }

    public Nullable<decimal> AvailedLate { get; set; }

    public Nullable<decimal> Absent { get; set; }

    public Nullable<decimal> TotalLeaves { get; set; }

    public Nullable<decimal> CBalance { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public string EmployeeName { get; set; }

    public string Designation_Name { get; set; }

    public Nullable<int> SubDept_ID { get; set; }

    public string DName { get; set; }

    public string MonthName { get; set; }

}

}
