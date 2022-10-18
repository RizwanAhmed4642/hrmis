using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Vacancy
{
    public class VacancyReportDetail
    {
        public string column { get; set; }
        public string type { get; set; }
        public string geoLevelName { get; set; }
        public string clickType { get; set; }
        public List<int> designationIds { get; set; }
        public List<string> hfmisCodes { get; set; }
    }
    public class VacancyMasDtlViewModel
    {
        public VpMProfileView vpMaster { get; set; }
        public Entity_Lifecycle elc { get; set; }
        public List<VpDProfileView> vpDetails { get; set; }
        public List<VPProfileView> vpProfiles { get; set; }
        public List<VpMasterLog> vpMasterLogs { get; set; }
        public List<VpDetailLog> vpDetailLogs { get; set; }
        public List<Entity_Modified_Log_View> emls { get; set; }
    }
    public class VacancyViewModel
    {
        public Nullable<Int32> DesignationID { get; set; }
        public string DesignationName { get; set; }
        public Nullable<Int32> TotalSanctioned { get; set; }
        public Nullable<Int32> TotalWorking { get; set; }
        public string CadreName { get; set; }
        public Nullable<Int32> BPS { get; set; }
        public string HFMISCode { get; set; }
        public string HfFullName { get; set; }
        public string HfTypeName { get; set; }
        public Nullable<Int32>  Adhoc { get; set; }
        public Nullable<Int32>  Contract { get; set; }
        public Nullable<Int32>  DailyWages { get; set; }
        public Nullable<Int32>  Regular { get; set; }
        public Nullable<Int32> PHFMC { get; set; }
        public Nullable<Int32> TotalVacant { get; set; }
        public Nullable<Int32> TotalProfile { get; set; }

    }
    public class VacancyReportModel
    {
        public List<VacancyViewModel> reportModel { get; set; }
        public int GrandTotalSanctioned { get; set; }
        public int GrandTotalWorking { get; set; }
        public int GrandTotalProfiles { get; set; }
        public int GrandTotalVacant { get; set; }
    }
}
