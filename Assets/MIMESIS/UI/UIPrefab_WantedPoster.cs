using Bifrost.Cooked;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_WantedPoster : UIPrefabScript
{
	public const string UEID_ImageInteract = "ImageInteract";

	public const string UEID_Image_BG = "Image_BG";

	public const string UEID_Image_Item = "Image_Item";

	public const string UEID_Image_Texture = "Image_Texture";

	private Transform _UE_ImageInteract;

	private Image _UE_Image_BG;

	private Image _UE_Image_Item;

	private Image _UE_Image_Texture;

	public Transform UE_ImageInteract => _UE_ImageInteract ?? (_UE_ImageInteract = PickTransform("ImageInteract"));

	public Image UE_Image_BG => _UE_Image_BG ?? (_UE_Image_BG = PickImage("Image_BG"));

	public Image UE_Image_Item => _UE_Image_Item ?? (_UE_Image_Item = PickImage("Image_Item"));

	public Image UE_Image_Texture => _UE_Image_Texture ?? (_UE_Image_Texture = PickImage("Image_Texture"));

	public void SetWantedWallPaper(ItemMasterInfo? itemMasterInfo)
	{
		if (itemMasterInfo == null)
		{
			UE_ImageInteract.gameObject.SetActive(value: false);
			return;
		}
		MMLootingObjectTable.SkinnedItemInfo item = Hub.s.tableman.lootingObject.FindSkinnedItemInfo(itemMasterInfo.MasterID).skinnedItemInfo;
		if (item == null)
		{
			Logger.RError($"skinnedItemInfo is null @ SetWantedWallPaper: ItemMasterID={itemMasterInfo.MasterID}");
			return;
		}
		string posterIconSpriteId = item.posterIconSpriteId;
		if (!string.IsNullOrEmpty(posterIconSpriteId))
		{
			UE_Image_Item.sprite = Hub.s.tableman.posterIconSprite.GetSprite(posterIconSpriteId);
			UE_ImageInteract.gameObject.SetActive(UE_Image_Item.sprite != null);
		}
	}
}
