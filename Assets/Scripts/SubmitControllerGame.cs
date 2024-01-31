using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubmitControllerGame : MonoBehaviour
{
    public OpenAIController openAIGame = null;
    public OpenAIController promptGetter = null;
    public OpenAIController aiPlayer = null;
    public bool useAIPlayer = false;
    public string promptInstructions = null;
    public TMP_InputField inputField = null;
    public TMP_Text outputText;
    public DALLEImageGetter imageGetter = null;
    public bool displayImages = true;
    public GameStateSetter gameState = null;
    public ScrollRect textScroll;

    public string[] placeSettings = new string[] { "in a post-apocolyptic wasteland.", "in a fantasy setting.", "in a scifi setting.", "on an alien world.", "in a dungeon.",
    "in an abandoned castle.", "in an abandoned space station.", "on a mysterious island.", "in a strangely empty city.", "in a labyrinth."};

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
        if(aiPlayer)
        {
            aiPlayer.onOpenAIResponse.AddListener(HandleAIPlayerResponse);
        }

        string genre = placeSettings[Random.Range(0, placeSettings.Length)];

        openAIGame.startInstruction += "\\nMake this adventure take place " + genre;

        Debug.Log(genre);
    }

    private void OnDisable()
    {
        openAIGame.onOpenAIResponse.RemoveListener(HandleGameResponse);
        if (promptGetter)
        {
            promptGetter.onOpenAIResponse.RemoveListener(HandlePromptResponse);
        }
        if (aiPlayer)
        {
            aiPlayer.onOpenAIResponse.AddListener(HandleAIPlayerResponse);
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

            textScroll.verticalNormalizedPosition = 0; // send scroll bar to bottom

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

                textScroll.verticalNormalizedPosition = 1; // reset scroll bar back to top
            }
        }
        else if(response[0] == '*')
        {
            Debug.Log("Got instuction from AI");
            if (response.ToLower().Contains("fight"))
            {
                openAIGame.submitUserMessage(gameState.FightMonster());
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
            else if (response.ToLower().Contains("monster subdued"))
            {
                openAIGame.submitUserMessage(gameState.SubdueMonster());
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
            if(useAIPlayer && aiPlayer)
            {
                aiPlayer.submitUserMessage(response);
            }

            outputText.text = response.Replace("\\n", "\n");
            textScroll.verticalNormalizedPosition = 1; // reset scroll bar back to top
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

    void HandleAIPlayerResponse(string response)
    {
        inputField.text = response;

        SubmitClicked();
    }
}
