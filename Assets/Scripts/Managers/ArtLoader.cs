using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ArtLoader : MonoBehaviour
{
    public int imageCountToLoadAtStart = 6;

    private const int imageWidth = 270;
    private const int imageHeight = 240;
    private string url = "https://picsum.photos";

    private List<Texture2D> freeTextures = new List<Texture2D>();

    public static ArtLoader instance;
    private void Awake()
    {
        if(instance != null)
        {
            this.enabled = false;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        url = url + "/" + imageWidth + "/" + imageHeight;
        StartCoroutine(DownloadImages(url, imageCountToLoadAtStart));
    }

    public void SetImage(Image img)
    {
        if(freeTextures.Count > 0)
        {
            img.sprite = Sprite.Create(freeTextures[0], new Rect(0, 0, imageWidth, imageHeight), new Vector2());
            freeTextures.RemoveAt(0);
        }
        else
        {
            StartCoroutine(DownloadImage(url, img));
        }
    }

    public void DownloadImages(int count)
    {
        StartCoroutine(DownloadImages(url, count));
    }

    IEnumerator DownloadImages(string MediaUrl, int count)
    {
        while (count > 0)
        {
            yield return StartCoroutine(DownloadImage(MediaUrl));
            count--;
        }
    }

    IEnumerator DownloadImage(string MediaUrl, Image outImg = null)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            if (outImg != null)
            {
                outImg.sprite = Sprite.Create(texture, new Rect(0, 0, imageWidth, imageHeight), new Vector2());
            }
            else
            {
                freeTextures.Add(texture);
            }
        }
    }
}
