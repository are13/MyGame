

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog",menuName = "Game/Dialog Content")]
public class DialogContent : ScriptableObject
{
    
    [SerializeReference,SerializeReferenceSelector] public DialogNode start;
    [SerializeReference,SerializeReferenceSelector] public List<DialogNode> dialogs = new List<DialogNode>();
}