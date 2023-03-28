using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StateMahineDemo.PlayerBehaviour;

public class PlayerInput : MonoBehaviour
{
    public Inputs _inputs;

    #region Inputs

    public bool _facingLeft;

    private void GatherInput()
    {
        _inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
        _inputs.RawY = (int)Input.GetAxisRaw("Vertical");
        _inputs.X = Input.GetAxis("Horizontal");
        _inputs.Y = Input.GetAxis("Vertical");

        _facingLeft = _inputs.RawX != 1 && (_inputs.RawX == -1 || _facingLeft);
        SetFacingDirection(_facingLeft);
    }

    private void SetFacingDirection(bool left)
    {
        this.transform.localScale = new Vector3(left ? -1 : 1, transform.localScale.y, transform.localScale.z);
        //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        GatherInput();
    }

    [Serializable]
    public struct Inputs
    {
        public float X, Y;
        public int RawX, RawY;
    }
}
