
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
    
public partial class AdhocGreivanceUploadView
{

    public int Id { get; set; }

    public Nullable<int> Applicant_Id { get; set; }

    public Nullable<int> DocId { get; set; }

    public string DocName { get; set; }

    public Nullable<bool> IsQualification { get; set; }

    public Nullable<int> QualificationId { get; set; }

    public bool IsActive { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public string UploadPath { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string CNIC { get; set; }

}

}
