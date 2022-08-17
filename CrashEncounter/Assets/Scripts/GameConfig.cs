using UnityEngine;

namespace Runamuck
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public Color[] TeamColors = new Color[]
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.cyan,
        };
    }
}
