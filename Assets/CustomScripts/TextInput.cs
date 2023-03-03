using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextInput : MonoBehaviour
{
    void Start()
    {
        GameObject locationInputGameObject = GameObject.Find("Canvas/LocationInput");
        TMP_InputField locationInput = locationInputGameObject.GetComponent<TMP_InputField>();

        locationInput.onEndEdit.AddListener(LockInput);
    }

    // Checks if there is anything entered into the input field.
    private void LockInput(string input)
    {
        if (input.Length > 0)
        {
            Debug.Log("Text has been entered");
            Debug.Log(input);

            GameObject changeSkyboxScriptObject = GameObject.Find("ChangeSkyboxScript");
            changeSkyboxScriptObject.GetComponent<ChangeSkybox>().LoadImagesFromString(input);
        }
        else if (input.Length == 0)
        {
            Debug.Log("Main Input Empty");
        }
    }
}
