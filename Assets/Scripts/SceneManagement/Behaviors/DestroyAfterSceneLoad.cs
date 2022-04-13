using DependencyInjection;
using UnityEditor;
using UnityEngine;

namespace Rubworld.SceneManagement.Behaviors
{
    public class DestroyAfterSceneLoad : MonoBehaviour
    {
        [SerializeField]
        private SceneAsset scene;
        
        [SerializeField, Inject]
        public SceneLoadedEvent _sceneLoadedEvent;
        
        void Awake()
        {
            _sceneLoadedEvent.AddEventListener(SceneLoadedEventHandler);
        }

        private void SceneLoadedEventHandler(SceneAsset scene)
        {
            if (scene == this.scene)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _sceneLoadedEvent.RemoveEventListener(SceneLoadedEventHandler);
        }
    }
}