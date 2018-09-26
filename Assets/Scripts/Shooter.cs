using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    public Camera mainCamera;
    public Plane hitPlane;

    public GameObject newBallPosition;

    public float BallSpeed = 200.0f;

    GameObject newBall;
    int numberOfBallsCreated = 0;

    // Use this for initialization
    void Start () {
        hitPlane = new Plane(
            Vector3.zero, Vector3.up, Vector3.right);

        CreateNewBall();
	}

    Vector2 touchPos;

    // Update is called once per frame
    void Update()
    {
        if (newBall != null)
        {
            if (GetTouchPosition(out touchPos))
            {
                //Create a ray from the Mouse click position
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                //Initialise the enter variable
                float enter, angle;

                if (hitPlane.Raycast(ray, out enter))
                {
                    Vector3 mouse_pos = ray.GetPoint(enter);
                    Vector3 object_pos = transform.position;
                    mouse_pos.x = mouse_pos.x - object_pos.x;
                    mouse_pos.y = mouse_pos.y - object_pos.y;
                    angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                }

                newBall.transform.position = newBallPosition.transform.position;
            }
            if (TouchUp(out touchPos))
            {
                Vector3 force = (newBall.transform.position - transform.position) * BallSpeed;
                Main.main.MessageManager.BallShot(newBall.transform.localPosition, force, newBall.GetComponent<Bubble>().ID, newBall.GetComponent<Bubble>().color);
                
                newBall.GetComponent<Rigidbody>().AddForce(force);
                newBall = null;
                StartCoroutine(CreateNewBallDelayed());
            }
        }

    }


    private bool GetTouchPosition(out Vector2 position)
    {
        position = Vector2.negativeInfinity;
        if (Input.touchCount > 0)// && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        if (Input.GetMouseButton(0))
        {
            position = Input.mousePosition;
            return true;
        }
        return false;
    }

    private bool TouchUp(out Vector2 position) {
        position = Vector2.negativeInfinity;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            position = Input.mousePosition;
            return true;
        }
        return false;
    }

    private IEnumerator CreateNewBallDelayed() {
        yield return new WaitForSeconds(0.5f);
        CreateNewBall();
    }

    private void CreateNewBall() {
        GameObject nextBall;
        if (numberOfBallsCreated < Main.main.NextBallsColors.Length)
        {
            nextBall = Main.main.BallsPerColor[Main.main.NextBallsColors[numberOfBallsCreated]].gameObject;
        }
        else
        {
            int ballIdx = Random.Range(0, Main.main.Balls.Length);
            nextBall = Main.main.Balls[ballIdx].gameObject;
        }
        GameObject created = Instantiate(nextBall, newBallPosition.transform.position, transform.rotation);
        created.GetComponent<Bubble>().ID = "id" + numberOfBallsCreated;
        created.transform.parent = Main.main.LocalMapHolder.transform;
        newBall = created;
        numberOfBallsCreated++;
    }
}
