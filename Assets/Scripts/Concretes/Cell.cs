using TileMatchGame.Controller;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileMatchGame
{
	public class Cell : MonoBehaviour
	{
        //[HideInInspector] public int X;
		//[HideInInspector] public int Y;
		[HideInInspector] public Vector2Int Position;
        [HideInInspector] public int Tier;
        [HideInInspector] public Cell FallStopPosition;
		[HideInInspector] public bool IsTopRowCell;
        
		ItemController _item;
        Board _board;
		float _distanceBetweenItems;

        List<Cell> _neighbours = new List<Cell>();
		public List<Cell> Neighbours => _neighbours;
		public ItemController Item
		{
			get
			{
				return _item;
			}
			set
			{
				if (_item == value) return;

				var oldItem = _item;
				_item = value;

				if (oldItem != null && Equals(oldItem.Cell, this))
				{
					oldItem.Cell = null;
				}
				if (value != null)
				{
					value.Cell = this;
				}
			}
		}

		public void Init()
		{
			//X = x;
			//Y = y;
			var gameManager = GameManager.Instance;
			var level = gameManager.Level;
			//IsTopRowCell = Y == level.Rows - 1;
			_distanceBetweenItems = level.DistanceBetweenItems;
			_board = gameManager.Board;
			transform.localPosition = new Vector3(Position.x * _distanceBetweenItems, Position.y * _distanceBetweenItems);
            ArrangeCellName();
			UpdateNeighbours();
		}

		private void UpdateNeighbours()
		{
			var neighBourDirections = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach (Direction direction in neighBourDirections)
            {
				var neighbour = _board.GetNeighbourWithDirection(this, direction);
				if (neighbour == null) continue;
                _neighbours.Add(neighbour);
				if(direction == Direction.Down)
				{
					FallStopPosition = neighbour;
				}
            }
		}

		public List<Cell> CellsInTheBombBoosterArea()
		{
			var _cellsInTheBombBoosterArea = new List<Cell>();
			foreach (Direction direction in Enum.GetValues(typeof(Direction)))
			{
				var cell = _board.GetNeighbourWithDirection(this, direction);
				if(cell == null) continue;
                _cellsInTheBombBoosterArea.Add(cell);
			}
			return _cellsInTheBombBoosterArea;
		}

		private void ArrangeCellName()
		{
			gameObject.name = $"Cell {Position.x}:{Position.y} [{Tier}]";
		}

		public Cell GetFallTargetCell()
		{
			var fallTargetCell = this;
			while (fallTargetCell.FallStopPosition != null && fallTargetCell.FallStopPosition.Item == null)
			{
				fallTargetCell = fallTargetCell.FallStopPosition;
			}
			return fallTargetCell;
		}
	}
}
