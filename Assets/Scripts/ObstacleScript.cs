using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;

    public bool isPill;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerScript>())
        {
            if (isPill)
            {
                collision.GetComponent<PlayerScript>().Boost();
                Destroy(gameObject.gameObject);
            }
            else
            {
                collision.GetComponent<PlayerScript>().Lose();
                Debug.Log(collision);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(0, -speed);
    }
}
