// DialogueGraphWindow.cs
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView graphView;
    private DialogContent currentGraph;

    [MenuItem("Tools/Dialogue Graph")]
    public static void OpenWindow()
    {
        var window = GetWindow<DialogueGraphWindow>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        toolbar.Add(new Button(() => SaveGraph()) { text = "Save" });
        toolbar.Add(new Button(() => LoadGraph(Selection.activeObject as DialogContent)) { text = "Load" });
        
        rootVisualElement.Add(toolbar);
    }
    
    public void LoadGraph(DialogContent graph)
    {
        currentGraph = graph;
        if (currentGraph != null)
        {
            graphView.PopulateView(currentGraph);
        }
    }

    private void SaveGraph()
    {
        if (currentGraph != null)
        {
            EditorUtility.SetDirty(currentGraph);
            AssetDatabase.SaveAssets();
        }
    }
}