using UnityEngine;

[RequireComponent(typeof(ObjectPlacer))]
public class TapListener : MonoBehaviour
{
    private ObjectPlacer objectPlacer;

    private void Start()
    {
        objectPlacer = GetComponent<ObjectPlacer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            objectPlacer.Place();
    }
}
