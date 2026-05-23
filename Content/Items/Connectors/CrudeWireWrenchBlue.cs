using Macrocosm.Common.ID;
using System;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Content.Items.Connectors;

public class CrudeWireWrenchBlue : CrudeWireWrench
{
    protected override Func<int, int, bool> PlaceWireFunc => WorldGen.PlaceWire2;
    protected override int NetAction => WireActionID.PlaceWireBlue;

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CopperBar, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
