
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
    
public partial class JobApplicantView
{

    public int Id { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public string StatusName { get; set; }

    public Nullable<int> ApplicationNumber { get; set; }

    public string CNIC { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string MobileNumber { get; set; }

    public string Address { get; set; }

    public Nullable<int> Domicile_Id { get; set; }

    public string DomicileName { get; set; }

    public Nullable<int> Religion_Id { get; set; }

    public Nullable<System.DateTime> DOB { get; set; }

    public string MaritalStatus { get; set; }

    public string MobileSec { get; set; }

    public string PMDCNumber { get; set; }

    public Nullable<bool> Experience { get; set; }

    public Nullable<bool> RelevantExperience { get; set; }

    public Nullable<bool> IsDisabled { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

}

}
