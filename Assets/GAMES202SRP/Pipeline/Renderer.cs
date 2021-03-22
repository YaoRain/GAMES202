using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MySRP
{
    public class Renderer
    {
        const string name = "forward";
        ScriptableRenderContext currentContext;

        CommandBuffer _renderCmdBuf;

        RenderData renderData;
        List<RenderPass> passes;

        // TODO:添加add render pass的代码；
        public Renderer()
        {
            GlobalShaderProperties.GetShaderPropertyIDs();
            if(_renderCmdBuf is null)
            {
                _renderCmdBuf = new CommandBuffer();
            }
        }

        public void UpdatePreFrameData()    
        {
            //GlobalShaderProperties.SetPreCameraBuffer();         
        }

        public void UpdataPreCameraData(Camera cam)
        {
            //GlobalShaderProperties.SetPreCameraBuffer();
            renderData.cameraData.camera = cam;
        }

        public void UpdataPreLightData()
        {

        }

        // Render只负责数据的准备，具体数据的处理交给pass去做。
        public void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            UpdatePreFrameData();
            foreach(var pass in passes)
            {
                pass.Excute(context, renderData);
            }
        }

        public void Setup(in ScriptableRenderContext context, Camera camera)
        {
            currentContext = context;
            renderData.cameraData.camera = camera;
            currentContext.SetupCameraProperties(camera);
        }

        public void Clear()
        {
            _renderCmdBuf.ClearRenderTarget(true, true, Color.clear );
            ExcuteBuffer(_renderCmdBuf);
        }

        public void SetupCullingParamete()
        {
            ScriptableCullingParameters cullingParameters;
            if(renderData.cameraData.camera.TryGetCullingParameters(out cullingParameters))
            {
                renderData.cullResults = currentContext.Cull(ref cullingParameters);
            }            
        }

        private void ExcuteBuffer(CommandBuffer cmdBuf)
        {
            currentContext.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();
        }

        public void SubmitRenderCommand()
        {
            currentContext.Submit();
        }

        public void ExcutePasses()
        {
            Camera camTmp = renderData.cameraData.camera;
            var sortSetting = new SortingSettings(camTmp);
            var drawSetting = new DrawingSettings(new ShaderTagId("baseDraw"), sortSetting);
            var filterSetting = new FilteringSettings(RenderQueueRange.all);

            GlobalShaderProperties.SetPreLightBuffer();

            currentContext.DrawRenderers(renderData.cullResults, ref drawSetting, ref filterSetting);

            currentContext.SetupCameraProperties(camTmp, false);
            currentContext.DrawSkybox(camTmp);
        }   
    }
}