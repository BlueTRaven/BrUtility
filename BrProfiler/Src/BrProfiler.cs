using BrUtility.Src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrProfiler
{
	public class BrProfiler
	{
		public ref struct ProfileResult
		{
			public Dictionary<string, ProfileSnippet> snippetsByName;
			public FastList<ProfileSnippet> iterableSnippets;
		}

		public struct ProfileSnippet
		{
			internal readonly string name;
			private Stopwatch watch;

			internal ProfileSnippet(string name)
			{
				this.name = name;
				watch = null;
			}

			internal void Start()
			{
				watch = Stopwatch.StartNew();
			}

			internal void Stop()
			{
				watch.Stop();
			}

			public TimeSpan GetTime()
			{
				if (!watch.IsRunning)
					return watch.Elapsed;
				else return default;
			}
		}

		//fast list supports ref indexing
		private FastList<ProfileSnippet> snippets = new FastList<ProfileSnippet>();

		public void StartProfiling()
		{
			snippets.Clear();
		}

		public ProfileResult EndProfiling()
		{
			ProfileResult result = new ProfileResult();
			result.iterableSnippets = snippets;
			result.snippetsByName = new Dictionary<string, ProfileSnippet>();

			for (int i = 0; i < snippets.Length; i++)
			{
				result.snippetsByName.Add(snippets[i].name, snippets[i]);
			}
			
			return result;
		}

		public void StartSnippet(string name)
		{
			ProfileSnippet snippet = new ProfileSnippet(name);
			snippets.Add(snippet);
			snippet.Start();
		}
    }
}
