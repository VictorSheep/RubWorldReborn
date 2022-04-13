using Events;
using UnityEditor;

namespace Rubworld.SceneManagement.Behaviors
{
    [DependencyInjection.Dependency(true, "GameEventSceneManagement")]
    public class SceneStartLoadEvent : GameEvent<SceneAsset>
    {
    }
}