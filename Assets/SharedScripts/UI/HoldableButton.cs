using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMath.UI
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // Crane objects
        private GameObject crane;
        private GameObject trolley;
        private GameObject cable;
        private GameObject hook;
        private GameObject concrete;
        private GameObject trolleyFar;
        private GameObject trolleyNear;

        // UI objects
        private GameObject buttonLeft;
        private GameObject buttonRight;
        private GameObject trolleySlider;
        private GameObject cableSlider;

        // Coordinate variables
        public Vector3 trolleyOffset;
        public Vector3 trolleyFarOffset;
        public Vector3 trolleyNearOffset;
        public Vector3 cableOffset;
        public Vector3 hookOffset;

        public int cableLowLimit;
        public float rotationSpeed = 5f;
        public float trolleySliderLocation;
        public float cableSliderLocation;
        public Vector3 trolleyPositionTarget;

        private bool isPointerDown;

        public bool IsHeldDown => isPointerDown;

        public void Start()
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

            trolleyOffset = crane.transform.position - trolley.transform.position;
            trolleyNearOffset = crane.transform.position - trolleyNear.transform.position;
            trolleyFarOffset = crane.transform.position - trolleyFar.transform.position;
            cableOffset = trolley.transform.position - cable.transform.position;
            hookOffset = hook.transform.position - trolley.transform.position;
            cableLowLimit = 13;
        }

        public void Update()
        {
            trolley.transform.position = Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleySliderLocation);
            cable.transform.localScale = new(1, 0 + 2 * cableSliderLocation);
            cable.transform.position = new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z);
            hook.transform.position = Vector3.Lerp(trolley.transform.position, new(trolley.transform.position.x, cableLowLimit, trolley.transform.position.z), cableSliderLocation);
            
            if (isPointerDown)
            {
                //GetTrolleyOffset(crane, trolley);
                //trolleyOffset = crane.transform.position - trolley.transform.position;


                // trolley.transform.position = new Vector3(Mathf.Cos(orbitAngleRadians) * orbitRadius, 0f, Mathf.Sin(orbitAngleRadians) * orbitRadius);
                //print("Trolley position " + trolley.transform.position);
                

                if (isPointerDown && gameObject.name == buttonLeft.name)
                {
                    crane.transform.Rotate(0, -5 * Time.deltaTime, 0);

                    // Attach cable to trolley
                    Vector3 cablePosition = trolley.transform.TransformPoint(cableOffset);
                    cable.transform.SetPositionAndRotation(new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z), trolley.transform.rotation);

                    // Attach hook to trolley
                    Vector3 hookPosition = cable.transform.TransformPoint(hookOffset);
                    hook.transform.SetPositionAndRotation(new(hookPosition.x, hookPosition.y, hookPosition.z), cable.transform.rotation);

                    // Attach trolley points to crane
                    Vector3 trolleyNearPosition = crane.transform.TransformPoint(-trolleyNearOffset);
                    trolleyNear.transform.SetPositionAndRotation(new(trolleyNearPosition.x, trolleyNear.transform.position.y, trolleyNearPosition.z), trolleyNear.transform.rotation);
                    Vector3 trolleyFarPosition = crane.transform.TransformPoint(-trolleyFarOffset);
                    trolleyFar.transform.SetPositionAndRotation(new(trolleyFarPosition.x, trolleyFar.transform.position.y, trolleyFarPosition.z), trolleyFar.transform.rotation);

                    // Attach trolley to crane
                    trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleySliderLocation), crane.transform.rotation);
                }

                else if (isPointerDown && gameObject.name == buttonRight.name)
                {
                    crane.transform.Rotate(0, 5 * Time.deltaTime, 0);

                    // Attach cable to trolley
                    Vector3 cablePosition = trolley.transform.TransformPoint(cableOffset);
                    cable.transform.SetPositionAndRotation(new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z), trolley.transform.rotation);

                    // Attach hook to trolley
                    Vector3 hookPosition = cable.transform.TransformPoint(hookOffset);
                    hook.transform.SetPositionAndRotation(new(hookPosition.x, hook.transform.position.y, hookPosition.z), cable.transform.rotation);

                    // Attach trolley points to crane
                    Vector3 trolleyNearPosition = crane.transform.TransformPoint(-trolleyNearOffset);
                    trolleyNear.transform.SetPositionAndRotation(new(trolleyNearPosition.x, trolleyNear.transform.position.y, trolleyNearPosition.z), trolleyNear.transform.rotation);
                    Vector3 trolleyFarPosition = crane.transform.TransformPoint(-trolleyFarOffset);
                    trolleyFar.transform.SetPositionAndRotation(new(trolleyFarPosition.x, trolleyFar.transform.position.y, trolleyFarPosition.z), trolleyFar.transform.rotation);

                    // Attach trolley to crane
                    trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleySliderLocation), crane.transform.rotation);
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

        public void GetTrolleyOffset(GameObject crane, GameObject trolley)
        {
            trolleyOffset = new(crane.transform.position.x - trolley.transform.position.x, trolley.transform.position.y, crane.transform.position.z - trolley.transform.position.z);
        }

        public void TrolleySliderLocation()
        {
            trolleySliderLocation = trolleySlider.GetComponent<Slider>().value;
        }

        public void CableSliderLocation()
        {
            cableSliderLocation = cableSlider.GetComponent<Slider>().value;
        }
    }
}