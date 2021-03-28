using System;
using Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace MySRP
{
    public class Pipeline : RenderPipeline
    {
        public const string shaderTagName = "MySRP";
        public static ScriptableRenderContext currentContext;
        public static Renderer currentRenderer;

        CommandBuffer drawBuffer = new CommandBuffer();
        

        public Pipeline(RenderPipelineAsset srpAsset)
        {
            drawBuffer.name = "drawBuffer";
            if(currentRenderer is null)
            {
                currentRenderer = new Renderer();
            }
            // 如果使能了keyword，则需要Disable才能失效，去掉Enable没有用
            Shader.EnableKeyword("UseMainLight");
            //Shader.DisableKeyword("UseMainLight");
            Shader.EnableKeyword("UsePointLight");
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            BeginFrameRendering(context, cameras);
            currentContext = context;

            GlobalShaderProperties.SetPreFrameBuffer();

            SortCameras(ref cameras);
            foreach (var cam in cameras)
            {
                BeginCameraRendering(context, cam);
                // TODO:传正确的MVP进去
                Matrix4x4 _VP = cam.projectionMatrix * cam.worldToCameraMatrix;
                GlobalShaderProperties.SetPreCameraBuffer(cam.transform.position, _VP);
                
                RenderSingleCamera(context, cam);

                EndCameraRendering(context, cam);
                
            }

            context.Submit();
        }

        void SortCameras(ref Camera[] cams)
        {

        }

        void RenderSingleCamera(in ScriptableRenderContext context, in Camera cam)
        {
            if(currentRenderer is null)
            {
                currentRenderer = new Renderer();
            }
            currentRenderer.Setup(context, cam);
            currentRenderer.SetupCullingParamete();
            currentRenderer.ExcutePasses();
            currentRenderer.SubmitRenderCommand();  
        }

        void SetRenderingData(ref RenderData renderData)
        {

        }

    }
}