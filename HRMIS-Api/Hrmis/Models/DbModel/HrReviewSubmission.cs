
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
    
public partial class HrReviewSubmission
{

    public int Id { get; set; }

    public Nullable<int> ProfileId { get; set; }

    public string SubmitBy { get; set; }

    public string Remarks { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public string UserId { get; set; }

    public string Username { get; set; }

    public Nullable<bool> IsActive { get; set; }

}

}
