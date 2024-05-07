using GroqSharp.Annotations;

namespace GroqSharp.Samples.StructuredPoco.Models
{
    public class SuperHero
    {
        [ContextDescription("Name of the superhero - make it up not a common name")]
        public string Name { get; set; }

        [ContextDescription("Description of the superhero as a pirate")]
        [ContextLength(250)]
        public string Description { get; set; }

        public string RealName { get; set; }

        [ContextDescription("The age in human years")]
        [ContextRange(min:0, max:1000)]
        public int Age { get; set; }

        [ContextDescription("The freelance hiring rate in dollars and cents")]
        [ContextRange(min: 10.00, max: 999.99)]
        public double HiringRatePerHour { get; set; }

        public PowerDetail Powers { get; set; }
    }
}
