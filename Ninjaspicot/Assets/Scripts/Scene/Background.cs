using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private Sprite _image;
    [SerializeField] private float _depth;
    private List<RectTransform> _images;
    private Transform _cameraPos;
    private Material _litMaterial;

    private void Awake()
    {
        gameObject.AddComponent<Canvas>().sortingLayerName = "Background";
    }

    private void Start()
    {
        _litMaterial = Resources.Load<Material>("Sprite-Lit-Default");
        _cameraPos = CameraBehaviour.Instance.Transform;

        _images = new List<RectTransform>
        {
            CreateImage(_image, new Vector2(0, 0)),
            CreateImage(_image, new Vector2(1, 0)),
            CreateImage(_image, new Vector2(0, 1)),
            CreateImage(_image, new Vector2(1, 1))
        };
    }

    private void Update()
    {
        if (Time.frameCount % Utils.EXPENSIVE_FRAME_INTERVAL == 0)
        {
            CalculateImgPositions();
        }
    }

    private RectTransform CreateImage(Sprite sprite, Vector2 pivot)
    {
        var imageObject = new GameObject("backgroundImage", typeof(Image));
        var img = imageObject.GetComponent<Image>();
        img.sprite = sprite;
        img.material = _litMaterial;

        imageObject.AddComponent<ParallaxObject>().SetParallaxFactor(_depth);

        var rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width / 4, Screen.height / 4);
        rectTransform.pivot = pivot;
        rectTransform.position = _cameraPos.position;
        rectTransform.parent = transform;

        return rectTransform;
    }

    private void CalculateImgPositions()
    {
        _images.ForEach(img =>
        {
            if (img.position.x - _cameraPos.position.x > img.rect.width / 2)
            {
                img.position += Vector3.left * img.rect.width;
            }
            else if (_cameraPos.position.x - img.position.x > img.rect.width / 2)
            {
                img.position += Vector3.right * img.rect.width;
            }
            else if (img.position.y - _cameraPos.position.y > img.rect.height / 2)
            {
                img.position += Vector3.down * img.rect.height;
            }
            else if (_cameraPos.position.y - img.position.y > img.rect.height / 2)
            {
                img.position += Vector3.up * img.rect.height;
            }
        });
    }
}
