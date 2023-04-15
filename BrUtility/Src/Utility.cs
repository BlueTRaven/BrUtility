using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BrUtility
{
    public static class Utility
    {
        internal static bool setup;

		public static Random rand = new Random();

		[Obsolete("Use Camera.CreateInstance and DrawHelper.Initialize instead of this.", true)]
        public static void Setup(Camera camera, Texture2D whitePixel)
        {
            Camera.camera = camera;
            DrawHelper.WhitePixel = whitePixel;

			camera.Initialize();

            setup = true;
        }

        internal static void CheckIsSetup()
        {
            if (!setup)
            {
                throw new Exception("BrUtility has not been correctly setup! Use Utility.Setup!");
            }
        }

		public static bool HasAttribute(Type attributeType, Type objType)
		{
			var attrs = objType.GetCustomAttributes(attributeType);

			return attrs.Count() > 0;
		}

		public static Type GetType(string name)
		{
			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assemb.GetTypes())
				{
					if (type.Name == name)
						return type;
				}
			}

			return null;
		}

		public static Type GetType(string assemblyName, string name)
		{
			Assembly ass = Assembly.Load(assemblyName);
            foreach (Type type in ass.GetTypes())
            {
                if (type.Name == name || type.FullName == name)
                    return type;
            }

			return null;
        }

		public static List<Type> GetTypes<T>(bool withAbstract = false)
		{
			Type ourType = typeof(T);
			List<Type> allTypes = new List<Type>();
			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assemb.GetTypes())
				{
					if (withAbstract && type.IsAbstract)
						continue;

					if (type.IsSubclassOf(ourType))
					{
						allTypes.Add(type);
					}
				}
			}
			return allTypes;
		}

		/// <summary>
		/// Get all types that have the given attribute type.
		/// </summary>
		/// <param name="assembly">The executing assembly to search in.</param>
		/// <param name="t">The type of the attribute.</param>
		public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
		{
			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assemb.GetTypes())
				{
					if (type.GetCustomAttributes(attributeType, true).Length > 0)
					{
						yield return type;
					}
				}
			}
		}

        /// <summary>
        /// Get all types that have the given attribute type.
        /// </summary>
        /// <param name="assembly">The executing assembly to search in.</param>
        /// <param name="t">The type of the attribute.</param>
        public static IEnumerable<(Type, T)> GetTypesWithAttribute<T>() where T : Attribute
        {
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assemb.GetTypes())
                {
					IEnumerable<T> attribs = type.GetCustomAttributes<T>();
                    if (attribs.Count() > 0)
                    {
                        yield return (type, attribs.First());
                    }
                }
            }
        }

        /// <summary>
        /// Get all types that have the given attribute type.
        /// </summary>
        /// <param name="assembly">The executing assembly to search in.</param>
        /// <param name="t">The type of the attribute.</param>
        public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetCustomAttributes(attributeType, true).Length > 0)
				{
					yield return type;
				}
			}
		}

		/// <summary>
		/// Gets all types with the given attribute type, as well as the attribute object.
		/// </summary>
		public static IEnumerable<Tuple<Type, object>> GetTypesWithAttributeExtended(Type attributeType)
		{
			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assemb.GetTypes())
				{
					object[] attr = type.GetCustomAttributes(attributeType, true);
					if (attr.Length > 0)
					{
						yield return new Tuple<Type, object>(type, attr[0]);
					}
				}
			}
		}

		/// <summary>
		/// Gets all types with the given attribute type, as well as the attribute object.
		/// </summary>
		public static IEnumerable<Tuple<Type, T>> GetTypesWithAttributeExtended<T>() where T : Attribute
		{
			foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assemb.GetTypes())
				{
					List<T> attr = type.GetCustomAttributes<T>(true).ToList();
					if (attr.Count > 0)
					{
						yield return new Tuple<Type, T>(type, attr[0]);
					}
				}
			}
		}

		/// <summary>
		/// Gets all types with the given attribute type, as well as the attribute object.
		/// </summary>
		public static IEnumerable<Tuple<Type, T>> GetTypesWithAttributeExtended<T>(Assembly assembly) where T : Attribute
		{
			foreach (Type type in assembly.GetTypes())
			{
				List<T> attr = type.GetCustomAttributes<T>(true).ToList();
				if (attr.Count > 0)
				{
					yield return new Tuple<Type, T>(type, attr[0]);
				}
			}
		}

		public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }

		public static bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
		{
			intersection = Vector2.Zero;

			Vector2 b = a2 - a1;
			Vector2 d = b2 - b1;
			float bDotDPerp = b.X * d.Y - b.Y * d.X;

			// if b dot d == 0, it means the lines are parallel so have infinite intersection points
			if (bDotDPerp == 0)
				return false;

			Vector2 c = b1 - a1;
			float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
			if (t < 0 || t > 1)
				return false;

			float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
			if (u < 0 || u > 1)
				return false;

			intersection = a1 + t * b;

			return true;
		}

		public static bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			Vector2 b = a2 - a1;
			Vector2 d = b2 - b1;
			float bDotDPerp = b.X * d.Y - b.Y * d.X;

			// if b dot d == 0, it means the lines are parallel so have infinite intersection points
			if (bDotDPerp == 0)
				return false;

			Vector2 c = b1 - a1;
			float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
			if (t < 0 || t > 1)
				return false;

			float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
			if (u < 0 || u > 1)
				return false;

			return true;
		}

		public static bool LineIntersectsRect(Vector2 start, Vector2 end, Rectangle rect)
		{
			if (LineIntersectsLine(start, end, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Right, rect.Bottom), new Vector2(rect.Left, rect.Bottom)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Left, rect.Top)))
				return true;

			return false;
		}

		public static bool LineIntersectsRect(Vector2 start, Vector2 end, RectangleF rect)
		{
			if (LineIntersectsLine(start, end, new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Top)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Right, rect.Top), new Vector2(rect.Right, rect.Bottom)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Right, rect.Bottom), new Vector2(rect.Left, rect.Bottom)))
				return true;

			if (LineIntersectsLine(start, end, new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Left, rect.Top)))
				return true;

			return false;
		}

		public static T MultiLerp<T>(float t, Func<T, T, float, T> lerpFunc, params T[] values)
		{
			int c = values.Length - 1;  // number of transitions
			t = MathHelper.Clamp(t, 0, 1) * c;   // expand t from 0-1 to 0-c
			int index = (int)MathHelper.Clamp((float)Math.Floor(t), 0, c - 1); // get current index and clamp
			t -= index; // subract the index to get back a value of 0-1

			return lerpFunc.Invoke(values[index], values[index + 1], t);
		}

		/*public static object GetSpriteBatchItem()
		{
			Type.GetType("SpriteBatchItem");
		}*/
	}
}
