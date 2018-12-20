using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusMoveItem : MonoBehaviour {
    public Vector2 sunrise;
    public Vector2 sunset;
    public RectTransform myRect;
    public float journeyTime = 1.0F;
    private float startTime;
    public float radius;
    public bool hasTrail;
    bool hasDestroyed = false;
    bool isStarted = false;
    // Use this for initialization
    void Start ()
    {
        myRect = GetComponent<RectTransform>();
        startTime = Time.time;
        if (hasTrail)
            MainUIManager.instance.PlayTrailEffect(myRect);
    }
	
	// Update is called once per frame
	void Update () {
        if (isStarted)
        {
            Vector2 center = (sunrise + sunset) * 0.5F;
            center -= new Vector2(0, radius);
            Vector2 riseRelCenter = sunrise - center;
            Vector2 setRelCenter = sunset - center;
            float fracComplete = (Time.time - startTime) / journeyTime;
            Vector3 newPos = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            myRect.anchoredPosition = newPos;
            myRect.anchoredPosition += center;
            if (Vector2.Distance(myRect.anchoredPosition, sunset) < 5 && !hasDestroyed)
            {
                hasDestroyed = true;
                Destroy(gameObject);
            }
        }
        
    }

    public void SetStartPoint(Vector2 pos)
    {
        sunrise = pos;
        isStarted = true;
    }


}

public class UIPosition
{
    static public Vector2 WorldToUI(RectTransform r, Vector3 pos)
    {
        Vector2 screenPos = Camera.main.WorldToViewportPoint(pos); //世界物件在螢幕上的座標，螢幕左下角為(0,0)，右上角為(1,1)
        Vector2 viewPos = (screenPos - r.pivot) * 2; //世界物件在螢幕上轉換為UI的座標，UI的Pivot point預設是(0.5, 0.5)，這邊把座標原點置中，並讓一個單位從0.5改為1
        float width = r.rect.width / 2; //UI一半的寬，因為原點在中心
        float height = r.rect.height / 2; //UI一半的高
        return new Vector2(viewPos.x * width, viewPos.y * height); //回傳UI座標
    }

    static public Vector3 UIToWorld(RectTransform r, Vector3 uiPos)
    {
        float width = r.rect.width / 2; //UI一半的寬
        float height = r.rect.height / 2; //UI一半的高
        Vector3 screenPos = new Vector3(((uiPos.x / width) + 1f) / 2, ((uiPos.y / height) + 1f) / 2, uiPos.z); //須小心Z座標的位置
        return Camera.main.ViewportToWorldPoint(screenPos);
    }
}