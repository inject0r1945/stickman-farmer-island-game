using HCGame.Utils.Static;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Worlds
{
    public class ChunkWalls : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private GameObject _frontWall;
        [SerializeField] private GameObject _rightWall;
        [SerializeField] private GameObject _bottomWall;
        [SerializeField] private GameObject _leftWall;

        public static readonly int FrontWallBits = 0b_0001;
        public static readonly int RightWallBits = 0b_0010;
        public static readonly int BottomWallBits = 0b_0100;
        public static readonly int LeftWallBits = 0b_1000;

        private const int WallsCount = 4;

        public static int[] WallsBits => new int[WallsCount] { FrontWallBits, RightWallBits, BottomWallBits, LeftWallBits };

        public void Init(int configuration)
        {
            UpdateWallState(configuration, FrontWallBits, _frontWall);
            UpdateWallState(configuration, RightWallBits, _rightWall);
            UpdateWallState(configuration, BottomWallBits, _bottomWall);
            UpdateWallState(configuration, LeftWallBits, _leftWall);
        }

        private void UpdateWallState(int configuration, int wallBits, GameObject wall)
        {
            bool isActiveWall = !StaticUtils.IsBitsSet(configuration, wallBits);
            wall.SetActive(isActiveWall);
        }
    }
}