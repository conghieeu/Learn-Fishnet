namespace UnityEngine.UI.Extensions
{
	public abstract class FancyScrollRectCell<TItemData, TContext> : FancyCell<TItemData, TContext> where TContext : class, IFancyScrollRectContext, new()
	{
		public override void UpdatePosition(float position)
		{
			(float, float) tuple = base.Context.CalculateScrollSize();
			float item = tuple.Item1;
			float item2 = tuple.Item2;
			float normalizedPosition = (Mathf.Lerp(0f, item, position) - item2) / (item - item2 * 2f);
			float num = 0.5f * item;
			float b = 0f - num;
			UpdatePosition(normalizedPosition, Mathf.Lerp(num, b, position));
		}

		protected virtual void UpdatePosition(float normalizedPosition, float localPosition)
		{
			base.transform.localPosition = ((base.Context.ScrollDirection == ScrollDirection.Horizontal) ? new Vector2(0f - localPosition, 0f) : new Vector2(0f, localPosition));
		}
	}
	public abstract class FancyScrollRectCell<TItemData> : FancyScrollRectCell<TItemData, FancyScrollRectContext>
	{
		public sealed override void SetContext(FancyScrollRectContext context)
		{
			base.SetContext(context);
		}
	}
}
