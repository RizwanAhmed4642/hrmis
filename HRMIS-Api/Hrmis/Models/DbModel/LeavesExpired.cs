
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
    
public partial class LeavesExpired
{

    public int Id { get; set; }

    public string Barcode { get; set; }

    public string FileNumber { get; set; }

    public string EmployeeName { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public string StatusName { get; set; }

    public string Designation_Name { get; set; }

    public string LeaveTypeName { get; set; }

    public Nullable<System.DateTime> FromDate { get; set; }

    public Nullable<System.DateTime> ToDate { get; set; }

    public Nullable<byte> Officer_Id { get; set; }

    public string SignedBy { get; set; }

    public Nullable<int> DateDiff { get; set; }

}

}
