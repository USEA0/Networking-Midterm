using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    //impulse
    public Queue<Vector2> I;
    //acceleration
    public Vector2 A;
    //velocity
    public Vector2 V;

    //drag
    public float drag;

    // Update is called once per frame
    void FixedUpdate()
    {
        //velocity calculation
        V = (V * (1 - drag)) + A;

        //add impulse
        foreach (Vector2 impulse in I) {
            V += impulse;
        }

        //reset impulse
        I.Clear();

        //update position
        this.transform.position = new Vector2(this.transform.position.x + V.x, this.transform.position.y + V.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered");

        //collision handling
        if (collision.tag == "HorizWall")
        {
            V.y = -V.y;
        }
        else if (collision.tag == "VertWall")
        {
            V.x = -V.x;
        }
        else if (collision.tag == "Player")
        {
            I.Enqueue((collision.transform.position - this.transform.position).normalized * collision.GetComponent<Paddle>().velo);
        }
        else if (collision.tag == "GoalR") {
            Debug.Log("Winner: Blue");
        }
        else if (collision.tag == "GoalR")
        {
            Debug.Log("Winner: Red");
        }

    }
}
