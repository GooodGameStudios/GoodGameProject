using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

// Rysuje outline w przestrzeni ekranu dla obiektow na warstwie outlineLayer -
// dziala niezaleznie od ksztaltu/kantow siatki (w przeciwienstwie do starej
// metody opartej na wypychaniu wierzcholkow wzdluz normalnych).
// Pass 1 (OutlineMaskPass): rysuje tylko obiekty z outlineLayer jako biala sylwetke do malej tekstury maski.
// Pass 2 (OutlineEdgePass): wykrywa krawedzie tej maski i rysuje outline na obraz kamery.
public class OutlineRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private LayerMask outlineLayer;
    [SerializeField] private Material maskMaterial;
    [SerializeField] private Material edgeMaterial;

    private OutlineMaskPass maskPass;
    private OutlineEdgePass edgePass;

    public override void Create()
    {
        maskPass = new OutlineMaskPass(outlineLayer, maskMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };
        edgePass = new OutlineEdgePass(edgeMaterial, maskPass)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (maskMaterial == null || edgeMaterial == null) return;

        renderer.EnqueuePass(maskPass);
        renderer.EnqueuePass(edgePass);
    }

    private class OutlineMaskPass : ScriptableRenderPass
    {
        private readonly LayerMask layerMask;
        private readonly Material material;
        private static readonly ShaderTagId ShaderTag = new("SRPDefaultUnlit");

        public TextureHandle MaskTexture { get; private set; }

        private class PassData
        {
            public RendererListHandle rendererList;
        }

        public OutlineMaskPass(LayerMask layerMask, Material material)
        {
            this.layerMask = layerMask;
            this.material = material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();

            TextureDesc desc = resourceData.activeColorTexture.GetDescriptor(renderGraph);
            desc.name = "_OutlineMask";
            desc.clearBuffer = true;
            desc.clearColor = Color.black;
            desc.depthBufferBits = 0;
            desc.colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm;

            MaskTexture = renderGraph.CreateTexture(desc);

            using IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Outline Mask", out PassData passData);

            builder.SetRenderAttachment(MaskTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
            builder.AllowPassCulling(false);

            DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(
                ShaderTag, renderingData, cameraData, lightData, SortingCriteria.CommonOpaque);
            drawingSettings.overrideMaterial = material;
            // Obiekty na scenie zwykle maja standardowy material URP/Lit, ktorego
            // przebitki sa otagowane jako UniversalForward/UniversalForwardOnly,
            // nie SRPDefaultUnlit - dolaczamy te tagi, zeby RendererList faktycznie je zlapal.
            drawingSettings.SetShaderPassName(1, new ShaderTagId("UniversalForward"));
            drawingSettings.SetShaderPassName(2, new ShaderTagId("UniversalForwardOnly"));

            FilteringSettings filteringSettings = new(RenderQueueRange.opaque, layerMask);

            RendererListParams listParams = new(renderingData.cullResults, drawingSettings, filteringSettings);
            passData.rendererList = renderGraph.CreateRendererList(listParams);
            builder.UseRendererList(passData.rendererList);

            builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.rendererList);
            });
        }
    }

    private class OutlineEdgePass : ScriptableRenderPass
    {
        private readonly Material material;
        private readonly OutlineMaskPass maskPass;

        private class PassData
        {
            public TextureHandle mask;
            public Material material;
        }

        public OutlineEdgePass(Material material, OutlineMaskPass maskPass)
        {
            this.material = material;
            this.maskPass = maskPass;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            using IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Outline Edge", out PassData passData);

            passData.mask = maskPass.MaskTexture;
            passData.material = material;

            builder.UseTexture(passData.mask);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, data.mask, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }
    }
}
