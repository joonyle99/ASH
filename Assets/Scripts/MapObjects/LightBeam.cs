using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    LineRenderer _lineRenderer;

    LanternLike _startLantern;
    LanternLike _endLantern;

    public bool IsConnectedTo(Lantern lantern)
    {
        return _startLantern == lantern || _endLantern == lantern;
    }
    public void SetLanterns(LanternLike start, LanternLike end)
    {
        _startLantern = start;
        _endLantern = end;
    }
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    void ResetLinePositions()
    {
        if (_startLantern == null)
            return;
        _lineRenderer.SetPosition(0, _startLantern.transform.position);
        _lineRenderer.SetPosition(1, _endLantern.transform.position);
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetLinePositions();
        //TODO : Connecting animation
    }
    private void OnEnable()
    {
        ResetLinePositions();
    }
    // Update is called once per frame
    void Update()
    {
        ResetLinePositions();
    }
}
