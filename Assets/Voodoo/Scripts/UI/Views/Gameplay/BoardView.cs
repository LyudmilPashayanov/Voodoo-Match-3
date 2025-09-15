using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    [SerializeField] private RectTransform _boardTransform;
    [SerializeField] private RectTransform _arrowsMoveOverlay;
    [SerializeField] private List<RectTransform> _arrowsTransform;

    private float _spriteSize;
    private int _boardWidth;
    private Vector2 _origin;

    public void InitializeBoard(int gridWidth, int gridHeight)
    {
        _boardWidth = gridWidth;
        _boardTransform.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridWidth, gridHeight);
        _spriteSize = _boardTransform.rect.width / _boardWidth;
        _arrowsMoveOverlay.sizeDelta = new Vector2(_spriteSize,_spriteSize);
        foreach (RectTransform arrow in _arrowsTransform)
        {
            arrow.sizeDelta = new Vector2(_spriteSize/2, _spriteSize/2);
        }
        
        _origin = new Vector2(
            -(_boardWidth - 1) * (_spriteSize + 0) * 0.5f,
            -(_boardWidth - 1) * (_spriteSize + 0) * 0.5f
        );
    }

    public Transform GetBoardTransform()
    {
        return _boardTransform;
    }
    
    public Vector2 GetBoardPositionBasedOnIndex(int index)
    {
        int x = index % _boardWidth;
        int y = index / _boardWidth;

        float cell = _spriteSize + 0;
        float posX = x * cell;
        float posY = y * cell;

        return _origin + new Vector2(posX, posY);

    }
    
    public float GetSpriteSize()
    {
        return _spriteSize;
    }

    public void SetArrowOverlayPosition(int index)
    {
        _arrowsMoveOverlay.transform.localPosition = GetBoardPositionBasedOnIndex(index);
    }

    public void EnableArrowOverlay(bool enable)
    {
        _arrowsMoveOverlay.gameObject.SetActive(enable);
    }

}
