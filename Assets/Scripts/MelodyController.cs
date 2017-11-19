using UnityEngine;

public class MelodyController : MonoBehaviour {

    struct IRFPos                   { public bool Left, Right; }
    IRFPos                          _position;

    public static MelodyController  instance;
    public float                    pb_speed = 5.00f;

    private CharacterController     _controller;
    private Vector3                 _moveVector;
    private float                   _direction = 0.00f;
    private float                   _verticalVelocity = 0.00f;
    private float                   _gravity = 12.00f;
    private bool                    _enable = true;
    private bool                    _isForwardOnZ = true;
    private bool                    _isForwardOnX = true;

    void Awake() {

        // Allow other classes to get this instance
        instance = this;
    }

    void Start() {
        _controller = GetComponent<CharacterController>();

        // Initialize Melody's permission to walk away
        _position.Left = false;
        _position.Right = false;

        // Affect Melody's velocity at the beginning
        _verticalVelocity = -0.50f;
        _moveVector.y = _verticalVelocity;
    }
	
	void Update() {

        // Stop script if enable = false
        if (!_enable)
            return;

        _melodyHandler();
	}

    public  void setEnable(bool pb_state) {
        _enable = pb_state;
    }

    public  void setIsForwardOnZ() {

        // Don't need a degree because we always go forward
        _isForwardOnZ = !_isForwardOnZ;
        _position.Left = false;
        _position.Right = false;
    }

    public  void setIsForwardOnX(float _degree) {

        // Need a degree to know if we go left or right
        _isForwardOnX = _degree > 0.00f ? true : false;
        _position.Left = false;
        _position.Right = false;
    }

    public  bool getIsForwardOnZ() {
        return _isForwardOnZ;
    }

    public  bool getIsForwardOnX() {
        return _isForwardOnX;
    }

    public  void limitLeftSide(bool _authorization) {
        _position.Left = _authorization;
        if (!_authorization) {
            _direction = -1.00f;
            _position.Right = _authorization;
        }
    }

    public  void limitRightSide(bool _authorization) {
        _position.Right = _authorization;
        if (!_authorization) {
            _direction = 1.00f;
            _position.Left = _authorization;
        }
    }

    private void _melodyHandler() {

        // Reset moveVector all frames
        _moveVector = Vector3.zero;

        // Affect same Melody's velocity on all devices
        if (_controller.isGrounded)
            _verticalVelocity = -0.50f;
        else
            _verticalVelocity -= (_gravity * Time.deltaTime);

        // Take a look on permission before allow Melody to walk
        if (Input.GetAxisRaw("Horizontal") == -1 && !(_position.Left))
            limitLeftSide(false);
        else if (Input.GetAxisRaw("Horizontal") == 1 && !(_position.Right))
            limitRightSide(false);

        // Melody walks on different axis
        _moveVector.x = _directionHandlerOnX(_direction);
        _moveVector.z = _directionHandlerOnZ(_direction);

        // Fall action
        _moveVector.y = _verticalVelocity;

        // Move Melody's controller * deltaTime
        _controller.Move(_moveVector * Time.deltaTime);

        // Disabled this line if you want infinite walk until collision detection
        _direction = 0.00f;
    }

    private float _directionHandlerOnX(float _direction) {

        // Keep direction input during frames rendering and affect object's vector.x
        if (_isForwardOnZ)
            return (_direction * pb_speed);
        else
            return _isForwardOnX ? pb_speed : ((-1) * pb_speed);
    }

    private float _directionHandlerOnZ(float _direction) {
        if (_isForwardOnZ)
            return ((Input.GetAxisRaw("Vertical") < 1) ? pb_speed : (Input.GetAxisRaw("Vertical") * 50.00f));
        else if (_isForwardOnX) // Z negative axis to walk right
            return (((-1) * (_direction)) * pb_speed);
        else // Z positive axis to walk left
            return (_direction * pb_speed);
    }

    /* GetAllChildrenFromAnObject
    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
    */
}
