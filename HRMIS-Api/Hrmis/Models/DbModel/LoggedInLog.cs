
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
    
public partial class LoggedInLog
{

    public int Id { get; set; }

    public string Browser { get; set; }

    public string IPAddress { get; set; }

    public Nullable<System.DateTime> LoggedInDate { get; set; }

    public Nullable<System.DateTime> LoggedOffDate { get; set; }

    public string ForwardedIPAddress { get; set; }

    public string UserId { get; set; }

    public string Remarks { get; set; }

    public string RemoteAddress { get; set; }

    public string HttpXForwardedFor { get; set; }

    public string RemoteIP { get; set; }

    public string RemoteDetailJSON { get; set; }

    public string ActionName { get; set; }

    public string SearchingValue { get; set; }

}

}
