using UnityEngine;

namespace Runamuck
{
    public class Game : MonoBehaviour
    {
        private static Game instance;
        public static Game Instance => instance;

        [SerializeField] private GameConfig config;
        public GameConfig Config => config;
        
        [SerializeField] private GameSkin skin;
        public static GameSkin Skin => Instance.skin;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
