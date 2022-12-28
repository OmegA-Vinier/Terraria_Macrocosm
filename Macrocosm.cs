using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rocket;

namespace Macrocosm
{
	public enum MessageType : byte
	{
		SyncPlayerDashDirection,
		SyncPlayerRocketStatus,
		EmbarkPlayerInRocket,
		LaunchRocket
	}

	public class Macrocosm : Mod
	{
		public static Mod Instance => ModContent.GetInstance<Macrocosm>();

		public const string EffectAssetPath = "Macrocosm/Assets/Effects/";
		public const string EmptyTexPath = "Macrocosm/Assets/Textures/Empty";
		public static Texture2D EmptyTex => ModContent.Request<Texture2D>(EmptyTexPath).Value;

		public override void Load()
		{
			ModCalls();
			LoadEffects();
		}

		private static void LoadEffects()
		{
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
			
			Filters.Scene["Macrocosm:RadiationNoiseEffect"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>(EffectAssetPath + "RadiationNoiseEffect", mode).Value), "RadiationNoiseEffect"));
			Filters.Scene["Macrocosm:RadiationNoiseEffect"].Load();
		}

		private void ModCalls()
		{
			#region Ryan's mods calls

			if (ModLoader.TryGetMod("TerrariaAmbience", out Mod ta))
				ta.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[]
				{
					ModContent.TileType<Regolith>(),
					ModContent.TileType<Protolith>()
				});

			if (ModLoader.TryGetMod("TerrariaAmbienceAPI", out Mod taAPI))
				taAPI.Call("Ambience", this, "MoonAmbience", "Assets/Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));

			#endregion
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType messageType = (MessageType)reader.ReadByte();

			switch (messageType)
			{
				case MessageType.SyncPlayerDashDirection:
 						DashPlayer.ReceiveSyncPlayer(reader, whoAmI);
						break;
 
				case MessageType.SyncPlayerRocketStatus:
 						RocketPlayer.ReceiveSyncPlayer(reader, whoAmI);
						break;
 
				case MessageType.EmbarkPlayerInRocket:
 						RocketNPC.ReceiveEmbarkedPlayer(reader, whoAmI);
						break;
 
				case MessageType.LaunchRocket:
 						RocketNPC.ReceiveLaunchMessage(reader, whoAmI);
						break;
 
				default:
					Logger.WarnFormat("Macrocosm: Unknown Message type: {messageType}", messageType);
					break;
			}
		}
	}
}