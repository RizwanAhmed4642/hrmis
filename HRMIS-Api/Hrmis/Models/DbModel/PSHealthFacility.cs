
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
    
public partial class PSHealthFacility
{

    public int Id { get; set; }

    public int Project_Id { get; set; }

    public int HealthFacility_Id { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedBy_UserId { get; set; }

    public Nullable<System.DateTime> CreatedOn { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedBy_UserId { get; set; }

    public Nullable<System.DateTime> UpdatedOn { get; set; }



    public virtual HealthFacility HealthFacility { get; set; }

    public virtual PSHealthProject PSHealthProject { get; set; }

}

}