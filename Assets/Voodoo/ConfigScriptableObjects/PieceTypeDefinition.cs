using UnityEngine;

[CreateAssetMenu(fileName = "PieceType", menuName = "Match3/Piece Type", order = 0)]
public class PieceTypeDefinition : ScriptableObject
{
    [Tooltip("Unique string ID used to identify this piece type")]
    public string id;

    [Tooltip("Prefab used for rendering this piece")]
    public GameObject prefab;

    [Tooltip("Optional color for debugging or UI")]
    public Color displayColor;
}