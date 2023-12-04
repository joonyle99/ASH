using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(DestructEventCaller))]
public class EnableParticlesOnDestruct : MonoBehaviour, IDestructionListener
{
    [SerializeField] bool _setRotationToIdentity = true;

    [ContextMenuItem("Get all children", "SetParticlesContextMenu")]
    [SerializeField] ParticleHelper[] _particles;

    void SetParticlesContextMenu()
    {
        _particles = GetComponentsInChildren<ParticleHelper>(true);
#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
    public void SetEmissionRotations(Vector3 worldRotation)
    {
        foreach (ParticleHelper particle in _particles)
            particle.SetEmissionRotation(worldRotation);
    }
    public void AddEmissionRotations(float degree)
    {
        foreach (ParticleHelper particle in _particles)
            particle.AddEmissionRotation(degree);
    }
    public void OnDestruction()
    {
        foreach (ParticleHelper particle in _particles)
        {
            particle.Activate();
            particle.transform.parent = null;
            if (_setRotationToIdentity)
                particle.transform.rotation = Quaternion.identity;
        }
    }
}
