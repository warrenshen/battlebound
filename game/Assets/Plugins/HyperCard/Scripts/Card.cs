﻿/***************************************************************************\
Project:        HyperCard
Copyright (c)   Enixion
Developer       Bourgot Jean-Louis
\***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Rendering;

#pragma warning disable 649
namespace HyperCard
{
    [Serializable]
    public enum BlendDebug
    {
        None,
        Red,
        Green,
        Blue
    }

    [Serializable]
    public enum TextMeshProParamDisplayMode
    {
        Field,
        TextArea
    }

    [ExecuteInEditMode]
    [Serializable]
    public class Card : MonoBehaviour
    {
        // Begin Nick Extensions
        public static Color DEFAULT_OUTLINE_START_COLOR = new Color(1, 0.8f, 0);
        public static Color DEFAULT_OUTLINE_END_COLOR = new Color(1, 0.25F, 0);
        // End Nick ext.

        public Renderer Renderer
        {
            get { return GetComponent<MeshRenderer>(); } // Renderer
        }

        //custom
        public struct Reset
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
        }
        public Reset reset;

        //
        public bool Disabled { get; set; }

        #region TextMeshPro

        [SerializeField]
        private bool _showTmpProps;

        [Serializable]
        public class TextMeshProParam
        {
            public TextMeshProParam()
            {
                DisplayMode = TextMeshProParamDisplayMode.Field;
            }

            public string Key;

            public TMP_Text TmpObject;

            public string Value;

            public TextMeshProParamDisplayMode DisplayMode;
        }

        public List<TextMeshProParam> TmpTextObjects = new List<TextMeshProParam>(0);
        #endregion

        #region Custom sprites

        [Serializable]
        public class CustomSpriteParam
        {
            public string Key;
            public SpriteRenderer Sprite;
            public Texture2D Value;
            public Color Color = Color.white;
            public bool IsHidden;
            public Vector3 Position = Vector3.zero;
            public Vector2 Scale = Vector2.one;
            public bool ShowAdvancedSettings;
            public int RenderQueue = 3000;
            public bool IsAffectedByFilters;
        }

#if UNITY_EDITOR
        public void CreateSprite(int index)
        {
            var prefab =
                (GameObject)AssetDatabase.LoadAssetAtPath("Assets/HyperCard/Prefabs/Sprite.prefab",
                    typeof(GameObject));

            var sprite = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

            sprite.gameObject.transform.parent = this.gameObject.transform;
            sprite.gameObject.transform.position = Vector3.zero;
            sprite.gameObject.name = "HyperCardSprite-" + index;

            var spriteObject = SpriteObjects.Last();
            spriteObject.Sprite = sprite.GetComponent<SpriteRenderer>();
            spriteObject.Color = Color.white;
            spriteObject.Key = "Sprite" + index;
            spriteObject.Scale = Vector2.one;
            spriteObject.RenderQueue = 3000;
            spriteObject.IsAffectedByFilters = true;

            Redraw();
        }
#endif

        public void RemoveSprite(int index)
        {
            var sprite = SpriteObjects.ElementAt(index);

            DestroyImmediate(sprite.Sprite.gameObject);
        }

        public List<CustomSpriteParam> SpriteObjects = new List<CustomSpriteParam>(0);

        #endregion

        // don't modify these
        public Material BaseCardFaceMat;
        public Material BaseCardBackMat;
        public Material BaseSpriteMat;

        // settings
        public int Stencil = 0;
        public int Seed;

        // debug RGB channel
        public BlendDebug DebugBlend;

        public Texture2D CardAlpha;
        public Texture2D CardMask;
        public Texture2D CardFaceFrame;
        public Color CardFaceFrameColor = Color.white;

        public Texture2D CardFaceArtwork;
        public Vector2 CardFaceArtworkOffset = Vector2.zero;
        public Vector2 CardFaceArtworkScale = Vector2.one;

        public Texture2D CardFaceBackgroundArtwork;
        public Vector2 CardFaceBackgroundArtworkOffset = Vector2.zero;
        public Vector2 CardFaceBackgroundArtworkScale = Vector2.one;

        // distortion fx
        public bool EnableDistortion0;
        public Texture2D Distortion0Mask;

        public float Distortion0Freq_Red;
        public float Distortion0Amp_Red;
        public float Distortion0Speed_Red;
        public Vector2 Distortion0Direction_Red;

        public float Distortion0Freq_Green;
        public float Distortion0Amp_Green;
        public float Distortion0Speed_Green;
        public Vector2 Distortion0Direction_Green;

        public float Distortion0Freq_Blue;
        public float Distortion0Amp_Blue;
        public float Distortion0Speed_Blue;
        public Vector2 Distortion0Direction_Blue;

        // overlay fx
        public bool EnableOverlay;
        public Texture2D Overlay1Mask;
        public Texture2D Overlay1Diffuse;
        public Vector2 Overlay1MovSpeed;
        public float Overlay1AlphaMult;
        public Color Overlay1Color;

        public float Distortion1Freq;
        public float Distortion1Amp;
        public float Distortion1Speed;
        public Vector2 Distortion1Direction;

        // periodical fx
        public bool EnablePeriodicalFx;
        public Texture2D PeriodicalFxDiffuse;
        public Texture2D PeriodicalFxMask;
        public Color PeriodicalFxColor = Color.white;
        public float PeriodicalFxAlpha;
        public float PeriodicalFxDelayOffMin = 1;
        public float PeriodicalFxDelayOffMax = 1;
        public float PeriodicalFxDelayOnMin = 1;
        public float PeriodicalFxDelayOnMax = 1;
        public float PeriodicalFxFadeDelay = 0;

        // outline
        public bool EnableOutline = false;
        public bool EnableBackOutline = false;

        public float OutlineNoiseFreq = 5;
        public float OutlineNoiseSpeed = 0.2f;
        public float OutlineNoiseMult = 0.5f;
        public float OutlineNoiseDistance = 0.1f;
        public float OutlineNoiseOffset = 0.5f;
        public float OutlineNoiseThreshold = 1f;
        public float OutlineNoiseVerticalAjust = 0;

        public float OutlineTrimOffset = 0.03f;
        public Vector2 OutlinePosOffset = Vector2.zero;

        public Color OutlineColor = new Color(1, 0.8f, 1);
        public Color OutlineEndColor = new Color(1, 0.25f, 1);
        public float OutlineEndColorDistance = 0.15f;

        public float OutlineAlphaMult = 5;
        public float OutlineWidth;
        public float OutlineHeight;
        public float OutlineSmooth;
        public float OutlineSmoothSpeed = 1;

        public float BurnNoiseFreq = 3;
        public float BurningAmount;
        public float BurnMapRotateSpeed;
        public float BurningOutline = 0.05f;
        public Color BurnColor = Color.white;
        public Color BurnEndColor = Color.red;
        public float BurnAlphaCut = 0.25f;
        public float BurnExposure = 1;

        // blend fx
        public bool EnableMixTexture = false;
        public Texture2D MixTextureMask;
        public Texture2D MixTexture;
        public Color MixTextureColor = Color.black;
        public Vector2 MixTextureOffset = Vector2.zero;
        public Vector2 MixTextureScale = Vector2.one;
        public float MixAlpha = 1;
        public float MixNoiseFreq = 5;
        public float MixNoiseMult = 0.5f;
        public float MixNoiseOffset = 0.5f;
        public float MixNoiseThreshold = 1f;
        public Vector2 MixNoiseMoveDir = new Vector2(1, 0);
        public Color MixNoiseStartColor = Color.blue;
        public Color MixNoiseEndColor = Color.white;
        public Texture2D MixNoiseTextureMask;
        public bool MixNoiseThresholdInvert;
        public float MixNoiseAlpha = 1;

        public float MixNoiseDistFreq;
        public float MixNoiseDistAmp;
        public Vector2 MixNoiseDistSpeed;
        public Vector2 MixNoiseDistDir;
        public Vector2 MixNoiseMaskOffset = Vector2.zero;
        public Vector2 MixNoiseMaskScale = Vector2.one;
        public float MixNoiseColorExposure = 1;

        // glitter fx
        public bool EnableGlitter = false;
        public Texture2D GlitterMask;
        public Texture2D GlitterMap;
        public Vector2 GlitterMapScale = Vector2.one;
        public Color GlitterColor = Color.white;
        public float GlitterPower;
        public float GlitterSpeed;
        public float GlitterContrast;
        public Texture2D GlitterSpecMap;
        public float GlitterSpecPower;
        public float GlitterSpecContrast;

        // canvas
        public bool CanvasMode;
        public float CanvasOffsetX;
        public float CanvasOffsetY;
        public RectTransform ParentCanvasTransform;
        public RectTransform CanvasTransform;

        // effects
        public bool BlackAndWhite = false;

        // misc
        public float CardOpacity = 1;

        // back
        public Texture2D CardBack;
        public Texture2D CardBackAlpha;

        public void Copy(HyperCard.Card copyFrom)
        {
            this.CardFaceArtwork = copyFrom.CardFaceArtwork;
            this.CardFaceBackgroundArtwork = copyFrom.CardFaceBackgroundArtwork;
            this.CardFaceFrameColor = copyFrom.CardFaceFrameColor;

            this.OutlineColor = copyFrom.OutlineColor;
            this.OutlineEndColor = copyFrom.OutlineEndColor;
            this.EnableOutline = copyFrom.EnableOutline;

            this.CardFaceArtworkScale = copyFrom.CardFaceArtworkScale;
            this.CardFaceArtworkOffset = copyFrom.CardFaceArtworkOffset;
            this.CardFaceBackgroundArtworkScale = copyFrom.CardFaceBackgroundArtworkScale;
            this.CardFaceBackgroundArtworkOffset = copyFrom.CardFaceBackgroundArtworkOffset;

            this.CardOpacity = copyFrom.CardOpacity;
            this.SetTextFieldWithKey("Cost", copyFrom.GetTextFieldWithKey("Cost").Value);
            this.SetTextFieldWithKey("Attack", copyFrom.GetTextFieldWithKey("Attack").Value);
            this.SetTextFieldWithKey("Health", copyFrom.GetTextFieldWithKey("Health").Value);
            this.SetTextFieldWithKey("Title", copyFrom.GetTextFieldWithKey("Title").Value);
            this.SetTextFieldWithKey("Description", copyFrom.GetTextFieldWithKey("Description").Value);

            this.Redraw();
        }

        public void Redraw()
        {
            if (BaseCardFaceMat == null || BaseCardBackMat == null || BaseSpriteMat == null)
            {
                Debug.LogError("Hypercard : no materials.");
                return;
            }
            else if (Renderer.sharedMaterials[0] == null || Renderer.sharedMaterials[1] == null)
            {
                Material[] materials = Renderer.sharedMaterials;
                materials[0] = BaseCardFaceMat;
                materials[1] = BaseCardBackMat;
                Renderer.sharedMaterials = materials;
            }

            if (Seed == 0 || Seed > 99999)
            {
                Seed = UnityEngine.Random.Range(0, 99999);
            }

            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;

            ForceTextUpdate();
            StartCoroutine(ApplyCulling());
            UpdateCustomSprites();
            StartCoroutine(ComputeSprites());

#if UNITY_EDITOR
            var tmpHideFlags = HideFlags.None;

            if (!_showTmpProps)
            {
                tmpHideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            }

            foreach (var txt in TmpTextObjects.Where(x => x.TmpObject != null))
            {
                txt.TmpObject.gameObject.hideFlags = tmpHideFlags;
            }

            foreach (var x in SpriteObjects.Where(x => x.Sprite != null))
            {
                x.Sprite.gameObject.hideFlags = tmpHideFlags | HideFlags.NotEditable;
            }

            foreach (Transform x in transform)
            {
                x.gameObject.hideFlags = HideFlags.None;
            }
#endif

            var mat = Renderer.sharedMaterials;

            var faceMat = new Material(mat[0]);
            var backMat = mat[1];

            ComputePeriodicalFx();
            var mask = CardMask;

#if UNITY_EDITOR
            switch (DebugBlend)
            {
                case BlendDebug.Red:
                    mask = new Texture2D(1, 1);
                    mask.SetPixel(0, 0, Color.red);
                    mask.Apply();
                    break;
                case BlendDebug.Green:
                    mask = new Texture2D(1, 1);
                    mask.SetPixel(0, 0, Color.green);
                    mask.Apply();
                    break;
                case BlendDebug.Blue:
                    mask = new Texture2D(1, 1);
                    mask.SetPixel(0, 0, Color.blue);
                    mask.Apply();
                    break;
                case BlendDebug.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif

            faceMat.SetInt("_Stencil", Stencil);
            backMat.SetInt("_Stencil", Stencil);

            faceMat.SetInt("_Seed", Seed);

            faceMat.SetTexture("_CardMask", mask);

            faceMat.SetTexture("_CardAlpha", CardAlpha);
            backMat.SetTexture("_CardAlpha", CardAlpha);

            faceMat.SetFloat("_CardOpacity", CardOpacity);

            faceMat.SetTexture("_CardFrame", CardFaceFrame);
            faceMat.SetColor("_CardFrameColor", CardFaceFrameColor);
            faceMat.SetTexture("_CardForeground", CardFaceArtwork);
            faceMat.SetTextureOffset("_CardForeground", CardFaceArtworkOffset);
            faceMat.SetTextureScale("_CardForeground", CardFaceArtworkScale);

            faceMat.SetTexture("_CardBackground", CardFaceBackgroundArtwork);
            faceMat.SetTextureOffset("_CardBackground", CardFaceBackgroundArtworkOffset);
            faceMat.SetTextureScale("_CardBackground", CardFaceBackgroundArtworkScale);

            if (EnableDistortion0)
            {
                faceMat.SetFloat("_Dist0_Enabled", 1);

                faceMat.SetTexture("_CardDist0Mask", Distortion0Mask);

                faceMat.SetFloat("_Dist0Freq_Red", Distortion0Freq_Red);
                faceMat.SetFloat("_Dist0Amp_Red", Distortion0Amp_Red);
                faceMat.SetFloat("_Dist0Speed_Red", Distortion0Speed_Red);
                faceMat.SetVector("_Dist0Pos_Red", Distortion0Direction_Red);

                faceMat.SetFloat("_Dist0Freq_Green", Distortion0Freq_Green);
                faceMat.SetFloat("_Dist0Amp_Green", Distortion0Amp_Green);
                faceMat.SetFloat("_Dist0Speed_Green", Distortion0Speed_Green);
                faceMat.SetVector("_Dist0Pos_Green", Distortion0Direction_Green);

                faceMat.SetFloat("_Dist0Freq_Blue", Distortion0Freq_Blue);
                faceMat.SetFloat("_Dist0Amp_Blue", Distortion0Amp_Blue);
                faceMat.SetFloat("_Dist0Speed_Blue", Distortion0Speed_Blue);
                faceMat.SetVector("_Dist0Pos_Blue", Distortion0Direction_Blue);
            }
            else
            {
                faceMat.SetFloat("_Dist0_Enabled", 0);
            }

            if (EnableOverlay)
            {
                faceMat.SetTexture("_CardDist1Tex", Overlay1Diffuse);
                faceMat.SetTexture("_CardDist1Mask", Overlay1Mask);
                faceMat.SetFloat("_Dist1SpeedX", Overlay1MovSpeed.x);
                faceMat.SetFloat("_Dist1SpeedY", Overlay1MovSpeed.y);
                faceMat.SetFloat("_Dist1AlphaMult", Overlay1AlphaMult);
                faceMat.SetColor("_Dist1Color", Overlay1Color);
                faceMat.SetFloat("_Dist1Freq", Distortion1Freq);
                faceMat.SetFloat("_Dist1Amp", Distortion1Amp);
                faceMat.SetFloat("_Dist1Speed", Distortion1Speed);
                faceMat.SetVector("_Dist1Pos", Distortion1Direction);
            }
            else
            {
                faceMat.SetTexture("_CardDist1Tex", null);
                faceMat.SetTexture("_CardDist1Mask", null);
                faceMat.SetFloat("_Dist1AlphaMult", 0);
            }

            faceMat.SetInt("_BlackAndWhite", BlackAndWhite ? 1 : 0);

            if (EnableOutline)
            {
                faceMat.SetInt("_ShowOutline", 1);

                faceMat.SetFloat("_OutlineNoiseFreq", OutlineNoiseFreq);
                faceMat.SetFloat("_OutlineNoiseSpeed", OutlineNoiseSpeed);
                faceMat.SetFloat("_OutlineNoiseMult", OutlineNoiseMult);
                faceMat.SetFloat("_OutlineNoiseDistance", OutlineNoiseDistance);
                faceMat.SetFloat("_OutlineNoiseOffset", OutlineNoiseOffset);
                faceMat.SetFloat("_OutlineNoiseThreshold", OutlineNoiseThreshold);
                faceMat.SetFloat("_OutlineNoiseVerticalAjust", OutlineNoiseVerticalAjust);
                faceMat.SetFloat("_OutlineWidth", OutlineWidth / 10);
                faceMat.SetFloat("_OutlineHeight", OutlineHeight / 10);
                faceMat.SetColor("_OutlineEndColor", OutlineEndColor);
                faceMat.SetFloat("_OutlineEndColorDistance", OutlineEndColorDistance);
                faceMat.SetColor("_OutlineColor", OutlineColor);
                faceMat.SetFloat("_OutlineSmooth", OutlineSmooth);
                faceMat.SetFloat("_OutlineSmoothSpeed", OutlineSmoothSpeed);
                faceMat.SetFloat("_OutlineTrimOffset", OutlineTrimOffset);
                faceMat.SetVector("_OutlinePosOffset", OutlinePosOffset);
                faceMat.SetFloat("_OutlineAlphaMult", OutlineAlphaMult);
            }
            else
            {
                faceMat.SetInt("_ShowOutline", 0);
            }

            faceMat.SetTexture("_PeriodicalFxTex", PeriodicalFxDiffuse);
            faceMat.SetTexture("_PeriodicalFxMask", PeriodicalFxMask);
            faceMat.SetColor("_PeriodicalFxColor", PeriodicalFxColor);
            faceMat.SetFloat("_PeriodicalFxAlpha", PeriodicalFxAlpha);

            // dissolve
            faceMat.SetFloat("_BurnNoiseFreq", BurnNoiseFreq);
            faceMat.SetFloat("_BurningAmount", BurningAmount);
            faceMat.SetFloat("_BurningRotateSpeed", BurnMapRotateSpeed);
            faceMat.SetFloat("_BurningOutline", BurningOutline);
            faceMat.SetColor("_BurnColor", BurnColor);
            faceMat.SetColor("_BurnEndColor", BurnEndColor);
            faceMat.SetFloat("_BurnAlphaCut", BurnAlphaCut);
            faceMat.SetFloat("_BurnExposure", BurnExposure);

            // blend mix
            faceMat.SetInt("_MixTexture_Enabled", EnableMixTexture ? 1 : 0);

            if (EnableMixTexture)
            {
                faceMat.SetTexture("_MixTextureMask", MixTextureMask);
                faceMat.SetTexture("_MixTexture", MixTexture);
                faceMat.SetTextureOffset("_MixTexture", MixTextureOffset);
                faceMat.SetTextureScale("_MixTexture", MixTextureScale);
                faceMat.SetColor("_MixTextureColor", MixTextureColor);
                faceMat.SetFloat("_MixAlpha", MixAlpha);

                faceMat.SetFloat("_MixNoiseFreq", MixNoiseFreq);
                faceMat.SetFloat("_MixNoiseMult", MixNoiseMult);
                faceMat.SetVector("_MixNoiseMoveDir", MixNoiseMoveDir);

                faceMat.SetFloat("_MixNoiseOffset", MixNoiseOffset);
                faceMat.SetFloat("_MixNoiseThreshold", MixNoiseThreshold);

                faceMat.SetColor("_MixNoiseStartColor", MixNoiseStartColor);
                faceMat.SetColor("_MixNoiseEndColor", MixNoiseEndColor);

                faceMat.SetTexture("_MixNoiseTextureMask", MixNoiseTextureMask);
                faceMat.SetFloat("_MixNoiseThresholdInvert", MixNoiseThresholdInvert ? 1 : 0);
                faceMat.SetFloat("_MixNoiseAlpha", MixNoiseAlpha);
                faceMat.SetFloat("_MixNoiseDistFreq", MixNoiseDistFreq);
                faceMat.SetFloat("_MixNoiseDistAmp", MixNoiseDistAmp);
                faceMat.SetVector("_MixNoiseDistSpeed", MixNoiseDistSpeed);
                faceMat.SetVector("_MixNoiseDistDir", MixNoiseDistDir);
                faceMat.SetFloat("_MixNoiseColorExposure", MixNoiseColorExposure);

                faceMat.SetTextureOffset("_MixNoiseTextureMask", MixNoiseMaskOffset);
                faceMat.SetTextureScale("_MixNoiseTextureMask", MixNoiseMaskScale);
            }

            // holo, removed by nick 6/30/18, needed the sampler for background

            // Glitter
            faceMat.SetInt("_Glitter_Enabled", EnableGlitter ? 1 : 0);

            if (EnableGlitter)
            {
                faceMat.SetTexture("_GlitterMask", GlitterMask);
                faceMat.SetTexture("_GlitterMap", GlitterMap);
                faceMat.SetTextureScale("_GlitterMap", GlitterMapScale);
                faceMat.SetColor("_GlitterColor", GlitterColor);
                faceMat.SetFloat("_GlitterPower", GlitterPower);
                faceMat.SetFloat("_GlitterSpeed", GlitterSpeed);
                faceMat.SetFloat("_GlitterContrast", GlitterContrast);
                faceMat.SetTexture("_GlitterSpecMap", GlitterSpecMap);
                faceMat.SetFloat("_GlitterSpecPower", GlitterSpecPower);
                faceMat.SetFloat("_GlitterSpecContrast", GlitterSpecContrast);
            }

            // back
            backMat.SetTexture("_BackTexture", CardBack);
            backMat.SetTexture("_CardAlpha", CardBackAlpha);

            faceMat.name = Guid.NewGuid().ToString();

            Renderer.materials = new[] { faceMat, backMat };
        }

        private void Start()
        {
            StartCoroutine(ApplyCulling());

            _fxOffTime = UnityEngine.Random.Range(PeriodicalFxDelayOffMin, PeriodicalFxDelayOffMax);
        }

        private void OnEnable()
        {
            Redraw();
        }

        private IEnumerator ApplyCulling()
        {
            yield return new WaitForEndOfFrame();

            foreach (var txt in TmpTextObjects.Where(x => x.TmpObject != null))
            {
                txt.TmpObject.fontSharedMaterial.SetFloat("_CullMode", 2f);
            }
        }

        private IEnumerator ComputeSprites()
        {
            yield return new WaitForEndOfFrame();

            foreach (var txt in TmpTextObjects.Where(x => x.TmpObject != null))
            {
                txt.TmpObject.overrideColorTags = BlackAndWhite;
            }

            Material spriteMat = new Material(BaseSpriteMat);

            foreach (var x in SpriteObjects.Where(x => x.Sprite != null))
            {
                spriteMat.renderQueue = x.RenderQueue;
                spriteMat.SetInt("_BlackAndWhite", BlackAndWhite && x.IsAffectedByFilters ? 1 : 0);
                x.Sprite.material = spriteMat;
            }
        }

        private void UpdateCustomSprites()
        {
            foreach (var x in SpriteObjects.Where(x => x.Sprite != null))
            {
                x.Sprite.color = x.Color;

                if (x.Value != null && x.Sprite.sprite == null)
                {
                    x.Sprite.sprite = Sprite.Create(
                        x.Value,
                        new Rect(0.0f, 0.0f, x.Value.width, x.Value.height),
                        new Vector2(0.5f, 0.5f),
                        100.0f,
                        0,
                        SpriteMeshType.FullRect
                    );
                }

                x.Sprite.gameObject.transform.localPosition = new Vector3(x.Position.x / 10, x.Position.y / 10, 0.01f + x.Position.z);
                x.Sprite.gameObject.transform.localScale = new Vector3(x.Scale.x / 10, x.Scale.y / 10, 1);
                x.Sprite.gameObject.SetActive(!x.IsHidden);
            }
        }

        private void ForceTextUpdate()
        {
            foreach (var txt in TmpTextObjects.Where(x => x.TmpObject != null))
            {
                txt.TmpObject.text = txt.Value;
            }
        }

        private void Update()
        {
            if (Renderer.sharedMaterials[0] == null || Renderer.sharedMaterials[1] == null)
            {
                return;
            }

            if (Disabled) return;

            // Change opacity of custom sprites without redrawing all card
            foreach (var x in SpriteObjects.Where(x => x.Sprite != null))
            {
                //color.a *= CardOpacity;
                x.Sprite.color = x.Color;
            }

            ComputePeriodicalFx();

            var mat = Renderer.sharedMaterials;

            if (CanvasMode)
            {
                if (CanvasTransform != null && ParentCanvasTransform != null)
                {
                    var dx = Utils.Remap(CanvasTransform.anchoredPosition.x, 0, ParentCanvasTransform.rect.width,
                        ParentCanvasTransform.rect.width / 2, -ParentCanvasTransform.rect.width / 2);
                    var dy = Utils.Remap(CanvasTransform.anchoredPosition.y, 0, ParentCanvasTransform.rect.height,
                        ParentCanvasTransform.rect.height / 2, -ParentCanvasTransform.rect.height / 2);

                    dx *= (OutlineWidth / 10 * 10) - 1;
                    dy *= (OutlineHeight / 10 * 10) - 1;

                    mat[0].SetFloat("_CanvasMode", CanvasMode ? 1 : 0);
                    mat[0].SetFloat("_CanvasOffsetX", dx);
                    mat[0].SetFloat("_CanvasOffsetY", dy);
                }
            }

            mat[0].SetFloat("_BurningAmount", BurningAmount);
        }

        private float _fxTime;
        private bool _fxIn;
        private bool _fxOut;
        private float _fxInTime;
        private float _fxOutTime;

        private float _fxOffTime;
        private float _fxOnTime;

        private void ComputePeriodicalFx()
        {
            if (!EnablePeriodicalFx)
            {
                PeriodicalFxAlpha = 0;
                return;
            }

            _fxTime += Time.deltaTime;

            if (_fxTime > _fxOffTime && !_fxIn && !_fxOut)
            {
                //Debug.Log("Fade in...");
                _fxIn = true;
                _fxOut = false;
                _fxTime = 0;
                _fxInTime = 0;
                _fxOnTime = UnityEngine.Random.Range(PeriodicalFxDelayOnMin, PeriodicalFxDelayOnMax);
            }

            if (_fxIn)
            {
                _fxInTime += Time.deltaTime;

                PeriodicalFxAlpha = Mathf.Lerp(0, 1, _fxInTime / PeriodicalFxFadeDelay);

                if (_fxInTime >= PeriodicalFxFadeDelay)
                {
                    if (_fxTime > _fxOnTime)
                    {
                        //Debug.Log("Fade out...");
                        _fxOffTime = UnityEngine.Random.Range(PeriodicalFxDelayOffMin, PeriodicalFxDelayOffMax);
                        _fxIn = false;
                        _fxOutTime = 0;
                        _fxOut = true;
                        _fxTime = 0;
                    }
                }
            }

            if (_fxOut)
            {
                _fxOutTime += Time.deltaTime;

                PeriodicalFxAlpha = Mathf.Lerp(1, 0, _fxOutTime / PeriodicalFxFadeDelay);

                if (_fxOutTime >= PeriodicalFxFadeDelay)
                {
                    _fxOut = false;
                }
            }

            var faceMat = new Material(Renderer.sharedMaterials[0]);

            faceMat.SetFloat("_PeriodicalFxAlpha", PeriodicalFxAlpha * PeriodicalFxColor.a);

            Renderer.materials = new[] { faceMat, Renderer.sharedMaterials[1] };
        }

        private int _index;
        private float _sheetTime;
        private float _nextSheetTime;

        public void SetCardArtwork(Texture2D front, Texture2D back)
        {
            this.CardFaceArtwork = front;
            this.CardFaceBackgroundArtwork = back;

            Renderer.material.SetTexture("_CardForeground", this.CardFaceArtwork);
            Renderer.material.SetTexture("_CardBackground", this.CardFaceArtwork);
        }
        public void SetFrontTiling(Vector2 scale, Vector2 offset)
        {
            this.CardFaceArtworkScale = scale;
            this.CardFaceArtworkOffset = offset;

            Renderer.material.SetTextureOffset("_CardForeground", CardFaceArtworkOffset);
            Renderer.material.SetTextureScale("_CardForeground", CardFaceArtworkScale);
        }
        public void SetBackTiling(Vector2 scale, Vector2 offset)
        {
            this.CardFaceBackgroundArtworkScale = scale;
            this.CardFaceBackgroundArtworkOffset = offset;

            Renderer.material.SetTextureOffset("_CardBackground", CardFaceBackgroundArtworkOffset);
            Renderer.material.SetTextureScale("_CardBackground", CardFaceBackgroundArtworkScale);
        }

        public void SetOutline(bool value)
        {
            this.EnableOutline = value;
            this.Redraw();  //to-do: should REALLY optimize this later, imp!
        }

        public void SetOutlineColors(Color start, Color end)
        {
            this.OutlineColor = start;
            this.OutlineEndColor = end;

            Renderer.material.SetColor("_OutlineColor", this.OutlineColor);
            Renderer.material.SetColor("_OutlineEndColor", this.OutlineEndColor);
            this.Redraw(); //to-do: try not to use this! some version of the above + showoutline
        }

        public void SetBlackAndWhite(bool value)
        {
            this.BlackAndWhite = value;
            this.Redraw(); //to-do: should REALLY optimize this later, imp!
        }
        public void SetGrayscale(bool value)
        {
            SetBlackAndWhite(value);
        }

        public void SetOpacity(float opacity)
        {
            this.CardOpacity = opacity;
            this.Redraw(); //to-do: should REALLY optimize this later, imp!
        }

        public TextMeshProParam GetTextFieldWithKey(string key)
        {
            foreach (TextMeshProParam element in this.TmpTextObjects)
            {
                if (element.Key != key)
                {
                    continue;
                }

                return element;
            }
            return null;
        }

        public void SetTextFieldWithKey(string key, string value)
        {
            TextMeshProParam target = GetTextFieldWithKey(key);
            if (target == null)
            {
                Debug.LogWarning(String.Format("No text field with key `{0}` found.", key));
                return;
            }

            target.Value = value;
        }

        public CustomSpriteParam GetSpriteWithKey(string key)
        {
            foreach (CustomSpriteParam element in this.SpriteObjects)
            {
                if (element.Key != key)
                    continue;
                return element;
            }
            return null;
        }

        public void SetSpriteWithKey(string key, Texture2D value)
        {
            CustomSpriteParam target = GetSpriteWithKey(key);
            if (target == null)
            {
                Debug.LogWarning(String.Format("No text field with key `{0}` found.", key));
                return;
            }
            target.Value = value;
        }

        public void ShowSprites()
        {
            foreach (CustomSpriteParam element in this.SpriteObjects)
            {
                element.Sprite.enabled = true;
            }
        }

        public void HideSprites()
        {
            foreach (CustomSpriteParam element in this.SpriteObjects)
            {
                element.Sprite.enabled = false;
            }
        }

        public void SetFrameColor(Color color)
        {
            this.CardFaceFrameColor = color;
        }

        public void EmptyAllText()
        {
            foreach (TextMeshProParam element in this.TmpTextObjects)
            {
                element.Value = "";
            }
            this.Redraw();
        }

        public void ResetParams()
        {
            GetTextFieldWithKey("Cost").TmpObject.color = Color.white;

            if (GetTextFieldWithKey("Attack") != null)
            {
                GetTextFieldWithKey("Attack").TmpObject.color = Color.white;
            }
            if (GetTextFieldWithKey("Health") != null)
            {
                GetTextFieldWithKey("Health").TmpObject.color = Color.white;
            }

            this.ShowSprites();
            this.BurningAmount = 0;
            this.BlackAndWhite = false;
            this.CardOpacity = 1;
            this.EnableOutline = false;
            this.OutlineColor = DEFAULT_OUTLINE_START_COLOR;
            this.OutlineEndColor = DEFAULT_OUTLINE_END_COLOR;
            this.EnableBackOutline = true; //to-do call methods once redraw is taken out of them

            Redraw();
        }
    }
}