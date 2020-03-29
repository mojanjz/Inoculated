using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class NextSceneName : MonoBehaviour
{
    public int SceneIndex;
    [SerializeField] private DialogueTrigger trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInteract otherPlayer = collision.GetComponent<PlayerInteract>();
            Examiner ex = collision.GetComponent<Examiner>();
            KeyMap keyMap = collision.GetComponent<KeyMap>();

            otherPlayer.OnInteractStart();

            UnityAction<Node> handler = null;
            handler = (Node entryNode) =>
            {
                otherPlayer.OnInteractEnd();
                DialogueManager.Instance.OnEndDialogueEvent.RemoveListener(handler);
            };

            DialogueManager.Instance.OnEndDialogueEvent.AddListener(handler);
            trigger.Trigger(keyMap.SelectKey, keyMap.PrevKey, keyMap.NextKey, ex.Stats);
        }
    }
}
