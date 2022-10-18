
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
    
public partial class VPHolder
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public VPHolder()
    {

        this.ApplicationMasters = new HashSet<ApplicationMaster>();

        this.ApplicationMasters1 = new HashSet<ApplicationMaster>();

    }


    public int Id { get; set; }

    public Nullable<int> TotalSeats { get; set; }

    public Nullable<int> TotalSeatsVacant { get; set; }

    public Nullable<int> TotalSeatsHold { get; set; }

    public Nullable<long> VpMaster_Id { get; set; }

    public Nullable<long> VpDetail_Id { get; set; }

    public string FileNumber { get; set; }

    public Nullable<int> TrackingNumber { get; set; }

    public string EmployeeName { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public string ESR { get; set; }

    public string ELR { get; set; }

    public string OrderNumber { get; set; }

    public Nullable<int> OrderAttachment_Id { get; set; }

    public Nullable<long> Elc_Id { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationMaster> ApplicationMasters { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationMaster> ApplicationMasters1 { get; set; }

    public virtual Entity_Lifecycle Entity_Lifecycle { get; set; }

    public virtual VPDetail VPDetail { get; set; }

    public virtual VPMaster VPMaster { get; set; }

}

}
