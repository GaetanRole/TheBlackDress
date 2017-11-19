using UnityEngine;

public class TurnAnimation : MonoBehaviour {

    public enum State   { Middle, Left, Right };
    public State        pb_direction = State.Middle;

    // The target where animation will end
    public Transform    pb_targetEndAnimationPosition;

    private Vector3     _cameraTargetEndAnimation;
    private Vector3     _melodyTargetEndAnimation;
    private Vector3     _camera_rotation_affect;
    private Vector3     _melody_rotation_affect;
    private bool        _enable = false;
    private float       _angleY, _limit = 1.50f;

    void Start() {

        // New Vector3 based on initial CameraController's eulerAngles
        _camera_rotation_affect = new Vector3(CameraController.instance.transform.eulerAngles.x, CameraController.instance.transform.eulerAngles.y, CameraController.instance.transform.eulerAngles.z);

        // New Vector3 based on initial MelodyController's eulerAngles
        _melody_rotation_affect = new Vector3(MelodyController.instance.transform.eulerAngles.x, MelodyController.instance.transform.eulerAngles.y, MelodyController.instance.transform.eulerAngles.z);
    }

    void Update() {

        // Stop script if enable = false
        if (!_enable)
            return;

        _chooseAnimation();
    }

    // Link to Left/Right/Middle Turn Animation object
    void OnTriggerEnter(Collider _col) {
        if (_col.tag == "Melody") {
            _initEndTargetAnimation();

            // Send the end animation position to CameraController
            CameraController.instance.setTargetWalkPosition(pb_targetEndAnimationPosition.position);
            _scriptsHandler(false);
        }
    }

    private void _initEndTargetAnimation() {

        // Have to create Vector3 to modify transform.position
        Vector3 _camera_transform_affect;
        _camera_transform_affect = pb_targetEndAnimationPosition.position;
        _camera_transform_affect.y = pb_targetEndAnimationPosition.position.y + CameraController.instance.pb_camera_offset_y;

        // Proper way to add camera_offset on each axis
        if (MelodyController.instance.getIsForwardOnZ()) {
            if (pb_direction == State.Left)
                _camera_transform_affect.x = pb_targetEndAnimationPosition.position.x + Mathf.Abs(CameraController.instance.pb_camera_offset_z);
            else if (pb_direction == State.Right)
                _camera_transform_affect.x = pb_targetEndAnimationPosition.position.x - Mathf.Abs(CameraController.instance.pb_camera_offset_z);
        }
        else {
            _camera_transform_affect.x = pb_targetEndAnimationPosition.position.x;
            _camera_transform_affect.z = pb_targetEndAnimationPosition.position.z - Mathf.Abs(CameraController.instance.pb_camera_offset_z);
        }
        _cameraTargetEndAnimation = _camera_transform_affect;
        _melodyTargetEndAnimation = pb_targetEndAnimationPosition.position;
    }

    private void _scriptsHandler(bool _state) {

        // Disable Melody and Camera scripts to play animation
        if (!_state) {
            _enable = true;
            MelodyController.instance.setEnable(false);
            CameraController.instance.setEnable(false);
        }
        else {
            _enable = false;
            MelodyController.instance.setEnable(true);
            CameraController.instance.setEnable(true);
        }
    }

    private void _chooseAnimation() {
        switch (pb_direction) {
            case State.Middle:
                break;
            case State.Left:
                _turnAnimationWithAngle(-90.00f);
                break;
            case State.Right:
                _turnAnimationWithAngle(90.00f);
                break;
        }
    }

    private void _turnAnimationWithAngle(float _degree) {

        // If Melody walks on Z, we don't need _degree
        if (!MelodyController.instance.getIsForwardOnZ())
            _degree = 0.00f;

        // Launch animation based on _degrees
        _rotateAnimation(CameraController.instance.transform, _cameraTargetEndAnimation, _camera_rotation_affect, _degree);
        _rotateAnimation(MelodyController.instance.transform, _melodyTargetEndAnimation, _melody_rotation_affect, _degree);
        if (_isAnimationFinished(_degree)) {

            // Round y eulerAngle for a perfect direction / rotation angle and set Melody's position on axis
            CameraController.instance.transform.rotation = Quaternion.Euler(_camera_rotation_affect.x, Mathf.Round(_camera_rotation_affect.y + _degree), _camera_rotation_affect.z);
            MelodyController.instance.transform.rotation = Quaternion.Euler(_melody_rotation_affect.x, Mathf.Round(_melody_rotation_affect.y + _degree), _melody_rotation_affect.z);
            MelodyController.instance.setIsForwardOnZ();
            MelodyController.instance.setIsForwardOnX(_degree);
        }
    }

    private void _rotateAnimation(Transform objectTransform, Vector3 _targetEndAnimationPosition, Vector3 _targetEndAnimationRotation, float _degree) {
        objectTransform.position = Vector3.Lerp(objectTransform.position, _targetEndAnimationPosition, Time.deltaTime);
        objectTransform.rotation = Quaternion.Lerp(objectTransform.rotation, Quaternion.Euler(_targetEndAnimationRotation.x, _targetEndAnimationRotation.y + _degree, _targetEndAnimationRotation.z), Time.deltaTime);
    }

    private bool _isAnimationFinished(float _degree) {
        _angleY = CameraController.instance.transform.eulerAngles.y;
        _angleY = Mathf.Abs((_angleY > 180) ? _angleY - 360 : _angleY);
        if (Mathf.Abs(_angleY - Mathf.Abs(_degree)) <= _limit) {
            
            // Animation is complete, stoping update function
            _scriptsHandler(true);
            return true;
        }
        return false;
    }
}