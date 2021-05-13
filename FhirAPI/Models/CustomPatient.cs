using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirAPI.Models
{
    // wrapper class on Fhir.Patient
    public class CustomPatient
    {
        public string Name { get; set; }
        public IEnumerable<string> FirstNames { get; set; }
    }
}
