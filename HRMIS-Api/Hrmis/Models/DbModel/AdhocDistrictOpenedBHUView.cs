
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
    
public partial class AdhocDistrictOpenedBHUView
{

    public int Id { get; set; }

    public string DistrictCode { get; set; }

    public string DistrictName { get; set; }

    public Nullable<int> BHUForMO { get; set; }

    public Nullable<int> BHUForWMO { get; set; }

    public Nullable<int> BHUForMOLeft { get; set; }

    public Nullable<int> BHUForWMOLeft { get; set; }

    public Nullable<int> Phase { get; set; }

    public Nullable<int> BatchNo { get; set; }

}

}
