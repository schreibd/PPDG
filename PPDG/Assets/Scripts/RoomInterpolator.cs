using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInterpolator : MonoBehaviour {

    public static RoomInterpolator Instance { get; private set; }

    Image fadeTexture;
    public float fadingTime = 0.50f;
    float currentFade = 0.0f;
    bool fade = true;
    bool black = false;
    bool isFading = true;

    bool fadeToBlack = false;
    // Use this for initialization

    private void Awake () {
        if (!Instance)
        {
            fadeTexture = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Image>();
            Instance = this;
            fadeTexture.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            fadeTexture.color = Color.black;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (isFading)
            FadeToClear();
    }

    public bool isActive()
    {
        return isFading;
    }

    public void activate()
    {
        BlackScreen();
    }

    public void FadeToClear()
    {
        fadeTexture.color = Color.Lerp(fadeTexture.color, Color.clear, 1.5f * Time.deltaTime);
        if (fadeTexture.color.a <= 0.01f)
        {
            fadeTexture.color = Color.clear;
            fadeTexture.enabled = false;
            fade = false;
            isFading = false;
            GameObject minimap = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        }
    }
    public void BlackScreen()
    {
        fadeTexture.enabled = true;
        fadeTexture.color = Color.black;
        isFading = true;
    }

}
