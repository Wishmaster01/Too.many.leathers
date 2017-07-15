using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WM.TooManyLeathers
{
	public static class Utils
	{
		public static IEnumerable<ThingDef> AllPawnsDefWithLeather
		{
			get
			{
				return DefDatabase<ThingDef>.AllDefs.Where((arg) => arg.race != null && arg.race.leatherDef != null && arg.race.useLeatherFrom == null);
			}
		}

		internal static Dictionary<ThingDef, ThingDef> ConvertedLeathers = new Dictionary<ThingDef, ThingDef>();

		internal static IEnumerable<Thing> AllWorldDiscardedLeatherThings()
		{
#if DEBUG
				int totalProcessedThings = 0;
#endif
				var allLeatherThings = new List<Thing>();

				Action<IEnumerable<Thing>> ProcessThingsList = delegate (IEnumerable<Thing> list)
				{
					allLeatherThings.AddRange(
						list.Where((arg) => ConvertedLeathers.ContainsKey(arg.def) && ConvertedLeathers[arg.def] != arg.def));
				};

				Find.Maps.ForEach((obj) => ProcessThingsList(obj.listerThings.ThingsInGroup(ThingRequestGroup.HaulableEver)));
				Find.WorldObjects.Caravans.ForEach((obj) => ProcessThingsList(obj.Goods));

				//might not be necessary
				//Find.WorldObjects.Caravans.ForEach((obj) => obj.PawnsListForReading.ForEach((obj2) => ProcessThingsList(obj2.inventory.innerContainer)));

#if DEBUG
				Log.Message(string.Format("allLeatherThings.Count = {0}/{1}", allLeatherThings.Count(), totalProcessedThings));
#endif

				return allLeatherThings;
		}


		//public static ThingDef MakeNewLeatherDef(this ThingDef leatherDef)
		//{
		//	ThingDef newdef = DefDatabase<ThingDef>.AllDefs.First((arg) => arg == leatherDef);

		//	newdef.race.lea
		//}

		public static LeatherHash GetLeatherHash(this RaceProperties race)
		{
			return new LeatherHash(race);
		}

		public static ModContentPack GetMod(this Def def)
		{
			foreach (var mod in LoadedModManager.RunningMods.Reverse())
			{
				var AllThingDefs = mod.AllDefs.Where((Def arg) => arg is ThingDef);

				bool result = AllThingDefs.Any(delegate (Def arg)
				{
					if (!(arg is ThingDef))
						return false;

					if (arg == def)
						return true;

					if (((ThingDef)arg).race != null)
					{
						if (((ThingDef)arg).race.meatDef == def)
							return true;
						if (((ThingDef)arg).race.corpseDef == def)
							return true;
					}

					return false;
				});

				if (result)
					return mod;
			}

			Log.Warning("Could not figure out the mod of Def " + def);
			return null;
		}
	}
}
