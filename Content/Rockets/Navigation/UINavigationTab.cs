﻿using Macrocosm.Common.Config;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Rockets.Navigation.CrewPanel;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Macrocosm.Content.Rockets.Navigation.NavigationPanel;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UINavigationTab : UIPanel, ITabUIElement, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; }

        public UICustomizationPreview CustomizationPreview { get; set; }

        private UILaunchButton launchButton;
        private UINavigationPanel navigationPanel;

        private UIFlightChecklist flightChecklist;
        private UICrewPanel crewPanel;
        private UIListScrollablePanel worldInfoPanel;
        private UIListScrollablePanel launchLocationsList;

        private UIMapTarget lastTarget;
        private UIMapTarget target;

        private LaunchPad targetLaunchPad;

        public UINavigationTab()
        {
            MacrocosmConfig.Instance.OnConfigChanged += (_, _) => 
            {
                if (target is not null)
					CreateWorldInfoPanel(target.Name);
			};
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(3f);

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

            navigationPanel = new();
            Append(navigationPanel);
            navigationPanel.Activate();

            flightChecklist = CreateFlightChecklist();
            Append(flightChecklist);
            flightChecklist.Activate();

            crewPanel = CreateCrewPanel();
            Append(crewPanel);
            crewPanel.Activate();

            worldInfoPanel = CreateWorldInfoPanel(MacrocosmSubworld.CurrentPlanet);
            Append(worldInfoPanel);
            worldInfoPanel.Activate();

            launchLocationsList = CreateLaunchLocationPanel(MacrocosmSubworld.CurrentPlanet);
            Append(launchLocationsList);
            launchLocationsList.Activate();

			launchButton = new();
            launchButton.ZoomIn = navigationPanel.ZoomIn;
            launchButton.Launch = () => Rocket.Launch(target.Name, targetLaunchPad);
            Append(launchButton);
            launchButton.Activate();

            CustomizationPreview = new();
            Append(CustomizationPreview);
            CustomizationPreview.Activate();
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();
			Main.LocalPlayer.RocketPlayer().TargetSubworldID = target is null ? "" : target.Name;

			base.Update(gameTime);

            UpdateWorldInfoPanel();
            UpdateLaunchLocationsList();
			UpdateChecklist();
            UpdateLaunchButton();
        }

        private void UpdateWorldInfoPanel()
        {
			if (target is not null && target != lastTarget)
			{
				CreateWorldInfoPanel(target.Name);
				launchLocationsList.ClearList();
			}
		}

        private void UpdateLaunchLocationsList()
        {
			if (target is not null)
			{
				CreateLaunchLocationPanel(target.Name);
				launchLocationsList.UpdateOrder();

				foreach (var lpInfo in launchLocationsList.OfType<UILaunchPadInfoElement>())
                {
                    if (lpInfo.HasFocus && lpInfo.LaunchPad is not null)
                    {
                        targetLaunchPad = lpInfo.LaunchPad;
                        break;
                    }
                }
			}
		}

		private void UpdateChecklist()
        {
            flightChecklist.MapTarget = target;
        }

        private void UpdateLaunchButton()
        {
            if (target is null)
                launchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (navigationPanel.CurrentMap.HasNext)
                launchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (!flightChecklist.CheckLaunchConditions())
                launchButton.ButtonState = UILaunchButton.StateType.CantReach;
            else if (Main.LocalPlayer.GetModPlayer<RocketPlayer>().IsCommander)
                launchButton.ButtonState = UILaunchButton.StateType.Launch;
            else
                launchButton.ButtonState = UILaunchButton.StateType.LaunchInactive;
        }

		private UIFlightChecklist CreateFlightChecklist()
        {
            flightChecklist = new UIFlightChecklist
            {
                Top = new(0, 0.365f),
                Width = new(0f, 0.31f),
                Left = new(0, 0.68f),
                Height = new(0, 0.45f)
            };
            flightChecklist.SetPadding(2f);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                flightChecklist.Height.Percent = 0.28f;
            }

			return flightChecklist;
        }

        private UICrewPanel CreateCrewPanel()
        {
            crewPanel = new UICrewPanel
            {
                Top = new(0f, 0.835f),
                Width = new(0f, 0.31f),
                Left = new(0, 0.68f),
                Height = new(0f, 0.15f),
		    };
            crewPanel.SetPadding(2f);

			if (Main.netMode == NetmodeID.MultiplayerClient)
            {
				crewPanel.Height.Percent = 0.322f;
				crewPanel.Top.Percent = 0.66f;
            }

			return crewPanel;
        }

		private UIListScrollablePanel CreateWorldInfoPanel(string subworldId)
		{
            if(worldInfoPanel is null)
            {
				worldInfoPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f))
				{
					Width = new(0, 0.31f),
					Height = new(0, 0.62f),
					Left = new(0, 0.01f),
					Top = new(0, 0.365f),
					BackgroundColor = new Color(53, 72, 135),
					BorderColor = new Color(89, 116, 213, 255)
				};
				worldInfoPanel.SetPadding(0f);
			}
            else
            {
				worldInfoPanel.Deactivate();
				worldInfoPanel.ClearList();
                worldInfoPanel.SetTitle(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f));
			}
			
			LocalizedText flavorText = Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.Subworlds." + subworldId + ".FlavorText");
			if (flavorText != LocalizedText.Empty && flavorText.Value != "default")
			{
				worldInfoPanel.Add(new UIDynamicTextPanel(new LocalizedColorScaleText(flavorText, Color.White, scale: 0.85f)));
				worldInfoPanel.AddHorizontalSeparator();
			}

			List<InfoElement> elements = WorldInfo.GetInfoElements(subworldId);

			if (elements is not null)
			{
				bool foundHazards = false;
				foreach (InfoElement element in elements)
				{
					if (!foundHazards && element is HazardInfoElement)
					{
						worldInfoPanel.AddHorizontalSeparator();
						foundHazards = true;
					}

					worldInfoPanel.Add(element.ProvideUI());
				}
			}

            worldInfoPanel.Activate();
			return worldInfoPanel;
		}

		private UIListScrollablePanel CreateLaunchLocationPanel(string subworldId)
		{
            if(launchLocationsList is null)
            {
				launchLocationsList = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.LaunchLocations"), scale: 1.2f))
				{
					Top = new(0, 0.365f),
					Width = new(0f, 0.34f),
					HAlign = 0.5f,
					Height = new(0, 0.505f),
					BorderColor = new Color(89, 116, 213, 255),
					BackgroundColor = new Color(53, 72, 135)
				};
				launchLocationsList.SetPadding(0f);

                // Sort by vacant > current > occupied
                launchLocationsList.ManualSortMethod = (list) =>
                {
                    list.Sort((a, b) =>
                    {
                        bool HasRocket(object element) => element is UILaunchPadInfoElement infoElement && !infoElement.IsSpawnPointDefault && infoElement.LaunchPad.HasRocket;
						bool IsCurrent(object element) => element is UILaunchPadInfoElement infoElement && infoElement.IsCurrent;

						bool aHasRocket = HasRocket(a);
						bool bHasRocket = HasRocket(b);

						bool aIsCurrent = IsCurrent(a);
						bool bIsCurrent = IsCurrent(b);

						if (!aHasRocket && !bHasRocket) return 0;  
						if (!aHasRocket) return -1;                
						if (!bHasRocket) return 1;                 
                                                                   
						if (aIsCurrent && bIsCurrent) return 0;    
						if (aIsCurrent) return -1;                 
						if (bIsCurrent) return 1;                  
                                                                   
						if (aHasRocket && bHasRocket) return 0;    
                                                                   
						return 0;                                  
					});
				};
			}

            // Add the launchpads
			foreach (var launchPad in LaunchPadManager.GetLaunchPads(subworldId))
            {
                var storedInfoElement = launchLocationsList.OfType<UILaunchPadInfoElement>()
                                                           .FirstOrDefault(e => e.LaunchPad == launchPad);

                bool notFound = storedInfoElement is null;

                // Add any newly created launchpads
				if (notFound)
                {
					UILaunchPadInfoElement infoElement = new(launchPad)
					{
						FocusContext = "LaunchLocations",
					};
					infoElement.OnLeftClick += InfoElement_OnLeftClick;

                    launchLocationsList.Add(infoElement);
  				}
                else
                {
					storedInfoElement.IsCurrent = false;

                    // Mark the current launchpad of this rocket 
					if (Rocket is not null && launchPad.RocketID == Rocket.WhoAmI)
						storedInfoElement.IsCurrent = true;
				}
			}

			// Remove inactive launchpads
			launchLocationsList.OfType<UILaunchPadInfoElement>()
                               .Where(e => !e.IsSpawnPointDefault && !e.LaunchPad.Active)
                               .ToList()
                               .ForEach(launchLocationsList.RemoveFromList);


			// Add the "Spawn Point" launch location if no vacant launchpads were found
			if (!launchLocationsList.OfType<UILaunchPadInfoElement>().Any(e => e.IsSpawnPointDefault || (!e.IsSpawnPointDefault && !e.LaunchPad.HasRocket)))
			{
				UILaunchPadInfoElement spawnInfoElement = new()
				{
					FocusContext = "LaunchLocations",
				};
				spawnInfoElement.OnLeftClick += InfoElement_OnLeftClick;
				launchLocationsList.Add(spawnInfoElement);
			}

            launchLocationsList.Activate();
			return launchLocationsList;
		}


		private void InfoElement_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
            if(listeningElement is UILaunchPadInfoElement infoElement)
                 if (infoElement.IsSpawnPointDefault || !infoElement.LaunchPad.HasRocket)
 					infoElement.HasFocus = true;
		}

        private void HandleSeparators()
        {
			//Remove all separators 
			launchLocationsList.OfType<UIHorizontalSeparator>()
							   .ToList()
							   .ForEach(launchLocationsList.RemoveFromList);

			// Insert separator after vacant list
			var lastVacantElement = launchLocationsList.OfType<UILaunchPadInfoElement>()
													   .LastOrDefault(e => e.IsSpawnPointDefault || (!e.IsSpawnPointDefault && !e.LaunchPad.HasRocket));

			if (lastVacantElement != null)
			{
				int lastVacantIndex = launchLocationsList.ToList().IndexOf(lastVacantElement);
				bool hasCurrentOrOccupiedAfterLastVacant = launchLocationsList.OfType<UILaunchPadInfoElement>()
																			  .Any(e => e.IsCurrent || (!e.IsSpawnPointDefault && e.LaunchPad.HasRocket));


				if (hasCurrentOrOccupiedAfterLastVacant)
					launchLocationsList.InsertHorizontalSeparator(lastVacantIndex + 1);
			}


			// Insert separator after the current launchpad
			var currentElement = launchLocationsList.OfType<UILaunchPadInfoElement>()
													.FirstOrDefault(e => e.IsCurrent);

			if (currentElement != null)
			{
				int currentIndex = launchLocationsList.ToList().IndexOf(currentElement);
				bool hasOtherOccupiedLaunchPads = launchLocationsList.OfType<UILaunchPadInfoElement>()
																	 .Any(e => !e.IsCurrent && !e.IsSpawnPointDefault && e.LaunchPad.HasRocket);

				if (hasOtherOccupiedLaunchPads)
					launchLocationsList.InsertHorizontalSeparator(currentIndex + 1);
			}
		}
	}
}
