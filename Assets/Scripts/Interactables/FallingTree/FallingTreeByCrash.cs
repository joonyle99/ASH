using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ���� ���� �ı��Ǹ� �������� ����
/// </summary>
public class FallingTreeByCrash : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;

    [SerializeField] private float _fallingAngle;
    [SerializeField] private float _rotatedAngle;

    [SerializeField] private bool _isOnceCrashed;
    [SerializeField] private bool _isFallingEnd;
    [SerializeField] private float _pushDir;

    private Rigidbody2D _rigid;

    private Quaternion _startRotation;
    private Quaternion _curRotation;

    private bool _isChangedLayer;

    [SerializeField] SoundList _soundList;

    bool _isFallingSoundPlayed;
    bool _isLandingSoundPlayed;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        _startRotation = this.transform.rotation;
    }

    void Update()
    {
        // update current rotation
        _curRotation = this.transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(_startRotation, _curRotation);

        // falling down tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
        {
            _isFallingEnd = true;

            if (!_isFallingSoundPlayed)
            {
                _isFallingSoundPlayed = true;
                _soundList.PlaySFX("SE_FallingTree_Break");
            }
        }

        // ������ �������� Ÿ�ֿ̹� ���̾ �ѹ��� �ٲ��ش�.
        if (!_isChangedLayer && _isFallingEnd)
        {
            _isChangedLayer = true;
            ChangeLayer();
        }
    }

    public void ExcuteCrash()
    {
        _soundList.PlaySFX("SE_FallingTree_Collision");
    }

    private int ChangeToIndex(int v)
    {
        int value = 1;
        int index = 0;

        while (v != value)
        {
            value <<= 1;
            index++;
        }

        return index;
    }

    private void ChangeLayer()
    {
        Transform parent = this.transform.parent;

        parent.transform.GetChild(0).gameObject.layer = ChangeToIndex(_targetLayerMask.value);
        parent.transform.GetChild(1).gameObject.layer = ChangeToIndex(_targetLayerMask.value);

        _isChangedLayer = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ū ���̶� �浹 �� ������
        if (!_isOnceCrashed && collision.gameObject.GetComponent<RollingStone>())
        {
            _isOnceCrashed = true;
            ExcuteCrash();
        }

        if (!_isLandingSoundPlayed)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _soundList.PlaySFX("SE_FallingTree_Landing");
                _isLandingSoundPlayed = true;
            }
        }
    }
}
