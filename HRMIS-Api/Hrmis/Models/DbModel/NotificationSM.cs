
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
    
public partial class NotificationSM
{

    public int Id { get; set; }

    public string MessageId { get; set; }

    public string Number { get; set; }

    public string Message { get; set; }

    public Nullable<int> FKId { get; set; }

    public Nullable<bool> Status { get; set; }

    public string Mask { get; set; }

    public string ProjectId { get; set; }

    public Nullable<System.DateTime> DateTime { get; set; }

    public Nullable<bool> Sent { get; set; }

    public Nullable<System.DateTime> SentDatetime { get; set; }

    public string SentResponse { get; set; }

}

}