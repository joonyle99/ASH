using UnityEngine;

public class SwingFloor : MonoBehaviour
{
    [SerializeField] private Collider2D _floorCollider;
    [SerializeField] private HingeJoint2D _joint;
    [SerializeField] private float _recoverSpeed;

    private AudioSource _loopSound;

    private const float EPSILON = 0.1f;
    private bool _isPlayerContacted = false;

    private void Awake()
    {
        _loopSound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (_isPlayerContacted)
        {
            _joint.useMotor = false;
        }
        else
        {
            float angle = transform.rotation.eulerAngles.z;

            /*
            _joint.useMotor = true;
            var motorInfo = _joint.motor;
            if (angle > 180) 
                angle -= 360;
            motorInfo.motorSpeed = _recoverSpeed * (angle); 

            _joint.motor = motorInfo;
            */

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), _recoverSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerBehaviour>() != null)
        {
            _isPlayerContacted = true;
            _loopSound.Play();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = Utils.Collisions.GetCollidedComponent<PlayerBehaviour>(_floorCollider);
        if (player == null)
        {
            _isPlayerContacted = false;
            _loopSound.Stop();
        }
    }
}
