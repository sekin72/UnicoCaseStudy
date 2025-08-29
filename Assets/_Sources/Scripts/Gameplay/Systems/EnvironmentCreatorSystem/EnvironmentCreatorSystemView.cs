using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnicoCaseStudy.Configs;
using UnicoCaseStudy.Gameplay.Logic;
using UnicoCaseStudy.Managers.Asset;
using UnicoCaseStudy.Managers.Gameplay;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnicoCaseStudy.Gameplay.Systems.EnvironmentCreatorSystem
{
    public class EnvironmentCreatorSystemView : MonoBehaviour, IDisposable
    {
        [SerializeField] private AssetReferenceT<GameObject> _forestGOReference;
        [SerializeField] private AssetReferenceT<GameObject> _villageGOReference;
        private GameObject _forestGO;
        private GameObject _villageGO;

        private Tile[,] _groundTileArray;
        private Tile[,] _gameplayTileArray;

        private GameObject _boardParent;

        private readonly List<GameObject> _groundTilesList = new();
        private readonly List<GameplayTile> _gameplayTiles = new();

        private AddressableManager _addressableManager;
        private PoolManager _poolManager;

        private GameSettings _gameSettings;

        private PoolKeys _tileKey;

        private int _totalWidth;
        private int _totalHeight;
        private int _gameplayWidth;
        private int _gameplayHeight;

        private Camera _camera;

        public void Initialize(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            SetParameters();

            for (var i = 0; i < _totalWidth; i++)
            {
                for (var j = 0; j < _totalHeight; j++)
                {
                    _groundTileArray[i, j] = new Tile
                    {
                        TileObject = _poolManager.GetGameObject(_tileKey),
                        BoardIndex = new Vector2Int(i, j),
                        Position = new Vector3(i, j, 0),
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
            _addressableManager = AppManager.GetManager<AddressableManager>();
            _poolManager = AppManager.GetManager<PoolManager>();
            _camera = AppManager.GetManager<GameplayManager>().GameplaySceneController.SceneCamera;

            _boardParent = new GameObject("Board");
            _boardParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            _tileKey = PoolKeys.Tile;
            _totalWidth = _gameSettings.TotalWidth;
            _totalHeight = _gameSettings.TotalHeight;
            _gameplayWidth = _gameSettings.GameplayWidth;
            _gameplayHeight = _gameSettings.GameplayHeight;

            _groundTileArray = new Tile[_totalWidth, _totalHeight];
            _gameplayTileArray = new Tile[_gameplayWidth, _gameplayHeight];
        }

        public void Dispose()
        {
            _addressableManager.ReleaseInstance(_forestGO);

            DisposeGroundTiles();
            DisposeGameplayTiles();

            GameObject.Destroy(_boardParent);
        }

        public Tile GetTile(Vector2Int index)
        {
            return _groundTileArray[index.x, index.y];
        }

        public async UniTask CreateGroundTiles(CancellationToken cancellationToken)
        {
            var poolKey = PoolKeys.GroundTile;

            _forestGO = await _addressableManager.InstantiateAssetAsync(_forestGOReference, cancellationToken);
            int forestX = ((_totalWidth - _gameplayWidth) / 2) - 2;
            int forestY = ((_totalHeight - _gameplayHeight) / 2) + _gameplayHeight;

            _villageGO = await _addressableManager.InstantiateAssetAsync(_villageGOReference, cancellationToken);
            int villageX = ((_totalWidth - _gameplayWidth) / 2) - 2;
            int villageY = ((_totalHeight - _gameplayHeight) / 2) - 1;

            for (var i = 0; i < _totalWidth; i++)
            {
                for (var j = 0; j < _totalHeight; j++)
                {
                    var tile = _groundTileArray[i, j];
                    var groundTile = _poolManager.GetGameObject(poolKey);
                    groundTile.transform.SetParent(tile.TileObject.transform);
                    groundTile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                    groundTile.transform.localScale = Vector3.one;
                    _groundTilesList.Add(groundTile);

                    var groundTileComponent = groundTile.GetComponent<GroundTile>();
                    groundTileComponent.SpriteRenderer.sprite = j >= _totalHeight / 2 ?
                        _gameSettings.GreySpriteWrapper.BG :
                        _gameSettings.DarkGreenSpriteWrapper.BG;

                    if (i == forestX && j == forestY)
                    {
                        _forestGO.transform.SetParent(tile.TileObject.transform);
                        _forestGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                        _forestGO.transform.localScale = Vector3.one;
                    }

                    if (i == villageX && j == villageY)
                    {
                        _villageGO.transform.SetParent(tile.TileObject.transform);
                        _villageGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                        _villageGO.transform.localScale = Vector3.one;
                    }
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

                    var (isChecker, overrideSprite) = DecideBackgroundSprite(i, j);

                    var gameplayTile = _poolManager.GetGameObject(PoolKeys.GameplayTile).GetComponent<GameplayTile>();
                    gameplayTile.transform.SetParent(tile.TileObject.transform);
                    gameplayTile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                    gameplayTile.transform.localScale = Vector3.one;
                    gameplayTile.Initialize(tile, new Vector2Int(i, j), (_gameplayHeight - j) * 100, isChecker, overrideSprite: overrideSprite);
                    _gameplayTiles.Add(gameplayTile);
                }
            }
        }

        public void FinalBoardAdjustments()
        {
            CenterBoardToShowPathTiles();

            _boardParent.transform.position += Vector3.up * _gameSettings.BoardYOffset;

            _boardParent.gameObject.SetActive(false);
            _boardParent.gameObject.SetActive(true);
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
                _gameplayTiles[i].Dispose();
                _poolManager.SafeReleaseObject(PoolKeys.GameplayTile, _gameplayTiles[i].gameObject);
            }

            _gameplayTiles.Clear();
            _gameplayTileArray = null;
        }

        private void CenterBoardToShowPathTiles()
        {
            Vector3 camPos = _camera.transform.position;
            Vector3 camDir = _camera.transform.forward;

            float t = -camPos.z / camDir.z;
            Vector3 screenPlanePoint = camPos + (camDir * t); // hit XY plane (z = 0)

            float boardWorldWidth = (_totalWidth - 1);
            float boardWorldHeight = (_totalHeight - 1);
            Vector3 boardCenter = new(boardWorldWidth * 0.5f, boardWorldHeight * 0.5f, 0f);

            _boardParent.transform.position = screenPlanePoint - boardCenter;

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

        private (bool, Sprite) DecideBackgroundSprite(int i, int j)
        {
            bool isBottom = j == 0;
            bool isTop = j == _gameplayHeight - 1;
            bool isChecker = false;

            BackgroundSpriteWrapper wrapper;

            if (j < _gameSettings.DefencePlaceHeight)
            {
                isChecker = (i + j) % 2 == 0;
                wrapper = _gameSettings.DarkGreenSpriteWrapper;
            }
            else
            {
                wrapper = _gameSettings.GreySpriteWrapper;
            }

            return (isTop, isBottom) switch
            {
                (true, _) => (isChecker, wrapper.Top),
                (_, true) => (isChecker, wrapper.Bottom),
                _ => (isChecker, wrapper.Middle)
            };
        }
    }
}
