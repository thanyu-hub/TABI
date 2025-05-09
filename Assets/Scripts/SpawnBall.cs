using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBall : MonoBehaviour
{
    public GameObject prefab;
    public float spawnSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach (OVRInput.Button button in System.Enum.GetValues(typeof(OVRInput.Button)))
        {
            if (OVRInput.GetDown(button))
            {
                Debug.Log("Button pressed: " + button);
            }
        }

        float indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        Debug.Log("PrimaryIndexTrigger value: " + indexTrigger);
    }
}
