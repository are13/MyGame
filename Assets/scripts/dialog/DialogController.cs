// ***********added**************
// Drives SO based dialog: shows name/text in the scene's "dialog" panel and pushes
// the current node's choices into the Player's Prompt, so the existing
// Prompt/Option keyboard interaction keeps driving everything.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// **************changed*****************
public class DialogController : MonoBehaviour
{

    // ***********added**************
    public static bool isShowing { get; private set; }

    static DialogController instance;

    public static DialogController Instance
    {
        get
        {
            if (instance == null) instance = findOrCreate();
            return instance;
        }
    }

    public static DialogController Ensure()
    {
        return Instance;
    }

    // ***********added**************
    public static void reset()
    {
        instance = null;
        isShowing = false;
    }
    // ***********added**************

    public float autoDelay = 1.5f;
    public string nextLabel = "Next";
    public string endLabel = "End";

    GameObject panel;
    CanvasGroup group;
    TextMeshProUGUI nameText;
    TextMeshProUGUI lineText;
    Image autoImage;
    Prompt prompt;
    Player player;
    NPCEntity speaker;
    DialogNode currentNode;
    bool isAuto;
    float autoTimer;
    bool isBound;

    static DialogController findOrCreate()
    {
        DialogInitializer.init();

        DialogController existing = Object.FindFirstObjectByType<DialogController>();
        if (existing != null)
        {
            existing.bind();
            return existing;
        }

        GameObject panelObject = findDialogPanel();
        if (panelObject == null)
        {
            Debug.LogWarning("[Dialog] No 'dialog' panel with 'name'/'text' children found; dialog UI disabled.");
            return null;
        }

        if (!panelObject.activeSelf) panelObject.SetActive(true);
        DialogController controller = panelObject.GetComponent<DialogController>();
        if (controller == null) controller = panelObject.AddComponent<DialogController>();
        controller.bind();
        return controller;
    }

