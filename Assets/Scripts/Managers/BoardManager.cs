using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers
{
    public class BoardManager
    {
        private readonly GameObject[][] _blockPositions;
        
        /// <summary>
        /// Instantiate a board with a given width and height, the board should be at 0,0 in world space.
        /// This is because most of the logic depends on x and y in world space.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoardManager(int width, int height)
        {
            _blockPositions = new GameObject[9][];
            for (int i = 0; i < _blockPositions.Length; i++)
            {
                _blockPositions[i] = new GameObject[9];
            }
        }

        /// <summary>
        /// Checks if the given block formation is in a valid position and can be placed.
        /// </summary>
        /// <param name="blockFormation">The block formation to check</param>
        /// <returns>True if the position is valid, false if not</returns>
        /// <exception cref="ArgumentException">If there are no blocks in the given block formation.</exception>
        public bool IsValidPosition(BlockFormation blockFormation)
        {
            if (blockFormation.blocks.Count == 0)
                throw new ArgumentException("There are no blocks in the given block formation", nameof(blockFormation));
            
            foreach (GameObject block in blockFormation.blocks)
            {
                var blockComponent = block.GetComponent<Block>();
                Vector2 noOffsetXY = blockComponent.GetRoundedPosition();
                int blockX = (int)noOffsetXY.x;
                int blockY = (int)noOffsetXY.y;

                if (blockX < 0 || blockX > _blockPositions[0].Length - 1)
                    return false;

                if (blockY < 0 || blockY > _blockPositions.Length - 1)
                    return false;

                if (!ReferenceEquals(_blockPositions[blockY][blockX], null))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Will place the block formation on the board, but will throw an exception if the block is in an invalid position.
        /// </summary>
        /// <remarks>You should call <see cref="IsValidPosition"/> before calling this method to check if the <see cref="BlockFormation"/> is in a valid position</remarks>
        /// <param name="blockFormation">The block formation to place</param>
        /// <returns>The number of resulting matches (if any, 0 if there aren't)</returns>
        /// <exception cref="InvalidOperationException">When the block formation to place is not a valid placement</exception>
        public int PlacePiece(BlockFormation blockFormation)
        {
            if (!IsValidPosition(blockFormation))
                throw new InvalidOperationException("Tried placing a block in an invalid position.");
            
            var toRemove = new List<GameObject>();

            foreach (GameObject block in blockFormation.blocks)
            {
                // Remove the parent because the block should be free!!
                block.transform.SetParent(null);

                var blockComponent = block.GetComponent<Block>();
                Vector2 noOffsetXY = blockComponent.GetRoundedPosition();
                int blockX = (int)noOffsetXY.x;
                int blockY = (int)noOffsetXY.y;
                block.transform.position = blockComponent.GetPositionWithOffset();

                _blockPositions[blockY][blockX] = block;

                var sectionBlocks = GetBlocksInSection(blockX, blockY);
                if (sectionBlocks.Count == 9)
                {
                    Debug.Log("Section match found!");
                    toRemove.AddRange(sectionBlocks);
                }

                var rowBlocks = GetBlocksInRow(blockY);
                if (rowBlocks.Count == 9)
                {
                    Debug.Log("Row match found!");
                    toRemove.AddRange(rowBlocks);
                }

                var columnBlocks = GetBlocksInColumn(blockX);
                if(columnBlocks.Count == 9)
                {
                    Debug.Log("Column match found!");
                    toRemove.AddRange(columnBlocks);
                }
            }
            
            // Destroy the block formation because it's no longer needed.
            Object.Destroy(blockFormation.gameObject);

            if (toRemove.Count <= 0) return 0;

            int totalMatches = toRemove.Count;
            foreach (GameObject block in toRemove)
            {
                var blockComponent = block.GetComponent<Block>();
                Vector2 noOffsetXY = blockComponent.GetRoundedPosition();
                int blockX = (int)noOffsetXY.x;
                int blockY = (int)noOffsetXY.y;

                _blockPositions[blockY][blockX] = null;
                block.GetComponent<Block>().Destroy();
            }

            return totalMatches;
        }

        public bool CheckForMove(BlockFormation formation)
        {
            
            for (int y = 0; y < _blockPositions.Length; y++)
            {
                for (int x = 0; x < _blockPositions[y].Length; x++)
                {
                    // This position has a block
                    if (!ReferenceEquals(_blockPositions[y][x], null))
                        continue;

                    bool isValid = true;
                    // Loop through each block in the formations
                    // Check that all of the blocks fit in the grid
                    // On the first instance where every block has a legal location, then return true
                    foreach (GameObject block in formation.blocks)
                    {
                        var blockComponent = block.GetComponent<Block>();
                        int xToCheck = x + blockComponent.formationX;
                        int yToCheck = y + blockComponent.formationY;

                        if (xToCheck < 0 || xToCheck > _blockPositions[0].Length - 1)
                        {
                            isValid = false;
                            break;
                        }

                        if (yToCheck < 0 || yToCheck > _blockPositions.Length - 1)
                        {
                            isValid = false;
                            break;
                        }

                        if (!ReferenceEquals(_blockPositions[yToCheck][xToCheck], null))
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if(isValid)
                        return true;
                }
            }

            return false;
        }

        public List<GameObject> GetBlocksInSection(int toCheckX, int toCheckY)
        {
            int beginningX = toCheckX - (toCheckX % 3);
            int beginningY = toCheckY - (toCheckY % 3);
            var matches = new List<GameObject>();

            for (int x = beginningX; x < beginningX + 3; x++)
            {
                for (int y = beginningY; y < beginningY + 3; y++)
                {
                    if(_blockPositions[y][x] != null)
                        matches.Add(_blockPositions[y][x]);
                }
            }

            return matches;
        }

        public List<GameObject> GetBlocksInRow(int toCheckY)
        {
            var matches = new List<GameObject>();
            for (int x = 0; x < _blockPositions[0].Length; x++)
            {
                if(_blockPositions[toCheckY][x]  != null)
                    matches.Add(_blockPositions[toCheckY][x] );
            }

            return matches;
        }

        public List<GameObject> GetBlocksInColumn(int toCheckX)
        {
            var matches = new List<GameObject>();
            for (int y = 0; y < _blockPositions.Length; y++)
            {
                if(_blockPositions[y][toCheckX]  != null)
                    matches.Add(_blockPositions[y][toCheckX] );
            }

            return matches;
        }
    }
}