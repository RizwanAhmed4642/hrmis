
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
    
public partial class PerView
{

    public int Id { get; set; }

    public int PerType_Id { get; set; }

    public int HrProfile_Id { get; set; }

    public Nullable<int> LeaveType_Id { get; set; }

    public int PostHeld { get; set; }

    public int Scale { get; set; }

    public int Year { get; set; }

    public System.DateTime FromPeriod { get; set; }

    public System.DateTime ToPeriod { get; set; }

    public string Duties { get; set; }

    public string ExceptionalContribution { get; set; }

    public string MoreEffective { get; set; }

    public string PenPicture { get; set; }

    public string CoName { get; set; }

    public Nullable<int> CoDesignation_Id { get; set; }

    public Nullable<System.DateTime> CoDate { get; set; }

    public string RoName { get; set; }

    public Nullable<int> RoDesignation_Id { get; set; }

    public Nullable<System.DateTime> RoDate { get; set; }

    public int EntityLifecycle_Id { get; set; }

    public string EmployeeName { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public Nullable<System.DateTime> DateOfBirth { get; set; }

    public Nullable<System.DateTime> DateOfFirstAppointment { get; set; }

    public Nullable<int> proDesignationID { get; set; }

    public Nullable<int> CurrentGradeBPS { get; set; }

    public Nullable<int> Qualification_Id { get; set; }

    public string DoctorDesignationName { get; set; }

    public string DoctorQualificationName { get; set; }

    public string RoDesignationName { get; set; }

    public string CoDesignationName { get; set; }

    public string RoPostHeldName { get; set; }

    public string OgByRO { get; set; }

    public Nullable<int> OgByCoID { get; set; }

    public string OgByCO { get; set; }

    public string Integrity { get; set; }

    public Nullable<int> FitnessGradingValsID { get; set; }

    public string Fitness { get; set; }

    public string TypeName { get; set; }

    public string LeaveTypeName { get; set; }

    public decimal Score { get; set; }

}

}
