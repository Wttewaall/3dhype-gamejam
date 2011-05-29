// Hard Surface Free shader created for the Unity community by Bruno Rime version 1.0 http://www.behance.net/brunorime
Shader "HardSurface/Hardsurface Free/Transparent Diffuse"{

Properties {
	_Color ("Main Color", Color) = (.5,.5,.5,.5)
	_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
	_Shininess ("Shininess", Range (0.01, 3)) = 1.5
	_Gloss("Gloss", Range (0.00, 1)) = .5
	_Reflection("Reflection", Range (0.00, 1)) = 0.5
	_Cube ("Reflection Cubemap", Cube) = "Black" { TexGen CubeReflect }
	_FrezPow("Fresnel Reflection",Range(0,1024)) = 1024
	_FrezFalloff("Fresnal/EdgeAlpha Falloff",Range(0,10)) = 2
	_EdgeAlpha("Edge Alpha",Range(0,1)) = 0
	_Metalics("Metalics",Range(0,1)) = .5
	
	
	_MainTex ("Diffuse(RGB) Alpha(A)",2D) = "White" {}
	
	
}

	SubShader {
		
		// Zprime Frontfaces
		Pass {
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "LightMode" = "Always"}
			zwrite on Cull back
			colormask 0
		}

		 // Front Faces
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		UsePass "Hidden/Hardsurface Pro Front Transparent Diffuse/FORWARD"
		
	} 
}
