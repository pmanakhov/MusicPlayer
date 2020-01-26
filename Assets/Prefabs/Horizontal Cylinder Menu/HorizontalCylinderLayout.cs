using UnityEngine;

namespace MusicPlayer
{
    [ExecuteInEditMode]
    public class HorizontalCylinderLayout : MonoBehaviour
    {
        [Header("Menu Item")]
        [SerializeField]
        protected GameObject menuItemPrefab;
        [SerializeField]
        protected GameObject menuItemHolderPrefab;
        [SerializeField, Range(0f, 1f)]
        protected float scaleFactor = 1f;
        [SerializeField]
        protected bool tryToDetermineHeight = true;
        [SerializeField, Tooltip("This value is used when the component fails to determine the vertical size of a menu item prefab or when the checkbox above is unchecked")]
        protected float height = 0f;

        [Header("Layout")]
        [SerializeField]
        protected float cylinderRadius = 1f;
        [SerializeField, Tooltip("Angular offset from the horizon line. It determines where the first element is placed [In degrees]")]
        protected float startFromAngle = 0f;
        [SerializeField, Tooltip("Margin between two consecutive menu times [In degrees]")]
        protected float angularMargin = 0f;

        [Header("Data Provider")]
        [SerializeField]
        protected uint repeat = 10; // TO DO: Remove! For testing purposes only

        protected virtual void OnEnable()
        {
            if (menuItemPrefab == null)
            {
                Debug.LogError("HorizontalCylinderLayout: The 'Prefab' field cannot be left unassigned. Disabling the script");
                enabled = false;
                return;
            }

            if (menuItemHolderPrefab == null)
            {
                Debug.LogError("HorizontalCylinderLayout: The 'HolderPrefab' field cannot be left unassigned. Disabling the script");
                enabled = false;
                return;
            }

            if (scaleFactor <= 0f)
            {
                Debug.LogError("HorizontalCylinderLayout: The 'ScaleFactor' field has to be greater than zero. Disabling the script");
                enabled = false;
                return;
            }

            ClearMenu();
            InstantiateMenuItems();
        }

        protected virtual void OnDisable()
        {
            ClearMenu();
        }

        protected virtual void InstantiateMenuItems()
        {
            Vector3 menuItemLocalPosition = Quaternion.Euler(startFromAngle, 0, 0) * (Vector3.forward * cylinderRadius);
            Quaternion pitchRotation = Quaternion.Euler(GetMenuItemAngularHeight() + angularMargin, 0, 0);
            Quaternion upwardsRelativeTo = Quaternion.Euler(-90, 0, 0);

            int i = 0;
            while (i < repeat)
            {
                GameObject go = Instantiate(menuItemHolderPrefab, transform);
                go.name = "MI Holder (instantiated procedurally)";
                Vector3 ls = go.transform.localScale;
                ls *= scaleFactor;
                go.transform.localScale = ls;
                go.transform.localRotation = Quaternion.LookRotation(menuItemLocalPosition, upwardsRelativeTo * menuItemLocalPosition);
                go.transform.localPosition = menuItemLocalPosition;

                GameObject menuItem = Instantiate(menuItemPrefab, go.transform);
                menuItem.name = $"{menuItemPrefab.name} (instantiated procedurally)";

                go.SetActive(true);

                menuItemLocalPosition = pitchRotation * menuItemLocalPosition;
                i++;
            }
        }

        protected void ClearMenu()
        {
            int i = transform.childCount;
            while (--i >= 0)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        protected float GetMenuItemAngularHeight()
        {
            float menuItemHeight;
            if (!tryToDetermineHeight || !DetermineMenuItemHeight(menuItemPrefab, out menuItemHeight))
                menuItemHeight = height;
            menuItemHeight *= scaleFactor;

            return DealingWithAngles.CalcAngularSize(menuItemHeight, cylinderRadius);
        }

        protected static bool DetermineMenuItemHeight(GameObject prefab, out float height)
        {
            height = 0f;
            GameObject menuItem = Instantiate(prefab);
            menuItem.SetActive(true);
            Collider menuItemCollider = menuItem.GetComponent<Collider>();

            if (menuItemCollider == null)
                return false;
            else
                height = menuItemCollider.bounds.size.y;

            DestroyImmediate(menuItem);
            return height != 0f;
        }
    }
}
