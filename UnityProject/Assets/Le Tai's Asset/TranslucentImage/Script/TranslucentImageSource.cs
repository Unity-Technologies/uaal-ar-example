using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LeTai.Asset.TranslucentImage
{
    /// <summary>
    /// Common source of blur for Translucent Images. 
    /// </summary>
    /// <remarks>
    /// It is an Image effect that blur the render target of the Camera it attached to, then save the result to a global read-only  Render Texture
    /// </remarks>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
    public class TranslucentImageSource : MonoBehaviour
    {
        #region Public field

        /// <summary>
        /// Maximum number of times to update the blurred image each second
        /// </summary>
        public float maxUpdateRate = float.PositiveInfinity;

        /// <summary>
        /// Render the blurred result to the render target
        /// </summary>
        [Tooltip("Preview the effect on entire screen")]
        public bool preview;

        #endregion

        #region Field

        [SerializeField]
        float size = 5;

        [SerializeField]
        int iteration = 4;

        [SerializeField]
        Rect blurRegion = new Rect(0, 0, 1, 1);

        Rect lastBlurRegion = new Rect(0, 0, 1, 1);

        [SerializeField]
        int maxDepth = 4;

        [SerializeField]
        int downsample,
            lastDownsample;

        [SerializeField]
        float strength;

        float lastUpdate;

        //Disable non-sense warning from Unity
#pragma warning disable 0108
        Camera camera;
#pragma warning restore 0108
        Shader   shader;
        Material material;
        Material previewMaterial;

        #endregion

        #region Properties

        /// <summary>
        /// Result of the image effect. Translucent Image use this as their content (read-only)
        /// </summary>
        public RenderTexture BlurredScreen { get; private set; }

        /// <summary>
        /// The Camera attached to the same GameObject. Cached in field 'camera'
        /// </summary>
        Camera Cam
        {
            get { return camera ? camera : camera = GetComponent<Camera>(); }
        }

        /// <summary>
        /// User friendly property to control the amount of blur
        /// </summary>
        ///<value>
        /// Must be non-negative
        /// </value>
        public float Strength
        {
            get { return strength = Size * Mathf.Pow(2, Iteration + Downsample); }
            set
            {
                strength = Mathf.Max(0, value);
                SetAdvancedFieldFromSimple();
            }
        }

        /// <summary>
        /// Distance between the base texel and the texel to be sampled.
        /// </summary>
        public float Size
        {
            get { return size; }
            set { size = Mathf.Max(0, value); }
        }

        /// <summary>
        /// Half the number of time to process the image. It is half because the real number of iteration must alway be even. Using half also make calculation simpler
        /// </summary>
        /// <value>
        /// Must be non-negative
        /// </value>
        public int Iteration
        {
            get { return iteration; }
            set { iteration = Mathf.Max(0, value); }
        }

        /// <summary>
        /// The rendered image will be shrinked by a factor of 2^{{this}} before bluring to reduce processing time
        /// </summary>
        /// <value>
        /// Must be non-negative. Default to 0
        /// </value>
        public int Downsample
        {
            get { return downsample; }
            set { downsample = Mathf.Max(0, value); }
        }

        /// <summary>
        /// Define the rectangular area on screen that will be blurred.
        /// </summary>
        /// <value>
        /// Between 0 and 1
        /// </value>
        public Rect BlurRegion
        {
            get { return blurRegion; }
            set
            {
                Vector2 min = new Vector2(1 / (float) Cam.pixelWidth, 1 / (float) Cam.pixelHeight);
                value.x      = Mathf.Clamp(value.x,      0,     1 - min.x);
                value.y      = Mathf.Clamp(value.y,      0,     1 - min.y);
                value.width  = Mathf.Clamp(value.width,  min.x, 1 - value.x);
                value.height = Mathf.Clamp(value.height, min.y, 1 - value.y);
                blurRegion   = value;
            }
        }

        /// <summary>
        /// Clamp the minimum size of the intermediate texture. Reduce flickering and blur
        /// </summary>
        /// <value>
        /// Must larger than 0
        /// </value>
        public int MaxDepth
        {
            get { return maxDepth; }
            set { maxDepth = Mathf.Max(1, value); }
        }

        /// <summary>
        /// A small number base on the smaller dimension of the camera render target. 
        /// Used to retain the blur amount across screen size
        /// </summary>
        float ScreenSize
        {
            //1080f is an arbitrary number. It keep 'size' in a reasonable range
            get { return Mathf.Min(Cam.pixelWidth, Cam.pixelHeight) / 1080f; }
        }

        /// <summary>
        /// Minimum time in second to wait before refresh the blurred image. 
        /// If maxUpdateRate non-positive then just stop updating
        /// </summary>
        float MinUpdateCycle
        {
            get { return (maxUpdateRate > 0) ? (1f / maxUpdateRate) : float.PositiveInfinity; }
        }

        #endregion

        /// <summary>
        /// Calculate size and iteration from strength
        /// </summary>
        protected virtual void SetAdvancedFieldFromSimple()
        {
            Size = strength / Mathf.Pow(2, Iteration + Downsample);
            //Does not handle negative size
            while (Size < 1)
            {
                if (Downsample > 0)
                {
                    Downsample--;
                    Size *= 2;
                } else if (Iteration > 0)
                {
                    Iteration--;
                    Size *= 2;
                }

                break;
            }

            while (Size > 8)
            {
                Size /= 2;
                Iteration++;
            }
        }

        static int _sizePropId;
        static int _cropRegionPropId;

#if UNITY_EDITOR
        Texture2D previewCropTexture;

        void OnEnable()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Start();
            }

            if (!previewCropTexture)
            {
                previewCropTexture = new Texture2D(3, 3) {filterMode = FilterMode.Point};
                previewCropTexture.SetPixels(new[]
                                             {
                                                 Color.yellow, Color.yellow, Color.yellow, Color.yellow, Color.clear,
                                                 Color.yellow, Color.yellow, Color.yellow, Color.yellow,
                                             });
                previewCropTexture.Apply();
            }
        }

        void OnGUI()
        {
            if (!preview)
                return;

            var pixelArea = new Rect(blurRegion);
            pixelArea.x      *= Cam.pixelWidth;
            pixelArea.width  *= Cam.pixelWidth;
            pixelArea.y      *= Cam.pixelHeight;
            pixelArea.y      =  Cam.pixelHeight - pixelArea.y;
            pixelArea.height *= -Cam.pixelHeight;

            var style = new GUIStyle(GUI.skin.box);
            style.normal.background = previewCropTexture;
            style.border            = new RectOffset(1, 1, 1, 1);

            GUI.Box(pixelArea, GUIContent.none, style);
        }
