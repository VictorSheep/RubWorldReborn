using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR && UNITY_EDITOR_WIN
using System.Collections.Generic;
using UnityEditor;

namespace DependencyInjection
{
	public class DependencyDataBaseProcessor : AssetPostprocessor
	{
		private const string SCRIPTABLE_EXTENSION = ".asset";
		private const string SCRIPT_EXTENSION     = ".cs";

		private const int MAX_AUTO_CREATE_DEPENDENCIES = 5;


		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string assetPath in deletedAssets)
			{
				if (!DependencyDatabase.Instance.IsDataPath(assetPath))
				{
					continue;
				}
				if (assetPath.EndsWith(SCRIPTABLE_EXTENSION))
				{
					TryRemoveDependency(assetPath);
				}

				
			}

			for(int i = 0; i < movedAssets.Length; ++i)
			{
				if (!DependencyDatabase.Instance.IsDataPath(movedAssets[i]))
				{
					continue;
				}

				if (movedAssets[i].EndsWith(SCRIPTABLE_EXTENSION))
				{
					TryMoveDependency(movedFromAssetPaths[i], movedAssets[i]);	
				}
				
			}

			foreach (string assetPath in importedAssets)
			{
				
				if (DependencyDatabase.Instance.IsScriptPath(assetPath) && assetPath.EndsWith(SCRIPT_EXTENSION) && !CheckIsFirstLoading(importedAssets))
				{
					AddToAutoPendingDependencies(assetPath);

				}

				if (DependencyDatabase.Instance.IsDataPath(assetPath) && assetPath.EndsWith(SCRIPTABLE_EXTENSION))
				{
					TryAddDependency(assetPath);
				}
			}

			DependencyDatabase.Instance.RefreshDependencies();
		}

		private static bool CheckIsFirstLoading(string[] importedAssets)
		{
			return importedAssets.Length >= MAX_AUTO_CREATE_DEPENDENCIES;
		}

		private static bool TryMoveDependency(string prevPath, string newPath)
		{
			return DependencyDatabase.Instance.MoveDependency(prevPath, newPath);
		}

		private static bool TryRemoveDependency(string assetPath)
		{
			return DependencyDatabase.Instance.RemoveDependency(assetPath);
		}

		private static bool TryAddDependency(string assetPath)
		{

			return DependencyDatabase.Instance.AddDependency(assetPath);
		}

		private static void AddToAutoPendingDependencies(string assetPath)
		{
			DependencyDatabase.Instance.AddToAutoPendingDependencies(assetPath);
		}

		[MenuItem("Tools/Dependency Injection/Regenerate Dependencies")]
		public static void RefreshDatabase()
		{
			DependencyDatabase.Instance.Refresh();
		}

	}
}
#endif