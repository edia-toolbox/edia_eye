using System.Collections;
using System.Collections.Generic;
using Edia.Eye;
using UnityEngine;

public class TrackedEyePoseDriver : MonoBehaviour, IEyeDataClient {
    public bool ApplyPositionLocal = true;
    public bool ApplyRotationLocal = true;


    public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
        if (currentSamples.Count == 0) {
            return;
        }

        EyeDataPackage latestSample = currentSamples[^1];

        if (ApplyPositionLocal) {
            transform.localPosition = new Vector3(latestSample.position_x_local, latestSample.position_y_local,
                latestSample.position_z_local);
        }

        if (ApplyRotationLocal) {
            transform.localRotation = Quaternion.Euler(latestSample.rotation_x_local, latestSample.rotation_y_local,
                latestSample.rotation_z_local);
        }
    }
}