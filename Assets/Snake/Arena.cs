using UnityEngine;

namespace Snake
{
    public class Arena : MonoBehaviour
    {
        [SerializeReference] private Transform centerPiece;
        [SerializeReference] private Transform bottomBorder;
        [SerializeReference] private Transform topBorder;
        [SerializeReference] private Transform leftBorder;
        [SerializeReference] private Transform rightBorder;

        [SerializeReference] private Vector3 borderScaleOffset = new Vector3(0.2f, 0.5f, 0.1f);
        [SerializeReference] private Vector2 borderPositionOffset = new Vector2(0.05f, 0.25f);

        public void Init(Vector2Int size)
        {
            //offset size of map to cover size of object
            size += new Vector2Int(2, 2);
            centerPiece.localScale = new Vector3(size.x, 0.5f, size.y);
            topBorder.localScale = new Vector3(size.x + borderScaleOffset.x, borderScaleOffset.y, borderScaleOffset.z);
            bottomBorder.localScale = new Vector3(size.x + borderScaleOffset.x, borderScaleOffset.y, borderScaleOffset.z);
            leftBorder.localScale = new Vector3(size.y + borderScaleOffset.x, borderScaleOffset.y, borderScaleOffset.z);
            rightBorder.localScale = new Vector3(size.y + borderScaleOffset.x, borderScaleOffset.y, borderScaleOffset.z);

            int halfX = size.x / 2;
            int halfY = size.y / 2;

            transform.localPosition = new Vector3(halfX, 0, halfY);

            topBorder.localPosition = new Vector3(0, borderPositionOffset.y, halfY + borderPositionOffset.x);
            bottomBorder.localPosition = new Vector3(0, borderPositionOffset.y, -halfY - borderPositionOffset.x);
            leftBorder.localPosition = new Vector3(-halfX - borderPositionOffset.x, borderPositionOffset.y, 0);
            rightBorder.localPosition = new Vector3(halfX + borderPositionOffset.x, borderPositionOffset.y, 0);
        }
    }
}
