using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    //impulse
    public Queue<Vector2> I = new Queue<Vector2>();
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

        //collision handling
        if (collision.gameObject.tag == "HorizWall")
        {
            V.y = -V.y;
        }
        else if (collision.gameObject.tag == "VertWall")
        {
            V.x = -V.x;
        }
        else if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Triggered");

            if (!collision.gameObject.GetComponent<Paddle>().isPlayer)
            {

                //calculate impulse
                Vector2 impulseTemp = (collision.transform.position - this.transform.position).normalized * collision.gameObject.GetComponent<Paddle>().velo;

                //queue impulse locally
                I.Enqueue(impulseTemp);

                //send to the network
                NetworkManager.SendImpulse(impulseTemp);
            }

        }
        else if (collision.gameObject.tag == "GoalR")
        {
            Debug.Log("Winner: Blue");
        }
        else if (collision.gameObject.tag == "GoalR")
        {
            Debug.Log("Winner: Red");
        }

    }

    /*
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //manual collision
            float dx = this.transform.position.x - collision.transform.position.x;
            float dy = this.transform.position.y - collision.transform.position.y;

            var distance = Mathf.Sqrt(dx * dx + dy * dy);

            if (distance < this.transform.localScale.y + collision.transform.localScale.y)
            {
                Vector3 dir = -((collision.transform.position - this.transform.position).normalized);
                float nDist = (this.transform.localScale.y + collision.transform.localScale.y - distance);

                this.transform.position = this.transform.position + (dir * (nDist+0.01f));
            }

        }
    }
    */
}
