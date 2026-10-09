using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Editor tool that builds the styled Main Menu and In-Game (Pause) Menu.
// Menu: Tools > Three-in-One
// Each build replaces the old menu canvas in the scene and wires the buttons automatically.
public static class MenuBuilder
{
    // ---- Texts (edit these) ----
    const string GameTitle = "GAME COLLISION";
    const string Subtitle = "3 GAMES  |  1 MENU";
    const string Author = "By Mohammad Rahemi";

    const string MainMenuScene = "Assets/Scenes/MainMenu.unity";
    static readonly string[] GameScenes =
    {
        "Assets/Scenes/Prototype 1.unity",
        "Assets/Scenes/Challenge 1.unity",
        "Assets/Scenes/Prototype 4.unity",
    };

    // ---- Palette ----
    static readonly Color BackgroundColor = Hex("14171F");
    static readonly Color CardColor = Hex("1F2430", 0.94f);
    static readonly Color DimColor = Hex("080A10", 0.45f);
    static readonly Color ButtonColor = Hex("2A3142");
    static readonly Color AccentColor = Hex("4CC9F0");
    static readonly Color AccentPressedColor = Hex("1B8DB0");
    static readonly Color ExitColor = Hex("5A2A2A");
    static readonly Color ExitHoverColor = Hex("E05555");
    static readonly Color ExitPressedColor = Hex("A03C3C");
    static readonly Color SoftTextColor = Hex("9AA3B5");

    const float ButtonWidth = 520f;
    const float ButtonHeight = 80f;
    const float ButtonSpacing = 22f;

    static readonly Vector2 Center = new Vector2(0.5f, 0.5f);
    static readonly Vector2 TopCenter = new Vector2(0.5f, 1f);
    static readonly Vector2 BottomRight = new Vector2(1f, 0f);

    [MenuItem("Tools/Three-in-One/Build Main Menu (open scene)")]
    static void BuildMainMenuInOpenScene()
    {
        Undo.SetCurrentGroupName("Build Main Menu");
        int group = Undo.GetCurrentGroup();
        BuildMainMenu();
        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Three-in-One/Build Pause Menu (open scene)")]
    static void BuildPauseMenuInOpenScene()
    {
        Undo.SetCurrentGroupName("Build Pause Menu");
        int group = Undo.GetCurrentGroup();
        BuildPauseMenu();
        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Three-in-One/Rebuild All Menus (all 4 scenes)")]
    static void RebuildAll()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }
        string startScene = SceneManager.GetActiveScene().path;

        EditorSceneManager.OpenScene(MainMenuScene);
        BuildMainMenu();
        EditorSceneManager.SaveOpenScenes();

        foreach (string path in GameScenes)
        {
            EditorSceneManager.OpenScene(path);
            BuildPauseMenu();
            EditorSceneManager.SaveOpenScenes();
        }

        if (!string.IsNullOrEmpty(startScene))
        {
            EditorSceneManager.OpenScene(startScene);
        }
        Debug.Log("Three-in-One: rebuilt the Main Menu and the Pause Menu in all 3 games.");
    }

    // ------------------------------------------------------------------ Main Menu

    static void BuildMainMenu()
    {
        RemoveOldCanvases<MainMenu>();

        Canvas canvas = CreateCanvas("MainMenuCanvas", 0);
        MainMenu menu = canvas.gameObject.AddComponent<MainMenu>();

        Image background = CreateImage("Background", canvas.transform, BackgroundColor, false);
        Stretch(background.rectTransform);

        TextMeshProUGUI title = CreateText("Title", canvas.transform, GameTitle, 110, Color.white, FontStyles.Bold);
        title.characterSpacing = 8;
        Place(title.rectTransform, TopCenter, Center, new Vector2(0, -190), new Vector2(1600, 160));

        Image line = CreateImage("AccentLine", canvas.transform, AccentColor, false);
        Place(line.rectTransform, TopCenter, Center, new Vector2(0, -290), new Vector2(500, 6));

        TextMeshProUGUI subtitle = CreateText("Subtitle", canvas.transform, Subtitle, 34, AccentColor, FontStyles.Normal);
        subtitle.characterSpacing = 15;
        Place(subtitle.rectTransform, TopCenter, Center, new Vector2(0, -340), new Vector2(1000, 60));

        RectTransform buttons = CreateButtonGroup(canvas.transform, new Vector2(0, -120), 4);
        AddButton(buttons, "DriveButton", "Mad Driver", menu.PlayDriving, false);
        AddButton(buttons, "FlyButton", "Fly Like a Bird", menu.PlayFlying, false);
        AddButton(buttons, "SumoButton", "I'm a Sumo and a Ball", menu.PlaySumo, false);
        AddButton(buttons, "ExitButton", "Exit", menu.ExitGame, true);

        TextMeshProUGUI author = CreateText("Author", canvas.transform, Author, 28, SoftTextColor, FontStyles.Normal);
        author.alignment = TextAlignmentOptions.Right;
        Place(author.rectTransform, BottomRight, BottomRight, new Vector2(-40, 40), new Vector2(700, 50));

        Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Build Main Menu");
        EnsureEventSystem();
        Selection.activeGameObject = canvas.gameObject;
    }

