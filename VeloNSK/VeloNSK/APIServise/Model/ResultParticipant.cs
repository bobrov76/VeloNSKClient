using System;
using System.Collections.Generic;
using System.Text;

namespace VeloNSK.APIServise.Model
{
    public class ResultParticipant
    {
        public int IdResultParticipation { get; set; }
        public int IdParticipation { get; set; }
        public DateTime ResultTime { get; set; }
        public int Mesto { get; set; }
    }
}