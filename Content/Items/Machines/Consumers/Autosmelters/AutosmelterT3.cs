using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tech;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines.Consumers.Autosmelters;

public class AutosmelterT3 : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.Consumers.Autosmelters.AutosmelterT3>());
        Item.width = 36;
        Item.height = 22;
        Item.value = Item.sellPrice(gold: 1);
        Item.mech = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.LunarBar, 20)
            .AddIngredient<SteelBar>(10)
            .AddIngredient<Plastic>(7)
            .AddIngredient<AdvancedCircuitBoard>(10)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile<Fabricator>()
            .Register();
    }
}

