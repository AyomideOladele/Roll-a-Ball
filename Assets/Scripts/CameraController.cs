using System.Collections;
using System . Collections . Generic ;
using UnityEngine ;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position;
    }

    void LateUpdate()
    {
       transform.position = player.transform.position + offset;
    }
}


/*
    Code explanation from Lab 2

   - A public game object, the player, target of the camera.
   - A private offset, from the player to the camera.
   - We use Start() to initialize the offset.This takes the initial position of the camera (note that we can use
     this initialization because originally the player is at the origin).
   - We modify the position of the camera (through its transform, directly accessible through the variable
     transform), by adding the offset to the position of the player.
   - We are doing this on the LateUpdate() method.LateUpdate() is the appropriate place for follow cameras,
     procedural animations and gathering last known states.
*/