using System.Collections.Generic;
using Edia.Eye;
using UnityEngine;

/// <summary>
/// A client that processes eye-tracking data to update the position and rotation of a GameObject.
/// This class applies the eye pose (position and rotation) from the latest eye-tracking data sample
/// to the associated GameObject's transform in local space.
/// </summary>
public class EyeDataClientTrackedEyePoseDriver : EyeDataClient {

    /// <summary>
    /// If true, the local position of the GameObject will be updated based on the latest eye-tracking sample.
    /// </summary>
    [Tooltip("If true, the local position of the GameObject will be updated based on the latest eye-tracking sample.")]
    public bool ApplyPositionLocal = true;

    /// <summary>
    /// If true, the local rotation of the GameObject will be updated based on the latest eye-tracking sample.
    /// </summary>
    [Tooltip("If true, the local rotation of the GameObject will be updated based on the latest eye-tracking sample.")]
    public bool ApplyRotationLocal = true;

    /// <summary>
    /// Processes the current eye-tracking samples and updates the position and/or rotation of the GameObject accordingly.
    /// This method uses the latest eye data sample to apply position and rotation changes to the GameObject's transform.
    /// </summary>
    /// <param name="currentSamples">A list of the current eye-tracking data samples.</param>
    public override void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
        // If there are no samples, do nothing
        if (currentSamples.Count == 0) {
            return;
        }

        // Get the latest eye-tracking sample (most recent data)
        EyeDataPackage latestSample = currentSamples[^1]; // 'latestSample' is the last element in the list

        // If ApplyPositionLocal is enabled, update the GameObject's local position
        if (ApplyPositionLocal) {
            transform.localPosition = new Vector3(latestSample.position_x_local, latestSample.position_y_local,
                latestSample.position_z_local);
        }

        // If ApplyRotationLocal is enabled, update the GameObject's local rotation
        if (ApplyRotationLocal) {
            transform.localRotation = Quaternion.Euler(latestSample.rotation_x_local, latestSample.rotation_y_local,
                latestSample.rotation_z_local);
        }
    }
}
