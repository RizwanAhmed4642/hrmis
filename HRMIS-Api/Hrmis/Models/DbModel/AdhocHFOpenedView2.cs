
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
    
public partial class AdhocHFOpenedView2
{

    public int Id { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> HF_Id { get; set; }

    public string HFMISCode { get; set; }

    public Nullable<int> VPMaster_Id { get; set; }

    public Nullable<int> Vacant { get; set; }

    public Nullable<int> VacantInitial { get; set; }

    public Nullable<int> VacantPromotion { get; set; }

    public Nullable<int> Posted { get; set; }

    public Nullable<int> SeatsLeft { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public string HFName { get; set; }

    public string DistrictCode { get; set; }

    public string DistrictName { get; set; }

    public string HFTypeCode { get; set; }

    public string HFCategoryCode { get; set; }

    public string Name { get; set; }

    public Nullable<double> SanctionVacant { get; set; }

    public Nullable<double> SanctionPromotion { get; set; }

}

}
