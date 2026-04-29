namespace UnityEngine.UI.Extensions
{
	public abstract class FancyGridViewCell<TItemData, TContext> : FancyScrollRectCell<TItemData, TContext> where TContext : class, IFancyGridViewContext, new()
	{
		protected override void UpdatePosition(float normalizedPosition, float localPosition)
		{
			float num = base.Context.GetCellSize();
			float num2 = base.Context.GetStartAxisSpacing();
			int num3 = base.Context.GetGroupCount();
			int num4 = base.Index % num3;
			float num5 = (num + num2) * ((float)num4 - (float)(num3 - 1) * 0.5f);
			base.transform.localPosition = ((base.Context.ScrollDirection == ScrollDirection.Horizontal) ? new Vector2(0f - localPosition, 0f - num5) : new Vector2(num5, localPosition));
		}
	}
	public abstract class FancyGridViewCell<TItemData> : FancyGridViewCell<TItemData, FancyGridViewContext>
	{
		public sealed override void SetContext(FancyGridViewContext context)
		{
			base.SetContext(context);
		}
	}
}
