
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
    
public partial class LawHearing
{

    public int ID { get; set; }

    public int CaseID { get; set; }

    public System.DateTime DateOfhearing { get; set; }

    public int CourtID { get; set; }

    public int BenchID { get; set; }

    public string Description { get; set; }

    public bool isDeleted { get; set; }

    public string DeletedBy { get; set; }

    public Nullable<System.DateTime> DeletedOn { get; set; }

    public string CreatedBy { get; set; }

    public System.DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public Nullable<System.DateTime> LastModifiedOn { get; set; }

}

}
