
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
    
public partial class LawFile
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public LawFile()
    {

        this.LawFilesImages = new HashSet<LawFilesImage>();

        this.LawFilesPetitioners = new HashSet<LawFilesPetitioner>();

        this.LawFilesRepresentatives = new HashSet<LawFilesRepresentative>();

    }


    public int ID { get; set; }

    public string Title { get; set; }

    public string CourtTitle { get; set; }

    public string FileNumber { get; set; }

    public string CaseNumber { get; set; }

    public Nullable<int> Rack { get; set; }

    public Nullable<int> Shelf { get; set; }

    public string Brief { get; set; }

    public string StayStatus { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> isDisposed { get; set; }

    public Nullable<System.DateTime> DisposedOn { get; set; }

    public string DisposedBy { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public Nullable<bool> isDeleted { get; set; }

    public string DeletedBy { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public Nullable<System.DateTime> LastModifiedOn { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<LawFilesImage> LawFilesImages { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<LawFilesPetitioner> LawFilesPetitioners { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<LawFilesRepresentative> LawFilesRepresentatives { get; set; }

}

}