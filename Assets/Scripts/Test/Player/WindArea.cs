using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [SerializeField] bool _isStartActive = true;
    PreserveState _statePreserver;

/*    [SerializeField] private bool _isWorking = false;
    public float value = 15f;
    private GameObject _player = null;
    [SerializeField] vector windVector;
    private Vector2 _vector;

    enum vector
    {
        up,
        down,
        right,
        left
    }*/

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();

        if(_statePreserver != null)
            _isStartActive = _statePreserver.Load("isActive", gameObject.activeSelf);

        gameObject.SetActive(_isStartActive);
    }

    public void SetActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        _statePreserver.Save("isActive", gameObject.activeSelf);
    }

/*    private void Start()
    {
        switch (windVector)
        {
            case vector.up:
                _vector = Vector2.up;
                break;
            case vector.down:
                _vector = Vector2.down;
                break;
            case vector.left:
                _vector = Vector2.left;
                break;
            case vector.right:
                _vector = Vector2.right;
                break;
            default:
                _vector = Vector2.up;
                break;
        }
    }


    private void FixedUpdate()
    {
        if (_isWorking)
        {       
            _player.GetComponent<Rigidbody2D>().AddForce(_vector * value, ForceMode2D.Force);

            if(!_player.GetComponent<PlayerBehaviour>().CurrentStateIs<JumpState>())
            {
                _player.GetComponent<PlayerBehaviour>().ChangeState<JumpState>();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _player = other.gameObject;
            _isWorking = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isWorking = false;
        }
    }*/
}
