using System;
using UnityEngine;

namespace ColorRings.Runtime.UI
{
    public class BlockUI : MonoBehaviour
    {
        [SerializeField] private bool isBlockRingHolder;
        private int _blockCount;

        private void OnEnable()
        {
            var canvas = GetComponent<Canvas>();
            if(canvas == null) return;
            canvas.sortingLayerName = "UI";
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
        }

        public void Block()
        {
            _blockCount++;
            DevLog.Log("Block Count",_blockCount);
            if (_blockCount <= 0) return;
            gameObject.SetActive(true);
            RingHolder.IsLock = isBlockRingHolder;
        }
        
        public void UnBlock()
        {
            _blockCount--;
            DevLog.Log("UnBlock Count",_blockCount);
            if (_blockCount > 0) return;
            gameObject.SetActive(false);
            RingHolder.IsLock = false;
        }
    }
}