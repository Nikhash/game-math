using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMath.UI
{
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public ConcreteGrab concreteGrab;

        // Game objects
        private GameObject crane;
        private GameObject trolley;
        private GameObject cable;
        private GameObject hook;
        private GameObject concrete;
        private GameObject trolleyFar;
        private GameObject trolleyNear;
        private Camera camera;

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
        private float clickDetectionRange = 44;

        private bool isPointerDown;
        public bool IsHeldDown => isPointerDown;
        private bool mouseDown;

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

            trolleyNearOffset = crane.transform.position - trolleyNear.transform.position;
            trolleyFarOffset = crane.transform.position - trolleyFar.transform.position;
            cableOffset = trolley.transform.position - cable.transform.position;
            hookOffset = hook.transform.position - trolley.transform.position;
            cableLowLimit = 12;
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        public void Update()
        {
            cable.transform.localScale = new(1, 0 + 2 * cableSliderLocation);

            // Attach trolley to crane
            trolley.transform.SetPositionAndRotation(Vector3.Lerp(trolleyFar.transform.position, trolleyNear.transform.position, trolleySliderLocation), crane.transform.rotation);

            // Attach cable to trolley
            cable.transform.SetPositionAndRotation(new(trolley.transform.position.x, cable.transform.position.y, trolley.transform.position.z), trolley.transform.rotation);

            // Attach hook to trolley
            hook.transform.SetPositionAndRotation(Vector3.Lerp(trolley.transform.position, new(trolley.transform.position.x, cableLowLimit, trolley.transform.position.z), cableSliderLocation), cable.transform.rotation);

            // Attach trolley points to crane
            Vector3 trolleyNearPosition = crane.transform.TransformPoint(-trolleyNearOffset);
            trolleyNear.transform.SetPositionAndRotation(new(trolleyNearPosition.x, trolleyNear.transform.position.y, trolleyNearPosition.z), trolleyNear.transform.rotation);
            Vector3 trolleyFarPosition = crane.transform.TransformPoint(-trolleyFarOffset);
            trolleyFar.transform.SetPositionAndRotation(new(trolleyFarPosition.x, trolleyFar.transform.position.y, trolleyFarPosition.z), trolleyFar.transform.rotation);

            if (Input.GetMouseButtonDown(0) && !mouseDown)
            {
                mouseDown = true;
                Vector3 screenPoint = Input.mousePosition;
                Ray ray = camera.ScreenPointToRay(screenPoint);

                float distanceToObject = Vector3.Distance(ray.origin, concrete.transform.position);

                //print("distance:" + distanceToObject + "ray:" + ray.origin + "concrete:" + concrete.transform.position);

                if (distanceToObject < clickDetectionRange)
                {
                    print("Concrete detected");
                }
            }

            if (Input.GetMouseButtonUp(0) && mouseDown)
            {
                mouseDown = false;
            }

            if (concreteGrab.concreteAttached)
            {
                concrete.transform.SetLocalPositionAndRotation(new(hook.transform.localPosition.x, hook.transform.localPosition.y-2.5f, hook.transform.localPosition.z), hook.transform.localRotation);
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
    }
}