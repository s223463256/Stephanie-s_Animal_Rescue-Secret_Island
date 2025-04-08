using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewDialogue : MonoBehaviour
{
    // Store dialogue messages in list
    private List<GameObject> dialogueMessages = new List<GameObject>();
    private int index = 0;
    // Adding dialogue message to list
    public void AddDialogue(string text, GameObject d_template)
    {
        GameObject message = Instantiate(d_template, transform);
        Transform dialogueTextTransform = message.transform.Find("DialogueText");

        if(dialogueTextTransform != null)
        {
            TextMeshProUGUI dialogueText = dialogueTextTransform.GetComponent<TextMeshProUGUI>();
            if(dialogueText != null)
            {
                dialogueText.text = text;
            }
        }    
        else
        {
            Debug.LogWarning("DialogueText missing");
        }
        
        message.SetActive(false);
        dialogueMessages.Add(message);
    }
    // Start dialogue and display message
    public void StartDialogue()
    {
        if(dialogueMessages.Count > 0)
        {
            dialogueMessages[0].SetActive(true);
        }
    }
    // Handle dialogue progressing
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialogueMessages.Count > 0)
        {
            // Hide current dialogue and move next message
            dialogueMessages[index].SetActive(false);
            index++;
            // Checking if there are more dialogue to move to
            if(index < dialogueMessages.Count)
            {
                dialogueMessages[index].SetActive(true);
            }
            else
            // IF all dialogue are displayed end dialogue and destroy dialogue from scene
            {
                CharacterMover.dialogue = false;
                Destroy(gameObject);
            }
        }
    }
}
