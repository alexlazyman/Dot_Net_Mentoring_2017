using Task5.Attributes;

namespace Task5.Model1
{
    [Export(typeof(ICustomerDAL))]
    public class CustomerDAL : ICustomerDAL
    {
    }
}