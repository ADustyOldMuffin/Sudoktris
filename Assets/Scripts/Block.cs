using System;
using System.Collections;
using Levels;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int formationX;
    public int formationY;

    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime;
    private static readonly int Destroy1 = Animator.StringToHash("destroy");

    private Rigidbody2D _rigidbody;

    private void Start()
    {
        // We have to do this since we rotate the parent to make the different shapes, the actual blocks themselves should not rotate.
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        _rigidbody = GetComponent<Rigidbody2D>();
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
        if (ReferenceEquals(_rigidbody, null))
            return;

        _rigidbody.simulated = true;
    }

    public void Destroy()
    {
        StartCoroutine(DestroyBlock());
    }

    private IEnumerator DestroyBlock()
    {
        animator.SetTrigger(Destroy1);
        yield return new WaitForSeconds(animationTime);
        Destroy(gameObject);
    }

    public Vector2 GetRoundedPosition()
    {
        Vector3 position = transform.position;
        int blockX = Mathf.FloorToInt(position.x);
        int blockY = Mathf.FloorToInt(position.y);

        return new Vector2(blockX, blockY);
    }

    public Vector2 GetPositionWithOffset()
    {
        Vector3 position = transform.position;
        int blockX = Mathf.FloorToInt(position.x);
        int blockY = Mathf.FloorToInt(position.y);
        float offsetX = position.x - blockX < 0 ? -0.5f : 0.5f;
        float offsetY = position.y - blockY < 0 ? -0.5f : 0.5f;
        
        return new Vector2(blockX + offsetX, blockY + offsetY);
    }
}