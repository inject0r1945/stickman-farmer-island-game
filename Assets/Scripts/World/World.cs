using HCGame.Saving;
using HCGame.Utils.Static;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Saving.Json;
using Zenject;

namespace HCGame.Worlds
{
    public class World : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private int _gridSize = 20;
        [SerializeField] private int _gridScale = 8;
        [SerializeField] private WorldState _worldState;

        [Header("Chunk Meshes")]
        [SerializeField] private Mesh[] _chunkMeshes;

        private const int ChunkNeighborsCount = 4;

        private JsonSavingWrapper _savingWrapper;
        private string _chunkCurrentPriceSaveName = "CurrentPrice";
        private string _isChangedPriceChunkByPlayerSaveName = "IsChangedPriceByPlayer";
        private string _isUnlockedChunkSaveName = "IsUnlocked";
        private string _initialPriceChunkSaveName = "InitialPrice";
        private Chunk[,] _grid;
        private Dictionary<int, ChunkShape> _configurationToShapeDictionary;

        private enum ChunkShape {
            None,
            Top,
            Right,
            TopRight,
            Bottom,
            BottomRight,
            Left,
            TopLeft,
            BottomLeft,
            Four
        };

        private enum WorldState { Normal, ShowAllLocked, UnlockAndShowAll }

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _configurationToShapeDictionary = new Dictionary<int, ChunkShape>()
            {
                { 0b_0000, ChunkShape.None },
                { 0b_0001, ChunkShape.Top },
                { 0b_0010, ChunkShape.Right },
                { 0b_0011, ChunkShape.TopRight },
                { 0b_0100, ChunkShape.Bottom },
                { 0b_0101, ChunkShape.Four },
                { 0b_0110, ChunkShape.BottomRight },
                { 0b_0111, ChunkShape.Four },
                { 0b_1000, ChunkShape.Left },
                { 0b_1001, ChunkShape.TopLeft },
                { 0b_1010, ChunkShape.Four },
                { 0b_1011, ChunkShape.Four },
                { 0b_1100, ChunkShape.BottomLeft },
                { 0b_1101, ChunkShape.Four },
                { 0b_1110, ChunkShape.Four },
                { 0b_1111, ChunkShape.Four }
            };

