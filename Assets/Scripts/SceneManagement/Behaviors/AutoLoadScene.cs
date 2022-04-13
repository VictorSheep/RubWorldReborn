using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rubworld.SceneManagement.Behaviors
{
    public class AutoLoadScene : MonoBehaviour
    {
        [SerializeField]
        private float _delay;
        [SerializeField]
        private SceneAsset _scene;
        [SerializeField]
        private LoadSceneMode _loadSceneMode;
        
        [SerializeField]
        private SceneLoader _sceneLoader;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_delay);
            _sceneLoader.LoadScene(_scene, _loadSceneMode);
        }
    }
}