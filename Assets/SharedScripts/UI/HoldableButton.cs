using UnityEngine;
using UnityEngine.EventSystems;

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

        // UI objects
        private GameObject buttonLeft;
        private GameObject buttonRight;

        // Coordinate variables
        public Vector3 trolleyOffset;
        public Vector3 cableOffset;
        public Vector3 hookOffset;
        public float rotationSpeed = 5f;

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

            trolleyOffset = crane.transform.position - trolley.transform.position;
            cableOffset = trolley.transform.position - cable.transform.position;
            hookOffset = hook.transform.position - trolley.transform.position;
        }

        public void Update()
        {
            //Vector3 targetPosition = crane.transform.TransformPoint(offset);
            //trolley.transform.position = targetPosition;
            //Quaternion targetRotation = Quaternion.LookRotation(offset) * crane.transform.rotation;
            //trolley.transform.rotation = Quaternion.Slerp(trolley.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (isPointerDown)
            {
                //GetTrolleyOffset(crane, trolley);


                // trolley.transform.position = new Vector3(Mathf.Cos(orbitAngleRadians) * orbitRadius, 0f, Mathf.Sin(orbitAngleRadians) * orbitRadius);
                print("Trolley position " + trolley.transform.position);

                if (isPointerDown && gameObject.name == buttonLeft.name)
                {
                    crane.transform.Rotate(0, -5 * Time.deltaTime, 0);

                    // Attach trolley to crane
                    Vector3 trolleyPosition = crane.transform.TransformPoint(-trolleyOffset);
                    trolley.transform.SetPositionAndRotation(new(trolleyPosition.x, trolley.transform.position.y, trolleyPosition.z), crane.transform.rotation);

                    // Attach cable to trolley
                    Vector3 cablePosition = trolley.transform.TransformPoint(cableOffset);
                    cable.transform.SetPositionAndRotation(new(cablePosition.x, cable.transform.position.y, cablePosition.z), trolley.transform.rotation);
                    
                    // Attach hook to trolley
                    Vector3 hookPosition = cable.transform.TransformPoint(hookOffset);
                    hook.transform.SetPositionAndRotation(new(hookPosition.x, hook.transform.position.y, hookPosition.z), cable.transform.rotation);
                }

                else if (isPointerDown && gameObject.name == buttonRight.name)
                {
                    crane.transform.Rotate(0, 5 * Time.deltaTime, 0);

                    //  Attach trolley to crane
                    Vector3 targetPosition = crane.transform.TransformPoint(-trolleyOffset);
                    trolley.transform.SetPositionAndRotation(new(targetPosition.x, trolley.transform.position.y, targetPosition.z), crane.transform.rotation);
                    
                    // Attach cable to trolley
                    Vector3 cablePosition = trolley.transform.TransformPoint(cableOffset);
                    cable.transform.SetPositionAndRotation(new(cablePosition.x, cable.transform.position.y, cablePosition.z), trolley.transform.rotation);
                    
                    // Attach hook to trolley
                    Vector3 hookPosition = cable.transform.TransformPoint(hookOffset);
                    hook.transform.SetPositionAndRotation(new(hookPosition.x, hook.transform.position.y, hookPosition.z), cable.transform.rotation);
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
    }
}