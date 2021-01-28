using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class HoldEvent : MonoBehaviour, IClickDownHandler, IClickUpHandler
    {
        public enum ComparisonOp
        {
            LessThan,
            GreaterThan,
            EqualTo
        }

        public enum MultiLogic
        {
            All,
            Any
        }

        [Serializable]
        public class Item
        {
            public ComparisonOp op;
            public float        seconds;
        }
        
        [Serializable]
        public class MousePositionEvent : UnityEvent<Vector3> {}

        public MultiLogic logic;
        public List<Item> conditions;
        public bool       left = true;
        public bool       middle;
        public bool       right;
        public MousePositionEvent onConditionsSatisfied;

        #region IClickHandler

        public void LeftClick(PointerEventData ptrData)
        {
            if (left) Down(ptrData);
        }

        public void RightClick(PointerEventData ptrData)
        {
            if (right) Down(ptrData);
        }

        public void MiddleClick(PointerEventData ptrData)
        {
            if (middle) Down(ptrData);
        }

        private float   _downTime     = float.MinValue;
        private Vector3 _downPosition = Vector3.zero;

        public void Down(PointerEventData ptrData)
        {
            _downTime     = Time.time;
            _downPosition = ptrData.position;
        }

        public void Up()
        {
            bool success = false;
            switch (logic)
            {
                case MultiLogic.All:
                    success = conditions.TrueForAll(Satisfied);
                    break;
                case MultiLogic.Any:
                    success = conditions.Exists(Satisfied);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (success) onConditionsSatisfied.Invoke(_downPosition);
        }

        #endregion

        private bool Satisfied(Item item)
        {
            float elapsed = Time.time - _downTime;
            switch (item.op)
            {
                case ComparisonOp.LessThan:
                    return elapsed < item.seconds;
                case ComparisonOp.GreaterThan:
                    return elapsed > item.seconds;
                case ComparisonOp.EqualTo:
                    return Math.Abs(elapsed - item.seconds) < 0.01f;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}