using UnityEngine;

public class CrashableTree : MonoBehaviour
{
    [SerializeField] private FallingTreeTrunk _treeTrunk;

    private PreserveState _statePreserver;

    private void Awake()
    {
        // Debug.Log("pushable tree awake");

        _statePreserver = GetComponent<PreserveState>();

        if (_statePreserver)
        {
            var treeTransform = new TransformState(_treeTrunk.transform);
            var newTreeTransform = _statePreserver.LoadState("_isFallingTreeTransform", treeTransform);
            _treeTrunk.transform.localPosition = newTreeTransform.Position;
            _treeTrunk.transform.localRotation = newTreeTransform.Rotation;
            _treeTrunk.transform.localScale = newTreeTransform.Scale;
        }
    }


    private void OnDestroy()
    {
        if (_statePreserver)
        {
            // falling tree의 데이터를 저장한다.
            _statePreserver.SaveState("_isFallingTreeTransform", new TransformState(_treeTrunk.transform));
        }
    }
}
