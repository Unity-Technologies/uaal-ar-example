using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage.Editor
{
    [CustomEditor(typeof(TranslucentImageSource))]
    [CanEditMultipleObjects]
    public class TranslucentImageSourceEditor : UnityEditor.Editor
    {
        int      tab, previousTab;
        AnimBool advanced = new AnimBool(false);

        #region constants

        const int Min           = 0;
        const int MaxIteration  = 6;
        const int MaxDownsample = 6;

        readonly GUIContent sizeLabel = new GUIContent("Size",
                                                       "Blurriness. Does NOT affect performance");

        readonly GUIContent iterLabel = new GUIContent("Iteration",
                                                       "The number of times to run the algorithm to increase the smoothness of the effect. Can affect performance when increase");

        readonly GUIContent dsLabel = new GUIContent("Downsample",
                                                     "Reduce the size of the screen before processing. Increase will improve performance but create more artifact.");

        readonly GUIContent regionLabel = new GUIContent("Blur Region",
                                                         "Choose which part of the screen to blur. Blur smaller region is faster.");

        readonly GUIContent depthLabel = new GUIContent("Max Depth",
                                                        "Decrease will reduce flickering, blurriness and performance");

        readonly GUIContent updateRateLabel = new GUIContent("Max Update Rate",
                                                             "How many time to blur per second. Reduce to increase performance and save battery for slow moving background");

        readonly GUIContent previewLabel = new GUIContent("Preview",
                                                          "Preview the effect over the entire screen");

        #endregion

        void Awake()
        {
            LoadTab();
            advanced.value = tab > 0;
        }

        void OnEnable()
        {
            //Smoothly switch tab
            advanced.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            var tiSource = (TranslucentImageSource) target;


            DrawTabbar(tiSource);

            EditorGUILayout.Space();
            //Common properties

            tiSource.BlurRegion    = EditorGUILayout.RectField(regionLabel, tiSource.BlurRegion);
            tiSource.MaxDepth      = EditorGUILayout.IntField(depthLabel, tiSource.MaxDepth);
            tiSource.maxUpdateRate = EditorGUILayout.FloatField(updateRateLabel, tiSource.maxUpdateRate);
            tiSource.preview       = EditorGUILayout.Toggle(previewLabel, tiSource.preview);

            EditorUtility.SetDirty(target);
            Undo.RecordObject(target, "Change Translucent Image Source property");
        }

        void DrawTabsContent(TranslucentImageSource tiSource)
        {
            //Simple tab
            if (EditorGUILayout.BeginFadeGroup(1 - advanced.faded))
            {
                tiSource.Strength = Mathf.Max(0, EditorGUILayout.FloatField("Strength", tiSource.Strength));
            }

            EditorGUILayout.EndFadeGroup();

            //Advanced tab
            if (EditorGUILayout.BeginFadeGroup(advanced.faded))
            {
                tiSource.Size = EditorGUILayout.FloatField(sizeLabel, tiSource.Size);

                tiSource.Iteration = EditorGUILayout.IntSlider(new GUIContent(iterLabel),
                                                               tiSource.Iteration,
                                                               Min,
                                                               MaxIteration);

                tiSource.Downsample = EditorGUILayout.IntSlider(new GUIContent(dsLabel),
                                                                tiSource.Downsample,
                                                                Min,
                                                                MaxDownsample);
            }

            EditorGUILayout.EndFadeGroup();
        }

        void DrawTabbar(TranslucentImageSource tiSource)
        {
            EditorGUILayout.Space();

            using (var v = new EditorGUILayout.VerticalScope())
            {
                GUI.Box(v.rect, GUIContent.none);
                EditorGUILayout.LabelField("Blur settings", EditorStyles.centeredGreyMiniLabel);

                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    tab = GUILayout.Toolbar(tab,
                                            new[] {"Simple", "Advanced"},
                                            GUILayout.MinWidth(0),
                                            GUILayout.MaxWidth(EditorGUIUtility.pixelsPerPoint * 192));
                    if (tab != previousTab)
                    {
                        SaveTab();
                        previousTab = tab;
                    }

                    advanced.target = tab > 0;

                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.Space();

                DrawTabsContent(tiSource);

                EditorGUILayout.Space();
            }
        }

        //Persist selected tab between sessions and instances
        void SaveTab()
        {
            EditorPrefs.SetInt("tab", tab);
        }

        void LoadTab()
        {
            if (EditorPrefs.HasKey("tab"))
                tab = EditorPrefs.GetInt("tab");
        }
    }
}