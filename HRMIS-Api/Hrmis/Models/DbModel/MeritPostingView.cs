
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
    
public partial class MeritPostingView
{

    public int Id { get; set; }

    public Nullable<int> Merit_Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> MeritsActiveDesignationId { get; set; }

    public Nullable<int> HFOpened_Id { get; set; }

    public Nullable<int> Preference_Id { get; set; }

    public Nullable<int> PreferencesNumber { get; set; }

    public Nullable<int> PostingHF_Id { get; set; }

    public Nullable<bool> IsOnAdhocee { get; set; }

    public Nullable<bool> IsPG { get; set; }

    public string DistrictCode { get; set; }

    public string DistrictName { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

    public Nullable<System.DateTime> DOB { get; set; }

    public Nullable<int> MeritNumber { get; set; }

    public string PostingHFMISCode { get; set; }

    public Nullable<int> PostingHFSeatNumber { get; set; }

    public Nullable<int> PostingHFTotalSeats { get; set; }

    public Nullable<int> PostingHFAdhocSeats { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public Nullable<int> ESR_Id { get; set; }

    public Nullable<int> ActualPostingHF_Id { get; set; }

    public string ActualPostingHFMISCode { get; set; }

    public Nullable<int> GrievancePostingHF_Id { get; set; }

    public string GrievancePostingHFMISCode { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string Remarks { get; set; }

    public string DesignationName { get; set; }

    public string PostingHFName { get; set; }

    public Nullable<int> PostingHFAC { get; set; }

    public string ActualPostingHFName { get; set; }

    public string GrievancePostingHFName { get; set; }

}

}
