using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Clients
    {
        public string ClientsId { get; set; }

        public string Nom { get; set; }
        public string Prenoms { get; set; }
        public DateTime DateNais { get; set; }
        public string PaysNais { get; set; }
        public string PaysEl { get; set; }
        public string LieuNais { get; set; }
        public string Sexe { get; set; }
        public string PaysRes { get; set; }
        public string ZipCode { get; set; }
        public string AdrPos { get; set; }
        public string StatutMarital { get; set; }
        public string NbEnfts { get; set; }
        public string Diplome { get; set; }
        public string AutresDip { get; set; }
        public string ParainIdf { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Photo { get; set; }
        public string Etat { get; set; }
        public string Statut { get; set; }//defaut non validé
        public DateTime DateCreation { get; set; }
        public string ConfimationNumber { get; set; }
        public string Resultat { get; set; }




        // Navigation property 
        public virtual ICollection<Enfants> enfants { get; set; }



    }
}
