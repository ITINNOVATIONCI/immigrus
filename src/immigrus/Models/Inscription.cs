using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Inscription
    {
        public string InscriptionId { get; set; }
        public string ClientId { get; set; }
        public DateTime DateTrans { get; set; }
        public string Annee { get; set; }
        public string ConfimationNumber { get; set; }
        public string Resultat { get; set; }
        public string Etat { get; set; }
        public string Statut { get; set; }//defaut non validé
    }
}
