
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
    
public partial class UcData
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public UcData()
    {

        this.Coordinates = new HashSet<Coordinate>();

    }


    public int Id { get; set; }

    public string HfId { get; set; }

    public string DivisionCode { get; set; }

    public string DivisionName { get; set; }

    public string DistrictCode { get; set; }

    public string DistrictName { get; set; }

    public string TehsilCode { get; set; }

    public string TehsilName { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Coordinate> Coordinates { get; set; }

}

}