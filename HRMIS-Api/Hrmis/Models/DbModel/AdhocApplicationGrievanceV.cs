
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
    
public partial class AdhocApplicationGrievanceV
{

    public int Id { get; set; }

    public Nullable<int> Applicant_Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public string DomicileName { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string DesignationName { get; set; }

    public Nullable<int> StatusId { get; set; }

    public string Status { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

}

}