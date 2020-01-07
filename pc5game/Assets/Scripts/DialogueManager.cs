using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* DialogueManager will iterate over an sentences contained in a Dialogue, 
 * proceeding to the next sentence in the queue when prompted. It controls UI 
 * display of the dialogue. */
public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentenceQ;
    string arrow = "";

    // Start is called before the first frame update
    void Start()
    {
        sentenceQ = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyMap.NextSentence))
        {
            UpdateSentence();
        }
    }
    public void StartDialogue(Dialogue dialogue)
    {
        foreach (string sentence in dialogue.sentences)
        {
            sentenceQ.Enqueue(sentence);
        }

        if (sentenceQ.Count > 1)
        {
            arrow = ">>>";
        }
    }

    public void UpdateSentence()
    {

    }
    public void DisplayNextSentence()
    {
        /* End dialogue when there are no more sentences. */
        if (sentenceQ.Count == 0)
        {
            EndDialogue();
            return;
        }

        /* If the current sentence is alone in the queue, do not display arrows
         * that indicate there are more sentences. */
        if (sentenceQ.Count == 1)
        {
            arrow = "";
        }

        string sentence = sentenceQ.Dequeue();
        Debug.Log(sentence + "  " + arrow);
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue ended.");
    }
}
