using UnityEngine;

using System.Collections;

 

public class GUITextureFade : Object {

    private float duration;
    private float durCount = 0F;
    private Rect pos;
    public bool hasCompleted = false;
    private bool fadeIn;
    private int min, max;
    private Texture2D img;

    public GUITextureFade(float dur, Texture2D img, Rect pos,
                        bool fadeIn = true) {

        this.hasCompleted = false;
        this.duration = dur;
        this.img = img;
        this.pos = pos;
        this.fadeIn = fadeIn;

        if(fadeIn) {
            this.durCount = dur;
        }
    }

    public void Render(Color prevColor) {
        Debug.Log(durCount);
        if(!hasCompleted) {
            if(this.fadeIn) {
                durCount -= Time.deltaTime;
            } else {
                durCount += Time.deltaTime;
            }

            if(durCount < 0) {
                durCount = 0;
            } else if(durCount > duration) {
                Debug.Log("hey");
                durCount = duration;
            }

            GUI.color = new Color(1f,1f,1f, durCount == 0 ? (fadeIn ? 1 : 0) :
                                  Mathf.Lerp(1, 0, durCount / duration));
            //Debug.Log(durCount / duration);

            if((GUI.color.a == 1 && fadeIn )||(GUI.color.a == 0 && !fadeIn )) {
                hasCompleted = true;
                Debug.Log("complete!!!!");
            }
        }

        if((!hasCompleted && !fadeIn) 
            || (hasCompleted && fadeIn) || !hasCompleted) {
            GUI.DrawTexture(pos, img, ScaleMode.ScaleToFit);
            //GUI.color = prevColor;
        }
    }
}