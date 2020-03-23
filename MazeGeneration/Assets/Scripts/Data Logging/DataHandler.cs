using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Networking;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public bool allowOneLogPerDevice = false;
    public string baseURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSew2H4-sq_9sXdFTLtyhcZQ7fXo6DoR8OVnkFkAmuOM-vm0Jg/formResponse"; // Dummy form, please change later
    public string[] entryIds;

    public void SendData(List<float> data)
    {
        List<string> tempConvertedData = new List<string>();

        string spec = "G";
        CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");

        foreach (float floatData in data)
        {
            tempConvertedData.Add(floatData.ToString(spec, ci));
        }

        StartCoroutine(Post(tempConvertedData));
    }

    public void SendData(List<string> data)
    {
        StartCoroutine(Post(data));
    }

    IEnumerator Post(List<string> finalData)
    {
        if (entryIds == null)
        {
            Debug.LogError("Result POST error: entry ID array is empty!");
            yield return null;
        }
        else if (PlayerPrefs.GetInt("dataSubmitted") == 1)
        {
            Debug.Log("Data already submitted by this user - post request is ignored");
            yield return null;
        }

        WWWForm form = new WWWForm();

        for (int i = 0; i < finalData.Count; i++)
        { 
            if (entryIds.Length > i)
                form.AddField(entryIds[i], finalData[i]);
        }

        byte[] rawData = form.data;

        UnityWebRequest webRequest = new UnityWebRequest(baseURL, UnityWebRequest.kHttpVerbPOST);
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(rawData);
        uploadHandler.contentType = "application/x-www-form-urlencoded";
        webRequest.uploadHandler = uploadHandler;
        webRequest.SendWebRequest();

        if (allowOneLogPerDevice)
            PlayerPrefs.SetInt("dataSubmitted", 1);

        yield return webRequest;
    }
}