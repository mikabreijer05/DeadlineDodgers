using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLCustomerService : SQLDAL
{
    public IEnumerable<CustomerServiceTicket> GetAllCustomerServiceTickets()
    {
        const string sql = @"
            SELECT
                t.TicketId          AS Id,
                t.TicketDateCreated AS DateCreated,
                t.CustomerId        AS CustomerId,
                CASE WHEN t.TicketStatus = 'Open' THEN 1 ELSE 0 END AS IsActive,

                cust.AccId  AS Id,
                cust.AccName AS Name,

                ct.ContactId          AS Id,
                cm.MethodName         AS ContactType,
                ct.ContactDescription AS Description
            FROM dbo.CustomerServiceTicket t
            LEFT JOIN dbo.Account cust       ON cust.AccId = t.CustomerId
            LEFT JOIN dbo.Contact ct         ON ct.TicketId = t.TicketId
            LEFT JOIN dbo.ContactMethod cm   ON cm.CMId = ct.CMId;";

        var ticketMap = new Dictionary<int, CustomerServiceTicket>();

        try
        {
            connection.Open();

            connection.Query<CustomerServiceTicket, Customer, ContactDescriptions, CustomerServiceTicket>(
                sql,
                (ticket, customer, contact) =>
                {
                    if (!ticketMap.TryGetValue(ticket.Id, out var existingTicket))
                    {
                        existingTicket = ticket;
                        existingTicket.Customer = customer;
                        ticketMap.Add(existingTicket.Id, existingTicket);
                    }

                    if (contact is { Id: > 0 })
                    {
                        existingTicket.ContactDescriptions.Add(contact);
                    }

                    return existingTicket;
                },
                splitOn: "Id,Id");

            return ticketMap.Values.ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public void AddCustomerServiceTicket(CustomerServiceTicket newTicket)
    {
        const string insertSql = @"
            INSERT INTO dbo.CustomerServiceTicket (CustomerId, TicketTitle, TicketDescription, TicketStatus, TicketDateCreated, TicketPriority)
            VALUES (@CustomerId, @Title, @Description, @Status, @DateCreated, @Priority);
            SELECT CAST(SCOPE_IDENTITY() AS int);
        ";

        try
        {
            connection.Open();

            newTicket.Id = connection.QuerySingle<int>(insertSql, new
            {
                newTicket.CustomerId,
                Title = newTicket.Title,
                newTicket.Description,
                Status = newTicket.IsActive ? "Open" : "Closed",
                DateCreated = newTicket.DateCreated == default ? DateTime.UtcNow.Date : newTicket.DateCreated,
                Priority = (string?)null
            });
        }
        finally
        {
            CloseConnection();
        }
    }

    public void GetAllTickets()
    {
        throw new NotImplementedException();
    }

    public CustomerServiceTicket? GetTicketById(int id)
    {
        const string sql = @"
            SELECT
                t.TicketId          AS Id,
                t.TicketTitle       AS Title,
                t.TicketDescription AS Description,
                t.TicketDateCreated AS DateCreated,
                t.CustomerId        AS CustomerId,
                CASE WHEN t.TicketStatus = 'Open' THEN 1 ELSE 0 END AS IsActive,

                cust.AccId  AS Id,
                cust.AccName AS Name
            FROM dbo.CustomerServiceTicket t
            LEFT JOIN dbo.Account cust ON cust.AccId = t.CustomerId
            WHERE t.TicketId = @Id;";

        try
        {
            connection.Open();

            var ticket = connection.Query<CustomerServiceTicket, Customer, CustomerServiceTicket>(
                sql,
                (t, c) =>
                {
                    t.Customer = c;
                    return t;
                },
                new { Id = id },
                splitOn: "Id").FirstOrDefault();

            return ticket;
        }
        finally
        {
            CloseConnection();
        }
    }

    public void DeleteTicket(int id)
    {
        const string sql = "DELETE FROM dbo.CustomerServiceTicket WHERE TicketId = @Id";

        try
        {
            connection.Open();
            connection.Execute(sql, new { Id = id });
        }
        finally
        {
            CloseConnection();
        }
    }

    public void UpdateTicket(CustomerServiceTicket updatedTicket)
    {
        const string sql = @"
            UPDATE dbo.CustomerServiceTicket
            SET TicketTitle = @Title,
                TicketDescription = @Description,
                TicketStatus = @Status
            WHERE TicketId = @Id";

        try
        {
            connection.Open();
            connection.Execute(sql, new
            {
                updatedTicket.Id,
                Title = updatedTicket.Title,
                Description = updatedTicket.Description,
                Status = updatedTicket.IsActive ? "Open" : "Closed"
            });
        }
        finally
        {
            CloseConnection();
        }
    }
}