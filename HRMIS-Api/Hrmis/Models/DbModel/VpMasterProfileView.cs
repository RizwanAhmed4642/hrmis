
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
    
public partial class VpMasterProfileView
{

    public long Id { get; set; }

    public int TotalSanctioned { get; set; }

    public int TotalWorking { get; set; }

    public int Vacant { get; set; }

    public int TotalApprovals { get; set; }

    public Nullable<bool> Locked { get; set; }

    public int Profiles { get; set; }

    public int WorkingProfiles { get; set; }

    public string HFMISCode { get; set; }

    public int HF_Id { get; set; }

    public string DivisionCode { get; set; }

    public string DivisionName { get; set; }

    public string DistrictCode { get; set; }

    public string DistrictName { get; set; }

    public string TehsilCode { get; set; }

    public string TehsilName { get; set; }

    public string HFName { get; set; }

    public Nullable<int> HFAC { get; set; }

    public int PostType_Id { get; set; }

    public string PostTypeName { get; set; }

    public int Desg_Id { get; set; }

    public string DsgName { get; set; }

    public Nullable<int> BPSWorking { get; set; }

    public Nullable<int> Cadre_Id { get; set; }

    public string CadreName { get; set; }

    public Nullable<int> BPS { get; set; }

    public Nullable<int> BPS2 { get; set; }

    public Nullable<int> HFTypeId { get; set; }

    public string HFTypeCode { get; set; }

    public string HFTypeName { get; set; }

    public Nullable<long> EntityLifeCycle_Id { get; set; }

    public string Created_By { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public string Last_Modified_By { get; set; }

    public string Users_Id { get; set; }

}

}
