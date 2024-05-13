using DragToCast.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace DragToCast.Implementation;

#nullable enable

internal class CastingLineRenderer : MonoBehaviour
{
    internal enum Curvature
    {
        Line,
        BezierQuadratic,
        BezierCubic,
    }

    public static CastingLineRenderer? Instance;

    private Canvas? _canvas;
    private LineRenderer? _lineRenderer;
    private Texture2D? _lineTexture;

    private void Start()
    {
        Instance = this;

        var system = GetComponent<BattleSystem>() ?? throw new MissingComponentException(nameof(BattleSystem));
        var manaSprite = system.ActWindow.APObject.transform
            .GetFirstChildWithName("On")?
            .GetComponent<Image>()
            .sprite ?? throw new MissingComponentException();
        _lineTexture = manaSprite.texture.Blit(manaSprite.textureRect);
        _lineTexture.wrapMode = TextureWrapMode.Repeat;

        _canvas = system.MainUICanvas;
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.sortingLayerName = _canvas.sortingLayerName;
        _lineRenderer.sortingOrder = short.MaxValue;

        _lineRenderer.material = new(Shader.Find("Sprites/Default")) {
            mainTexture = _lineTexture
        };
        _lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;
        _lineRenderer.widthMultiplier = 0.2f;

        _lineRenderer.enabled = false;
    }

    internal void DrawToPointer(Vector3 startPoint, Curvature curvature)
    {
        var pointer = Input.mousePosition with { z = 9f };
        DrawLine(startPoint, _canvas!.worldCamera.ScreenToWorldPoint(pointer), curvature);
    }

    internal void DrawLine(Vector3 startPoint, Vector3 endPoint, Curvature curvature)
    {
        if (_canvas == null || _lineRenderer == null || _lineTexture == null) {
            return;
        }
        _lineRenderer.enabled = true;

        var lineLength = Vector3.Distance(endPoint, startPoint);
        var segments = Mathf.FloorToInt(Display.main.systemWidth / 170);
        _lineRenderer.positionCount = segments + 1;
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
                }
                break;
            }
            case Curvature.BezierCubic: {
                var firstControlPoint = (startPoint + endPoint) / 3 + Vector3.up * 2;
                var secondControlPoint = (startPoint + endPoint) / 3 * 2 + Vector3.up * 2;
                for (int i = 0; i <= segments; ++i) {
                    var point = startPoint.BezierCubic(firstControlPoint, secondControlPoint, endPoint, i / (float)segments);
                    _lineRenderer.SetPosition(i, point);
                }
                break;
            }
        }

        _lineRenderer.material.mainTextureScale = new(lineLength / _lineRenderer.widthMultiplier, 1f);
    }

    internal void Clear()
    {
        if (_lineRenderer != null) {
            _lineRenderer.enabled = false;
        }
    }
}
