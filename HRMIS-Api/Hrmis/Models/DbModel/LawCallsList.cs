
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
    
public partial class LawCallsList
{

    public int Id { get; set; }

    public string CaseNumber { get; set; }

    public string CaseTitle { get; set; }

    public string CourtName { get; set; }

    public Nullable<System.DateTime> LastDate { get; set; }

    public Nullable<System.DateTime> NextDate { get; set; }

    public Nullable<int> Section_Id { get; set; }

    public string DealingOfficer { get; set; }

    public string DealingOfficer_Id { get; set; }

    public string Proceedings { get; set; }

    public string Remarks { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public string Created_By { get; set; }

    public string Users_Id { get; set; }

    public Nullable<bool> IsActive { get; set; }

}

}
