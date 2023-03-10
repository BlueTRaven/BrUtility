using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public interface IPool<T> where T : IPoolable
    {
		T Get();

		void Return(T item);
	}

	public interface IPoolable
	{
		bool IsUsed { get; set; }
		int PoolIndex { get; set; }

		void OnGet<T>(IPool<T> pool) where T : IPoolable;

		void OnReturned<T>(IPool<T> pool) where T : IPoolable;
	}
}
