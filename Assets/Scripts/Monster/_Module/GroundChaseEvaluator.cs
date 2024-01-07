using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChaseEvaluator : MonoBehaviour
{
    [Header("Ground Chase Evaluator")]
    [Space]

    [SerializeField] private bool isGood = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }
}
