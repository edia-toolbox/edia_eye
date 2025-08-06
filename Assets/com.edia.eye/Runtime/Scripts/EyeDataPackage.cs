/*
    Data container for one frame of eye data coming from any eye tracker.
*/

namespace Edia.Eye
{
    /// <summary>
    /// Data container for one single frame of eye data from an eye tracker.
    /// This class holds various properties related to the eye's position, orientation,
    /// gaze direction, pupil size, and timestamps. It can be used to track eye movements,
    /// gaze points, and eye state (such as pupil diameter and openness) for further analysis.
    /// </summary>
    public class EyeDataPackage {
        
        // Default value for numeric fields
        static float defaultValue = 0f;

        /// <summary>
        /// Indicates if the data package is valid. 
        /// </summary>
        public bool isValid = false;

        /// <summary>
        /// Specifies which eye the data refers to. It can be "left", "right", or "center".
        /// </summary>
        public string eye = "";

        /// <summary>
        /// The x-component of the normalized gaze vector in the local space of the HMD.
        /// </summary>
        public float direction_x_local = defaultValue;

        /// <summary>
        /// The y-component of the normalized gaze vector in the local space of the HMD.
        /// </summary>
        public float direction_y_local = defaultValue;

        /// <summary>
        /// The z-component of the normalized gaze vector in the local space of the HMD.
        /// </summary>
        public float direction_z_local = defaultValue;

        /// <summary>
        /// The x-coordinate of the eye center in the local space of the HMD.
        /// </summary>
        public float position_x_local = defaultValue;

        /// <summary>
        /// The y-coordinate of the eye center in the local space of the HMD.
        /// </summary>
        public float position_y_local = defaultValue;

        /// <summary>
        /// The z-coordinate of the eye center in the local space of the HMD.
        /// </summary>
        public float position_z_local = defaultValue;

        /// <summary>
        /// The x rotation of the eye in the local space of the HMD, representing the eye's orientation.
        /// </summary>
        public float rotation_x_local = defaultValue;

        /// <summary>
        /// The y rotation of the eye in the local space of the HMD, representing the eye's orientation.
        /// </summary>
        public float rotation_y_local = defaultValue;

        /// <summary>
        /// The z rotation of the eye in the local space of the HMD, representing the eye's orientation.
        /// </summary>
        public float rotation_z_local = defaultValue;

        /// <summary>
        /// The x-coordinate of the intersection point of the gaze with an object in world space.
        /// </summary>
        public float intersection_x = defaultValue;

        /// <summary>
        /// The y-coordinate of the intersection point of the gaze with an object in world space.
        /// </summary>
        public float intersection_y = defaultValue;

        /// <summary>
        /// The z-coordinate of the intersection point of the gaze with an object in world space.
        /// </summary>
        public float intersection_z = defaultValue;

        /// <summary>
        /// The identifier of the object that was intersected by the gaze ray.
        /// </summary>
        public string target_id = "NA";

        /// <summary>
        /// The diameter of the pupil in millimeters.
        /// </summary>
        public float diameter = defaultValue;

        /// <summary>
        /// The first axis of the pupil ellipse, representing the diameter in millimeters.
        /// </summary>
        public float diameter_x = defaultValue;

        /// <summary>
        /// The second axis of the pupil ellipse, representing the diameter in millimeters.
        /// </summary>
        public float diameter_y = defaultValue;

        /// <summary>
        /// The confidence level in the reliability of this sample, ranging from 0 (low) to 1 (high).
        /// </summary>
        public float confidence = defaultValue;

        /// <summary>
        /// The timestamp from the eye tracker, in milliseconds.
        /// </summary>
        public double timestamp_et = (double)defaultValue;

        /// <summary>
        /// The timestamp in LSL (Lab Streaming Layer) time, representing the time when the sample was recorded, in seconds.
        /// </summary>
        public double timestamp_lsl = defaultValue;

        /// <summary>
        /// The openness of the eye, with a value ranging from 0 (closed) to 1 (fully open).
        /// </summary>
        public float openness = defaultValue;
    }
}