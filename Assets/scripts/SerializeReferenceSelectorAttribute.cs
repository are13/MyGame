// SerializeReferenceSelectorAttribute.cs
using System;
using UnityEngine;

/// <summary>
/// 让标记了 [SerializeReference] 的字段在 Inspector 中显示子类型选择下拉框。
/// 必须配合 SerializeReferenceSelectorDrawer（Editor 程序集）一起使用。
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class SerializeReferenceSelectorAttribute : PropertyAttribute
{
    /// <summary>下拉框中是否显示 "(None)" 选项用于把字段置空。默认 true。</summary>
    public bool ShowNullOption { get; set; } = true;

    /// <summary>
    /// 单字段的下拉框是否额外列出"宿主对象上已有的同类型实例"（例如 DialogContent.dialogs 里的节点）。
    /// 选中后字段会直接指向那个实例（共享引用），而不是新建一个副本。
    /// 列表/数组本身以及列表元素不受影响，仍然只会新建实例。默认 true。
    /// </summary>
    public bool ShowExistingInstances { get; set; } = true;

    /// <summary>
    /// 给"已有实例"做标签时读取的字段名，显示成 "值 — 类型名"。默认 "id"。
    /// </summary>
    public string LabelField { get; set; } = "id";
}