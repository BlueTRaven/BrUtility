using BlueRavenUtility;
using BrUtility;
using BrUtility.Src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrProfiler
{
	public static class Profiler
	{
		public ref struct ProfileResult
		{
			public Dictionary<string, TimeSpan> timespans;
		}

		public class ProfileSnippet : IPoolable
		{
			internal string name;
			private Stopwatch watch;

			internal bool started;

			internal void Start(string name)
			{
				this.name = name;
				watch = Stopwatch.StartNew();
				started = true;
			}

			internal void Stop()
			{
				watch.Stop();
				started = false;
			}

			public TimeSpan GetTime()
			{
				if (!watch.IsRunning)
					return watch.Elapsed;
				else return default;
			}

			public void OnGet<T>(GenericPool<T> pool) where T : IPoolable
			{
				watch = null;
				name = null;
			}

			public void OnReturned<T>(GenericPool<T> pool) where T : IPoolable
			{
			}
		}

		private static GenericPool<ProfileSnippet> pooledSnippets = new GenericPool<ProfileSnippet>(() => { return new ProfileSnippet(); });

		//fast list supports ref indexing
		private static FastList<ProfileSnippet> snippets = new FastList<ProfileSnippet>();
		private static Dictionary<string, ProfileSnippet> snippetsByName = new Dictionary<string, ProfileSnippet>();

		private static bool started;

		public static void StartProfiling()
		{
			if (started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Cannot start profiling as profiling was already started.");
				return;
			}

			snippets.Clear();
			started = true;
		}

		public static ProfileResult EndProfiling()
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before EndProfiling can be called.");
				return default;
			}

			ProfileResult result = new ProfileResult();
			result.timespans = new Dictionary<string, TimeSpan>();

			for (int i = 0; i < snippets.Length; i++)
			{
				result.timespans.Add(snippets[i].name, snippets[i].GetTime());
				pooledSnippets.Return(snippets[i]);
			}

			snippets.Clear();
			snippetsByName.Clear();

			started = false;
			return result;
		}

		public static void StartSnippet(string name)
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before StartSnippet can be called.");
				return;
			}

			ProfileSnippet snippet = pooledSnippets.Get();
			snippets.Add(snippet);
			snippetsByName.Add(name, snippet);
			snippet.Start(name);
		}

		public static void EndSnippet(string name)
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before EndSnippet can be called.");
				return;
			}

			if (snippetsByName.ContainsKey(name))
			{
				if (!snippetsByName[name].started)
				{
					Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Tried to end snippet with name " + name + ", but this snippet has not been started yet.");
				}
				snippetsByName[name].Stop();
			}
			else
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Tried to end snippet with name " + name + ", but no snippet with that name exists. Did you typo or forget a StartSnippet?");
			}
		}

	}
}
