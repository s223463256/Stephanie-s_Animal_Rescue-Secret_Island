using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Multiplayer.Center.Common.Analytics;
public class NPCSystem : MonoBehaviour
{
    public GameObject d_template;
    public GameObject canva;
    public string characterName;
    public Sprite characterImage;
    
    private bool player_detection = false;
    private bool dialogueTriggered = false;
    private GameObject currentDialogue;
    public bool story_dialogue = false;
    public string JsonFileName;
    private NewDialogue dialogueScript;
    // Store multiple dialogues for npcs
    public List<InteractionDataType> interactionStages = new List<InteractionDataType>();
    private int interactionCount = 0;
    // Serializable class storing multiple lines of dialogue in the interaction stage
    [System.Serializable]
    public class InteractionDataType{
        public List<string> dialogueLines;
    }

    [System.Serializable]
    public class InteractionSet
    {
        public List<string> dialogueLines;
    }

    [System.Serializable]
    public class StoryDialogueData
    {
        public List<InteractionSet> interactions;
    }
    

    // Function to detect if player is detected and no dialogue is playing to start a new dialogue
    void Update()
    {
        if (player_detection && !CharacterMover.dialogue && !dialogueTriggered)
        {
            dialogueTriggered = true;
            canva.SetActive(true);
            CharacterMover.dialogue = true;
            StartDialogue();
        }

    }

    void CharacterUI(GameObject templateInstance)
    {
        TMP_Text nameText = templateInstance.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
        if(nameText!=null)
        {
            nameText.text = characterName;
            nameText.alignment = TextAlignmentOptions.Center;
        }
        
        UnityEngine.UI.Image charImage = templateInstance.transform.Find("CharacterImage")?.GetComponent<UnityEngine.UI.Image>();
        if(charImage != null && characterImage !=null)
        {
            charImage.sprite = characterImage;
        }
    }

    void StartDialogue()
    {
        // Destroy Dialoge from GameObject if exists
        if(currentDialogue != null)
        {
            Destroy(currentDialogue);
        }

        List<string> selectedDialogue;

        if (story_dialogue)
        {
            TextAsset jsonText = Resources.Load<TextAsset>($"StoryDialogue/{JsonFileName}");
            if (jsonText == null)
            {
                Debug.LogError("Story JSON file not found!");
                return;
            }
            
            StoryDialogueData storyData = JsonUtility.FromJson<StoryDialogueData>(jsonText.text);

            // Clamp index to available interactions
            int stageIndex = Mathf.Clamp(interactionCount, 0, storyData.interactions.Count - 1);
            selectedDialogue = storyData.interactions[stageIndex].dialogueLines;
        }
        else
        {
            // Ensure the NPC dialogue is available logging errors
            if (interactionStages.Count == 0)
            {
                Debug.LogError("NPC has no dialogue");
                return;
            }
            // Selecting dialogue stage based on number of interactions
            int stageIndex = Mathf.Clamp(interactionCount, 0, interactionStages.Count - 1);
            selectedDialogue = interactionStages[stageIndex].dialogueLines;

        }

        // Create new dialogue
        currentDialogue = new GameObject("DialogueContrainer");
        currentDialogue.transform.SetParent(canva.transform, false);

        dialogueScript = currentDialogue.AddComponent<NewDialogue>();

        GameObject templateInstance = Instantiate(d_template, currentDialogue.transform);
        CharacterUI(templateInstance);

        // Adding each line of dialogue to dialogue system
        foreach (string line in selectedDialogue)
        {
            dialogueScript.AddDialogue(line, templateInstance);
        }
        // Display dialogue
        dialogueScript.StartDialogue();
        // Changes dialogue will need to add condition if want to only move dialogue if part of the game is completed
        interactionCount++;
    }
    // Detect when player enters npc bubble
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "3D Character")
        {
            player_detection = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        player_detection = false;
        // Retrigger same dialogue
        dialogueTriggered = false;
    }
}
