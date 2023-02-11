using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public interface IHealthManagement
    {
        public void Damage(int damage);
        public void Heal(int heal);
    }
}