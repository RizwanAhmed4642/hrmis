
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
    
public partial class VPChartsCount
{

    public int Id { get; set; }

    public string HFMISCode { get; set; }

    public Nullable<int> DesignationId { get; set; }

    public Nullable<int> SanctionedInitial { get; set; }

    public Nullable<int> SanctionedPromotion { get; set; }

    public Nullable<int> AdhocInitial { get; set; }

    public Nullable<int> AdhocPromotion { get; set; }

    public Nullable<int> RegularInitial { get; set; }

    public Nullable<int> RegularPromotion { get; set; }

    public Nullable<int> ContractInitial { get; set; }

    public Nullable<int> ContractPromotion { get; set; }

    public Nullable<int> VacantInitial { get; set; }

    public Nullable<int> VacantPromotion { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string CreatedBy { get; set; }

    public string UserId { get; set; }

    public Nullable<System.DateTime> Datetime { get; set; }

}

}