using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI.Extensions.EasingCore;

namespace UnityEngine.UI.Extensions
{
	public abstract class FancyGridView<TItemData, TContext> : FancyScrollRect<TItemData[], TContext> where TContext : class, IFancyGridViewContext, new()
	{
		protected abstract class DefaultCellGroup : FancyCellGroup<TItemData, TContext>
		{
		}

		[SerializeField]
		protected float startAxisSpacing;

		[SerializeField]
		protected int startAxisCellCount = 4;

		[SerializeField]
		protected Vector2 cellSize = new Vector2(100f, 100f);

		private GameObject cellGroupTemplate;

		protected sealed override GameObject CellPrefab => cellGroupTemplate;

		protected override float CellSize
		{
			get
			{
				if (base.Scroller.ScrollDirection != ScrollDirection.Horizontal)
				{
					return cellSize.y;
				}
				return cellSize.x;
			}
		}

		public int DataCount { get; private set; }

		protected override void Initialize()
		{
			base.Initialize();
			base.Context.ScrollDirection = base.Scroller.ScrollDirection;
			base.Context.GetGroupCount = () => startAxisCellCount;
			base.Context.GetStartAxisSpacing = () => startAxisSpacing;
			base.Context.GetCellSize = () => (base.Scroller.ScrollDirection != ScrollDirection.Horizontal) ? cellSize.x : cellSize.y;
			SetupCellTemplate();
		}

		protected abstract void SetupCellTemplate();

		protected virtual void Setup<TGroup>(FancyCell<TItemData, TContext> cellTemplate) where TGroup : FancyCell<TItemData[], TContext>
		{
			base.Context.CellTemplate = cellTemplate.gameObject;
			cellGroupTemplate = new GameObject("Group").AddComponent<TGroup>().gameObject;
			cellGroupTemplate.transform.SetParent(cellContainer, worldPositionStays: false);
			cellGroupTemplate.SetActive(value: false);
		}

		public virtual void UpdateContents(IList<TItemData> items)
		{
			DataCount = items.Count;
			TItemData[][] itemsSource = (from x in items.Select((TItemData item, int index) => (item: item, index: index))
				group x.item by x.index / startAxisCellCount into @group
				select @group.ToArray()).ToArray();
			UpdateContents(itemsSource);
		}

		protected override void JumpTo(int itemIndex, float alignment = 0.5f)
		{
			int itemIndex2 = itemIndex / startAxisCellCount;
			base.JumpTo(itemIndex2, alignment);
		}

		protected override void ScrollTo(int itemIndex, float duration, float alignment = 0.5f, Action onComplete = null)
		{
			int index = itemIndex / startAxisCellCount;
			base.ScrollTo(index, duration, alignment, onComplete);
		}

		protected override void ScrollTo(int itemIndex, float duration, Ease easing, float alignment = 0.5f, Action onComplete = null)
		{
			int index = itemIndex / startAxisCellCount;
			base.ScrollTo(index, duration, easing, alignment, onComplete);
		}
	}
	public abstract class FancyGridView<TItemData> : FancyGridView<TItemData, FancyGridViewContext>
	{
	}
}
