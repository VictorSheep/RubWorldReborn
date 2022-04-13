#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DependencyInjection.Editor
{
	[CustomPropertyDrawer(typeof(InjectAttribute))]
	public class InjectReadOnlyDrawer : PropertyDrawer
	{
		private static FieldInfo GetPropertyFieldInfo(SerializedProperty property)
		{
			Type      parentComponentType = property.serializedObject.targetObject.GetType();
			return parentComponentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		}

		private static FieldInfo GetPropertyWithPath(SerializedProperty property, string path)
		{
			Type      parentComponentType = property.serializedObject.targetObject.GetType();
			return parentComponentType.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		}

		private static Type GetPropertyType(SerializedProperty property)
		{
			return GetPropertyFieldInfo(property).FieldType;
		}

		private static InjectAttribute GetPropertyAttribute(SerializedProperty property)
		{
			FieldInfo fieldInfo = GetPropertyFieldInfo(property);
			return fieldInfo.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
		}

		public override float GetPropertyHeight(SerializedProperty property,
												GUIContent         label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect               position,
								   SerializedProperty property,
								   GUIContent         label)
		{
			GUI.enabled = false;
			Type type = GetPropertyType(property);
			if (type != null)
			{
				InjectAttribute  injectAttribute = GetPropertyAttribute(property);
				ScriptableObject dependency      = DependencyDatabase.Instance.Resolve(type, injectAttribute.DependencyName);
				if (dependency != property.objectReferenceValue)
				{
					property.objectReferenceValue = dependency;
					if (property.objectReferenceValue)
					{
						EditorUtility.SetDirty(property.objectReferenceValue);
					}
				}

				if (injectAttribute.OtherFieldsToFill != null)
				{
					foreach (string otherField in injectAttribute.OtherFieldsToFill)
					{
						FieldInfo otherFieldInfo = GetPropertyWithPath(property, otherField);
						if (otherFieldInfo != null)
						{
							otherFieldInfo.SetValue(property.serializedObject.targetObject, dependency);
						}
					}
				}
			}

			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
#endif