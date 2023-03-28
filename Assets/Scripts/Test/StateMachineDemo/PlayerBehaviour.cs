using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace StateMahineDemo
{

    public class PlayerBehaviour : StateMachineBase
    {
        [SerializeField] Rigidbody2D _rb;
        [SerializeField] int _attackPower;

        public Rigidbody2D Rigidbody2D { get { return _rb; } }
        public int AttackPower { get { return _attackPower; } }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

    }
}
