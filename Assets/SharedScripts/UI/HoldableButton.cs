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
        private float angle;
        public float trolleyCurrentPosition;
        private float trolleyDistanceToConcrete;
        private float trolleyTargetPosition;
        private float trolleyMaxDistance;
        private float trolleyDistanceFromFar;
        private float concreteDistanceFromFar;
        private float startTrolleyLerpPosition;
        private float concreteLerpPositionTrolley;
        private float currentTrolleyLerpPosition;
        private float trolleyMovingDistanceToConcrete;
        private float currentTrolleyDistanceToConcrete;
        private float trolleyMovementIndex;
        private float cableLocation;
        private float cableMoveIndex;
        private float concreteDefaultHeight;

        private bool isPointerDown;
        public bool IsHeldDown => isPointerDown;
        private bool mouseDown;
        public bool facingTarget;
        public bool rotating;
        public bool trolleyMoving;
        public bool cableMoving;
        public bool concreteAttached;

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
        }

        public void Start()
        {

            trolleyNearOffset = crane.transform.position - trolleyNear.transform.position;
            trolleyFarOffset = crane.transform.position - trolleyFar.transform.position;
            cableOffset = trolley.transform.position - cable.transform.position;
            hookOffset = hook.transform.position - trolley.transform.position;
            cableLowLimit = 12;
            cableMoveIndex = 0.002083f;
            concreteDefaultHeight = concrete.transform.position.y;

            // Trolley calculations

            // Get concrete's distance from trolley far limit
            concreteDistanceFromFar = Vector3.Distance(crane.transform.position, trolleyFar.transform.position) - Vector3.Distance(new(crane.transform.position.x, concrete.transform.position.y, crane.transform.position.z), concrete.transform.position);

            // Get trolley's distance from trolley far limit
            trolleyDistanceFromFar = Vector3.Distance(trolleyFar.transform.position, trolley.transform.position);

            // Get trolley's max movement range)
            trolleyMaxDistance = Vector3.Distance(trolleyNear.transform.position, trolleyFar.transform.position);

            // Get trolley's lerp position at the start of the operation
            concreteLerpPositionTrolley = trolleyMaxDistance / concreteDistanceFromFar;

            // Get trolley's current lerp position
            currentTrolleyLerpPosition = trolleyDistanceFromFar / trolleyMaxDistance;

            // Update trolley's distance to concrete
            currentTrolleyDistanceToConcrete = concreteLerpPositionTrolley - currentTrolleyLerpPosition;

            print($"currentTrolleyLerpPosition: {currentTrolleyLerpPosition}, concreteLerpPosition: {concreteLerpPositionTrolley}");
        }

        public void LateUpdate()
        {
            cable.transform.localScale = new(1, 0 + 2 * cableLocation);

            // Get trolley's distance from lerp scale's 0 point
            trolleyDistanceFromFar = Vector3.Distance(trolleyFar.transform.position, trolley.transform.position);

            // Update trolley's distance from concrete
            currentTrolleyDistanceToConcrete = currentTrolleyLerpPosition - concreteLerpPositionTrolley;

            trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, currentTrolleyLerpPosition), crane.transform.rotation);

            // Attach cable to trolley
            cable.transform.SetPositionAndRotation(new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z), trolley.transform.rotation);

            // Attach hook to trolley
            hook.transform.SetPositionAndRotation(Vector3.Lerp(trolley.transform.position, new(trolley.transform.position.x, cableLowLimit, trolley.transform.position.z), cableLocation), cable.transform.rotation);

            // Attach trolley points to crane
            Vector3 trolleyNearPosition = crane.transform.TransformPoint(-trolleyNearOffset);
            trolleyNear.transform.SetPositionAndRotation(new(trolleyNearPosition.x, trolleyNear.transform.position.y, trolleyNearPosition.z), trolleyNear.transform.rotation);
            Vector3 trolleyFarPosition = crane.transform.TransformPoint(-trolleyFarOffset);
            trolleyFar.transform.SetPositionAndRotation(new(trolleyFarPosition.x, trolleyFar.transform.position.y, trolleyFarPosition.z), trolleyFar.transform.rotation);

            trolleyCurrentPosition = Vector3.Distance(trolleyNear.transform.position, trolley.transform.position) / Vector3.Distance(trolleyNear.transform.position, trolleyFar.transform.position);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
            }

            if (concreteGrab.concreteAttached || concreteAttached)
            {
                concrete.transform.SetLocalPositionAndRotation(new(hook.transform.localPosition.x, hook.transform.localPosition.y - 1.5f, hook.transform.localPosition.z), concrete.transform.localRotation);
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

            //print($"currentTrolleyDistanceToConcrete: {currentTrolleyDistanceToConcrete}, currentTrolleyLerpPosition: {currentTrolleyLerpPosition}, concreteLerpPosition: {concreteLerpPositionTrolley}");
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
            if (!rotating && !trolleyMoving && !cableMoving && !concreteAttached)
            {
                angle = Vector3.SignedAngle(new(hook.transform.position.x, 0, hook.transform.position.z), new(concrete.transform.position.x, 0, concrete.transform.position.z), Vector3.up);
                print(angle);
                if (angle < 0)
                {
                    //angle += 360;
                }

                else if (angle > 180)
                {

                }
                float angleCounter = angle;
                float rotationStep = angleCounter / 480;
                rotating = true;
                trolleyMoving = true;
                cableMoving = true;

                // Trolley calculations

                // Get trolley's distance
                trolleyDistanceFromFar = Vector3.Distance(trolleyFar.transform.position, trolley.transform.position);

                // Get concrete's distance
                concreteDistanceFromFar = Vector3.Distance(new(crane.transform.position.x, trolleyFar.transform.position.y, crane.transform.position.z), trolleyFar.transform.position) - Vector3.Distance(new(crane.transform.position.x, concrete.transform.position.y, crane.transform.position.z), concrete.transform.position);

                // Get trolley's lerp position at the start of the operation
                startTrolleyLerpPosition = trolleyDistanceFromFar / trolleyMaxDistance;

                // Get trolley's lerp position at the start of the operation
                concreteLerpPositionTrolley = concreteDistanceFromFar / trolleyMaxDistance;

                // Get the distance the trolley has to move for this operation
                trolleyMovingDistanceToConcrete = concreteLerpPositionTrolley - startTrolleyLerpPosition;

                // Get the movement index for the trolley
                trolleyMovementIndex = trolleyMovingDistanceToConcrete / 480;

                // Get trolley's current lerp position
                currentTrolleyLerpPosition = trolleyDistanceFromFar / trolleyMaxDistance;

                if (trolleyDistanceFromFar > concreteDistanceFromFar)
                {
                    //trolleyMovementIndex *= -1;
                    print($"The trolley is closer than the concrete. trolleyDistanceFromFar: {trolleyDistanceFromFar}, concreteDistanceFromFar: {concreteDistanceFromFar}");
                }

                // Update trolley's distance from concrete
                currentTrolleyDistanceToConcrete = currentTrolleyLerpPosition - concreteLerpPositionTrolley;

                print($"trolleyDistanceFromFar: {trolleyDistanceFromFar}, concreteDistanceFromFar: {concreteDistanceFromFar}, currentTrolleyDistanceToConcrete: {currentTrolleyDistanceToConcrete}");

                StartCoroutine(SpinCrane(angleCounter, rotationStep));
                StartCoroutine(MoveTrolley());
                StartCoroutine(MoveCable());

            }
        }

        public IEnumerator SpinCrane(float angleCounter, float rotationStep)
        {
            if (angleCounter < 0.5f && angleCounter > -0.5f)
            {
                facingTarget = true;
                angle = 0;
                rotating = false;
                angleCounter = 0;

                print("Stopping crane rotation");
            }

            else
            {
                if (angle < 0)
                {
                    crane.transform.Rotate(new(0, 1, 0), rotationStep);
                    angleCounter -= rotationStep;
                }

                else if (angle > 0)
                {
                    crane.transform.Rotate(new(0, 1, 0), rotationStep);
                    angleCounter -= rotationStep;
                    print($"angleCounter: {angleCounter}, rotationStep: {rotationStep}");
                }
                //print (angleCounter);

                print($"currentTrolleyDistanceToConcrete: {currentTrolleyDistanceToConcrete}, currentTrolleyLerpPosition: {currentTrolleyLerpPosition}");
                yield return new WaitForSecondsRealtime(0.008333f);
                StartCoroutine(SpinCrane(angleCounter, rotationStep));
            }
        }

        public IEnumerator MoveTrolley()
        {

            if (currentTrolleyDistanceToConcrete < 0.01f && currentTrolleyDistanceToConcrete > -0.01f)
            {
                print("Stopping trolley movement");
                trolleyMoving = false;
            }

            else
            {
                currentTrolleyLerpPosition += trolleyMovementIndex;
                //print($"currentTrolleyDistanceToConcrete: {currentTrolleyDistanceToConcrete}, trolleyCurrentPosition: {trolleyCurrentPosition}, trolleyTargetPosition: {trolleyTargetPosition}, currentTrolleyLerpPosition: {currentTrolleyLerpPosition}");

                yield return new WaitForSecondsRealtime(0.008333f);
                StartCoroutine(MoveTrolley());
            }
        }

        public IEnumerator MoveCable()
        {
            if (cableLocation <= 1 && cableLocation > 0.995f)
            {
                print("Stopping cable movement");
                cableMoving = false;
            }

            else
            {
                cableLocation += cableMoveIndex;
                //print($"cableLocation: {cableLocation}");

                yield return new WaitForSecondsRealtime(0.008333f);
                StartCoroutine(MoveCable());
            }
        }

        public IEnumerator LiftConcrete1()
        {
            yield return new WaitForSecondsRealtime(1);
            print("Lifting concrete");
            StartCoroutine(LiftConcrete2());
        }

        private IEnumerator LiftConcrete2()
        {
            if (cableLocation <= 0.1f)
            {
                print("Stopping cable movement");
                cableMoving = false;

                StartCoroutine(RespawnConcrete());
            }

            else
            {
                cableLocation -= cableMoveIndex;
                //print($"cableLocation: {cableLocation}");

                yield return new WaitForSecondsRealtime(0.008333f);
                StartCoroutine(LiftConcrete2());
            }
        }

        public IEnumerator RespawnConcrete()
        {
            yield return new WaitForSecondsRealtime(3);
            concreteAttached = false;
            concreteGrab.concreteAttached = false;
            print("Moving concrete");
            SpawnConcrete();
        }

        public void SpawnConcrete()
        {
            float spawnRange = Random.Range(Vector3.Distance(crane.transform.position, new(trolleyNear.transform.position.x, crane.transform.position.y, trolleyNear.transform.position.z)),
                                            Vector3.Distance(crane.transform.position, new(trolleyFar.transform.position.x, crane.transform.position.y, trolleyFar.transform.position.z)));

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 spawnLocation = new(Mathf.Cos(angle) * spawnRange, concreteDefaultHeight, Mathf.Sin(angle) * spawnRange);

            facingTarget = false;
            concrete.transform.position = spawnLocation;

            if (Vector3.Distance(new(crane.transform.position.x, concreteDefaultHeight, crane.transform.position.z), concrete.transform.position) > Vector3.Distance(new(crane.transform.position.x, concreteDefaultHeight, crane.transform.position.z), trolleyFar.transform.position))
            {
                print("Concrete spawned too far, repositioning...");
                SpawnConcrete();
            }
        }
    }
}