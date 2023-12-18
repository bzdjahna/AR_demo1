using System.Text;
using UnityEngine;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class NativeGPSUI : MonoBehaviour
{
    public Text text;
    bool locationIsReady = false;
    bool locationGrantedAndroid = false;
    GameObject dialog = null;
    
    public double startLat;
    public double startLon;
    public double maxDistance = 50.0f;

    private void Start() 
    {
        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            dialog = new GameObject();
        }
        else
        {
            locationGrantedAndroid = true;
            locationIsReady = NativeGPSPlugin.StartLocation();
        }

        #elif PLATFORM_IOS

        locationIsReady = NativeGPSPlugin.StartLocation();
    
        #endif
    }

    private void Update() 
    {
        if (locationIsReady)
        {

            double currentLat = NativeGPSPlugin.GetLatitude();
            double currentLon = NativeGPSPlugin.GetLongitude();
            //double currentLat = 42.73061095089415; 
            //double currentLon = -84.53334051853945;


            double distance = CalculateDistance(startLat, startLon, currentLat, currentLon);
            //distanceText.text = "Distancia: " + distance.ToString("F2") + " metros";

            text.text = "Distancia " + distance;
            if (distance < maxDistance)
            {
                //TO DO
                
            }



            //StringBuilder sb = new StringBuilder();

            //sb.AppendLine("Longitude: "+NativeGPSPlugin.GetLongitude());
            //sb.AppendLine("Latitude: "+NativeGPSPlugin.GetLatitude());
            //sb.AppendLine("Accuracy: "+NativeGPSPlugin.GetAccuracy());
            //sb.AppendLine("Altitude: "+NativeGPSPlugin.GetAltitude());
            //sb.AppendLine("Speed: "+NativeGPSPlugin.GetSpeed());
            //sb.AppendLine("Speed Accuracy Meters Per Second: "+NativeGPSPlugin.GetSpeedAccuracyMetersPerSecond());
            //sb.AppendLine("Vertical Accuracy Meters: "+NativeGPSPlugin.GetVerticalAccuracyMeters());

            //text.text = sb.ToString();
        }
    }

    void OnGUI ()
    {
        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // The user denied permission to use the fineLocation.
            // Display a message explaining why you need it with Yes/No buttons.
            // If the user says yes then present the request again
            // Display a dialog here.
            dialog.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialog != null)
        {
            if (!locationGrantedAndroid)
            {
                locationGrantedAndroid = true;
                locationIsReady = NativeGPSPlugin.StartLocation();
            }

            Destroy(dialog);
        }
        #endif
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