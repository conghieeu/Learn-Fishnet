using System;
using System.Collections.Generic;
using UnityEngine.UI.Extensions.EasingCore;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Scroller))]
	public abstract class FancyScrollRect<TItemData, TContext> : FancyScrollView<TItemData, TContext> where TContext : class, IFancyScrollRectContext, new()
	{
		[SerializeField]
		protected float reuseCellMarginCount;

		[SerializeField]
		protected float paddingHead;

		[SerializeField]
		protected float paddingTail;

		[SerializeField]
		protected float spacing;

		private Scroller cachedScroller;

		protected abstract float CellSize { get; }

		protected virtual bool Scrollable => MaxScrollPosition > 0f;

		protected Scroller Scroller => cachedScroller ?? (cachedScroller = GetComponent<Scroller>());

		private float ScrollLength => 1f / Mathf.Max(cellInterval, 0.01f) - 1f;

		private float ViewportLength => ScrollLength - reuseCellMarginCount * 2f;

		private float PaddingHeadLength => (paddingHead - spacing * 0.5f) / (CellSize + spacing);

		private float MaxScrollPosition => (float)base.ItemsSource.Count - ScrollLength + reuseCellMarginCount * 2f + (paddingHead + paddingTail - spacing) / (CellSize + spacing);

		protected override void Initialize()
		{
			base.Initialize();
			base.Context.ScrollDirection = Scroller.ScrollDirection;
			base.Context.CalculateScrollSize = delegate
			{
				float num = CellSize + spacing;
				float num2 = num * reuseCellMarginCount;
				return (ScrollSize: Scroller.ViewportSize + num + num2 * 2f, ReuseMargin: num2);
			};
			AdjustCellIntervalAndScrollOffset();
			Scroller.OnValueChanged(OnScrollerValueChanged);
		}

		private void OnScrollerValueChanged(float p)
		{
			base.UpdatePosition(Scrollable ? ToFancyScrollViewPosition(p) : 0f);
			if ((bool)Scroller.Scrollbar)
			{
				if (p > (float)(base.ItemsSource.Count - 1))
				{
					ShrinkScrollbar(p - (float)(base.ItemsSource.Count - 1));
				}
				else if (p < 0f)
				{
					ShrinkScrollbar(0f - p);
				}
			}
		}

		private void ShrinkScrollbar(float offset)
		{
			float num = 1f - ToFancyScrollViewPosition(offset) / (ViewportLength - PaddingHeadLength);
			UpdateScrollbarSize((ViewportLength - PaddingHeadLength) * num);
		}

		protected override void Refresh()
		{
			AdjustCellIntervalAndScrollOffset();
			RefreshScroller();
			base.Refresh();
		}

		protected override void Relayout()
		{
			AdjustCellIntervalAndScrollOffset();
			RefreshScroller();
			base.Relayout();
		}

		protected void RefreshScroller()
		{
			Scroller.Draggable = Scrollable;
			Scroller.ScrollSensitivity = ToScrollerPosition(ViewportLength - PaddingHeadLength);
			Scroller.Position = ToScrollerPosition(currentPosition);
			if ((bool)Scroller.Scrollbar)
			{
				Scroller.Scrollbar.gameObject.SetActive(Scrollable);
				UpdateScrollbarSize(ViewportLength);
			}
		}

		protected override void UpdateContents(IList<TItemData> items)
		{
			AdjustCellIntervalAndScrollOffset();
			base.UpdateContents(items);
			Scroller.SetTotalCount(items.Count);
			RefreshScroller();
		}

		protected new void UpdatePosition(float position)
		{
			Scroller.Position = ToScrollerPosition(position, 0.5f);
		}

		protected virtual void JumpTo(int itemIndex, float alignment = 0.5f)
		{
			Scroller.Position = ToScrollerPosition(itemIndex, alignment);
		}

		protected virtual void ScrollTo(int index, float duration, float alignment = 0.5f, Action onComplete = null)
		{
			Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, onComplete);
		}

		protected virtual void ScrollTo(int index, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
		{
			Scroller.ScrollTo(ToScrollerPosition(index, alignment), duration, easing, onComplete);
		}

		protected void UpdateScrollbarSize(float viewportLength)
		{
			float num = Mathf.Max((float)base.ItemsSource.Count + (paddingHead + paddingTail - spacing) / (CellSize + spacing), 1f);
			Scroller.Scrollbar.size = (Scrollable ? Mathf.Clamp01(viewportLength / num) : 1f);
		}

		protected float ToFancyScrollViewPosition(float position)
		{
			return position / (float)Mathf.Max(base.ItemsSource.Count - 1, 1) * MaxScrollPosition - PaddingHeadLength;
		}

		protected float ToScrollerPosition(float position)
		{
			return (position + PaddingHeadLength) / MaxScrollPosition * (float)Mathf.Max(base.ItemsSource.Count - 1, 1);
		}

		protected float ToScrollerPosition(float position, float alignment = 0.5f)
		{
			float num = alignment * (ScrollLength - (1f + reuseCellMarginCount * 2f)) + (1f - alignment - 0.5f) * spacing / (CellSize + spacing);
			return ToScrollerPosition(Mathf.Clamp(position - num, 0f, MaxScrollPosition));
		}

		protected void AdjustCellIntervalAndScrollOffset()
		{
			float num = Scroller.ViewportSize + (CellSize + spacing) * (1f + reuseCellMarginCount * 2f);
			cellInterval = (CellSize + spacing) / num;
			scrollOffset = cellInterval * (1f + reuseCellMarginCount);
		}

		protected virtual void OnValidate()
		{
			AdjustCellIntervalAndScrollOffset();
			if (loop)
			{
				loop = false;
				Debug.LogError("Loop is currently not supported in FancyScrollRect.");
			}
			if (Scroller.SnapEnabled)
			{
				Scroller.SnapEnabled = false;
				Debug.LogError("Snap is currently not supported in FancyScrollRect.");
			}
			if (Scroller.MovementType == MovementType.Unrestricted)
			{
				Scroller.MovementType = MovementType.Elastic;
				Debug.LogError("MovementType.Unrestricted is currently not supported in FancyScrollRect.");
			}
		}
	}
	public abstract class FancyScrollRect<TItemData> : FancyScrollRect<TItemData, FancyScrollRectContext>
	{
	}
}
