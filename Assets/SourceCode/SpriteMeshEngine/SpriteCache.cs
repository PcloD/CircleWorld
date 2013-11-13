using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpriteCacheEntry
{
    public string spritesheetId;
    public Sprite[] sprites;
}

public class SpriteCache : MonoBehaviour
{
    public SpriteCacheEntry[] cacheEntries;

    public Sprite GetSprite(string spritesheetId, string spriteId)
    {
        for (int i = 0; i < cacheEntries.Length; i++)
        {
            if (cacheEntries[i].spritesheetId == spritesheetId)
            {
                for (int j = 0; j < cacheEntries[i].sprites.Length; j++)
                    if (cacheEntries[i].sprites[j].name == spriteId)
                        return cacheEntries[i].sprites[j];
            }
        }

        return null;
    }
}


