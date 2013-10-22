Shader "SpriteMeshEngine/DefaultShader" 
{
 Properties {
     _MainTex ("Base (RGBA)", 2D) = "white" {}
 }

 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
    Lighting Off
    ZWrite Off
    ZTest Off
     
     Pass {
         ColorMaterial AmbientAndDiffuse
         
         SetTexture [_MainTex] {
             Combine texture * primary, texture * primary
         }
     }
    }
}