    static GameObject findDialogPanel()
    {
        GameObject direct = GameObject.Find("dialog");
        if (isDialogPanel(direct)) return direct;

        Transform[] all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform candidate in all)
        {
            if (candidate == null) continue;
            if (!candidate.name.Equals("dialog", System.StringComparison.OrdinalIgnoreCase)) continue;
            if (isDialogPanel(candidate.gameObject)) return candidate.gameObject;
        }
        return null;
    }

    static bool isDialogPanel(GameObject candidate)
    {
        if (candidate == null) return false;
        return candidate.transform.Find("text") != null && candidate.transform.Find("name") != null;
    }

    void bind()
    {
        if (isBound) return;
        isBound = true;

        panel = gameObject;
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();

        nameText = findText("name");
        lineText = findText("text");
        autoImage = findImage("auto");

        addButton("next", next);
        addButton("skip", skip);
        addButton("auto", toggleAuto);

        if (!isShowing) hide();
    }

    TextMeshProUGUI findText(string childName)
    {
        Transform child = transform.Find(childName);
        return child == null ? null : child.GetComponent<TextMeshProUGUI>();
    }

    Image findImage(string childName)
    {
        Transform child = transform.Find(childName);
        return child == null ? null : child.GetComponent<Image>();
    }

    void addButton(string childName, UnityAction action)
    {
        Transform child = transform.Find(childName);
        if (child == null) return;

        Button button = child.GetComponent<Button>();
        if (button == null)
        {
            button = child.gameObject.AddComponent<Button>();
            button.targetGraphic = child.GetComponent<Graphic>();
        }
        button.onClick.AddListener(action);
    }

    void show()
    {
        if (panel != null && !panel.activeSelf) panel.SetActive(true);
        if (group != null)
        {
            group.alpha = 1f;
            group.blocksRaycasts = true;
            group.interactable = true;
        }
    }

    void hide()
    {
        if (group != null)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }
    }

    public bool startDialog(Player player, DialogNode node, NPCEntity speaker = null)
    {
        if (node == null) return false;

        if (player != null) this.player = player;
        this.speaker = speaker;

        Prompt owner = resolvePrompt(this.player);
        if (owner == null)
        {
            Debug.LogWarning("[Dialog] The player has no Prompt component; dialog cannot run.");
            return false;
        }

        prompt = owner;
        isAuto = false;
        if (autoImage != null) autoImage.color = Color.white;
        isShowing = true;
        show();
        prompt.beginDialog();
        goTo(node);
        return true;
    }

    Prompt resolvePrompt(Player owner)
    {
        if (owner == null) return prompt;
        if (owner.prompt != null) return owner.prompt;

        Prompt child = owner.GetComponentInChildren<Prompt>(true);
        if (child != null) return child;

        return Object.FindFirstObjectByType<Prompt>();
    }

    public void goTo(DialogNode node)
    {
        if (node == null)
        {
            endDialog();
            return;
        }

        currentNode = node;
        if (nameText != null) nameText.text = speakerName(node);
        if (lineText != null) lineText.text = node.text == null ? string.Empty : node.text;

        executeActions(node.actions);
        buildOptions();
        autoTimer = Mathf.Max(0.1f, autoDelay);
    }

    void buildOptions()
    {
        if (prompt == null || currentNode == null) return;

        DialogNode node = currentNode;
        prompt.removeAll();

        bool hasChoices = node.options != null && node.options.Count > 0;
        if (hasChoices)
        {
            foreach (DialogOptionData data in node.options)
            {
                if (data == null) continue;
                DialogOptionData captured = data;
                string label = string.IsNullOrEmpty(data.text) ? "..." : data.text;
                prompt.addOption(label, (p) => choose(captured));
            }
        }
        else if (node.next != null)
        {
            DialogNode nextNode = node.next;
            string label = string.IsNullOrEmpty(nextLabel) ? "Next" : nextLabel;
            prompt.addOption(label, (p) => goTo(nextNode));
        }
        else
        {
            string label = string.IsNullOrEmpty(endLabel) ? "End" : endLabel;
            prompt.addOption(label, (p) => endDialog());
        }

        prompt.resetSelection();
    }

    public void choose(DialogOptionData option)
    {
        if (!isShowing || option == null) return;

        executeActions(option.actions);

        if (option.endDialog)
        {
            endDialog();
            return;
        }

        if (option.next != null) goTo(option.next);
        else endDialog();
    }

    public void next()
    {
        if (!isShowing || prompt == null) return;
        if (hasChoices(currentNode)) return;
        prompt.execute();
    }

    public void skip()
    {
        if (!isShowing) return;
        endDialog();
    }

    public void toggleAuto()
    {
        if (!isShowing) return;

        isAuto = !isAuto;
        autoTimer = Mathf.Max(0.1f, autoDelay);
        if (autoImage != null) autoImage.color = isAuto ? new Color(0.4f, 1f, 0.4f) : Color.white;
    }

    public void endDialog()
    {
        isShowing = false;
        currentNode = null;
        isAuto = false;
        if (autoImage != null) autoImage.color = Color.white;

        hide();
        if (prompt != null) prompt.endDialog();
    }

    bool hasChoices(DialogNode node)
    {
        return node != null && node.options != null && node.options.Count > 0;
    }

    void executeActions(List<DialogActionData> actions)
    {
        if (actions == null) return;
        foreach (DialogActionData action in actions)
        {
            if (action == null) continue;
            action.execute(player);
        }
    }

    string speakerName(DialogNode node)
    {
        if (node != null && !string.IsNullOrEmpty(node.characterName)) return node.characterName;
        if (speaker == null) return string.Empty;

        string id = speaker.getId();
        if (!string.IsNullOrEmpty(id)) return id;

        string typeName = speaker.GetType().Name;
        if (!string.IsNullOrEmpty(typeName)) return typeName;

        return speaker.name;
    }

    void Update()
    {
        if (!isShowing) return;

        if (player == null)
        {
            endDialog();
            return;
        }

        // Player.Update only feeds the prompt while an NPC is nearby. If something
        // pushes the player out mid dialog, keep the choices usable from here.
        if (prompt != null && prompt.nearbyCount == 0 && Keyboard.current != null)
        {
            if (Keyboard.current.uKey.wasPressedThisFrame) prompt.updateSelected(-1);
            if (Keyboard.current.jKey.wasPressedThisFrame) prompt.updateSelected(1);
            if (Keyboard.current.bKey.wasPressedThisFrame) prompt.execute();
        }

        if (isAuto && !hasChoices(currentNode))
        {
            autoTimer -= Time.unscaledDeltaTime;
            if (autoTimer <= 0f)
            {
                autoTimer = Mathf.Max(0.1f, autoDelay);
                next();
            }
        }
    }

    void LateUpdate()
    {
        if (!isShowing || prompt == null) return;
        if (!prompt.gameObject.activeSelf) prompt.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            isShowing = false;
        }
    }
    // ***********added**************
}
