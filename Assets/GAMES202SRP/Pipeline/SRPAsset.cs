using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MySRP
{
    [CreateAssetMenu(menuName = "Rendering/MySRP")]
    public class SRPAsset : RenderPipelineAsset
    {
        /* 后续在此开放一些管线的可配置数据 */
        SRPAsset()
        {
            
        }
        protected override RenderPipeline CreatePipeline()
        {
            
            return new Pipeline(this);
        }
    }
}