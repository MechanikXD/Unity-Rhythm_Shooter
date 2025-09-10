using UnityEngine;

namespace Core.Behaviour.SingletonBehaviour {
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour {
        public static T Instance { get; private set; }

        protected virtual void Awake() => ToSingleton();

        protected void ToSingleton(bool dontDestroyOnLoad=true) {
            if (Instance != null) {
                Debug.LogWarning($"Multiple Instances of {typeof(T)} was found on the scene!\n" +
                                 $"{gameObject.name} will be destroyed upon start.");
                Destroy(gameObject);
                return;
            }

            Instance = (T)(MonoBehaviour)this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        protected void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }
    }
}