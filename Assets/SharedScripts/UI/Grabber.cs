using GameMath.UI;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public HoldableButton holdableButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        holdableButton.GrabConcrete();
        print("Calling GrabConcrete");
    }
}
