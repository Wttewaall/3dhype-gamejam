Shader "Projector/Additive" {
   Properties {
      _ProjTex ("Logo", 2D) = "gray" { TexGen ObjectLinear }
   }

   Subshader {
      Pass {
         ZWrite Off
         Offset -1, -1
         
         AlphaTest Less 1
         ColorMask RGB   
         Blend  OneMinusSrcAlpha   SrcAlpha
         SetTexture [_ProjTex] {
            combine texture, ONE - texture
            Matrix [_Projector]
         }
      }   
   }
}