
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
    
public partial class P_SConcernedDesignations
{

    public int Id { get; set; }

    public int Officer_Id { get; set; }

    public int Designation_Id { get; set; }



    public virtual HrDesignation HrDesignation { get; set; }

    public virtual P_SOfficers P_SOfficers { get; set; }

}

}