using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Vozon.UI
{
    public class UISystem : MonoBehaviour
    {
        private static UISystem instance;
        public static UISystem Instance => instance;

        private Dictionary<string, GameObject> uiPanels;
        private Stack<GameObject> panelHistory;
        private Canvas mainCanvas;
        private UISettings settings;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUISystem();
        }

        private void InitializeUISystem()
        {
            Debug.Log("Initializing VOZON UI System...");
            uiPanels = new Dictionary<string, GameObject>();
            panelHistory = new Stack<GameObject>();
            settings = new UISettings();
            SetupMainCanvas();
        }

        private void SetupMainCanvas()
        {
            GameObject canvasObj = new GameObject("MainCanvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.SetParent(transform);
        }

        public void RegisterPanel(string panelName, GameObject panel)
        {
            if (!uiPanels.ContainsKey(panelName))
            {
                panel.transform.SetParent(mainCanvas.transform, false);
                uiPanels.Add(panelName, panel);
                panel.SetActive(false);
                Debug.Log($"Registered UI panel: {panelName}");
            }
        }

        public void ShowPanel(string panelName)
        {
            if (uiPanels.TryGetValue(panelName, out GameObject panel))
            {
                if (panelHistory.Count > 0)
                {
                    GameObject currentPanel = panelHistory.Peek();
                    if (settings.hideCurrentPanelOnShow)
                        currentPanel.SetActive(false);
                }

                panel.SetActive(true);
                panelHistory.Push(panel);
                OnPanelShown(panel);
            }
        }

        public void HideCurrentPanel()
        {
            if (panelHistory.Count > 0)
            {
                GameObject panel = panelHistory.Pop();
                panel.SetActive(false);
                OnPanelHidden(panel);

                if (panelHistory.Count > 0 && settings.showPreviousPanelOnHide)
                {
                    GameObject previousPanel = panelHistory.Peek();
                    previousPanel.SetActive(true);
                }
            }
        }

        private void OnPanelShown(GameObject panel)
        {
            IUIPanel uiPanel = panel.GetComponent<IUIPanel>();
            uiPanel?.OnPanelShow();
        }

        private void OnPanelHidden(GameObject panel)
        {
            IUIPanel uiPanel = panel.GetComponent<IUIPanel>();
            uiPanel?.OnPanelHide();
        }

        public void SetCanvasScaleMode(CanvasScaler.ScaleMode mode)
        {
            CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = mode;
            }
        }

        public void SetReferenceResolution(Vector2 resolution)
        {
            CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.referenceResolution = resolution;
            }
        }
    }

    public interface IUIPanel
    {
        void OnPanelShow();
        void OnPanelHide();
    }

    public class UISettings
    {
        public bool hideCurrentPanelOnShow = true;
        public bool showPreviousPanelOnHide = true;
        public float panelTransitionDuration = 0.3f;
    }
} 