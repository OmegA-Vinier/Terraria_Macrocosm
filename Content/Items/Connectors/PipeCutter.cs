using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

/// <summary> Dedicated tool for removing conveyor pipes and attachments. </summary>
public class PipeCutter : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.mech = true;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override bool CanUseItem(Player player) => player.ItemInTileReach(Item) && player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY);

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        return ConveyorSystem.Remove(player.TargetCoords());
    }
}
