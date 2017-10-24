using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.TooManyLeathers
{
	public static class WorldUpdate
	{
		public static void CheckWorldItemsForUpdate()
		{
			var leatherThings = Utils.AllWorldDiscardedLeatherThings();
#if DEBUG
			Log.Message(string.Format("Mapscount = {0} | AllMapItemsCount = {1}", Find.Maps.Count, Find.Maps.Sum((arg) => arg.listerThings.ThingsInGroup(ThingRequestGroup.HaulableEver).Count)));
#endif

			if (leatherThings.Any())
			{
				Action UpdateWorldItems = delegate
				{
					foreach (var item in leatherThings)
					{
						item.def = Utils.ConvertedLeathers[item.def];
					}
				};
				var dialog = new Dialog_MessageBox("UpdateWorldItemsDialogText".Translate(), "UpdateWorldItemsDialogTextDoUpdate".Translate(), UpdateWorldItems, "UpdateWorldItemsDialogTextDoNothing".Translate(), null);
				dialog.forcePause = true;

				Find.WindowStack.Add(dialog);
			}
		}
	}
}
