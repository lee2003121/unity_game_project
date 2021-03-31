using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class myScript : MonoBehaviour
{
    protected Joystick joystick;
    protected JoyButton joybutton;
    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<JoyButton>();
    }

    // Update is called once per frame
    void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(joystick.Horizontal * 3f, rigidbody.velocity.y, joystick.Vertical * 3f);
    }

    private void OnTriggerEnter(Collider colider)
    {
        if (colider.gameObject.tag == "FinishLine")
        {
            SceneManager.LoadScene(2);
        }
    }
}
