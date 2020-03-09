using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Puck : MonoBehaviour
{
    //impulse
    public Queue<Vector2> I = new Queue<Vector2>();
    //acceleration
    public Vector2 A;
    //velocity
    public Vector2 V;

    public Text Score;
    public int scoreR = 0;
    public int scoreB = 0;
    public bool ScoreFlag = false;

    public float maxSpeed;
    public float minSpeed;
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

        if (V.magnitude > maxSpeed) {
            V = V.normalized * maxSpeed;
        }
        if (V.magnitude < minSpeed)
        {
            V = new Vector2(0, 0);
        }
            //reset impulse
            I.Clear();

        //update position
        this.transform.position = new Vector2(this.transform.position.x + V.x, this.transform.position.y + V.y);
    }

    private void Update()
    {
        if (ScoreFlag) {
            Score.text = scoreR.ToString() + "-" + scoreB.ToString();
            ScoreFlag = false;
        }
    }

    public void ResetPosition() {
        this.transform.position = new Vector2(0, 0);
        I.Clear();
        A = new Vector2(0, 0);
        V = new Vector2(0, 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //collision handling
        if (collision.gameObject.tag == "HorizWall")
        {
            V.y = -V.y;
            if (this.transform.position.y < -3.8f)
            {
                this.transform.position = new Vector2(this.transform.position.x, -3.8f);
            }
            else if (this.transform.position.y > 3.75f)
            {
                this.transform.position = new Vector2(this.transform.position.x, 3.75f);
            }

        }
        else if (collision.gameObject.tag == "VertWall")
        {
            V.x = -V.x;
            if (this.transform.position.x < -7.9f)
            {
                this.transform.position = new Vector2( -7.9f, this.transform.position.y);
            }
            else if (this.transform.position.x > 7.9f)
            {
                this.transform.position = new Vector2( 7.9f, this.transform.position.y);
            }

        }
        else if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Paddle>().isPlayer)
            {
                Debug.Log("Triggered");

                //calculate impulse
                Vector2 impulseTemp = -collision.gameObject.GetComponent<Paddle>().velo;

                //queue impulse locally
                I.Enqueue(impulseTemp);

                //send to the network
                NetworkManager.SendImpulse(impulseTemp);
            }
            //manual collision
            float dx = this.transform.position.x - collision.transform.position.x;
            float dy = this.transform.position.y - collision.transform.position.y;

            var distance = Mathf.Sqrt(dx * dx + dy * dy);

            if (distance < this.transform.localScale.y + collision.transform.localScale.y)
            {
                Vector3 dir = -((collision.transform.position - this.transform.position).normalized);
                float nDist = (this.transform.localScale.y + collision.transform.localScale.y - distance);

                this.transform.position = this.transform.position + (dir * (nDist + 0.01f));
            }
        }
        else if (collision.gameObject.tag == "GoalR")
        {
            ResetPosition();
            //add score
            scoreB++;
            ScoreFlag = true;

            Debug.Log("Winner: Blue");
        }
        else if (collision.gameObject.tag == "GoalB")
        {
            ResetPosition();
            //add score
            scoreR++;
            ScoreFlag = true;

            Debug.Log("Winner: Red");
        }

    }
}
