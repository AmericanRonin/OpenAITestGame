using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmitControllerGame : MonoBehaviour
{
    public OpenAIController openAIGame = null;
    public OpenAIController promptGetter = null;
    public string promptInstructions = null;
    public TMP_InputField inputField = null;
    public TMP_Text outputText;
    public DALLEImageGetter imageGetter = null;
    public bool displayImages = true;
    public GameStateSetter gameState = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        openAIGame.onOpenAIResponse.AddListener(HandleGameResponse);
        if (promptGetter)
        {
            promptGetter.onOpenAIResponse.AddListener(HandlePromptResponse);
        }
    }

    private void OnDisable()
    {
        openAIGame.onOpenAIResponse.RemoveListener(HandleGameResponse);
        if (promptGetter)
        {
            promptGetter.onOpenAIResponse.RemoveListener(HandlePromptResponse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitClicked()
    {
        if (inputField.text != "")
        {
            string newMessage = inputField.text;
            inputField.text = "";

            outputText.text += "\n\n" + newMessage;

            openAIGame.submitUserMessage(newMessage);
        }
    }

    void HandleGameResponse(string response)
    {
        if (response[0] == '[') // TODO: remove this part
        {
            string[] parts = response.Split(new char[] { '[', ']' }, 3);

            if (parts.Length > 1)
            {
                string textInBrackets = parts[1];
                string textAfterBrackets = parts[2];
                Debug.Log(textInBrackets);
                Debug.Log(textAfterBrackets);

                imageGetter.SubmitImagePrompt(textInBrackets);
                outputText.text = textAfterBrackets.Replace("\\n","\n");
            }
        }
        else if(response[0] == '*')
        {
            Debug.Log("Got instuction from AI");
            if (response.ToLower().Contains("fight"))
            {
                openAIGame.submitUserMessage("*the monster was deafeated*");
            }
            else if (response.ToLower().Contains("inventory"))
            {
                string inventory = "*contents of inventory: level 1 weapon, healing item";
                if(gameState.hasImportantItem)
                {
                    inventory += ", important item";
                }
                inventory += "*";
                openAIGame.submitUserMessage(inventory);
            }
            else if (response.ToLower().Contains("got item"))
            {
                if(gameState.GetCurrentLocationData().containsItem)
                {
                    openAIGame.submitUserMessage("*player obtains item*");
                    gameState.hasImportantItem = true;
                    // TODO: need to set that item no longer at this location
                }
                else
                {
                    openAIGame.submitUserMessage("*item is not at this location*");
                }
            }
            else if (response.ToLower().Contains("use goal"))
            {
                if(gameState.GetCurrentLocationData().containsGoal)
                {
                    if(gameState.hasImportantItem)
                    {
                        openAIGame.submitUserMessage("*player has won the game*");
                    }
                    else
                    {
                        openAIGame.submitUserMessage("*player is missing item to use goal*");
                    }
                }
                else
                {
                    openAIGame.submitUserMessage("*the goal is not in this area*");
                }
            }
            else
            {
                string gridInfo = gameState.MovePlayer(response);
                Debug.Log(gridInfo);
                openAIGame.submitUserMessage(gridInfo);
            }
        }
        else
        {
            outputText.text = response.Replace("\\n", "\n");
            if (displayImages)
            {
                promptGetter.resetChat();
                promptGetter.submitUserMessage(promptInstructions + "\\n" + response);
            }
        }
    }

    void HandlePromptResponse(string response)
    {
        imageGetter.SubmitImagePrompt(response);
    }
}
