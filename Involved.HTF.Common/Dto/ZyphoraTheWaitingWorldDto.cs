namespace Involved.HTF.Common.Dto;

public class ZyphoraTheWaitingWorldDto
{
    /// <summary>
    ///     Time on the planet
    /// </summary>
    public DateTime SendDateTime { get; set; }

    /// <summary>
    ///     The speed at which the message travels in lightYears/minute
    /// </summary>
    public int TravelSpeed { get; set; }

    /// <summary>
    ///     The distance between the planet and the base station in light years
    /// </summary>
    public int Distance { get; set; }

    /// <summary>
    ///     The nr of hours in a day on the planet
    /// </summary>
    public int DayLength { get; set; }
}