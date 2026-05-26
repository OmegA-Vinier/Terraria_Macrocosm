using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Tech;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters;

public class AutosmelterT2Adamantite : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT2>(), 0);
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AdamantiteBar, 15)
            .AddIngredient<SteelBar>(10)
            .AddIngredient<Gear>(4)
            .AddIngredient<AdvancedCircuitBoard>(5)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile<Fabricator>()
            .Register();
    }
}

