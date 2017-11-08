using Task5.Attributes;

namespace Task5.Model1
{
    [ImportConstructor]
    public class CustomerBLL
    {
        public CustomerBLL(ICustomerDAL dal, Logger logger)
        {
        }
    }
}