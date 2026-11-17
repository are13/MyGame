// DialogueGraphView.cs
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    private readonly DialogueGraphWindow editorWindow;
    private DialogContent currentGraph;

    public DialogueGraphView(DialogueGraphWindow editorWindow)
    {
        this.editorWindow = editorWindow;

        // 基础设置：缩放、拖拽、框选
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // 添加网格背景 (可选，需要自定义 GridBackground)
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // 注册右键菜单
        RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenu);
        
        // 监听连线变化
        graphViewChanged += OnGraphViewChanged;

    }

    public void PopulateView(DialogContent graph)
    {
        currentGraph = graph;
        graphViewChanged -= OnGraphViewChanged; // 暂停监听避免重绘时触发
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (currentGraph == null) return;

        // 1. 创建节点
        foreach (var node in currentGraph.dialogs)
        {
            CreateNodeView(node);
        }

        // 2. 创建连线 (这里需要根据你的 next 和 target 字段来连)
        foreach (var node in currentGraph.dialogs)
        {
            DrawEdgesForNode(node);
        }
    }

    private void CreateNodeView(DialogNode node)
    {
        var nodeView = new DialogueNodeView(node);
        nodeView.SetPosition(new Rect(node.position, new Vector2(200, 150)));
        AddElement(nodeView);
    }

    private void DrawEdgesForNode(DialogNode node)
    {
        // 示例：如果是 DialogText 且 next 不为空，则连线
        if (node is DialogText textNode && textNode.next != null)
        {
            var sourceNodeView = GetNodeView(textNode);
            var targetNodeView = GetNodeView(textNode.next);
            if (sourceNodeView != null && targetNodeView != null)
            {
                var edge = sourceNodeView.OutputPort.ConnectTo(targetNodeView.InputPort);
                AddElement(edge);
            }
        }
        // OptionNode 的连线逻辑类似，遍历 options 列表...
    }

    private DialogueNodeView GetNodeView(DialogNode node)
    {
        return GetNodeByGuid(node.GetHashCode().ToString()) as DialogueNodeView; // 简化的查找
    }

private void OnContextualMenu(ContextualMenuPopulateEvent evt)
{
    if (currentGraph == null) return;

    // 1. 获取鼠标相对于 GraphView 的坐标
    Vector2 mousePos = evt.localMousePosition;
    
    // 2. 核心转换：将屏幕坐标转换为考虑了缩放/平移的“画布逻辑坐标”
    // contentViewContainer 是 GraphView 内部用于承载节点的容器
    Vector2 spawnPos = contentViewContainer.WorldToLocal(mousePos);

    // 3. 将算好的 spawnPos 传进去
    evt.menu.AppendAction("Create/Text Node", action => AddNewNode(new DialogText(), spawnPos));
    evt.menu.AppendAction("Create/Option Node", action => AddNewNode(new OptionNode(), spawnPos));
    evt.menu.AppendAction("Create/End Node", action => AddNewNode(new EndNode(), spawnPos));
}

    private void AddNewNode(DialogNode node, Vector2 spawnPosition)
{
    node.id = System.Guid.NewGuid().ToString();
    
    // 把正确的坐标赋值给节点数据
    node.position = spawnPosition; 
    
    currentGraph.dialogs.Add(node);
    
    // 创建 UI 节点并应用位置
    var nodeView = new DialogueNodeView(node);
    nodeView.SetPosition(new Rect(node.position, new Vector2(200, 150))); 
    AddElement(nodeView);
}

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // 处理连线添加
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                var sourceNodeView = edge.output.node as DialogueNodeView;
                var targetNodeView = edge.input.node as DialogueNodeView;
                
                if (sourceNodeView != null && targetNodeView != null)
                {
                    // 将连线数据写回 SO 数据结构
                    // 注意：这里如果有多输出口 (如 OptionNode)，需要根据 Port 的标识区分
                    if (sourceNodeView.NodeData is DialogText textNode)
                    {
                        textNode.next = targetNodeView.NodeData;
                    }
                }
            }
        }
        // 处理连线删除... (同理，将 target 置空)
        return graphViewChange;
    }
}