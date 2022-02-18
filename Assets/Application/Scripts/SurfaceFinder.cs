using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using UnityEngine.Events;

namespace LocalJoost.Examples
{
    public class SurfaceFinder : MonoBehaviour
    {
        [Tooltip("Surface magnetism component being used to control the process")]
        [SerializeField]
        private SurfaceMagnetism surfaceMagnet;
        
        [SerializeField]
        [Tooltip("Prompt to encourage the user to look at the floor")]
        private GameObject lookPrompt;

        [SerializeField]
        [Tooltip("Prompt to ask the user if this is indeed the floor")]
        private GameObject confirmPrompt;
        
        [SerializeField]
        [Tooltip("Sound that should be played when the conform prompt is displayed")]
        private AudioSource locationFoundSound;
        
        [SerializeField]
        [Tooltip("Triggered once when the location is accepted.")]
        private UnityEvent<MixedRealityPose> locationFound = new UnityEvent<MixedRealityPose>();
        
        private float delayMoment;
        private float initTime;
        private Vector3? foundPosition = null;
        private Vector3 previousPosition = Vector3.positiveInfinity;
        private SolverHandler solverHandler;
        private RadialView radialView;
        
        private void Awake()
        {
            solverHandler = surfaceMagnet.GetComponent<SolverHandler>();
            radialView = surfaceMagnet.GetComponent<RadialView>();
            surfaceMagnet.enabled = false;
            radialView.enabled = true;
            initTime = Time.time + 2;
        }

        private void OnEnable()
        {
            Reset();
        }

        private void Update()
        {
            CheckLocationOnSurface();
        }

        public void Reset()
        {
            previousPosition = Vector3.positiveInfinity;
            delayMoment = Time.time + 2;
            foundPosition = null;
            lookPrompt.SetActive(true);
            confirmPrompt.SetActive(false);
            solverHandler.enabled = true;
        }

        public void Accept()
        {
            if (foundPosition != null)
            {
                locationFound?.Invoke(new MixedRealityPose(
                    foundPosition.Value, solverHandler.transform.rotation));
                lookPrompt.SetActive(false);
                confirmPrompt.SetActive(false);
                gameObject.SetActive(false);
            }
        }
        
        private void CheckLocationOnSurface()
        {
            if (Time.time > initTime && radialView.enabled)
            {
                radialView.enabled = false;
                surfaceMagnet.enabled = true;
                delayMoment = Time.time + 2;
            }
            
            if (foundPosition == null && Time.time > delayMoment)
            {
                if (surfaceMagnet.OnSurface)
                {
                    foundPosition = surfaceMagnet.transform.position;
                }
                
                if (foundPosition != null)
                {
                    var isMoving = Mathf.Abs((previousPosition - foundPosition.Value).magnitude) > 0.005;
                    previousPosition = foundPosition.Value;
                    if( !isMoving )
                    {
                        solverHandler.enabled = false;
                        lookPrompt.SetActive(false);
                        confirmPrompt.SetActive(true);
                        locationFoundSound.Play();
                    }
                    else
                    {
                        foundPosition = null;
                    }
                }
            }
        }
    }
}