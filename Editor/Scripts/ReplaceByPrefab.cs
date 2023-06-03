// =================================================================================================
//    Author: Tomas "SkyTosS" Szilagyi
//    Date: 26.01.2023
// =================================================================================================

using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Unobex.ReplacePrefab.Editor
{
    internal struct ErrorData
    {
        // TODO: Dokoncit zobrazovanie Errorov - red color vybrat        
        public bool _isNullComponent01;
        public bool _isNullComponent02;
    }

    public class ReplaceByPrefab : EditorWindow
    {
        const string TOOLTIP_MESSAGE = "Replace Hierarchy Component By Prefab From Project";

#region [PRIVATE]

        private bool _initialized;
        private RectTransform _originalComponent;
        private RectTransform _hierarchyComponent;
        private bool _copyHierarchyComponentName = true;
        private bool _copyHierarchyComponentValues = true;
        private bool _destroyHierarchyComponent;
        private bool _copyText;
        private bool _copyImage;
        private GUIStyle _headerStyle;
        private GUIStyle _headerTextStyle;
        private GUIContent _replaceButtonGUIContent;

#endregion

#region [TOOL COLORS]

        private readonly Color _colorWhite = Color.white;
        private readonly Color _buttonInvalidState = new Color(0.6f, 0.6f, 0.6f);
        private readonly Color _buttonColorOrange = new Color(1f, 0.5f, 0.2f);
        private readonly Color _buttonColorGreen = new Color(0.4f, 1f, 0.1f);
        private readonly Color _buttonColorBlue = new Color(0.5f, 0.8f, 1f);
        private readonly Color _buttonColorPink = new Color(1.0f, 0.5f, 1f);
        private readonly Color _addBookmarkButtonColor = new Color(0.4f, 0.8f, 1f);
        private readonly Color _deleteButtonColor = new Color(1f, 0.2f, 0f);

#endregion

        private void OnEnable()
        {
            InitializeValues();
        }

        private void InitializeValues()
        {
            // TODO: Dokoncit123!
        }

        [MenuItem("Tools/Unobex/Replace by Prefab")]
        private static void UnityMenuQuickAccess()
        {
            DrawMainWindow();
        }

        private static void DrawMainWindow()
        {
            var mainWindow = (ReplaceByPrefab)GetWindow(typeof(ReplaceByPrefab));
            mainWindow.titleContent.text = "Replace by Prefab";
            mainWindow.titleContent.tooltip = TOOLTIP_MESSAGE;
            mainWindow.minSize = new Vector2(280f, 380f);
            mainWindow.Show();
        }

        private void OnGUI()
        {
            if (!_initialized)
            {
                InitializeStyles();
                _initialized = true;
            }

            GUILayout.BeginVertical();
            {
                DrawSectionHeader("Component from Hierarchy");
                DrawHierarchyComponent();
                EditorGUILayout.Space(10f);
                DrawSectionHeader("Component from Project (Reference)");
                DrawSetionOriginalComponent();
                EditorGUILayout.Space(10f);
                DrawSectionHeader("Basic Settings");
                DrawBasicSettings();
                EditorGUILayout.Space(10f);
                DrawSectionHeader("Additional Settings");
                DrawAdditionalSettings();
                EditorGUILayout.Space(20f);
                DrawMainButton();
            }
            GUILayout.EndVertical();
        }

        private void DrawAdditionalSettings()
        {
            // TODO: Spravit metodu na ToggleLeft GUI zafarbenie
            GUI.color = _copyText ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            _copyText = EditorGUILayout.ToggleLeft("Copy Text", _copyText);
            GUI.color = _copyImage ? Color.white : new Color(0.7f, 0.7f, 0.7f);
            _copyImage = EditorGUILayout.ToggleLeft("Copy Image", _copyImage);
        }

        private void DrawMainButton()
        {
            GUI.color = _buttonColorBlue;
            if (GUILayout.Button(_replaceButtonGUIContent, GUILayout.Height(30f)))
            {
                if (_originalComponent == null || _hierarchyComponent == null)
                {
                    // TODO: Dokoncit vypis
                    Debug.LogError("NIECO JE ZLE!!!!");
                }
                else
                {
                    var instComponent = (RectTransform)PrefabUtility.InstantiatePrefab(
                        _originalComponent,
                        _hierarchyComponent.parent);
                    if (_copyHierarchyComponentName)
                    {
                        instComponent.name = _hierarchyComponent.name;
                    }

                    if (_copyHierarchyComponentValues)
                    {
                        instComponent.sizeDelta = _hierarchyComponent.sizeDelta;
                        instComponent.anchoredPosition = _hierarchyComponent.anchoredPosition;
                        instComponent.anchorMin = _hierarchyComponent.anchorMin;
                        instComponent.anchorMax = _hierarchyComponent.anchorMax;
                        instComponent.pivot = _hierarchyComponent.pivot;
                    }

                    if (_copyText)
                    {
                        var isHierarchyComponentText = GetComponentOfType<TextMeshProUGUI>(
                            _hierarchyComponent,
                            out var hierarchyComponentText);
                        var isInstComponentText = GetComponentOfType<TextMeshProUGUI>(
                            instComponent,
                            out var instComponentText);

                        if (isHierarchyComponentText && isInstComponentText)
                        {
                            instComponentText.text = hierarchyComponentText.text;
                            instComponentText.alignment = hierarchyComponentText.alignment;
                        }
                    }

                    if (_copyImage)
                    {
                        var isHierarchyComponentImage = GetComponentOfType<Image>(
                            _hierarchyComponent,
                            out var hierarchyComponentImage);
                        var isInstComponentImage = GetComponentOfType<Image>(
                            instComponent,
                            out var instComponentImage);

                        if (isHierarchyComponentImage && isInstComponentImage)
                        {
                            instComponentImage.sprite = hierarchyComponentImage.sprite;
                        }
                    }

                    if (_destroyHierarchyComponent)
                    {
                        DestroyImmediate(_hierarchyComponent.gameObject);
                    }
                }
            }

            GUI.color = Color.white;
        }

        private bool GetComponentOfType<T>(Component component, out T outComponent)
        {
            T componentOfType = component.GetComponent<T>();

            if (componentOfType == null)
            {
                outComponent = default;
                return false;
            }

            outComponent = componentOfType;
            return true;
        }

        private void DrawBasicSettings()
        {
            GUI.color = _copyHierarchyComponentName ? Color.white : _buttonColorBlue;
            _copyHierarchyComponentName = EditorGUILayout.ToggleLeft(
                "Copy Hierarchy Component Name",
                _copyHierarchyComponentName);
            GUI.color = Color.white;

            _copyHierarchyComponentValues = EditorGUILayout.ToggleLeft(
                "Copy Hierarchy Component Values",
                _copyHierarchyComponentValues);
            _destroyHierarchyComponent = EditorGUILayout.ToggleLeft(
                "Destroy Hierarchy Component",
                _destroyHierarchyComponent);
        }

        private void DrawHierarchyComponent()
        {
            SetGUIColored(_hierarchyComponent == null);
            _hierarchyComponent = (RectTransform)EditorGUILayout.ObjectField(
                _hierarchyComponent,
                typeof(RectTransform),
                true);
            SetGUIColored();
        }

        private void DrawSetionOriginalComponent()
        {
            SetGUIColored(_originalComponent == null);
            _originalComponent = (RectTransform)EditorGUILayout.ObjectField(
                _originalComponent,
                typeof(RectTransform),
                false);
            SetGUIColored();
        }

        private void DrawSectionHeader(string labelText)
        {
            GUILayout.BeginHorizontal(_headerStyle, GUILayout.Height(30f));
            {
                GUILayout.Label(labelText, _headerTextStyle, GUILayout.Height(30f));
                GUI.color = _colorWhite;
            }
            GUILayout.EndHorizontal();
        }

        private void InitializeStyles()
        {
            var headerTexture = GetTexture2D("header-bg");
            _headerStyle = new GUIStyle
            {
                normal =
                {
                    background = headerTexture
                }
            };

            _headerTextStyle = new GUIStyle
            {
                normal =
                {
                    textColor = new Color(0.4f, 0.4f, 0.4f)
                },
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(6, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft
            };

            _replaceButtonGUIContent = new GUIContent
            {
                image = EditorGUIUtility.IconContent("d_PlayButton On").image,
                text = "Replace"
            };
        }

        private Texture2D GetTexture2D(string textureName)
        {
            var guidPrefabHistoryData = AssetDatabase.FindAssets(textureName);

            if (guidPrefabHistoryData.Length == 0) return null;

            var pathPrefabHistoryData = AssetDatabase.GUIDToAssetPath(guidPrefabHistoryData[0]);

            return (Texture2D)AssetDatabase.LoadAssetAtPath(
                pathPrefabHistoryData,
                typeof(Texture2D));
        }

        private void SetGUIColored(bool isError = false)
        {
            if (isError)
            {
                GUI.contentColor = Color.yellow;
                GUI.backgroundColor = Color.red;
                return;
            }

            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
        }
    }
}