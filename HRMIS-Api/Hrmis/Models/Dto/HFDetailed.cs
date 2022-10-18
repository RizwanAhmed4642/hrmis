using System.Collections.Generic;
using Hrmis.Models.DbModel;

namespace Hrmis.Models.Dto
{
    public class HFDetailed
    {
        List<HFBlock> hfBlocks = new List<HFBlock>(); 
        List<HFWard> hFWards = new List<HFWard>(); 
        List<HFService> hFServices = new List<HFService>(); 
        HealthFacility healthFacility = new HealthFacility(); 
    }
}