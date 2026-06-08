using Dapper;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLCustomer : SQLDAL
{
    public List<Customer> GetAllCustomers()
    {
        const string sql = """
                           SELECT
                               a.AccId     AS Id,
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
                    splitOn: "Street,HouseNumber,PostalCode,City,Country")
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
                                        INSERT INTO Account (CustName, AccActive)
                                        VALUES (@Name, @Active);

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
                new { customer.Name, customer.Active },
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

    public void UpdateCustomer(Customer customer)
    {
        const string updateAccountSql = """
            UPDATE Account
            SET CustName = @Name,
                AccActive = @Active
            WHERE AccId = @Id
            """;

        const string selectAddressIdSql = """
            SELECT TOP 1 aa.AddressId
            FROM AccountAddress aa
            WHERE aa.Account = @AccountId
            """;

        const string updateAddressSql = """
            UPDATE Address
            SET Street = @Street,
                HouseNumber = @HouseNumber,
                PostalCode = @PostalCode,
                City = @City,
                Country = @Country
            WHERE AddressId = @AddressId
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

            connection.Execute(
                updateAccountSql,
                new { customer.Id, customer.Name, customer.Active },
                transaction);

            var addressId = connection.QuerySingleOrDefault<int?>(
                selectAddressIdSql,
                new { AccountId = customer.Id },
                transaction);

            if (addressId.HasValue)
            {
                connection.Execute(
                    updateAddressSql,
                    new
                    {
                        customer.Address.Street,
                        customer.Address.HouseNumber,
                        customer.Address.PostalCode,
                        customer.Address.City,
                        customer.Address.Country,
                        AddressId = addressId.Value
                    },
                    transaction);
            }
            else if (customer.Address != null)
            {
                var newAddressId = connection.QuerySingle<int>(
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
                    new { AccountId = customer.Id, AddressId = newAddressId },
                    transaction);
            }

            transaction.Commit();
        }
        finally
        {
            CloseConnection();
        }
    }

    public void DeleteCustomer(int id)
    {
        const string deleteAccountAddressSql = """
                                               DELETE FROM AccountAddress
                                               WHERE Account = @Id
                                               """;

        const string deleteAccountSql = """
                                        DELETE FROM Account
                                        WHERE AccId = @Id
                                        """;

        try
        {
            connection.Open();

            using var transaction = connection.BeginTransaction();

            connection.Execute(deleteAccountAddressSql, new { Id = id }, transaction);
            connection.Execute(deleteAccountSql, new { Id = id }, transaction);

            transaction.Commit();
        }
        finally
        {
            CloseConnection();
        }
    }
}