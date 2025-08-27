using System;
using System.Collections.Generic;
using GameClient.GameData;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems.EnvironmentCreatorSystem
{
    public class EnvironmentCreatorSystemView : MonoBehaviour, IDisposable
    {
        private Tile[,] _groundTileArray;
        private Tile[,] _gameplayTileArray;

        private GameObject _boardParent;

        private readonly List<GameObject> _groundTilesList = new();
        private readonly List<GameplayTile> _gameplayTiles = new();

        private PoolManager _poolManager;

        private EnvironmentData _environmentData;

        private PoolKeys _tileKey;
        private int _tileSize;

        private int _totalWidth;
        private int _totalHeight;
        private int _gameplayWidth;
        private int _gameplayHeight;

        private Camera _camera;

        public void Initialize(EnvironmentData environmentData)
        {
            _environmentData = environmentData;
            SetParameters();

            for (var i = 0; i < _totalWidth; i++)
            {
                for (var j = 0; j < _totalHeight; j++)
                {
                    _groundTileArray[i, j] = new Tile
                    {
                        TileObject = _poolManager.GetGameObject(_tileKey),
                        Index = new Vector2Int(i, j),
                        Position = new Vector3(i * _tileSize, 0, j * _tileSize),
                        TileID = 0
                    };

                    _groundTileArray[i, j].TileObject.name = $"Tile_{i}_{j}";
                    _groundTileArray[i, j].TileObject.transform.SetParent(_boardParent.transform);
                    _groundTileArray[i, j].TileObject.transform.position = _groundTileArray[i, j].Position;
                }
            }
        }

        private void SetParameters()
        {
            _poolManager = AppManager.GetManager<PoolManager>();
            _camera = AppManager.GetManager<GameplayManager>().GameplaySceneController.SceneCamera;

            _boardParent = new GameObject("Board");
            _boardParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.up * 180));

            _tileKey = PoolKeys.Tile;
            _tileSize = _environmentData.TileSize;
            _totalWidth = _environmentData.TotalWidth;
            _totalHeight = _environmentData.TotalHeight;
            _gameplayWidth = _environmentData.GameplayWidth;
            _gameplayHeight = _environmentData.GameplayHeight;

            _groundTileArray = new Tile[_totalWidth, _totalHeight];
            _gameplayTileArray = new Tile[_gameplayWidth, _gameplayHeight];
        }

        public void Dispose()
        {
            DisposeGroundTiles();
            DisposeGameplayTiles();

            GameObject.Destroy(_boardParent);
        }

        public Tile GetTile(Vector2Int index)
        {
            return _groundTileArray[index.x, index.y];
        }

        public void CreateGroundTiles()
        {
            var poolKey = PoolKeys.GroundTile;

            for (var i = 0; i < _totalWidth; i++)
            {
                for (var j = 0; j < _totalHeight; j++)
                {
                    var tile = _groundTileArray[i, j];
                    var groundTile = _poolManager.GetGameObject(poolKey);
                    groundTile.transform.SetParent(tile.TileObject.transform);
                    groundTile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                    groundTile.transform.localScale = new Vector3(1, 1, 1);
                    _groundTilesList.Add(groundTile);
                }
            }
        }

        public void CreateGameplayTiles()
        {
            int startX = (_totalWidth - _gameplayWidth) / 2;
            int startY = (_totalHeight - _gameplayHeight) / 2;

            for (int i = 0; i < _gameplayWidth; i++)
            {
                for (int j = 0; j < _gameplayHeight; j++)
                {
                    var tilePosition = new Vector2Int(startX + i, startY + j);
                    var tile = _groundTileArray[tilePosition.x, tilePosition.y];
                    _gameplayTileArray[i, j] = tile;

                    var gameplayTile = _poolManager.GetGameObject(PoolKeys.GameplayTile).GetComponent<GameplayTile>();
                    gameplayTile.transform.SetParent(tile.TileObject.transform);
                    gameplayTile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                    gameplayTile.transform.localScale = new Vector3(1, 1, 1);
                    gameplayTile.Initialize(tile);
                    _gameplayTiles.Add(gameplayTile);
                }
            }

            CenterBoardToShowPathTiles();
        }

        private void DisposeGroundTiles()
        {
            for (var i = 0; i < _groundTilesList.Count; i++)
            {
                _poolManager.SafeReleaseObject(PoolKeys.GroundTile, _groundTilesList[i]);
            }

            _groundTilesList.Clear();
            _groundTileArray = null;
        }

        private void DisposeGameplayTiles()
        {
            for (var i = 0; i < _gameplayTiles.Count; i++)
            {
                _poolManager.SafeReleaseObject(PoolKeys.GameplayTile, _gameplayTiles[i].gameObject);
            }

            _gameplayTiles.Clear();
            _gameplayTileArray = null;
        }

        private void CenterBoardToShowPathTiles()
        {
            Vector3 camPos = _camera.transform.position;
            Vector3 camDir = _camera.transform.forward;

            float t = -camPos.y / camDir.y;
            Vector3 groundPoint = camPos + (camDir * t);

            float boardWorldWidth = (_totalWidth - 1) * _tileSize;
            float boardWorldHeight = (_totalHeight - 1) * _tileSize;
            Vector3 boardCenter = new(boardWorldWidth * 0.5f, 0f, boardWorldHeight * 0.5f);

            _boardParent.transform.position = groundPoint - boardCenter;

            float step = 1f;
            int maxSteps = 100;

            for (int i = 0; i < maxSteps; i++)
            {
                if (AllPathTilesVisible())
                {
                    break;
                }

                _boardParent.transform.position += camDir * step;
            }
        }

        private bool AllPathTilesVisible()
        {
            var padding = 0.2f;

            foreach (var tile in _gameplayTiles)
            {
                Vector3 worldPos = tile.transform.position;
                Vector3 viewport = _camera.WorldToViewportPoint(worldPos);

                if (viewport.z < padding || viewport.x < padding || viewport.x > 1 - padding || viewport.y < padding || viewport.y > 1 - padding)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
