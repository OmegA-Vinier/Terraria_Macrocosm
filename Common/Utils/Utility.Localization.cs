﻿using Terraria.Localization;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
		public static string GetLanguageValueOrEmpty(string key)
		{
			string value = Language.GetTextValue(key);
			return (value == key) ? "" : value;
		}

	}
}