using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3.Tests
{
    [TestFixture]
    public class LinqTests
    {
        private const decimal AverageCustomerTotal = 14222.393932584269662921348315M;
        private const decimal AverageOrderTotal = 1158.0510175700020780629472474M;

        private List<Product> _products;
        private List<Supplier> _suppliers;
        private List<Customer> _customers;

        [SetUp]
        public void SetUp()
        {
            _products = DataSource.CreateProducts();
            _suppliers = DataSource.CreateSuppliers();
            _customers = DataSource.CreateCustomers();
        }

        [Test]
        [Description("Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X.")]
        public async Task Linq1()
        {
            decimal x = AverageCustomerTotal;

            var filteredCustomersRequest = _customers.Where(c => c.Orders.Sum(o => o.Total) > x);

            await TestContext.Out.WriteLineAsync($"x = {x}");
            foreach(var c in filteredCustomersRequest)
            {
                await TestContext.Out.WriteLineAsync($"Customer: {c.CustomerID}");
            }
        }

        [Test]
        [Description("Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. С группировкой.")]
        public async Task Linq2WithGroup()
        {
            var request = _customers.GroupJoin(_suppliers,
                c => new { c.City, c.Country },
                s => new { s.City, s.Country },
                (c, s) => new { CustomerID = c.CustomerID, Suppliers = s });

            foreach (var customer in request)
            {
                await TestContext.Out.WriteLineAsync($"Customer: {customer.CustomerID}");
                foreach (var supplier in customer.Suppliers)
                {
                    await TestContext.Out.WriteLineAsync($"Supplier: {supplier.SupplierName}");
                }
            }
        }

        [Test]
        [Description("Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. Без группировки.")]
        public async Task Linq2WithoutGroup()
        {
            foreach(var customer in _customers)
            {
                await TestContext.Out.WriteLineAsync($"Customer: {customer.CustomerID}");
                foreach (var supplier in _suppliers.Where(s => s.Country == customer.Country && s.City == customer.City))
                {
                    await TestContext.Out.WriteLineAsync($"Supplier: {supplier.SupplierName}");
                }
            }
        }

        [Test]
        [Description("Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X.")]
        public async Task Linq3()
        {
            decimal x = AverageOrderTotal;

            var filteredCustomersRequest = _customers.Where(c => c.Orders.Any(o => o.Total > x));

            await TestContext.Out.WriteLineAsync($"x = {x}");
            foreach (var c in filteredCustomersRequest)
            {
                await TestContext.Out.WriteLineAsync($"Customer: {c.CustomerID}");
            }
        }

        [Test]
        [Description("Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами (принять за таковые месяц и год самого первого заказа).")]
        public async Task Linq4()
        {
            var request = _customers.Where(c => c.Orders.Any()).Select(c => new
            {
                CustomerID = c.CustomerID,
                FirstOrderDate = c.Orders.Min(o => o.OrderDate)
            });

            foreach (var data in request)
            {
                await TestContext.Out.WriteLineAsync($"{data.CustomerID} - {data.FirstOrderDate}");
            }
        }

        [Test]
        [Description("Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента (от максимального к минимальному) и имени клиента.")]
        public async Task Linq5()
        {
            var request = _customers.Where(c => c.Orders.Any())
                .Select(c => new
                {
                    CustomerID = c.CustomerID,
                    CustomerName = c.CompanyName,
                    Total = c.Orders.Sum(o => o.Total),
                    FirstOrderDate = c.Orders.Min(o => o.OrderDate)
                })
                .OrderBy(d => d.FirstOrderDate.Year)
                .ThenBy(d => d.FirstOrderDate.Month)
                .ThenBy(d => d.Total)
                .ThenBy(d => d.CustomerName);

            foreach (var data in request)
            {
                await TestContext.Out.WriteLineAsync($"{data.CustomerID} - {data.FirstOrderDate} ({data.FirstOrderDate.Year}, {data.FirstOrderDate.Month}, {data.Total}, {data.CustomerName})");
            }
        }

        [Test]
        [Description("Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).")]
        public async Task Linq6()
        {
            var regexp = new Regex(@"^.+\(.+\).+$");

            var request = _customers
                .Where(c =>
                {
                    return (c.PostalCode != null && !c.PostalCode.All(char.IsDigit))
                        || !string.IsNullOrEmpty(c.Region)
                        || regexp.IsMatch(c.Phone);
                });

            foreach (var c in request)
            {
                await TestContext.Out.WriteLineAsync($"{c.CustomerID}: {c.PostalCode ?? "Empty"}, {c.Region}, {c.Phone}");
            }
        }

        [Test]
        [Description("Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости.")]
        public async Task Linq7()
        {
            var request = _products
                .GroupBy(p => p.Category)
                .Select(g => {
                    var innerGroups = g.GroupBy(p => p.UnitsInStock).OrderBy(ig => ig.Count()).ToArray();
                    var lastGroup = innerGroups.LastOrDefault();

                    return new
                    {
                        Category = g.Key,
                        Group = innerGroups.Select(ig => new
                        {
                            UnitsInStock = ig.Key,
                            Products = ig == lastGroup ? ig.OrderBy(e => e.UnitPrice) : (IEnumerable<Product>)ig
                        })
                    };
                });

            foreach (var g1 in request)
            {
                await TestContext.Out.WriteLineAsync($"Category: {g1.Category}");
                foreach (var g2 in g1.Group)
                {
                    await TestContext.Out.WriteLineAsync($"UnitsInStock: {g2.UnitsInStock}");
                    foreach (var p in g2.Products)
                    {
                        await TestContext.Out.WriteLineAsync($"    Product: {p.ProductName}, {p.UnitPrice}");
                    }
                }
                await TestContext.Out.WriteLineAsync();
            }
        }

        [Test]
        [Description("Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами.")]
        public async Task Linq8()
        {
            var request = _products.GroupBy(p => {
                if (p.UnitPrice < 10)
                {
                    return 1;
                }
                else if (p.UnitPrice > 20)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            });

            foreach (var g in request)
            {
                foreach (var p in g)
                {
                    await TestContext.Out.WriteLineAsync($"{p.ProductName} - {p.UnitPrice}");
                }
                await TestContext.Out.WriteLineAsync();
            }
        }

        [Test]
        [Description("Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города) и среднюю интенсивность (среднее количество заказов, приходящееся на клиента из каждого города).")]
        public async Task Linq9()
        {
            var request = _customers
                .GroupBy(c => c.City)
                .Select(g => new
                {
                    City = g.Key,
                    Revenue = g.Where(c => c.Orders.Any()).Average(c => c.Orders.Average(o => o.Total)),
                    Intensity = g.Aggregate(0, (a, c) => c.Orders.Length + a) / g.Count()
                });

            foreach (var g in request)
            {
                await TestContext.Out.WriteLineAsync($"{g.City}: {g.Revenue}, {g.Intensity}");
            }
        }

        [Test]
        [Description("Сделайте среднегодовую статистику активности клиентов по месяцам (без учета года), статистику по годам, по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).")]
        public async Task Linq10()
        {
            foreach (var filter in new Func<Order, object>[] {
                o => o.OrderDate.Month,
                o => o.OrderDate.Year,
                o => new { o.OrderDate.Year, o.OrderDate.Month }
            })
            {
                foreach(var data in _customers.SelectMany(c => c.Orders).GroupBy(filter))
                {
                    await TestContext.Out.WriteLineAsync($"{data.Key}: {data.Sum(s => s.Total)}");
                }
                await TestContext.Out.WriteLineAsync();
            }
        }
    }
}
