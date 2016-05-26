using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.ViewModels.Account
{
    public class ApplicationUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Compare("Email", ErrorMessage = "L'Email et la confirmation de l'Email ne correspondent pas.")]
        public string ConfirmEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }


        
        public string ClientsId { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenoms { get; set; }

        [Required]
        //[DataType(DataType.Date)]
        public string DateNais { get; set; }

        [Required]
        public string PaysNais { get; set; }

        [Required]
        public string PaysEl { get; set; }

        [Required]
        public string LieuNais { get; set; }

        [Required]
        public string Sexe { get; set; }

        [Required]
        public string PaysRes { get; set; }

        
        public string ZipCode { get; set; }

        public string AdrPos { get; set; }

        [Required]
        public string StatutMarital { get; set; }

        [Required]
        public string NbEnfts { get; set; }

       
        public string Diplome { get; set; }
        public string AutresDip { get; set; }
        public string ParainIdf { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 8)]   
        public string Tel1 { get; set; }

        public string Tel2 { get; set; }
        public string Photo { get; set; }
        public string Etat { get; set; }
        public string Statut { get; set; }//defaut non validé
        public DateTime DateCreation { get; set; }





        

    }
}
