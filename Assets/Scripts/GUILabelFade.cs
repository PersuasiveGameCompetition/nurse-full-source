using UnityEngine;

using System.Collections;

 

public class GUILabelFade : Object {

    private float duration;
    private float durCount = 0F;
    private string text;
    private Rect pos;
    public bool hasCompleted = false;
    private bool fadeIn;
    private int min, max;

    private GUIStyle txtStyle;

    

    public GUILabelFade(float dur, string text, Rect pos, GUIStyle style,
                        bool fadeIn = true) {

        this.hasCompleted = false;
        this.duration = dur;
        this.text = text;
        this.pos = pos;
        this.fadeIn = fadeIn;
        this.txtStyle = style;

        if(fadeIn) {
            this.durCount = dur;
        }
    }

    public void Render(Color prevColor) {
        if(!hasCompleted) {
            if(this.fadeIn) {
                durCount -= Time.deltaTime;
            } else {
                durCount += Time.deltaTime;
            }
            if(durCount < 0) {
                durCount = 0;
            } else if(durCount > duration) {
                durCount = duration;
            }

            GUI.color = new Color(prevColor.r, prevColor.g,prevColor.b,
                                  durCount == 0 ? (fadeIn ? 1 : 0) :
                                  Mathf.Lerp(1, 0, durCount / duration));

            if((GUI.color.a == 1 && fadeIn )||(GUI.color.a == 0 && !fadeIn )) {
                hasCompleted = true;
            }
        }

        if((!hasCompleted && !fadeIn) 
            || (hasCompleted && fadeIn) || !hasCompleted) {
            GUI.Label(pos, text, txtStyle);
            GUI.color = prevColor;
        }
    }
}