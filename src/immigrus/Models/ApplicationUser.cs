using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace immigrus.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
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
        public string Password { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Photo { get; set; }
        public string Etat { get; set; }
        public string Statut { get; set; }//defaut non validé
        public DateTime DateCreation { get; set; }


    }
}
