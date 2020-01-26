using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MusicPlayer
{
    public class MenuItemEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Serializable]
        public class UnityOneArgEvent : UnityEvent<string>
        {
            public UnityOneArgEvent() : base() { }
        }

        [SerializeField]
        protected string code = null;
        public UnityOneArgEvent ItemSelected = new UnityOneArgEvent();

        protected TextMeshPro title;
        protected GameObject background;

        protected virtual void Start()
        {
            Transform obj = transform.Find("Title");
            if (obj == null || (title = obj.GetComponent<TextMeshPro>()) == null)
            {
                Debug.LogError("MenuItemEventHandler: Couldn't find a child object named 'Title' or the 'TextMeshPro' component in it. Disabling the script");
                enabled = false;
                return;
            }

            obj = transform.Find("Background");
            if (obj == null)
            {
                Debug.LogError("MenuItemEventHandler: Couldn't find a child object named 'Background'. Disabling the script");
                enabled = false;
                return;
            }
            else
                background = obj.gameObject;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            background.SetActive(true);
            if (transform.parent != null)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            background.SetActive(false);
            if (transform.parent != null)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerExitHandler);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            ItemSelected.Invoke(String.IsNullOrEmpty(code) ? title.text : code);
            if (transform.parent != null)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }
    }
}
