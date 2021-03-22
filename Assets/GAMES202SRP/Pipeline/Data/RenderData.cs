using System;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MySRP
{
    public struct RenderData
    {
        public CullingResults cullResults;
        public CameraData cameraData;
        public LightData lightData;
        public Attachments attachments;
        public void SetRenderData(CullingResults cullingResults, CameraData camData, LightData inLightData, Attachments attaches)
        {
            cullResults = cullingResults;
            cameraData = camData;
            lightData = inLightData;
            attachments = attaches;
        }
    }

    public struct CameraData
    {
        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectMatrix;        
        void SetCameraMatrix(Matrix4x4 viewMat, Matrix4x4 projMat)
        {
            viewMatrix = viewMat;
            projectMatrix = projMat;
        }
        public Camera camera;
    }

    public class LightData
    {
        public List<Light> lights;
    }

    public struct Attachments
    {
        public RenderTexture colorAttach0;
        public RenderTexture depthAttach;
        public RenderTexture shadowMap;
    }
}