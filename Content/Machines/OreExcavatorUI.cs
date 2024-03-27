﻿using Macrocosm.Common.Bases.Machines;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace Macrocosm.Content.Machines
{
    internal class OreExcavatorUI : MachineUI
    {
        public OreExcavatorTE OreExcavator => MachineTE as OreExcavatorTE;

        private UIPanel inventoryPanel;
        private UIListScrollablePanel dropRateList;

        public OreExcavatorUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(745f, 0f);
            Height.Set(394f, 0f);

            Recalculate();

            if(OreExcavator.Inventory is not null)
            {
                inventoryPanel = OreExcavator.Inventory.ProvideUI(iconsPerRow: 10, rowsWithoutScrollbar: 5, buttonMenuTopPercent: 0.765f);
                inventoryPanel.Width = new(0, 0.69f);
                inventoryPanel.BorderColor = UITheme.Current.ButtonStyle.BorderColor;
                inventoryPanel.BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
                inventoryPanel.Activate();
                Append(inventoryPanel);
            }

            dropRateList = CreateDroprateList();
            Append(dropRateList);
        }

        private UIListScrollablePanel CreateDroprateList()
        {
            dropRateList = new("Loot")
            {
                Width = new(0, 0.306f),
                Height = new(0, 1f),
                HAlign = 1f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                PaddingTop = 0f,
                PaddingLeft = 2f,
                PaddingRight = 2f
            };

            List<DropRateInfo> dropRates = [];
            DropRateInfoChainFeed ratesInfo = new(1f);
            foreach (var drop in OreExcavator.Loot.Get())
                drop.ReportDroprates(dropRates, ratesInfo);

            foreach (DropRateInfo info in dropRates)
            {
                UIBestiaryInfoItemLine infoElement = new(info, new() { UnlockState = BestiaryEntryUnlockState.CanShowDropsWithDropRates_4 })
                {
                    Left = new(0,0),
                    Width = new(0, 1f),
                    BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor,
                    BorderColor = UITheme.Current.InfoElementStyle.BorderColor
                };
                
                dropRateList.Add(infoElement);
            }

            return dropRateList;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
