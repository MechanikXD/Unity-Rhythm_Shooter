using System;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    [Serializable]
    public class WeaponController {
        [SerializeField] private WeaponBase _currentWeapon;
    }
}