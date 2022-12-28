﻿using Macrocosm.Common.Global.GlobalItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Common.Utility
{
	public static class ItemUtils
	{

		/// <summary> Returns whether this item instance is a torch </summary>
		public static bool IsTorch(this Item item) => TorchGlobalItem.IsTorch(item);

		/// <summary>
		/// Helper method that converts the first rocket ammo found in the inventory 
		/// to the projectile ID generated by rocket ammo of a vanilla launcher 
		/// (either Grenade, Rocket or Mine)
		/// </summary>
		/// <param name="player"> The player using the picked rocket ammo </param>
		/// <param name="copyWeaponType"> The launcher type to copy </param>
		/// <returns> The projectile ID, defaults to Rocket I </returns>
		public static int ToRocketProjectileID(Player player, int copyWeaponType)
		{

			if (copyWeaponType != ItemID.GrenadeLauncher && copyWeaponType != ItemID.RocketLauncher && copyWeaponType != ItemID.ProximityMineLauncher)
				return ProjectileID.RocketI;

			Item launcher = new(copyWeaponType);
			Item ammo = player.ChooseAmmo(launcher);

			if (ammo is null)
				return 0;

			int type;

			// for mini nukes, liquid rockets
			if (TryFindingSpecificMatches(copyWeaponType, ammo.type, out int pickedProjectileId))
			{
				type = pickedProjectileId;
			}
			// for rockets I to IV
			else if (ammo.ammo == AmmoID.Rocket)
			{
				type = launcher.shoot + ammo.shoot;
			}
			else
			{
				type = ProjectileID.RocketI;
			}

			return type;

		}

		public static void DrawBossBagEffect(this Item item, SpriteBatch spriteBatch, Color colorFront, Color colorBack, float rotation, float scale)
		{
			Texture2D texture = TextureAssets.Item[item.type].Value;
			Rectangle frame = texture.Frame();

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(item.width / 2 - frameOrigin.X, item.height - frame.Height);
			Vector2 drawPos = item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
				time = 2f - time;

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, colorFront, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, colorBack, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}
		}

		/// <summary>
		/// Copied private method from vanilla 
		/// </summary>
		private static bool TryFindingSpecificMatches(int launcher, int ammo, out int pickedProjectileId)
		{
			pickedProjectileId = 0;
			return AmmoID.Sets.SpecificLauncherAmmoProjectileMatches.TryGetValue(launcher, out Dictionary<int, int> value) && value.TryGetValue(ammo, out pickedProjectileId);
		}
	}
}
