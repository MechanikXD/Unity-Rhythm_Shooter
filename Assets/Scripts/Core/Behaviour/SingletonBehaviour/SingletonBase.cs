using UnityEngine;

namespace Core.Behaviour.SingletonBehaviour {
    public class SingletonBase : MonoBehaviour {
        public SingletonBase Instance { get; private set; }

        protected virtual void Awake() => ToSingleton();

        private void ToSingleton(bool dontDestroyOnLoad=true) {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}