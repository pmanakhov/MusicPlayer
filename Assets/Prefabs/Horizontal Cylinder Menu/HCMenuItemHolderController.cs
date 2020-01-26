using UnityEngine;

namespace MusicPlayer
{
    [ExecuteInEditMode]
    public class HCMenuItemHolderController : MonoBehaviour
    {
        protected float ceiling;
        protected float floor;

        protected virtual void Awake()
        {
            HorizontalCylinderScrollArea scrollArea = FindScrollAreaInParents();
            scrollArea.RegisterNewMenuItemHolder(this, out ceiling, out floor);
        }

        protected virtual void OnEnable()
        {
            SetChildrenActive(IsRotationWithinRange());
        }

        protected virtual void Update()
        {
            if (transform.parent.hasChanged)
                SetChildrenActive(IsRotationWithinRange());
        }

        protected virtual void OnDestroy()
        {
            HorizontalCylinderScrollArea scrollArea = FindScrollAreaInParents();
            scrollArea.UnregisterMenuItemHolder(this);
        }

        protected HorizontalCylinderScrollArea FindScrollAreaInParents()
        {
            // FIX IT: For testing purposes
            return transform.parent.parent.GetComponent<HorizontalCylinderScrollArea>();
        }

        protected bool IsRotationWithinRange()
        {
            Quaternion relativeRotation = transform.localRotation * transform.parent.localRotation;
            float edX = DealingWithAngles.Runtime2Editor(relativeRotation.eulerAngles.x);
            if (edX >= ceiling && edX <= floor)
                return true;

            return false;
        }

        protected void SetChildrenActive(bool value)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(value);
        }
    }
}
