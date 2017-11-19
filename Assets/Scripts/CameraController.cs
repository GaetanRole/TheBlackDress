using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController  instance;
    public float                    pb_camera_offset_y = 8.00f;
    public float                    pb_camera_offset_z = -10.00f;
    public float                    pb_max_pos_camera = 2.00f;
    public float                    pb_speed_of_rideup = 0.04f;

    private Transform               _lookAt;
    private Vector3                 _targetWalkPosition;
    private Vector3                 _offSet;
    private Vector3                 _moveVector;
    private float                   _max_posY_toReach;
    private bool                    _moveMaxPos = false;
    private bool                    _enable = true;

    void Awake() {

        // Allow other classes to get this instance
        instance = this;
    }

    void Start() {

        // Don't forget to add a <tag> in UnityGUI
        _lookAt = GameObject.FindGameObjectWithTag("Melody").transform;

        // Have to create Vector3 to modify transform.position
        Vector3 _transform_affect;
        _transform_affect   = transform.position;
        _transform_affect.y = _lookAt.position.y + pb_camera_offset_y;
        _transform_affect.z = _lookAt.position.z + pb_camera_offset_z;
        transform.position  = _transform_affect;

        // Set camera's max position and first player position at the beginning
        _max_posY_toReach   = (_lookAt.position.y + pb_camera_offset_y) + pb_max_pos_camera;

        // Set the first walking path for Camera and Inertial Reference Frame
        _targetWalkPosition = new Vector3(0.00f, 0.00f, 0.00f);
    }

    void Update() {

        // Stop script if enable = false
        if (!_enable)
            return;

        // Start camera after 3 frames to let vertical velocity affects Melody (player)
        if (Time.frameCount >= 3)
            _cameraHandler();
    }

    public  void setEnable(bool pb_state) {
        _enable = pb_state;
    }

    public  void setTargetWalkPosition(Vector3 pb_targetEndAnimationPosition) {
        _targetWalkPosition = pb_targetEndAnimationPosition;
    }

    public  void isMelodyOnPlane() {
        _moveMaxPos = true;
    }

    public  void isMelodyOnStairs() {
        _moveMaxPos = false;
    }

    private void _cameraHandler() {
        _moveVector = _lookAt.position;

        // Camera follows Melody (y axis) with an offset
        _moveVector.y += pb_camera_offset_y;

        // If Melody walks forward, camera don't move on X, else camera don't move on targetPosZ
        if (MelodyController.instance.getIsForwardOnZ()) {

            // The first _targetWalkPosition is a Vector3(0,0,0);
            _moveVector.x = _targetWalkPosition.x;
            _moveVector.z += pb_camera_offset_z;
        }
        else {
            _moveVector.x = MelodyController.instance.getIsForwardOnX() ? _lookAt.position.x + pb_camera_offset_z : _lookAt.position.x - pb_camera_offset_z;
            _moveVector.z = _targetWalkPosition.z;
        }

        // Clamp posY between player position & max positon in Y
        _moveVector.y = Mathf.Clamp(_moveVector.y, _lookAt.position.y + pb_camera_offset_y, _max_posY_toReach);

        // If max position has to be reset (moveMaxPos)
        if (_max_posY_toReach != _moveVector.y && _moveMaxPos == true) {
            _max_posY_toReach = (_lookAt.position.y + pb_camera_offset_y) + pb_max_pos_camera;
            _moveMaxPos = false;
        }

        // If Melody walks away stairs
        if (_moveVector.y == _max_posY_toReach && _moveMaxPos == true) {
            _moveVector.y = Mathf.Clamp(_moveVector.y, _moveVector.y, _max_posY_toReach + pb_camera_offset_y);
            _moveVector.y = _moveVector.y + pb_speed_of_rideup;
            _max_posY_toReach += pb_speed_of_rideup;
        }
       
        // Set camera position with calculated vector
        transform.position = _moveVector;
    }
}