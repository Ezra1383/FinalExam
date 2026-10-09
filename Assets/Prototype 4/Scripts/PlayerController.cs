using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public bool hasPowerup;
    public GameObject powerupIndicator;

    private Rigidbody playerRb;
    private GameObject focalPoint;

    private float powerupStrength = 15f;
    private Vector3 indicatorOffset = new Vector3(0, -0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        // roll in whichever direction the camera is currently facing
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        // keep the powerup indicator sitting underneath the player
        if (powerupIndicator != null)
        {
            powerupIndicator.transform.position = transform.position + indicatorOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            if (powerupIndicator != null)
            {
                powerupIndicator.SetActive(true);
            }

            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    // Takes the powerup away again after a few seconds
    private IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);

        hasPowerup = false;

        if (powerupIndicator != null)
        {
            powerupIndicator.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            Debug.Log("Player collided with " + collision.gameObject.name
                      + " with powerup set to " + hasPowerup);

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }
}
