using GameMath.UI;
using UnityEngine;

public class ConcreteGrab : MonoBehaviour
{
    private GameObject concrete;
    public bool concreteAttached;
    public HoldableButton holdableButton;

    // Start is called before the first frame update
    void Start()
    {
        concrete = GameObject.Find("Concrete");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Concrete")
        {
            concreteAttached = true;
            holdableButton.concreteAttached = true;
            StartCoroutine(holdableButton.LiftConcrete1());
            holdableButton.cableMoving = true;
            print("ConcreteGrab concrete hit");
        }
    }
}
