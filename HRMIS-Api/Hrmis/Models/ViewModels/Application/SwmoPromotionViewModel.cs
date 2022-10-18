using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Application
{
    public class SwmoPromotionViewModel
    {
        public SwmoPromotion swmoPromotion { get; set; } = new SwmoPromotion();
        public List<SwmoPromotionDetail> swmoPromotionDetail { get; set; } = new List<SwmoPromotionDetail>();
    }
}