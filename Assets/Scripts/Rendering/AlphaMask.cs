using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlphaMask : ScriptableRendererFeature
{
    [SerializeField] Material material;
    AlphaMaskPass pass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    public override void Create()
    {
        pass = new AlphaMaskPass(material);
    }
}
