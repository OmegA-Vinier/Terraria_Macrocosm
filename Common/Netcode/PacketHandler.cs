﻿using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.Netcode
{
    public enum MessageType : byte
    {
        SyncParticle,
        SyncRocketData,
        SyncRocketCustomizationData,
        SyncLaunchPadData,
        SyncInventory,
        SyncRocketPlayer,
        SyncDashPlayer,
        SyncMacrocosmPlayer,
        RequestLastSubworld,
        LastSubworldCheck,
        SyncTEFromClient,
        SyncPowerWire
    }

    public class PacketHandler
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType messageType = (MessageType)reader.ReadByte();

            DebugPackets(messageType, reader.BaseStream.Length, whoAmI);

            switch (messageType)
            {
                case MessageType.SyncParticle:
                    Particle.ReceiveSyncParticle(reader, whoAmI);
                    break;

                case MessageType.SyncRocketData:
                    Rocket.ReceiveSyncRocketData(reader, whoAmI);
                    break;

                case MessageType.SyncInventory:
                    Inventory.HandlePacket(reader, whoAmI);
                    break;

                case MessageType.SyncLaunchPadData:
                    LaunchPad.ReceiveSyncLaunchPadData(reader, whoAmI);
                    break;

                case MessageType.SyncRocketPlayer:
                    RocketPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.SyncDashPlayer:
                    DashPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.SyncMacrocosmPlayer:
                    MacrocosmPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.LastSubworldCheck:
                    SubworldTravelPlayer.ReceiveLastSubworldCheck(reader, whoAmI);
                    break;

                case MessageType.SyncTEFromClient:
                    TENetHelper.ReceiveSyncTEFromClient(reader, whoAmI);
                    break;

                case MessageType.SyncPowerWire:
                    PowerWiring.ReceiveSyncPowerWire(reader, whoAmI);
                    break;

                default:
                    Macrocosm.Instance.Logger.WarnFormat("Macrocosm: Unknown Message type: {0}", messageType);
                    break;
            }
        }

        public static bool DebugModeActive { get; set; }

        static private void DebugPackets(MessageType messageType, long length, int sender)
        {
            string message = $"Received message of type {messageType} of length {length} from {sender}";

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText(message);
                Macrocosm.Instance.Logger.Info(message);
            }

            if (Main.netMode == NetmodeID.Server)
            {
                if (Main.dedServ) Console.WriteLine(message);
                Macrocosm.Instance.Logger.Info(message);
            }
        }
    }
}
