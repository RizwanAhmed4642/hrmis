
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
    
public partial class ESR_FINAL
{

    public string cnic { get; set; }

    public int Id { get; set; }

    public Nullable<int> joininggrade { get; set; }

    public Nullable<int> currentgrade { get; set; }

    public Nullable<System.DateTime> currentdoj { get; set; }

    public Nullable<System.DateTime> currentdot { get; set; }

    public int DSG_FROM { get; set; }

    public Nullable<int> DSG_TO { get; set; }

    public string postingorderno { get; set; }

    public Nullable<System.DateTime> postingorderdate { get; set; }

    public string hfmiscodefrom { get; set; }

    public string hfmiscodeto { get; set; }

    public string lengthofservice { get; set; }

    public long srno { get; set; }

    public string responsibleuser { get; set; }

    public Nullable<System.DateTime> dayandtime { get; set; }

    public string joiningstatus { get; set; }

    public string targetuser { get; set; }

    public string vpot { get; set; }

    public string currentuser { get; set; }

    public string wdesigfrom { get; set; }

    public string Order_detail { get; set; }

    public string OrderNumer { get; set; }

    public string TransferOrderType { get; set; }

    public string TranferOrderStatus { get; set; }

    public string VerbalOrderByDesignation { get; set; }

    public string VerbalOrderByName { get; set; }

    public string NotingFile { get; set; }

    public string EmbossedFile { get; set; }

    public string SectionOfficer { get; set; }

    public string EmployeeFileNO { get; set; }

}

}