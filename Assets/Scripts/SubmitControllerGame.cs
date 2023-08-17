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
            Debug.Log("Got direction");
            string gridInfo = gameState.MovePlayer(response);
            Debug.Log(gridInfo);
            openAIGame.submitUserMessage(gridInfo);
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
