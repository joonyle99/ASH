using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPath : MonoBehaviour
{
    [SerializeField] Vector2 [] _points;
    [SerializeField] float _pixelPerUnit = 100;
    [SerializeField] SpriteMask _spriteMask;

    PolygonCollider2D _polygonCollider;
    // Start is called before the first frame update
    void Start()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        StartCoroutine(RevealCoroutine());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))

        GenerateTexture();
    }
    void GenerateTexture()
    {
        //Get rect
        _points = _polygonCollider.points;
        Rect maskRect = new Rect(_points[0].x, _points[0].y, 0, 0);
        foreach(var point in _points)
        {
            if (maskRect.xMin > point.x) maskRect.xMin = point.x;
            if (maskRect.xMax < point.x) maskRect.xMax = point.x;
            if (maskRect.yMin > point.y) maskRect.yMin = point.y;
            if (maskRect.yMax < point.y) maskRect.yMax = point.y;
        }
        maskRect.size.Scale(transform.lossyScale);
        int width = (int)(maskRect.width * _pixelPerUnit);
        int height = (int)(maskRect.height * _pixelPerUnit);
        print(maskRect + " " + width + " " + height);
        //GenTexture
        Texture2D texture = new Texture2D(width, height);

        Color[] pixels = new Color[width *height];
        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 point = new Vector2(_polygonCollider.bounds.min.x, _polygonCollider.bounds.min.y) + new Vector2(x, y) / _pixelPerUnit;
                print(point);
                
                if (_polygonCollider.OverlapPoint(point))
                    pixels[y * width + x] = Color.white;
                else
                    pixels[y * width + x] = new Color(0, 0, 0, 0);
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();

        _spriteMask.sprite = Sprite.Create(texture, new Rect(0,0, width, height), new Vector2(0.5f,0.5f));
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0,0, width, height), new Vector2(0.5f,0.5f));
    }
    IEnumerator RevealCoroutine()
    {
        yield return null;

    }
}
