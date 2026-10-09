using UnityEngine;

namespace Prototype1
{
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject player;

        [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);

        // LateUpdate runs after the player has moved this frame
        void LateUpdate()
        {
            if (player == null)
            {
                return;
            }

            transform.position = player.transform.position + offset;
        }
    }
}
