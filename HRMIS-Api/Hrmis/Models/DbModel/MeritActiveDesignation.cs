
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
    
public partial class MeritActiveDesignation
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public MeritActiveDesignation()
    {

        this.Merits = new HashSet<Merit>();

        this.MeritsVps = new HashSet<MeritsVp>();

        this.MeritsVpDistricts = new HashSet<MeritsVpDistrict>();

    }


    public int Id { get; set; }

    public int Desg_Id { get; set; }

    public string Name { get; set; }

    public string IsActive { get; set; }

    public Nullable<System.DateTime> DateStart { get; set; }

    public Nullable<System.DateTime> DateEnd { get; set; }

    public Nullable<int> ApplicantsCount { get; set; }

    public Nullable<bool> PreferencesOnly { get; set; }

    public string OfferLetterFileName { get; set; }

    public string AcceptanceLetterFileName { get; set; }

    public Nullable<bool> IsEnable { get; set; }

    public string ExcludedDistrictCode { get; set; }

    public Nullable<bool> IsGrievanceOnly { get; set; }

    public string GrievanceLetterFileName { get; set; }

    public Nullable<bool> IsDiplomaOfferOnly { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Merit> Merits { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<MeritsVp> MeritsVps { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<MeritsVpDistrict> MeritsVpDistricts { get; set; }

}

}
