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

        static ShadowPass shadowPass;
        OpaquePass opaquePass;
        SkyBoxPass skyboxPass;

        // TODO:添加add render pass的代码；
        public Renderer()
        {
            GlobalShaderProperties.GetShaderPropertyIDs();
            renderData.meshes = GameObject.FindObjectsOfType<MeshFilter>();
            renderData.lights = GameObject.FindObjectsOfType<Light>();

            shadowPass = new ShadowPass();
            opaquePass = new OpaquePass();
            skyboxPass = new SkyBoxPass();

            _renderCmdBuf = new CommandBuffer();
            _renderCmdBuf.name = "RenderDrawCommand";
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
                pass.Excute(renderData);
            }
        }

        public void Setup(in ScriptableRenderContext context, Camera camera)
        {
            currentContext = context;
            RenderPass.Setup(context);
            renderData.cameraData.camera = camera;
            currentContext.SetupCameraProperties(camera);
        }

        public void SetupCullingParamete()
        {
            ScriptableCullingParameters cullingParameters;
            if(renderData.cameraData.camera.TryGetCullingParameters(out cullingParameters))
            {
                renderData.cullResults = currentContext.Cull(ref cullingParameters);
            }            
        }

        public void SubmitRenderCommand()
        {
            currentContext.Submit();
        }

        public void ExcutePasses()
        {
            SetLighProperties();

            shadowPass.Setup();
            shadowPass.Excute(renderData);

            opaquePass.Setup();
            opaquePass.Excute(renderData);

            skyboxPass.Setup();
            skyboxPass.Excute(renderData);

            shadowPass.Clear();

        }

        void SetLighProperties()
        {
            // TODO:后续实现手动裁剪
            int mainLightIndex = 0;
            int currentIndex = 0;
            foreach (var visiableLight in renderData.cullResults.visibleLights)
            {
                if (visiableLight.lightType == LightType.Directional)
                {
                    mainLightIndex = currentIndex;
                }
                MainLight.SetLightData(visiableLight);
                PointLight.SetLightData(visiableLight);
                currentIndex++;
            }
        }
    }
}