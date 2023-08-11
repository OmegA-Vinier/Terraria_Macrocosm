﻿using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public class LaunchPadManager : ModSystem
	{
		private static Dictionary<string, List<LaunchPad>> launchPadStorage;

		public override void Load()
		{
			launchPadStorage = new Dictionary<string, List<LaunchPad>>();
		}

		public override void Unload()
		{
			launchPadStorage.Clear();
			launchPadStorage = null;
		}

		public static void Add(string subworldId, LaunchPad launchPad)
		{
			if (launchPadStorage.ContainsKey(subworldId))
			{
				launchPadStorage[subworldId].Add(launchPad);
			}
			else
			{
				List<LaunchPad> launchPadsList = new() { launchPad };
				launchPadStorage.Add(subworldId, launchPadsList);
			}
		}

		public static void Remove(string subworldId, LaunchPad launchPad)
		{
			if (launchPadStorage.ContainsKey(subworldId))
 				launchPadStorage[subworldId].Remove(launchPad);
 		}

		public static bool Any(string subworldId) => GetLaunchPads(subworldId).Any();
		public static bool AnyNonDefault(string subworldId) => GetLaunchPads(subworldId).Any(lp => !lp.IsDefault);
		public static bool None(string subworldId) => !Any(subworldId);


		public static List<LaunchPad> GetLaunchPads(string subworldId)
		{
			if (launchPadStorage.ContainsKey(subworldId))
				return launchPadStorage[subworldId];

			return new List<LaunchPad>();
		}

		public static LaunchPad GetLaunchPadAtTileCoordinates(string subworldId, Point16 startTile)
			=> GetLaunchPads(subworldId).FirstOrDefault(lp => lp.StartTile == startTile);

		public static LaunchPad GetDefaultLaunchPad(string subworldId)
		{
			List<LaunchPad> launchPads = GetLaunchPads(subworldId);
			return launchPads.FirstOrDefault(lp => lp.IsDefault);
		}

		public static void SetDefaultLaunchPad(string subworldId, LaunchPad launchPad)
		{
			List<LaunchPad> launchPads = GetLaunchPads(subworldId);
			foreach (var lp in launchPads)
 				lp.IsDefault = false;
 
			launchPad.IsDefault = true;

			if (None(subworldId))
			{
				Add(subworldId, launchPad);
			}
			else
			{
				LaunchPad existingDefault = launchPads.FirstOrDefault(lp => lp.IsDefault);
				if (existingDefault != null)
 					existingDefault.IsDefault = false;
 
				int defaultIndex = launchPads.IndexOf(existingDefault);
				if (defaultIndex >= 0)
 					launchPads[defaultIndex] = launchPad;
 			}
		}

		private int checkTimer;

		public override void PostUpdateNPCs()
		{
			checkTimer++;

			if (checkTimer >= 10)
			{
				checkTimer = 0;

				if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentSubworld))
					foreach (LaunchPad launchPad in launchPadStorage[MacrocosmSubworld.CurrentSubworld])
						launchPad.Update();
			}
		}

		public override void PostDrawTiles()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			if (launchPadStorage.ContainsKey(MacrocosmSubworld.CurrentSubworld))
 				foreach (LaunchPad launchPad in launchPadStorage[MacrocosmSubworld.CurrentSubworld])
 					launchPad.Draw(Main.spriteBatch, Main.screenPosition);
 
			Main.spriteBatch.End();
		}

		public override void PostWorldGen()
		{
			SetDefaultLaunchPad("Earth", new LaunchPad(Utility.SpawnTilePoint16));
		}

		public override void ClearWorld()
		{
			launchPadStorage.Clear();
		}

		public override void SaveWorldData(TagCompound tag) => SaveLaunchPads(tag);

		public override void LoadWorldData(TagCompound tag) => LoadLaunchPads(tag);
			
		public static void SaveLaunchPads(TagCompound tag)
		{
			foreach (var lpKvp in launchPadStorage)
				tag[lpKvp.Key] = lpKvp.Value;
		}

		public static void LoadLaunchPads(TagCompound tag)
		{
			foreach (var lpKvp in launchPadStorage)
				if (tag.ContainsKey(lpKvp.Key))
					launchPadStorage[lpKvp.Key] = (List<LaunchPad>)tag.GetList<LaunchPad>(lpKvp.Key);

			SetDefaultLaunchPad(MacrocosmSubworld.CurrentSubworld, new LaunchPad(Utility.SpawnTilePoint16));
 		}
	}
}
