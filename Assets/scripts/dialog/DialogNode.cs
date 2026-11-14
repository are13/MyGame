// ***********added**************
// The dialog data model. Every line of dialog / every branch is a ScriptableObject
// stored under Assets/resources/dialog so DialogInitializer can load it by id/name.

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogNode", menuName = "Game/Dialog/Dialog Node")]
public class DialogNode : ScriptableObject
{

    [Header("Identity")]
    [Tooltip("Optional key. When empty the asset name is used, which is also what NPCs look up by default.")]
    public string id;

    [Header("Content")]
    [Tooltip("Speaker name shown in the dialog box. Falls back to the NPC name when empty.")]
    public string characterName;
    [TextArea(2, 6)]
    public string text;

    [Header("Flow")]
    [Tooltip("Actions executed once when this node is entered.")]
    public List<DialogActionData> actions = new List<DialogActionData>();

    [Tooltip("Choices offered while this node is displayed. When empty a single Continue/End choice is generated.")]
    public List<DialogOptionData> options = new List<DialogOptionData>();

    [Tooltip("Node entered when this node has no options and the player continues.")]
    public DialogNode next;
}

[Serializable]
public class DialogOptionData
{
    [Tooltip("Text shown in the option list.")]
    public string text;

    [Tooltip("Node entered when this option is chosen. When empty the dialog ends.")]
    public DialogNode next;

    [Tooltip("Actions executed when this option is chosen.")]
    public List<DialogActionData> actions = new List<DialogActionData>();

    [Tooltip("End the dialog right after the actions, ignoring 'next'.")]
    public bool endDialog;
}
