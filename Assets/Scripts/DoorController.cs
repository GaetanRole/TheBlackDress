using UnityEngine;

public class DoorController : MonoBehaviour {

    // Link to Door_Opener and link Door_a as a GameObject
    public GameObject Door;

    public bool pb_haveToOpen = true;

    void OnTriggerEnter(Collider _col) {

        // Door animation when Melody's collision is triggered
        if (pb_haveToOpen) {
            Door.GetComponent<Animation>().Play();
            pb_haveToOpen = false;
        }
    }
}