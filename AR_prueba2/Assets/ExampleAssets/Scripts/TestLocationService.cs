using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestLocationService : MonoBehaviour
{
    //Patio
    public double startLat;
    public double startLon;

    //Biblioteca
    public double startLat2;
    public double startLon2;

    //Auditorio
    public double startLat3;
    public double startLon3;

    public static int state = 0;

    public TMP_Text texto1;
    public TMP_Text texto2;
    public TMP_Text texto3;
    public TMP_Text texto4;


    // Start is called before the first frame update
    IEnumerator Start()
    {

#if UNITY_EDITOR
        // No permission handling needed in Editor
#elif UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation) &&  
            !UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }

        // First, check if user has location service enabled
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("Android and Location not enabled");
            yield break;
        }

#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("IOS and Location not enabled");
            yield break;
        }
#endif
        // Start service before querying location
        UnityEngine.Input.location.Start(500f, 500f);

        // Wait until service initializes
        int maxWait = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            maxWait--;
        }

        // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
#if UNITY_EDITOR
        int editorMaxWait = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            editorMaxWait--;
        }
#endif

        // Service didn't initialize in 15 seconds
        if (maxWait < 1)
        {
            // TODO Failure
            Debug.LogFormat("Timed out");
            yield break;
        }

        // Connection has failed
        if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
        {
            // TODO Failure
            Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
            yield break;
        }
        else
        {
            Debug.LogFormat("Location service live. status {0}", UnityEngine.Input.location.status);
            // Access granted and location value could be retrieved
            Debug.LogFormat("Location: "
                + UnityEngine.Input.location.lastData.latitude + " "
                + UnityEngine.Input.location.lastData.longitude + " "
                + UnityEngine.Input.location.lastData.altitude + " "
                + UnityEngine.Input.location.lastData.horizontalAccuracy + " "
                + UnityEngine.Input.location.lastData.timestamp);

            var _latitude = UnityEngine.Input.location.lastData.latitude;
            var _longitude = UnityEngine.Input.location.lastData.longitude;
            // TODO success do something with location
        }

        // Stop service if there is no need to query location updates continuously
        //UnityEngine.Input.location.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        texto4.text = UnityEngine.Input.location.status.ToString() + "   " + state;
        Debug.LogFormat("Location service live. status {0}", UnityEngine.Input.location.status);

        if(UnityEngine.Input.location.status == LocationServiceStatus.Running)
        {
            var _latitude = UnityEngine.Input.location.lastData.latitude;
            var _longitude = UnityEngine.Input.location.lastData.longitude;

            double distance = CalculateDistance(startLat, startLon, _latitude, _longitude);
            double distance2 = CalculateDistance(startLat2, startLon2, _latitude, _longitude);
            double distance3 = CalculateDistance(startLat3, startLon3, _latitude, _longitude);

            texto1.text = distance.ToString();
            texto2.text = distance2.ToString();
            texto3.text = distance3.ToString();

            if(distance < 20 || distance2 < 20 || distance3 < 20)
            {
                if (distance < distance2)
                {
                    if (distance < distance3)
                    {
                        state = 1;
                    }
                    else
                    {
                        state = 3;
                    }
                }
                else
                {
                    if (distance2 < distance3)
                    {
                        state = 2;
                    }
                    else
                    {
                        state = 3;
                    }
                }

            }
            else
            {
                state = 0;
            }
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        float R = 6371000; // Radio de la Tierra en metros
        float dLat = ((float)lat2 - (float)lat1) * Mathf.Deg2Rad;
        float dLon = ((float)lon2 - (float)lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos((float)lat1 * Mathf.Deg2Rad) * Mathf.Cos((float)lat2 * Mathf.Deg2Rad) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float distance = R * c;
        return distance;
    }
}
