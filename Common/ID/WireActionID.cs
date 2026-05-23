namespace Macrocosm.Common.ID;

/// <summary>
/// Named constants for the <c>number</c> parameter of
/// <see cref="Terraria.NetMessage.SendData(int, int, int, Terraria.Localization.NetworkText, int, float, float, float, int, int, int)"/>
/// when <c>msgType</c> is <see cref="Terraria.ID.MessageID.TileManipulation"/>.
/// These correspond to the <see cref="Terraria.WorldGen"/> methods dispatched server-side.
/// </summary>
public static class WireActionID
{
    public const int PlaceWireRed    = 5;
    public const int KillWireRed     = 6;
    public const int PlaceWireBlue   = 10;
    public const int KillWireBlue    = 11;
    public const int PlaceWireGreen  = 12;
    public const int KillWireGreen   = 13;
    public const int PlaceWireYellow = 16;
    public const int KillWireYellow  = 17;
}
