
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
    
public partial class P_SOfficers
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public P_SOfficers()
    {

        this.ApplicationFileRecositionLogs = new HashSet<ApplicationFileRecositionLog>();

        this.P_SConcernedCadres = new HashSet<P_SConcernedCadres>();

        this.P_SConcernedDesignations = new HashSet<P_SConcernedDesignations>();

        this.P_SConcernedOfficers = new HashSet<P_SConcernedOfficers>();

        this.P_SConcernedOfficers1 = new HashSet<P_SConcernedOfficers>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public string CNIC { get; set; }

    public Nullable<int> FingerPrint_Id { get; set; }

    public string Contact { get; set; }

    public string DesignationName { get; set; }

    public string Program { get; set; }

    public Nullable<int> Code { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string User_Id { get; set; }

    public Nullable<int> OrderBy { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationFileRecositionLog> ApplicationFileRecositionLogs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<P_SConcernedCadres> P_SConcernedCadres { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<P_SConcernedDesignations> P_SConcernedDesignations { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<P_SConcernedOfficers> P_SConcernedOfficers { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<P_SConcernedOfficers> P_SConcernedOfficers1 { get; set; }

}

}
