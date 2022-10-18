
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
    
public partial class ApplicationDocument
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ApplicationDocument()
    {

        this.ApplicationAttachments = new HashSet<ApplicationAttachment>();

        this.AppTypeDocs = new HashSet<AppTypeDoc>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public Nullable<bool> IsRequired { get; set; }

    public Nullable<bool> Original { get; set; }

    public Nullable<bool> Scanned { get; set; }

    public Nullable<bool> AttachementAllow { get; set; }

    public Nullable<bool> Signed { get; set; }

    public Nullable<int> OrderBy { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string User_Id { get; set; }

    public string CreatedBy { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ApplicationAttachment> ApplicationAttachments { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<AppTypeDoc> AppTypeDocs { get; set; }

}

}
