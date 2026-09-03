namespace KE03_INTDEV_SE_2_Base.Models;

public class VehicleDefect
{
    public int Id { get; set; }
    public Vehicle Vehicle { get; set; }
    public string DefectDescription { get; set; }
    public DateTime DateReported { get; set; }
    public string ImageUrl { get; set; }
}