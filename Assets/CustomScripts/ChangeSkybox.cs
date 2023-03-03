using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChangeSkybox : MonoBehaviour
{
    private string streetViewApiRoot = "https://maps.googleapis.com/maps/api/streetview?";
    private string geocodeApiRoot = "https://maps.googleapis.com/maps/api/geocode/json?";
    public static string key = "AIzaSyBPVjWSZ7gY_1eT70KFeSIn4Yc6zULaLEI"; // Get key from Google Cloud Platform
    private int fov = 90;
    private int size = 640; // Max size from StreetView API is 640 unfortunately
    private string lat;
    private string lng;
    private bool loadImages = true; // For debugging purposes, set to false when you don't want images loading

    // Holds the requested textures
    public static Texture2D[] textures = new Texture2D[6];

    public void LoadImagesFromString(string location)
    {
        Debug.Log("Starting up");

        Debug.Log("Checking if API key is null");
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Please enter an API key for Google Street View Access.  ");
            return;
        }

        toggleVignette(true);
        SetLatAndLng(location);
    }

    private void SetLatAndLng(string location)
    {
        //string ExampleUrl = "https://maps.googleapis.com/maps/api/geocode/json?place_id=ChIJeRpOeF67j4AR9ydy_PIzPuM&key=YOUR_API_KEY"
        string processedLocation = location.Replace(" ", "%20");
        string latLngRequest = geocodeApiRoot + "address=" + processedLocation + "&key=" + key;
        StartCoroutine(HandleLatLngRequest(latLngRequest));
    }

    private IEnumerator HandleLatLngRequest(string latLngRequest)
    {
        Debug.Log("Handling lat and long request: " + latLngRequest);

        UnityWebRequest webRequest = UnityWebRequest.Get(latLngRequest);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            var jsonString = webRequest.downloadHandler.text;
            GeocodeJson geocodeJson = JsonUtility.FromJson<GeocodeJson>(jsonString);

            if (geocodeJson.results.Length > 0)
            {
                lat = geocodeJson.results[0].geometry.location.lat;
                lng = geocodeJson.results[0].geometry.location.lng;

                DownloadStreetView();
            }
            else
            {
                // TODO: create location invalid screen
                Debug.Log("Location Invalid");
            }
        }
    }

    public void ChangeSkyboxFromMap(float normalizedXDir, float normalizedYDir)
    {
        toggleVignette(true);

        float currentLat = float.Parse(lat);
        float currentLng = float.Parse(lng);

        if ((currentLat < 0 & normalizedYDir < 0) | (currentLat < 0 & normalizedYDir > 0))
        {
            normalizedYDir = -normalizedYDir;
        }

        if ((currentLng < 0 & normalizedXDir < 0) | (currentLng < 0 & normalizedXDir > 0))
        {
            normalizedXDir = -normalizedXDir;
        }

        float newLat = currentLat + ((normalizedYDir * currentLat) / 25000);
        float newLng = currentLng + ((normalizedXDir * currentLng) / 25000);

        Debug.Log(currentLat + " " + newLat);

        lat = newLat.ToString();
        lng = newLng.ToString();

        DownloadStreetView();
    }

    private void DownloadStreetView()
    {
        //string ExampleUrl = "https://maps.googleapis.com/maps/api/streetview?size=512x512&location=-23.5834057,-46.681568&fov=180&heading=270&pitch=0&key=[GOOGLE_API_KEY_HERE]";
        string frontRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 0 + "&key=" + key;
        string rightRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 90 + "&key=" + key;
        string backRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 180 + "&key=" + key;
        string leftRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 270 + "&key=" + key;
        string upRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 180 + "&pitch=90" + "&key=" + key;
        string downRequest = streetViewApiRoot + "size=" + size + "x" + size + "&location=" + lat + "," + lng + "&fov=" + fov + "&heading=" + 180 + "&pitch=-90" + "&key=" + key;

        if (loadImages)
        {
            Debug.Log("Beginning requests");

            StartCoroutine(HandleTextureResponse(backRequest, 0));
            StartCoroutine(HandleTextureResponse(frontRequest, 1));
            StartCoroutine(HandleTextureResponse(leftRequest, 2));
            StartCoroutine(HandleTextureResponse(rightRequest, 3));
            StartCoroutine(HandleTextureResponse(upRequest, 4));
            StartCoroutine(HandleTextureResponse(downRequest, 5));
        }
        else
        {
            Debug.Log("Load images is false, images will not load");
        }
    }

    private IEnumerator HandleTextureResponse(string request, int faceIndex)
    {
        Debug.Log("Handling texture request: " + request);

        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(request);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            CopyTexture(myTexture, faceIndex);
        }
    }

    private void CopyTexture(Texture2D origin, int index)
    {
        Debug.Log("Copying texture");

        Texture2D dest = new Texture2D(size, size);

        for (int i = 0; i < origin.width; i++)
        {
            for (int j = 0; j < origin.height; j++)
            {
                Color pixel = origin.GetPixel(i, j);
                dest.SetPixel(i, j, pixel);
            }
        }

        dest.Apply();
        textures[index] = dest;

        if (index == 5)
        {
            Material material = CreateSkyboxMaterial();
            SetSkybox(material);
        }
    }


    private static Material CreateSkyboxMaterial()
    {
        Debug.Log("Creating skybox material");
        Material result = Resources.Load("Materials/SkyboxMaterial", typeof(Material)) as Material;

        result.SetTexture("_FrontTex", textures[0]);
        result.SetTexture("_BackTex", textures[1]);
        result.SetTexture("_LeftTex", textures[2]);
        result.SetTexture("_RightTex", textures[3]);
        result.SetTexture("_UpTex", textures[4]);
        result.SetTexture("_DownTex", textures[5]);

        return result;
    }

    private void SetSkybox(Material material)
    {
        Debug.Log("Setting skybox");
        RenderSettings.skybox = material;
        // After setting the skybox remove the vignette and update map
        toggleVignette(false);
        GameObject mapScriptGameObject = GameObject.Find("MapScript");
        StartCoroutine(mapScriptGameObject.GetComponent<Map>().UpdateMap(lat, lng));
    }

    private void toggleVignette(bool visibility)
    {
        Color endValue;

        if (visibility)
        {
            endValue = new Color(0.0f, 0.0f, 0.0f, 1);
        }
        else
        {
            endValue = new Color(0.0f, 0.0f, 0.0f, 0);
        }

        StartCoroutine(LerpFunction(endValue, 1));
    }

    IEnumerator LerpFunction(Color endValue, float duration)
    {
        GameObject panelGameObject = GameObject.Find("Canvas/Panel");
        Image image = panelGameObject.GetComponent<Image>();

        float time = 0;
        Color startValue = image.color;

        while (time < duration)
        {
            image.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        image.color = endValue;
    }
}

// Classes required for deserialize geocoding json data, strings are data that's not needed other than lat and lng
[System.Serializable]
public class GeocodeJson
{
    public Results[] results;
    public string status;
}

[System.Serializable]
public class Results
{
    public string address_components;
    public string formatted_address;
    public Geometry geometry;
    public string place_id;
    public string types;
}

[System.Serializable]
public class Geometry
{
    public Location location;
    public string location_type;
    public string viewport;
}

[System.Serializable]
public class Location
{
    public string lat;
    public string lng;
}