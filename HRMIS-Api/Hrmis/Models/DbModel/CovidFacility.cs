
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
    
public partial class CovidFacility
{

    public int Id { get; set; }

    public string Name { get; set; }

    public Nullable<int> TypeId { get; set; }

    public Nullable<int> HF_Id { get; set; }

    public string DivisionCode { get; set; }

    public string DistrictCode { get; set; }

    public string TehsilCode { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string CreatedBy { get; set; }

    public string UserId { get; set; }

}

}