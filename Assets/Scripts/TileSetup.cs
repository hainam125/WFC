using UnityEngine;

public class TileSetup : MonoBehaviour
{
    [SerializeField] private Tile curTile;
    [SerializeField] private Shader shader;
    [SerializeField] private bool isDebug;

    private Material material;
    private Material Mat
    {
        get
        {
            if (material == null) material = new Material(shader);
            return material;
        }
    }

    private void OnDrawGizmos()
    {
        if (!isDebug || curTile == null) return;

        var origin = -0.5f * Vector2.one;
        DrawTile(curTile.sprite, origin);
        DrawTiles(curTile.upNeighbors, origin, 0, 1);
        DrawTiles(curTile.downNeighbors, origin, 0, -1);
        DrawTiles(curTile.leftNeighbors, origin, -1, 0);
        DrawTiles(curTile.rightNeighbors, origin, 1, 0);
    }

    private void DrawTiles(Sprite[] sprites, Vector2 origin, int dirX, int dirY)
    {
        for (var i = 0; i < sprites.Length; i++)
        {
            DrawTile(sprites[i],
                origin + new Vector2(dirX, dirY) * (i*1.5f) + new Vector2(dirX, dirY) * 1.5f);
        }
    }

    private void DrawTile(Sprite sprite, Vector2 pos)
    {
        var texWidth = sprite.texture.width;
        var texHeight = sprite.texture.height;
        var spriteRect = sprite.rect;
        Mat.SetVector("_Offset",
            new Vector4(spriteRect.x / texWidth, spriteRect.y / texHeight,
                        spriteRect.width / texWidth, spriteRect.height / texHeight));
        Gizmos.DrawGUITexture(new Rect(pos.x, pos.y, 1, 1), sprite.texture, Mat);
    }
}
