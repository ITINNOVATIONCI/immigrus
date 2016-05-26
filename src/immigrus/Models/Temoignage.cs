using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Temoignage
    {
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Ordre { get; set; }
        public string images { get; set; }
        public string Nom { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Position { get; set; }
        public bool isPublish { get; set; }
        public string Etat { get; set; }
    }
}
