﻿using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Items.Miscellaneous;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Meteors
{
	public class FallingMeteorMedium : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.damage = 500;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
		}

		public override void Kill(int timeLeft)
		{

			// handled by server 
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{

				#region Spawn Items

				if (Main.rand.NextBool(3))
				{
					//int type = Utils.SelectRandom<int>(Main.rand, SomeGeode, SomeOtherGeode); -- maybe WeigthedRandom?
					int type = ModContent.ItemType<MeteoricChunk>();
					Vector2 position = new Vector2(Projectile.position.X, Projectile.position.Y - Projectile.height);
					int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
				}

				#endregion

				#region Screenshake effect

				// let the server do it for every player

				float maxDist = 110f * 16f; // 110 tiles max distance (least screenshake) 
				float maxScreenshake = 75f; // max screenshake (up to 100) for distance = 0

				for (int i = 0; i < 255; i++)
				{

					Player player = Main.player[i];

					if (player.active)
					{
						float distance = Vector2.Distance(player.Center, Projectile.Center);

						if (distance < maxDist)
							player.GetModPlayer<MacrocosmPlayer>().ScreenShakeIntensity += maxScreenshake - distance / maxDist * maxScreenshake;
					}
				}

				#endregion
			}

			// handled by clients 
			if (Main.netMode != NetmodeID.Server)
			{

				#region Dusts

				for (int i = 0; i < Main.rand.Next(100, 120); i++)
				{
					Dust dust = Dust.NewDustDirect(
						new Vector2(Projectile.position.X, Projectile.position.Y + 1.25f * Projectile.height),
						Projectile.width,
						Projectile.height,
						ModContent.DustType<RegolithDust>(),
						Main.rand.NextFloat(-2f, 2f),
						Main.rand.NextFloat(0f, -7.5f),
						Scale: Main.rand.NextFloat(1f, 1.4f)
					);

					dust.noGravity = false;
				}

				#endregion

				#region Gores

				for (int i = 0; i < Main.rand.Next(4,6); i++)
				{
					Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.5f, -Projectile.velocity.Y * 1.2f) * Main.rand.NextFloat(0.5f, 1f), ModContent.GoreType<RegolithDebris>());
				}

				#endregion

				#region Sounds

				#endregion
			}
		}

		override public void AI()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(
						new Vector2(Projectile.position.X, Projectile.position.Y),
						Projectile.width,
						Projectile.height,
						ModContent.DustType<RegolithDust>(),
						0f,
						0f,
						Scale: Main.rand.NextFloat(1f, 1.4f)
					);

				dust.noGravity = true;
			}
		}
	}
}
