
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
    
public partial class PromotionJobApplicationServiceStatement
{

    public int Id { get; set; }

    public int PromotionJobApplication_Id { get; set; }

    public string Text { get; set; }



    public virtual PromotionJobApplication PromotionJobApplication { get; set; }

}

}
