using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.IO;
using ViveSR.anipal.Eye;
using ViveSR.anipal;
using ViveSR;
using UnityEditor.PackageManager;
using System.Collections.Generic;
using ClipperLib;
using Unity.VisualScripting;
using static UnityEngine.Tilemaps.Tile;

public class focusObject : MonoBehaviour
{
    public string filename;
    StreamWriter writer;

    private static Ray testRay;
    private static FocusInfo focusInfo;

    public LayerMask layerMask;


    // Start is called before the first frame update
    void Start()
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        writer = new StreamWriter(string.Format("ObjectData/{0}_{1}.txt", filename, unixTimestamp));
        string header = "date,currentTime,timeStamp,focusedObject,objectName";
        writer.WriteLine(header);
        //0.05s == 20 Hz, can change based on the device
        InvokeRepeating("ObjectName", 2.0F, 0.05F);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ObjectName();
        }
    }

    // get the focused object
    public static GameObject Focus()
    {

            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out testRay, out focusInfo)) { }
            else if (SRanipal_Eye_v2.Focus(GazeIndex.LEFT, out testRay, out focusInfo)) { }
            else if (SRanipal_Eye_v2.Focus(GazeIndex.RIGHT, out testRay, out focusInfo)) { }
            else return null;


        return focusInfo.collider.gameObject;
        

    }


    // Checks name for current object in focus.

    public static string FocusName()
    {

        if (Focus() is null)
            return "null";
        else 
            return Focus().name;
    }

    //Checks type 

    public static string ObjectPart()
    {
        string objectBeingLookedAt = FocusName();
        //get the collider type
        System.Type colliderType = focusInfo.collider.GetType();
        //convert it to string
        string colliderName = colliderType.ToString();

        return colliderName;
    }


    private void OnApplicationQuit()
    {
        writer.Close();
    }

    private void ObjectName()
    {
        string timeStamp = DateTime.Now.ToString("yyyy.MM.dd, HH:mm:ss.ffffff");
        long ux = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        string objectBeingLookedAt = FocusName();
        string objectPart = ObjectPart();
        string line = timeStamp + "," + ux + "," + objectBeingLookedAt + "," + objectPart;
        writer.WriteLine(line);
    }

}
