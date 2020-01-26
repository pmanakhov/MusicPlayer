using UnityEngine;

public class TorsoReferencedContent : MonoBehaviour
{
    [SerializeField]
    protected new Transform camera;

    protected static readonly float POSITION_LERP_SPEED = 5f;

    protected virtual void Start()
    {
        if (camera == null)
        {
            Debug.LogError("TorsoRef: The 'Camera' field cannot be left unassigned. Disabling the script");
            enabled = false;
            return;
        }
    }

    protected virtual void Update()
    {
        float posSpeed = Time.deltaTime * POSITION_LERP_SPEED;
        transform.position = Vector3.SlerpUnclamped(transform.position, camera.position, posSpeed);
    }
}
