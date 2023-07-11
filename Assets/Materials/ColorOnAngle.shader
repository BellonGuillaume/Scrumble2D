Shader "Custom/ColorOnAngle"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _Color("Color", Color) = (1, 1, 1, 1)
        _StartAngle("Start Angle", Range(0, 360)) = 0
        _OpenAngle("Open Angle", Range(0, 360)) = 90
    }
 
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Stencil{
            Ref 1
            Comp Less
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            float _StartAngle;
            float _OpenAngle;
            fixed4 _BaseColor;
            fixed4 _Color;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = 0.5;
                float2 dir = i.uv - center;
                float angle = atan2(dir.y, dir.x) * (180 / 3.14159) + 180;
                angle = angle % 360;
                angle = (angle + 90) % 360; // Décalage de l'angle de 90 degrés vers le haut
                angle = (360 - angle) % 360; // Inversion de l'angle dans le sens horaire
 
                if (_OpenAngle == 360)
                {
                    return _Color;
                }
                else if (angle >= _StartAngle && angle <= (_StartAngle + _OpenAngle) % 360)
                {
                    return _Color;
                }
                else
                {
                    return _BaseColor;
                }
            }
            ENDCG
        }
    }
}
