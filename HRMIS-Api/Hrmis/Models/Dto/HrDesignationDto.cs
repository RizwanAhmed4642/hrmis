using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class HrDesignationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cadre_Id { get; set; }
        public int HrScale_Id { get; set; }
    }
}