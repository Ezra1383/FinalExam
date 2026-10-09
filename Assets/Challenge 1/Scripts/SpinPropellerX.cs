using UnityEngine;

public class SpinPropellerX : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 500f;

    // Update is called once per frame
    void Update()
    {
        // spin around the propeller's own forward axis (the nose of the plane)
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }
}