    // ------------------------------------------------------------------ Pause Menu

    static void BuildPauseMenu()
    {
        RemoveOldCanvases<PauseMenu>();

        Canvas canvas = CreateCanvas("PauseCanvas", 10);
        PauseMenu pause = canvas.gameObject.AddComponent<PauseMenu>();

        // Full-screen tint over the blurred game; also blocks clicks to anything behind it
        Image panel = CreateImage("PausePanel", canvas.transform, DimColor, false);
        panel.raycastTarget = true;
        Stretch(panel.rectTransform);

        Image card = CreateImage("Card", panel.transform, CardColor, true);
        Place(card.rectTransform, Center, Center, Vector2.zero, new Vector2(640, 620));

        TextMeshProUGUI pausedText = CreateText("PausedText", card.transform, "PAUSED", 96, Color.white, FontStyles.Bold);
        pausedText.characterSpacing = 14;
        Place(pausedText.rectTransform, TopCenter, Center, new Vector2(0, -95), new Vector2(600, 120));

        Image line = CreateImage("AccentLine", card.transform, AccentColor, false);
        Place(line.rectTransform, TopCenter, Center, new Vector2(0, -170), new Vector2(220, 5));

        RectTransform buttons = CreateButtonGroup(card.transform, new Vector2(0, -60), 3);
        AddButton(buttons, "ResumeButton", "Resume", pause.Resume, false);
        AddButton(buttons, "RestartButton", "Restart", pause.Restart, false);
        AddButton(buttons, "MainMenuButton", "Back to Main Menu", pause.BackToMainMenu, false);

        pause.pausePanel = panel.gameObject;

        Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Build Pause Menu");
        EnsureEventSystem();
        Selection.activeGameObject = canvas.gameObject;
    }

    // ------------------------------------------------------------------ Helpers

    // Deletes the whole canvas that holds the old menu script, so rebuilding never duplicates menus
    static void RemoveOldCanvases<T>() where T : Component
    {
        Scene scene = SceneManager.GetActiveScene();
        foreach (T old in Resources.FindObjectsOfTypeAll<T>())
        {
            if (old == null || EditorUtility.IsPersistent(old) || old.gameObject.scene != scene)
            {
                continue;
            }
            Canvas parentCanvas = old.GetComponentInParent<Canvas>(true);
            GameObject root = parentCanvas != null ? parentCanvas.rootCanvas.gameObject : old.gameObject;
            Undo.DestroyObjectImmediate(root);
        }
    }

    static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }
        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
    }

    static Canvas CreateCanvas(string name, int sortingOrder)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go;
    }

    static Image CreateImage(string name, Transform parent, Color color, bool rounded)
    {
        Image image = NewUI(name, parent).AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        if (rounded)
        {
            image.sprite = RoundedSprite();
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 0.6f;
        }
        return image;
    }

    static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, Color color, FontStyles style)
    {
        TextMeshProUGUI tmp = NewUI(name, parent).AddComponent<TextMeshProUGUI>();
        if (tmp.font == null)
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.raycastTarget = false;
        return tmp;
    }

    static RectTransform CreateButtonGroup(Transform parent, Vector2 position, int buttonCount)
    {
        RectTransform rect = NewUI("ButtonGroup", parent).GetComponent<RectTransform>();
        float height = buttonCount * ButtonHeight + (buttonCount - 1) * ButtonSpacing;
        Place(rect, Center, Center, position, new Vector2(ButtonWidth, height));

        VerticalLayoutGroup layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = ButtonSpacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return rect;
    }

    static void AddButton(RectTransform group, string name, string label, UnityAction onClick, bool isExit)
    {
        GameObject go = NewUI(name, group);

        // Image stays white: the Button's colour tint multiplies with it
        Image image = go.AddComponent<Image>();
        image.sprite = RoundedSprite();
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 0.6f;
        image.color = Color.white;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = isExit ? ExitColor : ButtonColor;
        colors.highlightedColor = isExit ? ExitHoverColor : AccentColor;
        colors.pressedColor = isExit ? ExitPressedColor : AccentPressedColor;
        colors.selectedColor = colors.normalColor; // stops a clicked button staying lit
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        go.AddComponent<LayoutElement>().preferredHeight = ButtonHeight;
        go.AddComponent<MenuButtonFX>();

        UnityEventTools.AddPersistentListener(button.onClick, onClick);

        TextMeshProUGUI text = CreateText("Label", go.transform, label, 38, Color.white, FontStyles.Bold);
        Stretch(text.rectTransform);
    }

    static void Place(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    static Sprite RoundedSprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    static Color Hex(string hex, float alpha = 1f)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color color);
        color.a = alpha;
        return color;
    }
}
