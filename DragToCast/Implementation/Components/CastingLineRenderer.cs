using ChronoArkMod;
using ChronoArkMod.ModData;
using DragToCast.Helper;
using System.IO;
using UnityEngine;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class CastingLineRenderer : MonoBehaviour
{
    internal enum Curvature
    {
        Line,
        BezierQuadratic,
        BezierCubic,
    }

    internal enum TextureOrientation
    {
        Up,
        Down,
        Left,
        Right,
    }

    public const string LineTextureShader = "UI/Default";
    public const string LineTextureFile = "linedot_anri_gave_me_must_use_with_caution.png";
    public const string LineHeadTextureFile = "arrowhead_anri_gave_me_must_use_with_caution.png";
    public const bool UseMyBelovedManaCrystal = false;

    public static CastingLineRenderer? Instance;
    private static TextureOrientation _lineHeadOrientation = TextureOrientation.Right;

    private Canvas? _canvas;
    private LineRenderer? _lineRenderer;
    private Texture2D? _lineTexture;
    private Texture2D? _lineHeadTexture;
    private SpriteRenderer? _lineHeadRenderer;

    private int TextureOrientationOffset
    {
        get
        {
            return _lineHeadOrientation switch {
                TextureOrientation.Up => -90,
                TextureOrientation.Down => 90,
                TextureOrientation.Left => 180,
                TextureOrientation.Right => 0,
                _ => 0,
            };
        }
    }

    private void Start()
    {
        Instance = this;

        var system = GetComponent<BattleSystem>() ?? throw new MissingComponentException(nameof(BattleSystem));
        if (UseMyBelovedManaCrystal) {
            /**
            var manaSprite = system.ActWindow.APObject.transform
                .GetFirstChildWithName("On")?
                .GetComponent<Image>()
                .sprite ?? throw new MissingComponentException();
            _lineTexture = manaSprite.texture.Blit(manaSprite.textureRect);
            /**/
        } else {
            _lineHeadOrientation = TextureOrientation.Down;
            _lineTexture = LoadAssetInternal(LineTextureFile);
        }
        _lineTexture.wrapMode = TextureWrapMode.Repeat;
        _lineHeadTexture = LoadAssetInternal(LineHeadTextureFile);

        _canvas = system.MainUICanvas;
        _lineHeadRenderer = gameObject.AddComponent<SpriteRenderer>();
        _lineHeadRenderer.sortingLayerName = _canvas.sortingLayerName;
        _lineHeadRenderer.sortingOrder = short.MaxValue;
        _lineHeadRenderer.sprite = Misc.CreatSprite(_lineHeadTexture);

        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.sortingLayerName = _canvas.sortingLayerName;
        _lineRenderer.sortingOrder = _lineHeadRenderer.sortingOrder - 1;

        _lineRenderer.material = new(Shader.Find(LineTextureShader)) {
            mainTexture = _lineTexture,
            mainTextureScale = new(3f, 1f),
        };
        _lineRenderer.textureMode = LineTextureMode.Tile;
        _lineRenderer.widthMultiplier = 0.33f;

        _lineRenderer.enabled = false;
        _lineHeadRenderer.enabled = false;
    }

    internal void DrawToPointer(Vector3 startPoint, Curvature curvature)
    {
        var pointer = Input.mousePosition with { z = 9f };
        DrawLine(startPoint, _canvas!.worldCamera.ScreenToWorldPoint(pointer), curvature);
    }

    internal void DrawLine(Vector3 startPoint, Vector3 endPoint, Curvature curvature)
    {
        if (_canvas == null || _lineRenderer == null || _lineHeadRenderer == null) {
            return;
        }
        _lineRenderer.enabled = true;
        _lineHeadRenderer.enabled = true;

        //var lineLength = Vector3.Distance(endPoint, startPoint);
        var segments = Mathf.FloorToInt(Display.main.systemWidth / 170);
        _lineRenderer.positionCount = segments + 1;
        var endOfLine = endPoint;
        var beforeEndOfLine = startPoint;
        switch (curvature) {
            case Curvature.Line: {
                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPositions([startPoint, endPoint]);
                break;
            }
            case Curvature.BezierQuadratic: {
                var controlPoint = (startPoint + endPoint) / 2 + Vector3.up * 2;
                for (int i = 0; i <= segments; ++i) {
                    var point = startPoint.BezierQuadratic(controlPoint, endPoint, i / (float)segments);
                    _lineRenderer.SetPosition(i, point);
                    // fix end of line
                    if (i == segments - 1) {
                        beforeEndOfLine = point;
                    }
                    if (i == segments) {
                        endOfLine = point;
                    }
                }
                break;
            }
            case Curvature.BezierCubic: {
                var firstControlPoint = (startPoint + endPoint) / 3 + Vector3.up * 2;
                var secondControlPoint = (startPoint + endPoint) / 3 * 2 + Vector3.up * 2;
                for (int i = 0; i <= segments; ++i) {
                    var point = startPoint.BezierCubic(firstControlPoint, secondControlPoint, endPoint, i / (float)segments);
                    _lineRenderer.SetPosition(i, point);
                    // fix end of line
                    if (i == segments - 1) {
                        beforeEndOfLine = point;
                    }
                    if (i == segments) {
                        endOfLine = point;
                    }
                }
                break;
            }
        }
        // attach line head
        _lineHeadRenderer.transform.position = endOfLine;
        var tangent = endOfLine - beforeEndOfLine;
        var angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg + TextureOrientationOffset;
        _lineHeadRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    internal void Clear()
    {
        if (_lineRenderer != null) {
            _lineRenderer.enabled = false;
        }
        if (_lineHeadRenderer != null) {
            _lineHeadRenderer.enabled = false;
        }
    }

    private Texture2D LoadAssetInternal(string name)
    {
        var assetInfo = ModManager.getModInfo(DragToCastMod.Instance!.ModId).assetInfo;
        return AssetGeneratingTools.LoadTexture(Path.Combine(assetInfo.AssetDirectory, name));
    }
}
