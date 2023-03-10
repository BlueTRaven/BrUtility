using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public class GenericPool<T> : IPool<T> where T : IPoolable
	{
		public delegate T CreateObject();

		public readonly int startSize = 128;
		public readonly int pageSize = 128;
		public readonly int pageRate = 2;

		public int CurrentPageSize { get; private set; }

		private const int MAX_PAGE = ushort.MaxValue;

		private T[] elements;
		private int lastActive;	//the last element pulled or returned.

		//private Stack<T> unused = new Stack<T>();
		//private List<T> used = new List<T>();

		//public IReadOnlyList<T> Used => used;
		public int Count;

		private CreateObject createAction;

		public delegate void OnGetDelegate(T item);
		public delegate void OnReturnDelegate(T item);

		public event OnGetDelegate OnGet;
		public event OnReturnDelegate OnReturn;

		public GenericPool(CreateObject createAction, int startSize = 128, int pageSize = 128, int pageRate = 2)
		{
			elements = new T[startSize];

			this.createAction = createAction;

			this.startSize = startSize;
			this.pageSize = pageSize;
			this.pageRate = pageRate;

			CurrentPageSize = pageSize;

			Initialize();
		}

		private void Initialize()
		{
			for (int i = 0; i < startSize; i++)
			{
				elements[i] = createAction.Invoke();
				elements[i].PoolIndex = i;
			}

			/*for (int i = 0; i < startSize; i++)
			{
				unused.Push(createAction.Invoke());
			}*/
		}

		private void Expand()
		{
			var oldElements = elements;

			elements = new T[elements.Length + CurrentPageSize];

			for (int i = 0; i < elements.Length; i++)
			{
				//Copy old elements and create new ones on the end.
				if (i < oldElements.Length)
					elements[i] = oldElements[i];
				else elements[i] = createAction.Invoke();

				elements[i].PoolIndex = i;
			}

			/*for (int i = 0; i < CurrentPageSize; i++)
			{
				unused.Push(createAction.Invoke());
			}*/

			CurrentPageSize *= pageRate;

			CurrentPageSize = MathHelper.Clamp(CurrentPageSize, 0, MAX_PAGE);
		}

		public T Get()
		{
			int numIter = 0;
			while (numIter < elements.Length)
			{
				if (!elements[lastActive].IsUsed)
				{
					elements[lastActive].OnGet(this);
					OnGet?.Invoke(elements[lastActive]);

					Count++;
					return elements[lastActive];
				}

				numIter++;
				lastActive = (lastActive + 1) % elements.Length;
			}

			//if the above while loop fails, we'll fall back to here.
			//The only case that the above while loop fails is when it iterates over all elements and finds none of them to be unused.
			//If this is the case, we need to expand to create new unused elements.
			//We can also safely assume that the first element that's created is unused, so we can set lastActive to elements.Length,
			//which will be incremented by 1 in get, which will return the first element of the expanded section.
			lastActive = elements.Length;
			Expand();
			Count++;
			return Get();

			//return null; 
			/*if (unused.Count <= 0)
				Expand();

			T item = unused.Pop();
			used.Add(item);

			item.OnGet(this);

			OnGet?.Invoke(item);
			
			return item;*/
		}

		public void Return(T item)
		{
			Count--;
			lastActive = item.PoolIndex;

			item.OnReturned(this);
			OnReturn?.Invoke(item);
			return;

			//unused.Push(item);
			//used.Remove(item);

			//item.OnReturned(this);

			//OnReturn?.Invoke(item);
		}
	}
}
