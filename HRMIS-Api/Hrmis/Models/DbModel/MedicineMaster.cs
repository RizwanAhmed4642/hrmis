
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
    
public partial class MedicineMaster
{

    public int Id { get; set; }

    public string MediName { get; set; }

    public string Potency { get; set; }

    public string MediType { get; set; }

    public Nullable<System.DateTime> TimeDate { get; set; }

    public string Description { get; set; }

    public string Qty_In { get; set; }

    public string Qty_Out { get; set; }

    public string ResponsibleUser { get; set; }

    public Nullable<System.DateTime> DayAndTime { get; set; }

    public string Entered_By { get; set; }

    public string Enable_Flag { get; set; }

    public string Manufacturer { get; set; }

    public string Vendor { get; set; }

    public Nullable<double> Rate { get; set; }

    public string UpdatedBy { get; set; }

    public Nullable<System.DateTime> UpdatedAt { get; set; }

}

}
