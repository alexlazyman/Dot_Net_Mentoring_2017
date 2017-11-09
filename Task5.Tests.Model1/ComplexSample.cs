using Task5.Attributes;

namespace Task5.Tests.Model1
{
    public class ComplexSample
    {
        [Import]
        public ISample Sample { get; set; }

        public ISample Sample_NotImport { get; set; }

        [Import]
        public Sample1 Sample1 { get; set; }

        public Sample1 Sample1_NotImport { get; set; }

        public ISample2 Sample2 { get; set; }

        public Sample3 Sample3 { get; set; }

        public ComplexSample(ISample2 sample2, Sample3 sample3)
        {
            Sample2 = sample2;
            Sample3 = sample3;
        }
    }
}