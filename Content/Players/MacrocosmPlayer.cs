﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using SubworldLibrary;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Systems;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Terraria.Localization;

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
		public int AccMoonArmorDebuff = 0;

		public bool ZoneMoon = false;
		public bool ZoneBasalt = false;
		public bool ZoneIrradiation = false;

		public float RadNoiseIntensity = 0f;

		public int ChandriumEmpowermentStacks = 0;

		/// <summary> Chance to not consume ammo from equipment and weapons, stacks additively </summary>
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
			if (SubworldSystem.IsActive<Moon>())
			{
				if (SpaceProtection > SpaceProtection.None)
					Player.AddBuff(ModContent.BuffType<SuitBreach>(), 2);
			}
		}

		public override void PostUpdateMiscEffects()
		{
			UpdateGravity();
			UpdateFilterEffects();

			if (AccMoonArmorDebuff > 0)
			{
				Player.buffImmune[ModContent.BuffType<SuitBreach>()] = false;
				AccMoonArmorDebuff--;
			}
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
			if (Player.Macrocosm().SpaceProtection > SpaceProtection.None)
				Player.setBonus = Language.GetTextValue("Mods.Macrocosm.SetBonuses.SpaceProtection_" + SpaceProtection.ToString());
		}

		private void UpdateGravity()
		{
			if (MacrocosmSubworld.AnyActive)
				Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current.GravityMultiplier;
		}

		private void UpdateFilterEffects()
		{
			if (ZoneIrradiation)
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
