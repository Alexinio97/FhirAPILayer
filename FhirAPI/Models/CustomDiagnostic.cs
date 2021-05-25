using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirAPI.Models
{
    public class CustomDiagnostic
    {
        public string Id { get; set; }
        public string PractitionerRef { get; set; }
        public string PatientRef { get; set; }
        public DateTimeOffset IssueDate { get; set; }
    }
}
