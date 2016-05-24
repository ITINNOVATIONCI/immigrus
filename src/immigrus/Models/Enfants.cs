using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Enfants
    {
        public string EnfantsId { get; set; }

        public string Nom { get; set; }
        public string Prenoms { get; set; }
        public DateTime DateNais { get; set; }
        public string sexe { get; set; }

        //Foreign key for Clients
        public string ClientsId { get; set; }

        public Clients clients { get; set; }
    }
}
