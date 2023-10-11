using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveValue;
    public float speed;
    private int count;
    private int numPickUps = 5; // Must equals number of pickUp objects in hierarchy/scene/game
    private float ClosestPickUp;
    private GameObject ClosestPickUpLocation;
    private GameObject[]PickUp;
    private Vector3 OldPosition;
    private Vector3 Position;
    private float minDistance;
    private Color PickUpColour;
    private string mode;
    private LineRenderer lineRenderer;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI playersPositionText;
    public TextMeshProUGUI playersVelocityText;
    public TextMeshProUGUI closestPickupText;

    void Start()
    {
        OldPosition = transform.position;
        count = 0;
        winText.text = "";
        ClosestPickUp = 1000;
        PickUp = GameObject.FindGameObjectsWithTag("PickUp");
        PickUpColour = new Vector4(1f, 0.7129012f, 0f);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false;
        playersPositionText.text = "";
        playersVelocityText.text = "";
        closestPickupText.text = "";
        SetCountText();
    }

    private enum Debugmode
    {
        Normal,
        Distance,
        Vision
    }

    private Debugmode debugmode;

    void OnSwapMode()
    {
        if (debugmode == Debugmode.Normal)
        {
            debugmode = Debugmode.Distance;
            playersPositionText.enabled = true;
            playersVelocityText.enabled = true;
            closestPickupText.enabled = true;
            lineRenderer.enabled = true;
        }
        else if (debugmode == Debugmode.Distance)
        {
            debugmode = Debugmode.Vision;
            playersPositionText.enabled = false;
            playersVelocityText.enabled = false;
            closestPickupText.enabled = false;
        }
        else if (debugmode == Debugmode.Vision)
        {
            debugmode = Debugmode.Normal;
            lineRenderer.enabled = false;
        }
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < PickUp.Length; i++)
        {
            PickUp[i].GetComponent<Renderer>().material.color = PickUpColour;
        }
        Position = transform.position;
        ClosestPickUp = 1000;
        if (debugmode == Debugmode.Distance)
        {
            SetPlayersPosition();
            SetPlayersVelocity();
            GetClosestTarget();
        }
        else if(debugmode == Debugmode.Vision)
        {
            mode3();
        }
        OldPosition = transform.position;
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void SetCountText()
    {
        scoreText.text = "Score: " + count.ToString();
        if (count >= numPickUps)
        {
            winText.text = "You win!";
        }
    }

    private void SetPlayersPosition()
    {
        playersPositionText.text = "Players Position: " + gameObject.transform.position.ToString();
    }

    private void SetPlayersVelocity()
    {
        playersVelocityText.text = "Players Velocity: " + ((Position - OldPosition) / Time.deltaTime).magnitude.ToString("0.00");
    }

    private void GetClosestTarget()
    {
        for (int i = 0; i < PickUp.Length; i++)
        {
            if ((PickUp[i].transform.position-gameObject.transform.position).magnitude < ClosestPickUp)
            {
                if (PickUp[i].activeInHierarchy)
                {
                    ClosestPickUp = (PickUp[i].transform.position - gameObject.transform.position).magnitude;
                    ClosestPickUpLocation = PickUp[i];
                }
            }
        }
        closestPickupText.text = "Distance to Closest Pick up: "+ ClosestPickUp.ToString("0.00");
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, ClosestPickUpLocation.transform.position);
        ClosestPickUpLocation.GetComponent<Renderer>().material.color = Color.blue;
    }

    private float CountDistance(Vector3 d1, Vector3 d2)
    {
        float distance = 0;
        distance = (float)Math.Pow(Math.Pow((d1.x - d2.x), 2) + Math.Pow((d1.z - d2.z),2),0.5);
        return distance;
    }

    private float CountDistance2(Vector3 d1, Vector3 d2)
    {
        float distance = 0;
        distance = (float)(Math.Pow((d1.x * d2.z - d2.x * d1.z), 0.5) / Math.Pow(Math.Pow(d1.x, 2) + Math.Pow(d1.z, 2), 0.5));
        return distance;
    }

    private void mode3()
    {
        Vector3 towards = (transform.position - OldPosition) * 50;

        minDistance = 999999;
        int min = 9;
        for (int n=0; n < PickUp.Length; n++)
        {
            if (PickUp[n].activeInHierarchy)
            {
                if (CountDistance2(towards, PickUp[n].transform.position - transform.position) <= minDistance && (towards.x * (PickUp[n].transform.position - transform.position).x + towards.z * (PickUp[n].transform.position - transform.position).z) >= 0)
                {
                    minDistance = CountDistance2(towards, PickUp[n].transform.position - transform.position);
                    min = n;
                }
            }
        }

        for (int n=0;n < PickUp.Length;n++)
        {
            if (n == min)
            {
                PickUp[n].GetComponent<Renderer>().material.color = Color.blue;
                PickUp[n].transform.LookAt(transform.position);
            }
            else
            {
                PickUp[n].GetComponent<Renderer>().material.color = PickUpColour;
            }
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + towards);

    }

}


/*
    Code explanation from Lab 2
  
   - A public variable speed.
   - A public variable moveValue of type Vector2 (2-dimensional vector).
   - We capture the input in the ”OnMove” function, automatically called by Unity whenever a ”Move” action
   - is executed by the player (name corresponds to action defined in the input actions asset). Here, we simply
   - save the value received as input. Input is WASD or arrow keys.
   - We are coding in the FixedUpdate() function, as the operations involve physics.
   - We form a vector with the direction the ball should move, according to input received.
   - We are moving the player by applying a physics force. This force is composed of the direction (movement),
   - multiplied by a speed and by Time.fixedDeltaTime. This indicates the interval in seconds at which physics
   - and other fixed rate updates are performed. Including this assures a slower movement (try to remove it
   - and see what happens!).
   - You can check the meaning of any method by hovering your mouse over them
 */