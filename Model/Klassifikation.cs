using KlassifikationSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StsKlassifikation.Model
{
    public class Klassifikation
    {
        [Key]
        public string UUID { get; set; }
        public DateTime? Timestamp { get; set; }
        public LivscyklusKodeType? Livscykluskode { get; set; }
        public string BrugervendtNoegle { get; set; }
        public string Titel { get; set; }
        public string Beskrivelse { get; set; }
        public bool Synkroniser { get; set; }
        public string Ansvarlig { get; set; }
        public string Ejer { get; set; }
        public bool? Publiceret { get; set; }
        public List<Facet> Facetter { get; set; }
    }
}
