using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using FhirAPI.Models;

namespace FhirAPI.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : Controller
    {
        private readonly string BASEURL = "http://hapi.fhir.org/baseDstu3";
        FhirClient _client;
        public PatientController(FhirClient client)
        {
            _client = client;
            // fhir client settings
            _client.Settings.PreferredFormat = ResourceFormat.Json;
            _client.Settings.PreferredReturn = Prefer.ReturnRepresentation;
        }

        [HttpGet]
        public async Task<ActionResult <IEnumerable<CustomPatient>>> GetPatientsAsync()
        {
            List<CustomPatient> patientList = new List<CustomPatient>();
            var filterParams = new SearchParams()
                                .Where("identifier=UPTValue");

            Bundle result = await _client.SearchAsync<Patient>(filterParams);

            while( result != null)
            {
                foreach (var item in result.Entry)
                {
                    var patientRetrieved = (Patient)item.Resource;
                    var customPatient = new CustomPatient()
                    {
                        Name = patientRetrieved.Name.FirstOrDefault().Family,
                        FirstNames = patientRetrieved.Name.FirstOrDefault().Given.ToList(),
                        Id = patientRetrieved.Id
                    };
                    patientList.Add(customPatient);
                }
                result = _client.Continue(result, PageDirection.Next);
            }

            return Ok(patientList);
        }

        // GET: PatientController/Details/5
        [HttpGet("{id}",Name ="GetPatientByIdAsync")]
        public async Task<ActionResult> GetPatientByIdAsync(int id)
        {
            var patient = await _client.ReadAsync<Patient>($"Patient/{id}");
            
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        // POST: PatientController/Create
        [HttpPost]
        public async Task<ActionResult> Create(CustomPatient customPatient)
        {
            try
            {
                // add new patient to fhir api test server
                var patientToAdd = new Patient();
                var patientName = new HumanName();
                patientName.Family = customPatient.Name;
                patientName.Given = customPatient.FirstNames;
                patientToAdd.Name.Add(patientName);

                var identifier = new Identifier("UptSystem", "UPTValue");
                

                patientToAdd.Identifier.Add(identifier);

                var result = await _client.CreateAsync(patientToAdd);
                return Created($"{BASEURL}/Patient/{result.Id}", "Created with succes!");
            }
            catch
            {
                return BadRequest("Wrong object!");
            }
        }
    }
}
