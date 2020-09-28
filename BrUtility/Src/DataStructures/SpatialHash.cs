using BlueRavenUtility;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility.DataStructures
{
	public class SpatialHash
	{
		/// <summary>
		/// wraps a Unit32,List<IAABBF> Dictionary. It's main purpose is to hash the int,int x,y coordinates into a single
		/// Uint32 key which hashes perfectly resulting in an O(1) lookup.
		/// </summary>
		private class IntIntDictionary
		{
			Dictionary<long, List<IAABBF>> _store = new Dictionary<long, List<IAABBF>>();


			/// <summary>
			/// computes and returns a hash key based on the x and y value. basically just packs the 2 ints into a long.
			/// </summary>
			/// <returns>The key.</returns>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			long GetKey(int x, int y)
			{
				return unchecked((long)x << 32 | (uint)y);
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Add(int x, int y, List<IAABBF> list)
			{
				_store.Add(GetKey(x, y), list);
			}


			/// <summary>
			/// removes the collider from the Lists the Dictionary stores using a brute force approach
			/// </summary>
			/// <param name="obj">Object.</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Remove(IAABBF obj)
			{
				foreach (var list in _store.Values)
				{
					if (list.Contains(obj))
						list.Remove(obj);
				}
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetValue(int x, int y, out List<IAABBF> list)
			{
				return _store.TryGetValue(GetKey(x, y), out list);
			}


			/// <summary>
			/// gets all the Colliders currently in the dictionary
			/// </summary>
			/// <returns>The all objects.</returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public HashSet<IAABBF> GetAllObjects()
			{
				var set = new HashSet<IAABBF>();

				foreach (var list in _store.Values)
					set.UnionWith(list);

				return set;
			}


			/// <summary>
			/// clears the backing dictionary
			/// </summary>
			public void Clear()
			{
				_store.Clear();
			}
		}
		public Rectangle GridBounds = new Rectangle();

		/// <summary>
		/// the size of each cell in the hash
		/// </summary>
		private int _cellSize;

		/// <summary>
		/// 1 over the cell size. cached result due to it being used a lot.
		/// </summary>
		private float inverseCellSize;

		/// <summary>
		/// cached box used for overlap checks
		/// </summary>
		private RectangleF overlapTestBox = RectangleF.Empty;
		
		/// <summary>
		/// the Dictionary that holds all of the data
		/// </summary>
		private IntIntDictionary cellDict = new IntIntDictionary();

		/// <summary>
		/// shared HashSet used to return collision info
		/// </summary>
		private HashSet<IAABBF> tempHashset = new HashSet<IAABBF>();

		public SpatialHash(int cellSize = 100)
		{
			_cellSize = cellSize;
			inverseCellSize = 1f / _cellSize;
		}


		/// <summary>
		/// gets the cell x,y values for a world-space x,y value
		/// </summary>
		/// <returns>The coords.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Point CellCoords(int x, int y)
		{
			return new Point((int)Math.Floor(x * inverseCellSize), (int)Math.Floor(y * inverseCellSize));
		}


		/// <summary>
		/// gets the cell x,y values for a world-space x,y value
		/// </summary>
		/// <returns>The coords.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Point CellCoords(float x, float y)
		{
			return new Point((int)Math.Floor(x * inverseCellSize), (int)Math.Floor(y * inverseCellSize));
		}


		/// <summary>
		/// gets the cell at the world-space x,y value. If the cell is empty and createCellIfEmpty is true a new cell will be created.
		/// </summary>
		/// <returns>The at position.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="createCellIfEmpty">If set to <c>true</c> create cell if empty.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private List<IAABBF> CellAtPosition(int x, int y, bool createCellIfEmpty = false)
		{
			List<IAABBF> cell = null;
			if (!cellDict.TryGetValue(x, y, out cell))
			{
				if (createCellIfEmpty)
				{
					cell = new List<IAABBF>();
					cellDict.Add(x, y, cell);
				}
			}

			return cell;
		}

		/// <summary>
		/// adds the object to the SpatialHash
		/// </summary>
		/// <param name="collider">Object.</param>
		public void Add(IAABBF collider)
		{
			RectangleF bounds = collider.AABB;
			Point p1 = CellCoords(bounds.x, bounds.y);
			Point p2 = CellCoords(bounds.Right, bounds.Bottom);

			// update our bounds to keep track of our grid size
			if (!GridBounds.Contains(p1))
				GridBounds.Union(p1);

			if (!GridBounds.Contains(p2))
				GridBounds.Union(p2);

			for (var x = p1.X; x <= p2.X; x++)
			{
				for (var y = p1.Y; y <= p2.Y; y++)
				{
					// we need to create the cell if there is none
					var c = CellAtPosition(x, y, true);
					c.Add(collider);
				}
			}
		}


		/// <summary>
		/// removes the object from the SpatialHash
		/// </summary>
		/// <param name="collider">Collider.</param>
		public void Remove(IAABBF collider)
		{
			RectangleF bounds = collider.AABB;
			var p1 = CellCoords(bounds.x, bounds.y);
			var p2 = CellCoords(bounds.Right, bounds.Bottom);

			for (var x = p1.X; x <= p2.X; x++)
			{
				for (var y = p1.Y; y <= p2.Y; y++)
				{
					// the cell should always exist since this collider should be in all queryed cells
					var cell = CellAtPosition(x, y);
					Assert.AssertNotNull(cell, string.Format("removing Collider [{0}] from a cell that it is not present in", collider), Logger.LogLevel.Error);
					if (cell != null)
						cell.Remove(collider);
				}
			}
		}

		/// <summary>
		/// removes the object from the SpatialHash using a brute force approach
		/// </summary>
		/// <param name="obj">Object.</param>
		public void ForceRemove(IAABBF obj)
		{
			cellDict.Remove(obj);
		}

		public void Clear()
		{
			cellDict.Clear();
		}


		/// <summary>
		/// returns all the Colliders in the SpatialHash
		/// </summary>
		/// <returns>The all objects.</returns>
		public HashSet<IAABBF> GetAllObjects()
		{
			return cellDict.GetAllObjects();
		}

		/// <summary>
		/// returns all objects in cells that the bounding box intersects
		/// </summary>
		/// <returns>The neighbors.</returns>
		/// <param name="bounds">Bounds.</param>
		public HashSet<IAABBF> Query(ref RectangleF bounds)
		{
			tempHashset.Clear();

			var p1 = CellCoords(bounds.x, bounds.y);
			var p2 = CellCoords(bounds.Right, bounds.Bottom);

			for (var x = p1.X; x <= p2.X; x++)
			{
				for (var y = p1.Y; y <= p2.Y; y++)
				{
					var cell = CellAtPosition(x, y);
					if (cell == null)
						continue;

					// we have a cell. loop through and fetch all the Colliders
					for (var i = 0; i < cell.Count; i++)
					{
						var collider = cell[i];
						
						if (bounds.Intersects(collider.AABB))
							tempHashset.Add(collider);
					}
				}
			}

			return tempHashset;
		}
	}
}
