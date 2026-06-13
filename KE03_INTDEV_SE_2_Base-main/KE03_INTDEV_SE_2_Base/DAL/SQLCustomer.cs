using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLCustomer : SQLDAL
{
    
    private static bool AddressChanged(Address? currentAddress, Address? newAddress)
    {
        if (currentAddress == null && newAddress == null)
        {
            return false;
        }

        if (currentAddress == null || newAddress == null)
        {
            return true;
        }

        return
            !string.Equals(currentAddress.Street?.Trim(), newAddress.Street?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.HouseNumber?.Trim(), newAddress.HouseNumber?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.PostalCode?.Trim(), newAddress.PostalCode?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.City?.Trim(), newAddress.City?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.Country?.Trim(), newAddress.Country?.Trim(), StringComparison.OrdinalIgnoreCase);
    }
    public List<Customer> GetAllCustomers()
    {
        const string sql = """
                           SELECT
                               a.AccId     AS Id,
                               a.CustName  AS Name,
                               a.AccActive AS Active,
                               aa.Address  AS AddressId,
                               addr.AddressId,
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
                    customer.Address = address;
                    customer.AddressId = address?.AddressId ?? customer.AddressId;
                    return customer;
                },
                splitOn: "AddressId").ToList();
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
                               aa.Address  AS AddressId,
                               addr.AddressId,
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

            return connection.Query<Customer, Address, Customer>(
                sql,
                (customer, address) =>
                {
                    customer.Address = address;
                    customer.AddressId = address?.AddressId ?? customer.AddressId;
                    return customer;
                },
                new { Id = id },
                splitOn: "AddressId").SingleOrDefault();
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
                INSERT INTO Address (
                    Street,
                    HouseNumber,
                    PostalCode,
                    City,
                    Country
                )
                VALUES (
                    @Street,
                    @HouseNumber,
                    @PostalCode,
                    @City,
                    @Country
                );

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

            const string selectAddressSql = """
                SELECT TOP 1
                    addr.AddressId,
                    addr.Street,
                    addr.HouseNumber,
                    addr.PostalCode,
                    addr.City,
                    addr.Country
                FROM AccountAddress aa
                LEFT JOIN Address addr ON addr.AddressId = aa.Address
                WHERE aa.Account = @AccountId
                """;

            const string insertAddressSql = """
                INSERT INTO Address (
                    Street,
                    HouseNumber,
                    PostalCode,
                    City,
                    Country
                )
                VALUES (
                    @Street,
                    @HouseNumber,
                    @PostalCode,
                    @City,
                    @Country
                );

                SELECT CAST(SCOPE_IDENTITY() AS int);
                """;

            const string updateAccountAddressSql = """
                UPDATE AccountAddress
                SET Address = @AddressId
                WHERE Account = @AccountId
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

                var currentAddress = connection.QuerySingleOrDefault<Address>(
                    selectAddressSql,
                    new { AccountId = customer.Id },
                    transaction);

                if (customer.Address != null && AddressChanged(currentAddress, customer.Address))
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

                    if (currentAddress != null)
                    {
                        connection.Execute(
                            updateAccountAddressSql,
                            new { AccountId = customer.Id, AddressId = newAddressId },
                            transaction);
                    }
                    else
                    {
                        connection.Execute(
                            insertAccountAddressSql,
                            new { AccountId = customer.Id, AddressId = newAddressId },
                            transaction);
                    }
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