// DialogueGraphView.cs
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly DialogueGraphWindow editorWindow;
    private DialogContent currentGraph;

    // DialogNode 是普通 C# 类（不是 UnityEngine.Object），用引用相等来索引节点视图最可靠。
    private readonly Dictionary<DialogNode, DialogueNodeView> nodeViews = new Dictionary<DialogNode, DialogueNodeView>();

    // 加载时算出的显示坐标：不改动资源里保存的 position，只有用户真的拖动节点时才写回数据。
    private readonly Dictionary<DialogNode, Vector2> nodePositions = new Dictionary<DialogNode, Vector2>();

    private const float NodeWidth = 200f;
    private const float NodeHeight = 150f;
    private const float LayoutStepX = 280f;
    private const float LayoutStepY = 180f;

    // 自动布局的起点：往下让开顶部工具栏，否则 (0,0) 的节点会被工具栏挡住
    private static readonly Vector2 LayoutOrigin = new Vector2(40f, 60f);

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
        nodeViews.Clear();
        nodePositions.Clear();
        graphViewChanged += OnGraphViewChanged;

        if (currentGraph == null) return;

        // 关键修复：节点不一定都在 dialogs 列表里。
        // 对话文件里经常只保存了 start 以及 start 沿 next 串起来的一整条链，
        // 而 dialogs 仍然是空的（Keine.asset / Shelf.asset 就是这样）。
        // 只遍历 dialogs 会导致“文件能选中，但图上一片空白”。
        var nodes = CollectNodes(currentGraph);
        if (nodes.Count == 0) return;

        BuildDisplayPositions(nodes);

        // 1. 创建节点
        foreach (var node in nodes)
        {
            CreateNodeView(node);
        }

        // 2. 创建连线（next 是 DialogNode 的公共字段，所有节点类型都可能有）
        foreach (var node in nodes)
        {
            DrawEdgesForNode(node);
        }
    }

    /// <summary>
    /// 收集所有需要显示的节点：先取 dialogs 列表，再从 start 沿 next 把所有能到达的节点补齐。
    /// 用 visited 去重，顺便防止 next 形成环时死循环。
    /// </summary>
    private static List<DialogNode> CollectNodes(DialogContent graph)
    {
        var result = new List<DialogNode>();
        var visited = new HashSet<DialogNode>();
        var pending = new Stack<DialogNode>();

        if (graph.dialogs != null)
        {
            foreach (var node in graph.dialogs)
            {
                AddNode(node, result, visited, pending);
            }
        }

        AddNode(graph.start, result, visited, pending);

        while (pending.Count > 0)
        {
            var node = pending.Pop();
            AddNode(node.next, result, visited, pending);
        }

        return result;
    }

    private static void AddNode(DialogNode node, List<DialogNode> result, HashSet<DialogNode> visited, Stack<DialogNode> pending)
    {
        if (node == null) return;      // 列表里可能残留空元素
        if (!visited.Add(node)) return;

        result.Add(node);
        pending.Push(node);
    }

    /// <summary>
    /// 计算显示坐标。资源里新建/未布局的节点 position 往往是 (0,0)，
    /// 多个节点会完全重叠在同一个点上，而且 (0,0) 正好被顶部工具栏挡住，
    /// 看起来就像“没加载出来”。这里把默认坐标的节点依次排到空位上
    /// （只影响显示，不污染资源数据）。
    /// </summary>
    private void BuildDisplayPositions(List<DialogNode> nodes)
    {
        nodePositions.Clear();
        var used = new HashSet<Vector2>();

        foreach (var node in nodes)
        {
            var position = node.position;
            bool unassigned = position == Vector2.zero;

            if (!unassigned && used.Add(position))
            {
                nodePositions[node] = position;
                continue;
            }

            Vector2 placed = position;
            int cell = 0;
            while (cell < 4096)
            {
                var candidate = LayoutOrigin + new Vector2((cell % 4) * LayoutStepX, (cell / 4) * LayoutStepY);
                if (used.Add(candidate))
                {
                    placed = candidate;
                    break;
                }
                cell++;
            }

            nodePositions[node] = placed;
        }
    }

    private DialogueNodeView CreateNodeView(DialogNode node)
    {
        var nodeView = new DialogueNodeView(node);

        Vector2 position = nodePositions.TryGetValue(node, out var displayPosition)
            ? displayPosition
            : node.position;

        nodeView.SetPosition(new Rect(position, new Vector2(NodeWidth, NodeHeight)));

        nodeViews[node] = nodeView;
        AddElement(nodeView);
        return nodeView;
    }

    private void DrawEdgesForNode(DialogNode node)
    {
        if (node == null || node.next == null) return;
        if (ReferenceEquals(node, node.next)) return; // 自连会在运行时死循环，也不该画出来

        ConnectPorts(node, node.next);
    }

    private void ConnectPorts(DialogNode fromNode, DialogNode toNode)
    {
        if (fromNode == null || toNode == null) return;
        if (!nodeViews.TryGetValue(fromNode, out var fromView)) return;
        if (!nodeViews.TryGetValue(toNode, out var toView)) return;
        if (fromView.OutputPort == null || toView.InputPort == null) return;

        var edge = fromView.OutputPort.ConnectTo(toView.InputPort);
        if (edge != null) AddElement(edge);
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
        if (currentGraph == null || node == null) return;

        if (currentGraph.dialogs == null) currentGraph.dialogs = new List<DialogNode>();

        node.id = System.Guid.NewGuid().ToString();

        // 把正确的坐标赋值给节点数据
        node.position = spawnPosition;

        currentGraph.dialogs.Add(node);

        // 创建 UI 节点并应用位置
        nodePositions[node] = spawnPosition;
        CreateNodeView(node);

        // 第一个节点顺手挂到 start 上，否则保存后整张图没有入口
        if (currentGraph.start == null) currentGraph.start = node;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // 1. 拖动节点后把坐标写回数据，否则保存再打开位置就丢了
        if (graphViewChange.movedElements != null)
        {
            foreach (var element in graphViewChange.movedElements)
            {
                if (element is DialogueNodeView nodeView && nodeView.NodeData != null)
                {
                    nodeView.NodeData.position = nodeView.GetPosition().position;
                    nodePositions[nodeView.NodeData] = nodeView.NodeData.position;
                }
            }
        }

        // 2. 处理连线添加
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                var sourceNodeView = edge.output != null ? edge.output.node as DialogueNodeView : null;
                var targetNodeView = edge.input != null ? edge.input.node as DialogueNodeView : null;

                if (sourceNodeView != null && targetNodeView != null &&
                    sourceNodeView.NodeData != null && targetNodeView.NodeData != null)
                {
                    // next 是 DialogNode 的公共字段，DialogText / ActionNode / OptionNode 都适用
                    sourceNodeView.NodeData.next = targetNodeView.NodeData;
                }
            }
        }

        // 3. 处理删除（否则删掉的节点还留在资源里，下次加载又冒出来）
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (var element in graphViewChange.elementsToRemove)
            {
                RemoveDataFor(element);
            }
        }

        return graphViewChange;
    }

    private void RemoveDataFor(GraphElement element)
    {
        if (element is DialogueNodeView nodeView && nodeView.NodeData != null)
        {
            var data = nodeView.NodeData;
            nodeViews.Remove(data);
            nodePositions.Remove(data);

            if (currentGraph != null)
            {
                if (currentGraph.dialogs != null) currentGraph.dialogs.Remove(data);
                if (ReferenceEquals(currentGraph.start, data)) currentGraph.start = null;

                // 断开所有指向被删节点的 next，避免留下悬空引用
                foreach (var other in nodeViews.Keys)
                {
                    if (other != null && ReferenceEquals(other.next, data)) other.next = null;
                }
            }
        }
        else if (element is Edge edge)
        {
            var sourceNodeView = edge.output != null ? edge.output.node as DialogueNodeView : null;
            var targetNodeView = edge.input != null ? edge.input.node as DialogueNodeView : null;

            if (sourceNodeView != null && targetNodeView != null &&
                sourceNodeView.NodeData != null && targetNodeView.NodeData != null &&
                ReferenceEquals(sourceNodeView.NodeData.next, targetNodeView.NodeData))
            {
                sourceNodeView.NodeData.next = null;
            }
        }
    }
}
