using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters;

public class AutosmelterT1 : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT1>());
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GrayBrick, 10)
            .AddIngredient<SteelBar>(6)
            .AddIngredient<Gear>(2)
            .AddIngredient<PrintedCircuitBoard>()
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
