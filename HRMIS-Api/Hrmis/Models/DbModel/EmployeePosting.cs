
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
    
public partial class EmployeePosting
{

    public int Id { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> PostingHF_Id { get; set; }

    public string PostingHFMISCode { get; set; }

    public string CNIC { get; set; }

    public string MeritNo { get; set; }

    public Nullable<int> Division_Id { get; set; }

    public Nullable<int> District_Id { get; set; }

    public Nullable<int> Tehsil_Id { get; set; }

    public Nullable<int> ConcernedSection_Id { get; set; }

    public Nullable<int> AuthorizedOfficer_Id { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> AgainstAdhoc { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public string Created_By { get; set; }

}

}
