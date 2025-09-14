using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Voodoo.Scripts.GameSystems.Utilities;
using Voodoo.Scripts.UI.Views.Gameplay;

public class BoardView : MonoBehaviour
{

    [SerializeField] private RectTransform _boardTransform;

    private float _spriteSize;
    private readonly Dictionary<int, GamePieceView> _activePieces = new Dictionary<int, GamePieceView>();
    private int _boardWidth;
    private Vector2 _origin;

    public void InitializeBoard(int gridWidth, int gridHeight)
    {
        _boardWidth = gridWidth;
        _boardTransform.GetComponent<RawImage>().uvRect = new Rect(0, 0, gridWidth, gridHeight);
        CalculateSpriteSize(gridWidth);
        
        _origin = new Vector2(
            -(_boardWidth - 1) * (_spriteSize + 0) * 0.5f,
            -(_boardWidth - 1) * (_spriteSize + 0) * 0.5f
        );
    }
    
    public void ClearPieces(int[] indices)
    {
        foreach (var idx in indices)
        {
            if (!_activePieces.TryGetValue(idx, out var piece)) continue;
            piece.transform.DOScale(Vector3.zero, 0.3f);
                //.OnComplete(() => PiecePool.Instance.Release(piece));
            _activePieces.Remove(idx);
        }
    }
    
    public void PlacePiece(GamePieceView piece, int index)
    {
        _activePieces.Add(index, piece);
        piece.gameObject.name = index.ToString();
        piece.SetSize(_spriteSize);
        piece.transform.SetParent(_boardTransform, false);
        piece.transform.localPosition = GetCellLocalPosition(index);
    }
    
    private Vector2 GetCellLocalPosition(int index)
    {
        int x = index % _boardWidth;
        int y = index / _boardWidth;

        // Flip Y so model row 0 (top) renders at the bottom
        int flippedY = (_boardWidth - 1) - y;

        float cell = _spriteSize + 0;   // make spacing a float
        float posX = x * cell;
        float posY = flippedY * cell;

        return _origin + new Vector2(posX, posY);        // local to the board root
    }
    
    private void CalculateSpriteSize(int gridWidth)
    {
        _spriteSize = _boardTransform.rect.width / gridWidth;
    }

}
