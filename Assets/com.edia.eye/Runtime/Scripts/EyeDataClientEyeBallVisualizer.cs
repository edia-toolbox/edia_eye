using Edia;
using Edia.Eye;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeDataClientEyeBallVisualizer : MonoBehaviour, IEyeDataClient
{

    public Constants.EyeId Eye = Constants.EyeId.CENTER;
    public float openness;
    Vector3 rotation;
    Vector3 position;
    Quaternion rotClosed = Quaternion.Euler(0, 180, 0);
    Quaternion rotOpen = Quaternion.Euler(90, 180, 0);

    GameObject eyeLid;
    GameObject eyeBall;

    // Start is called before the first frame update
    void Start()
    {
        eyeLid = transform.Find("EyeLid").gameObject;
        eyeBall = transform.Find("EyeBall").gameObject;
    }

    public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
        foreach (var sample in currentSamples) {
            if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                openness = sample.openness;
                rotation = new Vector3(sample.rotation_x_local, sample.rotation_y_local, sample.rotation_z_local);
                position = new Vector3(sample.position_x_local, sample.position_y_local, sample.position_z_local);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        eyeLid.transform.localRotation = Quaternion.Lerp(rotClosed, rotOpen, openness);
        eyeLid.transform.localPosition = position;
        eyeBall.transform.localRotation = Quaternion.Euler(rotation);
        eyeBall.transform.localPosition = position;
    }
}
