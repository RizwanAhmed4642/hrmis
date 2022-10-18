using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class MeritPgDistrictViewModel
    {
        public int Id { get; set; }
        public int? Merit_Id { get; set; }
        public int? MeritPG_Id { get; set; }
        public int? Districts_Id { get; set; }
        public string DistrictCode { get; set; }
        public DistrictViewModel District { get; set; }
    }

    public class DistrictViewModel {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}