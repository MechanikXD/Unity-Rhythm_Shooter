using UnityEngine;

namespace Player.Weapons.Base {
    [CreateAssetMenu(fileName = "WeaponSettings", menuName = "Scriptable Objects/WeaponSettings")]
    public class WeaponSettings : ScriptableObject {
        [SerializeField] private GameObject _weaponObject;
        [SerializeField] private Animator _weaponAnimator;
        [SerializeField] private bool _canDoDoubleAction;
        [SerializeField] private float _maxShootDistance;

        public GameObject WeaponObject => _weaponObject;
        public Animator Animator => _weaponAnimator;
        public bool CanDoDoubleAction => _canDoDoubleAction;
        public float MaxShootDistance => _maxShootDistance;
    }
}