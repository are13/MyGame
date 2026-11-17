// DialogueNodeView.cs
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNodeView : Node
{
    public DialogNode NodeData { get; private set; }
    public Port InputPort;
    public Port OutputPort;

    public DialogueNodeView(DialogNode nodeData)
    {
        NodeData = nodeData;
        title = nodeData.GetType().Name;

        // 1. 输入端口 (所有节点基本都有一个输入)
        InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        InputPort.portName = "In";
        inputContainer.Add(InputPort);

        // 2. 输出端口 (根据节点类型决定有无，以及几个)
        if (!(nodeData is EndNode)) // EndNode 没有输出
        {
            // 如果是 OptionNode，可能会有多个输出口，这里简化为一个，后续你可以根据 List 动态生成
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);
        }

        // 3. 节点内部的自定义 UI (显示部分数据)
        var customDataContainer = new VisualElement();
        if (nodeData is DialogText dialogText)
        {
            customDataContainer.Add(new Label($"Speaker: {dialogText.speaker}"));
            customDataContainer.Add(new Label($"Text: {dialogText.text}"));
        }
        
        extensionContainer.Add(customDataContainer);
        RefreshExpandedState();
    }
}