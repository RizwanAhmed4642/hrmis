
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
    
public partial class PerGrading
{

    public int Per_Id { get; set; }

    public int Gradings_Id { get; set; }

    public int GradingsVals_Id { get; set; }



    public virtual Grading Grading { get; set; }

    public virtual GradingVal GradingVal { get; set; }

    public virtual PER PER { get; set; }

}

}