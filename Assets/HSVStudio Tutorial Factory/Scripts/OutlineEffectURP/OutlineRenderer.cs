using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HSVStudio.Tutorial.Outline
{
    public class OutlineRenderer : ScriptableRendererFeature
    {
        public OutlineRendererSettings m_outlineSettings = new OutlineRendererSettings();
        private OutlineRendererPass m_scriptablePass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (OutlineEffectControllerURP.Instance != null && (renderingData.cameraData.camera.cullingMask & m_outlineSettings.LayerMask) != 0)
            {
                IOutlineRendererCache outlineRenderCache = OutlineEffectControllerURP.Instance.outlineCache;

                if (outlineRenderCache == null || outlineRenderCache.Renderers.Count == 0)
                {
                    return;
                }

                var src = renderer.cameraColorTarget;
                m_scriptablePass.Setup(src, outlineRenderCache);
                renderer.EnqueuePass(m_scriptablePass);
            }
        }

        public override void Create()
        {
            m_scriptablePass = new OutlineRendererPass();
            m_scriptablePass.Settings = m_outlineSettings;
            m_scriptablePass.renderPassEvent = m_outlineSettings.RenderPassEvent;
        }
    }

    [Serializable]
    public class OutlineRendererSettings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        //public float ObjectID = 0.5f;
        public LayerMask LayerMask = -1;
        public Material HighlightMaterial = null;
        public Color OutlineColor = Color.white;
        [Range(0.01f, 20f)]
        public float OutlineStength = 1.1f;

        [Range(0.01f, 10)]
        public float BlurSize = 3f;
    }

    public class OutlineRendererPass : ScriptableRenderPass
    {
        public OutlineRendererSettings Settings;
        private IOutlineRendererCache m_outlineRendererCache;
        private RenderTargetIdentifier m_ColorRT;

        private int m_prepassRTID;
        private RenderTargetIdentifier m_prepassRT;

        private int m_blurredRTID;
        private RenderTargetIdentifier m_blurredRT;

        private int m_tmpRTID;
        private RenderTargetIdentifier m_tmpRT;

        private int m_outlineColorId;
        private int m_outlineStrengthId;
        private int m_blurDirectionId;

        public void Setup(RenderTargetIdentifier colorRT, IOutlineRendererCache outlineRendererCache)
        {
            m_ColorRT = colorRT;
            m_outlineRendererCache = outlineRendererCache;
        }

        private RenderTextureDescriptor GetStereoCompatibleDescriptor(RenderTextureDescriptor descriptor, int width, int height, GraphicsFormat format, int depthBufferBits = 0)
        {
            // Inherit the VR setup from the camera descriptor
            var desc = descriptor;
            desc.depthBufferBits = depthBufferBits;
            desc.msaaSamples = 1;
            desc.width = width;
            desc.height = height;
            desc.graphicsFormat = format;
            return desc;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;

            m_prepassRTID = Shader.PropertyToID("_PrepassRT");
            m_blurredRTID = Shader.PropertyToID("_BlurredRT");
            m_tmpRTID = Shader.PropertyToID("_TmpRT");
            m_outlineColorId = Shader.PropertyToID("_OutlineColor");
            m_outlineStrengthId = Shader.PropertyToID("_OutlineStrength");
            m_blurDirectionId = Shader.PropertyToID("_BlurDirection");

            var desc = GetStereoCompatibleDescriptor(cameraTextureDescriptor, width, height, cameraTextureDescriptor.graphicsFormat);
            cmd.GetTemporaryRT(m_prepassRTID, desc);
            cmd.GetTemporaryRT(m_blurredRTID, desc);
            cmd.GetTemporaryRT(m_tmpRTID, desc);

            m_prepassRT = new RenderTargetIdentifier(m_prepassRTID);
            m_blurredRT = new RenderTargetIdentifier(m_blurredRTID);
            m_tmpRT = new RenderTargetIdentifier(m_tmpRTID);

            ConfigureTarget(m_prepassRT);
            ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 1));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("OutlineEffectURP");

            if (m_outlineRendererCache != null)
            {
                //cmd.SetGlobalFloat("_ObjectId", Settings.ObjectID);
                IList<Renderer> renderers = m_outlineRendererCache.Renderers;
                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer renderer = renderers[i];
                    if (renderer != null && renderer.enabled && renderer.gameObject.activeSelf)
                    {
                        Material[] materials = renderer.sharedMaterials;

                        for (int j = 0; j < materials.Length; ++j)
                        {
                            if (materials[j] != null)
                            {
                                cmd.DrawRenderer(renderer, Settings.HighlightMaterial, j, 1);
                                //cmd.DrawRenderer(renderer, Settings.HighlightMaterial, j, 0);
                            }
                        }
                    }
                }
            }

            //object ID and blur passes
            //cmd.Blit(m_prepassRT, m_blurredRT, Settings.HighlightMaterial, 3);
            cmd.Blit(m_prepassRT, m_blurredRT);
            cmd.SetGlobalFloat(m_outlineStrengthId, Settings.OutlineStength);
            cmd.SetGlobalVector(m_blurDirectionId, new Vector2(Settings.BlurSize, 0));
            cmd.Blit(m_blurredRT, m_tmpRT, Settings.HighlightMaterial, 2);
            cmd.SetGlobalVector(m_blurDirectionId, new Vector2(0, Settings.BlurSize));
            cmd.Blit(m_tmpRT, m_blurredRT, Settings.HighlightMaterial, 2);

            // copy screen color buffer
            cmd.Blit(m_ColorRT, m_tmpRT);
            cmd.SetGlobalTexture(m_prepassRTID, m_prepassRT);
            cmd.SetGlobalTexture(m_blurredRTID, m_blurredRT);
            cmd.SetGlobalColor(m_outlineColorId, Settings.OutlineColor);
            cmd.Blit(m_tmpRT, m_ColorRT, Settings.HighlightMaterial, 4);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_prepassRTID);
            cmd.ReleaseTemporaryRT(m_blurredRTID);
            cmd.ReleaseTemporaryRT(m_tmpRTID);
        }
    }
}