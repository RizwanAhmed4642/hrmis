
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
    
public partial class ESR
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ESR()
    {

        this.EsrForwardToOfficers = new HashSet<EsrForwardToOfficer>();

    }


    public long Id { get; set; }

    public Nullable<System.DateTime> VDT { get; set; }

    public Nullable<int> Profile_Id { get; set; }

    public string CNIC { get; set; }

    public Nullable<long> BPSFrom { get; set; }

    public Nullable<long> BPSTo { get; set; }

    public Nullable<System.DateTime> CurrentDoJ { get; set; }

    public Nullable<System.DateTime> CurrentDoT { get; set; }

    public Nullable<int> DesignationFrom { get; set; }

    public Nullable<int> DesignationTo { get; set; }

    public Nullable<int> DepartmentFrom { get; set; }

    public Nullable<int> DepartmentTo { get; set; }

    public Nullable<int> HF_Id_From { get; set; }

    public Nullable<int> HF_Id_To { get; set; }

    public string HfmisCodeFrom { get; set; }

    public string HfmisCodeTo { get; set; }

    public string PostingOrderNo { get; set; }

    public Nullable<System.DateTime> PostingOrderDate { get; set; }

    public string LengthOfService { get; set; }

    public string JoiningStatus { get; set; }

    public string EmployeeFileNO { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<long> EntityLifecycle_Id { get; set; }

    public string Remarks { get; set; }

    public string COMMENTS { get; set; }

    public byte[] PostingOrderPhoto { get; set; }

    public string TargetUser { get; set; }

    public string VPot { get; set; }

    public string CurrentUser { get; set; }

    public string WDesigFrom { get; set; }

    public string OrderDetail { get; set; }

    public string OrderNumer { get; set; }

    public string TransferOrderType { get; set; }

    public string TranferOrderStatus { get; set; }

    public string VerbalOrderByDesignation { get; set; }

    public string VerbalOrderByName { get; set; }

    public string NotingFile { get; set; }

    public string EmbossedFile { get; set; }

    public string SectionOfficer { get; set; }

    public string ResponsibleUser { get; set; }

    public Nullable<int> TransferTypeID { get; set; }

    public Nullable<int> DisposalofID { get; set; }

    public string Disposalof { get; set; }

    public Nullable<byte> EsrSectionOfficerID { get; set; }

    public Nullable<int> MutualESR_Id { get; set; }

    public string OrderHTML { get; set; }

    public string ModuleSource { get; set; }

    public string AppointmentEffect { get; set; }

    public Nullable<System.DateTime> AppointmentDate { get; set; }



    public virtual EsrSectionOfficer EsrSectionOfficer { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<EsrForwardToOfficer> EsrForwardToOfficers { get; set; }

}

}