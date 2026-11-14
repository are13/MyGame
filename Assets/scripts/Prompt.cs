
// ***********added**************
// The option list UI. It now owns the "nearby NPC" list and the "dialog choice"
// list, exposing addOption/resetSelection for DialogController. A template Option
// is resolved from optionPrefab/buttonPrefab or from a child of this prompt, with a
// runtime built fallback, so the system works even when the prefabs are not wired.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{

    // public GameObject optionPrefab;
    // public Option buttonPrefab;
    // ***********added**************
    public Option optionPrefab;
    // ***********added**************
    public Player player;
    // public List<Option> options;
    List<Option> options;
    int selected = 0;

    // ***********added**************
    Option template;
    readonly List<IInteractable> nearby = new List<IInteractable>();
    bool isInDialog;

    public int nearbyCount
    {
        get { return nearby.Count; }
    }

    void Awake()
    {

        options = new List<Option>();
        if (player == null) player = GetComponentInParent<Player>();
        resolveTemplate();
    }

    public void execute()
    {
        // **************changed*****************
        if (options == null || options.Count == 0) return;
        if (selected < 0 || selected >= options.Count) selected = 0;

        // **************changed*****************
        Player who = player != null ? player : UnityEngine.Object.FindFirstObjectByType<Player>();
        options[selected].onClick(who);
    }

    public void updateNearBy(List<IInteractable> interactables)
    {

        // ***********added**************
        nearby.Clear();
        if (interactables != null)
        {
            foreach (IInteractable interactable in interactables)
            {
                if (interactable != null) nearby.Add(interactable);
            }
        }

        // Dialog choices must survive trigger enter/exit noise.
        if (isInDialog) return;
        // ***********added**************

        removeAll();

        // **************changed*****************
        foreach (IInteractable target in nearby)
        {
            IInteractable interactable = target;
            addOption(labelOf(interactable), (p) => interactWith(interactable, p));
        }
        resetSelection();
    }

    // ***********added**************
    void interactWith(IInteractable interactable, Player who)
    {
        Player actual = who != null ? who : player;
        List<Option> result = interactable.interact(actual);

        // A DialogController took over the option list already.
        if (DialogController.isShowing) return;

        removeAll();
        if (result != null && result.Count > 0) updateDialogOption(result);
    }

    string labelOf(IInteractable interactable)
    {
        string label = interactable == null ? null : interactable.getId();
        // **************changed*****************
        // NPCs without an id fall back to their concrete type (e.g. "Keine") instead of
        // the child object name that actually owns the trigger collider.
        if (string.IsNullOrEmpty(label) && interactable != null) label = interactable.GetType().Name;
        if (string.IsNullOrEmpty(label) && interactable is Component component) label = component.name;
        if (string.IsNullOrEmpty(label)) label = "Talk";
        return label;
    }

    public Option addOption(string text, Action<Player> action)
    {
        if (options == null) options = new List<Option>();

        Option source = getTemplate();
        Option temp = Instantiate(source, transform, false);
        temp.gameObject.SetActive(true);
        temp.transform.localScale = Vector3.one;
        temp.init(action, text);
        options.Add(temp);
        return temp;
    }

    public void resetSelection()
    {
        if (options == null || options.Count == 0) return;
        selected = 0;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] != null) options[i].select(i == selected);
        }
    }

    void resolveTemplate()
    {
        if (template != null) return;

        if (optionPrefab != null)
        {
            template = optionPrefab;
            return;
        }

        // Placeholder options authored inside the prompt double as the template.
        for (int i = 0; i < transform.childCount; i++)
        {
            Option child = transform.GetChild(i).GetComponent<Option>();
            if (child == null) continue;
            child.gameObject.SetActive(false);
            if (template == null) template = child;
        }

        // Last prefab based fallback: the project's styled option prefab lives in Resources.
        if (template == null)
        {
            GameObject optionAsset = Resources.Load<GameObject>("sprite/prefabs/UI/option");
            if (optionAsset != null) template = optionAsset.GetComponent<Option>();
        }
        if (template == null) template = createRuntimeTemplate();
    }

    Option getTemplate()
    {
        if (template == null) resolveTemplate();
        return template;
    }

    Option createRuntimeTemplate()
    {
        GameObject root = new GameObject("optionTemplate", typeof(RectTransform), typeof(Option));
        root.transform.SetParent(transform, false);

        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(220f, 30f);

        Image background = root.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.35f);

        GameObject selectedObject = new GameObject("selected", typeof(RectTransform), typeof(Image));
        selectedObject.transform.SetParent(root.transform, false);
        RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();
        selectedRect.anchorMin = Vector2.zero;
        selectedRect.anchorMax = Vector2.one;
        selectedRect.offsetMin = Vector2.zero;
        selectedRect.offsetMax = Vector2.zero;
        Image selectedImage = selectedObject.GetComponent<Image>();
        selectedImage.color = new Color(1f, 0.85f, 0.3f, 0.45f);
        selectedImage.raycastTarget = false;

        GameObject textObject = new GameObject("text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(root.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(28f, 2f);
        textRect.offsetMax = new Vector2(-6f, -2f);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 20f;
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;
        text.raycastTarget = false;
        if (TMP_Settings.defaultFontAsset != null) text.font = TMP_Settings.defaultFontAsset;

        Option option = root.GetComponent<Option>();
        option.isSelected = selectedImage;
        option.text = text;
        root.SetActive(false);
        return option;
    }

    public void beginDialog()
    {
        isInDialog = true;
        removeAll();
    }

    public void endDialog()
    {
        isInDialog = false;
        removeAll();
        updateNearBy(new List<IInteractable>(nearby));
    }

    public void updateSelected(int dSelected)
    {
        if (options == null || options.Count == 0) return;
        int newSelected = Mathf.Clamp(selected + dSelected, 0, options.Count - 1);
        if (newSelected == selected) return;
        if (options[selected] != null) options[selected].select(false);
        if (options[newSelected] != null) options[newSelected].select(true);
        selected = newSelected;
    }

    public void updateDialogOption(List<Option> dialogOptions)
    {

        if (dialogOptions == null) return;
        if (options == null) options = new List<Option>();

        foreach (Option option in dialogOptions)
        {
            if (option == null) continue;

            Option temp = Instantiate(option, transform, false);
            temp.transform.localScale = Vector3.one;
            temp.gameObject.SetActive(true);
            temp.action = option.action;
            options.Add(temp);

            // print("aaaaaaaa");
        }
        resetSelection();
    }

    public void removeAll()
    {

        // **************changed*****************
        if (options == null) options = new List<Option>();
        foreach (Option option in options)
        {
            if (option == null) continue;
            option.transform.SetParent(null, false);
            Destroy(option.gameObject);
        }

        options.Clear();
        selected = 0;
    }
}
