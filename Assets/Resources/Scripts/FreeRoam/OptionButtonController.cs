using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionButtonController : MonoBehaviour
{
    
    public Image image;
    public TextMeshProUGUI textMeshPro;
    private Color imageColor;
    private Color textColor;

    /*
        Set initial values so we can modify opacity without touching
        the color values.
    */
    public void Awake() {
        imageColor = image.color;
        textColor = textMeshPro.color;
    }

    /*
        This function takes in a Color `c` and sets the object's
        color to that color.
    */
    public void SetColor(Color c) {
        image.color = c;
        textMeshPro.color = c;
    }

    /*
        This function takes in an float `opacity` and sets the
        object's opacity to that value (0-1).
    */
    public void SetOpacity(float opacity) {
        image.color = new Color(imageColor.r, imageColor.g, imageColor.b, opacity);
        textMeshPro.color = new Color(textColor.r, textColor.g, textColor.b, opacity);
    }

    /*
        This function takes in a string `str` and sets the text
        color to that color.
    */
    public void SetText(string str) {
        textMeshPro.text = str;
    }

}
