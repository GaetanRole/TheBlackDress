using UnityEngine;

public class DetectCollision : MonoBehaviour {

    // Link to Melody
    void OnTriggerEnter(Collider _col) {

        // Don't forget to add tag on object (Unity GUI)
        switch (_col.tag) {
            case "Plane":
                CameraController.instance.isMelodyOnPlane();
                break;
            case "Stairs":
                CameraController.instance.isMelodyOnStairs();
                break;
            case "Left_IRF":
                MelodyController.instance.limitLeftSide(true);
                break;
            case "Right_IRF":
                MelodyController.instance.limitRightSide(true);
                break;
        }
    }
}