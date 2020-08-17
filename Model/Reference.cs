namespace StsKlassifikation.Model
{
    public class KlasseLovligeKombinationer
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Klasse Klasse { get; set; }
    }

    public class KlasseTilfoejelse
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Klasse Klasse { get; set; }
    }

    public class KlasseSideordnet
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Klasse Klasse { get; set; }
    }

    public class KlasseRedaktoer
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Klasse Klasse { get; set; }
    }

    public class FacetRedaktoer
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Facet Facet { get; set; }
    }

    public class KlasseErstatter
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Klasse Klasse { get; set; }
    }
}
