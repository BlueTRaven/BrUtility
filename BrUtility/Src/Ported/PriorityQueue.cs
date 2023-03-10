using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility.Ported
{
	public class PriorityQueue<T>
	{	//super simple implementation of a priority queue. not at *all* optimized.
		private bool firstOut;
		private Func<T, int> determinePriorityFunc;

		private List<T> objs = new List<T>();

		public int Count { get { return objs.Count; } }

		public PriorityQueue(bool firstOut, Func<T, int> determinePriorityFunc)
		{
			this.firstOut = firstOut;
			this.determinePriorityFunc = determinePriorityFunc;
		}

		public void EnqueueWithoutSorting(T obj)
		{
			objs.Add(obj);
		}

		public void Enqueue(T obj)
		{
			objs.Add(obj);

			Sort();
		}

		public void Sort()
		{
			if (firstOut)
				objs = objs.OrderBy(determinePriorityFunc).ToList();
			else objs = objs.OrderByDescending(determinePriorityFunc).ToList();
		}

		public T Dequeue()
		{
			if (objs.Count <= 0)
				throw new IndexOutOfRangeException();

			T obj = default(T);
			
			obj = objs[0];
			objs.RemoveAt(0);

			return obj;
		}

		public void Clear()
		{
			objs.Clear();
		}
	}
}
