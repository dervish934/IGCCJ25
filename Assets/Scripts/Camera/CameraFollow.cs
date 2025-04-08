using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform protagonist;
    public Vector3 offset = new Vector3(0, 0, 0);

    private void LateUpdate()
    {
        if (protagonist != null)
        {
            transform.position = protagonist.position + offset;
        }
        else
        {
            Debug.LogError("CameraFollow en MainCamera no se ha asignado al protagonista en el inspector. La camara no sabe a quien seguir.");
            enabled = false;
        }
    }
}
