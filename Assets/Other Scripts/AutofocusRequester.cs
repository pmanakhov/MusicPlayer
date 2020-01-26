using UnityEngine;
using Vuforia;

namespace MusicPlayer
{
    public class AutofocusRequester : MonoBehaviour
    {
        private bool vuforiaStarted = false;

        private void Awake()
        {
            VuforiaARController vuforia = VuforiaARController.Instance;
            if (vuforia != null)
                vuforia.RegisterVuforiaStartedCallback(StartAfterVuforia);
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (vuforiaStarted)
                    SetAutofocus();
            }
        }

        private void StartAfterVuforia()
        {
            vuforiaStarted = true;
            SetAutofocus();
        }

        private void SetAutofocus()
        {
            if (!CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
                Debug.Log("CameraFocus: This device doesn't support auto focus");
        }
    }
}