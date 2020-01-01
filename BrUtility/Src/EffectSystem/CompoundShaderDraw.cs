using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRavenUtility.EffectSystem
{
	public struct EffectParameter
	{
		public string name;
		public Type type;
		public object param;

		public EffectParameter(string name, Type type, object param)
		{
			this.name = name;
			this.type = type;
			this.param = param;
		}
	}

	public struct EffectInstance
	{
		public Effect effect;
		public Dictionary<string, EffectParameter> parameters;

		public EffectInstance(Effect effect, params EffectParameter[] parameters)
		{
			this.effect = effect;
			this.parameters = new Dictionary<string, EffectParameter>();

			for (int i = 0; i < parameters.Length; i++)
				this.parameters.Add(parameters[i].name, parameters[i]);
		}
	}

	public static class CompoundShaderDraw
	{
		/// <summary>
		/// Draws all effects in <see cref="effects"/> to the compound target.
		/// The initial target is drawn to the compound target, then the compound target is re-drawn with each effect applied, in order.
		/// </summary>
		/// <param name="compoundTarget">The "Compound" target. The initial target is drawn to this, is operated upon by the given effects, and is then redrawn over the initial target.
		/// The initial target is therefore the final output product.</param>
		public static void Draw(SpriteBatch batch, RenderTarget2D initialTarget, RenderTarget2D compoundTarget, Color color, BlendState blendState, List<EffectInstance> effects)
		{
			batch.GraphicsDevice.SetRenderTarget(compoundTarget);
			batch.GraphicsDevice.Clear(color);

			SetParameters(effects);

			batch.Begin(SpriteSortMode.FrontToBack, blendState, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null, null);
			batch.Draw(initialTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			batch.End();

			foreach (EffectInstance effect in effects)
			{	//loop through all the effects, and re-draw the compound target (which initially has the intial target drawn to it) to the compound target
				batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, null, effect.effect, null);

				//effect.effect.CurrentTechnique.Passes[0].Apply();
				batch.Draw(compoundTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

				batch.End();
			}/**/

			//now, redraw what's on the compound target back onto the intial target (clearing first)
			//So we can reuse the compound target for other Draw calls.
			batch.GraphicsDevice.SetRenderTarget(initialTarget);
			batch.GraphicsDevice.Clear(color);

			batch.Begin(SpriteSortMode.FrontToBack, blendState, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null, null);
			batch.Draw(compoundTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			batch.End();
		}

		public static void SetParameters(List<EffectInstance> effects)
		{
			foreach (EffectInstance effect in effects)
			{
				foreach (EffectParameter parameter in effect.parameters.Values)
				{
					if (effect.effect.Parameters.FirstOrDefault(x => x.Name == parameter.name) != null)
					{
						if (parameter.type == typeof(Texture2D))
							effect.effect.Parameters[parameter.name].SetValue((Texture2D)parameter.param);
						if (parameter.type == typeof(float))
							effect.effect.Parameters[parameter.name].SetValue((float)parameter.param);
						if (parameter.type == typeof(int))
							effect.effect.Parameters[parameter.name].SetValue((int)parameter.param);
						if (parameter.type == typeof(Vector2))
							effect.effect.Parameters[parameter.name].SetValue((Vector2)parameter.param);
						if (parameter.type == typeof(Vector3))
							effect.effect.Parameters[parameter.name].SetValue((Vector3)parameter.param);
						if (parameter.type == typeof(Vector4))
							effect.effect.Parameters[parameter.name].SetValue((Vector4)parameter.param);
						if (parameter.type == typeof(Color))
							effect.effect.Parameters[parameter.name].SetValue(((Color)parameter.param).ToVector4());
					}
					else Logger.GetOrCreate("BrUtility").Log(Logger.LogLevel.Fatal, "Effect Parameter " + parameter.name + " not found.");
				}
			}
		}
	}
}
