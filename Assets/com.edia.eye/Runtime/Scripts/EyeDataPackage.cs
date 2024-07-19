/*
    Data container for one frame of eye data coming from any eye tracker.
*/

namespace Edia.Eye
{
    public class EyeDataPackage {
        static float defaultValue = 0f;

        public bool isValid = false;
        public string eye = ""; // Which eye the package is referring to (can be left, right, or center)
        public float direction_x_local = defaultValue; // x element of normalized gaze vector in local space of HMD
        public float direction_y_local = defaultValue; // y element of normalized gaze vector in local space of HMD
        public float direction_z_local = defaultValue; // z element of normalized gaze vector in local space of HMD
        public float position_x_local = defaultValue; // x coordinate of eye center vector in local space of HMD
        public float position_y_local = defaultValue; // y coordinate of eye center vector in local space of HMD
        public float position_z_local = defaultValue; // z coordinate of eye center vector in local space of HMD
        public float rotation_x_local = defaultValue; // x rotation of the eye in local space of HMD
        public float rotation_y_local = defaultValue; // y rotation of the eye in local space of HMD
        public float rotation_z_local = defaultValue; // z rotation of the eye in local space of HMD
        public float intersection_x = defaultValue; // x position of the intersection point with a plane in world space
        public float intersection_y = defaultValue; // y position of the intersection point with a plane in world space
        public float intersection_z = defaultValue; // z position of the intersection point with a plane in world space
        public float diameter = defaultValue; // pupil diameter of the eye
        public float diameter_x = defaultValue; // first axis of pupil ellipse
        public float diameter_y = defaultValue; // second axis of pupil ellipse
        public float confidence = defaultValue; // confidence in reliability of this sample (ideally in [0;1])
        public float timestamp_et = defaultValue; // timestamp provided by the eye tracker
        public double timestamp_lsl = defaultValue; // timestamp provided by Time.realtimeSinceStartup
        public float openness = defaultValue; // openness value provided by SDK
        public string target_id = ""; // identifier of the object that was intersected by the gaze ray
    }
}