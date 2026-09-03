using Dapper;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLCustomer(IConfiguration configuration) : SQLDAL(configuration)
{
    public List<Customer> GetAllCustomers()
    {
        const string sql = """
                           SELECT
                               a.AccId     AS Id,
                               a.AccName   AS UserName,
                               a.CustName  AS Name,
                               a.AccActive AS Active,
                               addr.Street,
                               addr.HouseNumber,
                               addr.PostalCode,
                               addr.City,
                               addr.Country
                           FROM Account a
                           LEFT JOIN AccountAddress aa ON aa.Account = a.AccId
                           LEFT JOIN Address addr      ON addr.AddressId = aa.Address
                           WHERE a.CustName IS NOT NULL
                           ORDER BY a.CustName
                           """;

        try
        {
            connection.Open();

            return connection.Query<Customer, Address, Customer>(
                    sql,
                    (customer, address) =>
                    {
                        customer.Address = new Address
                        {
                            Street = address.Street,
                            HouseNumber = address.HouseNumber,
                            PostalCode = address.PostalCode,
                            City = address.City,
                            Country = address.Country
                        };
                        return customer;
                    },
                    splitOn: "Street")
                .ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public Customer? GetCustomerById(int id)
    {
        const string sql = """
                           SELECT
                               a.AccId     AS Id,
                               a.AccName   AS UserName,
                               a.CustName  AS Name,
                               a.AccActive AS Active,
                               addr.Street,
                               addr.HouseNumber,
                               addr.PostalCode,
                               addr.City,
                               addr.Country
                           FROM Account a
                           LEFT JOIN AccountAddress aa ON aa.Account = a.AccId
                           LEFT JOIN Address addr      ON addr.AddressId = aa.Address
                           WHERE a.AccId = @Id
                           """;

        try
        {
            connection.Open();

            var result = connection.QueryFirstOrDefault(sql, new { Id = id });

            if (result == null)
                return null;

            var customer = new Customer
            {
                Id = result.Id,
                UserName = result.UserName,
                Name = result.Name,
                Active = result.Active,
                Address = new Address
                {
                    Street = result.Street,
                    HouseNumber = result.HouseNumber,
                    PostalCode = result.PostalCode,
                    City = result.City,
                    Country = result.Country
                }
            };

            return customer;
        }
        finally
        {
            CloseConnection();
        }
    }

    public void AddCustomer(Customer customer)
    {
        const string insertAccountSql = """
                                        INSERT INTO Account (AccName, CustName, AccActive)
                                        VALUES (@UserName, @Name, @Active);

                                        SELECT CAST(SCOPE_IDENTITY() AS int);
                                        """;

        const string insertAddressSql = """
                                        INSERT INTO Address (Street, HouseNumber, PostalCode, City, Country)
                                        VALUES (@Street, @HouseNumber, @PostalCode, @City, @Country);

                                        SELECT CAST(SCOPE_IDENTITY() AS int);
                                        """;

        const string insertAccountAddressSql = """
                                               INSERT INTO AccountAddress (Account, Address)
                                               VALUES (@AccountId, @AddressId);
                                               """;

        try
        {
            connection.Open();

            using var transaction = connection.BeginTransaction();

            customer.Id = connection.QuerySingle<int>(
                insertAccountSql,
                new { customer.UserName, customer.Name, customer.Active },
                transaction);

            if (customer.Address != null)
            {
                var addressId = connection.QuerySingle<int>(
                    insertAddressSql,
                    new
                    {
                        customer.Address.Street,
                        customer.Address.HouseNumber,
                        customer.Address.PostalCode,
                        customer.Address.City,
                        customer.Address.Country
                    },
                    transaction);

                connection.Execute(
                    insertAccountAddressSql,
                    new { AccountId = customer.Id, AddressId = addressId },
                    transaction);
            }

            transaction.Commit();
        }
        finally
        {
            CloseConnection();
        }
    }
}