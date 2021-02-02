using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using Data.Util;
using UnityEngine;

namespace ShaderGraphEssentials
{
    static class SGEShaderGraphUtilities
    {
        public static void SetRenderState(SurfaceMaterialOptions options, ref ShaderPass pass)
        {
            // Update render state on ShaderPass if there is no active override
            if(string.IsNullOrEmpty(pass.ZWriteOverride))
            {
                pass.ZWriteOverride = "ZWrite " + options.zWrite.ToString();
            }

            if(string.IsNullOrEmpty(pass.ZTestOverride))
            {
                pass.ZTestOverride = "ZTest " + options.zTest.ToString();
            }

            if(string.IsNullOrEmpty(pass.CullOverride))
            {
                pass.CullOverride = "Cull " + options.cullMode.ToString();
            }
            
            if(string.IsNullOrEmpty(pass.BlendOverride))
            {
                pass.BlendOverride = string.Format("Blend {0} {1}, {2} {3}", options.srcBlend, options.dstBlend, options.alphaSrcBlend, options.alphaDstBlend);
            }
        }
    }
}