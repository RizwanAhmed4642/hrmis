
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
    
public partial class AdhocScrutinyCommittee
{

    public int Id { get; set; }

    public string DistrictCode { get; set; }

    public string MSName { get; set; }

    public Nullable<int> MSHF_Id { get; set; }

    public string MSHFName { get; set; }

    public string MSRole { get; set; }

    public string FPName { get; set; }

    public Nullable<int> FPHF_Id { get; set; }

    public string FPHFName { get; set; }

    public string FPRole { get; set; }

    public string DHOName { get; set; }

    public Nullable<int> DHOHF_Id { get; set; }

    public string DHOHFName { get; set; }

    public string DHORole { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public string UserId { get; set; }

    public string NotificationPath { get; set; }

    public string NotificationNumber { get; set; }

    public Nullable<System.DateTime> NotificationDated { get; set; }

    public Nullable<System.DateTime> MeetingDate { get; set; }

    public string District { get; set; }

    public string NotificationCreatedBy { get; set; }

    public Nullable<System.DateTime> NotificationCreateDate { get; set; }

    public string NotificationUserId { get; set; }

}

}