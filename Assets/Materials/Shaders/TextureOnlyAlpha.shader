Shader "Universe/Texture Only Alpha" 
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {
	 	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
	 	ZWrite Off
	 	ZTest Off
	 	
        Pass {
            SetTexture [_MainTex] {
                //Combine texture * primary, texture * primary
            }
        }
    }
}
