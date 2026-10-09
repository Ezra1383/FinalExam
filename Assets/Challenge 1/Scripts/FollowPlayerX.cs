using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public GameObject plane;

    [SerializeField] private Vector3 offset = new Vector3(0, 5, -30);

    // LateUpdate runs after the plane has moved this frame
    void LateUpdate()
    {
        if (plane == null)
        {
            return;
        }

        transform.position = plane.transform.position + offset;
    }
}
