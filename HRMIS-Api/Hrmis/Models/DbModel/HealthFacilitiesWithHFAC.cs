
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
    
public partial class HealthFacilitiesWithHFAC
{

    public int Id { get; set; }

    public string HfmisCode { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public Nullable<long> Entity_Lifecycle_Id { get; set; }

    public int HFAC { get; set; }

    public string PhoneNo { get; set; }

    public string FaxNo { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string Status { get; set; }

    public Nullable<int> CoveredArea { get; set; }

    public Nullable<int> UnCoveredArea { get; set; }

    public Nullable<int> ResidentialArea { get; set; }

    public Nullable<int> NonResidentialArea { get; set; }

    public string NA { get; set; }

    public string PP { get; set; }

    public string Mauza { get; set; }

    public string UcName { get; set; }

    public string UcNo { get; set; }

}

}
