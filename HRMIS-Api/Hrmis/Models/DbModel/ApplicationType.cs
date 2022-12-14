
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
    
public partial class ApplicationType
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ApplicationType()
    {

        this.ApplicationMasters = new HashSet<ApplicationMaster>();

        this.AppTypeDocs = new HashSet<AppTypeDoc>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public Nullable<int> OrderBy { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<bool> FacilitationPortal { get; set; }

    public Nullable<bool> RNIBranchPortal { get; set; }

    public Nullable<bool> IsPendency { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationMaster> ApplicationMasters { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AppTypeDoc> AppTypeDocs { get; set; }

}

}
