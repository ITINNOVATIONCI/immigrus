using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace immigrus.Models
{
    public class Pays
    {
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Code { get; set; }
        public string Descriptions { get; set; }
        public string Nationalite { get; set; }
        
        
    }
}
