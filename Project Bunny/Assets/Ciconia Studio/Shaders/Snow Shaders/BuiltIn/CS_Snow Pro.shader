Shader "Ciconia Studio/CS_Ice & Snow/Builtin/Snow/Pro"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Space(35)][Header(Main Properties________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Space(15)]_GlobalXYTilingXYZWOffsetXY("Global --> XY(TilingXY) - ZW(OffsetXY)", Vector) = (1,1,0,0)
		_BaseColor("Color", Color) = (1,1,1,1)
		[Toggle]_InvertABaseColor1("Invert Alpha", Float) = 0
		_BaseMap("Base Color", 2D) = "white" {}
		_Saturation("Saturation", Float) = 0
		_Brightness("Brightness", Range( 1 , 8)) = 1
		[Space(35)]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Intensity", Float) = 0.3
		[Space(35)]_MetallicGlossMapMAHS("Mask Map  -->M(R) - Ao(G) - H(B) - S(A)", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 2)) = 0
		_Glossiness("Smoothness", Range( 0 , 2)) = 0.5
		[Space(15)]_Parallax("Height Scale", Range( -0.1 , 0.1)) = 0
		_AoIntensity("Ao Intensity", Range( 0 , 2)) = 0
		[HDR][Space(45)]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_EmissionMap("Emission Map", 2D) = "white" {}
		_EmissionIntensity("Intensity", Range( 0 , 2)) = 1
		[Space(35)][Header(Mask Properties________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Space(15)][Toggle]_VisualizeMask("Visualize Mask", Float) = 0
		[KeywordEnum(DetailMask,BaseColorAlpha,Both)] _SourceMask("Source", Float) = 0
		[Toggle]_ExcludeMask("Exclude BaseColorAlpha", Float) = 0
		[Toggle]_TopCoverageMask("Coverage : Top", Float) = 0
		[Space(15)][Toggle]_InvertMask("Invert Mask", Float) = 0
		_DetailMask("Detail Mask", 2D) = "white" {}
		_IntensityMask("Intensity", Range( 0 , 1)) = 1
		[Space(15)]_ContrastDetailMap("Contrast", Float) = 0
		_SpreadDetailMap("Spread", Range( 0 , 1)) = 0.5
		[Space(35)][Header(Snow Properties________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________)][Space(15)]_GlobalXYTilingXYZWOffsetXYSnow("Global --> XY(TilingXY) - ZW(OffsetXY)", Vector) = (1,1,0,0)
		[Space(15)]_DetailColor("Color", Color) = (1,1,1,1)
		_DetailAlbedoMap("Base Color", 2D) = "white" {}
		_DetailSaturation("Saturation", Float) = 0
		_DetailBrightness("Brightness", Range( 1 , 8)) = 1
		[Space(35)]_DetailNormalMap("Normal Map", 2D) = "bump" {}
		_DetailNormalMapScale("Scale", Float) = 0.3
		[Toggle]_BlendNormal("Blend Normal", Float) = 0
		[Space(35)]_DetailMAoHSMap("Mask Map  -->M(R) - Ao(G) - H(B) - S(A)", 2D) = "white" {}
		_MetallicDetail("Metallic", Range( 0 , 2)) = 0
		_GlossinessDetail("Smoothness", Range( 0 , 2)) = 0.5
		[Space(15)]_ParallaxDetail("Height Scale", Range( -0.1 , 0.1)) = 0
		_AoIntensityDetail("Ao Intensity", Range( 0 , 2)) = 0
		[Space(35)][Header(__________ Sparkles Properties __________)][Space(15)][KeywordEnum(None,SmoothnessSparkles,EmissiveSparkles)] _SparkleSource("Source", Float) = 2
		[Toggle]_BlendingModeCrystals("Blending Mode Additive", Float) = 0
		[Space(15)][KeywordEnum(None,DotMask,SparkleMap)] _VisualizeSparkle("Visualize Maps", Float) = 0
		_SparkleMask("Sparkle Mask", 2D) = "white" {}
		[Space(30)][Header(Dot Mask)]_TilingDotMask("Tiling", Float) = 5
		_IntensityDotMask("Intensity", Range( 0 , 1)) = 1
		[Space(15)]_ContrastDotMask("Contrast", Float) = 0
		_SpreadDotMask("Spread", Range( 0 , 1)) = 0.5
		[Space(30)][Header(Sparkle Map)]_TilingCrystals("Tiling", Float) = 7
		_CrystalsIntensity("Intensity", Float) = 5
		[Space(15)]_ContrastSparkles("Contrast", Float) = 3.5
		_AmountSparkle("Amount", Range( 0 , 1)) = 1
		[Space(30)][Header(Custom Properties)]_TilingInstance("Tiling Instance", Float) = 1
		_SparklePower("Sparkle Power", Float) = 10
		_Desaturatecrystals("Desaturate", Range( 0 , 1)) = 0.5
		[Space(10)]_ShadowMask("Shadow Mask", Range( 0 , 1)) = 0
		[Toggle]_AoMask("Ao Mask", Float) = 0
		_UseEmissionFromMainProperties("Use Emission From Main Properties", Range( 0 , 1)) = 0
		[Space(35)][Header(__________ Snow Accum __________)][Space(15)][Toggle]_EnableTopAccum("Enable Top Accum", Float) = 0
		[Toggle]_TopCoverage("Coverage : All", Float) = 0
		_SnowTopIntensity("Intensity", Range( 0 , 1)) = 1
		[Space(15)]_AmountTop("Coverage Amount", Range( 0 , 1)) = 0.5
		_GradientTop("Gradient Blend", Float) = 5
		[Space(35)][Toggle]_EnableBottomAccum("Enable Bottom Accum", Float) = 0
		_SnowBottomIntensity("Intensity", Range( 0 , 1)) = 1
		[Space(15)]_AmountBottom("Coverage Amount", Range( 0 , 1)) = 0.555584
		_GradientBottom("Gradient Blend", Float) = 5
		[Space(35)][Header(Source Blend)][Space(15)][KeywordEnum(None,HeightmapB,Noise,Both)] _SourceEdgeBlendTop("Source", Float) = 0
		_SharpenEdge("Sharpen Edge", Float) = 10
		_NoiseScale("Noise Scale", Float) = 10

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 


		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70403

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_SHADOWCOORDS
			#pragma shader_feature_local _VISUALIZESPARKLE_NONE _VISUALIZESPARKLE_DOTMASK _VISUALIZESPARKLE_SPARKLEMAP
			#pragma shader_feature_local _SOURCEMASK_DETAILMASK _SOURCEMASK_BASECOLORALPHA _SOURCEMASK_BOTH
			#pragma shader_feature_local _SOURCEEDGEBLENDTOP_NONE _SOURCEEDGEBLENDTOP_HEIGHTMAPB _SOURCEEDGEBLENDTOP_NOISE _SOURCEEDGEBLENDTOP_BOTH
			#pragma shader_feature_local _SPARKLESOURCE_NONE _SPARKLESOURCE_SMOOTHNESSSPARKLES _SPARKLESOURCE_EMISSIVESPARKLES


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailAlbedoMap_ST;
			float4 _DetailNormalMap_ST;
			float4 _DetailMask_ST;
			float4 _BumpMap_ST;
			float4 _DetailMAoHSMap_ST;
			float4 _GlobalXYTilingXYZWOffsetXYSnow;
			float4 _EmissionMap_ST;
			float4 _DetailColor;
			float4 _EmissionColor;
			float4 _MetallicGlossMapMAHS_ST;
			float4 _GlobalXYTilingXYZWOffsetXY;
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _TilingCrystals;
			float _ContrastSparkles;
			float _AmountSparkle;
			float _CrystalsIntensity;
			float _BlendNormal;
			float _BumpScale;
			float _IntensityDotMask;
			float _Desaturatecrystals;
			float _VisualizeMask;
			float _AoMask;
			float _EmissionIntensity;
			float _ContrastDotMask;
			float _AoIntensityDetail;
			float _SparklePower;
			float _ShadowMask;
			float _UseEmissionFromMainProperties;
			float _Metallic;
			float _MetallicDetail;
			float _Glossiness;
			float _GlossinessDetail;
			float _DetailNormalMapScale;
			float _SpreadDotMask;
			float _AmountBottom;
			float _TilingDotMask;
			float _Brightness;
			float _Parallax;
			float _Saturation;
			float _DetailBrightness;
			float _ParallaxDetail;
			float _DetailSaturation;
			float _ExcludeMask;
			float _ContrastDetailMap;
			float _InvertMask;
			float _InvertABaseColor1;
			float _SpreadDetailMap;
			float _TilingInstance;
			float _TopCoverageMask;
			float _EnableTopAccum;
			float _AmountTop;
			float _NoiseScale;
			float _SharpenEdge;
			float _GradientTop;
			float _TopCoverage;
			float _SnowTopIntensity;
			float _EnableBottomAccum;
			float _BlendingModeCrystals;
			float _GradientBottom;
			float _SnowBottomIntensity;
			float _IntensityMask;
			float _AoIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _BaseMap;
			sampler2D _MetallicGlossMapMAHS;
			sampler2D _DetailAlbedoMap;
			sampler2D _DetailMAoHSMap;
			sampler2D _DetailMask;
			sampler2D _SparkleMask;
			sampler2D _BumpMap;
			sampler2D _DetailNormalMap;
			sampler2D _EmissionMap;


			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			
			float4 CalculateContrast( float contrastValue, float4 colorTarget )
			{
				float t = 0.5 * ( 1.0 - contrastValue );
				return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
			}
			//https://www.shadertoy.com/view/XdXGW8
			float2 GradientNoiseDir( float2 x )
			{
				const float2 k = float2( 0.3183099, 0.3678794 );
				x = x * k + k.yx;
				return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
			}
			
			float GradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 i = floor( p );
				float2 f = frac( p );
				float2 u = f * f * ( 3.0 - 2.0 * f );
				return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
						lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_texcoord8 = v.vertex;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						, FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_BaseMap = IN.ase_texcoord7.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				float2 break26_g1528 = uv_BaseMap;
				float GlobalTilingX5 = ( _GlobalXYTilingXYZWOffsetXY.x - 1.0 );
				float GlobalTilingY4 = ( _GlobalXYTilingXYZWOffsetXY.y - 1.0 );
				float2 appendResult14_g1528 = (float2(( break26_g1528.x * GlobalTilingX5 ) , ( break26_g1528.y * GlobalTilingY4 )));
				float GlobalOffsetX6 = _GlobalXYTilingXYZWOffsetXY.z;
				float GlobalOffsetY3 = _GlobalXYTilingXYZWOffsetXY.w;
				float2 appendResult13_g1528 = (float2(( break26_g1528.x + GlobalOffsetX6 ) , ( break26_g1528.y + GlobalOffsetY3 )));
				float2 uv_MetallicGlossMapMAHS = IN.ase_texcoord7.xy * _MetallicGlossMapMAHS_ST.xy + _MetallicGlossMapMAHS_ST.zw;
				float2 break26_g1239 = uv_MetallicGlossMapMAHS;
				float2 appendResult14_g1239 = (float2(( break26_g1239.x * GlobalTilingX5 ) , ( break26_g1239.y * GlobalTilingY4 )));
				float2 appendResult13_g1239 = (float2(( break26_g1239.x + GlobalOffsetX6 ) , ( break26_g1239.y + GlobalOffsetY3 )));
				float4 tex2DNode3_g1238 = tex2D( _MetallicGlossMapMAHS, ( ( appendResult14_g1239 + appendResult13_g1239 ) + float2( 0,0 ) ) );
				float3 tanToWorld0 = float3( WorldTangent.x, WorldBiTangent.x, WorldNormal.x );
				float3 tanToWorld1 = float3( WorldTangent.y, WorldBiTangent.y, WorldNormal.y );
				float3 tanToWorld2 = float3( WorldTangent.z, WorldBiTangent.z, WorldNormal.z );
				float3 ase_tanViewDir =  tanToWorld0 * WorldViewDirection.x + tanToWorld1 * WorldViewDirection.y  + tanToWorld2 * WorldViewDirection.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 paralaxOffset38_g1238 = ParallaxOffset( tex2DNode3_g1238.b , _Parallax , ase_tanViewDir );
				float2 switchResult37_g1238 = (((ase_vface>0)?(paralaxOffset38_g1238):(0.0)));
				float2 Parallax23 = switchResult37_g1238;
				float4 tex2DNode7_g1527 = tex2D( _BaseMap, ( ( appendResult14_g1528 + appendResult13_g1528 ) + Parallax23 ) );
				float clampResult27_g1527 = clamp( _Saturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1527 = ( _BaseColor * tex2DNode7_g1527 ).rgb;
				float desaturateDot29_g1527 = dot( desaturateInitialColor29_g1527, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1527 = lerp( desaturateInitialColor29_g1527, desaturateDot29_g1527.xxx, -clampResult27_g1527 );
				float4 temp_output_712_0 = CalculateContrast(_Brightness,float4( desaturateVar29_g1527 , 0.0 ));
				float2 uv_DetailAlbedoMap = IN.ase_texcoord7.xy * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
				float2 break26_g1594 = uv_DetailAlbedoMap;
				float GlobalTilingDX312 = ( _GlobalXYTilingXYZWOffsetXYSnow.x - 1.0 );
				float GlobalTilingDY311 = ( _GlobalXYTilingXYZWOffsetXYSnow.y - 1.0 );
				float2 appendResult14_g1594 = (float2(( break26_g1594.x * GlobalTilingDX312 ) , ( break26_g1594.y * GlobalTilingDY311 )));
				float GlobalOffsetDX310 = _GlobalXYTilingXYZWOffsetXYSnow.z;
				float GlobalOffsetDY313 = _GlobalXYTilingXYZWOffsetXYSnow.w;
				float2 appendResult13_g1594 = (float2(( break26_g1594.x + GlobalOffsetDX310 ) , ( break26_g1594.y + GlobalOffsetDY313 )));
				float2 uv_DetailMAoHSMap = IN.ase_texcoord7.xy * _DetailMAoHSMap_ST.xy + _DetailMAoHSMap_ST.zw;
				float2 break26_g1530 = uv_DetailMAoHSMap;
				float2 appendResult14_g1530 = (float2(( break26_g1530.x * GlobalTilingDX312 ) , ( break26_g1530.y * GlobalTilingDY311 )));
				float2 appendResult13_g1530 = (float2(( break26_g1530.x + GlobalOffsetDX310 ) , ( break26_g1530.y + GlobalOffsetDY313 )));
				float4 tex2DNode3_g1529 = tex2D( _DetailMAoHSMap, ( ( appendResult14_g1530 + appendResult13_g1530 ) + float2( 0,0 ) ) );
				float2 paralaxOffset38_g1529 = ParallaxOffset( tex2DNode3_g1529.b , _ParallaxDetail , ase_tanViewDir );
				float2 switchResult37_g1529 = (((ase_vface>0)?(paralaxOffset38_g1529):(0.0)));
				float2 DetailParallax323 = switchResult37_g1529;
				float4 tex2DNode7_g1593 = tex2D( _DetailAlbedoMap, ( ( appendResult14_g1594 + appendResult13_g1594 ) + DetailParallax323 ) );
				float4 lerpResult52_g1593 = lerp( _DetailColor , ( ( _DetailColor * tex2DNode7_g1593 ) * _DetailColor.a ) , _DetailColor.a);
				float clampResult27_g1593 = clamp( _DetailSaturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1593 = lerpResult52_g1593.rgb;
				float desaturateDot29_g1593 = dot( desaturateInitialColor29_g1593, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1593 = lerp( desaturateInitialColor29_g1593, desaturateDot29_g1593.xxx, -clampResult27_g1593 );
				float4 DetailBaseColor301 = CalculateContrast(_DetailBrightness,float4( desaturateVar29_g1593 , 0.0 ));
				float temp_output_34_0_g1531 = ( _ContrastDetailMap + 1.0 );
				float2 uv_DetailMask = IN.ase_texcoord7.xy * _DetailMask_ST.xy + _DetailMask_ST.zw;
				float4 tex2DNode27_g1531 = tex2Dlod( _DetailMask, float4( uv_DetailMask, 0, 0.0) );
				float BaseColorAlpha14 = (( _InvertABaseColor1 )?( ( 1.0 - tex2DNode7_g1527.a ) ):( tex2DNode7_g1527.a ));
				float temp_output_46_0_g1531 = BaseColorAlpha14;
				#if defined(_SOURCEMASK_DETAILMASK)
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#elif defined(_SOURCEMASK_BASECOLORALPHA)
				float staticSwitch61_g1531 = temp_output_46_0_g1531;
				#elif defined(_SOURCEMASK_BOTH)
				float staticSwitch61_g1531 = ( (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) )) + temp_output_46_0_g1531 );
				#else
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#endif
				float temp_output_37_0_g1531 = ( staticSwitch61_g1531 + (-1.2 + (_SpreadDetailMap - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float4 temp_cast_4 = (temp_output_37_0_g1531).xxxx;
				float clampResult38_g1531 = clamp( (( _ExcludeMask )?( ( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r * ( 1.0 - temp_output_46_0_g1531 ) ) ):( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r )) , 0.0 , 1.0 );
				float MaskWorldY458 = WorldNormal.y;
				float4 transform472 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord8.xyz , 0.0 ));
				float lerpResult464 = lerp( -1.0 , 2.0 , _AmountTop);
				float temp_output_466_0 = ( ( ( transform472.y - 1.0 ) / 2.0 ) + lerpResult464 );
				float HeightmapB59 = tex2DNode3_g1238.b;
				float temp_output_513_0 = ( 1.0 - HeightmapB59 );
				float2 texCoord591 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float gradientNoise590 = GradientNoise(texCoord591,_NoiseScale);
				gradientNoise590 = gradientNoise590*0.5 + 0.5;
				float temp_output_589_0 = ( 1.0 - gradientNoise590 );
				float blendOpSrc593 = temp_output_513_0;
				float blendOpDest593 = temp_output_589_0;
				#if defined(_SOURCEEDGEBLENDTOP_NONE)
				float staticSwitch560 = 1.0;
				#elif defined(_SOURCEEDGEBLENDTOP_HEIGHTMAPB)
				float staticSwitch560 = temp_output_513_0;
				#elif defined(_SOURCEEDGEBLENDTOP_NOISE)
				float staticSwitch560 = temp_output_589_0;
				#elif defined(_SOURCEEDGEBLENDTOP_BOTH)
				float staticSwitch560 = ( saturate( max( blendOpSrc593, blendOpDest593 ) ));
				#else
				float staticSwitch560 = 1.0;
				#endif
				float temp_output_510_0 = (-0.5 + (( IN.ase_color.r + staticSwitch560 ) - 0.0) * (0.1 - -0.5) / (1.0 - 0.0));
				float clampResult524 = clamp( _SharpenEdge , 0.0 , 100000.0 );
				float lerpResult511 = lerp( temp_output_510_0 , staticSwitch560 , clampResult524);
				float temp_output_507_0 = ( temp_output_510_0 - lerpResult511 );
				float clampResult467 = clamp( _GradientTop , 0.0 , 100000.0 );
				float blendOpSrc471 = ( temp_output_466_0 - temp_output_507_0 );
				float blendOpDest471 = ( temp_output_466_0 * clampResult467 );
				float clampResult469 = clamp( ( ( ( saturate( ( blendOpSrc471 + blendOpDest471 ) )) * (( _TopCoverage )?( 1.0 ):( MaskWorldY458 )) ) * _SnowTopIntensity ) , 0.0 , 1.0 );
				float4 transform600 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord8.xyz , 0.0 ));
				float lerpResult602 = lerp( -4.0 , 2.0 , _AmountBottom);
				float temp_output_605_0 = ( ( 1.0 - ( ( transform600.y - 1.0 ) / 2.0 ) ) + lerpResult602 );
				float clampResult608 = clamp( _GradientBottom , 0.0 , 100000.0 );
				float blendOpSrc610 = ( temp_output_605_0 - temp_output_507_0 );
				float blendOpDest610 = ( temp_output_605_0 * clampResult608 );
				float clampResult614 = clamp( ( ( saturate( ( blendOpSrc610 + blendOpDest610 ) )) * _SnowBottomIntensity ) , 0.0 , 1.0 );
				float blendOpSrc616 = (( _EnableTopAccum )?( clampResult469 ):( 0.0 ));
				float blendOpDest616 = (( _EnableBottomAccum )?( clampResult614 ):( 0.0 ));
				float SnowAccumTop470 = ( saturate( max( blendOpSrc616, blendOpDest616 ) ));
				float blendOpSrc517 = ( ( clampResult38_g1531 * (( _TopCoverageMask )?( saturate( MaskWorldY458 ) ):( 1.0 )) ) * _IntensityMask );
				float blendOpDest517 = SnowAccumTop470;
				float Mask78 = ( saturate( max( blendOpSrc517, blendOpDest517 ) ));
				float4 lerpResult347 = lerp( temp_output_712_0 , DetailBaseColor301 , Mask78);
				float4 temp_cast_7 = (Mask78).xxxx;
				float4 Albedo15 = (( _VisualizeMask )?( temp_cast_7 ):( lerpResult347 ));
				float2 texCoord106_g1599 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult149_g1599 = clamp( _TilingInstance , 0.0 , 1000.0 );
				float2 temp_output_175_0_g1599 = DetailParallax323;
				float temp_output_123_0_g1599 = ( tex2D( _SparkleMask, ( ( ( texCoord106_g1599 * _TilingDotMask ) * clampResult149_g1599 ) + temp_output_175_0_g1599 ) ).a + (-1.2 + (_SpreadDotMask - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float saferPower230_g1599 = max( temp_output_123_0_g1599 , 0.0001 );
				float temp_output_121_0_g1599 = ( _ContrastDotMask + 1.0 );
				float clampResult118_g1599 = clamp( pow( saferPower230_g1599 , temp_output_121_0_g1599 ) , 0.0 , 1.0 );
				float temp_output_119_0_g1599 = ( clampResult118_g1599 * _IntensityDotMask );
				float VisualizeDotMap230 = temp_output_119_0_g1599;
				float4 temp_cast_8 = (VisualizeDotMap230).xxxx;
				float temp_output_135_0_g1599 = ( _ContrastSparkles - -1.0 );
				float4 unityObjectToClipPos78_g1599 = TransformWorldToHClip(TransformObjectToWorld(IN.ase_texcoord8.xyz));
				float clampResult49_g1599 = clamp( _Desaturatecrystals , 0.0 , 1.0 );
				float3 desaturateInitialColor53_g1599 = tex2D( _SparkleMask, ( ( ( unityObjectToClipPos78_g1599 * ( _TilingCrystals / 10.0 ) ) * clampResult149_g1599 ) + float4( temp_output_175_0_g1599, 0.0 , 0.0 ) ).xy ).rgb;
				float desaturateDot53_g1599 = dot( desaturateInitialColor53_g1599, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar53_g1599 = lerp( desaturateInitialColor53_g1599, desaturateDot53_g1599.xxx, clampResult49_g1599 );
				float lerpResult246_g1599 = lerp( 0.6 , 1.0 , _AmountSparkle);
				float4 temp_cast_13 = (1.0).xxxx;
				float4 clampResult74_g1599 = clamp( ( saturate( ( CalculateContrast(temp_output_135_0_g1599,float4( desaturateVar53_g1599 , 0.0 )) + (( -1.0 - temp_output_135_0_g1599 ) + (lerpResult246_g1599 - 0.0) * (0.0 - ( -1.0 - temp_output_135_0_g1599 )) / (1.0 - 0.0)) ) ) * _CrystalsIntensity ) , float4( 0,0,0,0 ) , temp_cast_13 );
				float4 VisualizeSparklesMap231 = clampResult74_g1599;
				#if defined(_VISUALIZESPARKLE_NONE)
				float4 staticSwitch242 = Albedo15;
				#elif defined(_VISUALIZESPARKLE_DOTMASK)
				float4 staticSwitch242 = temp_cast_8;
				#elif defined(_VISUALIZESPARKLE_SPARKLEMAP)
				float4 staticSwitch242 = VisualizeSparklesMap231;
				#else
				float4 staticSwitch242 = Albedo15;
				#endif
				
				float2 uv_BumpMap = IN.ase_texcoord7.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
				float2 break26_g1607 = uv_BumpMap;
				float2 appendResult14_g1607 = (float2(( break26_g1607.x * GlobalTilingX5 ) , ( break26_g1607.y * GlobalTilingY4 )));
				float2 appendResult13_g1607 = (float2(( break26_g1607.x + GlobalOffsetX6 ) , ( break26_g1607.y + GlobalOffsetY3 )));
				float3 unpack4_g1606 = UnpackNormalScale( tex2D( _BumpMap, ( ( appendResult14_g1607 + appendResult13_g1607 ) + Parallax23 ) ), _BumpScale );
				unpack4_g1606.z = lerp( 1, unpack4_g1606.z, saturate(_BumpScale) );
				float3 tex2DNode4_g1606 = unpack4_g1606;
				float3 temp_output_181_0 = tex2DNode4_g1606;
				float2 uv_DetailNormalMap = IN.ase_texcoord7.xy * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw;
				float2 break26_g1603 = uv_DetailNormalMap;
				float2 appendResult14_g1603 = (float2(( break26_g1603.x * GlobalTilingDX312 ) , ( break26_g1603.y * GlobalTilingDY311 )));
				float2 appendResult13_g1603 = (float2(( break26_g1603.x + GlobalOffsetDX310 ) , ( break26_g1603.y + GlobalOffsetDY313 )));
				float3 unpack4_g1602 = UnpackNormalScale( tex2D( _DetailNormalMap, ( ( appendResult14_g1603 + appendResult13_g1603 ) + DetailParallax323 ) ), _DetailNormalMapScale );
				unpack4_g1602.z = lerp( 1, unpack4_g1602.z, saturate(_DetailNormalMapScale) );
				float3 DetailNormal302 = unpack4_g1602;
				float3 lerpResult362 = lerp( temp_output_181_0 , DetailNormal302 , Mask78);
				float3 lerpResult694 = lerp( temp_output_181_0 , BlendNormal( temp_output_181_0 , DetailNormal302 ) , Mask78);
				float3 Normal35 = (( _BlendNormal )?( lerpResult694 ):( lerpResult362 ));
				
				float2 uv_EmissionMap = IN.ase_texcoord7.xy * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
				float2 break26_g1605 = uv_EmissionMap;
				float2 appendResult14_g1605 = (float2(( break26_g1605.x * GlobalTilingX5 ) , ( break26_g1605.y * GlobalTilingY4 )));
				float2 appendResult13_g1605 = (float2(( break26_g1605.x + GlobalOffsetX6 ) , ( break26_g1605.y + GlobalOffsetY3 )));
				float4 temp_output_182_0 = ( _EmissionColor * tex2D( _EmissionMap, ( ( appendResult14_g1605 + appendResult13_g1605 ) + Parallax23 ) ) * _EmissionIntensity );
				float clampResult212_g1599 = clamp( temp_output_119_0_g1599 , 0.0 , 1.0 );
				float4 temp_output_108_0_g1599 = ( clampResult74_g1599 * clampResult212_g1599 );
				float blendOpSrc34_g1529 = tex2DNode3_g1529.g;
				float blendOpDest34_g1529 = ( 1.0 - _AoIntensityDetail );
				float DetailAOG324 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc34_g1529 ) * ( 1.0 - blendOpDest34_g1529 ) ) ));
				float4 temp_cast_15 = (DetailAOG324).xxxx;
				float dotResult152_g1599 = dot( WorldNormal , SafeNormalize(_MainLightPosition.xyz) );
				float temp_output_153_0_g1599 = max( dotResult152_g1599 , 0.0 );
				float clampResult240_g1599 = clamp( _SparklePower , 0.0 , 100000.0 );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				float lerpResult258_g1599 = lerp( 0.0 , clampResult240_g1599 , ( temp_output_153_0_g1599 * ( ase_lightAtten * _MainLightColor.a ) ));
				float lerpResult169_g1599 = lerp( ( temp_output_153_0_g1599 * _MainLightColor.a ) , lerpResult258_g1599 , _ShadowMask);
				float4 temp_output_165_0_g1599 = ( (( _AoMask )?( ( temp_output_108_0_g1599 * temp_cast_15 ) ):( temp_output_108_0_g1599 )) * lerpResult169_g1599 );
				float saferPower709 = max( Mask78 , 0.0001 );
				float MaskandAccum518 = pow( saferPower709 , 5.0 );
				float blendOpSrc506 = MaskandAccum518;
				float blendOpDest506 = SnowAccumTop470;
				float4 temp_output_174_0 = ( temp_output_165_0_g1599 * ( saturate( ( 1.0 - ( 1.0 - blendOpSrc506 ) * ( 1.0 - blendOpDest506 ) ) )) );
				float4 SparklesCrystals173 = temp_output_174_0;
				float4 lerpResult392 = lerp( temp_output_182_0 , SparklesCrystals173 , MaskandAccum518);
				float4 blendOpSrc400 = temp_output_182_0;
				float4 blendOpDest400 = SparklesCrystals173;
				float4 lerpResult398 = lerp( lerpResult392 , ( saturate( ( blendOpSrc400 + blendOpDest400 ) )) , _UseEmissionFromMainProperties);
				float4 temp_cast_17 = (0.0).xxxx;
				float4 lerpResult374 = lerp( temp_output_182_0 , temp_cast_17 , MaskandAccum518);
				float4 lerpResult395 = lerp( lerpResult374 , temp_output_182_0 , _UseEmissionFromMainProperties);
				#if defined(_SPARKLESOURCE_NONE)
				float4 staticSwitch252 = lerpResult395;
				#elif defined(_SPARKLESOURCE_SMOOTHNESSSPARKLES)
				float4 staticSwitch252 = lerpResult395;
				#elif defined(_SPARKLESOURCE_EMISSIVESPARKLES)
				float4 staticSwitch252 = lerpResult398;
				#else
				float4 staticSwitch252 = lerpResult398;
				#endif
				float4 Emission53 = staticSwitch252;
				
				float DetailMetallic325 = ( tex2DNode3_g1529.r * _MetallicDetail );
				float lerpResult365 = lerp( ( tex2DNode3_g1238.r * _Metallic ) , DetailMetallic325 , MaskandAccum518);
				float Metallic54 = lerpResult365;
				
				float temp_output_1_0_g1238 = ( tex2DNode3_g1238.a * _Glossiness );
				float temp_output_1_0_g1529 = ( tex2DNode3_g1529.a * _GlossinessDetail );
				float DetailSmoothness326 = temp_output_1_0_g1529;
				float lerpResult368 = lerp( temp_output_1_0_g1238 , DetailSmoothness326 , MaskandAccum518);
				float4 temp_cast_19 = (lerpResult368).xxxx;
				float4 temp_cast_20 = (lerpResult368).xxxx;
				float4 temp_cast_21 = (lerpResult368).xxxx;
				float4 blendOpSrc227 = temp_cast_21;
				float4 blendOpDest227 = SparklesCrystals173;
				float4 temp_output_227_0 = ( saturate( max( blendOpSrc227, blendOpDest227 ) ));
				float4 temp_cast_22 = (lerpResult368).xxxx;
				float4 blendOpSrc223 = ( temp_cast_22 - SparklesCrystals173 );
				float4 blendOpDest223 = SparklesCrystals173;
				float4 temp_output_223_0 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc223 ) * ( 1.0 - blendOpDest223 ) ) ));
				float4 temp_cast_23 = (lerpResult368).xxxx;
				#if defined(_SPARKLESOURCE_NONE)
				float4 staticSwitch260 = temp_cast_20;
				#elif defined(_SPARKLESOURCE_SMOOTHNESSSPARKLES)
				float4 staticSwitch260 = (( _BlendingModeCrystals )?( temp_output_223_0 ):( temp_output_227_0 ));
				#elif defined(_SPARKLESOURCE_EMISSIVESPARKLES)
				float4 staticSwitch260 = temp_cast_19;
				#else
				float4 staticSwitch260 = temp_cast_19;
				#endif
				float4 Smoothness71 = staticSwitch260;
				
				float blendOpSrc34_g1238 = tex2DNode3_g1238.g;
				float blendOpDest34_g1238 = ( 1.0 - _AoIntensity );
				float AOG47 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc34_g1238 ) * ( 1.0 - blendOpDest34_g1238 ) ) ));
				float lerpResult380 = lerp( AOG47 , DetailAOG324 , Mask78);
				float AmbientOcclusion52 = lerpResult380;
				
				float3 Albedo = staticSwitch242.rgb;
				float3 Normal = Normal35;
				float3 Emission = Emission53.rgb;
				float3 Specular = 0.5;
				float Metallic = Metallic54;
				float Smoothness = Smoothness71.r;
				float Occlusion = AmbientOcclusion52;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70403

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailAlbedoMap_ST;
			float4 _DetailNormalMap_ST;
			float4 _DetailMask_ST;
			float4 _BumpMap_ST;
			float4 _DetailMAoHSMap_ST;
			float4 _GlobalXYTilingXYZWOffsetXYSnow;
			float4 _EmissionMap_ST;
			float4 _DetailColor;
			float4 _EmissionColor;
			float4 _MetallicGlossMapMAHS_ST;
			float4 _GlobalXYTilingXYZWOffsetXY;
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _TilingCrystals;
			float _ContrastSparkles;
			float _AmountSparkle;
			float _CrystalsIntensity;
			float _BlendNormal;
			float _BumpScale;
			float _IntensityDotMask;
			float _Desaturatecrystals;
			float _VisualizeMask;
			float _AoMask;
			float _EmissionIntensity;
			float _ContrastDotMask;
			float _AoIntensityDetail;
			float _SparklePower;
			float _ShadowMask;
			float _UseEmissionFromMainProperties;
			float _Metallic;
			float _MetallicDetail;
			float _Glossiness;
			float _GlossinessDetail;
			float _DetailNormalMapScale;
			float _SpreadDotMask;
			float _AmountBottom;
			float _TilingDotMask;
			float _Brightness;
			float _Parallax;
			float _Saturation;
			float _DetailBrightness;
			float _ParallaxDetail;
			float _DetailSaturation;
			float _ExcludeMask;
			float _ContrastDetailMap;
			float _InvertMask;
			float _InvertABaseColor1;
			float _SpreadDetailMap;
			float _TilingInstance;
			float _TopCoverageMask;
			float _EnableTopAccum;
			float _AmountTop;
			float _NoiseScale;
			float _SharpenEdge;
			float _GradientTop;
			float _TopCoverage;
			float _SnowTopIntensity;
			float _EnableBottomAccum;
			float _BlendingModeCrystals;
			float _GradientBottom;
			float _SnowBottomIntensity;
			float _IntensityMask;
			float _AoIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70403

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailAlbedoMap_ST;
			float4 _DetailNormalMap_ST;
			float4 _DetailMask_ST;
			float4 _BumpMap_ST;
			float4 _DetailMAoHSMap_ST;
			float4 _GlobalXYTilingXYZWOffsetXYSnow;
			float4 _EmissionMap_ST;
			float4 _DetailColor;
			float4 _EmissionColor;
			float4 _MetallicGlossMapMAHS_ST;
			float4 _GlobalXYTilingXYZWOffsetXY;
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _TilingCrystals;
			float _ContrastSparkles;
			float _AmountSparkle;
			float _CrystalsIntensity;
			float _BlendNormal;
			float _BumpScale;
			float _IntensityDotMask;
			float _Desaturatecrystals;
			float _VisualizeMask;
			float _AoMask;
			float _EmissionIntensity;
			float _ContrastDotMask;
			float _AoIntensityDetail;
			float _SparklePower;
			float _ShadowMask;
			float _UseEmissionFromMainProperties;
			float _Metallic;
			float _MetallicDetail;
			float _Glossiness;
			float _GlossinessDetail;
			float _DetailNormalMapScale;
			float _SpreadDotMask;
			float _AmountBottom;
			float _TilingDotMask;
			float _Brightness;
			float _Parallax;
			float _Saturation;
			float _DetailBrightness;
			float _ParallaxDetail;
			float _DetailSaturation;
			float _ExcludeMask;
			float _ContrastDetailMap;
			float _InvertMask;
			float _InvertABaseColor1;
			float _SpreadDetailMap;
			float _TilingInstance;
			float _TopCoverageMask;
			float _EnableTopAccum;
			float _AmountTop;
			float _NoiseScale;
			float _SharpenEdge;
			float _GradientTop;
			float _TopCoverage;
			float _SnowTopIntensity;
			float _EnableBottomAccum;
			float _BlendingModeCrystals;
			float _GradientBottom;
			float _SnowBottomIntensity;
			float _IntensityMask;
			float _AoIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70403

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_FRAG_SHADOWCOORDS
			#pragma shader_feature_local _VISUALIZESPARKLE_NONE _VISUALIZESPARKLE_DOTMASK _VISUALIZESPARKLE_SPARKLEMAP
			#pragma shader_feature_local _SOURCEMASK_DETAILMASK _SOURCEMASK_BASECOLORALPHA _SOURCEMASK_BOTH
			#pragma shader_feature_local _SOURCEEDGEBLENDTOP_NONE _SOURCEEDGEBLENDTOP_HEIGHTMAPB _SOURCEEDGEBLENDTOP_NOISE _SOURCEEDGEBLENDTOP_BOTH
			#pragma shader_feature_local _SPARKLESOURCE_NONE _SPARKLESOURCE_SMOOTHNESSSPARKLES _SPARKLESOURCE_EMISSIVESPARKLES
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailAlbedoMap_ST;
			float4 _DetailNormalMap_ST;
			float4 _DetailMask_ST;
			float4 _BumpMap_ST;
			float4 _DetailMAoHSMap_ST;
			float4 _GlobalXYTilingXYZWOffsetXYSnow;
			float4 _EmissionMap_ST;
			float4 _DetailColor;
			float4 _EmissionColor;
			float4 _MetallicGlossMapMAHS_ST;
			float4 _GlobalXYTilingXYZWOffsetXY;
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _TilingCrystals;
			float _ContrastSparkles;
			float _AmountSparkle;
			float _CrystalsIntensity;
			float _BlendNormal;
			float _BumpScale;
			float _IntensityDotMask;
			float _Desaturatecrystals;
			float _VisualizeMask;
			float _AoMask;
			float _EmissionIntensity;
			float _ContrastDotMask;
			float _AoIntensityDetail;
			float _SparklePower;
			float _ShadowMask;
			float _UseEmissionFromMainProperties;
			float _Metallic;
			float _MetallicDetail;
			float _Glossiness;
			float _GlossinessDetail;
			float _DetailNormalMapScale;
			float _SpreadDotMask;
			float _AmountBottom;
			float _TilingDotMask;
			float _Brightness;
			float _Parallax;
			float _Saturation;
			float _DetailBrightness;
			float _ParallaxDetail;
			float _DetailSaturation;
			float _ExcludeMask;
			float _ContrastDetailMap;
			float _InvertMask;
			float _InvertABaseColor1;
			float _SpreadDetailMap;
			float _TilingInstance;
			float _TopCoverageMask;
			float _EnableTopAccum;
			float _AmountTop;
			float _NoiseScale;
			float _SharpenEdge;
			float _GradientTop;
			float _TopCoverage;
			float _SnowTopIntensity;
			float _EnableBottomAccum;
			float _BlendingModeCrystals;
			float _GradientBottom;
			float _SnowBottomIntensity;
			float _IntensityMask;
			float _AoIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _BaseMap;
			sampler2D _MetallicGlossMapMAHS;
			sampler2D _DetailAlbedoMap;
			sampler2D _DetailMAoHSMap;
			sampler2D _DetailMask;
			sampler2D _SparkleMask;
			sampler2D _EmissionMap;


			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			
			float4 CalculateContrast( float contrastValue, float4 colorTarget )
			{
				float t = 0.5 * ( 1.0 - contrastValue );
				return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
			}
			//https://www.shadertoy.com/view/XdXGW8
			float2 GradientNoiseDir( float2 x )
			{
				const float2 k = float2( 0.3183099, 0.3678794 );
				x = x * k + k.yx;
				return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
			}
			
			float GradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 i = floor( p );
				float2 f = frac( p );
				float2 u = f * f * ( 3.0 - 2.0 * f );
				return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
						lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord5.xyz = ase_worldBitangent;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord6 = v.vertex;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_BaseMap = IN.ase_texcoord2.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				float2 break26_g1528 = uv_BaseMap;
				float GlobalTilingX5 = ( _GlobalXYTilingXYZWOffsetXY.x - 1.0 );
				float GlobalTilingY4 = ( _GlobalXYTilingXYZWOffsetXY.y - 1.0 );
				float2 appendResult14_g1528 = (float2(( break26_g1528.x * GlobalTilingX5 ) , ( break26_g1528.y * GlobalTilingY4 )));
				float GlobalOffsetX6 = _GlobalXYTilingXYZWOffsetXY.z;
				float GlobalOffsetY3 = _GlobalXYTilingXYZWOffsetXY.w;
				float2 appendResult13_g1528 = (float2(( break26_g1528.x + GlobalOffsetX6 ) , ( break26_g1528.y + GlobalOffsetY3 )));
				float2 uv_MetallicGlossMapMAHS = IN.ase_texcoord2.xy * _MetallicGlossMapMAHS_ST.xy + _MetallicGlossMapMAHS_ST.zw;
				float2 break26_g1239 = uv_MetallicGlossMapMAHS;
				float2 appendResult14_g1239 = (float2(( break26_g1239.x * GlobalTilingX5 ) , ( break26_g1239.y * GlobalTilingY4 )));
				float2 appendResult13_g1239 = (float2(( break26_g1239.x + GlobalOffsetX6 ) , ( break26_g1239.y + GlobalOffsetY3 )));
				float4 tex2DNode3_g1238 = tex2D( _MetallicGlossMapMAHS, ( ( appendResult14_g1239 + appendResult13_g1239 ) + float2( 0,0 ) ) );
				float3 ase_worldTangent = IN.ase_texcoord3.xyz;
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 paralaxOffset38_g1238 = ParallaxOffset( tex2DNode3_g1238.b , _Parallax , ase_tanViewDir );
				float2 switchResult37_g1238 = (((ase_vface>0)?(paralaxOffset38_g1238):(0.0)));
				float2 Parallax23 = switchResult37_g1238;
				float4 tex2DNode7_g1527 = tex2D( _BaseMap, ( ( appendResult14_g1528 + appendResult13_g1528 ) + Parallax23 ) );
				float clampResult27_g1527 = clamp( _Saturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1527 = ( _BaseColor * tex2DNode7_g1527 ).rgb;
				float desaturateDot29_g1527 = dot( desaturateInitialColor29_g1527, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1527 = lerp( desaturateInitialColor29_g1527, desaturateDot29_g1527.xxx, -clampResult27_g1527 );
				float4 temp_output_712_0 = CalculateContrast(_Brightness,float4( desaturateVar29_g1527 , 0.0 ));
				float2 uv_DetailAlbedoMap = IN.ase_texcoord2.xy * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
				float2 break26_g1594 = uv_DetailAlbedoMap;
				float GlobalTilingDX312 = ( _GlobalXYTilingXYZWOffsetXYSnow.x - 1.0 );
				float GlobalTilingDY311 = ( _GlobalXYTilingXYZWOffsetXYSnow.y - 1.0 );
				float2 appendResult14_g1594 = (float2(( break26_g1594.x * GlobalTilingDX312 ) , ( break26_g1594.y * GlobalTilingDY311 )));
				float GlobalOffsetDX310 = _GlobalXYTilingXYZWOffsetXYSnow.z;
				float GlobalOffsetDY313 = _GlobalXYTilingXYZWOffsetXYSnow.w;
				float2 appendResult13_g1594 = (float2(( break26_g1594.x + GlobalOffsetDX310 ) , ( break26_g1594.y + GlobalOffsetDY313 )));
				float2 uv_DetailMAoHSMap = IN.ase_texcoord2.xy * _DetailMAoHSMap_ST.xy + _DetailMAoHSMap_ST.zw;
				float2 break26_g1530 = uv_DetailMAoHSMap;
				float2 appendResult14_g1530 = (float2(( break26_g1530.x * GlobalTilingDX312 ) , ( break26_g1530.y * GlobalTilingDY311 )));
				float2 appendResult13_g1530 = (float2(( break26_g1530.x + GlobalOffsetDX310 ) , ( break26_g1530.y + GlobalOffsetDY313 )));
				float4 tex2DNode3_g1529 = tex2D( _DetailMAoHSMap, ( ( appendResult14_g1530 + appendResult13_g1530 ) + float2( 0,0 ) ) );
				float2 paralaxOffset38_g1529 = ParallaxOffset( tex2DNode3_g1529.b , _ParallaxDetail , ase_tanViewDir );
				float2 switchResult37_g1529 = (((ase_vface>0)?(paralaxOffset38_g1529):(0.0)));
				float2 DetailParallax323 = switchResult37_g1529;
				float4 tex2DNode7_g1593 = tex2D( _DetailAlbedoMap, ( ( appendResult14_g1594 + appendResult13_g1594 ) + DetailParallax323 ) );
				float4 lerpResult52_g1593 = lerp( _DetailColor , ( ( _DetailColor * tex2DNode7_g1593 ) * _DetailColor.a ) , _DetailColor.a);
				float clampResult27_g1593 = clamp( _DetailSaturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1593 = lerpResult52_g1593.rgb;
				float desaturateDot29_g1593 = dot( desaturateInitialColor29_g1593, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1593 = lerp( desaturateInitialColor29_g1593, desaturateDot29_g1593.xxx, -clampResult27_g1593 );
				float4 DetailBaseColor301 = CalculateContrast(_DetailBrightness,float4( desaturateVar29_g1593 , 0.0 ));
				float temp_output_34_0_g1531 = ( _ContrastDetailMap + 1.0 );
				float2 uv_DetailMask = IN.ase_texcoord2.xy * _DetailMask_ST.xy + _DetailMask_ST.zw;
				float4 tex2DNode27_g1531 = tex2Dlod( _DetailMask, float4( uv_DetailMask, 0, 0.0) );
				float BaseColorAlpha14 = (( _InvertABaseColor1 )?( ( 1.0 - tex2DNode7_g1527.a ) ):( tex2DNode7_g1527.a ));
				float temp_output_46_0_g1531 = BaseColorAlpha14;
				#if defined(_SOURCEMASK_DETAILMASK)
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#elif defined(_SOURCEMASK_BASECOLORALPHA)
				float staticSwitch61_g1531 = temp_output_46_0_g1531;
				#elif defined(_SOURCEMASK_BOTH)
				float staticSwitch61_g1531 = ( (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) )) + temp_output_46_0_g1531 );
				#else
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#endif
				float temp_output_37_0_g1531 = ( staticSwitch61_g1531 + (-1.2 + (_SpreadDetailMap - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float4 temp_cast_4 = (temp_output_37_0_g1531).xxxx;
				float clampResult38_g1531 = clamp( (( _ExcludeMask )?( ( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r * ( 1.0 - temp_output_46_0_g1531 ) ) ):( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r )) , 0.0 , 1.0 );
				float MaskWorldY458 = ase_worldNormal.y;
				float4 transform472 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord6.xyz , 0.0 ));
				float lerpResult464 = lerp( -1.0 , 2.0 , _AmountTop);
				float temp_output_466_0 = ( ( ( transform472.y - 1.0 ) / 2.0 ) + lerpResult464 );
				float HeightmapB59 = tex2DNode3_g1238.b;
				float temp_output_513_0 = ( 1.0 - HeightmapB59 );
				float2 texCoord591 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float gradientNoise590 = GradientNoise(texCoord591,_NoiseScale);
				gradientNoise590 = gradientNoise590*0.5 + 0.5;
				float temp_output_589_0 = ( 1.0 - gradientNoise590 );
				float blendOpSrc593 = temp_output_513_0;
				float blendOpDest593 = temp_output_589_0;
				#if defined(_SOURCEEDGEBLENDTOP_NONE)
				float staticSwitch560 = 1.0;
				#elif defined(_SOURCEEDGEBLENDTOP_HEIGHTMAPB)
				float staticSwitch560 = temp_output_513_0;
				#elif defined(_SOURCEEDGEBLENDTOP_NOISE)
				float staticSwitch560 = temp_output_589_0;
				#elif defined(_SOURCEEDGEBLENDTOP_BOTH)
				float staticSwitch560 = ( saturate( max( blendOpSrc593, blendOpDest593 ) ));
				#else
				float staticSwitch560 = 1.0;
				#endif
				float temp_output_510_0 = (-0.5 + (( IN.ase_color.r + staticSwitch560 ) - 0.0) * (0.1 - -0.5) / (1.0 - 0.0));
				float clampResult524 = clamp( _SharpenEdge , 0.0 , 100000.0 );
				float lerpResult511 = lerp( temp_output_510_0 , staticSwitch560 , clampResult524);
				float temp_output_507_0 = ( temp_output_510_0 - lerpResult511 );
				float clampResult467 = clamp( _GradientTop , 0.0 , 100000.0 );
				float blendOpSrc471 = ( temp_output_466_0 - temp_output_507_0 );
				float blendOpDest471 = ( temp_output_466_0 * clampResult467 );
				float clampResult469 = clamp( ( ( ( saturate( ( blendOpSrc471 + blendOpDest471 ) )) * (( _TopCoverage )?( 1.0 ):( MaskWorldY458 )) ) * _SnowTopIntensity ) , 0.0 , 1.0 );
				float4 transform600 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord6.xyz , 0.0 ));
				float lerpResult602 = lerp( -4.0 , 2.0 , _AmountBottom);
				float temp_output_605_0 = ( ( 1.0 - ( ( transform600.y - 1.0 ) / 2.0 ) ) + lerpResult602 );
				float clampResult608 = clamp( _GradientBottom , 0.0 , 100000.0 );
				float blendOpSrc610 = ( temp_output_605_0 - temp_output_507_0 );
				float blendOpDest610 = ( temp_output_605_0 * clampResult608 );
				float clampResult614 = clamp( ( ( saturate( ( blendOpSrc610 + blendOpDest610 ) )) * _SnowBottomIntensity ) , 0.0 , 1.0 );
				float blendOpSrc616 = (( _EnableTopAccum )?( clampResult469 ):( 0.0 ));
				float blendOpDest616 = (( _EnableBottomAccum )?( clampResult614 ):( 0.0 ));
				float SnowAccumTop470 = ( saturate( max( blendOpSrc616, blendOpDest616 ) ));
				float blendOpSrc517 = ( ( clampResult38_g1531 * (( _TopCoverageMask )?( saturate( MaskWorldY458 ) ):( 1.0 )) ) * _IntensityMask );
				float blendOpDest517 = SnowAccumTop470;
				float Mask78 = ( saturate( max( blendOpSrc517, blendOpDest517 ) ));
				float4 lerpResult347 = lerp( temp_output_712_0 , DetailBaseColor301 , Mask78);
				float4 temp_cast_7 = (Mask78).xxxx;
				float4 Albedo15 = (( _VisualizeMask )?( temp_cast_7 ):( lerpResult347 ));
				float2 texCoord106_g1599 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult149_g1599 = clamp( _TilingInstance , 0.0 , 1000.0 );
				float2 temp_output_175_0_g1599 = DetailParallax323;
				float temp_output_123_0_g1599 = ( tex2D( _SparkleMask, ( ( ( texCoord106_g1599 * _TilingDotMask ) * clampResult149_g1599 ) + temp_output_175_0_g1599 ) ).a + (-1.2 + (_SpreadDotMask - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float saferPower230_g1599 = max( temp_output_123_0_g1599 , 0.0001 );
				float temp_output_121_0_g1599 = ( _ContrastDotMask + 1.0 );
				float clampResult118_g1599 = clamp( pow( saferPower230_g1599 , temp_output_121_0_g1599 ) , 0.0 , 1.0 );
				float temp_output_119_0_g1599 = ( clampResult118_g1599 * _IntensityDotMask );
				float VisualizeDotMap230 = temp_output_119_0_g1599;
				float4 temp_cast_8 = (VisualizeDotMap230).xxxx;
				float temp_output_135_0_g1599 = ( _ContrastSparkles - -1.0 );
				float4 unityObjectToClipPos78_g1599 = TransformWorldToHClip(TransformObjectToWorld(IN.ase_texcoord6.xyz));
				float clampResult49_g1599 = clamp( _Desaturatecrystals , 0.0 , 1.0 );
				float3 desaturateInitialColor53_g1599 = tex2D( _SparkleMask, ( ( ( unityObjectToClipPos78_g1599 * ( _TilingCrystals / 10.0 ) ) * clampResult149_g1599 ) + float4( temp_output_175_0_g1599, 0.0 , 0.0 ) ).xy ).rgb;
				float desaturateDot53_g1599 = dot( desaturateInitialColor53_g1599, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar53_g1599 = lerp( desaturateInitialColor53_g1599, desaturateDot53_g1599.xxx, clampResult49_g1599 );
				float lerpResult246_g1599 = lerp( 0.6 , 1.0 , _AmountSparkle);
				float4 temp_cast_13 = (1.0).xxxx;
				float4 clampResult74_g1599 = clamp( ( saturate( ( CalculateContrast(temp_output_135_0_g1599,float4( desaturateVar53_g1599 , 0.0 )) + (( -1.0 - temp_output_135_0_g1599 ) + (lerpResult246_g1599 - 0.0) * (0.0 - ( -1.0 - temp_output_135_0_g1599 )) / (1.0 - 0.0)) ) ) * _CrystalsIntensity ) , float4( 0,0,0,0 ) , temp_cast_13 );
				float4 VisualizeSparklesMap231 = clampResult74_g1599;
				#if defined(_VISUALIZESPARKLE_NONE)
				float4 staticSwitch242 = Albedo15;
				#elif defined(_VISUALIZESPARKLE_DOTMASK)
				float4 staticSwitch242 = temp_cast_8;
				#elif defined(_VISUALIZESPARKLE_SPARKLEMAP)
				float4 staticSwitch242 = VisualizeSparklesMap231;
				#else
				float4 staticSwitch242 = Albedo15;
				#endif
				
				float2 uv_EmissionMap = IN.ase_texcoord2.xy * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
				float2 break26_g1605 = uv_EmissionMap;
				float2 appendResult14_g1605 = (float2(( break26_g1605.x * GlobalTilingX5 ) , ( break26_g1605.y * GlobalTilingY4 )));
				float2 appendResult13_g1605 = (float2(( break26_g1605.x + GlobalOffsetX6 ) , ( break26_g1605.y + GlobalOffsetY3 )));
				float4 temp_output_182_0 = ( _EmissionColor * tex2D( _EmissionMap, ( ( appendResult14_g1605 + appendResult13_g1605 ) + Parallax23 ) ) * _EmissionIntensity );
				float clampResult212_g1599 = clamp( temp_output_119_0_g1599 , 0.0 , 1.0 );
				float4 temp_output_108_0_g1599 = ( clampResult74_g1599 * clampResult212_g1599 );
				float blendOpSrc34_g1529 = tex2DNode3_g1529.g;
				float blendOpDest34_g1529 = ( 1.0 - _AoIntensityDetail );
				float DetailAOG324 = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc34_g1529 ) * ( 1.0 - blendOpDest34_g1529 ) ) ));
				float4 temp_cast_15 = (DetailAOG324).xxxx;
				float dotResult152_g1599 = dot( ase_worldNormal , SafeNormalize(_MainLightPosition.xyz) );
				float temp_output_153_0_g1599 = max( dotResult152_g1599 , 0.0 );
				float clampResult240_g1599 = clamp( _SparklePower , 0.0 , 100000.0 );
				float ase_lightAtten = 0;
				Light ase_lightAtten_mainLight = GetMainLight( ShadowCoords );
				ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;
				float lerpResult258_g1599 = lerp( 0.0 , clampResult240_g1599 , ( temp_output_153_0_g1599 * ( ase_lightAtten * _MainLightColor.a ) ));
				float lerpResult169_g1599 = lerp( ( temp_output_153_0_g1599 * _MainLightColor.a ) , lerpResult258_g1599 , _ShadowMask);
				float4 temp_output_165_0_g1599 = ( (( _AoMask )?( ( temp_output_108_0_g1599 * temp_cast_15 ) ):( temp_output_108_0_g1599 )) * lerpResult169_g1599 );
				float saferPower709 = max( Mask78 , 0.0001 );
				float MaskandAccum518 = pow( saferPower709 , 5.0 );
				float blendOpSrc506 = MaskandAccum518;
				float blendOpDest506 = SnowAccumTop470;
				float4 temp_output_174_0 = ( temp_output_165_0_g1599 * ( saturate( ( 1.0 - ( 1.0 - blendOpSrc506 ) * ( 1.0 - blendOpDest506 ) ) )) );
				float4 SparklesCrystals173 = temp_output_174_0;
				float4 lerpResult392 = lerp( temp_output_182_0 , SparklesCrystals173 , MaskandAccum518);
				float4 blendOpSrc400 = temp_output_182_0;
				float4 blendOpDest400 = SparklesCrystals173;
				float4 lerpResult398 = lerp( lerpResult392 , ( saturate( ( blendOpSrc400 + blendOpDest400 ) )) , _UseEmissionFromMainProperties);
				float4 temp_cast_17 = (0.0).xxxx;
				float4 lerpResult374 = lerp( temp_output_182_0 , temp_cast_17 , MaskandAccum518);
				float4 lerpResult395 = lerp( lerpResult374 , temp_output_182_0 , _UseEmissionFromMainProperties);
				#if defined(_SPARKLESOURCE_NONE)
				float4 staticSwitch252 = lerpResult395;
				#elif defined(_SPARKLESOURCE_SMOOTHNESSSPARKLES)
				float4 staticSwitch252 = lerpResult395;
				#elif defined(_SPARKLESOURCE_EMISSIVESPARKLES)
				float4 staticSwitch252 = lerpResult398;
				#else
				float4 staticSwitch252 = lerpResult398;
				#endif
				float4 Emission53 = staticSwitch252;
				
				
				float3 Albedo = staticSwitch242.rgb;
				float3 Emission = Emission53.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 70403

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#pragma shader_feature_local _VISUALIZESPARKLE_NONE _VISUALIZESPARKLE_DOTMASK _VISUALIZESPARKLE_SPARKLEMAP
			#pragma shader_feature_local _SOURCEMASK_DETAILMASK _SOURCEMASK_BASECOLORALPHA _SOURCEMASK_BOTH
			#pragma shader_feature_local _SOURCEEDGEBLENDTOP_NONE _SOURCEEDGEBLENDTOP_HEIGHTMAPB _SOURCEEDGEBLENDTOP_NOISE _SOURCEEDGEBLENDTOP_BOTH


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailAlbedoMap_ST;
			float4 _DetailNormalMap_ST;
			float4 _DetailMask_ST;
			float4 _BumpMap_ST;
			float4 _DetailMAoHSMap_ST;
			float4 _GlobalXYTilingXYZWOffsetXYSnow;
			float4 _EmissionMap_ST;
			float4 _DetailColor;
			float4 _EmissionColor;
			float4 _MetallicGlossMapMAHS_ST;
			float4 _GlobalXYTilingXYZWOffsetXY;
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float _TilingCrystals;
			float _ContrastSparkles;
			float _AmountSparkle;
			float _CrystalsIntensity;
			float _BlendNormal;
			float _BumpScale;
			float _IntensityDotMask;
			float _Desaturatecrystals;
			float _VisualizeMask;
			float _AoMask;
			float _EmissionIntensity;
			float _ContrastDotMask;
			float _AoIntensityDetail;
			float _SparklePower;
			float _ShadowMask;
			float _UseEmissionFromMainProperties;
			float _Metallic;
			float _MetallicDetail;
			float _Glossiness;
			float _GlossinessDetail;
			float _DetailNormalMapScale;
			float _SpreadDotMask;
			float _AmountBottom;
			float _TilingDotMask;
			float _Brightness;
			float _Parallax;
			float _Saturation;
			float _DetailBrightness;
			float _ParallaxDetail;
			float _DetailSaturation;
			float _ExcludeMask;
			float _ContrastDetailMap;
			float _InvertMask;
			float _InvertABaseColor1;
			float _SpreadDetailMap;
			float _TilingInstance;
			float _TopCoverageMask;
			float _EnableTopAccum;
			float _AmountTop;
			float _NoiseScale;
			float _SharpenEdge;
			float _GradientTop;
			float _TopCoverage;
			float _SnowTopIntensity;
			float _EnableBottomAccum;
			float _BlendingModeCrystals;
			float _GradientBottom;
			float _SnowBottomIntensity;
			float _IntensityMask;
			float _AoIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _BaseMap;
			sampler2D _MetallicGlossMapMAHS;
			sampler2D _DetailAlbedoMap;
			sampler2D _DetailMAoHSMap;
			sampler2D _DetailMask;
			sampler2D _SparkleMask;


			inline float2 ParallaxOffset( half h, half height, half3 viewDir )
			{
				h = h * height - height/2.0;
				float3 v = normalize( viewDir );
				v.z += 0.42;
				return h* (v.xy / v.z);
			}
			
			float4 CalculateContrast( float contrastValue, float4 colorTarget )
			{
				float t = 0.5 * ( 1.0 - contrastValue );
				return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
			}
			//https://www.shadertoy.com/view/XdXGW8
			float2 GradientNoiseDir( float2 x )
			{
				const float2 k = float2( 0.3183099, 0.3678794 );
				x = x * k + k.yx;
				return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
			}
			
			float GradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 i = floor( p );
				float2 f = frac( p );
				float2 u = f * f * ( 3.0 - 2.0 * f );
				return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
						lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord4.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord5.xyz = ase_worldBitangent;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord6 = v.vertex;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_tangent = v.ase_tangent;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN , FRONT_FACE_TYPE ase_vface : FRONT_FACE_SEMANTIC ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_BaseMap = IN.ase_texcoord2.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				float2 break26_g1528 = uv_BaseMap;
				float GlobalTilingX5 = ( _GlobalXYTilingXYZWOffsetXY.x - 1.0 );
				float GlobalTilingY4 = ( _GlobalXYTilingXYZWOffsetXY.y - 1.0 );
				float2 appendResult14_g1528 = (float2(( break26_g1528.x * GlobalTilingX5 ) , ( break26_g1528.y * GlobalTilingY4 )));
				float GlobalOffsetX6 = _GlobalXYTilingXYZWOffsetXY.z;
				float GlobalOffsetY3 = _GlobalXYTilingXYZWOffsetXY.w;
				float2 appendResult13_g1528 = (float2(( break26_g1528.x + GlobalOffsetX6 ) , ( break26_g1528.y + GlobalOffsetY3 )));
				float2 uv_MetallicGlossMapMAHS = IN.ase_texcoord2.xy * _MetallicGlossMapMAHS_ST.xy + _MetallicGlossMapMAHS_ST.zw;
				float2 break26_g1239 = uv_MetallicGlossMapMAHS;
				float2 appendResult14_g1239 = (float2(( break26_g1239.x * GlobalTilingX5 ) , ( break26_g1239.y * GlobalTilingY4 )));
				float2 appendResult13_g1239 = (float2(( break26_g1239.x + GlobalOffsetX6 ) , ( break26_g1239.y + GlobalOffsetY3 )));
				float4 tex2DNode3_g1238 = tex2D( _MetallicGlossMapMAHS, ( ( appendResult14_g1239 + appendResult13_g1239 ) + float2( 0,0 ) ) );
				float3 ase_worldTangent = IN.ase_texcoord3.xyz;
				float3 ase_worldNormal = IN.ase_texcoord4.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord5.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float2 paralaxOffset38_g1238 = ParallaxOffset( tex2DNode3_g1238.b , _Parallax , ase_tanViewDir );
				float2 switchResult37_g1238 = (((ase_vface>0)?(paralaxOffset38_g1238):(0.0)));
				float2 Parallax23 = switchResult37_g1238;
				float4 tex2DNode7_g1527 = tex2D( _BaseMap, ( ( appendResult14_g1528 + appendResult13_g1528 ) + Parallax23 ) );
				float clampResult27_g1527 = clamp( _Saturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1527 = ( _BaseColor * tex2DNode7_g1527 ).rgb;
				float desaturateDot29_g1527 = dot( desaturateInitialColor29_g1527, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1527 = lerp( desaturateInitialColor29_g1527, desaturateDot29_g1527.xxx, -clampResult27_g1527 );
				float4 temp_output_712_0 = CalculateContrast(_Brightness,float4( desaturateVar29_g1527 , 0.0 ));
				float2 uv_DetailAlbedoMap = IN.ase_texcoord2.xy * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
				float2 break26_g1594 = uv_DetailAlbedoMap;
				float GlobalTilingDX312 = ( _GlobalXYTilingXYZWOffsetXYSnow.x - 1.0 );
				float GlobalTilingDY311 = ( _GlobalXYTilingXYZWOffsetXYSnow.y - 1.0 );
				float2 appendResult14_g1594 = (float2(( break26_g1594.x * GlobalTilingDX312 ) , ( break26_g1594.y * GlobalTilingDY311 )));
				float GlobalOffsetDX310 = _GlobalXYTilingXYZWOffsetXYSnow.z;
				float GlobalOffsetDY313 = _GlobalXYTilingXYZWOffsetXYSnow.w;
				float2 appendResult13_g1594 = (float2(( break26_g1594.x + GlobalOffsetDX310 ) , ( break26_g1594.y + GlobalOffsetDY313 )));
				float2 uv_DetailMAoHSMap = IN.ase_texcoord2.xy * _DetailMAoHSMap_ST.xy + _DetailMAoHSMap_ST.zw;
				float2 break26_g1530 = uv_DetailMAoHSMap;
				float2 appendResult14_g1530 = (float2(( break26_g1530.x * GlobalTilingDX312 ) , ( break26_g1530.y * GlobalTilingDY311 )));
				float2 appendResult13_g1530 = (float2(( break26_g1530.x + GlobalOffsetDX310 ) , ( break26_g1530.y + GlobalOffsetDY313 )));
				float4 tex2DNode3_g1529 = tex2D( _DetailMAoHSMap, ( ( appendResult14_g1530 + appendResult13_g1530 ) + float2( 0,0 ) ) );
				float2 paralaxOffset38_g1529 = ParallaxOffset( tex2DNode3_g1529.b , _ParallaxDetail , ase_tanViewDir );
				float2 switchResult37_g1529 = (((ase_vface>0)?(paralaxOffset38_g1529):(0.0)));
				float2 DetailParallax323 = switchResult37_g1529;
				float4 tex2DNode7_g1593 = tex2D( _DetailAlbedoMap, ( ( appendResult14_g1594 + appendResult13_g1594 ) + DetailParallax323 ) );
				float4 lerpResult52_g1593 = lerp( _DetailColor , ( ( _DetailColor * tex2DNode7_g1593 ) * _DetailColor.a ) , _DetailColor.a);
				float clampResult27_g1593 = clamp( _DetailSaturation , -1.0 , 100.0 );
				float3 desaturateInitialColor29_g1593 = lerpResult52_g1593.rgb;
				float desaturateDot29_g1593 = dot( desaturateInitialColor29_g1593, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar29_g1593 = lerp( desaturateInitialColor29_g1593, desaturateDot29_g1593.xxx, -clampResult27_g1593 );
				float4 DetailBaseColor301 = CalculateContrast(_DetailBrightness,float4( desaturateVar29_g1593 , 0.0 ));
				float temp_output_34_0_g1531 = ( _ContrastDetailMap + 1.0 );
				float2 uv_DetailMask = IN.ase_texcoord2.xy * _DetailMask_ST.xy + _DetailMask_ST.zw;
				float4 tex2DNode27_g1531 = tex2Dlod( _DetailMask, float4( uv_DetailMask, 0, 0.0) );
				float BaseColorAlpha14 = (( _InvertABaseColor1 )?( ( 1.0 - tex2DNode7_g1527.a ) ):( tex2DNode7_g1527.a ));
				float temp_output_46_0_g1531 = BaseColorAlpha14;
				#if defined(_SOURCEMASK_DETAILMASK)
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#elif defined(_SOURCEMASK_BASECOLORALPHA)
				float staticSwitch61_g1531 = temp_output_46_0_g1531;
				#elif defined(_SOURCEMASK_BOTH)
				float staticSwitch61_g1531 = ( (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) )) + temp_output_46_0_g1531 );
				#else
				float staticSwitch61_g1531 = (( _InvertMask )?( tex2DNode27_g1531.r ):( ( 1.0 - tex2DNode27_g1531.r ) ));
				#endif
				float temp_output_37_0_g1531 = ( staticSwitch61_g1531 + (-1.2 + (_SpreadDetailMap - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float4 temp_cast_4 = (temp_output_37_0_g1531).xxxx;
				float clampResult38_g1531 = clamp( (( _ExcludeMask )?( ( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r * ( 1.0 - temp_output_46_0_g1531 ) ) ):( CalculateContrast(temp_output_34_0_g1531,temp_cast_4).r )) , 0.0 , 1.0 );
				float MaskWorldY458 = ase_worldNormal.y;
				float4 transform472 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord6.xyz , 0.0 ));
				float lerpResult464 = lerp( -1.0 , 2.0 , _AmountTop);
				float temp_output_466_0 = ( ( ( transform472.y - 1.0 ) / 2.0 ) + lerpResult464 );
				float HeightmapB59 = tex2DNode3_g1238.b;
				float temp_output_513_0 = ( 1.0 - HeightmapB59 );
				float2 texCoord591 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float gradientNoise590 = GradientNoise(texCoord591,_NoiseScale);
				gradientNoise590 = gradientNoise590*0.5 + 0.5;
				float temp_output_589_0 = ( 1.0 - gradientNoise590 );
				float blendOpSrc593 = temp_output_513_0;
				float blendOpDest593 = temp_output_589_0;
				#if defined(_SOURCEEDGEBLENDTOP_NONE)
				float staticSwitch560 = 1.0;
				#elif defined(_SOURCEEDGEBLENDTOP_HEIGHTMAPB)
				float staticSwitch560 = temp_output_513_0;
				#elif defined(_SOURCEEDGEBLENDTOP_NOISE)
				float staticSwitch560 = temp_output_589_0;
				#elif defined(_SOURCEEDGEBLENDTOP_BOTH)
				float staticSwitch560 = ( saturate( max( blendOpSrc593, blendOpDest593 ) ));
				#else
				float staticSwitch560 = 1.0;
				#endif
				float temp_output_510_0 = (-0.5 + (( IN.ase_color.r + staticSwitch560 ) - 0.0) * (0.1 - -0.5) / (1.0 - 0.0));
				float clampResult524 = clamp( _SharpenEdge , 0.0 , 100000.0 );
				float lerpResult511 = lerp( temp_output_510_0 , staticSwitch560 , clampResult524);
				float temp_output_507_0 = ( temp_output_510_0 - lerpResult511 );
				float clampResult467 = clamp( _GradientTop , 0.0 , 100000.0 );
				float blendOpSrc471 = ( temp_output_466_0 - temp_output_507_0 );
				float blendOpDest471 = ( temp_output_466_0 * clampResult467 );
				float clampResult469 = clamp( ( ( ( saturate( ( blendOpSrc471 + blendOpDest471 ) )) * (( _TopCoverage )?( 1.0 ):( MaskWorldY458 )) ) * _SnowTopIntensity ) , 0.0 , 1.0 );
				float4 transform600 = mul(GetObjectToWorldMatrix(),float4( IN.ase_texcoord6.xyz , 0.0 ));
				float lerpResult602 = lerp( -4.0 , 2.0 , _AmountBottom);
				float temp_output_605_0 = ( ( 1.0 - ( ( transform600.y - 1.0 ) / 2.0 ) ) + lerpResult602 );
				float clampResult608 = clamp( _GradientBottom , 0.0 , 100000.0 );
				float blendOpSrc610 = ( temp_output_605_0 - temp_output_507_0 );
				float blendOpDest610 = ( temp_output_605_0 * clampResult608 );
				float clampResult614 = clamp( ( ( saturate( ( blendOpSrc610 + blendOpDest610 ) )) * _SnowBottomIntensity ) , 0.0 , 1.0 );
				float blendOpSrc616 = (( _EnableTopAccum )?( clampResult469 ):( 0.0 ));
				float blendOpDest616 = (( _EnableBottomAccum )?( clampResult614 ):( 0.0 ));
				float SnowAccumTop470 = ( saturate( max( blendOpSrc616, blendOpDest616 ) ));
				float blendOpSrc517 = ( ( clampResult38_g1531 * (( _TopCoverageMask )?( saturate( MaskWorldY458 ) ):( 1.0 )) ) * _IntensityMask );
				float blendOpDest517 = SnowAccumTop470;
				float Mask78 = ( saturate( max( blendOpSrc517, blendOpDest517 ) ));
				float4 lerpResult347 = lerp( temp_output_712_0 , DetailBaseColor301 , Mask78);
				float4 temp_cast_7 = (Mask78).xxxx;
				float4 Albedo15 = (( _VisualizeMask )?( temp_cast_7 ):( lerpResult347 ));
				float2 texCoord106_g1599 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult149_g1599 = clamp( _TilingInstance , 0.0 , 1000.0 );
				float2 temp_output_175_0_g1599 = DetailParallax323;
				float temp_output_123_0_g1599 = ( tex2D( _SparkleMask, ( ( ( texCoord106_g1599 * _TilingDotMask ) * clampResult149_g1599 ) + temp_output_175_0_g1599 ) ).a + (-1.2 + (_SpreadDotMask - 0.0) * (0.7 - -1.2) / (1.0 - 0.0)) );
				float saferPower230_g1599 = max( temp_output_123_0_g1599 , 0.0001 );
				float temp_output_121_0_g1599 = ( _ContrastDotMask + 1.0 );
				float clampResult118_g1599 = clamp( pow( saferPower230_g1599 , temp_output_121_0_g1599 ) , 0.0 , 1.0 );
				float temp_output_119_0_g1599 = ( clampResult118_g1599 * _IntensityDotMask );
				float VisualizeDotMap230 = temp_output_119_0_g1599;
				float4 temp_cast_8 = (VisualizeDotMap230).xxxx;
				float temp_output_135_0_g1599 = ( _ContrastSparkles - -1.0 );
				float4 unityObjectToClipPos78_g1599 = TransformWorldToHClip(TransformObjectToWorld(IN.ase_texcoord6.xyz));
				float clampResult49_g1599 = clamp( _Desaturatecrystals , 0.0 , 1.0 );
				float3 desaturateInitialColor53_g1599 = tex2D( _SparkleMask, ( ( ( unityObjectToClipPos78_g1599 * ( _TilingCrystals / 10.0 ) ) * clampResult149_g1599 ) + float4( temp_output_175_0_g1599, 0.0 , 0.0 ) ).xy ).rgb;
				float desaturateDot53_g1599 = dot( desaturateInitialColor53_g1599, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar53_g1599 = lerp( desaturateInitialColor53_g1599, desaturateDot53_g1599.xxx, clampResult49_g1599 );
				float lerpResult246_g1599 = lerp( 0.6 , 1.0 , _AmountSparkle);
				float4 temp_cast_13 = (1.0).xxxx;
				float4 clampResult74_g1599 = clamp( ( saturate( ( CalculateContrast(temp_output_135_0_g1599,float4( desaturateVar53_g1599 , 0.0 )) + (( -1.0 - temp_output_135_0_g1599 ) + (lerpResult246_g1599 - 0.0) * (0.0 - ( -1.0 - temp_output_135_0_g1599 )) / (1.0 - 0.0)) ) ) * _CrystalsIntensity ) , float4( 0,0,0,0 ) , temp_cast_13 );
				float4 VisualizeSparklesMap231 = clampResult74_g1599;
				#if defined(_VISUALIZESPARKLE_NONE)
				float4 staticSwitch242 = Albedo15;
				#elif defined(_VISUALIZESPARKLE_DOTMASK)
				float4 staticSwitch242 = temp_cast_8;
				#elif defined(_VISUALIZESPARKLE_SPARKLEMAP)
				float4 staticSwitch242 = VisualizeSparklesMap231;
				#else
				float4 staticSwitch242 = Albedo15;
				#endif
				
				
				float3 Albedo = staticSwitch242.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}