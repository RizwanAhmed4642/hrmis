
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
    
public partial class HisduEmpMonthelyLeaveSummery
{

    public Nullable<long> Id { get; set; }

    public Nullable<int> ProfileID { get; set; }

    public Nullable<int> Yrar { get; set; }

    public Nullable<int> Month { get; set; }

    public decimal TotalLeave { get; set; }

    public Nullable<decimal> TotalSL { get; set; }

    public Nullable<decimal> TotalSick { get; set; }

    public Nullable<decimal> TotalCasual { get; set; }

}

}