using System;
using UnityEngine.Rendering;
using UnityEngine;
namespace MySRP
{
    public class SkyBoxPass : RenderPass
    {
        CommandBuffer _SkyCmd;
        public SkyBoxPass()
        {
            _SkyCmd = new CommandBuffer();
            _SkyCmd.name = "Sky Cmd";
        }

        public override void Setup()
        {
            _SkyCmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget); 
            ExcuteBuffer(_SkyCmd);
        }

        public override void Excute(in RenderData renderData)
        {
            currentContext.DrawSkybox(renderData.cameraData.camera);
        }

        public void Clear()
        {
            _SkyCmd.ReleaseTemporaryRT(ShadowBuffer._ShadowMap);
            ExcuteBuffer(_SkyCmd);
        }
    }
}

