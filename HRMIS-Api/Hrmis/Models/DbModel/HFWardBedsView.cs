
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
    
public partial class HFWardBedsView
{

    public int Id { get; set; }

    public Nullable<int> HF_Id { get; set; }

    public Nullable<int> Ward_Id { get; set; }

    public Nullable<int> TotalGB { get; set; }

    public Nullable<int> TotalSB { get; set; }

    public Nullable<int> TotalSanctioned { get; set; }

    public Nullable<int> TotalDonated { get; set; }

    public Nullable<long> EntityLifecycle_Id { get; set; }

    public string Name { get; set; }

    public int Total { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public string Created_By { get; set; }

    public string Last_Modified_By { get; set; }

    public string Users_Id { get; set; }

}

}
