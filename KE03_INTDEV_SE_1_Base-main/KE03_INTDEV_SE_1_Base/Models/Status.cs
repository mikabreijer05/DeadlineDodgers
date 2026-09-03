namespace KE03_INTDEV_SE_1_Base.Models
{
    public class Status
    {
        public int StatusId { get; set; }

        // textual status value from DB (aliased as 'StatusName')
        public string? StatusName { get; set; }
    }
}
