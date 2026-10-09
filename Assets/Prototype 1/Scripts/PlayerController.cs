using UnityEngine;

// Namespaced so this car controller does not collide with Prototype 4's PlayerController.
namespace Prototype1
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float turnSpeed = 45f;

        private float horizontalInput;
        private float forwardInput;

        // Update is called once per frame
        void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            // Move the vehicle forward/backward based on the vertical input
            transform.Translate(Vector3.forward * forwardInput * speed * Time.deltaTime);

            // Steer the vehicle based on the horizontal input
            transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime);
        }
    }
}
