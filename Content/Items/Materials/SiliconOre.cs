﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
	public class SiliconOre : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 750;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			//Item.createTile = TileType<Tiles.SiliconOre>();
			Item.placeStyle = 0;
			Item.rare = ItemRarityID.White;
			Item.material = true;

			// Set other Item.X values here
		}

		public override void AddRecipes()
		{

		}
	}
}