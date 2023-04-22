using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    Transform Player;
    Transform Goal;
    Transform Walls;
    GameObject WallTemplate;
    GameObject FloorTemplate;
    float MovementSmoothing;

    [SerializeField] int Width = 3;     //starting size
    [SerializeField] int Height = 3;    //starting size

    public bool[,] HWalls, VWalls;  //vertical and horizontal walls
    public float HoleProbability;
    public int GoalX, GoalY;        //location of goal

    public int PlayerX, PlayerY;    //location of player

    public KeyCode upKey = KeyCode.UpArrow; // The key to move up
    public KeyCode downKey = KeyCode.DownArrow; // The key to move down
    public KeyCode leftKey = KeyCode.LeftArrow; // The key to move left
    public KeyCode rightKey = KeyCode.RightArrow;   // The key to move right

    void Start()
    {
        StartNext();
    }

    void Update()
    {

        PlayerMover();
        Vector3 target = new Vector3(PlayerX + 0.5f, PlayerY + 0.5f);
        Player.transform.position = Vector3.Lerp(Player.transform.position, target, Time.deltaTime * MovementSmoothing);
        NextLevelCheck();
    }

    public int Rand(int max)
    {
        return UnityEngine.Random.Range(0, max);
    }
    public float frand()
    {
        return UnityEngine.Random.value;
    }

    public void PlayerMover()
    {
        if (Input.GetKeyDown(leftKey) && !HWalls[PlayerX, PlayerY])
            PlayerX--;
        if (Input.GetKeyDown(rightKey) && !HWalls[PlayerX + 1, PlayerY])
            PlayerX++;
        if (Input.GetKeyDown(upKey) && !VWalls[PlayerX, PlayerY + 1])
            PlayerY++;
        if (Input.GetKeyDown(downKey) && !VWalls[PlayerX, PlayerY])
            PlayerY--;
    }
    public void NextLevelCheck()
    {
        if (Vector3.Distance(Player.transform.position, new Vector3(GoalX + 0.5f, GoalY + 0.5f)) < 0.12f)
        {
            if (Rand(5) < 3)
                Width++;
            else
                Height++;

            StartNext();
        }
        if (Input.GetKeyDown(KeyCode.G))
            StartNext();
    }
    public void StartNext()
    {
        foreach (Transform child in Walls)
            Destroy(child.gameObject);

        (HWalls, VWalls) = GenerateLevel(Width, Height);
        PlayerX = Rand(Width);
        PlayerY = Rand(Height);

        int minDiff = Mathf.Max(Width, Height) / 2;
        while (true)
        {
            GoalX = Rand(Width);
            GoalY = Rand(Height);
            if (Mathf.Abs(GoalX - PlayerX) >= minDiff) break;
            if (Mathf.Abs(GoalY - PlayerY) >= minDiff) break;
        }

        for (int x = 0; x < Width + 1; x++)
            for (int y = 0; y < Height; y++)
                if (HWalls[x, y])
                    Instantiate(WallTemplate, new Vector3(x, y + 0.5f, 0), Quaternion.Euler(0, 0, 90), Walls);
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height + 1; y++)
                if (VWalls[x, y])
                    Instantiate(WallTemplate, new Vector3(x + 0.5f, y, 0), Quaternion.identity, Walls);
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Instantiate(FloorTemplate, new Vector3(x + 0.5f, y + 0.5f), Quaternion.identity, Walls);

        Player.transform.position = new Vector3(PlayerX + 0.5f, PlayerY + 0.5f);
        Goal.transform.position = new Vector3(GoalX + 0.5f, GoalY + 0.25f);

        vcam.m_Lens.OrthographicSize = Mathf.Pow(Mathf.Max(Width / 1.5f, Height), 0.70f) * 0.95f;
    }

    public (bool[,], bool[,]) GenerateLevel(int w, int h)
    {
        bool[,] hwalls = new bool[w + 1, h];
        bool[,] vwalls = new bool[w, h + 1];

        bool[,] visited = new bool[w, h];
        bool dfs(int x, int y)  //to check if the maze is solveable
        {
            if (visited[x, y])
                return false;
            visited[x, y] = true;

            var dirs = new[]
            {
                (x - 1, y, hwalls, x, y),
                (x + 1, y, hwalls, x + 1, y),
                (x, y - 1, vwalls, x, y),
                (x, y + 1, vwalls, x, y + 1),
            };

            foreach (var (nx, ny, wall, wx, wy) in dirs.OrderBy(t => frand()))
                wall[wx, wy] = !(0 <= nx && nx < w && 0 <= ny && ny < h && (dfs(nx, ny) || frand() < HoleProbability));

            return true;
        }
        dfs(0, 0);

        return (hwalls, vwalls);
    }
}
