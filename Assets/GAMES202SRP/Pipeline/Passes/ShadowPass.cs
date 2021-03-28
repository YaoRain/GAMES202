using System;
using UnityEngine.Rendering;
using UnityEngine;
namespace MySRP
{
    public class ShadowPass : RenderPass
    {
        CommandBuffer _ShadowCmd;
        public static Material shadowMat = new Material(Shader.Find("MySRP/ShadowCast"));
        public ShadowPass()
        {
            _ShadowCmd = new CommandBuffer();
            _ShadowCmd.name = "Shadow Cmd";

            
        }

        public override void Setup()
        {
            _ShadowCmd.GetTemporaryRT(ShadowBuffer._ShadowMap, 1024, 1024, 32, FilterMode.Point, RenderTextureFormat.Depth);
            _ShadowCmd.SetRenderTarget(ShadowBuffer._ShadowMap, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            _ShadowCmd.ClearRenderTarget(true, true, Color.clear, 0f);  

            Transform shadowLightTrans = PointLight.shadowPointLight.transform;

            float near = 0.3f;
            float halfW = 0.5f;
            float halfH = 0.5f;
            Matrix4x4 _P = Matrix4x4.Frustum(-halfW, halfW, -halfH, halfH, near, 1000);
            Matrix4x4 _V = Matrix4x4.LookAt(
                shadowLightTrans.position,
                shadowLightTrans.position + shadowLightTrans.forward,
                shadowLightTrans.up);
            _V[5] *= -1;
            // y方向的视口变换取反
            _V[12] *= -1;  _V[13] *= 1;
            // Debug.Log(_V);
            Matrix4x4 _VP = _P * _V ;
            _ShadowCmd.SetGlobalMatrix(PreCameraBuffer._VP, _VP);
            ExcuteBuffer(_ShadowCmd);
        }   

        public override void Excute(in RenderData renderData)
        {
            // TODO : 根据点光源位置、near、far、fov生成正确的MVP
            foreach (var mesh in renderData.meshes)
            {
                Transform transTmp = mesh.transform;
                Matrix4x4 mMat = transTmp.localToWorldMatrix;
                mMat[10] *= -1;
                mMat[14] *= -1;
                _ShadowCmd.SetGlobalMatrix(PreObjBuffer._ObjToWorldMatrix, mMat);
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

