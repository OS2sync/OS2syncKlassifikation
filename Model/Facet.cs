using FacetWebService;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StsKlassifikation.Model
{
    public class Facet
    {
        [Key]
        public string UUID { get; set; }
        public string BrugervendtNoegle { get; set; }
        public string Titel { get; set; }
        public string Beskrivelse { get; set; }
        public string Opbygning { get; set; }
        public string Ophavsret { get; set; }
        public string PlanIdentifikator { get; set; }
        public string SupplementTekst { get; set; }
        public string RetskildeTekst { get; set; }
        public LivscyklusKodeType? Livscykluskode { get; set; }
        public bool? Publiceret { get; set; }
        public string Ansvarlig { get; set; }
        public string Ejer { get; set; }
        public string FacetTilhoerer { get; set; }
        public List<FacetRedaktoer> Redaktoerer { get; set; }
        public Klassifikation Klassifikation { get; set; }
        public List<Klasse> Klasser { get; set; }
    }
}
