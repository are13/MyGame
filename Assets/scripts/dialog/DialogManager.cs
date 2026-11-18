

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{

    public TextMeshProUGUI dialogShow;
    public TextMeshProUGUI speaker;
    public UnityEvent onDialogStart;
    public UnityEvent onDialogEnd;
    public List<GameObject> gameObjectsToDisable;
    DialogNode currentNode;
    public void startDialog(DialogContent content, String speaker)
    {
        

        currentNode = content.start;
        gameObjectsToDisable.ForEach(v => v.SetActive(true));
        onDialogStart?.Invoke();
        print(currentNode);
        updateNext();
    }

    public void updateNext()
    {

        if (currentNode is EndNode endNode)
        {
            exitDialog();
            currentNode = null;
        }

        if (currentNode is DialogText dialogText)
        {

            dialogShow.text = dialogText.text;
            speaker.text = dialogText.speaker;
        }

        if (currentNode is ActionNode actionNode)
        {
            resetBgs();
            foreach (var action in actionNode.actions)
            {
                action.execute(GameManager.manager.player);
            }
            currentNode = actionNode.next;
        }

        if (currentNode is OptionNode optionNode)
        {


        }

        currentNode = currentNode.next;
    }

    public void exitDialog()
    {

        dialogShow.text = "";
        speaker.text = "";
        resetBgs();
        onDialogEnd?.Invoke();
    }

    public void skip()
    {


    }

    public void resetBgs()
    {

        gameObjectsToDisable.ForEach(v => v.SetActive(false));
    }
}