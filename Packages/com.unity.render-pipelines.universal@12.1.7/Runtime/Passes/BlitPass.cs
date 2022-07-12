namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class BlitPass : ScriptableRenderPass
    {
        Material m_BlitMaterial;

        private int m_Width;
        private int m_Height;
        private BlitColorTransform m_BlitColorTransform;

        public enum BlitColorTransform
        {
            None,
            Gamma2Line,
            Line2Gamma
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPass(RenderPassEvent evt, Material blitMaterial)
        {
            base.profilingSampler = new ProfilingSampler(nameof(BlitPass));

            m_BlitMaterial = blitMaterial;
            renderPassEvent = evt;
            base.useNativeRenderPass = false;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(int width, int height, BlitColorTransform blitColorTransform)
        {
            m_Width = width;
            m_Height = height;
            m_BlitColorTransform = blitColorTransform;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_BlitMaterial == null)
            {
                Debug.LogErrorFormat(
                    "Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.",
                    m_BlitMaterial, GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            if (renderingData.cameraData.renderer is not UniversalRenderer renderer)
                return;
            
            var colorBuffer = renderer.m_ColorBufferSystem;
            var renderTextureDescriptor = RenderTargetBufferSystem.GetDesc();
            bool needChangeSize = renderTextureDescriptor.width != m_Width ||
                                  renderTextureDescriptor.height != m_Height;
            if (needChangeSize)
            {
                colorBuffer.ReSizeFrontBuffer(cmd, m_Width, m_Height);
            }

            bool useDrawProceduleBlit = renderingData.cameraData.xr.enabled;
            if (m_BlitColorTransform == BlitColorTransform.Gamma2Line)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, true);
            }
            else if (m_BlitColorTransform == BlitColorTransform.Line2Gamma)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, true);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, false);
            }

            RenderTargetIdentifier source;
            source = renderingData.cameraData.renderer.cameraColorTarget;
            RenderingUtils.Blit(cmd, source, colorBuffer.GetFrontBuffer().id, m_BlitMaterial, 0, useDrawProceduleBlit);
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, false);
            if (needChangeSize)
            {
                colorBuffer.ReSizeBackBufferAndSave(cmd, m_Width, m_Height);
                renderer.ResizeDepth(cmd, ref renderTextureDescriptor, m_Width, m_Height);
            }

            renderer.SwapColorBuffer(cmd);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}