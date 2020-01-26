using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MusicPlayer
{
    [RequireComponent(typeof(EventTrigger))]
    public class CircleButtonEventHandler : MonoBehaviour
    {
        [Serializable]
        public class UnityOneArgEvent : UnityEvent<string>
        {
            public UnityOneArgEvent() : base() { }
        }

        [SerializeField]
        protected string code = null;
        public UnityOneArgEvent ButtonPressed = new UnityOneArgEvent();

        public virtual void OnPointerClick(BaseEventData eventData)
        {
            ButtonPressed.Invoke(code);
        }
    }
}
