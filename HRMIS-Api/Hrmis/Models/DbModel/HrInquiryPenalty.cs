
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
    
public partial class HrInquiryPenalty
{

    public int Id { get; set; }

    public Nullable<int> EmpInquiry_Id { get; set; }

    public string PenaltyType { get; set; }

    public Nullable<int> PenaltyType_Id { get; set; }

    public Nullable<System.DateTime> DurationFrom { get; set; }

    public Nullable<System.DateTime> DurationTo { get; set; }

    public Nullable<int> Scale { get; set; }

    public Nullable<int> Amount { get; set; }

    public Nullable<int> AmountSource_Id { get; set; }

    public string OtherAmountSource { get; set; }

    public string Remarks { get; set; }

    public Nullable<int> NumberOfYears { get; set; }



    public virtual HrInquiry HrInquiry { get; set; }

}

}