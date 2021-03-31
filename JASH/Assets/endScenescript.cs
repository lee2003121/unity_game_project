using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endScenescript : MonoBehaviour
{
    float timesave = 0;
    public Text st;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timesave =PlayerPrefs.GetFloat("Timertime");
        string minutes = ((int)timesave / 60).ToString();
        string seconds = ((int)timesave % 60).ToString();
        st.text = "TIME\n" + minutes + ":" + seconds;
        
    }
}
