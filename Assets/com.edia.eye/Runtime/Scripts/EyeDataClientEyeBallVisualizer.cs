using Edia;
using Edia.Eye;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDataClientEyeBallVisualizer : MonoBehaviour, IEyeDataClient
{
    public Constants.EyeId Eye = Constants.EyeId.CENTER;
    public float Openness;
    Vector3 rotation;
    Vector3 position;
    Quaternion rotClosed = Quaternion.Euler(60, 0, 0);
    Quaternion rotOpen = Quaternion.Euler(0, 0, 0);

	public GameObject EyeLid;
	public GameObject EyeBall;
    public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
        foreach (var sample in currentSamples) {
            if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                Openness = sample.openness;
                rotation = new Vector3(sample.rotation_x_local, sample.rotation_y_local, sample.rotation_z_local);
                position = new Vector3(sample.position_x_local, sample.position_y_local, sample.position_z_local);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        EyeLid.transform.localRotation = Quaternion.Lerp(rotClosed, rotOpen, Openness);
        EyeLid.transform.localPosition = position;
        EyeBall.transform.localRotation = Quaternion.Euler(rotation);
        EyeBall.transform.localPosition = position;
    }
}
