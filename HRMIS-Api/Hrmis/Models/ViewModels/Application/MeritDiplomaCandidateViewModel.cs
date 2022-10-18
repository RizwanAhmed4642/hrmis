using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Application
{
    public class MeritDiplomaCandidateViewModel
    {
        public MeritDiplomaCandidate meritCandidate { get; set; } = new MeritDiplomaCandidate();
        public List<MeritDiplomaCandidateDetail> meritCandidateDetail { get; set; } = new List<MeritDiplomaCandidateDetail>();
    }
}