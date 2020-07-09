using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
	public interface IPoolable
	{
		void OnGet<T>(GenericPool<T> pool) where T : IPoolable;

		void OnReturned<T>(GenericPool<T> pool) where T : IPoolable;
	}
}
