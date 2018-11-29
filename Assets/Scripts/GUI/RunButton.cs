using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class RunButton : MonoBehaviour
{
    public KeyCode keyPress;
    void Update()
    {
        if (Input.GetKeyDown(keyPress))
            transform.GetComponent<Button>().onClick.Invoke();
    }
}

