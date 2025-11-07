using System.Collections.Generic;
using Edia.Eye;
using UnityEngine;

/// <summary>
/// A client that processes eye-tracking data to update the position and rotation of a GameObject.
/// This class applies the eye pose (position and rotation) from the latest eye-tracking data sample
/// to the associated GameObject's transform in local space.
/// </summary>
[EdiaHeader("EDIA EYE", "Local Pose Driver", "Updates local position and rotation from latest eye-tracking data sample.")]
public class EyeDataClientTrackedEyePoseDriver : EyeDataClient {

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

        EyeDataPackage latestSample = currentSamples[^1]; // 'latestSample' is the last element in the list
        
        transform.localPosition = new Vector3(latestSample.position_x_local, latestSample.position_y_local, latestSample.position_z_local);
        transform.localRotation = Quaternion.Euler(latestSample.rotation_x_local, latestSample.rotation_y_local, latestSample.rotation_z_local);
    }
}