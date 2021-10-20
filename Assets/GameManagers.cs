using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sokoban;

public class GameManagers : MonoBehaviour
{
    public LevelBuilder m_LevelBuilder;
    public GameObject m_NextButton;
    public DropdownScript m_AlgorithmSelector;
    
    private bool m_ReadyForInput;
    private Player m_Player;
    private List<Vector2> m_AlgorithmSteps;

    void Start()
    {
        m_NextButton.SetActive(false);
        ResetScene();
        m_AlgorithmSteps = new List<Vector2>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();
        if (m_AlgorithmSteps.Count > 0)
        {
            if (m_ReadyForInput)
            {
                m_ReadyForInput = false;
                m_Player.Move(m_AlgorithmSteps[0]);
                m_AlgorithmSteps.RemoveAt(0);
                StartCoroutine(ExampleCoroutine());
                m_NextButton.SetActive(IsLevelComplete());
            }
        }
        else if (moveInput.sqrMagnitude > 0.5)
        {
            if (m_ReadyForInput)
            {
                m_ReadyForInput = false;
                m_Player.Move(moveInput);
                m_NextButton.SetActive(IsLevelComplete());
            }
        }
        else
        {
            m_ReadyForInput = true;
        }
    }

    public void NextLevel()
    {
        m_NextButton.SetActive(false);
        m_LevelBuilder.NextLevel();
        StartCoroutine(ResetSceneAsync());

    }

    bool IsLevelComplete()
    {
        Yarn[] yarns = GameObject.FindObjectsOfType<Yarn>();
        GameObject[] cats = GameObject.FindGameObjectsWithTag("Cat");
        int count = 0;
        foreach (var yarn in yarns)
        {
            if (yarn.m_OnCat)
            {
                count += 1;
            }
        }

        return cats.Length <= count;
    }

    public void ResetScene()
    {
        StartCoroutine(ResetSceneAsync());
    }

    IEnumerator ResetSceneAsync()
    {
        if (SceneManager.sceneCount > 1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("LevelScene");
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
            Resources.UnloadUnusedAssets();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LevelScene"));
        m_LevelBuilder.Build();
        m_Player = FindObjectOfType<Player>();
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        m_ReadyForInput = true;
    }

    public void ExecuteAlgorithm()
    {
        List<Direction> victoryPath = this.GetPath();

        List<Vector2> algorithmVectors = new List<Vector2>();
        foreach (var direction in victoryPath)
        {
            switch (direction)
            {
                case (Direction.LEFT):
                    algorithmVectors.Add(new Vector2(-1, 0));
                    break;
                case (Direction.RIGHT):
                    algorithmVectors.Add(new Vector2(1, 0));
                    break;
                case (Direction.UP):
                    algorithmVectors.Add(new Vector2(0, -1));
                    break;
                case (Direction.DOWN):
                    algorithmVectors.Add(new Vector2(0, 1));
                    break;
            }
        }
        m_AlgorithmSteps = algorithmVectors;
    }

    List<Direction> GetPath()
    {
        State state = new State("Assets\\Resources\\Levels.txt.txt", m_LevelBuilder.m_CurrentLevel);
        switch (m_AlgorithmSelector.text)
        {
            case ("DFS"): return new DFS().solve(state);
            case ("BFS"): return new BFS().solve(state);
            case ("Q learning"):
                QFeatureAgent agent = new QFeatureAgent(0.8f, 0.4f, 0.75f);
                QHarness harness = new QHarness(agent, state);
                return harness.train(1000);
            case ("A*"): return new AStar(new UnMatchedBoxesHeuristic()).solve(state);
            default: Debug.Log("Invalid dropdown option"); return new BFS().solve(state);
        }
    }
}
