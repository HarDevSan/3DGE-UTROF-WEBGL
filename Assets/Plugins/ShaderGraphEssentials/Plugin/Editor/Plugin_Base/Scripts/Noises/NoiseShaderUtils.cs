//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using UnityEditor.ShaderGraph;

namespace ShaderGraphEssentials
{
    class ShaderUtils
    {
        public static void RandomValue2dTo1dFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction("SGE_RandomValue2dTo1d", s => s.Append(@"
inline float SGE_RandomValue2dTo1d(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453) * 2.0 - 1.0;
}"));
        }

        public static void RandomValue2dTo2dFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction("SGE_RandomValue2dTo2d", s => s.Append(@"
inline float2 SGE_RandomValue2dTo2d(float2 p)
{
    // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}"));
        }

        public static string ValueNoiseFunctionName = "SGE_ValueNoise";
        public static void ValueNoiseFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction(ValueNoiseFunctionName, s => s.Append(@"
inline float " + ValueNoiseFunctionName + @"(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    float2 u = f*f*(3.0-2.0*f);

    return lerp(lerp(SGE_RandomValue2dTo1d(i),
                     SGE_RandomValue2dTo1d(i + float2(1.0, 0.0)), u.x),
                lerp(SGE_RandomValue2dTo1d(i + float2(0.0, 1.0)),
                     SGE_RandomValue2dTo1d(i + float2(1.0, 1.0)), u.x), u.y);
}"));
        }

        // from https://www.shadertoy.com/view/3d2GRh
        // The MIT License
        // Copyright © 2019 Inigo Quilez
        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        public static string PeriodicValueNoiseFunctionName = "SGE_PeriodicValueNoise";
        public static void PeriodicValueNoiseFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction(PeriodicValueNoiseFunctionName, s => s.Append(@"
inline float " + PeriodicValueNoiseFunctionName + @"(float2 uv, int p)
{
    float2 rescaledUv = uv * p;
    int2 i = floor(rescaledUv);
    float2 f = frac(rescaledUv);
    float2 u = f*f*(3.0-2.0*f);

    return lerp(lerp(SGE_RandomValue2dTo1d(i &(p - 1)),
                     SGE_RandomValue2dTo1d((i + int2(1, 0)) &(p - 1)), u.x),
                lerp(SGE_RandomValue2dTo1d((i + int2(0, 1)) &(p - 1)),
                     SGE_RandomValue2dTo1d((i + int2(1, 1)) &(p - 1)), u.x), u.y);
}"));
        }

        public static string GradientNoiseFunctionName = "SGE_GradientNoise";
        public static void GradientNoiseFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction(GradientNoiseFunctionName, s => s.Append(@"
inline float " + GradientNoiseFunctionName + @"(float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    float2 u = f*f*(3.0-2.0*f);

    return lerp(lerp(dot(SGE_RandomValue2dTo2d(i), f),
                     dot(SGE_RandomValue2dTo2d(i + float2(1.0, 0.0)), f - float2(1.0,0.0)), u.x),
                lerp(dot(SGE_RandomValue2dTo2d(i + float2(0.0, 1.0)), f - float2(0.0,1.0)),
                     dot(SGE_RandomValue2dTo2d(i + float2(1.0, 1.0)), f - float2(1.0,1.0)), u.x), u.y);
}"));
        }

        public static string SimplexNoiseFunctionName = "SGE_SimplexNoise";
        // from https://www.shadertoy.com/view/Msf3WH
        // The MIT License
        // Copyright © 2013 Inigo Quilez
        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        public static void SimplexNoiseFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction(SimplexNoiseFunctionName, s => s.Append(@"
inline float " + SimplexNoiseFunctionName + @"(float2 uv)
{
    const float K1 = 0.366025404; // (sqrt(3) - 1)/ 2;
    const float K2 = 0.211324865; // (3 - sqrt(3)) / 6;

	float2 i = floor(uv + (uv.x + uv.y) * K1);
	
    float2 a = uv - i + (i.x + i.y) * K2;
    float2 o = step(a.yx, a.xy);    
    float2 b = a - o + K2;
	float2 c = a - 1.0 + 2.0 * K2;

    float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);

	float3 n = h * h * h * h * float3(dot(a, SGE_RandomValue2dTo2d(i + 0.0)),
        dot(b, SGE_RandomValue2dTo2d(i + o)),
        dot(c, SGE_RandomValue2dTo2d(i + 1.0)));

    return dot(n, float3(70.0, 70.0, 70.0));
}"));
        }

        //
        // Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
        //
        // Original work (webgl-noise) Copyright (C) 2011 Stefan Gustavson
        // Translation and modification was made by Keijiro Takahashi.
        //
        // This shader is based on the webgl-noise GLSL shader. For further details
        // of the original shader, please see the following description from the
        // original source code.
        //

        //
        // GLSL textureless classic 2D noise "cnoise",
        // with an RSL-style periodic variant "pnoise".
        // Author:  Stefan Gustavson (stefan.gustavson@liu.se)
        // Version: 2011-08-22
        //
        // Many thanks to Ian McEwan of Ashima Arts for the
        // ideas for permutation and gradient selection.
        //
        // Copyright (c) 2011 Stefan Gustavson. All rights reserved.
        // Distributed under the MIT license. See LICENSE file.
        // https://github.com/ashima/webgl-noise

        public static void PerlinNoiseHelperFunction(FunctionRegistry registry)
        {
            registry.ProvideFunction("SGE_Mod_F4", s => s.Append(@"
inline float4 SGE_Mod(float4 x, float4 y)
{
    return x - y * floor(x / y);
}"));

            registry.ProvideFunction("SGE_Mod_F3", s => s.Append(@"
inline float3 SGE_Mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}"));

            registry.ProvideFunction("SGE_Mod289_F4", s => s.Append(@"
inline float4 SGE_Mod289(float4 x)
{
    return x - floor(x / 289.0) * 289.0;
}"));

            registry.ProvideFunction("SGE_Mod289_F3", s => s.Append(@"
inline float3 SGE_Mod289(float3 x)
{
    return x - floor(x / 289.0) * 289.0;
}"));

            registry.ProvideFunction("SGE_Permute", s => s.Append(@"
inline float4 SGE_Permute(float4 x)
{
    return SGE_Mod289(((x*34.0)+1.0)*x);
}"));

            registry.ProvideFunction("SGE_TaylorInvSqrt", s => s.Append(@"
inline float4 SGE_TaylorInvSqrt(float4 r)
{
    return (float4)1.79284291400159 - r * 0.85373472095314;
}"));

            registry.ProvideFunction("SGE_Fade_F2", s => s.Append(@"
inline float2 SGE_Fade(float2 t)
{
    return t*t*t*(t*(t*6.0-15.0)+10.0);
}"));

            registry.ProvideFunction("SGE_Fade_F3", s => s.Append(@"
inline float3 SGE_Fade(float3 t)
{ 
    return t*t*t*(t*(t*6.0-15.0)+10.0);
}"));
        }

        public static string PerlinNoiseFunctionName = "SGE_PerlinNoise";
        public static void PerlinNoiseFunction(FunctionRegistry registry)
        {
            PerlinNoiseHelperFunction(registry);

            registry.ProvideFunction(PerlinNoiseFunctionName, s => s.Append(@"
inline float " + PerlinNoiseFunctionName + @"(float2 uv)
{
      float4 Pi = floor(uv.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
      float4 Pf = frac (uv.xyxy) - float4(0.0, 0.0, 1.0, 1.0);
      Pi = SGE_Mod289(Pi); // To avoid truncation effects in permutation
      float4 ix = Pi.xzxz;
      float4 iy = Pi.yyww;
      float4 fx = Pf.xzxz;
      float4 fy = Pf.yyww;

      float4 i = SGE_Permute(SGE_Permute(ix) + iy);

      float4 gx = frac(i / 41.0) * 2.0 - 1.0 ;
      float4 gy = abs(gx) - 0.5 ;
      float4 tx = floor(gx + 0.5);
      gx = gx - tx;

      float2 g00 = float2(gx.x,gy.x);
      float2 g10 = float2(gx.y,gy.y);
      float2 g01 = float2(gx.z,gy.z);
      float2 g11 = float2(gx.w,gy.w);

      float4 norm = SGE_TaylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
      g00 *= norm.x;
      g01 *= norm.y;
      g10 *= norm.z;
      g11 *= norm.w;

      float n00 = dot(g00, float2(fx.x, fy.x));
      float n10 = dot(g10, float2(fx.y, fy.y));
      float n01 = dot(g01, float2(fx.z, fy.z));
      float n11 = dot(g11, float2(fx.w, fy.w));

      float2 fade_xy = SGE_Fade(Pf.xy);
      float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
      float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
      return 2.3 * n_xy;
}"));
        }

        public static string PeriodicPerlinNoiseFunctionName = "SGE_PeriodicPerlinNoise";
        public static void PeriodicPerlinNoiseFunction(FunctionRegistry registry)
        {
            PerlinNoiseHelperFunction(registry);

            registry.ProvideFunction(PeriodicPerlinNoiseFunctionName, s => s.Append(@"
inline float " + PeriodicPerlinNoiseFunctionName + @"(float2 uv, float2 period)
{
      float2 rescaledUv = uv * period;
      float4 Pi = floor(rescaledUv.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
      float4 Pf = frac (rescaledUv.xyxy) - float4(0.0, 0.0, 1.0, 1.0);
      Pi = SGE_Mod(Pi, period.xyxy); // To create noise with explicit period
      Pi = SGE_Mod289(Pi);        // To avoid truncation effects in permutation
      float4 ix = Pi.xzxz;
      float4 iy = Pi.yyww;
      float4 fx = Pf.xzxz;
      float4 fy = Pf.yyww;

      float4 i = SGE_Permute(SGE_Permute(ix) + iy);

      float4 gx = frac(i / 41.0) * 2.0 - 1.0 ;
      float4 gy = abs(gx) - 0.5 ;
      float4 tx = floor(gx + 0.5);
      gx = gx - tx;

      float2 g00 = float2(gx.x,gy.x);
      float2 g10 = float2(gx.y,gy.y);
      float2 g01 = float2(gx.z,gy.z);
      float2 g11 = float2(gx.w,gy.w);

      float4 norm = SGE_TaylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
      g00 *= norm.x;
      g01 *= norm.y;
      g10 *= norm.z;
      g11 *= norm.w;

      float n00 = dot(g00, float2(fx.x, fy.x));
      float n10 = dot(g10, float2(fx.y, fy.y));
      float n01 = dot(g01, float2(fx.z, fy.z));
      float n11 = dot(g11, float2(fx.w, fy.w));

      float2 fade_xy = SGE_Fade(Pf.xy);
      float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
      float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
      return 2.3 * n_xy;
}"));
        }

        // Copyright (c) 2011 Stefan Gustavson. All rights reserved.
        // Distributed under the MIT license. See LICENSE file.
        // https://github.com/stegu/webgl-noise
        public static string Perlin3DNoiseFunctionName = "SGE_Perlin3DNoise";
        public static void Perlin3DNoiseFunction(FunctionRegistry registry)
        {
            PerlinNoiseHelperFunction(registry);

            registry.ProvideFunction(Perlin3DNoiseFunctionName, s => s.Append(@"
inline float " + Perlin3DNoiseFunctionName + @"(float3 uv)
{
      float3 Pi0 = floor(uv); // Integer part for indexing
      float3 Pi1 = Pi0 + float3(1.0, 1.0, 1.0); // Integer part + 1
      Pi0 = SGE_Mod289(Pi0);
      Pi1 = SGE_Mod289(Pi1);
      float3 Pf0 = frac(uv); // Fractional part for interpolation
      float3 Pf1 = Pf0 - float3(1.0, 1.0, 1.0); // Fractional part - 1.0
      float4 ix = float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
      float4 iy = float4(Pi0.yy, Pi1.yy);
      float4 iz0 = Pi0.zzzz;
      float4 iz1 = Pi1.zzzz;

      float4 ixy = SGE_Permute(SGE_Permute(ix) + iy);
      float4 ixy0 = SGE_Permute(ixy + iz0);
      float4 ixy1 = SGE_Permute(ixy + iz1);

      float4 gx0 = ixy0 * (1.0 / 7.0);
      float4 gy0 = frac(floor(gx0) * (1.0 / 7.0)) - 0.5;
      gx0 = frac(gx0);
      float4 gz0 = float4(0.5, 0.5, 0.5, 0.5) - abs(gx0) - abs(gy0);
      float4 sz0 = step(gz0, float4(0.0, 0.0, 0.0, 0.0));
      gx0 -= sz0 * (step(0.0, gx0) - 0.5);
      gy0 -= sz0 * (step(0.0, gy0) - 0.5);

      float4 gx1 = ixy1 * (1.0 / 7.0);
      float4 gy1 = frac(floor(gx1) * (1.0 / 7.0)) - 0.5;
      gx1 = frac(gx1);
      float4 gz1 = float4(0.5, 0.5, 0.5, 0.5) - abs(gx1) - abs(gy1);
      float4 sz1 = step(gz1, float4(0.0, 0.0, 0.0, 0.0));
      gx1 -= sz1 * (step(0.0, gx1) - 0.5);
      gy1 -= sz1 * (step(0.0, gy1) - 0.5);

      float3 g000 = float3(gx0.x,gy0.x,gz0.x);
      float3 g100 = float3(gx0.y,gy0.y,gz0.y);
      float3 g010 = float3(gx0.z,gy0.z,gz0.z);
      float3 g110 = float3(gx0.w,gy0.w,gz0.w);
      float3 g001 = float3(gx1.x,gy1.x,gz1.x);
      float3 g101 = float3(gx1.y,gy1.y,gz1.y);
      float3 g011 = float3(gx1.z,gy1.z,gz1.z);
      float3 g111 = float3(gx1.w,gy1.w,gz1.w);

      float4 norm0 = SGE_TaylorInvSqrt(float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
      g000 *= norm0.x;
      g010 *= norm0.y;
      g100 *= norm0.z;
      g110 *= norm0.w;
      float4 norm1 = SGE_TaylorInvSqrt(float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
      g001 *= norm1.x;
      g011 *= norm1.y;
      g101 *= norm1.z;
      g111 *= norm1.w;

      float n000 = dot(g000, Pf0);
      float n100 = dot(g100, float3(Pf1.x, Pf0.yz));
      float n010 = dot(g010, float3(Pf0.x, Pf1.y, Pf0.z));
      float n110 = dot(g110, float3(Pf1.xy, Pf0.z));
      float n001 = dot(g001, float3(Pf0.xy, Pf1.z));
      float n101 = dot(g101, float3(Pf1.x, Pf0.y, Pf1.z));
      float n011 = dot(g011, float3(Pf0.x, Pf1.yz));
      float n111 = dot(g111, Pf1);

      float3 fade_xyz = SGE_Fade(Pf0);
      float4 n_z = lerp(float4(n000, n100, n010, n110), float4(n001, n101, n011, n111), fade_xyz.z);
      float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
      float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x); 
      return 2.2 * n_xyz;
}"));
        }

        public static string PeriodicPerlin3DNoiseFunctionName = "SGE_PeriodicPerlin3DNoise";
        public static void PeriodicPerlin3DNoiseFunction(FunctionRegistry registry)
        {
            PerlinNoiseHelperFunction(registry);

            registry.ProvideFunction(PeriodicPerlin3DNoiseFunctionName, s => s.Append(@"
inline float " + PeriodicPerlin3DNoiseFunctionName + @"(float3 uv, float3 period)
{
      float3 Pi0 = SGE_Mod(floor(uv), period); // Integer part, modulo period
      float3 Pi1 = SGE_Mod(Pi0 + float3(1.0, 1.0, 1.0), period); // Integer part + 1, mod period
      Pi0 = SGE_Mod289(Pi0);
      Pi1 = SGE_Mod289(Pi1);
      float3 Pf0 = frac(uv); // fracional part for interpolation
      float3 Pf1 = Pf0 - float3(1.0, 1.0, 1.0); // fracional part - 1.0
      float4 ix = float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
      float4 iy = float4(Pi0.yy, Pi1.yy);
      float4 iz0 = Pi0.zzzz;
      float4 iz1 = Pi1.zzzz;

      float4 ixy = SGE_Permute(SGE_Permute(ix) + iy);
      float4 ixy0 = SGE_Permute(ixy + iz0);
      float4 ixy1 = SGE_Permute(ixy + iz1);

      float4 gx0 = ixy0 * (1.0 / 7.0);
      float4 gy0 = frac(floor(gx0) * (1.0 / 7.0)) - 0.5;
      gx0 = frac(gx0);
      float4 gz0 = float4(0.5, 0.5, 0.5, 0.5) - abs(gx0) - abs(gy0);
      float4 sz0 = step(gz0, float4(0.0, 0.0, 0.0, 0.0));
      gx0 -= sz0 * (step(0.0, gx0) - 0.5);
      gy0 -= sz0 * (step(0.0, gy0) - 0.5);

      float4 gx1 = ixy1 * (1.0 / 7.0);
      float4 gy1 = frac(floor(gx1) * (1.0 / 7.0)) - 0.5;
      gx1 = frac(gx1);
      float4 gz1 = float4(0.5, 0.5, 0.5, 0.5) - abs(gx1) - abs(gy1);
      float4 sz1 = step(gz1, float4(0.0, 0.0, 0.0, 0.0));
      gx1 -= sz1 * (step(0.0, gx1) - 0.5);
      gy1 -= sz1 * (step(0.0, gy1) - 0.5);

      float3 g000 = float3(gx0.x,gy0.x,gz0.x);
      float3 g100 = float3(gx0.y,gy0.y,gz0.y);
      float3 g010 = float3(gx0.z,gy0.z,gz0.z);
      float3 g110 = float3(gx0.w,gy0.w,gz0.w);
      float3 g001 = float3(gx1.x,gy1.x,gz1.x);
      float3 g101 = float3(gx1.y,gy1.y,gz1.y);
      float3 g011 = float3(gx1.z,gy1.z,gz1.z);
      float3 g111 = float3(gx1.w,gy1.w,gz1.w);

      float4 norm0 = SGE_TaylorInvSqrt(float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
      g000 *= norm0.x;
      g010 *= norm0.y;
      g100 *= norm0.z;
      g110 *= norm0.w;
      float4 norm1 = SGE_TaylorInvSqrt(float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
      g001 *= norm1.x;
      g011 *= norm1.y;
      g101 *= norm1.z;
      g111 *= norm1.w;

      float n000 = dot(g000, Pf0);
      float n100 = dot(g100, float3(Pf1.x, Pf0.yz));
      float n010 = dot(g010, float3(Pf0.x, Pf1.y, Pf0.z));
      float n110 = dot(g110, float3(Pf1.xy, Pf0.z));
      float n001 = dot(g001, float3(Pf0.xy, Pf1.z));
      float n101 = dot(g101, float3(Pf1.x, Pf0.y, Pf1.z));
      float n011 = dot(g011, float3(Pf0.x, Pf1.yz));
      float n111 = dot(g111, Pf1);

      float3 fade_xyz = SGE_Fade(Pf0);
      float4 n_z = lerp(float4(n000, n100, n010, n110), float4(n001, n101, n011, n111), fade_xyz.z);
      float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
      float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x); 
      return 2.2 * n_xyz;
}"));
        }

        public static string GetFractalFunctionName(string noiseFunctionName)
        {
            return "SGE_Fractal" + noiseFunctionName; 
        }

        public static void FractalFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetFractalFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetFractalFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, float persistence, float lacunarity)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    for (uint i = 0; i < 4; i++)
    {
        ret += currentPersistence * " + noiseFunctionName + @"(currentUV);
        currentPersistence *= persistence;
        currentUV *= lacunarity;
    }

    return ret;
}"));
        }

        public static void PeriodicFractalFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetFractalFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetFractalFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, int Period, float persistence, float lacunarity)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    int p = pow(2, Period);
    for (uint i = 0; i < 4; i++)
    {
        ret += currentPersistence * " + noiseFunctionName + @"(currentUV, p);
        currentPersistence *= persistence;
        currentUV *= lacunarity;
        p *= 2;
    }

    return ret;
}"));
        }

        public static string GetTurbulenceFunctionName(string noiseFunctionName)
        {
            return "SGE_Turbulence" + noiseFunctionName;
        }

        public static void TurbulenceFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetTurbulenceFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetTurbulenceFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, float persistence, float lacunarity)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    for (uint i = 0; i < 4; i++)
    {
        ret += currentPersistence * abs(" + noiseFunctionName + @"(currentUV));
        currentPersistence *= persistence;
        currentUV *= lacunarity;
    }

    return ret;
}"));
        }

        public static void PeriodicTurbulenceFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetTurbulenceFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetTurbulenceFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, int Period, float persistence, float lacunarity)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    int p = pow(2, Period);
    for (uint i = 0; i < 4; i++)
    {
        ret += currentPersistence * abs(" + noiseFunctionName + @"(currentUV, p));
        currentPersistence *= persistence;
        currentUV *= lacunarity;
        p *= 2;
    }

    return ret;
}"));
        }

        public static string GetRidgeFunctionName(string noiseFunctionName)
        {
            return "SGE_Ridge" + noiseFunctionName;
        }

        public static void RidgeFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetRidgeFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetRidgeFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, float persistence, float lacunarity, float sharpness)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    for (uint i = 0; i < 4; i++)
    {
        // create creases
        float n = saturate(abs(" + noiseFunctionName + @"(currentUV)));
        // invert so creases are at top
        n = 1.0 - n + 0.0001f;
        // sharpen creases
        n = pow(n, sharpness);
        ret += currentPersistence * n;
        currentPersistence *= persistence;
        currentUV *= lacunarity;
    }

    return ret;
}"));
        }

        public static void PeriodicRidgeFunction(FunctionRegistry registry, string noiseFunctionName, string dimensionTypeName = "float2")
        {
            registry.ProvideFunction(GetRidgeFunctionName(noiseFunctionName), s => s.Append(@"
inline float " + GetRidgeFunctionName(noiseFunctionName) + @"(" + dimensionTypeName + @" uv, int Period, float persistence, float lacunarity, float sharpness)
{
    float currentPersistence = persistence;
    " + dimensionTypeName + @" currentUV = uv; 
    float ret = 0.0;
    int p = pow(2, Period);
    for (uint i = 0; i < 4; i++)
    {
        // create creases
        float n = saturate(abs(" + noiseFunctionName + @"(currentUV, p)));
        // invert so creases are at top
        n = 1.0 - n + 0.0001f;
        // sharpen creases
        n = pow(n, sharpness);
        ret += currentPersistence * n;
        currentPersistence *= persistence;
        currentUV *= lacunarity;
        p *= 2;
    }

    return ret;
}"));
        }

    }
}
