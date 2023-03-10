using BrUtility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//THIS IS NOT MY CODE!
//Taken and modified from a static version elsewhere.
//I believe the original code came from dcrew.

namespace BrUtility.DataStructures
{
	public class QuadTree<T> where T : IAABB
	{
		private GenericPool<Node> pool = new GenericPool<Node>(() => new Node());

		public Rectangle Bounds
		{
			get => mainNode.Bounds;
			set
			{
				if (mainNode != null)
				{
					pool.Return(mainNode);
					mainNode = pool.Get();

					var items = stored.Keys.ToArray();
					
					mainNode.Bounds = value;
					foreach (var i in items)
						stored[i] = mainNode.Add(this, pool, i);
				}
				else
				{
					mainNode = pool.Get();
					mainNode.parent = null;
					mainNode.Bounds = value;
				}
			}
		}

		public readonly IDictionary<T, Node> stored = new Dictionary<T, Node>();

		private Node mainNode;

		private struct AABBSize
		{
			public T item;
			public Point halfSize;
			public Size size;

			public AABBSize(T item)
			{
				this.item = item;
				this.halfSize = new Point((int)Math.Ceiling(item.AABB.Width / 2d), (int)Math.Ceiling(item.AABB.Height / 2d));
				this.size = new Size(item.AABB.Width, item.AABB.Height);
			}
		}

		private AABBSize maxSizeAABB;

		public QuadTree(Rectangle bounds)
		{
			this.Bounds = bounds;
		}

		public void Add(T item)
		{
			stored.Add(item, mainNode.Add(this, pool, item));
			if (item.AABB.Width > maxSizeAABB.size.Width || item.AABB.Height > maxSizeAABB.size.Height)
				maxSizeAABB = new AABBSize(item);
		}

		public void Remove(T item)
		{
			stored[item].Remove(this, pool, item);
			stored.Remove(item);
		}

		public void Clear()
		{
			pool.Return(mainNode);
			mainNode = pool.Get();
			stored.Clear();
			maxSizeAABB = new AABBSize(default);
		}

		public void Update(T item)
		{
			stored[item].Remove(this, pool, item);
			stored[item] = mainNode.Add(this, pool, item);
		}

		public IEnumerable<T> Query(Rectangle area)
		{
			foreach (var i in mainNode.Query(new Rectangle(area.X - maxSizeAABB.halfSize.X, area.Y - maxSizeAABB.halfSize.Y, 
				(int)maxSizeAABB.size.Width + area.Width, (int)maxSizeAABB.size.Height + area.Height), area))
				yield return i;
		}

		public IEnumerable<T> Query(Vector2 pos)
		{
			foreach (var i in mainNode.Query(new Rectangle((int)Math.Round(pos.X - maxSizeAABB.halfSize.X), (int)Math.Round(pos.Y - maxSizeAABB.halfSize.Y), 
				(int)maxSizeAABB.size.Width + 1, (int)maxSizeAABB.size.Height + 1), new Rectangle((int)Math.Round(pos.X), (int)Math.Round(pos.Y), 1, 1)))
				yield return i;
		}

		public class Node : IPoolable
		{
			const int CAPACITY = 8;

			public bool IsUsed { get; set; }
			public int PoolIndex { get; set; }

			public Rectangle Bounds { get; internal set; }

			public int Count
			{
				get
				{
					int c = items.Count;
					if (nw != null)
					{
						c += ne.Count;
						c += se.Count;
						c += sw.Count;
						c += nw.Count;
					}
					return c;
				}
			}

			public IEnumerable<T> AllItems
			{
				get
				{
					foreach (var i in items)
						yield return i;
					if (nw != null)
					{
						foreach (var i in ne.AllItems)
							yield return i;
						foreach (var i in se.AllItems)
							yield return i;
						foreach (var i in sw.AllItems)
							yield return i;
						foreach (var i in nw.AllItems)
							yield return i;
					}
				}
			}
			public IEnumerable<T> AllSubItems
			{
				get
				{
					foreach (var i in ne.AllItems)
						yield return i;
					foreach (var i in se.AllItems)
						yield return i;
					foreach (var i in sw.AllItems)
						yield return i;
					foreach (var i in nw.AllItems)
						yield return i;
				}
			}

			private readonly HashSet<T> items = new HashSet<T>();

			internal Node parent, ne, se, sw, nw;

			private bool HasChildren { get { return ne != null; } }

