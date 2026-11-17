using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]
public sealed class SerializeReferenceSelectorDrawer : PropertyDrawer
{
    private SerializeReferenceSelectorAttribute Attr => (SerializeReferenceSelectorAttribute)attribute;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        // 特性打在 List/Array 字段上时，Unity 默认把抽屉分发到"每个元素"上：
        // 此时 property 是元素本身，fieldInfo 仍是 List 字段，所以先把元素类型解出来。
        Type baseType = GetElementType(fieldInfo?.FieldType);
        if (baseType == null)
        {
            root.Add(new HelpBox($"无法解析 {property.displayName} 的字段类型。", HelpBoxMessageType.Error));
            return root;
        }

        // 只有极少数情况（自定义 Editor 直接绘制集合）才会走进 BuildList。
        if (property.isArray && property.propertyType != SerializedPropertyType.String)
        {
            BuildList(root, property, baseType);
        }
        else
        {
            // 列表元素只用"类名"选择（新建/换类型）；单字段才额外列出宿主对象里已有的实例。
            BuildSingle(root, property, baseType, allowExistingInstances: !IsArrayElement(property));
        }

        return root;
    }

    // ==================== List / 数组 ====================
    private void BuildList(VisualElement container, SerializedProperty property, Type elementType)
    {
        container.Clear();

        if (elementType == null)
        {
            container.Add(new HelpBox($"无法解析 {property.displayName} 的元素类型。", HelpBoxMessageType.Error));
            return;
        }

        var header = new Label(property.displayName)
        {
            style = { unityFontStyleAndWeight = FontStyle.Bold, marginBottom = 2 }
        };
        container.Add(header);

        var listRoot = new VisualElement();
        container.Add(listRoot);

        void Rebuild()
        {
            listRoot.Clear();

            for (int i = 0; i < property.arraySize; i++)
            {
                int index = i;
                var element = property.GetArrayElementAtIndex(index);

                var row = new VisualElement
                {
                    style = { flexDirection = FlexDirection.Row, marginBottom = 2 }
                };

                var body = new VisualElement { style = { flexGrow = 1 } };
                // 列表元素不允许指向别处已有的实例，否则两个元素会变成同一个对象。
                BuildSingle(body, element, elementType, allowExistingInstances: false);
                row.Add(body);

                var removeBtn = new Button(() =>
                {
                    int sizeBefore = property.arraySize;
                    property.DeleteArrayElementAtIndex(index);
                    if (property.arraySize == sizeBefore && index < property.arraySize)
                        property.DeleteArrayElementAtIndex(index);

                    property.serializedObject.ApplyModifiedProperties();
                    Rebuild();
                })
                { text = "✕", style = { width = 24, marginLeft = 2 } };
                row.Add(removeBtn);

                listRoot.Add(row);
            }

            var addBtn = new Button(() =>
            {
                property.InsertArrayElementAtIndex(property.arraySize);
                var newElement = property.GetArrayElementAtIndex(property.arraySize - 1);
                newElement.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
                Rebuild();
            })
            { text = "＋ 添加元素" };
            listRoot.Add(addBtn);
        }

        Rebuild();
    }

    // ==================== 单字段 ====================
    private void BuildSingle(VisualElement container, SerializedProperty property, Type baseType, bool allowExistingInstances)
    {
        container.Clear();

        if (baseType == null || baseType == typeof(object))
        {
            container.Add(new HelpBox("无法确定基类类型。", HelpBoxMessageType.Error));
            return;
        }

        var concreteTypes = GetConcreteTypes(baseType);
        if (concreteTypes.Count == 0)
        {
            container.Add(new HelpBox($"类型 {baseType.Name} 没有可实例化的非抽象子类。", HelpBoxMessageType.Warning));
            return;
        }

        var entries = BuildEntries(property, baseType, concreteTypes,
            listExistingInstances: allowExistingInstances && Attr.ShowExistingInstances);
        var choices = Enumerable.Range(0, entries.Count).ToList();

        var current = property.managedReferenceValue;
        int currentIndex = entries.FindIndex(e => e.kind == EntryKind.Existing && ReferenceEquals(e.instance, current));
        // 当前值是"自己新建的实例"时，选中同类型的条目即可（显示类名），不要在界面上标成异常状态。
        if (currentIndex < 0 && current != null)
            currentIndex = entries.FindIndex(e => e.kind == EntryKind.New && e.newType == current.GetType());
        if (currentIndex < 0) currentIndex = entries.FindIndex(e => e.kind == EntryKind.Keep);
        if (currentIndex < 0) currentIndex = entries.FindIndex(e => e.kind == EntryKind.Null);
        if (currentIndex < 0) currentIndex = 0;

        var row = new VisualElement
        {
            style = { flexDirection = FlexDirection.Row, marginBottom = 2 }
        };

        row.Add(new Label(property.displayName)
        {
            style = { minWidth = 100, unityTextAlign = TextAnchor.MiddleLeft }
        });

        var popup = new PopupField<int>(choices, currentIndex,
            i => entries[i].label,
            i => entries[i].label);
        popup.style.flexGrow = 1;

        popup.RegisterValueChangedCallback(evt =>
        {
            var entry = entries[evt.newValue];

            switch (entry.kind)
            {
                case EntryKind.Null:
                    property.managedReferenceValue = null;
                    break;

                case EntryKind.Existing:
                    // 关键：指向宿主对象里已有的那个实例（共享引用），而不是新建副本。
                    // 这样 start / next 才能链接到 dialogs 里真正配置过数据的节点。
                    property.managedReferenceValue = entry.instance;
                    break;

                case EntryKind.Keep:
                    return;   // 当前值保持不动

                case EntryKind.New:
                    if (property.managedReferenceValue != null &&
                        property.managedReferenceValue.GetType() == entry.newType)
                        return;   // 同类型不重建，避免丢掉已经填好的数据

                    try
                    {
                        property.managedReferenceValue = Activator.CreateInstance(entry.newType);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[SerializeReferenceSelector] 无法创建 {entry.newType.Name} 实例：{e.Message}");
                        return;
                    }
                    break;
            }

            property.serializedObject.ApplyModifiedProperties();
            BuildSingle(container, property, baseType, allowExistingInstances);
        });

        row.Add(popup);
        container.Add(row);

        // 子字段（用默认 PropertyField 递归绘制）
        if (property.managedReferenceValue != null)
        {
            var childField = new PropertyField(property);
            childField.Bind(property.serializedObject);
            container.Add(childField);
        }
    }

    // ==================== 下拉项 ====================
    private List<Entry> BuildEntries(SerializedProperty property, Type baseType, List<Type> concreteTypes, bool listExistingInstances)
    {
        var current = property.managedReferenceValue;
        var currentType = current?.GetType();
        var entries = new List<Entry>();

        if (Attr.ShowNullOption)
            entries.Add(new Entry(EntryKind.Null, "(None)"));

        if (listExistingInstances)
        {
            bool currentIsExisting = false;
            foreach (var existing in CollectExistingInstances(property, baseType, Attr.LabelField))
            {
                if (ReferenceEquals(existing.instance, current)) currentIsExisting = true;
                entries.Add(new Entry(EntryKind.Existing, existing.label, existing.instance));
            }

            foreach (var type in concreteTypes)
                entries.Add(new Entry(EntryKind.New, type.Name, newType: type));

            // 新建出来的实例本来就不在 dialogs 里，这是正常状态，不再加"不在列表中"的标签。
            // 只有当前类型连可实例化子类都不是（比如类型被删了）才补一条占位项，保证下拉能显示当前类型。
            if (current != null && !currentIsExisting && !concreteTypes.Contains(currentType))
                entries.Add(new Entry(EntryKind.Keep, currentType.Name, current));
        }
        else
        {
            // 列表元素：沿用原来的显示方式 —— 下拉项就是子类类名
            foreach (var type in concreteTypes)
            {
                entries.Add(type == currentType
                    ? new Entry(EntryKind.Keep, type.Name, current)
                    : new Entry(EntryKind.New, type.Name, newType: type));
            }

            if (current != null && currentType != null && !concreteTypes.Contains(currentType))
                entries.Add(new Entry(EntryKind.Keep, currentType.Name, current));
        }

        return entries;
    }

    // ==================== 收集"已有实例" ====================
    private static List<ExistingInstance> CollectExistingInstances(SerializedProperty property, Type baseType, string labelField)
    {
        var result = new List<ExistingInstance>();

        var serializedObject = property.serializedObject;
        if (serializedObject == null || serializedObject.targetObject == null)
            return result;

        // 当前字段所在的数组（列表元素场景），跳过它自己
        string containingArrayPath = GetContainingArrayPath(property.propertyPath);
        // 当前字段所属的列表元素实例（例如某个节点的 next），不允许把节点链接到自己
        object ownerInstance = GetOwnerInstance(property);

        var iterator = serializedObject.GetIterator();
        while (iterator.Next(true))
        {
            if (!iterator.isArray || iterator.propertyType == SerializedPropertyType.String) continue;
            if (iterator.propertyPath == property.propertyPath) continue;
            if (containingArrayPath != null && iterator.propertyPath == containingArrayPath) continue;

            int size = iterator.arraySize;
            for (int i = 0; i < size; i++)
            {
                var element = iterator.GetArrayElementAtIndex(i);
                if (element.propertyType != SerializedPropertyType.ManagedReference) continue;

                var instance = element.managedReferenceValue;
                if (instance == null) continue;
                if (!baseType.IsInstanceOfType(instance)) continue;
                if (ownerInstance != null && ReferenceEquals(instance, ownerInstance)) continue;
                if (result.Any(existing => ReferenceEquals(existing.instance, instance))) continue;

                result.Add(new ExistingInstance(instance, BuildLabel(element, instance, labelField)));
            }
        }

        return result;
    }

    private static string BuildLabel(SerializedProperty element, object instance, string labelField)
    {
        string typeName = instance.GetType().Name;
        string value = ReadLabelValue(element, instance, labelField);
        return string.IsNullOrEmpty(value) ? $"{typeName}（无 {labelField}）" : $"{value} — {typeName}";
    }

    private static string ReadLabelValue(SerializedProperty element, object instance, string labelField)
    {
        if (string.IsNullOrEmpty(labelField)) return null;

        var labelProperty = element.FindPropertyRelative(labelField);
        if (labelProperty != null && labelProperty.propertyType == SerializedPropertyType.String)
            return labelProperty.stringValue;

        var field = instance.GetType().GetField(labelField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return field != null ? field.GetValue(instance) as string : null;
    }

    // ==================== 工具 ====================
    /// <summary>当前属性是否正好是某个 List/Array 的元素（Unity 会把特性分发到元素上）。</summary>
    private static bool IsArrayElement(SerializedProperty property)
    {
        string path = property.propertyPath;
        if (!path.EndsWith("]", StringComparison.Ordinal)) return false;

        int marker = path.LastIndexOf(".Array.data[", StringComparison.Ordinal);
        if (marker < 0) return false;

        return path.IndexOf(']', marker) == path.Length - 1;
    }

    private static string GetContainingArrayPath(string path)
    {
        int index = path.LastIndexOf(".Array.data[", StringComparison.Ordinal);
        return index < 0 ? null : path.Substring(0, index);
    }

    private static object GetOwnerInstance(SerializedProperty property)
    {
        var serializedObject = property.serializedObject;
        var parts = property.propertyPath.Split('.');

        for (int i = parts.Length - 1; i > 0; i--)
        {
            var candidate = serializedObject.FindProperty(string.Join(".", parts, 0, i));
            if (candidate != null && candidate.propertyType == SerializedPropertyType.ManagedReference)
                return candidate.managedReferenceValue;
        }

        return null;
    }

    private static List<Type> GetConcreteTypes(Type baseType)
    {
        return TypeCache.GetTypesDerivedFrom(baseType)
            .Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition)
            .OrderBy(t => t.Name)
            .ToList();
    }

    private static Type GetElementType(Type fieldType)
    {
        if (fieldType == null) return null;
        if (fieldType.IsArray) return fieldType.GetElementType();
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            return fieldType.GetGenericArguments()[0];
        return fieldType;
    }

    private enum EntryKind { Null, Existing, New, Keep }

    private readonly struct Entry
    {
        public readonly EntryKind kind;
        public readonly string label;
        public readonly object instance;
        public readonly Type newType;

        public Entry(EntryKind kind, string label, object instance = null, Type newType = null)
        {
            this.kind = kind;
            this.label = label;
            this.instance = instance;
            this.newType = newType;
        }
    }

    private readonly struct ExistingInstance
    {
        public readonly object instance;
        public readonly string label;

        public ExistingInstance(object instance, string label)
        {
            this.instance = instance;
            this.label = label;
        }
    }
}