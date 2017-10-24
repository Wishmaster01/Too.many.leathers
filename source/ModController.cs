using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.TooManyLeathers
{
	internal static class Log
	{
		internal static void Message(string text)
		{
			Config.Log.Message(text);
		}
		internal static void Warning(string text)
		{
			Config.Log.Warning(text);
		}
		internal static void Error(string text)
		{
			Config.Log.Error(text);
		}
	}

	public class Config : HugsLib.ModBase
	{
		// ducktapestan
		private static Config running;

		static String modname = "WM_Too_Many_Leathers";

		public override String ModIdentifier
		{
			get
			{
				return modname;
			}
		}

		public override void Initialize()
		{
			//ducktapeistan
			running = this;
			//var dialog = new Dialog_MessageBox("UpdateWorldItemsDialogText", "Update items", null, "Do nothing", null);

			//if (Find.WindowStack != null && dialog != null)
		}

		public override void DefsLoaded()
		{
			//var commandSetting = Settings.GetHandle<bool>("commandUpdateWorldItems", "UpdateOldLeathersCommandTitle".Translate(), "UpdateOldLeathersCommandDesc".Translate());

			//commandSetting.VisibilityPredicate = () => Find.World != null;
			//commandSetting.CustomDrawer = delegate (Rect rect)
			//{
			//	if (Widgets.ButtonText(rect, "UpdateOldLeathersCommandButton".Translate()))
			//	{
			//		WorldUpdate.CheckWorldItemsForUpdate();
			//	}
			//	return true;
			//};

			// --------- Changing defs ---------

			var list = (from entry in Utils.AllPawnsDefWithLeather
						where entry.race != null && entry.race.leatherDef != null
						orderby entry.race.leatherInsulation descending,
								entry.race.leatherStatFactors == null ? 0 : entry.race.leatherStatFactors.Sum((arg) => arg.value) descending
						group entry by entry.race.GetLeatherHash()
			);

			//var list2 = (from entry in list
			//			 where entry.Count() > 1
			//			 select entry);
			var list2 = list;

			string text = "";
			int tierLevel = 0;

			int totalMergedLeathers = list2.Sum((arg) => arg.Count());
			int totalLeathersTier = list2.Count();
			int totalLeathersLeft = list.Count((arg) => arg.Count() == 1);

			text += string.Format("Merged {0} leather types into {1} tiers. {2} unique types left untouched.\n", totalMergedLeathers, totalLeathersTier, totalLeathersLeft);

			foreach (var currentgroup in list2)
			{
				tierLevel++;

				string speciesList = string.Join(" ; ", currentgroup.Select((arg) => arg.label).ToArray());

				string extraModDescriptionInfo = string.Format("\n\n{0}\n\n", ("LessLeatherExtraModInfo".Translate()));

				ThingDef newLeatherDef = currentgroup.OrderBy((arg) => arg.GetMod().Name != "Core").First().race.leatherDef;
				//var newLeatherColor = new UnityEngine.Color;
				string newLeatherLabel = string.Format("TierLeatherName".Translate(), tierLevel) + ((currentgroup.Count() == 1) ? (" (" + currentgroup.First().label + ")") : "");
				string newLeatherDesc = extraModDescriptionInfo + string.Format("TierLeatherDesc".Translate(), speciesList);

				newLeatherDef.description += newLeatherDesc;

				string extraSpecieDesc = extraModDescriptionInfo + string.Format("SpecieExtraDesc".Translate(), newLeatherLabel);

				foreach (ThingDef currentpawndef in currentgroup)
				{
#if DEBUG
					Log.Message(string.Format("{0} -> {1}", currentpawndef.race.leatherDef, newLeatherDef));
#endif
					if (currentpawndef.race.leatherDef != newLeatherDef)
					{
						//if (!Utils.AllConvertedLeathers.ContainsKey(currentpawndef.race.leatherDef))
						Utils.ConvertedLeathers.Add(currentpawndef.race.leatherDef, newLeatherDef);
						currentpawndef.race.leatherDef = newLeatherDef;
					}

					newLeatherDef.label = newLeatherLabel;
					currentpawndef.description += extraSpecieDesc;

				}
				text += string.Format("Created [{0}] (defname: {1}) with leathers from species: {2}\n", newLeatherLabel, newLeatherDef.defName, speciesList);
			}
			Log.Message(text);

#if DEBUG
			Log.Message(string.Join(";", Utils.ConvertedLeathers.Keys.Select((ThingDef arg) => arg.ToString()).ToArray()));
#endif
		}

		private bool ItemsCheckDone { get; set; }
		public override void Tick(int currentTick)
		{
			// For some reasons, doing it in WorldLoaded() won't work unless Find.World.FinalizeInit(); is done. I'm affraid this can cause issues.
			if (!ItemsCheckDone)
			{
				ItemsCheckDone = true;
				//Find.World.FinalizeInit();

				WorldUpdate.CheckWorldItemsForUpdate();
			}
		}

		public override void WorldLoaded()
		{
			ItemsCheckDone = false;
		}

		internal static class Log
		{
			internal static void Message(string text)
			{
				running.Logger.Message(text);
			}
			internal static void Warning(string text)
			{
				running.Logger.Warning(text);
			}
			internal static void Error(string text)
			{
				running.Logger.Error(text);
			}
		}
	}
}
