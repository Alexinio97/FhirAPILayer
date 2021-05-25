using FhirAPI.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.DiagnosticReport;

namespace FhirAPI.Controllers
{
    [ApiController]
    [Route("api/diagnostics")]
    public class DiagnosticController : Controller
    {
        private readonly string BASEURL = "http://hapi.fhir.org/baseDstu3";
        private readonly FhirClient _client;
        public DiagnosticController(FhirClient client)
        {
            _client = client;
            // fhir client settings
            _client.Settings.PreferredFormat = ResourceFormat.Json;
            _client.Settings.PreferredReturn = Prefer.ReturnRepresentation;
        }

        // GET: api/diagnostics
        [HttpGet]
        public async Task<ActionResult <IEnumerable<CustomDiagnostic>>> GetDiagnosticsAsync()
        {
            List<CustomDiagnostic> diagnostics = new List<CustomDiagnostic>();
            var filterParams = new SearchParams()
                                .Where("identifier=UPTValue");

            Bundle result = await _client.SearchAsync<DiagnosticReport>(filterParams);

            while (result != null)
            {
                foreach (var item in result.Entry)
                {
                    var diagnosticReceived = (DiagnosticReport)item.Resource;
                    var customDiagnostic = new CustomDiagnostic
                    {
                        Id = diagnosticReceived.Id,
                        PatientRef = diagnosticReceived.Performer.FirstOrDefault().ToString(),
                        PractitionerRef = diagnosticReceived.Subject.ToString(),
                        IssueDate = diagnosticReceived.Issued.Value
                    };
                    diagnostics.Add(customDiagnostic);
                }
                result = _client.Continue(result, PageDirection.Next);
            }

            return Ok(diagnostics);
        }

        // GET: api/diagnostics/{id}
        [HttpGet("{id}", Name = "GetDiagnosticByIdAsync")]
        public async Task<IActionResult> GetDiagnosticByIdAsync(int id)
        {
            var diag = await _client.ReadAsync<Patient>($"DiagnosticReport/{id}");

            if (diag == null)
            {
                return NotFound();
            }

            return Ok(diag);
        }


        // POST: api/diagnostics
        [HttpPost]
        public async Task<IActionResult> CreateDiagnosticAsync(string patientReference, string practitionerReference)
        {
            DiagnosticReport diagReport = new DiagnosticReport();
            var identifier = new Identifier("UptSystem", "UPTValue");

            diagReport.Identifier.Add(identifier);

            ResourceReference patientRef = new ResourceReference(patientReference);
            diagReport.Subject = patientRef;

            PerformerComponent performerComponent = new PerformerComponent();
            performerComponent.Actor = new ResourceReference(practitionerReference);
            // multiple doctors reference can be added here
            diagReport.Performer.Add(performerComponent);
            diagReport.Issued = DateTimeOffset.Now;

            var response = await _client.CreateAsync(diagReport);
            
            if(response == null)
            {
                return BadRequest();
            }
            return Created($"{BASEURL}/DiagnosticReport/{response.Id}", "Created with succes!");

        } 
    }
}