#endif

        protected virtual void Start()
        {
            camera = Cam;

            shader = Shader.Find("Hidden/EfficientBlur");
            if (!shader.isSupported) enabled = false;

            material        = new Material(shader);
            previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));

            _sizePropId       = Shader.PropertyToID("size");
            _cropRegionPropId = Shader.PropertyToID("_CropRegion");

            CreateNewBlurredScreen();

            lastDownsample = Downsample;
        }

        protected virtual void CreateNewBlurredScreen()
        {
            BlurredScreen = new RenderTexture(Mathf.RoundToInt(Cam.pixelWidth  * BlurRegion.width)  >> Downsample,
                                              Mathf.RoundToInt(Cam.pixelHeight * BlurRegion.height) >> Downsample,
                                              0) {filterMode = FilterMode.Bilinear};
        }

        /// <summary>
        /// Resize the source texture then run it through a shader before assign to target texure
        /// </summary>
        /// <param name="sourceRt"></param>
        /// <param name="level">Resampling depth</param>
        /// <param name="target"></param>
        protected virtual void ProgressiveResampling(int level, ref RenderTexture target)
        {
            level = Mathf.Min(level + Downsample, MaxDepth);
            int rtW = BlurredScreen.width  >> level;
            int rtH = BlurredScreen.height >> level;

            RenderTexture tmpRt = RenderTexture.GetTemporary(rtW, rtH, 0, BlurredScreen.format);
            tmpRt.filterMode = FilterMode.Bilinear;

            Graphics.Blit(target, tmpRt, material, 0);
            RenderTexture.ReleaseTemporary(target);
            target = tmpRt;
        }

        protected virtual void ProgressiveBlur(RenderTexture sourceRt)
        {
            //Resize final texture if base downsample changed
            if (Downsample != lastDownsample || !BlurRegion.Equals(lastBlurRegion))
            {
                CreateNewBlurredScreen();
                lastDownsample = Downsample;
                lastBlurRegion = BlurRegion;
            }

            if (BlurredScreen.IsCreated()) BlurredScreen.DiscardContents();

            //Relative blur size to maintain same look across multiple resolution
            material.SetFloat(_sizePropId, Size * ScreenSize);

            int firstDownsampleFactor = iteration > 0 ? 1 : 0;

            int rtW = BlurredScreen.width  >> firstDownsampleFactor; //= width / (downsample + 1)^2
            int rtH = BlurredScreen.height >> firstDownsampleFactor;

            RenderTexture tmpRt = RenderTexture.GetTemporary(rtW, rtH, 0, sourceRt.format);
            tmpRt.filterMode    = FilterMode.Bilinear;
            sourceRt.filterMode = FilterMode.Bilinear;

            //Initial downsample
            material.SetVector(_cropRegionPropId,
                               new Vector4(BlurRegion.xMin,
                                           BlurRegion.yMin,
                                           BlurRegion.xMax,
                                           BlurRegion.yMax));

            Graphics.Blit(sourceRt, tmpRt, material, 1);

            //Downsample. (iteration - 1) pass 
            for (int i = 2; i <= iteration; i++)
            {
                ProgressiveResampling(i, ref tmpRt);
            }

            //Upsample. (iteration - 1) pass 
            for (int i = iteration - 1; i >= 1; i--)
            {
                ProgressiveResampling(i, ref tmpRt);
            }

            //Final upsample. Blit to blurredRt and release tmp
            Graphics.Blit(tmpRt, BlurredScreen, material, 0);
            RenderTexture.ReleaseTemporary(tmpRt);
        }

        protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            float now = Time.unscaledTime;
#if UNITY_EDITOR
            now = (float) EditorApplication.timeSinceStartup;
#endif
            if (now - lastUpdate >= MinUpdateCycle)
            {
                ProgressiveBlur(source);

                lastUpdate = Time.unscaledTime;
#if UNITY_EDITOR
                lastUpdate = (float) EditorApplication.timeSinceStartup;
#endif
            }

            #region Preview

            if (preview)
            {
                previewMaterial.SetVector("_CropRegion",
                                          new Vector4(BlurRegion.xMin,
                                                      BlurRegion.yMin,
                                                      BlurRegion.xMax,
                                                      BlurRegion.yMax));
                Graphics.Blit(BlurredScreen, destination, previewMaterial);
                return;
            }

            //Draw the screen unmodified
            Graphics.Blit(source, destination);

            #endregion
        }
    }
}