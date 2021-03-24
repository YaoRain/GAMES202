using System;
using UnityEngine.Rendering;
using UnityEngine;
namespace MySRP
{
    public class ShadowPass : RenderPass
    {
        CommandBuffer _ShadowCmd;
        public ShadowPass()
        {
            _ShadowCmd = new CommandBuffer();
            _ShadowCmd.name = "Shadow Cmd";
        }

        public override void Setup()
        {
            _ShadowCmd.GetTemporaryRT(ShadowBuffer._ShadowMap, 1024, 1024, 32, FilterMode.Bilinear, RenderTextureFormat.Depth);
            _ShadowCmd.SetRenderTarget(ShadowBuffer._ShadowMap, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            _ShadowCmd.ClearRenderTarget(true, true, Color.white);
            ExcuteBuffer(_ShadowCmd);
        }

        public override void Excute(in RenderData renderData)
        {
            // TODO : 根据点光源位置、near、far、fov生成正确的MVP
            foreach (var mesh in renderData.meshes)
            {
                Transform transTmp = mesh.transform;
                _ShadowCmd.DrawMesh(mesh.mesh, transTmp.localToWorldMatrix, transTmp.GetComponent<MeshRenderer>().material, 0, 0);
            }
            ExcuteBuffer(_ShadowCmd);
        }

        public void Clear()
        {
            _ShadowCmd.ReleaseTemporaryRT(ShadowBuffer._ShadowMap);
            ExcuteBuffer(_ShadowCmd);
        }
    }
}

