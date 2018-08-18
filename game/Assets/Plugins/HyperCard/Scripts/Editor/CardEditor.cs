/***************************************************************************\
Project:      HyperCard
Copyright (c) Enixion
Developer    Bourgot Jean-Louis
\***************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace HyperCard
{
    [CustomEditor(typeof(Card))]
    [CanEditMultipleObjects]
    public class CardEditor : Editor
    {
        public SerializedProperty _cardFaceMat;
        public SerializedProperty _cardBackMat;
        public SerializedProperty _spriteMat;

        public SerializedProperty _spriteObjects;
        public SerializedProperty _tmpTextObjects;
        public SerializedProperty _showTmpProps;

        public SerializedProperty _seed;

        public SerializedProperty _debugBlend;
        public SerializedProperty _cardAlpha;
        public SerializedProperty _cardMask;
        public SerializedProperty _cardFaceFrame;
        public SerializedProperty _cardFaceFrameColor;
        public SerializedProperty _cardFaceArtwork;
        public SerializedProperty _cardFaceArtworkOffset;
        public SerializedProperty _cardFaceArtworkScale;
        public SerializedProperty _cardFaceBackgroundArtwork;
        public SerializedProperty _cardFaceBackgroundArtworkOffset;
        public SerializedProperty _cardFaceBackgroundArtworkScale;

        public SerializedProperty _enableDistortion0;
        public SerializedProperty _distortion0Mask;
        public SerializedProperty _distortion0Freq_Red;
        public SerializedProperty _distortion0Amp_Red;
        public SerializedProperty _distortion0Speed_Red;
        public SerializedProperty _distortion0Direction_Red;
        public SerializedProperty _distortion0Freq_Green;
        public SerializedProperty _distortion0Amp_Green;
        public SerializedProperty _distortion0Speed_Green;
        public SerializedProperty _distortion0Direction_Green;
        public SerializedProperty _distortion0Freq_Blue;
        public SerializedProperty _distortion0Amp_Blue;
        public SerializedProperty _distortion0Speed_Blue;
        public SerializedProperty _distortion0Direction_Blue;
        public SerializedProperty _enableOverlay;
        public SerializedProperty _overlay1Mask;
        public SerializedProperty _overlay1Diffuse;
        public SerializedProperty _overlay1MovSpeed;
        public SerializedProperty _overlay1AlphaMult;
        public SerializedProperty _overlay1Color;
        public SerializedProperty _distortion1Freq;
        public SerializedProperty _distortion1Amp;
        public SerializedProperty _distortion1Speed;
        public SerializedProperty _distortion1Direction;

        public SerializedProperty _enablePeriodicalFx;
        public SerializedProperty _periodicalFxDiffuse;
        public SerializedProperty _periodicalFxMask;
        public SerializedProperty _periodicalFxColor;
        public SerializedProperty _periodicalFxAlpha;
        public SerializedProperty _periodicalFxDelayOffMin;
        public SerializedProperty _periodicalFxDelayOffMax;
        public SerializedProperty _periodicalFxDelayOnMin;
        public SerializedProperty _periodicalFxDelayOnMax;
        public SerializedProperty _periodicalFxFadeDelay;

        public SerializedProperty _blackAndWhite;
        public SerializedProperty _enableOutline;

        public SerializedProperty _outlineNoiseFreq;
        public SerializedProperty _outlineNoiseSpeed;
        public SerializedProperty _outlineNoiseMult;
        public SerializedProperty _outlineNoiseDistance;
        public SerializedProperty _outlineNoiseOffset;
        public SerializedProperty _outlineNoiseThreshold;
        public SerializedProperty _outlineNoiseVerticalAjust;
        public SerializedProperty _outlineColor;
        public SerializedProperty _outlineEndColor;
        public SerializedProperty _outlineEndColorDistance;
        public SerializedProperty _outlineAlphaMult;
        public SerializedProperty _outlineWidth;
        public SerializedProperty _outlineHeight;
        public SerializedProperty _outlineSmooth;
        public SerializedProperty _outlineSmoothSpeed;
        public SerializedProperty _outlineTrimOffset;
        public SerializedProperty _outlinePosOffset;

        public SerializedProperty _burningAmount;
        public SerializedProperty _burnNoiseFreq;
        public SerializedProperty _burnMapRotateSpeed;
        public SerializedProperty _burningOutline;
        public SerializedProperty _burnColor;
        public SerializedProperty _burnEndColor;
        public SerializedProperty _burnAlphaCut;
        public SerializedProperty _burnExposure;

        public SerializedProperty _enableMix;
        public SerializedProperty _mixTexture;
        public SerializedProperty _mixTextureMask;
        public SerializedProperty _mixTextureColor;
        public SerializedProperty _mixTextureOffset;
        public SerializedProperty _mixTextureScale;
        public SerializedProperty _mixAlpha;
        public SerializedProperty _mixNoiseFreq;
        public SerializedProperty _mixNoiseMoveDir;
        public SerializedProperty _mixNoiseMult;
        public SerializedProperty _mixNoiseOffset;
        public SerializedProperty _mixNoiseThreshold;
        public SerializedProperty _mixNoiseStartColor;
        public SerializedProperty _mixNoiseEndColor;
        public SerializedProperty _mixNoiseColorExposure;
        public SerializedProperty _mixNoiseMask;
        public SerializedProperty _mixNoiseThresholdInvert;
        public SerializedProperty _mixNoiseAlpha;
        public SerializedProperty _mixNoiseMaskOffset;
        public SerializedProperty _mixNoiseMaskScale;
        public SerializedProperty _mixNoiseDistFreq;
        public SerializedProperty _mixNoiseDistAmp;
        public SerializedProperty _mixNoiseDistSpeed;
        public SerializedProperty _mixNoiseDistDir;

        public SerializedProperty _cardBack;
        public SerializedProperty _cardBackAlpha;

        //Holo/cubemap removed by nick 6/30/18
        public SerializedProperty _enableGlitter;
        public SerializedProperty _glitterMask;
        public SerializedProperty _glitterMap;
        public SerializedProperty _glitterMapScale;
        public SerializedProperty _glitterColor;
        public SerializedProperty _glitterSpeed;
        public SerializedProperty _glitterPower;
        public SerializedProperty _glitterContrast;
        public SerializedProperty _glitterSpecMap;
        public SerializedProperty _glitterSpecPower;
        public SerializedProperty _glitterSpecContrast;

        public SerializedProperty _canvasMode;
        public SerializedProperty _canvasParent;
        public SerializedProperty _canvas;

        public SerializedProperty _stencil;

        public SerializedProperty _cardOpacity;

        private Card _target;

        void OnEnable()
        {
            _target = (Card)target;

            try
            {
                _cardFaceMat = serializedObject.FindProperty("BaseCardFaceMat");
                _cardBackMat = serializedObject.FindProperty("BaseCardBackMat");
                _spriteMat = serializedObject.FindProperty("BaseSpriteMat");

                _spriteObjects = serializedObject.FindProperty("SpriteObjects");
                _showTmpProps = serializedObject.FindProperty("_showTmpProps");
                _tmpTextObjects = serializedObject.FindProperty("TmpTextObjects");
                _debugBlend = serializedObject.FindProperty("DebugBlend");
                _stencil = serializedObject.FindProperty("Stencil");
                _seed = serializedObject.FindProperty("Seed");

                _cardAlpha = serializedObject.FindProperty("CardAlpha");
                _cardMask = serializedObject.FindProperty("CardMask");
                _cardFaceFrame = serializedObject.FindProperty("CardFaceFrame");
                _cardFaceFrameColor = serializedObject.FindProperty("CardFaceFrameColor");
                _cardFaceArtwork = serializedObject.FindProperty("CardFaceArtwork");
                _cardFaceArtworkOffset = serializedObject.FindProperty("CardFaceArtworkOffset");
                _cardFaceArtworkScale = serializedObject.FindProperty("CardFaceArtworkScale");
                _cardFaceBackgroundArtwork = serializedObject.FindProperty("CardFaceBackgroundArtwork");
                _cardFaceBackgroundArtworkOffset = serializedObject.FindProperty("CardFaceBackgroundArtworkOffset");
                _cardFaceBackgroundArtworkScale = serializedObject.FindProperty("CardFaceBackgroundArtworkScale");

                _enableDistortion0 = serializedObject.FindProperty("EnableDistortion0");
                _distortion0Mask = serializedObject.FindProperty("Distortion0Mask");
                _distortion0Freq_Red = serializedObject.FindProperty("Distortion0Freq_Red");
                _distortion0Amp_Red = serializedObject.FindProperty("Distortion0Amp_Red");
                _distortion0Speed_Red = serializedObject.FindProperty("Distortion0Speed_Red");
                _distortion0Direction_Red = serializedObject.FindProperty("Distortion0Direction_Red");
                _distortion0Freq_Green = serializedObject.FindProperty("Distortion0Freq_Green");
                _distortion0Amp_Green = serializedObject.FindProperty("Distortion0Amp_Green");
                _distortion0Speed_Green = serializedObject.FindProperty("Distortion0Speed_Green");
                _distortion0Direction_Green = serializedObject.FindProperty("Distortion0Direction_Green");
                _distortion0Freq_Blue = serializedObject.FindProperty("Distortion0Freq_Blue");
                _distortion0Amp_Blue = serializedObject.FindProperty("Distortion0Amp_Blue");
                _distortion0Speed_Blue = serializedObject.FindProperty("Distortion0Speed_Blue");
                _distortion0Direction_Blue = serializedObject.FindProperty("Distortion0Direction_Blue");
                _enableOverlay = serializedObject.FindProperty("EnableOverlay");
                _overlay1Mask = serializedObject.FindProperty("Overlay1Mask");
                _overlay1Diffuse = serializedObject.FindProperty("Overlay1Diffuse");
                _overlay1MovSpeed = serializedObject.FindProperty("Overlay1MovSpeed");
                _overlay1AlphaMult = serializedObject.FindProperty("Overlay1AlphaMult");
                _overlay1Color = serializedObject.FindProperty("Overlay1Color");
                _distortion1Freq = serializedObject.FindProperty("Distortion1Freq");
                _distortion1Amp = serializedObject.FindProperty("Distortion1Amp");
                _distortion1Speed = serializedObject.FindProperty("Distortion1Speed");
                _distortion1Direction = serializedObject.FindProperty("Distortion1Direction");

                _enablePeriodicalFx = serializedObject.FindProperty("EnablePeriodicalFx");
                _periodicalFxDiffuse = serializedObject.FindProperty("PeriodicalFxDiffuse");
                _periodicalFxMask = serializedObject.FindProperty("PeriodicalFxMask");
                _periodicalFxColor = serializedObject.FindProperty("PeriodicalFxColor");
                _periodicalFxAlpha = serializedObject.FindProperty("PeriodicalFxAlpha");
                _periodicalFxDelayOffMin = serializedObject.FindProperty("PeriodicalFxDelayOffMin");
                _periodicalFxDelayOffMax = serializedObject.FindProperty("PeriodicalFxDelayOffMax");
                _periodicalFxDelayOnMin = serializedObject.FindProperty("PeriodicalFxDelayOnMin");
                _periodicalFxDelayOnMax = serializedObject.FindProperty("PeriodicalFxDelayOnMax");
                _periodicalFxFadeDelay = serializedObject.FindProperty("PeriodicalFxFadeDelay");

                _blackAndWhite = serializedObject.FindProperty("BlackAndWhite");
                _enableOutline = serializedObject.FindProperty("EnableOutline");

                _outlineNoiseFreq = serializedObject.FindProperty("OutlineNoiseFreq");
                _outlineNoiseSpeed = serializedObject.FindProperty("OutlineNoiseSpeed");
                _outlineNoiseMult = serializedObject.FindProperty("OutlineNoiseMult");
                _outlineNoiseDistance = serializedObject.FindProperty("OutlineNoiseDistance");
                _outlineNoiseOffset = serializedObject.FindProperty("OutlineNoiseOffset");
                _outlineNoiseThreshold = serializedObject.FindProperty("OutlineNoiseThreshold");
                _outlineNoiseVerticalAjust = serializedObject.FindProperty("OutlineNoiseVerticalAjust");

                _outlineColor = serializedObject.FindProperty("OutlineColor");
                _outlineEndColor = serializedObject.FindProperty("OutlineEndColor");
                _outlineEndColorDistance = serializedObject.FindProperty("OutlineEndColorDistance");
                _outlineAlphaMult = serializedObject.FindProperty("OutlineAlphaMult");
                _outlineWidth = serializedObject.FindProperty("OutlineWidth");
                _outlineHeight = serializedObject.FindProperty("OutlineHeight");
                _outlineSmooth = serializedObject.FindProperty("OutlineSmooth");
                _outlineSmoothSpeed = serializedObject.FindProperty("OutlineSmoothSpeed");

                _outlineTrimOffset = serializedObject.FindProperty("OutlineTrimOffset");
                _outlinePosOffset = serializedObject.FindProperty("OutlinePosOffset");

                _burnNoiseFreq = serializedObject.FindProperty("BurnNoiseFreq");
                _burningAmount = serializedObject.FindProperty("BurningAmount");
                _burnMapRotateSpeed = serializedObject.FindProperty("BurnMapRotateSpeed");
                _burningOutline = serializedObject.FindProperty("BurningOutline");
                _burnColor = serializedObject.FindProperty("BurnColor");
                _burnEndColor = serializedObject.FindProperty("BurnEndColor");
                _burnAlphaCut = serializedObject.FindProperty("BurnAlphaCut");
                _burnExposure = serializedObject.FindProperty("BurnExposure");

                // Card : Blend FX
                _enableMix = serializedObject.FindProperty("EnableMixTexture");
                _mixTexture = serializedObject.FindProperty("MixTexture");
                _mixTextureMask = serializedObject.FindProperty("MixTextureMask");
                _mixTextureColor = serializedObject.FindProperty("MixTextureColor");
                _mixTextureOffset = serializedObject.FindProperty("MixTextureOffset");
                _mixTextureScale = serializedObject.FindProperty("MixTextureScale");
                _mixAlpha = serializedObject.FindProperty("MixAlpha");
                _mixNoiseFreq = serializedObject.FindProperty("MixNoiseFreq");
                _mixNoiseMoveDir = serializedObject.FindProperty("MixNoiseMoveDir");
                _mixNoiseMult = serializedObject.FindProperty("MixNoiseMult");
                _mixNoiseOffset = serializedObject.FindProperty("MixNoiseOffset");
                _mixNoiseThreshold = serializedObject.FindProperty("MixNoiseThreshold");
                _mixNoiseStartColor = serializedObject.FindProperty("MixNoiseStartColor");
                _mixNoiseEndColor = serializedObject.FindProperty("MixNoiseEndColor");
                _mixNoiseMask = serializedObject.FindProperty("MixNoiseTextureMask");
                _mixNoiseColorExposure = serializedObject.FindProperty("MixNoiseColorExposure");
                _mixNoiseThresholdInvert = serializedObject.FindProperty("MixNoiseThresholdInvert");
                _mixNoiseAlpha = serializedObject.FindProperty("MixNoiseAlpha");
                _mixNoiseMaskOffset = serializedObject.FindProperty("MixNoiseMaskOffset");
                _mixNoiseMaskScale = serializedObject.FindProperty("MixNoiseMaskScale");
                _mixNoiseDistFreq = serializedObject.FindProperty("MixNoiseDistFreq");
                _mixNoiseDistAmp = serializedObject.FindProperty("MixNoiseDistAmp");
                _mixNoiseDistSpeed = serializedObject.FindProperty("MixNoiseDistSpeed");
                _mixNoiseDistDir = serializedObject.FindProperty("MixNoiseDistDir");

                // Card : Glitter fx
                _enableGlitter = serializedObject.FindProperty("EnableGlitter");
                _glitterMask = serializedObject.FindProperty("GlitterMask");
                _glitterMap = serializedObject.FindProperty("GlitterMap");
                _glitterColor = serializedObject.FindProperty("GlitterColor");
                _glitterSpeed = serializedObject.FindProperty("GlitterSpeed");
                _glitterPower = serializedObject.FindProperty("GlitterPower");
                _glitterContrast = serializedObject.FindProperty("GlitterContrast");
                _glitterMapScale = serializedObject.FindProperty("GlitterMapScale");
                _glitterSpecMap = serializedObject.FindProperty("GlitterSpecMap");
                _glitterSpecContrast = serializedObject.FindProperty("GlitterSpecContrast");
                _glitterSpecPower = serializedObject.FindProperty("GlitterSpecPower");

                _cardOpacity = serializedObject.FindProperty("CardOpacity");

                _canvasMode = serializedObject.FindProperty("CanvasMode");
                _canvasParent = serializedObject.FindProperty("ParentCanvasTransform");
                _canvas = serializedObject.FindProperty("CanvasTransform");


                _cardBack = serializedObject.FindProperty("CardBack");
                _cardBackAlpha = serializedObject.FindProperty("CardBackAlpha");
            }
            catch (Exception e) { Debug.Log(e); }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical();

            GUILayout.Space(10f);

            var u = new GUIStyle("box")
            {
                richText = true
            };

            if (DrawHeader("Materials"))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(20f);

                GUILayout.BeginVertical();

                GUILayout.Space(10f);

                EditorGUI.BeginDisabledGroup(true);

                _cardFaceMat.objectReferenceValue = EditorGUILayout.ObjectField("Face material", _cardFaceMat.objectReferenceValue, typeof(Material), false);
                _cardBackMat.objectReferenceValue = EditorGUILayout.ObjectField("Back material", _cardBackMat.objectReferenceValue, typeof(Material), false);
                _spriteMat.objectReferenceValue = EditorGUILayout.ObjectField("Sprite material", _spriteMat.objectReferenceValue, typeof(Material), false);

                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10f);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            if (DrawHeader("General settings"))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(20f);

                GUILayout.BeginVertical();

                GUILayout.Space(10f);

                GUILayout.Space(10f);

                _stencil.intValue = EditorGUILayout.IntField("Stencil", Mathf.Clamp(_stencil.intValue, 0, 255));

                EditorGUILayout.HelpBox("To avoid depth issues and text overlapping, each card should have a different Stencil value.", MessageType.Info);

                GUILayout.Space(10f);

                _seed.intValue = EditorGUILayout.IntField(new GUIContent("Random Seed", "Used to randomize generated noise."), _seed.intValue);

                if (_seed.intValue > 99999) _seed.intValue = 99999;

                GUILayout.Space(10f);

                _showTmpProps.boolValue = DrawToggle("Show child objects", _showTmpProps.boolValue);

                _canvasMode.boolValue = DrawToggle("Enable Canvas Mode", _canvasMode.boolValue, "Useful to avoid offset issues when using HyperCard with Canvas.");

                if (_canvasMode.boolValue)
                {
                    _canvasParent.objectReferenceValue = EditorGUILayout.ObjectField("Parent Canvas", _canvasParent.objectReferenceValue, typeof(RectTransform), true, GUILayout.ExpandWidth(true));
                    _canvas.objectReferenceValue = EditorGUILayout.ObjectField("Canvas", _canvas.objectReferenceValue, typeof(RectTransform), true, GUILayout.ExpandWidth(true));
                }

                GUILayout.Space(10f);

                GUI.backgroundColor = Color.white;

                GUILayout.Label("<color=black><b><size=12>Misc</size></b></color>", u, GUILayout.ExpandWidth(true));

                _cardOpacity.floatValue = EditorGUILayout.Slider(new GUIContent("Opacity", "Opacity is not compatible with card overlapping for the moment."), _cardOpacity.floatValue, 0.0f, 1f);

                GUILayout.Space(10f);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            if (DrawHeader("Face Properties"))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(20f);

                GUILayout.BeginVertical();

                GUILayout.Space(10f);

                GUI.backgroundColor = Color.white;

                GUILayout.Label("<color=black><b><size=12>General settings</size></b></color>", u, GUILayout.ExpandWidth(true));

                GUILayout.Space(10f);


                if (DrawHeader("Frame & artwork"))
                {
                    GUILayout.Space(10f);

                    _cardAlpha.objectReferenceValue = EditorGUILayout.ObjectField("Alpha Mask", _cardAlpha.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    GUILayout.Space(25f);
                    _cardMask.objectReferenceValue = EditorGUILayout.ObjectField("Artwork Mask", _cardMask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));

                    //var blendValues = Enum.GetNames(typeof(BlendDebug)).Select(x => x.ToString()).ToArray();

                    //var i = EditorGUILayout.Popup("[Debug]", _debugBlend.intValue, blendValues, GUILayout.ExpandWidth(false));

                    //_debugBlend.enumValueIndex = i;

                    GUILayout.Space(25f);

                    _cardFaceFrame.objectReferenceValue = EditorGUILayout.ObjectField("Frame diffuse", _cardFaceFrame.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _cardFaceFrameColor.colorValue = EditorGUILayout.ColorField("Frame color", _cardFaceFrameColor.colorValue);

                    GUILayout.Space(25f);
                    _cardFaceArtwork.objectReferenceValue = EditorGUILayout.ObjectField("Artwork", _cardFaceArtwork.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _cardFaceArtworkOffset.vector2Value = EditorGUILayout.Vector2Field("Offset", _cardFaceArtworkOffset.vector2Value);
                    _cardFaceArtworkScale.vector2Value = EditorGUILayout.Vector2Field("Scale", _cardFaceArtworkScale.vector2Value);


                    GUILayout.Space(25f);
                    _cardFaceBackgroundArtwork.objectReferenceValue = EditorGUILayout.ObjectField("Background Artwork", _cardFaceBackgroundArtwork.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _cardFaceBackgroundArtworkOffset.vector2Value = EditorGUILayout.Vector2Field("Background Offset", _cardFaceBackgroundArtworkOffset.vector2Value);
                    _cardFaceBackgroundArtworkScale.vector2Value = EditorGUILayout.Vector2Field("Background Scale", _cardFaceBackgroundArtworkScale.vector2Value);

                    GUILayout.Space(10f);
                }

                if (DrawHeader("Custom texts"))
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(20f);

                    GUILayout.BeginVertical();

                    GUILayout.Space(10f);

                    EditorStyles.textField.wordWrap = true;

                    GUILayout.Label(string.Format("<color=black><b><size=12>TMP_Text objects ({0})</size></b></color>", _tmpTextObjects.arraySize), u, GUILayout.ExpandWidth(true));

                    GUILayout.Space(10f);

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Link new TMP item", GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        _tmpTextObjects.InsertArrayElementAtIndex(_tmpTextObjects.arraySize);
                        _tmpTextObjects.serializedObject.ApplyModifiedProperties();
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();


                    for (var i = 0; i < _tmpTextObjects.arraySize; i++)
                    {
                        var tmpItem = _tmpTextObjects.GetArrayElementAtIndex(i);
                        var tmpObject = tmpItem.FindPropertyRelative("TmpObject");
                        var tmpKey = tmpItem.FindPropertyRelative("Key");
                        var tmpValue = tmpItem.FindPropertyRelative("Value");
                        var displayMode = tmpItem.FindPropertyRelative("DisplayMode");

                        GUI.backgroundColor = Color.white;

                        GUILayout.BeginVertical(u);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("#" + i, GUILayout.Width(20), GUILayout.Height(20));

                        var isTextArea = DrawMiniToggle("Tex", (TextMeshProParamDisplayMode)displayMode.intValue == TextMeshProParamDisplayMode.TextArea);

                        displayMode.intValue = isTextArea ? (int)TextMeshProParamDisplayMode.TextArea : (int)TextMeshProParamDisplayMode.Field;

                        tmpObject.objectReferenceValue = EditorGUILayout.ObjectField(tmpObject.objectReferenceValue, typeof(TMP_Text), true, GUILayout.ExpandWidth(false));

                        GUI.backgroundColor = new Color(0.8f, 0f, 0f);
                        if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            _tmpTextObjects.DeleteArrayElementAtIndex(i);
                            _tmpTextObjects.serializedObject.ApplyModifiedProperties();
                        }

                        GUI.backgroundColor = Color.white;

                        GUILayout.EndHorizontal();

                        if (i <= _tmpTextObjects.arraySize - 1)
                        {
                            tmpKey.stringValue = EditorGUILayout.TextField("Key", tmpKey.stringValue);

                            tmpValue.stringValue = isTextArea
                                ? EditorGUILayout.TextArea(tmpValue.stringValue, GUILayout.Height(100))
                                : EditorGUILayout.TextField(tmpValue.stringValue);
                        }

                        GUILayout.EndVertical();
                        GUILayout.Space(10f);
                    }

                    GUILayout.EndVertical();



                    GUILayout.Space(10f);

                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                if (DrawHeader("Custom sprites"))
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(20f);

                    GUILayout.BeginVertical();

                    GUILayout.Space(10f);

                    EditorStyles.textField.wordWrap = true;

                    GUILayout.Label(string.Format("<color=black><b><size=12>Custom sprites ({0})</size></b></color>", _spriteObjects.arraySize), u, GUILayout.ExpandWidth(true));

                    GUILayout.Space(10f);

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add a new sprite", GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        _spriteObjects.InsertArrayElementAtIndex(_spriteObjects.arraySize);
                        _spriteObjects.serializedObject.ApplyModifiedProperties();

                        _target.CreateSprite(_spriteObjects.arraySize - 1);
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();


                    for (var i = 0; i < _spriteObjects.arraySize; i++)
                    {
                        var spriteItem = _spriteObjects.GetArrayElementAtIndex(i);
                        var spriteRenderer = spriteItem.FindPropertyRelative("Sprite");
                        var spriteKey = spriteItem.FindPropertyRelative("Key");
                        var sprite = spriteItem.FindPropertyRelative("Value");
                        var spriteColor = spriteItem.FindPropertyRelative("Color");
                        var spritePosition = spriteItem.FindPropertyRelative("Position");
                        var spriteRenderQueue = spriteItem.FindPropertyRelative("RenderQueue");
                        var spriteScale = spriteItem.FindPropertyRelative("Scale");
                        var showAdvancedSettings = spriteItem.FindPropertyRelative("ShowAdvancedSettings");
                        var spriteIsAffectedByFilters = spriteItem.FindPropertyRelative("IsAffectedByFilters");

                        var hideSprite = spriteItem.FindPropertyRelative("IsHidden");

                        GUI.backgroundColor = Color.white;

                        GUILayout.BeginVertical(u);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("#" + i, GUILayout.Width(20), GUILayout.Height(20));

                        hideSprite.boolValue = DrawMiniToggle("Hide", hideSprite.boolValue);

                        EditorGUI.BeginDisabledGroup(false);
                        spriteRenderer.objectReferenceValue = EditorGUILayout.ObjectField(spriteRenderer.objectReferenceValue, typeof(SpriteRenderer), true, GUILayout.ExpandWidth(true));
                        EditorGUI.EndDisabledGroup();

                        GUI.backgroundColor = new Color(0.8f, 0f, 0f);
                        if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            _target.RemoveSprite(i);
                            _spriteObjects.DeleteArrayElementAtIndex(i);
                            _spriteObjects.serializedObject.ApplyModifiedProperties();
                        }

                        GUI.backgroundColor = Color.white;

                        GUILayout.EndHorizontal();

                        if (i <= _spriteObjects.arraySize - 1)
                        {
                            spriteKey.stringValue = EditorGUILayout.TextField("Key", spriteKey.stringValue);
                            sprite.objectReferenceValue = EditorGUILayout.ObjectField("Sprite", sprite.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                            GUILayout.Space(20f);

                            spritePosition.vector3Value = EditorGUILayout.Vector3Field("Position", spritePosition.vector3Value);
                            spriteScale.vector2Value = EditorGUILayout.Vector2Field("Scale", spriteScale.vector2Value);

                            showAdvancedSettings.boolValue = DrawToggle("Show advanced settings", showAdvancedSettings.boolValue);

                            if (showAdvancedSettings.boolValue)
                            {
                                spriteRenderQueue.intValue = EditorGUILayout.IntField("RenderQueue", spriteRenderQueue.intValue);

                                GUILayout.Space(20f);

                                spriteColor.colorValue = EditorGUILayout.ColorField("Color", spriteColor.colorValue);

                                spriteIsAffectedByFilters.boolValue = DrawToggle("Affected by filters ?", spriteIsAffectedByFilters.boolValue);
                            }

                            //spriteRotation.floatValue = EditorGUILayout.FloatField("Rotation", spriteRotation.floatValue);
                        }

                        GUILayout.EndVertical();
                        GUILayout.Space(10f);
                    }

                    GUILayout.EndVertical();

                    GUILayout.Space(10f);

                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(10f);

                GUI.backgroundColor = Color.white;

                GUILayout.Label("<color=black><b><size=12>Visual effects</size></b></color>", u, GUILayout.ExpandWidth(true));

                GUILayout.Space(10f);

                if (DrawHeader("Artwork - Distortion FX"))
                {
                    _enableDistortion0.boolValue = DrawToggle(_enableDistortion0.boolValue);
                    _distortion0Mask.objectReferenceValue = EditorGUILayout.ObjectField("Distortion Mask", _distortion0Mask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));

                    GUILayout.Space(10f);

                    if (DrawHeader("Red channel", "Dist1RC", false, 1, Color.red))
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Space(20f);

                        GUILayout.BeginVertical();

                        GUILayout.Space(10f);

                        _distortion0Freq_Red.floatValue = EditorGUILayout.FloatField("Distortion frequency", _distortion0Freq_Red.floatValue);
                        _distortion0Amp_Red.floatValue = EditorGUILayout.FloatField("Amplitude mult.", _distortion0Amp_Red.floatValue);
                        _distortion0Speed_Red.floatValue = EditorGUILayout.FloatField("Distortion speed", _distortion0Speed_Red.floatValue);
                        _distortion0Direction_Red.vector2Value = EditorGUILayout.Vector2Field("Direction", _distortion0Direction_Red.vector2Value);

                        GUILayout.EndVertical();

                        GUILayout.Space(10f);

                        GUILayout.EndHorizontal();

                        GUILayout.Space(10f);
                    }

                    GUILayout.Space(5f);

                    if (DrawHeader("Green channel", "Dist1GC", false, 1, Color.green))
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Space(20f);

                        GUILayout.BeginVertical();

                        GUILayout.Space(10f);

                        _distortion0Freq_Green.floatValue = EditorGUILayout.FloatField("Distortion frequency", _distortion0Freq_Green.floatValue);
                        _distortion0Amp_Green.floatValue = EditorGUILayout.FloatField("Amplitude mult.", _distortion0Amp_Green.floatValue);
                        _distortion0Speed_Green.floatValue = EditorGUILayout.FloatField("Distortion speed", _distortion0Speed_Green.floatValue);
                        _distortion0Direction_Green.vector2Value = EditorGUILayout.Vector2Field("Direction", _distortion0Direction_Green.vector2Value);

                        GUILayout.EndVertical();

                        GUILayout.Space(10f);

                        GUILayout.EndHorizontal();

                        GUILayout.Space(10f);
                    }

                    GUILayout.Space(5f);

                    if (DrawHeader("Blue channel", "Dist1BC", false, 1, Color.blue))
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Space(20f);

                        GUILayout.BeginVertical();

                        GUILayout.Space(10f);

                        _distortion0Freq_Blue.floatValue = EditorGUILayout.FloatField("Distortion frequency", _distortion0Freq_Blue.floatValue);
                        _distortion0Amp_Blue.floatValue = EditorGUILayout.FloatField("Amplitude mult.", _distortion0Amp_Blue.floatValue);
                        _distortion0Speed_Blue.floatValue = EditorGUILayout.FloatField("Distortion speed", _distortion0Speed_Blue.floatValue);
                        _distortion0Direction_Blue.vector2Value = EditorGUILayout.Vector2Field("Direction", _distortion0Direction_Blue.vector2Value);

                        GUILayout.EndVertical();

                        GUILayout.Space(10f);

                        GUILayout.EndHorizontal();

                        GUILayout.Space(10f);
                    }

                    GUILayout.Space(10f);
                }

                if (DrawHeader("Artwork - Animated Overlay FX"))
                {
                    _enableOverlay.boolValue = DrawToggle(_enableOverlay.boolValue);
                    _overlay1Mask.objectReferenceValue = EditorGUILayout.ObjectField("Mask", _overlay1Mask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _overlay1Diffuse.objectReferenceValue = EditorGUILayout.ObjectField("Diffuse", _overlay1Diffuse.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _overlay1Color.colorValue = EditorGUILayout.ColorField("Color", _overlay1Color.colorValue);
                    _overlay1AlphaMult.floatValue = EditorGUILayout.FloatField("Alpha strength", _overlay1AlphaMult.floatValue);
                    _overlay1MovSpeed.vector2Value = EditorGUILayout.Vector2Field("Move speed", _overlay1MovSpeed.vector2Value);

                    GUILayout.Space(20f);

                    _distortion1Freq.floatValue = EditorGUILayout.FloatField("Distortion frequency", _distortion1Freq.floatValue);
                    _distortion1Amp.floatValue = EditorGUILayout.FloatField("Amplitude mult.", _distortion1Amp.floatValue);
                    _distortion1Speed.floatValue = EditorGUILayout.FloatField("Distortion speed", _distortion1Speed.floatValue);
                    _distortion1Direction.vector2Value = EditorGUILayout.Vector2Field("Direction", _distortion1Direction.vector2Value);

                    GUILayout.Space(10f);
                }

                if (DrawHeader("Artwork - Periodical FX"))
                {
                    _enablePeriodicalFx.boolValue = DrawToggle(_enablePeriodicalFx.boolValue);
                    _periodicalFxDiffuse.objectReferenceValue = EditorGUILayout.ObjectField("Diffuse", _periodicalFxDiffuse.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _periodicalFxMask.objectReferenceValue = EditorGUILayout.ObjectField("Mask", _periodicalFxMask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _periodicalFxColor.colorValue = EditorGUILayout.ColorField("Color", _periodicalFxColor.colorValue);
                    _periodicalFxDelayOffMin.floatValue = EditorGUILayout.FloatField("Delay Off (Min)", _periodicalFxDelayOffMin.floatValue);
                    _periodicalFxDelayOffMax.floatValue = EditorGUILayout.FloatField("Delay Off (Max)", _periodicalFxDelayOffMax.floatValue);
                    _periodicalFxDelayOnMin.floatValue = EditorGUILayout.FloatField("Delay On (Min)", _periodicalFxDelayOnMin.floatValue);
                    _periodicalFxDelayOnMax.floatValue = EditorGUILayout.FloatField("Delay On (Max)", _periodicalFxDelayOnMax.floatValue);

                    _periodicalFxFadeDelay.floatValue = EditorGUILayout.FloatField("Fade Delay", _periodicalFxFadeDelay.floatValue);
                    GUILayout.Space(10f);
                    GUILayout.Label(_periodicalFxAlpha.floatValue.ToString());
                    GUILayout.Space(10f);
                }

                if (DrawHeader("Card - Outline"))
                {
                    _enableOutline.boolValue = DrawToggle(_enableOutline.boolValue);

                    _outlineWidth.floatValue = EditorGUILayout.Slider("Width", _outlineWidth.floatValue, 0, 10f);
                    _outlineHeight.floatValue = EditorGUILayout.Slider("Height", _outlineHeight.floatValue, 0, 10f);
                    _outlineSmooth.floatValue = EditorGUILayout.Slider("Smoothness", _outlineSmooth.floatValue, 0.05f, 0.5f);
                    _outlineSmoothSpeed.floatValue = EditorGUILayout.FloatField("Smooth speed", _outlineSmoothSpeed.floatValue);
                    _outlineTrimOffset.floatValue = EditorGUILayout.Slider("Trim offset", _outlineTrimOffset.floatValue, 0, 1f);
                    _outlinePosOffset.vector2Value = EditorGUILayout.Vector2Field("Pos. offset", _outlinePosOffset.vector2Value);

                    GUILayout.Space(10f);

                    _outlineColor.colorValue = EditorGUILayout.ColorField("Start Color", _outlineColor.colorValue);
                    _outlineEndColor.colorValue = EditorGUILayout.ColorField("End color", _outlineEndColor.colorValue);
                    _outlineEndColorDistance.floatValue = EditorGUILayout.Slider("End color distance", _outlineEndColorDistance.floatValue, 0, 1);
                    _outlineAlphaMult.floatValue = EditorGUILayout.FloatField("Exposure", _outlineAlphaMult.floatValue);

                    GUILayout.Space(10f);

                    _outlineNoiseFreq.floatValue = EditorGUILayout.FloatField("Noise freq.", _outlineNoiseFreq.floatValue);
                    _outlineNoiseSpeed.floatValue = EditorGUILayout.FloatField("Noise speed", _outlineNoiseSpeed.floatValue);
                    _outlineNoiseMult.floatValue = EditorGUILayout.FloatField("Noise mult.", _outlineNoiseMult.floatValue);
                    _outlineNoiseOffset.floatValue = EditorGUILayout.Slider("Noise offset", _outlineNoiseOffset.floatValue, 0, 1f);
                    _outlineNoiseThreshold.floatValue = EditorGUILayout.Slider("Noise alpha threshold", _outlineNoiseThreshold.floatValue, 0, 1);
                    _outlineNoiseDistance.floatValue = EditorGUILayout.Slider("Noise distance", _outlineNoiseDistance.floatValue, 0, 1);
                    _outlineNoiseVerticalAjust.floatValue = EditorGUILayout.Slider("V-ajust", _outlineNoiseVerticalAjust.floatValue, 0, 5);

                }

                if (DrawHeader("Card - Dissolve FX"))
                {
                    GUILayout.Space(10f);
                    _burnNoiseFreq.floatValue = EditorGUILayout.FloatField("Noise freq.", _burnNoiseFreq.floatValue);
                    _burnColor.colorValue = EditorGUILayout.ColorField("Inner color", _burnColor.colorValue);
                    _burnEndColor.colorValue = EditorGUILayout.ColorField("Outer color", _burnEndColor.colorValue);
                    _burnExposure.floatValue = EditorGUILayout.FloatField("Exposure", _burnExposure.floatValue);
                    _burningOutline.floatValue = EditorGUILayout.Slider("Burn outline", _burningOutline.floatValue, 0.0f, 3f);
                    _burningAmount.floatValue = EditorGUILayout.Slider("Burn amount", _burningAmount.floatValue, 0f, 1f);
                    _burnAlphaCut.floatValue = EditorGUILayout.Slider("Alpha cut", _burnAlphaCut.floatValue, 0.1f, 1f);
                    _burnMapRotateSpeed.floatValue = EditorGUILayout.FloatField("Rotation speed", _burnMapRotateSpeed.floatValue);

                    GUILayout.Space(10f);
                }

                if (DrawHeader("Card - Glitter FX"))
                {
                    _enableGlitter.boolValue = DrawToggle(_enableGlitter.boolValue);

                    _glitterMask.objectReferenceValue = EditorGUILayout.ObjectField("Mask", _glitterMask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _glitterMap.objectReferenceValue = EditorGUILayout.ObjectField("Glitter map", _glitterMap.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _glitterMapScale.vector2Value = EditorGUILayout.Vector2Field("Map scale", _glitterMapScale.vector2Value);
                    _glitterColor.colorValue = EditorGUILayout.ColorField("Color", _glitterColor.colorValue);
                    _glitterPower.floatValue = EditorGUILayout.Slider("Power", _glitterPower.floatValue, 0, 5);
                    _glitterSpeed.floatValue = EditorGUILayout.Slider("Speed", _glitterSpeed.floatValue, 0, 1f);
                    _glitterContrast.floatValue = EditorGUILayout.Slider("Contrast", _glitterContrast.floatValue, 0.1f, 2);
                    _glitterSpecMap.objectReferenceValue = EditorGUILayout.ObjectField("Specular map", _glitterSpecMap.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                    _glitterSpecPower.floatValue = EditorGUILayout.Slider("Specular power", _glitterSpecPower.floatValue, 0, 5);
                    _glitterSpecContrast.floatValue = EditorGUILayout.Slider("Specular contrast", _glitterSpecContrast.floatValue, 0.1f, 5);
                    GUILayout.Space(10f);
                }

                if (DrawHeader("Card - Noise FX"))
                {
                    _enableMix.boolValue = DrawToggle(_enableMix.boolValue);

                    GUILayout.Space(10f);

                    _mixNoiseMask.objectReferenceValue = EditorGUILayout.ObjectField("Mask", _mixNoiseMask.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(true));
                    _mixNoiseMaskOffset.vector2Value = EditorGUILayout.Vector2Field("Offset", _mixNoiseMaskOffset.vector2Value);
                    _mixNoiseMaskScale.vector2Value = EditorGUILayout.Vector2Field("Scale", _mixNoiseMaskScale.vector2Value);

                    GUILayout.Space(5f);

                    _mixNoiseDistFreq.floatValue = EditorGUILayout.FloatField("Distortion frequency", _mixNoiseDistFreq.floatValue);
                    _mixNoiseDistAmp.floatValue = EditorGUILayout.FloatField("Amplitude mult.", _mixNoiseDistAmp.floatValue);
                    _mixNoiseDistSpeed.vector2Value = EditorGUILayout.Vector2Field("Distortion speed", _mixNoiseDistSpeed.vector2Value);
                    _mixNoiseDistDir.vector2Value = EditorGUILayout.Vector2Field("Direction", _mixNoiseDistDir.vector2Value);

                    GUILayout.Space(10f);

                    _mixNoiseStartColor.colorValue = EditorGUILayout.ColorField("Start Color", _mixNoiseStartColor.colorValue);
                    _mixNoiseEndColor.colorValue = EditorGUILayout.ColorField("End color", _mixNoiseEndColor.colorValue);
                    _mixNoiseColorExposure.floatValue = EditorGUILayout.FloatField("Exposure", _mixNoiseColorExposure.floatValue);
                    _mixNoiseFreq.floatValue = EditorGUILayout.FloatField("Noise freq.", _mixNoiseFreq.floatValue);
                    _mixNoiseMoveDir.vector2Value = EditorGUILayout.Vector2Field("Noise move dir.", _mixNoiseMoveDir.vector2Value);
                    _mixNoiseMult.floatValue = EditorGUILayout.FloatField("Noise mult.", _mixNoiseMult.floatValue);
                    _mixNoiseOffset.floatValue = EditorGUILayout.Slider("Noise offset", _mixNoiseOffset.floatValue, 0, 1f);
                    _mixNoiseThreshold.floatValue = EditorGUILayout.Slider("Noise threshold", _mixNoiseThreshold.floatValue, 0, 1);
                    _mixNoiseThresholdInvert.boolValue = EditorGUILayout.Toggle("Invert", _mixNoiseThresholdInvert.boolValue);
                    _mixNoiseAlpha.floatValue = EditorGUILayout.FloatField("Alpha mult.", _mixNoiseAlpha.floatValue);

                    GUILayout.Space(10f);
                }

                GUILayout.Space(10f);

                GUI.backgroundColor = Color.white;

                GUILayout.Label("<color=black><b><size=12>Filters</size></b></color>", u, GUILayout.ExpandWidth(true));

                GUILayout.Space(10f);

                _blackAndWhite.boolValue = DrawToggle("Black & white", _blackAndWhite.boolValue);

                GUILayout.Space(10f);

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            if (DrawHeader("Back Properties"))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(20f);

                GUILayout.BeginVertical();

                GUILayout.Space(10f);

                _cardBack.objectReferenceValue = EditorGUILayout.ObjectField("Diffuse", _cardBack.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));
                _cardBackAlpha.objectReferenceValue = EditorGUILayout.ObjectField("Alpha", _cardBackAlpha.objectReferenceValue, typeof(Texture2D), false, GUILayout.ExpandWidth(false));


                GUILayout.Space(10f);

                GUILayout.EndVertical();


                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();

                ((Card)target).Redraw();

                Undo.RecordObject(target, "Hyper Card change");
            }
        }

        private void OnSceneGUI()
        {

        }

        [UnityEditor.Callbacks.PostProcessScene]
        private static void OnPostProcessScene()
        {
            ReloadAllCards();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ReloadAllCards();
        }

        public static void ReloadAllCards()
        {
            Card[] cards = (Card[])FindObjectsOfType(typeof(Card));

            foreach (var card in cards)
            {
                card.Redraw();
            }
        }

        static public bool DrawHeader(string text, int level = 0, bool forceOn = false)
        {
            return DrawHeader(text, text, forceOn, level, Color.black);
        }

        static public bool DrawHeader(string text, string key, bool forceOn, int level, Color textColor)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();

            GUILayout.Space(level * 20);

            var style = new GUIStyle("dragtab");

            GUI.color = textColor;

            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;

            text = "<color=white>" + text + "</color>";

            if (!GUILayout.Toggle(true, text, style, GUILayout.MinWidth(20f)))
                state = !state;

            GUI.color = Color.white;
            GUI.contentColor = Color.white;

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        public static bool DrawToggle(bool state)
        {
            return DrawToggle("Enable ?", state);
        }

        public static bool DrawToggle(string text, bool state, string tooltip = null)
        {
            GUILayout.BeginVertical();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();

            if (!String.IsNullOrEmpty(tooltip))
            {
                GUILayout.Label(new GUIContent(text, tooltip), GUILayout.ExpandWidth(true));
            }
            else
            {
                GUILayout.Label(text, GUILayout.ExpandWidth(true));
            }

            GUI.backgroundColor = !state ? new Color(0.8f, 0.8f, 0.8f) : new Color(0, 0.8f, 0f);

            if (!GUILayout.Toggle(true, state ? "Enabled" : "Disabled", "button", GUILayout.Width(100f)))
                state = !state;

            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.EndVertical();

            return state;
        }

        public static bool DrawMiniToggle(string text, bool state)
        {
            GUI.backgroundColor = !state ? new Color(0.8f, 0.8f, 0.8f) : new Color(0, 0.8f, 0f);

            if (!GUILayout.Toggle(true, state ? text : text, "button", GUILayout.Width(50f)))
                state = !state;

            GUI.backgroundColor = Color.white;

            return state;
        }
    }
}
#endif
