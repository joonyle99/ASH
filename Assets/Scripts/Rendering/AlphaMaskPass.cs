using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class AlphaMaskPass : ScriptableRenderPass
{
    RenderTargetIdentifier source;
    RenderTargetIdentifier destinationA;
    RenderTargetIdentifier destinationB;
    RenderTargetIdentifier lastDest;

    readonly int temporaryRTIdA = Shader.PropertyToID("_TempRTA");
    readonly int temporaryRTIdB = Shader.PropertyToID("_TempRTB");

    Material outlineMaterial;

    public AlphaMaskPass(Material material)
    {
        this.outlineMaterial = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var renderer = renderingData.cameraData.renderer;
        source = renderer.cameraColorTarget;

        cmd.GetTemporaryRT(temporaryRTIdA, descriptor, FilterMode.Bilinear);
        destinationA = new RenderTargetIdentifier(temporaryRTIdA);
        cmd.GetTemporaryRT(temporaryRTIdB, descriptor, FilterMode.Bilinear);
        destinationB = new RenderTargetIdentifier(temporaryRTIdB);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Alpha Mask");
        cmd.Clear();
        VolumeStack stack = VolumeManager.instance.stack;

        //Local Functions

        //Function that renders the screen using double buffer.
        void BlitTo(Material mat, int pass = 0)
        {
            RenderTargetIdentifier first = lastDest;
            RenderTargetIdentifier last = lastDest == destinationA ? destinationB : destinationA;
            Blit(cmd, first, last, mat, pass);
            lastDest = last;
        }

        lastDest = source;

        OutlineEffectComponent outlineEffect = stack.GetComponent<OutlineEffectComponent>();
        if (outlineEffect.IsActive())
        {
            Material material = outlineMaterial;
            material.SetFloat(Shader.PropertyToID("_OutlineWeight"), outlineEffect.outlineWeight.value);
            material.SetColor(Shader.PropertyToID("_OutlineColor"), outlineEffect.outlineColor.value);
            material.SetInteger(Shader.PropertyToID("_Scale"), outlineEffect.scale.value);
            material.SetFloat(Shader.PropertyToID("_DepthThreshold"), outlineEffect.depthThreshold.value);

            Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(renderingData.cameraData.GetProjectionMatrix(), true);
            material.SetMatrix("_ClipToView", clipToView);
            BlitTo(material);
        }

        Blit(cmd, lastDest, source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTIdA);
        cmd.ReleaseTemporaryRT(temporaryRTIdB);
    }
}