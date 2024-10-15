using System.Collections;
using GameMath.Cameras;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMath.UI
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public ConcreteGrab concreteGrab;

        // Game objects
        public CameraController cameraController;
        private GameObject crane;
        private GameObject trolley;
        private GameObject cable;
        private GameObject hook;
        public GameObject concrete;
        private GameObject trolleyFar;
        private GameObject trolleyNear;
        public GameObject camera1;
        public GameObject camera2;
        public GameObject camera3;
        public GameObject activeCamera;

        // UI objects
        private GameObject buttonLeft;
        private GameObject buttonRight;
        private GameObject trolleySlider;
        private GameObject cableSlider;

        // Coordinate variables
        public Vector3 trolleyFarOffset;
        public Vector3 trolleyNearOffset;
        public Vector3 cableOffset;
        public Vector3 hookOffset;

        public int cableLowLimit;
        public float rotationSpeed = 5f;
        public float trolleySliderLocation;
        public float cableSliderLocation;
        public Vector3 trolleyPositionTarget;
        private float angle;
        private float distanceConcrete;
        private float distanceTrolley;
        private float distanceTrolleyToConcrete;
        public float trolleyCurrentPosition;
        private float trolleyDistanceToConcrete;
        private float trolleyTargetPosition;
        private float trolleyMaxDistance;
        private float trolleyDistanceFromNear;
        private float concreteDistanceFromNear;
        private float startTrolleyLerpPosition;
        private float concreteLerpPosition;
        private float currentTrolleyLerpPosition;
        private float trolleyMovingDistanceToConcrete;
        private float currentTrolleyDistanceToConcrete;

        private bool isPointerDown;
        public bool IsHeldDown => isPointerDown;
        private bool mouseDown;
        public bool facingTarget;
        public bool rotating;
        public bool trolleyMoving;
        public bool cableMoving;

        public void Awake()
        {
            crane = GameObject.Find("Tower Crane");
            trolley = GameObject.Find("Trolley");
            cable = GameObject.Find("Cable");
            hook = GameObject.Find("Hook");
            concrete = GameObject.Find("Concrete");

            buttonLeft = GameObject.Find("Button");
            buttonRight = GameObject.Find("Button (1)");
            trolleySlider = GameObject.Find("Trolley Slider");
            cableSlider = GameObject.Find("Cable Slider");
            trolleyFar = GameObject.Find("Trolley Far Limit");
            trolleyNear = GameObject.Find("Trolley Near Limit");

            camera1 = GameObject.Find("Main Camera");
            camera2 = GameObject.Find("Top-down Camera");
            camera3 = GameObject.Find("Side-way Camera");

            activeCamera = camera1;
        }

        public void Start()
        {

            trolleyNearOffset = crane.transform.position - trolleyNear.transform.position;
            trolleyFarOffset = crane.transform.position - trolleyFar.transform.position;
            cableOffset = trolley.transform.position - cable.transform.position;
            hookOffset = hook.transform.position - trolley.transform.position;
            cableLowLimit = 12;
        }

        public void Update()
        {
            cable.transform.localScale = new(1, 0 + 2 * cableSliderLocation);

            if (!rotating)
            {
                // Attach trolley to crane
                //trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleySliderLocation), crane.transform.rotation);
            }

            // Get trolley's current lerp position
            currentTrolleyLerpPosition = trolleyDistanceFromNear / trolleyMaxDistance;
            
            // Update trolley's distance from concrete
            currentTrolleyDistanceToConcrete = currentTrolleyLerpPosition - concreteLerpPosition;
            
            trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleyDistanceToConcrete), crane.transform.rotation);

            // Attach cable to trolley
            cable.transform.SetPositionAndRotation(new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z), trolley.transform.rotation);

            // Attach hook to trolley
            hook.transform.SetPositionAndRotation(Vector3.Lerp(trolley.transform.position, new(trolley.transform.position.x, cableLowLimit, trolley.transform.position.z), cableSliderLocation), cable.transform.rotation);

            // Attach trolley points to crane
            Vector3 trolleyNearPosition = crane.transform.TransformPoint(-trolleyNearOffset);
            trolleyNear.transform.SetPositionAndRotation(new(trolleyNearPosition.x, trolleyNear.transform.position.y, trolleyNearPosition.z), trolleyNear.transform.rotation);
            Vector3 trolleyFarPosition = crane.transform.TransformPoint(-trolleyFarOffset);
            trolleyFar.transform.SetPositionAndRotation(new(trolleyFarPosition.x, trolleyFar.transform.position.y, trolleyFarPosition.z), trolleyFar.transform.rotation);
            
            // Update distance from crane to trolley and concrete
            distanceConcrete = Vector3.Distance(crane.transform.position, concrete.transform.position);
            distanceTrolley = Vector3.Distance(crane.transform.position, trolley.transform.position);
            
            trolleyCurrentPosition = Vector3.Distance(trolleyNear.transform.position, trolley.transform.position) / Vector3.Distance(trolleyNear.transform.position, trolleyFar.transform.position);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
            }

            if (concreteGrab.concreteAttached)
            {
                concrete.transform.SetLocalPositionAndRotation(new(hook.transform.localPosition.x, hook.transform.localPosition.y - 2.5f, hook.transform.localPosition.z), hook.transform.localRotation);
            }

            if (isPointerDown)
            {
                if (isPointerDown && gameObject.name == buttonLeft.name)
                {
                    crane.transform.Rotate(0, -5 * Time.deltaTime, 0);
                }

                else if (isPointerDown && gameObject.name == buttonRight.name)
                {
                    crane.transform.Rotate(0, 5 * Time.deltaTime, 0);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void TrolleySliderLocation()
        {
            trolleySliderLocation = trolleySlider.GetComponent<UnityEngine.UI.Slider>().value;
        }

        public void CableSliderLocation()
        {
            cableSliderLocation = cableSlider.GetComponent<UnityEngine.UI.Slider>().value;
        }

        public void GrabConcrete()
        {
            if (!rotating && !trolleyMoving && !cableMoving)
            {
                angle = Vector3.SignedAngle(new(hook.transform.position.x, 0, hook.transform.position.z), new(concrete.transform.position.x, 0, concrete.transform.position.z), Vector3.up);
                float angleCounter = angle;
                facingTarget = false;
                rotating = true;
                trolleyMoving = true;
                //cableMoving = true;
                StartCoroutine(SpinCrane(angleCounter));

                distanceTrolleyToConcrete = distanceTrolley / distanceConcrete;
                //float distance = Vector3.Distance(trolley.transform.position, concrete.transform.position);



                // Current WIP

                // Idea: Get the positions on the lerp scale, get the distance between trolley and concrete, and move trolley towards concrete until their coordinates match.
                // Example: Trolley at lerp 0.24, concrete at lerp 0.64. Trolley gets set to move towards concrete by the distance (0.4) divided by 25, the current movement index at intervals of 0.08 seconds.

                // Get the trolley's target position, at the x/z coordinates of concrete (Not sure if working right atm)
                trolleyTargetPosition = Vector3.Distance(trolleyNear.transform.position, concrete.transform.position) / Vector3.Distance(trolleyNear.transform.position, trolleyFar.transform.position);

                // Get trolley's distance to the concrete at the start of the procedure
                float trolleyDistanceToConcreteStart = trolleyCurrentPosition / trolleyTargetPosition;

                // Get trolley's distance to the concrete
                trolleyDistanceToConcrete = trolleyDistanceToConcreteStart;

                // Get trolley's max distance on the lerp scale (0-1, being 1 in this case)
                trolleyMaxDistance = Vector3.Distance(trolleyNear.transform.position, trolleyFar.transform.position);

                // Get trolley's distance from lerp scale's 0 point
                trolleyDistanceFromNear = Vector3.Distance(trolleyNear.transform.position, trolley.transform.position);
                
                // Get concrete's distance from lerp scale's 0 point
                concreteDistanceFromNear = Vector3.Distance(trolleyNear.transform.position, concrete.transform.position);

                // Get trolley's lerp position at the start of the operation
                startTrolleyLerpPosition = trolleyDistanceFromNear / trolleyMaxDistance;

                // Get trolley's lerp position at the start of the operation
                concreteLerpPosition = concreteDistanceFromNear / trolleyMaxDistance;

                // Get the distance the trolley has to move for this operation
                trolleyMovingDistanceToConcrete = startTrolleyLerpPosition - concreteLerpPosition;
                




                if (trolleyTargetPosition < trolleyCurrentPosition)
                {
                    trolleyDistanceToConcrete *= -1;
                }
                
                float movementIndex = trolleyDistanceToConcrete/25;

                StartCoroutine(MoveTrolley(movementIndex));

                print("Grabbing concrete");
            }
        }

        public IEnumerator SpinCrane(float angleCounter)
        {
            if (angleCounter < 0.25f && angleCounter > -0.25f)
            {
                facingTarget = true;
                angle = 0;
                rotating = false;
                angleCounter = 0;

                print("Stopping crane rotation");
                StopCoroutine(nameof(SpinCrane));
            }

            else
            {
                if (angle > 0)
                {
                    crane.transform.Rotate(new(0, 1, 0), 0.1f);
                    angleCounter -= 0.1f;
                }
                else
                {
                    crane.transform.Rotate(new(0, -1, 0), 0.1f);
                    angleCounter += 0.1f;
                }

                yield return new WaitForSecondsRealtime(0.005f);
                StartCoroutine(SpinCrane(angleCounter));
            }
        }

        public IEnumerator MoveTrolley(float movementIndex)
        {
            // Change these values
            if (trolleyDistanceToConcrete < 1.01f && trolleyDistanceToConcrete > 0.99f)
            {
                print("Stopping trolley movement");
                StopCoroutine(nameof(MoveTrolley));
                cableMoving = false;
                trolleyMoving = false;
            }

            else
            {
                yield return new WaitForSecondsRealtime(0.08f);
                TrolleyMovement(movementIndex);
            }
        }

        private void TrolleyMovement(float movementIndex)
        {
            trolleyDistanceToConcrete += movementIndex;
            print($"trolleyDistanceToConcrete: {trolleyDistanceToConcrete}, trolleyCurrentPosition: {trolleyCurrentPosition}, trolleyTargetPosition: {trolleyTargetPosition}");
            StartCoroutine(MoveTrolley(movementIndex));
        }
    }
}