using KlasseWebService;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StsKlassifikation.Model
{
    public class Klasse
    {
        [Key]
        public string UUID { get; set; }
        public string BrugervendtNoegle { get; set; }
        public string Titel { get; set; }
        public string Beskrivelse { get; set; }
        public string Omfang { get; set; }
        public string Retskilde { get; set; }
        public string Aendringsnotat { get; set; }
        public LivscyklusKodeType? Livscykluskode { get; set; }
        public bool? Tilstand { get; set; }
        public string Ansvarlig { get; set; }
        public string Ejer { get; set; }
        public string Soegeord { get; set; }
        public string KlasseTilhoerer { get; set; }
        public Facet Facet { get; set; }
        public List<KlasseRedaktoer> Redaktoerer { get; set; }
        public List<KlasseErstatter> Erstatter { get; set; }
        public List<KlasseSideordnet> Sideordnet { get; set; }
        public List<KlasseTilfoejelse> Tilfoejelser { get; set; }
        public List<KlasseLovligeKombinationer> LovligeKombinationer { get; set; }
        public string OverordnetKlasse { get; set; }
    }
}