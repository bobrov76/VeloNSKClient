using System;
using System.Collections.Generic;
using System.Text;

namespace VeloNSK.View.Admin.Participations
{
    internal class ModelPartisipant
    {
        public DateTime Date { get; set; }
        public bool IdStatusVerification { get; set; }
        public string NameDistantion { get; set; }
        public short Ot { get; set; }
        public short Do { get; set; }
        public string Name { get; set; }
        public string Patronimic { get; set; }
        public string Login { get; set; }
    }
}