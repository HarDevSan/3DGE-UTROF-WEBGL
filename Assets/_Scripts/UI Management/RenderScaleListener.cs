using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class RenderScaleListener : MonoBehaviour
{
    public UniversalRenderPipelineAsset URP_Asset;

    public Slider renderScaleSlider;



    private void Update()
    {

        URP_Asset.renderScale = renderScaleSlider.value;
    }

}
