#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class RFX1_CustomMaterialInspectorParticle : MaterialEditor
{
    private int blendIndex;
    private int srcMode = 1;
    private int dstMode = 1;
    private int cullMode = 2;
    private string[] blendMode = { "Additive", "Alpha Blend", "Mul", "Mul2" };
    private string[] cullModeEnum = { "Cull Off", "Cull Front", "Cull Back" };
    string[] lightOptions = { "Vert OFF", "Vert ON", "Vert Normal ON" };
    string[] lightOptionsKeyWords = { "VertLight_OFF", "VertLight4_ON", "VertLight4Normal_ON" };
    private int lightIndex;
    //private Vector2 tiling = new Vector2(800, 800);
    private bool isSoftLight;
    private bool isFresnel;
    private float invFade;
    private float fresnelStr;
    private float cutout;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!isVisible)
            return;
        var targetMat = target as Material;
        var tag = targetMat.GetTag("RFX1", false);
        if (tag != "Particle")
            return;
        var shaderKeyWords = targetMat.shaderKeywords;

        foreach (var keyWord in shaderKeyWords)
        {
            var index = lightOptionsKeyWords.ToList().IndexOf(keyWord);
            if (index >= 0)
                lightIndex = index;
        }
        //var tiling4 = targetMat.GetVector("_Tiling");
        //tiling = new Vector2(tiling4.x, tiling4.y);
        //tileX = tiling4.x;
        //tileY = tiling4.y;
        bool frameBlend = shaderKeyWords.Contains("FrameBlend_ON");
        bool isClip = shaderKeyWords.Contains("Clip_ON");
        bool isClipAlpha = shaderKeyWords.Contains("Clip_ON_Alpha");
        if (isClipAlpha) isClip = true;

        cutout = targetMat.GetFloat("_Cutout");
        invFade = targetMat.GetFloat("_InvFade");
        fresnelStr = targetMat.GetFloat("_FresnelStr");
        srcMode = targetMat.GetInt("SrcMode");
        dstMode = targetMat.GetInt("DstMode");
        cullMode = targetMat.GetInt("CullMode");
        isSoftLight = shaderKeyWords.Contains("SoftParticles_ON");
        isFresnel = shaderKeyWords.Contains("FresnelFade_ON");

        if (srcMode == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.One)
        {
            blendIndex = 0;
        }
        if (srcMode == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
        {
            blendIndex = 1;
        }
        if (srcMode == (int)UnityEngine.Rendering.BlendMode.DstColor &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.Zero)
        {
            blendIndex = 2;
        }
        if (srcMode == (int)UnityEngine.Rendering.BlendMode.DstColor &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.SrcColor)
        {
            blendIndex = 3;
        }

        EditorGUI.BeginChangeCheck();

        blendIndex = EditorGUILayout.Popup("Blend Mode", blendIndex, blendMode);
        cullMode = EditorGUILayout.Popup("Cull Mode", cullMode, cullModeEnum);
        lightIndex = EditorGUILayout.Popup("Use Light", lightIndex, lightOptions);
        frameBlend = EditorGUILayout.Toggle("Frame Blending", frameBlend);
        isClip = EditorGUILayout.Toggle("Cutout", isClip);
        isSoftLight = EditorGUILayout.Toggle("Soft Particles", isSoftLight);
        isFresnel = EditorGUILayout.Toggle("Fresnel Fade", isFresnel);
        if (frameBlend)
        {
            //tiling = EditorGUILayout.Vector2Field("Tiling", tiling);
            //tileX = EditorGUILayout.FloatField("TileX", tileX);
            //tileY = EditorGUILayout.FloatField("TileY", tileY);
        }

        if (isClip)
        {
            cutout = EditorGUILayout.FloatField("Cutout", cutout);
            isClipAlpha = EditorGUILayout.Toggle("Cutout From Alpha", isClipAlpha);
        }
        if (isSoftLight) invFade = EditorGUILayout.FloatField("Soft Particles Factor", invFade);
        if (isFresnel) fresnelStr = EditorGUILayout.FloatField("Fresnel Strength", fresnelStr);

        //if (EditorGUI.EndChangeCheck()) {

        var newKeyWords = new List<string>();
        newKeyWords.Add(lightOptionsKeyWords[lightIndex]);
        newKeyWords.Add(!frameBlend ? "FrameBlend_OFF" : "FrameBlend_ON");
        newKeyWords.Add(!isClip ? "Clip_OFF" : !isClipAlpha ? "Clip_ON" : "Clip_ON_Alpha");
        //newKeyWords.Add(isClipAlpha ? "Clip_OFF" : "Clip_ON_Alpha");
        newKeyWords.Add(!isSoftLight ? "SoftParticles_OFF" : "SoftParticles_ON");
        newKeyWords.Add(!isFresnel ? "FresnelFade_OFF" : "FresnelFade_ON");

        if (blendIndex == 0)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.One);
            newKeyWords.Add("BlendAdd");
        }
        if (blendIndex == 1)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            newKeyWords.Add("BlendAlpha");
        }
        if (blendIndex == 2)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.DstColor);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.Zero);
            newKeyWords.Add("BlendMul");
        }
        if (blendIndex == 3)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.DstColor);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.SrcColor);
            newKeyWords.Add("BlendMul2");
        }

        targetMat.SetInt("CullMode", cullMode);

        targetMat.shaderKeywords = newKeyWords.ToArray();

        //if (frameBlend) targetMat.SetVector("_Tiling", new Vector4(tileX, tileY, tileX / 100, tileY / 100));
        if (isClip) targetMat.SetFloat("_Cutout", cutout);
        if (isSoftLight) targetMat.SetFloat("_InvFade", invFade);
        if (isFresnel) targetMat.SetFloat("_FresnelStr", fresnelStr);
        EditorUtility.SetDirty(targetMat);
        //}

    }
}
#endif
