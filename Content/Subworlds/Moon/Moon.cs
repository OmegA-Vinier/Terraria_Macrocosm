﻿using Macrocosm.Content.WorldGeneration.Moon;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Map;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds.Moon
{
	/// <summary>
	/// Moon terrain and crater generation by 4mbr0s3 2
	/// Why isn't anyone else working on this
	/// I have saved the day - Ryan
	/// </summary>
	public class Moon : MacrocosmSubworld
	{
		/// <summary>  8 times slower than on Earth (a Terrarian lunar month lasts for 8 in-game days) </summary>
		public override double TimeRate => 0.125;
		/// <summary> About 6 times lower than default (1, as on Earth) </summary>
		public override float GravityMultiplier => 0.166f; 

		public override bool NormalUpdates => false;
		public override int Width => 4200;
		public override int Height => 1200;
		public override bool ShouldSave => true;
		public override bool NoPlayerSaving => false;
		public override List<GenPass> Tasks => new()
		{
			genGroundPass,
			new CraterPass("CraterPass", 1f),
 			new RegolithPass("RegolithPass", 5f),
			new OrePass("OrePass", 0.75f),
			new CavePass("CavePass", 1f, genGroundPass.RockLayerHigh, genGroundPass.RockLayerHigh),
			new IrradiationPass("IrradiationPass", 3f),
			new ScuffedSmoothPass("ScuffedSmoothPass", 1f),
			new AmbientPass("AmbientPass", 0.2f),
			new FinishPass("FinishPass", 0.1f)
		};

		public override Dictionary<MapColorType, Color> MapColors => new()
		{
			{MapColorType.SkyUpper, new Color(10, 10, 10)},
			{MapColorType.SkyLower, new Color(40, 40, 40)},
			{MapColorType.UndergroundUpper, new Color(40, 40, 40)},
			{MapColorType.UndergroundLower, new Color(30, 30, 30)},
 			{MapColorType.CavernUpper, new Color(30, 30, 30)},
 			{MapColorType.CavernLower, new Color(30, 30, 30)},
 			{MapColorType.Underworld,  new Color(30, 30, 30)}
		};


		private MoonSubworldLoadUI moonLoadUI;
		private GroundPass genGroundPass;

		public Moon()
		{
			this.moonLoadUI = new();
			this.genGroundPass = new GroundPass("GroundPass", 8f, Width, Height);
		}

		public override void OnEnter()
		{
			moonLoadUI.Setup(toEarth: false);
			SkyManager.Instance.Activate("Macrocosm:MoonSky");
 		}

		public override void OnExit()
		{
			moonLoadUI.Setup(toEarth: true);
			SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
 		}

		public override void Load()
		{
			SubworldSystem.hideUnderworld = true;
			SubworldSystem.noReturn = true;
			Main.numClouds = 0;
			Main.raining = false;
		}

		public override void DrawSetup(GameTime gameTime)
		{
			PlayerInput.SetZoom_Unscaled();
			Main.instance.GraphicsDevice.Clear(Color.Black);
			DrawMenu(gameTime);
		}

		public override void DrawMenu(GameTime gameTime)
		{
			moonLoadUI.DrawSelf(Main.spriteBatch);
		}
	}
}
