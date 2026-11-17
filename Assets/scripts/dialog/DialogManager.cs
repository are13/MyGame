

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
    int currentText;

    public void startDialog(DialogContent content, String speaker)
    {
        currentNode = content.start;
        gameObjectsToDisable.ForEach(v => v.SetActive(true));
        print(currentNode);
        updateNext();
    }

    public void updateNext()
    {

        if (currentNode is EndNode endNode)
        {
            exitDialog();
            currentNode = null;
            currentText = 0;
        }

        if (currentNode is DialogText dialogText)
        {

            dialogShow.text = dialogText.text;
            speaker.text = dialogText.speaker;
            currentText++;
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
    }

    public void exitDialog()
    {

        dialogShow.text = "";
        speaker.text = "";
        resetBgs();
    }

    public void skip()
    {


    }

    public void resetBgs()
    {

        gameObjectsToDisable.ForEach(v => v.SetActive(false));
    }
}