            InitializeGrid();
        }

        private void OnEnable()
        {
            Chunk.PriceChanged += OnChunkPriceChange;
            Chunk.Unlocked += OnChunkUnlocked;
        }

        private void OnDisable()
        {
            Chunk.PriceChanged -= OnChunkPriceChange;
            Chunk.Unlocked -= OnChunkUnlocked;
        }

        private void Start()
        {
            if (_worldState == WorldState.UnlockAndShowAll)
            {
                UnlockAll();
            }

            UpdateAllChunksRenders();
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            for (int i = 0; i < transform.childCount; i++)
            {
                Chunk chunk = transform.GetChild(i).GetComponent<Chunk>();
                JObject chunkState = new JObject();
                IDictionary<string, JToken> chunkStateDictionary = chunkState;
                chunkStateDictionary.Add(_chunkCurrentPriceSaveName, chunk.CurrentPrice);
                chunkStateDictionary.Add(_isChangedPriceChunkByPlayerSaveName, chunk.IsChangedPriceByPlayer);
                chunkStateDictionary.Add(_isUnlockedChunkSaveName, chunk.IsUnlocked);
                chunkStateDictionary.Add(_initialPriceChunkSaveName, chunk.InitialPrice);
                stateDictionary.Add(chunk.UniqueIdentifier, chunkState);
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            IDictionary<string, JToken> stateDictionary = (IDictionary<string, JToken>)state;

            for (int i = 0; i < transform.childCount; i++)
            {
                Chunk chunk = transform.GetChild(i).GetComponent<Chunk>();

                if (chunk == null)
                    continue;

                if (!stateDictionary.ContainsKey(chunk.UniqueIdentifier))
                    continue;

                if (!stateDictionary.TryGetValue(chunk.UniqueIdentifier, out JToken chunkState))
                    continue;

                IDictionary<string, JToken> chunkStateDictionary = (IDictionary<string, JToken>)chunkState;

                bool isChangedPriceByPlayer = false;

                if (chunkStateDictionary.TryGetValue(_isChangedPriceChunkByPlayerSaveName, out JToken isChangedPriceByPlayerToken))
                {
                    isChangedPriceByPlayer = isChangedPriceByPlayerToken.ToObject<bool>();
                }

                if (chunkStateDictionary.TryGetValue(_chunkCurrentPriceSaveName, out JToken chunkCurrentPriceToken) && isChangedPriceByPlayer)
                {

                    chunk.Init(currentPrice: chunkCurrentPriceToken.ToObject<int>(), isChangedPriceByPlayer: true);
                }
                else
                {
                    chunk.Init();
                }

                CalculateChunksSegment3x3Visualisation(chunk);
            }
        }

        private void InitializeGrid()
        {
            _grid = new Chunk[_gridSize, _gridSize];
            Vector2Int gridCenter = new Vector2Int(_gridSize / 2, _gridSize / 2);

            foreach (Chunk chunk in GetComponentsInChildren<Chunk>())
            {
                Vector2Int chunkGridPosition = new Vector2Int((int)(chunk.transform.position.x / _gridScale),
                    (int)(chunk.transform.position.z / _gridScale));

                chunkGridPosition += gridCenter;

                _grid[chunkGridPosition.x, chunkGridPosition.y] = chunk;
            }
        }

        private void OnChunkPriceChange(Chunk chunk)
        {
            _savingWrapper.SendSignalToSave();
        }

        private void OnChunkUnlocked(Chunk chunk)
        {
            _savingWrapper.SendSignalToSave();
            CalculateChunksSegment3x3Visualisation(chunk);
        }

        private void CalculateChunksSegment3x3Visualisation(Chunk chunk)
        {
            if (chunk == null)
                return;

            CalculateChunkVisualisation(chunk, out List<Chunk> neighboringChunks);

            if (chunk.IsUnlocked)
            {
                chunk.gameObject.SetActive(true);
                EnableChunks(neighboringChunks);
            }  
            else if (neighboringChunks.Where(neighborChunk => neighborChunk != null && neighborChunk.IsUnlocked).FirstOrDefault() != null 
                || _worldState == WorldState.ShowAllLocked)
            {
                chunk.gameObject.SetActive(true);
            }
            else
            {
                chunk.gameObject.SetActive(false);
            }

            foreach (Chunk neighboringChunk in neighboringChunks)
            {
                CalculateChunkVisualisation(neighboringChunk);
            }
        }

        private void CalculateChunkVisualisation(Chunk chunk)
        {
            CalculateChunkVisualisation(chunk, out _);
        }

        private void CalculateChunkVisualisation(Chunk chunk, out List<Chunk> neighboringChunks)
        {
            neighboringChunks = null;

            if (chunk == null)
                return;

            neighboringChunks = GetNeighboringChunks(chunk);
            int chunkConfiguration = GetChunkConfiguration(neighboringChunks);
            chunk.UpdateWalls(chunkConfiguration);
            SetChunkMesh(chunkConfiguration, chunk);
        }

        private List<Chunk> GetNeighboringChunks(Chunk chunk)
        {
            InitChunkGridCoordinates(chunk, out int gridX, out int gridY);
            return GetNeighboringChunks(gridX, gridY);
        }

        private void InitChunkGridCoordinates(Chunk chunk, out int gridX, out int gridY)
        {
            int halfGridSize = _gridSize / 2;

            gridX = (int)chunk.transform.position.x / _gridScale + halfGridSize;
            gridY = (int)chunk.transform.position.z / _gridScale + halfGridSize;
        }

        private List<Chunk> GetNeighboringChunks(int gridX, int gridY)
        {
            int neighborShift = 1;
            List<Vector2Int> neighborsPositions = new List<Vector2Int>()
            {
                new Vector2Int(gridX, gridY + neighborShift),
                new Vector2Int(gridX + neighborShift, gridY),
                new Vector2Int(gridX, gridY - neighborShift),
                new Vector2Int(gridX - neighborShift, gridY)
            };

            List<Chunk> neighboringChunks = new List<Chunk>();

            foreach (Vector2Int neighborPosition in neighborsPositions)
            {
                Chunk neighboringChunk = (IsValidGridPosition(neighborPosition)) ? _grid[neighborPosition.x, neighborPosition.y] : null;
                neighboringChunks.Add(neighboringChunk);
            }

            return neighboringChunks;
        }

        private bool IsValidGridPosition(Vector2Int gridCoordinates)
        {
            return IsValidGridPosition(gridCoordinates.x, gridCoordinates.y);
        }

        private bool IsValidGridPosition(int gridX, int gridY)
        {
            if (gridX < 0 || gridX >= _gridSize)
                return false;

            if (gridY < 0 || gridY >= _gridSize)
                return false;

            return true;
        }

        private int GetChunkConfiguration(List<Chunk> neighboringChunks)
        {
            if (neighboringChunks.Count != ChunkNeighborsCount)
                throw new ArgumentOutOfRangeException(nameof(neighboringChunks));

            int chunkConfiguration = 0b_0000;

            for (int i = 0; i < ChunkWalls.WallsBits.Length; i++)
            {
                if (neighboringChunks[i] != null && neighboringChunks[i].IsUnlocked)
                    chunkConfiguration = chunkConfiguration ^ ChunkWalls.WallsBits[i];
            }

            return chunkConfiguration;
        }

        private void EnableChunks(List<Chunk> chunks)
        {
            SetChunksActiveState(chunks, true);
        }

        private void SetChunksActiveState(List<Chunk> chunks, bool state)
        {
            foreach (Chunk chunk in chunks)
            {
                if (chunk == null)
                    continue;

                CalculateChunkVisualisation(chunk);
                chunk.gameObject.SetActive(state);
            }
        }

        private void DisableChunks(List<Chunk> chunks)
        {
            SetChunksActiveState(chunks, false);
        }

        private void SetChunkMesh(int configuration, Chunk chunk)
        {
            ChunkShape chunkShape = _configurationToShapeDictionary[configuration];
            int meshIndex = (int)chunkShape;
            chunk.SetMesh(_chunkMeshes[meshIndex]);
        }

        private void UnlockAll()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    Chunk chunk = _grid[x, y];

                    if (chunk == null)
                        continue;

                    chunk.Init(currentPrice: 0);
                }
            }
        }

        private void UpdateAllChunksRenders()
        {
            Chunk currentChunk;

            for (int gridX = 0; gridX < _grid.GetLength(0); gridX++)
            {
                for (int gridY = 0; gridY < _grid.GetLength(1); gridY++)
                {
                    currentChunk = _grid[gridX, gridY];

                    if (currentChunk == null)
                        continue;

                    CalculateChunksSegment3x3Visualisation(currentChunk);
                }
            }
        }

        [NaughtyAttributes.Button]
        private void PrintTotalWorlsCost()
        {
            int totalCost = 0;

            foreach (Transform chunkTransform in transform)
            {
                Chunk chunk = chunkTransform.GetComponent<Chunk>();
                totalCost += chunk.InitialPrice;
            }

            Debug.Log($"Total world cost: {totalCost}");
        }
    }
}
