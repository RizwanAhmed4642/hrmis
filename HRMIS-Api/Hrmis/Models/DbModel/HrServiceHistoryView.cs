
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
    
public partial class HrServiceHistoryView
{

    public int Id { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public Nullable<int> HF_Id { get; set; }

    public Nullable<long> ESR_Id { get; set; }

    public Nullable<int> ELR_Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> EmpMode_Id { get; set; }

    public Nullable<bool> PendingJoining { get; set; }

    public string OrderNumber { get; set; }

    public Nullable<System.DateTime> OrderDate { get; set; }

    public string OrderFilePath { get; set; }

    public Nullable<bool> Continued { get; set; }

    public Nullable<System.DateTime> From_Date { get; set; }

    public Nullable<System.DateTime> To_Date { get; set; }

    public Nullable<int> TotalDays { get; set; }

    public Nullable<int> Scale { get; set; }

    public string FullName { get; set; }

    public string DesignationName { get; set; }

    public Nullable<int> DesignationScale { get; set; }

    public string EmpModeName { get; set; }

    public string Created_By { get; set; }

    public string Users_Id { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

}

}
