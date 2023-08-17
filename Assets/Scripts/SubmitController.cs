using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmitController : MonoBehaviour
{
    public OpenAIController openAICriminal = null;
    public OpenAIController openAIDetective = null;
    public TMP_InputField inputField = null;
    public TMP_Text outputText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        openAICriminal.onOpenAIResponse.AddListener(HandleCriminalResponse);
        openAIDetective.onOpenAIResponse.AddListener(HandleDetectiveResponse);
    }

    private void OnDisable()
    {
        openAICriminal.onOpenAIResponse.RemoveListener(HandleCriminalResponse);
        openAIDetective.onOpenAIResponse.RemoveListener(HandleDetectiveResponse);
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

            outputText.text += newMessage + "\n\n";

            openAICriminal.submitUserMessage(newMessage);
        }
    }

    void HandleCriminalResponse(string response)
    {
        outputText.text += response + "\n\n";

        openAIDetective.submitUserMessage(response);
    }

    void HandleDetectiveResponse(string response)
    {
        inputField.text = response;
    }
}
