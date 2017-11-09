using Task5.Attributes;

namespace Task5.Tests.Model1
{
    public class ImportSample
    {
        [Import]
        public Sample Sample { get; set; }
    }
}