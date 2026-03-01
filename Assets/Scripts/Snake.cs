using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    private float _startTime;
    private int _score = 0;
    private int _highScore = 0;

    private static bool _shouldSkipHome = false;

    [Header("Movement Settings")]
    public float moveInterval = 0.15f;
    public float moveStep = 0.5f; 
    private float _nextMoveTime;

    [Header("Required Prefabs & Objects")]
    public GameObject bodyPrefab;
    
    [Header("Home Screen UI")]
    public GameObject homePanel;        
    public GameObject aboutPanel; 
    public TextMeshProUGUI menuHighScoreText; 

    [Header("Gameplay UI")]
    public TextMeshProUGUI scoreText;      
    public GameObject mobileControls; 

    [Header("Game Over UI")]
    public GameObject gameOverScreen;      
    public TextMeshProUGUI finalScoreText; 
    public TextMeshProUGUI highScoreText;  

    private void Start()
    {
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
        
        if (menuHighScoreText != null) {
            menuHighScoreText.text = "Best Score: " + _highScore;
        }

        if (_shouldSkipHome) {
            StartGame();
        } else {
            ShowHomeScreen();
        }

        ResetSnake();
    }

    private void ShowHomeScreen()
    {
        Time.timeScale = 0;
        if (homePanel != null) homePanel.SetActive(true);
        if (aboutPanel != null) aboutPanel.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (mobileControls != null) mobileControls.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        if (homePanel != null) homePanel.SetActive(false);
        if (aboutPanel != null) aboutPanel.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (mobileControls != null) mobileControls.SetActive(true);
        
        _startTime = Time.time;
        _score = 0;
        UpdateScoreUI();
    }

    public void RestartGame()
    {
        _shouldSkipHome = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        _shouldSkipHome = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenAbout() {
        if (homePanel != null) homePanel.SetActive(false);
        if (aboutPanel != null) aboutPanel.SetActive(true);
    }

    public void CloseAbout() {
        if (aboutPanel != null) aboutPanel.SetActive(false);
        if (homePanel != null) homePanel.SetActive(true);
    }

    public void MoveUp() { if (_direction != Vector2.down) _direction = Vector2.up; }
    public void MoveDown() { if (_direction != Vector2.up) _direction = Vector2.down; }
    public void MoveLeft() { if (_direction != Vector2.right) _direction = Vector2.left; }
    public void MoveRight() { if (_direction != Vector2.left) _direction = Vector2.right; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) MoveUp();
        else if (Input.GetKeyDown(KeyCode.S)) MoveDown();
        else if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        else if (Input.GetKeyDown(KeyCode.D)) MoveRight();
    }

    private void FixedUpdate()
    {
        if (Time.time < _nextMoveTime) return;
        _nextMoveTime = Time.time + moveInterval;

        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position = new Vector3(
            this.transform.position.x + (_direction.x * moveStep),
            this.transform.position.y + (_direction.y * moveStep),
            0.0f
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - _startTime < 0.1f) return;

        if (other.CompareTag("Food")) {
            _score += 10;
            if (_score > _highScore) {
                _highScore = _score;
                PlayerPrefs.SetInt("HighScore", _highScore);
            }
            UpdateScoreUI();
            Grow();
        } 
        else if (other.CompareTag("Obstacle")) {
            int segmentIndex = -1;
            for (int i = 0; i < _segments.Count; i++) {
                if (_segments[i] == other.transform) { segmentIndex = i; break; }
            }
            if (segmentIndex == -1 || segmentIndex >= 4) {
                GameOver();
            }
        }
    }

    private void Grow()
    {
        GameObject segment = Instantiate(bodyPrefab);
        segment.transform.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment.transform);
    }

    private void ResetSnake()
    {
        for (int i = 1; i < _segments.Count; i++) {
            if(_segments[i] != null) Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        this.transform.position = Vector3.zero;
        _direction = Vector2.right;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + _score;
        if (finalScoreText != null) finalScoreText.text = "Final Score: " + _score;
        if (highScoreText != null) highScoreText.text = "High Score: " + _highScore;
    }

    private void GameOver()
    {
        PlayerPrefs.Save();
        Time.timeScale = 0;
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (mobileControls != null) mobileControls.SetActive(false);
    }
}