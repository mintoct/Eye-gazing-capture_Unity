using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.IO;
using ViveSR.anipal.Eye;
using ViveSR.anipal;
using ViveSR;
using UnityEditor.PackageManager;

public class eyeTracking_v2 : MonoBehaviour
{
    //Calibration();
    public string filename;
    private static EyeData_v2 eyeData = new EyeData_v2();
    public static bool eye_callback_registered = false;
   // private static Vector3 Eyeposition;
    public bool cal_need;
    public bool result_cal;
    private static StreamWriter writer;
    private static Vector3 Eyeposition;
    private static Ray testRay;
    private static FocusInfo focusInfo;
    // Start is called before the first frame update
    void Start()
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();


        // initialize txt file

        writer = new StreamWriter(string.Format("EyeData/{0}_{1}.txt", filename, unixTimestamp));
        // Pupil diameter combined left and right pupil diameter (mm)
        // Gaze direction is eye gaze direction 
        // Gaze origin is gaze origin
        // openess is 0 close eye, 1 open eye
        // Frown is users's frown
        // Squeeze is to show how the eye is closed tightly
        // Wide is to show how the eye is opened widely
        // Distance from the central point of right and left eyes, but it shows 0. Currently, it doesnt work well
        string header = "sample_timestamp," + "current_timestamp," + "date," + "current_time," +  
            //"combined_pupil_diameter," + 
            "left_pupil_diameter," + "right_pupil_diameter," + 
            "combined_originX," + "combined_originY," + "combined_originZ," + 
            "left_originX," + "left_originY," + "left_originZ," + 
            "right_originX," + "right_originY," + "right_originZ," + 
            "combined_directionX," + "combined_directionY," + "combined_directionZ," + 
            "left_directionX," + "left_directionY," + "left_directionZ," + 
            "right_directionX," + "right_directionY," + "right_directionZ," + 
            "combined_openness," + "left_openess," + "right_openness," + 
            //"frown_left," + "frown_right," + 
            "squeeze_left," + "squeeze_right,"+ 
            "wide_left," + "wide_right," +
            "pupil_pos_left_X," + "pupil_pos_left_Y,"  +
            "pupil_pos_right_X," + "pupil_pos_right_Y" + "object";

        // first row indicates when data collection started

        //long ux = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        // milliseconds yyyy:mm:ss hh:ss:ss
        //DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(ux).DateTime;
        //microseconds yyyy:mm:ss hh:ss:ss ffffff
        //string dt = DateTime.Now.ToString("yyyy.MM.dd,HH:mm:ss.ffffff");
        //writer.WriteLine(ux);
        //writer.WriteLine(dt);
        writer.WriteLine(header);
    }

    // Update is called once per frame
    void Update()
    {
        
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
            {
                return;
            }

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }

