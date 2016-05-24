using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Actualite
    {
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Ordre { get; set; }
        public string images { get; set; }
        public string Libelle { get; set; }
        public string Etat { get; set; }
    }
}
