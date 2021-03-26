using System;
using UnityEngine.Rendering;
using UnityEngine;
namespace MySRP
{
    public class ShadowPass : RenderPass
    {
        CommandBuffer _ShadowCmd;
        Material shadowMat;
        public ShadowPass()
        {
            _ShadowCmd = new CommandBuffer();
            _ShadowCmd.name = "Shadow Cmd";

            shadowMat = new Material(Shader.Find("MySRP/ShadowCast"));
        }

        public override void Setup()
        {
            _ShadowCmd.GetTemporaryRT(ShadowBuffer._ShadowMap, 1024, 1024, 32, FilterMode.Point, RenderTextureFormat.Depth);
            _ShadowCmd.SetRenderTarget(ShadowBuffer._ShadowMap, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            _ShadowCmd.ClearRenderTarget(true, true, Color.clear);  

            Transform shadowLightTrans = PointLight.shadowPointLight.transform;

            float near = 0.3f;
            float halfW = 0.5f;
            float halfH = 0.5f;
            Matrix4x4 _P = Matrix4x4.Frustum(-halfW, halfW, -halfH, halfH, near, 10);
            Matrix4x4 _V = Matrix4x4.LookAt(
                shadowLightTrans.position,
                shadowLightTrans.position + Vector3.forward,
                Vector3.up);
                Debug.Log(_V);
            //_P = Matrix4x4.identity;
            // _V = Matrix4x4.identity;
            // _V[11] *= -1;
            Matrix4x4 matTmp = Matrix4x4.identity;
            matTmp[10] = -1; matTmp[0] = 1f; matTmp[5] = -1f;
            _V = _V * matTmp;   
            Matrix4x4 _VP = _P * _V;
            _ShadowCmd.SetGlobalMatrix(PreCameraBuffer._VP, _VP);
            ExcuteBuffer(_ShadowCmd);
        }   

        public override void Excute(in RenderData renderData)
        {
            // TODO : 根据点光源位置、near、far、fov生成正确的MVP
            foreach (var mesh in renderData.meshes)
            {
                int _M_Matrix = Shader.PropertyToID("_M");
                
                Transform transTmp = mesh.transform;
                _ShadowCmd.SetGlobalMatrix(_M_Matrix, transTmp.localToWorldMatrix);
                _ShadowCmd.DrawMesh(mesh.mesh, transTmp.localToWorldMatrix, shadowMat, 0, 0);
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