/*        Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

        if (eye_callback_registered)
        {
            if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else return;
        }
        else
        {
            if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else return;
        }

        Eyeposition = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
        Focus();
        FocusName();*/

    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;

        }
        try
        {
            // end writing to file

            long ux = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(ux).DateTime;
            string dt = DateTime.Now.ToString("yyyy.MM.dd,HH:mm:ss.ffffff");
            //writer.WriteLine(ux);
            //writer.WriteLine(dt);
            writer.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("File has been closed before");
            Debug.Log(ex.Message);
        }

    }

    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {

        eyeData = eye_data;

        writeFile(eye_data);

    }

    private static void writeFile(EyeData_v2 eye_data)
    {
        //output example: 13623670
        int sample_timestamp;
        //output example: 1676905175881
        long uxtime;
        //output example: 2023.02.20,14:59:35.882787
        string real_time;
        //output example: 0 invalid
        //float combined_pupil_diameter;
        //output example: 4.750717
        float left_pupil_diameter;
        //output example: 4.67366
        float right_pupil_diameter;
        //output example: -0.4656982,7.976929,-26.59496,
        Vector3 combined_origin;
        //output example: 33.62311,7.630936,-27.32239,
        Vector3 left_origin;
        //output example: -30.81018,8.284927,,-25.94743,
        Vector3 right_origin;
        //output example: -0.04589844,-0.1760559,0.9833069,
        Vector3 combined_direction;
        //output example: -0.04188538,-0.1742554,0.9837952,
        Vector3 left_direction;
        //output example: -0.04946899,-0.1776581,0.9828339,
        Vector3 right_direction;
        //output example: 0-1
        float combined_openness;
        //output example:0-1
        float left_openness;
        //output example: 0-1
        float right_openness;
        //output example:0 invalid
        //float frown_L;
        //float frown_R;
        //output example: 0-0.34223820
        float squeeze_L;
        //output example: 0-0.22394510
        float squeeze_R;
        //output example: 0-1
        float wide_L;
        float wide_R;
        //output example: 0.4787923,0.338675
        Vector2 pos_sensor_L;
        //output example: 0.4903919,0.3132238
        Vector2 pos_sensor_R;
        //float distance_C;
        //bool distance_valid_C;

        //string objectBeingLookedAt = FocusName();

        // timestamp
        sample_timestamp = eyeData.timestamp;

        //realtime && timestamp
        uxtime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //DateTime real_time = DateTimeOffset.FromUnixTimeMilliseconds(uxtime).DateTime;
        real_time = DateTime.Now.ToString("yyyy.MM.dd,HH:mm:ss.ffffff");

        //frameCount
        //int frameCount = Time.frameCount;

        //pupil diameter
        //combined_pupil_diameter = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;
        left_pupil_diameter = eyeData.verbose_data.left.pupil_diameter_mm;
        right_pupil_diameter = eyeData.verbose_data.right.pupil_diameter_mm;

        // origin
        combined_origin = eyeData.verbose_data.combined.eye_data.gaze_origin_mm;
        left_origin = eyeData.verbose_data.left.gaze_origin_mm;
        right_origin = eyeData.verbose_data.right.gaze_origin_mm;

        //direction
        combined_direction = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
        left_direction = eyeData.verbose_data.left.gaze_direction_normalized;
        right_direction = eyeData.verbose_data.right.gaze_direction_normalized;

        //openness
        combined_openness = eyeData.verbose_data.combined.eye_data.eye_openness;
        left_openness = eyeData.verbose_data.left.eye_openness;
        right_openness = eyeData.verbose_data.right.eye_openness;

        //frown
        //frown_L = eyeData.expression_data.left.eye_frown;
        //frown_R = eyeData.expression_data.right.eye_frown;

        //squeeze
        squeeze_L = eyeData.expression_data.left.eye_squeeze;
        squeeze_R = eyeData.expression_data.right.eye_squeeze;

        //wide
        wide_L = eyeData.expression_data.left.eye_wide;
        wide_R = eyeData.expression_data.right.eye_wide;

        //pupil position
        pos_sensor_L = eyeData.verbose_data.left.pupil_position_in_sensor_area;
        pos_sensor_R = eyeData.verbose_data.right.pupil_position_in_sensor_area;

        //Distance from the central point of right and left eyes
        //distance_C = eyeData.verbose_data.combined.convergence_distance_mm;

        //Distance validity
        //distance_valid_C = eyeData.verbose_data.combined.convergence_distance_validity;

        




        string data_row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
            sample_timestamp, uxtime, real_time,
            //combined_pupil_diameter, 
            left_pupil_diameter, right_pupil_diameter,
            vector3ToString(combined_origin), vector3ToString(left_origin), vector3ToString(right_origin),
            vector3ToString(combined_direction), vector3ToString(left_direction), vector3ToString(right_direction),
            combined_openness, left_openness, right_openness,
            //frown_L, frown_R,
            squeeze_L, squeeze_R,
            wide_L, wide_R,
            vector2ToString(pos_sensor_L), vector2ToString(pos_sensor_R));

        writer.WriteLine(data_row);
    }

    private static string vector3ToString(Vector3 v3)
    {
        return string.Format("{0},{1},{2}", v3.x, v3.y, v3.z);
    }

    private static string vector2ToString(Vector2 v2)
    {
        return string.Format("{0},{1}", v2.x, v2.y);
    }

    void Calibration()
    {
        SRanipal_Eye_API.IsUserNeedCalibration(ref cal_need);           // Check the calibration status. If needed, we perform the calibration.

        if (cal_need == true)
        {
            result_cal = SRanipal_Eye_v2.LaunchEyeCalibration();

            if (result_cal == true)
            {
                Debug.Log("Calibration is done successfully.");
            }

            else
            {
                Debug.Log("Calibration is failed.");
                if (UnityEditor.EditorApplication.isPlaying)
                {
                    UnityEditor.EditorApplication.isPlaying = false;    // Stops Unity editor if the calibration if failed.
                }
            }
        }

        if (cal_need == false)
        {
            Debug.Log("Calibration is not necessary");
        }
    }



/*    public static GameObject Focus()
    {
        
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out testRay, out focusInfo)) { }
            else if (SRanipal_Eye_v2.Focus(GazeIndex.LEFT, out testRay, out focusInfo)) { }
            else if (SRanipal_Eye_v2.Focus(GazeIndex.RIGHT, out testRay, out focusInfo)) { }
            else return null;
        
        
            return focusInfo.collider.gameObject;
        
    }
    /// <summary>
    /// Checks name for current object in focus.
    /// </summary>
    public static string FocusName()
    {
        return Focus().name;
    }*/


}
