using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Models
{
    public class KorisnikIgra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdKorisnik { get; set; }
        public int IdIgra { get; set; }

        public virtual List<Korisnik> Korisniks { get; set; }
        public virtual List<Igra> Igras { get; set; }
    }
}
