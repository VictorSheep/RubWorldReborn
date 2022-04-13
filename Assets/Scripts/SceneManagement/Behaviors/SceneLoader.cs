using System.Collections;
using DependencyInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rubworld.SceneManagement.Behaviors
{
    public class SceneLoader : MonoBehaviour
    {
        private bool           _sceneIsLoading     = false;
        private bool           _sceneIsLoaded      = false;
        private AsyncOperation _loadSceneOperation = null;
        
        public float LoadingProgress => _loadSceneOperation.progress;
        
        [SerializeField, Inject]
        public SceneLoadedEvent    _sceneLoadedEvent;
        [SerializeField, Inject]
        public SceneStartLoadEvent _sceneStartLoadEvent;
        
        public void LoadScene(SceneAsset scene, LoadSceneMode loadSceneMode)
        {
            _sceneIsLoading = true;
            _sceneIsLoaded  = false;
            StartCoroutine(LoadYourAsyncScene(scene, loadSceneMode));
        }
        
        private IEnumerator LoadYourAsyncScene(SceneAsset scene, LoadSceneMode loadSceneMode)
        {
            _loadSceneOperation = SceneManager.LoadSceneAsync(scene.name, loadSceneMode);
            _sceneStartLoadEvent.Trigger(scene);
            
            // Wait until the asynchronous scene fully loads
            while (!_loadSceneOperation.isDone)
            {
                yield return null;
            }
            
            _sceneLoadedEvent.Trigger(scene);
            _sceneIsLoading = false;
            _sceneIsLoaded  = true;
        }
    }
}