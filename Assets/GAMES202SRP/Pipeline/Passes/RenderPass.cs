using System;
using UnityEngine.Rendering;
namespace MySRP
{
    public class RenderPass
    {
        protected static ScriptableRenderContext currentContext;
        public static void Setup(in ScriptableRenderContext context)
        {
            currentContext = context;
        }

        public virtual void Setup()
        {

        }

        public virtual void Excute(in RenderData renderData)
        {
            
        }


        protected void ExcuteBuffer(in CommandBuffer cmdBuf)
        {
            currentContext.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();
        }
    }
}

