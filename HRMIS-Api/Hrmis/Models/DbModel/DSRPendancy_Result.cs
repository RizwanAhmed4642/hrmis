
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
    
public partial class DSRPendancy_Result
{

    public int OrderBy { get; set; }

    public int Code { get; set; }

    public string OfficerDesignation { get; set; }

    public string Program { get; set; }

    public Nullable<int> TodayUnderProcess { get; set; }

    public Nullable<int> TodayDispose { get; set; }

    public Nullable<int> UnderProcessGT7Days { get; set; }

    public Nullable<int> UnderProcessGT15Days { get; set; }

    public Nullable<int> UnderProcessGT30Days { get; set; }

    public Nullable<int> UnderProcessUntilToday { get; set; }

}

}
