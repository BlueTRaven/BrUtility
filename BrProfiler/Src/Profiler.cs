using BlueRavenUtility;
using BrUtility.Src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrProfiler
{
	public static class BrProfiler
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
		private static FastList<ProfileSnippet> snippets = new FastList<ProfileSnippet>();

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
			result.iterableSnippets = snippets;
			result.snippetsByName = new Dictionary<string, ProfileSnippet>();

			for (int i = 0; i < snippets.Length; i++)
			{
				result.snippetsByName.Add(snippets[i].name, snippets[i]);
			}

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

			ProfileSnippet snippet = new ProfileSnippet(name);
			snippets.Add(snippet);
			snippet.Start();
		}

		public static void EndSnippet(string name)
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before EndSnippet can be called.");
				return;
			}

			bool foundAny = false;

			for (int i = 0; i < snippets.Length; i++)
			{
				if (snippets[i].name == name)
				{
					snippets[i].Stop();
					foundAny = true;
					break;
				}
			}

			if (!foundAny)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Tried to end snippet with name " + name + ", but no snippet with that name exists. Did you typo or forget a StartSnippet?");
			}
		}

	}
}
