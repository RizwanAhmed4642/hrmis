
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
    
public partial class MoPrefferedHealthFacilityView
{

    public int Id { get; set; }

    public Nullable<int> PreferenceHfId { get; set; }

    public Nullable<int> Swmo_Promo_Id { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string DistrictCode { get; set; }

    public Nullable<System.DateTime> PermanentSince { get; set; }

    public string FullName { get; set; }

}

}