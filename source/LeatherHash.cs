using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace WM.TooManyLeathers
{
	public class LeatherHash
	{
		List<StatModifier> leatherStatFactors;
		float leatherInsulation;

		float leatherCommonalityFactor;
		float leatherMarketValueFactor;

		static readonly List<string> fieldNames = new List<string>() { "leatherStatFactors", "leatherInsulation" };
		List<object> fieldValues = new List<object>(fieldNames.Count);

		private IEnumerable<FieldInfo> AllFields
		{
			get
			{
				return typeof(LeatherHash).GetFields(BindingFlags.NonPublic);
			}
		}

		private IEnumerable<FieldInfo> TargetFields(RaceProperties obj)
		{
			var targettype = obj.GetType();
			return fieldNames.Select((arg) => targettype.GetField(arg));
		}

		public LeatherHash(RaceProperties race)
		{
			this.leatherInsulation = race.leatherInsulation;
			//this.leatherStatFactors = race.leatherStatFactors.ToList();
			this.leatherStatFactors = race.leatherStatFactors;
			this.leatherCommonalityFactor = race.leatherCommonalityFactor;
			this.leatherMarketValueFactor = race.leatherMarketValueFactor;

			//foreach (var fieldName in fieldNames)
			//{ 	
			//	var field = typeof(RaceProperties).GetField(fieldName);
			//	object value = null;

			//	if(field != null)
			//	{
			//		value = field.GetValue(race);
			//	}
			//	fieldValues.Add(value);

			//	Log.Message("LeatherHash() records field " + fieldName + " value:" + value != null ? value.ToString() : "(null)");
			//}
		}

		public override bool Equals(object arg)
		{
			var hash = arg as LeatherHash;

			if (hash == null)
			{
				return false;
			}

			if (Math.Abs(this.leatherInsulation - hash.leatherInsulation) < float.Epsilon
				&& leatherStatFactors == hash.leatherStatFactors
			    && leatherCommonalityFactor == hash.leatherCommonalityFactor
			    && leatherMarketValueFactor == hash.leatherMarketValueFactor
			   )
				return true;

			return false;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static implicit operator string(LeatherHash obj)
		{
			return "";
		}

		//public override string ToString()
		//{
		//	return string.Format(string.Format("[LeatherHash leatherInsulation={0} leatherStatFactors={1}", leatherInsulation, leatherStatFactors));
		//}
	}
}
