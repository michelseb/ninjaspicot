using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FontText_Script : MonoBehaviour
{
    //reference to font text element
    public Text fontDemoTxt;

	//set text at start 
	void Start ()
    {
        fontDemoTxt.text = "\n JazzCreateOriental(lite) \n abcdefghijklm \n nopqrstuvwxyz \n 1234567890 \n ABCDEFGHIJKLM \n NOPQRSTUVWXYZ \n !#$%&'()*+,-./:;<=>?@ \n ^_`~¢£§©«»’—‘’•€";
    }
}
