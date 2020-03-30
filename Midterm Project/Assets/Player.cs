using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isControllable = false;

    //manual physics
    public static Vector2 boundaries = new Vector2(8, 4);

    //velocity
    public float vx = 0;
    public float vy = 0;

    public float speed = 1.0f;

    [Range(0.0f, 1.0f)]
    public float drag = 0.95f;



    //server pos update
    public float px = 0;
    public float py = 0;
    public bool updated = false;
    public bool moved = false;
    [Range(0.0f, 1.0f)]
    public float deadReckoningFalloff = 0.95f;

    private void FixedUpdate()
    {
        if (isControllable)
        {
            float ax = 0;
            float ay = 0;

            //apply acceleration
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                ay += speed;
                moved = true;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                ay -= speed;
                moved = true;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                ax -= speed;
                moved = true;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                ax += speed;
                moved = true;
            }


            //update physics
            vx = (vx * drag) + ax;
            vy = (vy * drag) + ay;

            transform.position = new Vector2(transform.position.x + vx, transform.position.y + vy);        
        }
        else { 

            //dead-reckoning
            if (updated) {
                transform.position = new Vector2(px, py);
                updated = false;
            }
            else
            {
                vx = deadReckoningFalloff * vx;
                vy = deadReckoningFalloff * vy;
                //time is automatically updated through Fixedupdate [P = Pk + V*T]
                transform.position = new Vector2(transform.position.x + vx, transform.position.y + vy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check boundaries
        if (transform.position.x > boundaries.x) { 
            transform.position = new Vector2(boundaries.x, transform.position.y);
        }
        else if (transform.position.x < -boundaries.x)
        {
            transform.position = new Vector2(-boundaries.x, transform.position.y);
        }

        if (transform.position.y > boundaries.y)
        {
            transform.position = new Vector2(transform.position.x, boundaries.y);
        }
        else if (transform.position.y < -boundaries.y)
        {
            transform.position = new Vector2(transform.position.x, -boundaries.y);
        }

    }
}
