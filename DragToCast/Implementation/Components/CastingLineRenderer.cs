using ChronoArkMod;
using ChronoArkMod.ModData;
using DragToCast.Helper;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DragToCast.Implementation.Components;

#nullable enable
#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable IDE0031 // Use null propagation

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
    public const int LineHeadBoundPivot = 5;

    private TextureOrientation _lineHeadOrientation;
    private Canvas? _canvas;
    private LineRenderer? _lineRenderer;
    private Texture2D? _lineTexture;
    private SpriteRenderer? _lineHeadRenderer;

    public static CastingLineRenderer? Instance => BattleSystem.instance != null ? BattleSystem.instance.gameObject.GetOrAddComponent<CastingLineRenderer>() : null;

    public bool IsRendering => _lineRenderer?.enabled ?? false;

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
        var system = GetComponent<BattleSystem>() ?? throw new MissingComponentException(nameof(BattleSystem));
        if (UseMyBelovedManaCrystal) {
            var manaSprite = system.ActWindow.APObject.transform
                .GetFirstChildWithName("On")?
                .GetComponent<Image>()
                .sprite ?? throw new MissingComponentException();
            _lineTexture = manaSprite.texture.Blit(manaSprite.textureRect);
            _lineHeadOrientation = TextureOrientation.Right;
        } else {
            _lineTexture = LoadAssetInternal(LineTextureFile);
            _lineHeadOrientation = TextureOrientation.Down;
        }
        _lineTexture.wrapMode = TextureWrapMode.Repeat;

        _canvas = system.MainUICanvas;
        _lineHeadRenderer = gameObject.AddComponent<SpriteRenderer>();
        _lineHeadRenderer.sortingLayerName = _canvas.sortingLayerName;
        _lineHeadRenderer.sortingOrder = short.MaxValue;
        _lineHeadRenderer.sprite = Misc.CreatSprite(LoadAssetInternal(LineHeadTextureFile));

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

        var lineLength = Vector3.Distance(endPoint, startPoint);
        var segments = Mathf.FloorToInt(Display.main.systemWidth / 170f);
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
                var upOffset = Mathf.Lerp(0.5f, 2f, lineLength / 50f);
                var controlPoint = (startPoint + endPoint) / 2 + Vector3.up * upOffset;
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
                var upOffset = Mathf.Lerp(0.5f, 2f, lineLength / 50f);
                var firstControlPoint = (startPoint + endPoint) / 3 + Vector3.up * upOffset;
                var secondControlPoint = (startPoint + endPoint) / 3 * 2 + Vector3.up * upOffset;
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
        var tangent = endOfLine - beforeEndOfLine;
        var angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg + TextureOrientationOffset;
        var pivotOffset = _lineHeadRenderer.bounds.size.magnitude / LineHeadBoundPivot;
        _lineHeadRenderer.transform.position = endOfLine - tangent.normalized * pivotOffset;
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
