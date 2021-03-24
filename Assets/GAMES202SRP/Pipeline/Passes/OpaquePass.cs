using System;
using UnityEngine.Rendering;
using UnityEngine;
namespace MySRP
{
    public class OpaquePass : RenderPass
    {
        CommandBuffer _OpaqueCmd;
        public OpaquePass()
        {
            _OpaqueCmd = new CommandBuffer();
            _OpaqueCmd.name = "Opaque Cmd";
        }

        public override void Setup()
        {
            _OpaqueCmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            _OpaqueCmd.ClearRenderTarget(true, true, Color.white);
            ExcuteBuffer(_OpaqueCmd);
        }

        public override void Excute(in RenderData renderData)
        {
            foreach (var mesh in renderData.meshes)
            {
                Transform transTmp = mesh.transform;
                _OpaqueCmd.DrawMesh(mesh.mesh, transTmp.localToWorldMatrix, transTmp.GetComponent<MeshRenderer>().material, 0, 0);
            }
            ExcuteBuffer(_OpaqueCmd);
        }
    }
}

