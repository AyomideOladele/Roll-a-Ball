using System.Collections;
using System . Collections . Generic ;
using UnityEngine ;
using UnityEngine . InputSystem ;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveValue;
    public float speed;

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(speed * Time.fixedDeltaTime * movement);
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