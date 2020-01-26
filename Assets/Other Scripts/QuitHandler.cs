using UnityEngine;

namespace MusicPlayer
{
    public class QuitHandler : MonoBehaviour
    {
        private void Awake()
        {
            // Enable ARCore to target 60fps camera capture frame rate on supported devices
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            // Exit the app when the 'back' button is pressed
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
