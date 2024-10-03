using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConcreteGrab : MonoBehaviour
{
    private GameObject concrete;
    public bool concreteAttached;

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
            print("ConcreteGrab concrete hit");
        }
    }
}
