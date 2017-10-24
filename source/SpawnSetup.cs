using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;
using Harmony;

namespace WM.TooManyLeathers
{
	[HarmonyPatch(typeof(Verse.Thing), "SpawnSetup")]
	public static class SpawnSetup
	{
		[HarmonyPostfix]
		public static void Postfix(Thing __instance)
		{
			try
			{
				if (__instance is Thing)
				{
					var leather = __instance as Thing;

					if (Utils.IsDiscardedDef(leather.def))
					{
#if DEBUG
						Log.Message("Converted leather at spawn: " + leather);
#endif
						var newthing = ThingMaker.MakeThing(Utils.ConvertDef(leather.def));

						SwapThingsSpawn(leather, newthing);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("SpawnSetup of " + __instance + " failed. Reason: " + ex.Message + "\n" + ex.StackTrace);
			}
		}

		static void SwapThingsSpawn(Thing oldt, Thing newt)
		{
			newt.Position = oldt.Position;
			newt.HitPoints = oldt.HitPoints;
			newt.stackCount = oldt.stackCount;

			if (newt.def.CanHaveFaction)
				newt.SetFaction(oldt.Faction);

			QualityCategory quality;
			if (oldt.TryGetQuality(out quality))
			{
				//TODO: art ?
				var compQuality = newt.TryGetComp<CompQuality>();
				if (compQuality != null)
					compQuality.SetQuality(quality, ArtGenerationContext.Colony);
			}

			newt.SpawnSetup(oldt.Map);

			oldt.Destroy();
		}
	}
}
