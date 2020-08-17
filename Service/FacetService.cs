using FacetWebService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StsKlassifikation.Service
{
    public class FacetService
    {
        private readonly ClassificationContext classificationContext;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public FacetService(IServiceProvider sp)
        {
            classificationContext = sp.GetService<ClassificationContext>();
            logger = sp.GetService<ILogger<ClassificationService>>();
            configuration = sp.GetService<IConfiguration>();        
        }

        public List<Facet> GetFacets(string classificationUUID)
        {
            int maxResults = 100;
            FacetPortTypeClient port = new FacetPortTypeClient(configuration["FacetService:serviceUrl"], configuration["certPath"], configuration["certPassword"]);

            SoegRequestType request = new SoegRequestType();
            request.AuthorityContext = new AuthorityContextType();
            request.AuthorityContext.MunicipalityCVR = configuration["cvr"];

            request.SoegInput = new SoegInputType1();
            request.SoegInput.FoersteResultatReference = "0";
            request.SoegInput.MaksimalAntalKvantitet = maxResults + "";
            request.SoegInput.RelationListe = new RelationListeType();
            request.SoegInput.AttributListe = new AttributListeType();
            request.SoegInput.TilstandListe = new TilstandListeType();
            request.SoegInput.RelationListe.FacetTilhoerer = new KlassifikationRelationType();
            request.SoegInput.RelationListe.FacetTilhoerer.ReferenceID = new UnikIdType();
            request.SoegInput.RelationListe.FacetTilhoerer.ReferenceID.Item = classificationUUID;
            request.SoegInput.RelationListe.FacetTilhoerer.ReferenceID.ItemElementName = ItemChoiceType.UUIDIdentifikator;

            List<Facet> facets = new List<Facet>();

            var response = port.soegAsync(null, request).Result;

            if ("20".Equals(response.SoegResponse.SoegOutput.StandardRetur.StatusKode))
            {

                if (response.SoegResponse.SoegOutput.IdListe.Length >= maxResults)
                {
                    facets.AddRange(ReadFacetsFromResponse(response, classificationUUID));

                    while ("20".Equals(response.SoegResponse.SoegOutput.StandardRetur.StatusKode) || response.SoegResponse.SoegOutput.IdListe.Length > 0)
                    {
                        request.SoegInput.FoersteResultatReference = (int.Parse(request.SoegInput.FoersteResultatReference) + response.SoegResponse.SoegOutput.IdListe.Length) + "";
                        response = port.soegAsync(null, request).Result;

                        facets.AddRange(ReadFacetsFromResponse(response, classificationUUID));
                    }
                }
                else
                {
                    facets.AddRange(ReadFacetsFromResponse(response, classificationUUID));
                }

            }
            else
            {
                logger.LogWarning(response.SoegResponse.SoegOutput.StandardRetur.FejlbeskedTekst);
            }

            return facets;
        }

        private List<Facet> ReadFacetsFromResponse(soegResponse response, string classificationUUID)
        {
            FacetPortTypeClient port = new FacetPortTypeClient(configuration["FacetService:serviceUrl"], configuration["certPath"], configuration["certPassword"]);

            ListRequestType request = new ListRequestType();
            request.AuthorityContext = new AuthorityContextType();
            request.AuthorityContext.MunicipalityCVR = configuration["cvr"];

            request.ListInput = new ListInputType();
            request.ListInput.UUIDIdentifikator = response.SoegResponse.SoegOutput.IdListe;

            var listResponse = port.listAsync(null, request).Result;

            List<Facet> facets = new List<Facet>();
            var facetDTOs = listResponse.ListResponse.ListOutput.FiltreretOejebliksbillede ?? new FiltreretOejebliksbilledeType[0];
            foreach (var dto in facetDTOs)
            {
                Facet facet = new Facet();
                facet.UUID = dto.ObjektType.UUIDIdentifikator;
                facet.BrugervendtNoegle = dto.Registrering?[0]?.AttributListe?.Egenskab?[0]?.BrugervendtNoegleTekst;
                facet.Titel = dto.Registrering?[0]?.AttributListe?.Egenskab?[0]?.TitelTekst;
                facet.Beskrivelse = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].BeskrivelseTekst;
                facet.Opbygning = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].OpbygningTekst;
                facet.Ophavsret = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].OphavsretTekst;
                facet.PlanIdentifikator = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].PlanIdentifikator;
                facet.SupplementTekst = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].SupplementTekst;
                facet.RetskildeTekst = dto.Registrering?[0]?.AttributListe?.Egenskab?[0].RetskildeTekst;
                facet.Livscykluskode = dto.Registrering?[0]?.LivscyklusKode;
                facet.Publiceret = dto.Registrering?[0]?.TilstandListe?.PubliceretStatus?[0]?.ErPubliceretIndikator;
                facet.Ansvarlig = dto.Registrering?[0]?.RelationListe?.Ansvarlig?.ReferenceID?.Item;
                facet.Ejer = dto.Registrering?[0]?.RelationListe?.Ejer?.ReferenceID?.Item;
                facet.FacetTilhoerer = dto.Registrering?[0]?.RelationListe?.FacetTilhoerer?.ReferenceID?.Item;

                if (dto.Registrering?[0]?.RelationListe?.Redaktoerer != null)
                {
                    foreach (var redactorDTO in dto.Registrering?[0]?.RelationListe?.Redaktoerer)
                    {
                        if (redactorDTO.ReferenceID?.Item != null)
                        {
                            FacetRedaktoer redaktoer = new FacetRedaktoer();
                            redaktoer.Value = redactorDTO.ReferenceID?.Item;
                            redaktoer.Facet = facet;
                            facet.Redaktoerer.Add(redaktoer);
                        }
                    }
                }
                facet.Klassifikation = classificationContext.Klassifikation.SingleOrDefault(c => c.UUID.Equals(classificationUUID));

                facets.Add(facet);
            }

            return facets;
        }

        public void UpdateFacets(List<Facet> facets)
        {
            foreach (var facet in facets)
            {
                Facet result = classificationContext.Facet.SingleOrDefault(f => f.UUID.Equals(facet.UUID));
                if (result != null)
                {
                    result.BrugervendtNoegle = facet.BrugervendtNoegle;
                    result.Titel = facet.Titel;
                    result.Beskrivelse = facet.Beskrivelse;
                    result.Opbygning = facet.Opbygning;
                    result.Ophavsret = facet.Ophavsret;
                    result.PlanIdentifikator = facet.PlanIdentifikator;
                    result.SupplementTekst = facet.SupplementTekst;
                    result.RetskildeTekst = facet.RetskildeTekst;
                    result.Livscykluskode = facet.Livscykluskode;
                    result.Publiceret = facet.Publiceret;
                    result.Ansvarlig = facet.Ansvarlig;
                    result.Ejer = facet.Ejer;
                    result.FacetTilhoerer = facet.FacetTilhoerer;
                }
                else
                {
                    classificationContext.Facet.Add(facet);
                }
            }

            classificationContext.SaveChanges();
        }
    }
}
