using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
	public class OxygenTank : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ItemRarityID.Green;
			Item.material = true;

		}

		public override void AddRecipes()
		{
 		}
	}
}