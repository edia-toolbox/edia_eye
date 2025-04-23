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
        public float direction_y_local = defaultValue; // y element
        public float direction_z_local = defaultValue; // z element
        public float position_x_local = defaultValue; // x coordinate of eye center in local space of HMD
        public float position_y_local = defaultValue; // y coordinate
        public float position_z_local = defaultValue; // z coordinate
        public float rotation_x_local = defaultValue; // x rotation of the eye in local space of HMD
        public float rotation_y_local = defaultValue; // y rotation
        public float rotation_z_local = defaultValue; // z rotation
        public float intersection_x = defaultValue; // x position of the intersection point with a plane in world space
        public float intersection_y = defaultValue; // y
        public float intersection_z = defaultValue; // z
        public string target_id = ""; // identifier of the object that was intersected by the gaze ray
        public float diameter = defaultValue; // pupil diameter of the eye [in mm]
        public float diameter_x = defaultValue; // first axis of pupil ellipse [in mm]
        public float diameter_y = defaultValue; // second axis of pupil ellipse [in mm]
        public float confidence = defaultValue; // confidence in reliability of this sample [0-1]
        public double timestamp_et = (double)defaultValue; // timestamp provided by the eye tracker [in ms]
        public double timestamp_lsl = defaultValue; // timestamp in LSL time (approx) at the moment of sample recording [in s]
        public float openness = defaultValue; // eye openness value provided by SDK [0-1]
    }
}