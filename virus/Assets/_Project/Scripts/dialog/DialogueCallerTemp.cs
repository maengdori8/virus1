using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCallerTemp : MonoBehaviour
{
    public NPCConversation conv;
    // Start is called before the first frame update
    void Start()
    {
        ConversationManager.Instance.StartConversation(conv);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
