Shader "Universe/Vertex Color Alpha" 
{
	Properties {
	    //_Color ("Main Color", Color) = (1,1,1,1)
	    //_SpecColor ("Spec Color", Color) = (1,1,1,1)
	    //_Emission ("Emmisive Color", Color) = (0,0,0,0)
	    //_Shininess ("Shininess", Range (0.01, 1)) = 0.7
	    _MainTex ("Base (RGBA)", 2D) = "white" {}
	}

	SubShader {
	 	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
	 	ZWrite Off
	 	ZTest off
	 	
	    Pass {
	        //Material {
	        //    Shininess [_Shininess]
	        //    Specular [_SpecColor]
	        //    Emission [_Emission]    
	        //}
	        ColorMaterial AmbientAndDiffuse
	        
	        //Lighting On
	        //SeparateSpecular On
	        SetTexture [_MainTex] {
	            Combine texture * primary, texture * primary
	        }
	        //SetTexture [_MainTex] {
	        //    constantColor [_Color]
	        //    Combine previous * constant DOUBLE, previous * constant
	        //} 
	    }
	}

	Fallback " VertexLit", 1
}
