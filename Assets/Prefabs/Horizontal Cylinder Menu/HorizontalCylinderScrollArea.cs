using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MusicPlayer
{
    public class HorizontalCylinderScrollArea : MonoBehaviour, IScrollHandler
    {
        [Header("Scrolling")]
        [SerializeField]
        protected Rigidbody scrollableObject;
        [SerializeField, Tooltip("In newton metre (N⋅m)")]
        protected float torque = 1f;

        [Header("In-Editor Preview")]
        [SerializeField]
        protected float ceiling;
        [SerializeField]
        protected float floor;
        protected float angularOffset;

        protected float verticalScrollDelta;

        protected List<HCMenuItemHolderController> menuItemHolders = new List<HCMenuItemHolderController>();
        protected Quaternion? firstMenuItemLocalRotation;
        protected Quaternion? lastMenuItemLocalRotation;

        protected virtual void Awake()
        {
            if (scrollableObject == null)
            {
                Debug.LogError("HorizontalCylinderScrollArea: The 'ScrollableObject' field cannot be left unassigned. Disabling the script");
                enabled = false;
                return;
            }

            Transform obj = transform.Find("Ceiling Holder/Ceiling");
            if (obj == null)
            {
                Debug.LogError("HorizontalCylinderScrollArea: Couldn't find a child object named 'Ceiling' or its parent. Disabling the script");
                enabled = false;
                return;
            }
            ceiling = DealingWithAngles.Runtime2Editor(obj.parent.localRotation.eulerAngles.x);

            Collider c = obj.GetComponent<Collider>();
            if (c == null)
            {
                Debug.LogError("HorizontalCylinderScrollArea: Couldn't find a collider in an object named 'Ceiling'. Disabling the script");
                enabled = false;
                return;
            }
            angularOffset = DealingWithAngles.CalcAngularSize(c.bounds.size.y, obj.localPosition.z);

            obj = transform.Find("Floor Holder/Floor");
            if (obj == null)
            {
                Debug.LogError("HorizontalCylinderScrollArea: Couldn't find a child object named 'Floor' or its parent. Disabling the script");
                enabled = false;
                return;
            }
            floor = DealingWithAngles.Runtime2Editor(obj.parent.localRotation.eulerAngles.x);
        }

        protected virtual void Start()
        {
            scrollableObject.transform.Rotate(angularOffset, 0f, 0f);
        }

        protected virtual void FixedUpdate()
        {
            if (!firstMenuItemLocalRotation.HasValue)
                return;

            float x = GetRelativeRotationAroundX(firstMenuItemLocalRotation.Value);
            if (DealingWithAngles.Runtime2Editor(x) >= ceiling + angularOffset)
            {
                scrollableObject.angularVelocity = Vector3.zero;

                if (verticalScrollDelta > 0)
                    return;
            }

            x = GetRelativeRotationAroundX(lastMenuItemLocalRotation.Value);
            if (DealingWithAngles.Runtime2Editor(x) <= floor - angularOffset)
            {
                scrollableObject.angularVelocity = Vector3.zero;

                if (verticalScrollDelta < 0)
                    return;
            }

            if (verticalScrollDelta != 0f)
            {
                scrollableObject.AddRelativeTorque(Vector3.right * torque * verticalScrollDelta);
                verticalScrollDelta = 0f;
            }
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            verticalScrollDelta = eventData.scrollDelta.y;
            if (transform.parent != null)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.scrollHandler);
        }

        public virtual void RegisterNewMenuItemHolder(HCMenuItemHolderController menuItemHolder,
            out float ceiling, out float floor)
        {
            if (!enabled)
                throw new UnityException("You cannot call this method on a disabled HorizontalCylinderScrollArea component");
            if (menuItemHolder == null)
                throw new UnityException("Couldn't register 'null'");
            if (menuItemHolders.Contains(menuItemHolder))
                throw new UnityException("Re-registration is forbidden");

            ceiling = this.ceiling;
            floor = this.floor;
            
            UpdateFirstAndLastItemRotations(menuItemHolder.transform.localRotation);
            menuItemHolders.Add(menuItemHolder);
        }

        public virtual void UnregisterMenuItemHolder(HCMenuItemHolderController menuItemHolder)
        {
            if (menuItemHolder == null)
                throw new UnityException("Couldn't unregister 'null'");
            if (!menuItemHolders.Contains(menuItemHolder))
                throw new UnityException("Couldn't unregister an item which hasn't been registered before");

            menuItemHolders.Remove(menuItemHolder);
            // FIX IT: We need to update the first and the last item rotations here
        }

        protected void UpdateFirstAndLastItemRotations(Quaternion rotation)
        {
            if (!firstMenuItemLocalRotation.HasValue)
                firstMenuItemLocalRotation = lastMenuItemLocalRotation = rotation;
            else
            {
                float xRotation = DealingWithAngles.Runtime2Editor(rotation.eulerAngles.x);
                if (xRotation < DealingWithAngles.Runtime2Editor(firstMenuItemLocalRotation.Value.eulerAngles.x))
                {
                    firstMenuItemLocalRotation = rotation;
                    return;
                }

                if (xRotation > DealingWithAngles.Runtime2Editor(lastMenuItemLocalRotation.Value.eulerAngles.x))
                    lastMenuItemLocalRotation = rotation;
            }
        }

        protected float GetRelativeRotationAroundX(Quaternion q)
        {
            Quaternion relativeRotation = q * scrollableObject.transform.localRotation;
            return relativeRotation.eulerAngles.x;
        }
    }
}