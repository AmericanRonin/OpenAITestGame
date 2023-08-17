using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class APIKeyGetter : MonoBehaviour
{
    string apiKey = "";

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Application.dataPath + "/APIKey.txt";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                apiKey = reader.ReadLine();
            }
        }
        else
        {
            // TODO: popup asking for API Key
        }
    }
    public string GetAPIKey()
    {
        return apiKey;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
