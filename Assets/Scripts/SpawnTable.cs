using UnityEngine;

/// <summary>
/// Place an anchor prefab at the right-hand controller pose
/// whenever the user presses the primary index trigger.
/// </summary>
/// 


namespace Meta.XR.MRUtilityKit
{
    public class SpawnTable : MonoBehaviour
    {
        /* ───── 1. REFERENCES ───── */
        private EnvironmentRaycastManager _raycastManager;          // added in Awake
        [SerializeField] private Transform _rightControllerAnchor;  // OVRCameraRig/RightControllerAnchor
        [SerializeField] private GameObject _cubePrefab;            // drag your prefab here
        private GameObject spawnedCube;   // keeps reference to the only instance
        private bool locked = false;        // becomes true after pressing A
        private bool disableTutorial = false;
        public GameObject tutorialController;
        /* ───── 2. INITIALISE ───── */
        private void Awake()
        {
            // Adds the manager automatically if the scene does not already contain one
            _raycastManager = gameObject.AddComponent<EnvironmentRaycastManager>();
        }

        /* ───── 3. MAIN LOOP ───── */
        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) locked = !locked;
            if (OVRInput.GetDown(OVRInput.RawButton.X)) disableTutorial = !disableTutorial;
            if (disableTutorial) tutorialController.SetActive(false);
            if (!disableTutorial) tutorialController.SetActive(true);
            if (locked) return;
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                var ray = new Ray(_rightControllerAnchor.position,
                               _rightControllerAnchor.forward);

                if (_raycastManager.Raycast(ray, out var hit))
                {
                    // reuse existing cube if it already exists
                    if (spawnedCube == null)
                        spawnedCube = Instantiate(_cubePrefab);

                    spawnedCube.transform.SetPositionAndRotation(
                        hit.point,
                        Quaternion.LookRotation(hit.normal, Vector3.up)
                    );

                }
            }
            
        }
    }
}