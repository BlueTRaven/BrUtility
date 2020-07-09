using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public class GenericPool<T> where T : IPoolable
	{
		public delegate T CreateObject();

		public readonly int startSize = 128;
		public readonly int pageSize = 128;

		private Stack<T> unused = new Stack<T>();
		private List<T> used = new List<T>();

		public IReadOnlyList<T> Used => used;
		public int Count => used.Count;

		private CreateObject createAction;

		public delegate void OnGetDelegate(T item);
		public delegate void OnReturnDelegate(T item);

		public event OnGetDelegate OnGet;
		public event OnReturnDelegate OnReturn;

		public GenericPool(CreateObject createAction, int startSize = 128, int pageSize = 128)
		{
			this.createAction = createAction;

			this.startSize = startSize;
			this.pageSize = pageSize;

			Initialize();
		}

		private void Initialize()
		{
			for (int i = 0; i < startSize; i++)
			{
				unused.Push(createAction.Invoke());
			}
		}

		private void Expand()
		{
			for (int i = 0; i < pageSize; i++)
			{
				unused.Push(createAction.Invoke());
			}
		}

		public T Get()
		{
			if (unused.Count <= 0)
				Expand();

			T item = unused.Pop();
			used.Add(item);

			item.OnGet(this);

			OnGet?.Invoke(item);
			
			return item;
		}

		public void Return(T item)
		{
			unused.Push(item);
			used.Remove(item);

			item.OnReturned(this);

			OnReturn?.Invoke(item);
		}
	}
}
