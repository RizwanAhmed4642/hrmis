
using CsvHelper.Configuration;

namespace Hrmis.Models.Dto
{


    public class HealthFacilityDto
    {
        public string SrNo { get; set; }

        public string Division { get; set; }

        public string District { get; set; }

        public string Tehsil { get; set; }

        public string FacilityName { get; set; }

        public string BlockName { get; set; }

        public string WardName { get; set; }

        public int NoOfBeds { get; set; }

        public int NoOfGeneralBeds { get; set; }

        public int NoOfSpecialBeds { get; set; }
    }
    public sealed class HealthFacilityMap : ClassMap<HealthFacilityDto>
    {
        public HealthFacilityMap()
        {
            Map(m => m.SrNo).Name("Sr. No");
            Map(m => m.Division).Name("Division");
            Map(m => m.District).Name("Districts");
            Map(m => m.Tehsil).Name("Tehsils");
            Map(m => m.FacilityName).Name("Facility Name");
            Map(m => m.BlockName).Name("Block Name");
            Map(m => m.WardName).Name("Ward Name");
            Map(m => m.NoOfBeds).Name("No. of Beds").Default(0);
            Map(m => m.NoOfGeneralBeds).Name("No. of General Beds").Default(-1);
            Map(m => m.NoOfSpecialBeds).Name("No. of Special Beds").Default(-1);
        }
    }


    public class CDRDto
    {
        public string SrNo { get; set; }

        public string CallType { get; set; }

        public string A_Party { get; set; }

        public string B_Party { get; set; }

        public string DateTime { get; set; }

        public string Duration { get; set; }

        public string CellID { get; set; }

        public string IMSI { get; set; }

        public string Site { get; set; }

    }
    public sealed class CDRMap : ClassMap<CDRDto>
    {
        public CDRMap()
        {
            Map(m => m.SrNo).Name("Sr #");
            Map(m => m.CallType).Name("Call Type");
            Map(m => m.A_Party).Name("A-Party");
            Map(m => m.B_Party).Name("B-Party");
            Map(m => m.DateTime).Name("Date & Time");
            Map(m => m.Duration).Name("Duration");
            Map(m => m.CellID).Name("Cell ID");
            Map(m => m.IMSI).Name("IMSI");
            Map(m => m.Site).Name("Site");
        }
    }


    public class VaccineDTO
    {
        public string MobileNo { get; set; }
    }
    public sealed class VaccineMap : ClassMap<VaccineDTO>
    {
        public VaccineMap()
        {
            Map(m => m.MobileNo).Name("Mobile Number");
        }
    }

}