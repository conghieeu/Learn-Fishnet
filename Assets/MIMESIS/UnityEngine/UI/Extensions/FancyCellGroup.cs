using System.Linq;

namespace UnityEngine.UI.Extensions
{
	public abstract class FancyCellGroup<TItemData, TContext> : FancyCell<TItemData[], TContext> where TContext : class, IFancyCellGroupContext, new()
	{
		protected virtual FancyCell<TItemData, TContext>[] Cells { get; private set; }

		protected virtual FancyCell<TItemData, TContext>[] InstantiateCells()
		{
			return (from _ in Enumerable.Range(0, base.Context.GetGroupCount())
				select Object.Instantiate(base.Context.CellTemplate, base.transform) into x
				select x.GetComponent<FancyCell<TItemData, TContext>>()).ToArray();
		}

		public override void Initialize()
		{
			Cells = InstantiateCells();
			for (int i = 0; i < Cells.Length; i++)
			{
				Cells[i].SetContext(base.Context);
				Cells[i].Initialize();
			}
		}

		public override void UpdateContent(TItemData[] contents)
		{
			int num = base.Index * base.Context.GetGroupCount();
			for (int i = 0; i < Cells.Length; i++)
			{
				Cells[i].Index = i + num;
				Cells[i].SetVisible(i < contents.Length);
				if (Cells[i].IsVisible)
				{
					Cells[i].UpdateContent(contents[i]);
				}
			}
		}

		public override void UpdatePosition(float position)
		{
			for (int i = 0; i < Cells.Length; i++)
			{
				Cells[i].UpdatePosition(position);
			}
		}
	}
}
