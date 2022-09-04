using UnityEngine;

namespace Runamuck
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        [Tooltip("Allow game to continue on, ignoring any victory conditions for testing")]
        public bool DebugDontEndGame = true;

        public Color[] TeamColors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.cyan,
        };
    }
}
