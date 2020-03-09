/*
Names:
John Wang - 100657681
Boris Au - 100660279
*/

using UnityEngine;

public class Paddle : MonoBehaviour
{
    public bool isPlayer = false;
    public int playerNum = 0;

    private Vector2 prevPos;
    public Vector2 velo;

    // Update is called once per frame
    void Update()
    {
        if (isPlayer) {

            Vector3 pos = new Vector3(0,0,0);

            //boundaries on y
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -3.5f)
            {
                pos.y = -3.5f;
            }
            else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > 3.5f)
            {
                pos.y = 3.5f;
            }
            else {
                pos.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            }
            //boundaries on x
            if (playerNum == 1)
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > -0.5)
                {
                    pos.x = -0.5f;
                }
                else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < -7.75f)
                {
                    pos.x = -7.75f;
                }
                else
                {
                    pos.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                }
            }
            else {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < 0.5)
                {
                    pos.x = 0.5f;
                }
                else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 7.75f)
                {
                    pos.x = 7.75f;
                }
                else
                {
                    pos.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                }
            }
            this.transform.position = pos;
            
            Vector2 pos2D = new Vector2(pos.x, pos.y);

            //update velocity and position
            velo = (prevPos - pos2D);
            prevPos = pos2D;
            
        }
    }
}
