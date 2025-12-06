using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages interaction prompts that appear when players are near interactive objects.
/// Shows contextual messages like "Press X to interact" or "Press X to enter".
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Text promptText;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Prompt Settings")]
    [SerializeField] private string defaultPrompt = "Press X to interact";
    [SerializeField] private KeyCode interactionKey = KeyCode.X;

    // State
    private CanvasGroup canvasGroup;
    private bool isVisible = false;
    private string currentPrompt = "";

    void Start()
    {
        // Setup UI
        if (promptPanel == null)
        {
            CreatePromptUI();
        }

        canvasGroup = promptPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = promptPanel.AddComponent<CanvasGroup>();
        }

        // Hide initially
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        currentPrompt = defaultPrompt;
        UpdatePromptText();
    }

    void Update()
    {
        // Smooth fade in/out
        if (canvasGroup != null)
        {
            float targetAlpha = isVisible ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    /// <summary>
    /// Show the interaction prompt
    /// </summary>
    public void ShowPrompt(string customPrompt = null)
    {
        if (customPrompt != null)
        {
            currentPrompt = customPrompt;
            UpdatePromptText();
        }

        isVisible = true;
    }

    /// <summary>
    /// Hide the interaction prompt
    /// </summary>
    public void HidePrompt()
    {
        isVisible = false;
    }

    /// <summary>
    /// Set a custom prompt message
    /// </summary>
    public void SetPrompt(string prompt)
    {
        currentPrompt = prompt;
        UpdatePromptText();
    }

    /// <summary>
    /// Update the prompt text display
    /// </summary>
    private void UpdatePromptText()
    {
        if (promptText != null)
        {
            string keyName = interactionKey.ToString();
            promptText.text = currentPrompt.Replace("E", keyName);
        }
    }

    /// <summary>
    /// Create the prompt UI if it doesn't exist
    /// </summary>
    private void CreatePromptUI()
    {
        // Create canvas if needed
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("InteractionCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create prompt panel
        promptPanel = new GameObject("InteractionPrompt");
        promptPanel.transform.SetParent(canvas.transform, false);

        // Add image background
        Image background = promptPanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);

        // Add text
        GameObject textObj = new GameObject("PromptText");
        textObj.transform.SetParent(promptPanel.transform, false);
        promptText = textObj.AddComponent<Text>();
        promptText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        promptText.fontSize = 24;
        promptText.color = Color.white;
        promptText.alignment = TextAnchor.MiddleCenter;

        // Setup rect transforms
        RectTransform panelRect = promptPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(300, 60);
        panelRect.anchorMin = new Vector2(0.5f, 0.2f);
        panelRect.anchorMax = new Vector2(0.5f, 0.2f);
        panelRect.anchoredPosition = Vector2.zero;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(280, 40);
        textRect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Check if the interaction key is pressed
    /// </summary>
    public bool IsInteractionKeyPressed()
    {
        return Input.GetKeyDown(interactionKey);
    }
}
