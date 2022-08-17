using UnityEngine;

namespace Runamuck
{
    public class Game : MonoBehaviour
    {
        private static Game instance;
        public static Game Instance => instance;

        [SerializeField] private GameConfig config;
        public GameConfig Config => config;

        private void Awake()
        {
            instance = this;
        }
    }
}
