using System;
using System.Linq;
using UnityEditor;

namespace Core.Editor
{
	[InitializeOnLoad]
	public static class ConditionHolder
	{
		public static Type[] DerivedTypes;
		public static string[] DerivedTypesNames;
		
		static ConditionHolder()
		{
			var type = typeof(Condition);
			DerivedTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => p.IsAbstract == false && type.IsAssignableFrom(p)).ToArray();
			DerivedTypesNames = new string[DerivedTypes.Length];

			for (int index = 0; index < DerivedTypes.Length; index++)
			{
				DerivedTypesNames[index] = DerivedTypes[index].Name;
			}
		}
	}
}