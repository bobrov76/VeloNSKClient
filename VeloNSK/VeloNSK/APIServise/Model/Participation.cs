using System;
using System.Collections.Generic;
using System.Text;

namespace VeloNSK.APIServise.Model
{
    internal class Participation
    {
        public int IdParticipation { get; set; }
        public int IdUser { get; set; }
        public int IdCompetentions { get; set; }
        public bool IdStatusVerification { get; set; }
    }
}