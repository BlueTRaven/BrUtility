using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrIMGUI
{
	public struct IMGUIId
	{
		public readonly int id;
		public readonly int parent;
		public bool enabled;
		public bool visible;

		internal bool skipParent;

		internal bool valid;

		public static IMGUIId Inactive
		{
			get
			{
				return new IMGUIId(-1, -1) { valid = false, enabled = false, visible = false };
			}
		}

		internal IMGUIId(int id, int parent)
		{
			this.id = id;

			this.parent = parent;

			this.valid = true;
			this.skipParent = false;

			this.enabled = true;
			this.visible = true;

		}

		public static bool operator ==(IMGUIId t, IMGUIId other)
		{
			return t.Equals(other);
		}

		public static bool operator !=(IMGUIId t, IMGUIId other)
		{
			return !t.Equals(other);
		}

		public override string ToString()
		{
			return "ID: " + id + ", Parent: " + parent + ", Enabled: " + enabled + ", Visible: " + visible;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is IMGUIId))
			{
				return false;
			}

			var other = (IMGUIId)obj;
			return GetHashCode() == other.GetHashCode();
		}

		public override int GetHashCode()
		{
			var hashCode = -1576389609;
			hashCode = hashCode * -1521134295 + id.GetHashCode();
			hashCode = hashCode * -1521134295 + parent.GetHashCode();
			hashCode = hashCode * -1521134295 + enabled.GetHashCode();
			hashCode = hashCode * -1521134295 + visible.GetHashCode();
			hashCode = hashCode * -1521134295 + skipParent.GetHashCode();
			hashCode = hashCode * -1521134295 + valid.GetHashCode();
			return hashCode;
		}
	}
}
