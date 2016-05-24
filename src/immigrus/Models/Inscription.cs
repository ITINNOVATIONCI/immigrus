using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Inscription
    {
        public string ClientsId { get; set; }
        public DateTime DateTrans { get; set; }
        public string ConfimationNumber { get; set; }
        public string Resultat { get; set; }
        public string Statut { get; set; }//defaut non validé
    }
}
