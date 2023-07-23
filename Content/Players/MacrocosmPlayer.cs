﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Systems;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Terraria.Localization;
using Macrocosm.Content.Biomes;

namespace Macrocosm.Content.Players
{
	public enum SpaceProtection
	{
		None,
		Tier1,
		Tier2,
		Tier3
	}

    public class MacrocosmPlayer : ModPlayer
	{
		public SpaceProtection SpaceProtection = SpaceProtection.None;

		public float RadNoiseIntensity = 0f;

		public int ChandriumEmpowermentStacks = 0;

		/// <summary> Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance </summary>
        public float ChanceToNotConsumeAmmo 
		{ 
			get => chanceToNotConsumeAmmo; 
			set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
		}
        private float chanceToNotConsumeAmmo = 0f;

		/// <summary> Set or add to current screenshake </summary>
		public float ScreenShakeIntensity
		{
			get => screenShakeIntensity;
			set => screenShakeIntensity = MathHelper.Clamp(value, 0, 100);
		}
        private float screenShakeIntensity = 0f;

        public override void ResetEffects()
		{
			SpaceProtection = SpaceProtection.None;
			RadNoiseIntensity = 0f;
			ChanceToNotConsumeAmmo = 0f;
			Player.buffImmune[BuffType<Depressurized>()] = false;
		}

		public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
			bool consumeAmmo = true;

			if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
				consumeAmmo = false;

			return consumeAmmo;
        }

        public override void PostUpdateBuffs()
		{
			if (MacrocosmSubworld.AnyActive)
            {
				UpdateSpaceEnvironmentalDebuffs();
            }
		}

		public void UpdateSpaceEnvironmentalDebuffs()
        {
			if (!Player.RocketPlayer().InRocket)
			{
				if (SubworldSystem.IsActive<Moon>())
				{
					if (SpaceProtection == SpaceProtection.None)
						Player.AddBuff(BuffType<Depressurized>(), 2);
     				//if (protectTier <= SpaceProtection.Tier1)
     					//Player.AddBuff(BuffTpye<Irradiated>(), 2);
     			}
     			//else if (SubworldSystem.IsActive<Mars>())
			}
		}

		public override void PostUpdateMiscEffects()
		{
			UpdateGravity();
			UpdateFilterEffects();
		}

		public override void ModifyScreenPosition()
		{
			if (ScreenShakeIntensity > 0.1f)
			{
				Main.screenPosition += new Vector2(Main.rand.NextFloat(ScreenShakeIntensity), Main.rand.NextFloat(ScreenShakeIntensity));
				ScreenShakeIntensity *= 0.9f;
			}
		}

		public override void PostUpdateEquips()
		{
			UpdateSpaceArmourImmunities();
			if (Player.Macrocosm().SpaceProtection > SpaceProtection.None)
				Player.setBonus = Language.GetTextValue("Mods.Macrocosm.SetBonuses.SpaceProtection_" + SpaceProtection.ToString());
		}

		public void UpdateSpaceArmourImmunities()
        {
			if (SpaceProtection > SpaceProtection.None)
				Player.buffImmune[BuffType<Depressurized>()] = true;
			if (SpaceProtection > SpaceProtection.Tier1)
            {

            }
		}

		private void UpdateGravity()
		{
			if (MacrocosmSubworld.AnyActive)
				Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current.GravityMultiplier;
		}

		private void UpdateFilterEffects()
		{
			if (Player.InModBiome<IrradiationBiome>())
			{
				if (!Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Activate("Macrocosm:RadiationNoiseEffect");

				RadNoiseIntensity += 0.189f * Utility.InverseLerp(400, 10000, TileCounts.Instance.IrradiatedRockCount, clamped: true);

				Filters.Scene["Macrocosm:RadiationNoiseEffect"].GetShader().UseIntensity(RadNoiseIntensity);
			}
			else
			{
				if (Filters.Scene["Macrocosm:RadiationNoiseEffect"].IsActive())
					Filters.Scene.Deactivate("Macrocosm:RadiationNoiseEffect");
			}
		}
	}
}
