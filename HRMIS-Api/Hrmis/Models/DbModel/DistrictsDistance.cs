
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
    
public partial class DistrictsDistance
{

    public int Id { get; set; }

    public string OriginCode { get; set; }

    public Nullable<int> OriginId { get; set; }

    public string DestinationCode { get; set; }

    public Nullable<int> DestinationId { get; set; }

    public Nullable<double> Distance { get; set; }



    public virtual District District { get; set; }

    public virtual District District1 { get; set; }

}

}