Shader "Custom/yar"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _LeftColorHigh ("Left Color High", COLOR) = (0.8,0.8,0.8,1)
        _LeftColorLow ("Left Color Low", COLOR) = (0.3,0.3,0.3,1)
        _RightColorHigh ("Left Color High", COLOR) = (0.8,0.8,0.8,1)
        _RightColorLow ("Left Color Low", COLOR) = (0.3,0.3,0.3,1)
        _YColorHigh ("Y Color High", COLOR) = (1,1,1,1)
        _YColorLow ("Y Color Low", COLOR) = (1,1,1,1)
        _yPosLow ("Y Pos Low", Float) = 0
        _yPosHigh ("Y Pos High", Float) = 10
        _GradientStrength ("Graident Strength", Float) = 1
        _EmissiveStrengh ("Emissive Strengh ", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType"="Opaque"
        }

        CGPROGRAM
        #pragma surface surf Lambert
        #define WHITE3 fixed3(1,1,1)
        #define UP float3(0,1,0)
        #define RIGHT float3(0,0,-1)
        #define LEFT float3(-1,0,0)

        sampler2D _MainTex;
        fixed4 _LeftColorLow;
        fixed4 _LeftColorHigh;
        fixed4 _RightColorLow;
        fixed4 _RightColorHigh;
        fixed4 _YColorHigh;
        fixed4 _YColorLow;
        half _yPosLow;
        half _yPosHigh;
        half _GradientStrength;
        half _EmissiveStrengh;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 normal;
            float4 color : COLOR;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // calculate gradient colors at their height
            half3 leftGradient = lerp(_LeftColorLow, _LeftColorHigh, smoothstep(_yPosLow, _yPosHigh, IN.worldPos.y)).rgb;
            half3 rightGradient = lerp(_RightColorLow, _RightColorHigh, smoothstep(_yPosLow, _yPosHigh, IN.worldPos.y)).rgb;
            half3 yGradient = lerp(_YColorLow, _YColorHigh, smoothstep(_yPosLow, _yPosHigh, IN.worldPos.y)).rgb;
            leftGradient = lerp(WHITE3, leftGradient, _GradientStrength);
            rightGradient = lerp(WHITE3, rightGradient, _GradientStrength);
            yGradient = lerp(WHITE3, yGradient, _GradientStrength);

            // add the gradient colors
            half3 finalColor = leftGradient * max(0, dot(o.Normal, LEFT));

            finalColor += rightGradient * max(0, dot(o.Normal, RIGHT));

            finalColor += yGradient * max(0, dot(o.Normal, UP));
            
            // scale down to 0-1 values
            finalColor = saturate(finalColor);

            o.Albedo = finalColor;
            // how much should go to emissive
            o.Emission = lerp(half3(0, 0, 0), finalColor, _EmissiveStrengh);
            
            o.Alpha = 1;
        }
        ENDCG
    }
    fallback "Vertex Lit"
}