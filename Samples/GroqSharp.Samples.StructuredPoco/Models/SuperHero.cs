using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroqSharp.Samples.StructuredPoco.Models
{
    public class SuperHero
    {
        public string Name { get; set; }

        public PowerDetail Powers { get; set; }
    }
}
