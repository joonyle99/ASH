using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace StateMahineDemo
{

    public class PlayerBehaviour : StateMachineBase
    {
        [SerializeField] int _attackPower;

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
