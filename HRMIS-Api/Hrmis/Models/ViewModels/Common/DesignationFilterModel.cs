using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Common
{
    public class DesignationFilterModel : Paginator
    {
            public string searchTerm { get; set; }
            public List<int?> cadres;
            public int HF_Id;
        public List<int?> designations;
    }
    public class PandSOfficerFilters : Paginator
    {
        public string Query { get; set; }
        public int? Designation_Id;
        public string User_Id;
        public int? OfficerId;
        public bool RelatedData;
        public bool add;
        public int tableType;
        public int concernedId;
        public FPPrint fprint;
        public int fpNumber;
    }
}