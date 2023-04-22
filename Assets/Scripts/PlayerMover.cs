using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D rb; // The Rigidbody component of the object
    public LayerMask wallLayer; // The layer to detect collisions with
    public LayerMask flagLayer; // The layer to detect collisions with

    public int speed = 5;
    public KeyCode upKey = KeyCode.UpArrow; // The key to move up
    public KeyCode downKey = KeyCode.DownArrow; // The key to move down
    public KeyCode leftKey = KeyCode.LeftArrow; // The key to move left
    public KeyCode rightKey = KeyCode.RightArrow;   // The key to move right

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0)
            PlayerX(horizontalInput);
        else if (verticalInput != 0)
            PlayerY(verticalInput);
        else
            rb.velocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (wallLayer == (wallLayer | (1 << collision.gameObject.layer)))
        {
            // Stop the player from moving when they collide with a wall
            rb.velocity = Vector2.zero;
        }
        if (flagLayer == (flagLayer | (1 << collision.gameObject.layer)))
        {
            // Stop the player from moving when they collide with a wall
            speed = 100;
            Invoke("Done", 2f);

        }

    }
    void Done()
    {
        NextLevel.Next();
    }
    void PlayerX(float w)
    {
        rb.velocity = new Vector2(w * speed, 0f);
    }

    void PlayerY(float w)
    {
        rb.velocity = new Vector2(0f, w * speed);
    }
}
