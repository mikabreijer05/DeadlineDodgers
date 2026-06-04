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
        throw new NotImplementedException();
    }

    public void GetAllTickets()
    {
        throw new NotImplementedException();
    }

    public string? GetTicketById(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteTicket(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateTicket(CustomerServiceTicket updatedTicket)
    {
        throw new NotImplementedException();
    }
}