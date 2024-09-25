﻿using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class WireCutterGlobalItem : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WireCutter;

        public override bool? UseItem(Item item, Player player)
        {
            Tile targetTile = player.TargetTile();
            Point targetCoords = player.TargetCoords();
            if (targetTile.AnyWire() || targetTile.HasActuator)
                return null;

            if (CustomWiring.Map[targetCoords].AnyWire)
            {
                CustomWiring.CutWire(targetCoords);
                return true;
            }

            return null;
        }
    }
}