using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
public class OpenAIController : MonoBehaviour
{
    public StringEvent onOpenAIResponse;
    public string model = "gpt-3.5-turbo";
    private List<ChatMessage> messages = new List<ChatMessage>();
    //public TMP_Text outputField = null;
    public string startInstruction = null;
    public bool showFirst = false;
    public APIKeyGetter getter = null;
    public bool submitOnStart = true;

    string apiKey = null;

    public class ChatMessage
    {
        public string text;
        public string role;

        public ChatMessage(string text, string role)
        {
            this.text = text;
            this.role = role;
        }
    }

    private void OnEnable()
    {
        if (onOpenAIResponse == null)
        {
            onOpenAIResponse = new StringEvent();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        apiKey = getter.GetAPIKey();

        if (startInstruction != null && startInstruction != "" && submitOnStart)
        {
            messages.Add(new ChatMessage(startInstruction, "user"));
            StartCoroutine(GetRequest(showFirst));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendStartMessage()
    {
        if (startInstruction != null && startInstruction != "")
        {
            messages.Add(new ChatMessage(startInstruction, "user"));
            StartCoroutine(GetRequest(showFirst));
        }
    }

    public void submitUserMessage(string message)
    {
        Debug.Log("Sending: " + message);
        messages.Add(new ChatMessage(message, "user"));
        StartCoroutine(GetRequest());
    }

    public void resetChat()
    {
        // TODO: resubmit initial chat
        messages.Clear();
    }

    IEnumerator GetRequest(bool showAnswer = true)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        string data = "{\"model\": \"" + model + "\", \"messages\": [";

        int count = 0;
        foreach (ChatMessage message in messages)
        {
            if(count > 0)
            {
                data += ", ";
            }
            data += "{\"role\": \"" + message.role + "\", \"content\": \"" + message.text.Replace("\"","\\\"") + "\"}";
            count++;
        }

        data += "]}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);

            if(request.responseCode == 503)
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(GetRequest(showAnswer));
            }
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            /*var responseDict = JsonUtility.FromJson<Dictionary<string, object>>(responseJson);
            var responseList = (List<object>)responseDict["choices"];
            var responseDict2 = (Dictionary<string, object>)responseList[0];
            var content = (string)responseDict2["text"];*/

            JSONNode jsonNode = JSON.Parse(responseJson);

            string content = jsonNode["choices"][0]["message"]["content"];

            Debug.Log("Received: " + content);

            messages.Add(new ChatMessage(content, "assistant"));

            if (showAnswer)
            {
                onOpenAIResponse?.Invoke(content);
            }
        }
    }
}