			private static Node Bury(QuadTree<T> quadTree, GenericPool<Node> pool, T i, Node n)
			{
				if (n.ne.Bounds.Contains(i.AABB.Center))
					return n.ne.Add(quadTree, pool, i);
				if (n.se.Bounds.Contains(i.AABB.Center))
					return n.se.Add(quadTree, pool, i);
				if (n.sw.Bounds.Contains(i.AABB.Center))
					return n.sw.Add(quadTree, pool, i);
				if (n.nw.Bounds.Contains(i.AABB.Center))
					return n.nw.Add(quadTree, pool, i);
				return n;
			}

			public Node Add(QuadTree<T> quadTree, GenericPool<Node> pool, T item)
			{
				if (HasChildren)
				{
					if (items.Count >= CAPACITY && Bounds.Width * Bounds.Height > 1024)
					{
						int halfWidth = Bounds.Width / 2,
							halfHeight = Bounds.Height / 2;
						nw = pool.Get();
						nw.Bounds = new Rectangle(Bounds.Left, Bounds.Top, halfWidth, halfHeight);
						nw.parent = this;
						sw = pool.Get();
						int midY = Bounds.Top + halfHeight,
							height = Bounds.Bottom - midY;
						sw.Bounds = new Rectangle(Bounds.Left, midY, halfWidth, height);
						sw.parent = this;
						ne = pool.Get();
						int midX = Bounds.Left + halfWidth,
							width = Bounds.Right - midX;
						ne.Bounds = new Rectangle(midX, Bounds.Top, width, halfHeight);
						ne.parent = this;
						se = pool.Get();
						se.Bounds = new Rectangle(midX, midY, width, height);
						se.parent = this;
						foreach (var i in items)
							quadTree.stored[i] = Bury(quadTree, pool, i, this);
						items.Clear();

						return Bury(quadTree, pool, item, this);
					}
				}

				items.Add(item);
				return this;
			}

			public void Remove(QuadTree<T> quadTree, GenericPool<Node> pool, T item)
			{
				items.Remove(item);
				if (parent?.nw != null && parent.Count < CAPACITY)
				{
					foreach (var i in parent.AllSubItems)
					{
						parent.items.Add(i);
						quadTree.stored[i] = parent;
					}

					pool.Return(parent.ne);
					pool.Return(parent.se);
					pool.Return(parent.sw);
					pool.Return(parent.nw);
					parent.ne = null;
					parent.se = null;
					parent.sw = null;
					parent.nw = null;
				}
			}

			public IEnumerable<T> Query(Rectangle broad, Rectangle query)
			{
				if (nw == null)
				{
					foreach (T i in items)
						if (query.Intersects(i.AABB))
							yield return i;
					yield break;
				}
				if (ne.Bounds.Contains(broad))
				{
					foreach (var i in ne.Query(broad, query))
						yield return i;
					yield break;
				}
				if (broad.Contains(ne.Bounds))
					foreach (var i in ne.AllItems)
						yield return i;
				else if (ne.Bounds.Intersects(broad))
					foreach (var i in ne.Query(broad, query))
						yield return i;
				if (se.Bounds.Contains(broad))
				{
					foreach (var i in se.Query(broad, query))
						yield return i;
					yield break;
				}
				if (broad.Contains(se.Bounds))
					foreach (var i in se.AllItems)
						yield return i;
				else if (se.Bounds.Intersects(broad))
					foreach (var i in se.Query(broad, query))
						yield return i;
				if (sw.Bounds.Contains(broad))
				{
					foreach (var i in sw.Query(broad, query))
						yield return i;
					yield break;
				}
				if (broad.Contains(sw.Bounds))
					foreach (var i in sw.AllItems)
						yield return i;
				else if (sw.Bounds.Intersects(broad))
					foreach (var i in sw.Query(broad, query))
						yield return i;
				if (nw.Bounds.Contains(broad))
				{
					foreach (var i in nw.Query(broad, query))
						yield return i;
					yield break;
				}
				if (broad.Contains(nw.Bounds))
					foreach (var i in nw.AllItems)
						yield return i;
				else if (nw.Bounds.Intersects(broad))
					foreach (var i in nw.Query(broad, query))
						yield return i;
			}

			void IPoolable.OnGet<T1>(IPool<T1> pool)
			{
				IsUsed = true;
			}

			void IPoolable.OnReturned<T1>(IPool<T1> pool)
			{
				IsUsed = false;
				items.Clear();

				var castPool = pool as GenericPool<Node>;
				castPool.Return(ne);
				castPool.Return(se);
				castPool.Return(sw);
				castPool.Return(nw);
				
				ne = null;
				se = null;
				sw = null;
				nw = null;
			}
		}
	}
}
