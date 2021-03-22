using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MySRP
{
    [CreateAssetMenu(menuName = "Rendering/MySRP")]
    public class SRPAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new Pipeline(this);
        }
    }
}