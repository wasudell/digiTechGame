using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAddress : MonoBehaviour
{
    // getting next address from the postal cannon script
    public PostalCannon address;
    // declaring the text box to edit
    public Text txt;

    void Update()
    {
        // updating the text within the box to the address name
        txt.text = address.addrName;
    }
}
