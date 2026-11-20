// DialogueNodeView.cs
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueNodeView : Node
{
    public DialogNode NodeData { get; private set; }
    public Port InputPort;
    public Port OutputPort;

    // 节点内部字段被编辑后通知 GraphView：标记资源 dirty，Save 按钮/Ctrl+S 才会把改动写盘
    private readonly Action onDataChanged;

    public DialogueNodeView(DialogNode nodeData)
        : this(nodeData, null)
    {
    }

    public DialogueNodeView(DialogNode nodeData, Action onDataChanged)
    {
        NodeData = nodeData;
        this.onDataChanged = onDataChanged;

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

        // 3. 节点内部的自定义 UI：直接编辑节点数据，而不是只显示只读 Label
        var customDataContainer = new VisualElement();
        customDataContainer.style.minWidth = 180;

        if (nodeData is DialogText dialogText)
        {
            var speakerField = new TextField("Speaker")
            {
                value = dialogText.speaker ?? string.Empty
            };
            speakerField.RegisterValueChangedCallback(evt => ApplySpeaker(dialogText, evt.newValue));
            customDataContainer.Add(speakerField);

            var textField = new TextField("Text")
            {
                value = dialogText.text ?? string.Empty,
                multiline = true
            };
            // 多行输入需要 Normal 才会自动换行，再限制高度避免节点被撑得过大
            textField.style.whiteSpace = WhiteSpace.Normal;
            textField.style.height = 70;
            textField.verticalScrollerVisibility = ScrollerVisibility.Auto;
            textField.RegisterValueChangedCallback(evt => ApplyText(dialogText, evt.newValue));
            customDataContainer.Add(textField);
        }

        extensionContainer.Add(customDataContainer);
        RefreshExpandedState();

        // 必须刷新端口布局，否则拖出来的线会因为端口没有正确初始化而连不上
        RefreshPorts();
    }

    private void ApplySpeaker(DialogText node, string value)
    {
        value ??= string.Empty;
        if (node.speaker == value) return;

        node.speaker = value;
        onDataChanged?.Invoke();
    }

    private void ApplyText(DialogText node, string value)
    {
        value ??= string.Empty;
        if (node.text == value) return;

        node.text = value;
        onDataChanged?.Invoke();
    }
}
