using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace DependencyInjection
{
	[Serializable]
	public class DependencyDBItem
	{
		public string           path;
		public ScriptableObject scriptableObject;
		public bool             hasDependency;
	}

	public class DependencyDatabase : ScriptableObject
	{
#region Static
		private const  string                  DB_PATH  = "Assets/Data/InjectDatabase.asset";
		private static DependencyDatabase instance = null;

		public static DependencyDatabase Instance
		{
			get
			{
				if (!instance)
				{
					CreateDb();
				}
				return instance;
			}
		}

		private static void CreateDb()
		{
			instance = AssetDatabase.LoadAssetAtPath(DB_PATH, typeof(DependencyDatabase)) as DependencyDatabase;
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<DependencyDatabase>();
				if (!AssetDatabase.IsValidFolder("Assets/Data"))
				{
					AssetDatabase.CreateFolder("Assets", "Data");
				}
				AssetDatabase.CreateAsset(instance, DB_PATH);
			}
		}

#endregion

#region Exposed
		[SerializeField]
		private string [] lookupDataPaths = {"Assets/Data", "Assets/Samples"};

		[SerializeField]
		private string [] lookupScriptPaths = {"Assets/Scripts", "Packages"};
		[ReadOnly][SerializeField]
		private List<DependencyDBItem> dependencies = new List<DependencyDBItem>();
		[HideInInspector][SerializeField]
		private List<DependencyDBItem> failToAddDepdencies = new List<DependencyDBItem>();

		[SerializeField, HideInInspector]
		private List<string> pendingAutoAddDependencies = new List<string>();
#endregion

#region Accessors;

#endregion

#region Private Variables
#endregion

#region Private Methods
		private string FormatPath(string path)
		{
			path = path.Replace("/", "");
			path.Replace("\\", "");
			return path;
		}
		
		private DependencyDBItem LookupDBItem(string path)
		{
			path = FormatPath(path); 
			foreach(DependencyDBItem item in dependencies)
			{
				string itemPath  = FormatPath(item.path);
				if (itemPath == path)
				{
					return item;
				}
			}

			return null;
		}

		private DependencyDBItem LookupType(Type type, string specificName = "")
		{
			foreach (DependencyDBItem item in dependencies)
			{
				if (!item.hasDependency)
				{
					continue;
				}
				Type itemType = item.scriptableObject.GetType();
				if (itemType == type && (specificName == String.Empty || item.scriptableObject.name == specificName))
				{
					return item;
				}

				if (itemType.BaseType == type && type != typeof(ScriptableObject) && (specificName == String.Empty || item.scriptableObject.name == specificName))
				{
					return item;
				}

				Type [] interfaces = itemType.GetInterfaces();
				foreach (Type interf in interfaces)
				{
					if (interf == type && (specificName == String.Empty || item.scriptableObject.name == specificName))
					{
						return item;
					}
				}
			}

			return null;
		}

		private bool HasDependency(ScriptableObject scriptableObject)
		{
			Type       assetType  = scriptableObject.GetType();
			Dependency dependency = assetType.GetCustomAttributes(typeof(Dependency), true).FirstOrDefault() as Dependency;
			return dependency != null;
		}


#endregion

#region Internal Methods
		internal bool IsDataPath(string path)
		{
			foreach (string dataPath in lookupDataPaths)
			{
				if (path.StartsWith(dataPath))
				{
					return true;
				}
			}

			return false;
		}

		internal bool IsScriptPath(string path)
		{
			foreach (string scriptPath in lookupScriptPaths)
			{
				if (path.StartsWith(scriptPath))
				{
					return true;
				}
			}

			return false;
		}

		internal bool RemoveDependency(string assetPath)
		{
			DependencyDBItem item = LookupDBItem(assetPath);
			if (item == null)
			{
				return false;
			}

			dependencies.Remove(item);
			return true;
		}

		internal bool AddDependency(string assetPath)
		{
			ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
			if (scriptableObject == null)
			{
				DependencyDBItem failToAddItem = new DependencyDBItem
				{
					path             = assetPath,
					scriptableObject = scriptableObject,
					hasDependency    = false
				};
				failToAddDepdencies.Add(failToAddItem);
				return false;
			}
			if (LookupDBItem(assetPath) != null)
			{
				return false;
			}

			DependencyDBItem item = new DependencyDBItem
			{
				path             = assetPath,
				scriptableObject = scriptableObject,
				hasDependency = HasDependency(scriptableObject)
			};
			dependencies.Add(item);
			return true;
		}

		internal bool MoveDependency(string prevPath, string newPath)
		{
			DependencyDBItem item = LookupDBItem(prevPath);
			if (item == null)
			{
				return false;
			}

			item.path = newPath;
			return true;
		}

		internal void RefreshDependencies()
		{
			for(int i = dependencies.Count -1; i >= 0; --i)
			{
				DependencyDBItem item = dependencies[i];
				item.hasDependency = HasDependency(item.scriptableObject);
			}
		}

		internal void TryAddFailToAddDependencies()
		{
			for (int i = failToAddDepdencies.Count - 1; i >= 0; --i)
			{
				AddDependency(failToAddDepdencies[i].path);
				failToAddDepdencies.RemoveAt(i);
			}
		}

		internal void Refresh()
		{
			dependencies.Clear();
			string [] assetPaths = AssetDatabase.GetAllAssetPaths();
			foreach(var assetPath in assetPaths)
			{

				if(assetPath.EndsWith(".asset") && IsDataPath(assetPath))
				{
					AddDependency(assetPath);
				}
			}

			EditorUtility.SetDirty(this);
		}

		internal void RefreshIfEmpty()
		{
			if (dependencies.Count == 0)
			{
				Refresh();
			}
		}

		internal void TryAddAllPendingAutoDependencies()
		{
			for (int i = pendingAutoAddDependencies.Count - 1; i >= 0; --i)
			{
				TryAutoAddDependencyAsset(pendingAutoAddDependencies[i]);
				pendingAutoAddDependencies.RemoveAt(i);
			}
		}

		internal void AddToAutoPendingDependencies(string scriptAssetPath)
		{
			pendingAutoAddDependencies.Add(scriptAssetPath);
		}

		internal bool TryAutoAddDependencyAsset(string scriptAssetPath)
		{
			MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptAssetPath);
			if (monoScript == null)
			{
				return false;
			}
			Type type = monoScript.GetClass();
			if (type == null)
			{
				return false;
			}
			Dependency dependency = type.GetCustomAttributes(typeof(Dependency), true).FirstOrDefault() as Dependency;
			if (dependency != null && dependency.AutoCreateScriptable && type.IsSubclassOf(typeof(ScriptableObject)))
			{
				DependencyDBItem item = LookupType(type);
				if (item == null)
				{
					ScriptableObject dependencyAsset = ScriptableObject.CreateInstance(type);
					string basePath = lookupDataPaths[0];
					if (dependency.DataFolder != string.Empty)
					{
						string fullPath = Path.Combine(basePath, dependency.DataFolder);
						if (!AssetDatabase.IsValidFolder(fullPath))
						{
							AssetDatabase.CreateFolder(basePath, dependency.DataFolder);
						}
						basePath = fullPath;
					}
					string  path = Path.Combine(basePath, type.Name + ".asset");
					AssetDatabase.CreateAsset(dependencyAsset, path);
					AssetDatabase.SaveAssets();
					return true;
				}
			}

			return false;
		}

#endregion

#region Interface
		public ScriptableObject Resolve(Type type, string specificName = "")
		{
			DependencyDBItem item = LookupType(type, specificName);
			if (item != null)
			{
				return item.scriptableObject;
			}

			return null;
		}

#endregion
	}
}
#endif