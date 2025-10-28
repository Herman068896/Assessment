using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
   [Header("Grid & Speed")]
    public float tilesPerSecond = 6f;           
    public Vector2 cellSize = Vector2.one;    
    public Vector2Int currentGrid;
    private Vector2Int targetGrid;

    [Header("Input State")]
    public Vector2Int lastInput = Vector2Int.zero;  
    public Vector2Int currentInput = Vector2Int.right;

    [Header("Refs")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip moveEatClip;    
    public AudioClip moveEmptyClip;  
    public ParticleSystem dustTrail;

    
    bool isLerping = false;
    Vector3 startPos, endPos;
    float t = 0f;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!dustTrail) dustTrail = GetComponentInChildren<ParticleSystem>();
    }

    void Start()
    {
      
        currentGrid = WorldToGrid(transform.position);
        targetGrid = currentGrid;

 
        FaceDirection(Vector2Int.right);
    }

    void Update()
    {
        ReadInput();

        if (isLerping)
        {
            DoLerp();
        }
        else
        {
           
            if (TryBeginStep(lastInput)) return;
            if (TryBeginStep(currentInput)) return;

           
            SetMoving(false);
        }
    }

    void ReadInput()
    {
       
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2Int.right;
    }

    bool TryBeginStep(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return false;

        var next = currentGrid + dir;
        if (!LevelGenerator.Instance.IsWalkable(next)) return false;

       
        currentInput = dir;                    
        startPos = GridToWorld(currentGrid);
        endPos   = GridToWorld(next);
        t = 0f;
        isLerping = true;
        targetGrid = next;

       
        FaceDirection(dir);
        SetMoving(true);
        SelectAndPlayMoveClip(next);
        PlayDust(true);

        return true;
    }

    void DoLerp()
    {
       
        float duration = 1f / tilesPerSecond;  
        t += Time.deltaTime / duration;

        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (t >= 1f)
        {
            isLerping = false;
            currentGrid = targetGrid;

           
            SetMoving(false);
            PlayDust(false);
            StopMoveClipIfPlaying();
        }
    }

    void FaceDirection(Vector2Int dir)
    {
      
        if (dir == Vector2Int.left)  transform.localScale = new Vector3(-1, 1, 1);
        if (dir == Vector2Int.right) transform.localScale = new Vector3( 1, 1, 1);
       
    }

    void SetMoving(bool moving)
    {
        if (animator) animator.SetBool("isMoving", moving);
    }

    void SelectAndPlayMoveClip(Vector2Int nextGrid)
    {
        if (!audioSource) return;

       
        bool aboutToEat = LevelGenerator.Instance.HasPelletAt(nextGrid); 
        var clip = aboutToEat ? moveEatClip : moveEmptyClip;

        if (clip && audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (clip && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopMoveClipIfPlaying()
    {
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void PlayDust(bool on)
    {
        if (!dustTrail) return;
        if (on)
        {
            if (!dustTrail.isPlaying) dustTrail.Play();
        }
        else
        {
            if (dustTrail.isPlaying) dustTrail.Stop();
        }
    }

   
    Vector2Int WorldToGrid(Vector3 world)
    {
        int gx = Mathf.RoundToInt(world.x / cellSize.x);
        int gy = Mathf.RoundToInt(world.y / cellSize.y);
        return new Vector2Int(gx, gy);
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x * cellSize.x, grid.y * cellSize.y, 0f);
    }
}
