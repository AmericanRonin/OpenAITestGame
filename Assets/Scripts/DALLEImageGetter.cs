using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class DALLEImageGetter : MonoBehaviour
{
    private string apiMode = "generations";
    private string apiUrl = "https://api.openai.com/v1/images/generations";
    public string style = null;

    public Image image;

    public APIKeyGetter getter = null;

    string apiKey = null;

    // Start is called before the first frame update
    void Start()
    {
        apiKey = getter.GetAPIKey();
    }

    public void SubmitImagePrompt(string prompt)
    {
        StartCoroutine(GetDallEImage(prompt));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetDallEImage(string prompt)
    {
        string fullPrompt = prompt;

        if(style != null)
        {
            fullPrompt += ", " + style;
        }

        Dictionary<string, object> aiParams = new Dictionary<string, object>();
        aiParams.Add("model", "image-alpha-001");
        aiParams.Add("prompt", "a dragon");
        aiParams.Add("size", "1024x1024");
        aiParams.Add("response_format", "url");

        string jsonString = "{\"model\":\"image-alpha-001\",\"prompt\":\"" + fullPrompt + "\",\"size\":\"1024x1024\",\"response_format\":\"url\"}";
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            // Use the image URL returned by the API to display the image.
            // ...
            string responseJson = request.downloadHandler.text;

            JSONNode jsonNode = JSON.Parse(responseJson);

            string content = jsonNode["data"][0]["url"];

            Debug.Log(content);

            Texture2D texture = new Texture2D(1, 1);
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(content);
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(imageRequest.error);
            }
            else
            {
                texture = DownloadHandlerTexture.GetContent(imageRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
                image.sprite = sprite;
                image.preserveAspect = true;
            }
        }
    }
}
