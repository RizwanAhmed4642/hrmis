
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
    
public partial class VpDProfileView
{

    public long Id { get; set; }

    public long Master_Id { get; set; }

    public int TotalWorking { get; set; }

    public int TotalApprovals { get; set; }

    public Nullable<bool> Locked { get; set; }

    public int Profiles { get; set; }

    public int WorkingProfiles { get; set; }

    public int EmpMode_Id { get; set; }

    public string EmpModeName { get; set; }

    public Nullable<long> EntityLifeCycle_Id { get; set; }

    public string Created_By { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public Nullable<int> Modified_By { get; set; }

    public Nullable<int> Modified_Date { get; set; }

    public string Users_Id { get; set; }

}

}
