
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
    
public partial class ApplicationPersonAppeared
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ApplicationPersonAppeared()
    {

        this.ApplicationMasters = new HashSet<ApplicationMaster>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public string CNIC { get; set; }

    public string Mobile { get; set; }

    public string DistrictCode { get; set; }

    public string Reference { get; set; }

    public string Relation { get; set; }

    public string Constituency { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationMaster> ApplicationMasters { get; set; }

}

}
