
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
    
public partial class LeaveHistory
{

    public int Id { get; set; }

    public string Cnic { get; set; }

    public string HfmisCode { get; set; }

    public string DepartmentName { get; set; }

    public string TypeOfleave { get; set; }

    public Nullable<System.DateTime> LeaveStartDate { get; set; }

    public Nullable<System.DateTime> LeaveEndDate { get; set; }

    public Nullable<int> NoOfDays { get; set; }

    public string LeaveStatus { get; set; }

    public string LeaveOrDerno { get; set; }

    public Nullable<System.DateTime> LeaveOrderDate { get; set; }

    public byte[] LeaveOrderImage { get; set; }

    public long SrNo { get; set; }

    public byte[] LeaveOrderScan { get; set; }

    public Nullable<int> CurrentGrade { get; set; }

    public string ResponsibleUser { get; set; }

    public Nullable<System.DateTime> DayAndTime { get; set; }

    public string NotingFile { get; set; }

    public string EmbossedFile { get; set; }

    public string SectionOfficer { get; set; }

    public string EmployeeFileNo { get; set; }

    public string TransferOrderType { get; set; }

    public string TransferOrderStatus { get; set; }

    public string VerbalOrderByDesignation { get; set; }

    public string VerbalOrderName { get; set; }

}

}