using FhirAPI.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirAPI.Controllers
{
    [ApiController]
    [Route("api/practitioner")]
    public class PractitionerController : Controller
    {
        private readonly string BASEURL = "http://hapi.fhir.org/baseDstu3";
        private readonly FhirClient _client;
        public PractitionerController(FhirClient client)
        {
            _client = client;
            // fhir client settings
            _client.Settings.PreferredFormat = ResourceFormat.Json;
            _client.Settings.PreferredReturn = Prefer.ReturnRepresentation;
        }

        // GET: api/practitioner/{id}
        [HttpGet]
        public async Task<IActionResult> GetPractitionerByIdAsync(string id)
        {
            var practitioner = await _client.ReadAsync<Practitioner>($"Practitioner/{id}");

            if (practitioner == null)
            {
                return NotFound();
            }

            return Ok(practitioner);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePractitionerAsync(CustomPractitioner customPractitioner)
        {
            Practitioner practitioner = new Practitioner();
            practitioner.Identifier.Add(new Identifier("UptSystem", "UPTValue"));
            practitioner.Name.Add(new HumanName()
            {
                Given = customPractitioner.FirstNames,
                Family = customPractitioner.Name
            });

            var response = await _client.CreateAsync(practitioner);
            if(response == null)
            {
                return BadRequest("Object sent was not correct!");
            }
            return Created($"{BASEURL}/Practitioner/{response.Id}", "Created with succes!");
        }
    }
}
