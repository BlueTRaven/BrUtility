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
		public struct ProfileResult
		{
			public readonly string name;
			public readonly FastList<ProfileSnippetResult> snippetTree;

			public ProfileResult(string name, FastList<ProfileSnippetResult> snippetTree)
			{
				this.name = name;
				this.snippetTree = snippetTree;
			}
		}

		public class ProfileSnippetResult
		{
			public readonly string name;
			public TimeSpan elapsedTime;

			public ProfileSnippetResult parent;
			public FastList<ProfileSnippetResult> children;

			public ProfileSnippetResult(string name, ProfileSnippetResult parent, TimeSpan elapsedTime)
			{
				this.name = name;
				this.parent = parent;
				this.children = null;
				this.elapsedTime = elapsedTime;
			}

			public void AddChild(ProfileSnippetResult child)
			{
				if (children == null)
					children = new FastList<ProfileSnippetResult>();

				children.Add(child);
			}
		}

		public class ProfileSnippet : IPoolable
		{
			internal string name;
			private Stopwatch watch;

			internal bool started;

			internal string parent;

			internal void Start(string name, string parent = null)
			{
				this.name = name;
				this.parent = parent;
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
		private static Stack<string> startedSnippets = new Stack<string>();

		private static string name;
		private static bool started;

		public static void StartProfiling(string name)
		{
			Logger.GetOrCreate("Profiler");

			if (started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Tried to start profiling, but profiling was already started as " + name + ".");
				return;
			}

			snippets.Clear();
			started = true;
			Profiler.name = name;
		}

		public static ProfileResult EndProfiling()
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before EndProfiling can be called.");
				return default;
			}

			ProfileResult result = new ProfileResult(name, BuildTree());

			snippets.Clear();
			snippetsByName.Clear();
			startedSnippets.Clear();

			started = false;
			return result;
		}

		private static FastList<ProfileSnippetResult> BuildTree()
		{
			FastList<ProfileSnippetResult> rootNodes = new FastList<ProfileSnippetResult>();

			//Start from root nodes (nodes with no parents.)
			//Loop through all nodes. if those nodes have the same parent name as the current node, then add them to the list of children of that node.
			
			for (int i = 0; i < snippets.Length; i++)
			{
				if (snippets[i].parent == null)
				{
					ProfileSnippetResult result = new ProfileSnippetResult(snippets[i].name, null, snippets[i].GetTime());
					rootNodes.Add(result);

					SearchForChildren(result);
				}
			}

			return rootNodes;
		}

		private static void SearchForChildren(ProfileSnippetResult parentSearch)
		{
			for (int i = 0; i < snippets.Length; i++)
			{
				if (snippets[i].parent == parentSearch.name)
				{
					ProfileSnippetResult result = new ProfileSnippetResult(snippets[i].name, parentSearch, snippets[i].GetTime());
					parentSearch.AddChild(result);

					SearchForChildren(result);
				}
			}
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
			
			snippet.Start(name, startedSnippets.Count > 0 ? startedSnippets.Peek() : null);
			startedSnippets.Push(name);
		}

		public static void EndSnippet()
		{
			if (!started)
			{
				Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "StartProfiling must be called before EndSnippet can be called.");
				return;
			}

			string name = startedSnippets.Pop();
			
			if (snippetsByName.ContainsKey(name))
			{
				if (!snippetsByName[name].started)
				{
					Logger.GetLogger("Profiler").Log(Logger.LogLevel.Error, "Tried to end snippet with name " + name + ", but this snippet has not been started yet.");
					return;
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
