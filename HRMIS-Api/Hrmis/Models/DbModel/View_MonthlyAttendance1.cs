
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
    
public partial class View_MonthlyAttendance1
{

    public int Id { get; set; }

    public int HrId { get; set; }

    public Nullable<int> IndRegID { get; set; }

    public string EmployeeName { get; set; }

    public string Designation_Name { get; set; }

    public Nullable<int> Year { get; set; }

    public Nullable<int> Month { get; set; }

    public Nullable<System.DateTime> LogDate { get; set; }

    public string TImeIn { get; set; }

    public string TImeOut { get; set; }

    public int IsLate { get; set; }

    public Nullable<int> TotalInout { get; set; }

    public Nullable<int> WorkinHour { get; set; }

    public Nullable<int> SubDept_ID { get; set; }

    public string DName { get; set; }

    public string LogStatus { get; set; }

    public Nullable<int> IsHOD { get; set; }

    public string LeaveStatus { get; set; }

    public Nullable<decimal> BAL { get; set; }

    public decimal AvailedLate { get; set; }

    public decimal AbsentCount { get; set; }

    public decimal TotalLeaves { get; set; }

}

}
