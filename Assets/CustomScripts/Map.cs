using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Map : MonoBehaviour
{
    public static string staticMapKey = "AIzaSyBPVjWSZ7gY_1eT70KFeSIn4Yc6zULaLEI"; // Get key from Google Cloud Platform
    public enum mapTypes { roadmap, satellite, gybrid, terrain };
    public enum mapResolutions { low = 1, high = 2 };
    private const int ZOOM = 18;

    private float mapWidth;
    private float mapHeight;
    private GameObject mapButtonGameObject;

    void Start()
    {
        // Initialize the map button game object so we can use it

        mapButtonGameObject = GameObject.Find("Canvas/Map");

        Button button = mapButtonGameObject.GetComponent<Button>();
        RectTransform mapTransform = mapButtonGameObject.GetComponent<RectTransform>();

        mapWidth = mapTransform.sizeDelta.x;
        mapHeight = mapTransform.sizeDelta.y;

        float minX = mapTransform.position.x;
        float maxX = mapTransform.position.x + mapWidth;

        float minY = mapTransform.position.y;
        float maxY = mapTransform.position.y + mapHeight;

        button.onClick.AddListener(delegate { TaskOnClick(minX, maxX, minY, maxY); });
    }

    private void TaskOnClick(float minX, float maxX, float minY, float maxY)
    {
        // Transforms the current mouse position into normalized form so that we can add or subtract from the latidue and longitude
        // Ranges from -0.5 to +0.5 in both X and Y direction

        Vector3 mousePos = Input.mousePosition;
        float currentX = mousePos.x;
        float currentY = mousePos.y;

        float normalizedXDir = (currentX - minX) / (maxX - minX);
        float normalizedYDir = (currentY - minY) / (maxY - minY);

        Debug.Log("Normalized position: (" + normalizedXDir + ", " + normalizedYDir + ")");

        GameObject changeSkyboxScriptObject = GameObject.Find("ChangeSkyboxScript");
        changeSkyboxScriptObject.GetComponent<ChangeSkybox>().ChangeSkyboxFromMap(normalizedXDir, normalizedYDir);
    }

    public IEnumerator UpdateMap(string lat, string lng)
    {
        Debug.Log("Updating map");

        mapButtonGameObject.GetComponent<RawImage>().enabled = true;
        string url = "https://maps.googleapis.com/maps/api/staticmap?center=" + lat + "," + lng + "&zoom=" + ZOOM + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + mapResolutions.low + "&maptype=" + mapTypes.roadmap + "&key=" + staticMapKey;
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Update Failed: " + webRequest.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            mapButtonGameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture; ;

            Debug.Log("Updating complete");
        }

    }
}
