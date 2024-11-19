namespace Involved.HTF.Common.Dto;

public class BattleOfNovaCentauriDto
{
    public List<Alien> TeamA { get; set; } = null!;
    public List<Alien> TeamB { get; set; } = null!;
}

public sealed class Alien
{
    public int Strength { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
}