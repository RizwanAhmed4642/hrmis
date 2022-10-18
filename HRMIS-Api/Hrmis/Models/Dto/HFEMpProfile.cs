using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class HFEMpProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public bool Show { get; set; }
        public List<ProfileDetailsView> Profiles { get; set; }
    }
}