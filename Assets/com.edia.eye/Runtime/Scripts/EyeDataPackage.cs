/*
    Data container for one frame of eye data coming from any eye tracker.
*/

namespace Edia.Eye
{
    public class EyeDataPackage {
        public string eye = "NA"; // Which eye the package is referring to (can be left, right, or center)
        public float direction_x_local = float.NaN; // x element of normalized gaze vector in local space of HMD
        public float direction_y_local = float.NaN; // y element of normalized gaze vector in local space of HMD
        public float direction_z_local = float.NaN; // z element of normalized gaze vector in local space of HMD
        public float position_x_local = float.NaN; // x coordinate of eye center vector in local space of HMD
        public float position_y_local = float.NaN; // y coordinate of eye center vector in local space of HMD
        public float position_z_local = float.NaN; // z coordinate of eye center vector in local space of HMD
        public float rotation_x_local = float.NaN; // x rotation of the eye in local space of HMD
        public float rotation_y_local = float.NaN; // y rotation of the eye in local space of HMD
        public float rotation_z_local = float.NaN; // z rotation of the eye in local space of HMD
        public float intersection_x = float.NaN; // x position of the intersection point with a plane in world space
        public float intersection_y = float.NaN; // y position of the intersection point with a plane in world space
        public float intersection_z = float.NaN; // z position of the intersection point with a plane in world space
        public float diameter = float.NaN; // pupil diameter of the eye
        public float diameter_x = float.NaN; // first axis of pupil ellipse
        public float diameter_y = float.NaN; // second axis of pupil ellipse
        public float confidence = float.NaN; // confidence in reliability of this sample (ideally in [0;1])
        public float timestamp_et = float.NaN; // timestamp provided by the eye tracker
        public double timestamp_lsl = float.NaN; // timestamp provided by Time.realtimeSinceStartup
        public float openness = float.NaN; // openness value provided by SDK
        public string target_id = "NA"; // identifier of the object that was intersected by the gaze ray
    }
}