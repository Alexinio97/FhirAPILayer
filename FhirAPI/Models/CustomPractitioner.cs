using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FhirAPI.Models
{
    public class CustomPractitioner
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> FirstNames { get; set; }
    }
}
