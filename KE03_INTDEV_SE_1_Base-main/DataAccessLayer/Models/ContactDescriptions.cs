namespace DataAccessLayer.Models;

public class ContactDescriptions
{
    public int Id { get; set; }
    public string ContactType {get; set;}
    public string Description { get; set; }
    public DateTime DateCreated { get; set; }
    public int CoWorkerId { get; set; }
    public CoWorker CoWorker { get; set; }
}