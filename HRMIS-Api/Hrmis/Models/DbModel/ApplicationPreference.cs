
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
    
public partial class ApplicationPreference
{

    public int Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public Nullable<int> HealthFacility_Id { get; set; }

    public Nullable<int> PreferenceOrder { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string CreatedBy { get; set; }

    public string User_Id { get; set; }

}

}
