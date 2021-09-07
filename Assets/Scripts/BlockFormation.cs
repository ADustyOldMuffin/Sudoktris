using System;
using System.Collections.Generic;
using System.Linq;
using Levels;
using UnityEngine;

public class BlockFormation : MonoBehaviour
{
    public Vector2 SpawnLocation { get; set; }

    public bool IsDisabled { get; private set; }

    public List<GameObject> blocks;

    private GameObject _mainBlock;

    private void Start()
    {
        if (blocks.Count == 0)
            throw new Exception("Can't have a block formation without any blocks!");

        _mainBlock = blocks.Single(x => x.GetComponent<Block>().formationX == 0 && x.GetComponent<Block>().formationY == 0);
    }

    private void OnEnable()
    {
        LevelManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        LevelManager.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        Destroy(gameObject);
    }

    public void ResetChildSizeAndLayer()
    {
        foreach (GameObject block in blocks)
        {
            block.transform.localScale = new Vector3(1, 1);
            block.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    public void ShrinkChildrenAndBringToFront()
    {
        foreach (GameObject block in blocks)
        {
            block.transform.localScale = new Vector3(0.9f, 0.9f);
            block.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }

    public void DisableFormation()
    {
        if (IsDisabled)
            return;
        
        foreach (GameObject block in blocks)
        {
            var blockRenderer = block.GetComponent<SpriteRenderer>();
            Color color = blockRenderer.color;
            color = new Color(color.r, color.g, color.b, 0.4f);
            blockRenderer.color = color;
        }

        IsDisabled = true;
    }

    public void EnableFormation()
    {
        if (!IsDisabled)
            return;
        
        foreach (GameObject block in blocks)
        {
            var blockRenderer = block.GetComponent<SpriteRenderer>();
            Color color = blockRenderer.color;
            color = new Color(color.r, color.g, color.b, 1);
            blockRenderer.color = color;
        }

        IsDisabled = false;
    }

    public void ResetLocation()
    {
        transform.position = SpawnLocation;
    }
}
