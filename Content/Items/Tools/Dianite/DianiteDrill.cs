﻿using Macrocosm.Content.Projectiles.Friendly.Tools;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Dianite
{
	public class DianiteDrill : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 55;
			Item.DamageType = DamageClass.Melee;
			Item.width = 42;
			Item.height = 24;
			Item.useTime = 2;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.channel = true;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.UseSound = SoundID.Item23;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.pick = 235;
			Item.tileBoost = 3;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<DianiteDrillProjectile>();
			Item.shootSpeed = 32;
		}
	}
}