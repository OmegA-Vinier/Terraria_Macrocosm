﻿
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket 
	{
		public const int MaxRockets = byte.MaxValue;

		public const int DefaultWidth = 124;
		public const int DefaultHeight = 584;

		public static Vector2 DefaultSize => new(DefaultWidth, DefaultHeight);

		/// <summary> List of the module names, in the customization access order </summary>
		public static readonly List<string> ModuleNames = new() { "CommandPod", "ServiceModule", "ReactorModule", "EngineModule", "BoosterLeft", "BoosterRight" };

		public static Rocket Create(Vector2 position)
		{
			// Rocket will not be managed.. we have to avoid ever reaching this  
			if (RocketManager.Rockets.Count > RocketManager.MaxRockets)
 				throw new System.Exception("Max rockets reached. Should not ever reach this point during normal gameplay.");
 
			Rocket rocket = new()
			{
				Position = position,
				Active = true
			};

			RocketManager.Rockets.Add(rocket);
			rocket.NetSync();
			rocket.OnSpawn();
			return rocket;
		}
	}
}
