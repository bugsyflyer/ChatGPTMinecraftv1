using UnityEngine;

public class Block : MonoBehaviour
{
    public int durability = 100; // Durability of the block

    public void Break()
    {
        durability -= 25; // Reduce the durability of the block

        if (durability <= 0)
        {
            Destroy(gameObject); // Destroy the block when its durability reaches zero
        }
    }
}