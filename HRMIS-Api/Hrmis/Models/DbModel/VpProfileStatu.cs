
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
    
public partial class VpProfileStatu
{

    public int Id { get; set; }

    public Nullable<int> ProfileStatus_Id { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string User_Id { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<bool> IsActive { get; set; }



    public virtual HrProfileStatu HrProfileStatu { get; set; }

}

}
