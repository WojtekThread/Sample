using System;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
	[CustomPropertyDrawer(typeof(ConditionCollection))]
	public class ConditionCollectionDrawer : PropertyDrawer
	{
		private const float LINE_HEIGHT = 18f;
	
		private int _selectedIndex;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var conditions = GetConditions(property);
			float height = LINE_HEIGHT;

			if (conditions.isExpanded)
			{
				for (int index = 0; index < conditions.arraySize; index++)
				{
					 height += EditorGUI.GetPropertyHeight(conditions.GetArrayElementAtIndex(index));
				}
			}

			return height + LINE_HEIGHT;
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{ 
			EditorGUI.BeginProperty(position, label, property);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			var obj = GetConditions(property);
			
			float conditionsHeight = LINE_HEIGHT;
			
			EditorGUI.BeginChangeCheck();

			var rect = new Rect(position.position, new Vector2(position.size.x, LINE_HEIGHT));

			if (obj.arraySize > 0)
			{
				obj.isExpanded = EditorGUI.Foldout(rect, obj.isExpanded, "Conditions");

				if (obj.isExpanded)
				{
					rect = new Rect(rect.position + new Vector2(0, LINE_HEIGHT), rect.size);
					DrawConditions(rect, obj, out conditionsHeight);
				}
			}
			else
			{
				EditorGUI.LabelField(rect, "No conditions added");				
			}

			rect = new Rect(rect.position + new Vector2(0, conditionsHeight),
				new Vector2(rect.size.x, LINE_HEIGHT));
			DrawSelection(rect, obj);

			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		private void DrawSelection(Rect rect, SerializedProperty collection)
		{
			var leftSize = rect.size.x * 0.85f;
			var rightSize = rect.size.x * 0.15f;
			var rectLeft = new Rect(rect.position, new Vector2(leftSize, rect.size.y));
			var rectRight = new Rect(rect.position + new Vector2(leftSize, 0), new Vector2(rightSize, rect.size.y));
			
			_selectedIndex = EditorGUI.Popup(rectLeft, _selectedIndex, ConditionHolder.DerivedTypesNames);

			if (UnityEngine.GUI.Button(rectRight, "+"))
			{
				collection.arraySize++;
				var newEntry = collection.GetArrayElementAtIndex(collection.arraySize-1);
				var typeToCreate = ConditionHolder.DerivedTypes[_selectedIndex];
				newEntry.managedReferenceValue = Activator.CreateInstance(typeToCreate);
			}
		}

		private void DrawConditions(Rect rect, SerializedProperty collection, out float totalHeight)
		{
			totalHeight = 0;
			EditorGUI.indentLevel += 1;
			for (int index = 0; index < collection.arraySize; index++)
			{
				var property = collection.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(rect, property, true);
				var propertyHeight = EditorGUI.GetPropertyHeight(property);
				totalHeight += propertyHeight;
				rect = new Rect(rect.position + new Vector2(0, propertyHeight), rect.size);
			}
			EditorGUI.indentLevel -= 1;
		}

		public SerializedProperty GetConditions(SerializedProperty property)
		{
			return property.FindPropertyRelative("_listOfConditions");
		}
	}
}