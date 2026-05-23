using Macrocosm.Common.ID;
using System;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Content.Items.Connectors;

public class CrudeWireWrenchRed : CrudeWireWrench
{
    protected override Func<int, int, bool> PlaceWireFunc => WorldGen.PlaceWire;
    protected override int NetAction => WireActionID.PlaceWireRed;

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TinBar, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
