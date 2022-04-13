using System;
using System.Collections.Generic;
#if UNITY_EDITOR && UNITY_EDITOR_WIN
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DependencyInjection
{
    [InitializeOnLoad]
    public class DelayedRefreshDependencies
    {
        private const int MAX_REFRESH_COUNT = 2;

        private static int refreshCount = 0;
        static DelayedRefreshDependencies()
        {
            EditorApplication.update += EditorUpdate;
            DependencyDatabase.Instance.Refresh();
        }

        private static void EditorUpdate()
        {
            DependencyDatabase.Instance.RefreshDependencies();
            DependencyDatabase.Instance.TryAddAllPendingAutoDependencies();
            DependencyDatabase.Instance.TryAddFailToAddDependencies();
			
            ++refreshCount;
            if (refreshCount >= MAX_REFRESH_COUNT)
            {
                EditorApplication.update -= EditorUpdate;
            }

        }
    }
}

#endif