using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody enemyRb;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // chase the player across the island
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed);

        // clean up any enemy that gets knocked off the edge
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
