
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
    
public partial class Cadre
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Cadre()
    {

        this.HrDesignations = new HashSet<HrDesignation>();

        this.HrProfiles = new HashSet<HrProfile>();

        this.P_SConcernedCadres = new HashSet<P_SConcernedCadres>();

        this.VPostMasts = new HashSet<VPostMast>();

    }


    public int Id { get; set; }

    public string Pkcode { get; set; }

    public string Name { get; set; }

    public Nullable<int> OrderBy { get; set; }

    public string Remarks { get; set; }

    public string Created_By { get; set; }

    public Nullable<System.DateTime> Creation_Date { get; set; }

    public Nullable<bool> IsActive { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HrDesignation> HrDesignations { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<HrProfile> HrProfiles { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<P_SConcernedCadres> P_SConcernedCadres { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<VPostMast> VPostMasts { get; set; }

}

}
