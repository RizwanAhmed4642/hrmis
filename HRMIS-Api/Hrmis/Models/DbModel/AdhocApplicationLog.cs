
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
    
public partial class AdhocApplicationLog
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public Nullable<int> StatusId { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<int> QualificationId { get; set; }

    public Nullable<int> ExperienceId { get; set; }

    public string Reason { get; set; }

    public Nullable<int> ReasonId { get; set; }

}

}
