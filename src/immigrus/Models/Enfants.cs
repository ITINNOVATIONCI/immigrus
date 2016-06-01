using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Enfants
    {
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Nom { get; set; }
        public string Prenoms { get; set; }
        public DateTime DateNais { get; set; }
        public string Sexe { get; set; }
        public string Photo { get; set; }
        public string LieuNais { get; set; }
        public string Type { get; set; }
        


        //Foreign key for Clients
        public string ClientsId { get; set; }
        public string idPays { get; set; }
    }
}
