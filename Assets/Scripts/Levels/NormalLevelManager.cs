using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Levels
{
    public class NormalLevelManager : LevelManager
    {
        [SerializeField]
        private List<Transform> blockSpawnPositions;

        [SerializeField] private float initialMultiplier = 1.5f;
        [SerializeField] private float multiplierIncrement = 0.5f;
        [SerializeField] private AudioClip matchSound, blockPlaceSound;

        private List<BlockFormation> _currentToPlaceBlocks = new List<BlockFormation>();
        private BoardManager _boardManager;
        private BlockManager _blockManager;
        private bool _isPressing = false;
        private Camera _mainCamera;
        private InputManager _inputManager;
        private SoundManager _soundManager;
        private BlockFormation _currentSelectedPiece;
        private float _currentMultiplier = 1.5f;

        private void Start()
        {
            _mainCamera = Camera.allCameras.Single(x => x.name == "LevelCamera");
            
            // TODO this is temp, replace with button from main menu or restart
            SetupLevel();
            StartLevel();
        }

        private void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _blockManager = FindObjectOfType<BlockManager>();
            _soundManager = FindObjectOfType<SoundManager>();
        }

        public override void SetupLevel()
        {
            _boardManager = new BoardManager(9, 9);
            _blockManager.SetSpawnLocations(blockSpawnPositions);

            // In case we're restarting, destroy all blocks
            var blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                Destroy(block);
            }
            
            // Set score to 0 in case we're restarting
            UpdateScore(0);
        }

        public override void StartLevel()
        {
            if (ReferenceEquals(_boardManager, null))
                throw new Exception("Start level but board manager is null, did you forget to call setup level?");
            
            _currentToPlaceBlocks = _blockManager.SpawnBlocks();
        }

        private void OnEnable()
        {
            if (_inputManager is null)
            {
                return;
            }
            
            Debug.Log("Registered OnPressedEvent with InputManager", this);
            _inputManager.OnPressedEvent += OnPressedEvent;
        }

        private void OnDisable()
        {
            if (_inputManager is null)
                return;
            
            Debug.Log("UnRegistered OnPressedEvent with InputManager", this);
            _inputManager.OnPressedEvent -= OnPressedEvent;
        }

        private void Update()
        {
            if (!_isPressing || ReferenceEquals(_boardManager, null))
                return;
            
            Vector2 position = _mainCamera.ScreenToWorldPoint(new Vector3(_inputManager.SelectLocation.x,
                _inputManager.SelectLocation.y));

            // If we don't have a selected piece and we're clicking, then we should try and get one
            if (_currentSelectedPiece is null)
            {
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

                if (hit != null && !ReferenceEquals(hit.collider, null) && hit.collider.gameObject.CompareTag("BlockFormation"))
                    PickupPiece(hit.collider.gameObject);
            }

            // If we've gotten here, but we're not holding anything then just leave
            if (_currentSelectedPiece is null)
                return;

            // We have selected a piece so move it!
            _currentSelectedPiece.transform.position = position;
        }

        private void OnPressedEvent()
        {
            _isPressing = !_isPressing;

            // If we're pressing there is nothing else to do
            if (_isPressing)
                return;

            // If we don't have a current active piece nothing else to do
            if (_currentSelectedPiece == null)
                return;
            
            PlacePiece();
        }

        private void PlacePiece()
        {
            // If we aren't pressing, determine if we can place the piece, if so place it, if not return it.
            _currentSelectedPiece.ResetChildSizeAndLayer();
            
            if (_boardManager.IsValidPosition(_currentSelectedPiece))
            {
                int score = Score + _currentSelectedPiece.blocks.Count;
                int matchedPieces = _boardManager.PlacePiece(_currentSelectedPiece);

                if (matchedPieces == 0) _soundManager.PlaySound(blockPlaceSound);
                
                score = OnMatchFound(matchedPieces, score);
                UpdateScore(score);
                
                _currentToPlaceBlocks.Remove(_currentSelectedPiece);
                _currentSelectedPiece = null;

                if(_currentToPlaceBlocks.Count == 0)
                    _currentToPlaceBlocks = _blockManager.SpawnBlocks();

                CheckForValidMoves();
            }
            else
            {
                _currentSelectedPiece.ResetLocation();
                _currentSelectedPiece = null;
            }
        }

        private void PickupPiece(GameObject piece)
        {
            var formation = piece.GetComponent<BlockFormation>();
            if (formation.IsDisabled)
                return;

            _currentSelectedPiece = formation;
            _currentSelectedPiece.ShrinkChildrenAndBringToFront();
        }

        private int OnMatchFound(int numberMatched, int currentScore)
        {
            if (numberMatched == 0)
            {
                _currentMultiplier = initialMultiplier;
                return currentScore;
            }

            _soundManager.PlaySound(matchSound);

            int score = currentScore + Mathf.RoundToInt(numberMatched * _currentMultiplier);
            _currentMultiplier += multiplierIncrement;
            return score;
        }

        private void CheckForValidMoves()
        {
            bool hasMove = false;

            foreach (BlockFormation formation in _currentToPlaceBlocks)
            {
                if (_boardManager.CheckForMove(formation))
                {
                    hasMove = true;
                    formation.EnableFormation();
                }
                else
                {
                    formation.DisableFormation();
                }
            }

            if (!hasMove) 
                GameOver();
        }
    }
}