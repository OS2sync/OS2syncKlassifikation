using KlasseWebService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StsKlassifikation.Service
{
    public class KlasseService
    {
        private readonly ClassificationContext classificationContext;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public KlasseService(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
            logger = sp.GetService<ILogger<ClassificationService>>();
            configuration = sp.GetService<IConfiguration>();
        }

        public List<Klasse> GetAllKlasse(string facetUUID)
        {
            int maxResults = 100;
            KlassePortTypeClient port = new KlassePortTypeClient(configuration["KlasseService:serviceUrl"], configuration["certPath"], configuration["certPassword"]);

            SoegRequestType request = new SoegRequestType();
            request.AuthorityContext = new AuthorityContextType();
            request.AuthorityContext.MunicipalityCVR = configuration["cvr"];

            request.SoegInput = new SoegInputType1();
            request.SoegInput.FoersteResultatReference = "0";
            request.SoegInput.MaksimalAntalKvantitet = maxResults + "";
            request.SoegInput.RelationListe = new RelationListeType();
            request.SoegInput.AttributListe = new AttributListeType();
            request.SoegInput.TilstandListe = new TilstandListeType();
            request.SoegInput.RelationListe.Facet = new FacetRelationType();
            request.SoegInput.RelationListe.Facet.ReferenceID = new UnikIdType();
            request.SoegInput.RelationListe.Facet.ReferenceID.Item = facetUUID;
            request.SoegInput.RelationListe.Facet.ReferenceID.ItemElementName = ItemChoiceType.UUIDIdentifikator;

            RequestHeaderType requestHeaderType = new RequestHeaderType();
            requestHeaderType.TransactionUUID = "";

            var response = port.soegAsync(null, request).Result;

            List<Klasse> klasse = new List<Klasse>();

            if ("20".Equals(response.SoegResponse.SoegOutput.StandardRetur.StatusKode))
            {
                if (response.SoegResponse.SoegOutput.IdListe.Length >= maxResults)
                {
                    klasse.AddRange(ReadKlasseFromResponse(response, facetUUID));

                    while ("20".Equals(response.SoegResponse.SoegOutput.StandardRetur.StatusKode) || response.SoegResponse.SoegOutput.IdListe.Length > 0)
                    {
                        request.SoegInput.FoersteResultatReference = (int.Parse(request.SoegInput.FoersteResultatReference) + response.SoegResponse.SoegOutput.IdListe.Length) + "";
                        response = port.soegAsync(null, request).Result;

                        klasse.AddRange(ReadKlasseFromResponse(response, facetUUID));
                    }
                }
                else
                {
                    klasse.AddRange(ReadKlasseFromResponse(response, facetUUID));
                }
            }
            else
            {
                logger.LogWarning(response.SoegResponse.SoegOutput.StandardRetur.FejlbeskedTekst);
            }

            return klasse;
        }

        private List<Klasse> ReadKlasseFromResponse(soegResponse response, string facetUUID)
        {
            KlassePortTypeClient port = new KlassePortTypeClient(configuration["KlasseService:serviceUrl"], configuration["certPath"], configuration["certPassword"]);

            ListRequestType request = new ListRequestType();
            request.AuthorityContext = new AuthorityContextType();
            request.AuthorityContext.MunicipalityCVR = configuration["cvr"];

            request.ListInput = new ListInputType();
            request.ListInput.UUIDIdentifikator = response.SoegResponse.SoegOutput.IdListe;

            var listResponse = port.listAsync(null, request).Result;

            List<Klasse> klasse = new List<Klasse>();
            var facetDTOs = listResponse.ListResponse.ListOutput.FiltreretOejebliksbillede ?? new FiltreretOejebliksbilledeType[0];
            foreach (var dto in facetDTOs)
            {
                Klasse klasseEntity = new Klasse();
                klasseEntity.UUID = dto.ObjektType.UUIDIdentifikator;
                klasseEntity.BrugervendtNoegle = dto.Registrering?[0]?.AttributListe?.Egenskab?[0]?.BrugervendtNoegleTekst;
                klasseEntity.Titel = dto.Registrering?[0]?.AttributListe?.Egenskab?[0]?.TitelTekst;
                klasseEntity.Beskrivelse = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].BeskrivelseTekst;
                klasseEntity.Omfang = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].OmfangTekst;
                klasseEntity.Retskilde = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].RetskildeTekst;
                klasseEntity.Aendringsnotat = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].AendringsnotatTekst;
                klasseEntity.Livscykluskode = dto.Registrering?[0]?.LivscyklusKode;
                klasseEntity.Tilstand = dto.Registrering?[0]?.TilstandListe?.PubliceretStatus?[0]?.ErPubliceretIndikator;
                klasseEntity.Ansvarlig = dto.Registrering?[0]?.RelationListe?.Ansvarlig?.ReferenceID?.Item;
                klasseEntity.Ejer = dto.Registrering?[0]?.RelationListe?.Ejer?.ReferenceID?.Item;
                klasseEntity.OverordnetKlasse = dto.Registrering?[0]?.RelationListe?.OverordnetKlasse?.ReferenceID?.Item;
                klasseEntity.KlasseTilhoerer = facetUUID;

                if (dto.Registrering?[0]?.AttributListe?.Egenskab?[0]?.Soegeord != null)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var x in dto.Registrering[0].AttributListe.Egenskab[0].Soegeord)
                    {
                        if (!string.IsNullOrEmpty(x.SoegeordIdentifikator))
                        {
                            if (builder.Length > 0)
                            {
                                builder.Append(",");
                            }

                            builder.Append(x.SoegeordIdentifikator);
                        }
                    }

                    klasseEntity.Soegeord = builder.ToString();
                }

                if (dto.Registrering?[0]?.RelationListe?.Redaktoerer != null)
                {
                    klasseEntity.Redaktoerer = new List<KlasseRedaktoer>();
                    foreach (var referenceDTO in dto.Registrering?[0]?.RelationListe?.Redaktoerer)
                    {
                        if (referenceDTO.ReferenceID?.Item != null)
                        {
                            KlasseRedaktoer reference = new KlasseRedaktoer();
                            reference.Value = referenceDTO.ReferenceID?.Item;
                            reference.Klasse = klasseEntity;
                            klasseEntity.Redaktoerer.Add(reference);
                        }
                    }
                }
                if (dto.Registrering?[0]?.RelationListe?.Erstatter != null)
                {
                    klasseEntity.Erstatter = new List<KlasseErstatter>();
                    foreach (var referenceDTO in dto.Registrering?[0]?.RelationListe?.Erstatter)
                    {
                        if (referenceDTO.ReferenceID?.Item != null)
                        {
                            KlasseErstatter reference = new KlasseErstatter();
                            reference.Value = referenceDTO.ReferenceID?.Item;
                            reference.Klasse = klasseEntity;
                            klasseEntity.Erstatter.Add(reference);
                        }
                    }
                }
                if (dto.Registrering?[0]?.RelationListe?.Sideordnede != null)
                {
                    klasseEntity.Sideordnet = new List<KlasseSideordnet>();
                    foreach (var referenceDTO in dto.Registrering?[0]?.RelationListe?.Sideordnede)
                    {
                        if (referenceDTO.ReferenceID?.Item != null)
                        {
                            KlasseSideordnet reference = new KlasseSideordnet();
                            reference.Value = referenceDTO.ReferenceID?.Item;
                            reference.Klasse = klasseEntity;
                            klasseEntity.Sideordnet.Add(reference);
                        }
                    }
                }
                if (dto.Registrering?[0]?.RelationListe?.Tilfoejelser != null)
                {
                    klasseEntity.Tilfoejelser = new List<KlasseTilfoejelse>();
                    foreach (var referenceDTO in dto.Registrering?[0]?.RelationListe?.Tilfoejelser)
                    {
                        if (referenceDTO.ReferenceID?.Item != null)
                        {
                            KlasseTilfoejelse reference = new KlasseTilfoejelse();
                            reference.Value = referenceDTO.ReferenceID?.Item;
                            reference.Klasse = klasseEntity;
                            klasseEntity.Tilfoejelser.Add(reference);
                        }
                    }
                }
                if (dto.Registrering?[0]?.RelationListe?.LovligeKombinationer != null)
                {
                    klasseEntity.LovligeKombinationer = new List<KlasseLovligeKombinationer>();
                    foreach (var referenceDTO in dto.Registrering?[0]?.RelationListe?.LovligeKombinationer)
                    {
                        if (referenceDTO.ReferenceID?.Item != null)
                        {
                            KlasseLovligeKombinationer reference = new KlasseLovligeKombinationer();
                            reference.Value = referenceDTO.ReferenceID?.Item;
                            reference.Klasse = klasseEntity;
                            klasseEntity.LovligeKombinationer.Add(reference);
                        }
                    }
                }
                
                klasseEntity.Facet = classificationContext.Facet.SingleOrDefault(c => c.UUID.Equals(facetUUID));

                klasse.Add(klasseEntity);
            }

            return klasse;
        }

        public void UpdateKlasse(List<Klasse> klasse)
        {
            foreach (var entity in klasse)
            {
                Klasse result = classificationContext.Klasse.SingleOrDefault(k => k.UUID.Equals(entity.UUID));
                if (result != null)
                {
                    result.BrugervendtNoegle = entity.BrugervendtNoegle;
                    result.Titel = entity.Titel;
                    result.Beskrivelse = entity.Beskrivelse;
                    result.Omfang = entity.Omfang;
                    result.Retskilde = entity.Retskilde;
                    result.Aendringsnotat = entity.Aendringsnotat;
                    result.Livscykluskode = entity.Livscykluskode;
                    result.Tilstand = entity.Tilstand;
                    result.Ansvarlig = entity.Ansvarlig;
                    result.Ejer = entity.Ejer;
                    result.Redaktoerer = entity.Redaktoerer;
                    result.Erstatter = entity.Erstatter;
                    result.Sideordnet = entity.Sideordnet;
                    result.Tilfoejelser = entity.Tilfoejelser;
                    result.LovligeKombinationer = entity.LovligeKombinationer;
                    result.Facet = entity.Facet;
                    result.OverordnetKlasse = entity.OverordnetKlasse;
                    result.Soegeord = result.Soegeord;
                    result.KlasseTilhoerer = result.KlasseTilhoerer;
                }
                else
                {
                    classificationContext.Klasse.Add(entity);
                }
            }

            classificationContext.SaveChanges();
        }
    }
}
