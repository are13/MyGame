// ***********added**************
// Loads every DialogNode ScriptableObject from Assets/resources/dialog and keeps
// an id/name -> node registry. NPCs resolve their dialog through this registry so
// no scene wiring is required.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class DialogInitializer : MonoBehaviour
{

    public static String path = "dialog/";

    // ***********added**************
    static readonly Dictionary<string, DialogNode> nodes = new Dictionary<string, DialogNode>(StringComparer.OrdinalIgnoreCase);
    static bool isLoaded;

    public static void init()
    {
        if (isLoaded) return;
        isLoaded = true;
        nodes.Clear();

        string folder = path == null ? "dialog" : path.Trim('/');
        DialogNode[] loaded = Resources.LoadAll<DialogNode>(folder);
        foreach (DialogNode node in loaded)
        {
            if (node == null) continue;
            register(node.id, node);
            register(node.name, node);
        }
        Debug.Log($"[Dialog] {nodes.Count} dialog node(s) loaded from Resources/{folder}");
    }

    static void register(string key, DialogNode node)
    {
        if (string.IsNullOrEmpty(key) || node == null) return;
        nodes[key] = node;
    }

    public static DialogNode find(string key)
    {
        if (!isLoaded) init();
        if (string.IsNullOrEmpty(key)) return null;
        nodes.TryGetValue(key, out DialogNode node);
        return node;
    }

    public static void reload()
    {
        isLoaded = false;
        init();
    }

    // Hides the dialog panel as soon as a scene containing it is loaded and keeps a
    // controller alive for the life of that panel.
    // ***********added**************
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void bootstrap()
    {
        // Enter Play Mode Options may skip the domain reload, so clear the caches.
        isLoaded = false;
        DialogController.reset();
        DialogController.Ensure();
        SceneManager.sceneLoaded -= onSceneLoaded;
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    static void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DialogController.Ensure();
    }
    // ***********added**************
}
