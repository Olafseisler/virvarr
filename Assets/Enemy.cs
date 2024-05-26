using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private MyColor enemyColor;
    [SerializeField] private bool isMoving = false;
    
    public MyColor GetColor()
    {
        return enemyColor;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the sprite color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (enemyColor)
        {
            case MyColor.Red:
                spriteRenderer.color = Color.red;
                break;
            case MyColor.Green:
                spriteRenderer.color = Color.green;
                break;
            case MyColor.Blue:
                spriteRenderer.color = Color.blue;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
