using UnityEngine;

namespace Interfaces
{
    public interface ICollectable
    {
        public void OnMagnetRangeEnter(Transform origin);
        public void OnMagnetRangeExit(Transform origin);
        public void Collect();

        public bool CanCollect();
    }
}