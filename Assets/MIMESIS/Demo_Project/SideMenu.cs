using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demo_Project
{
	public class SideMenu : MonoBehaviour
	{
		public bool isMenuOpen;

		private const float openPositon = -263f;

		private const float closePosition = -726f;

		public float menuMoveSpeed = 5f;

		private string menuState = "";

		private int currentSelectedItem;

		private int totalNumberOfItems;

		public string selectedItem = "";

		public GameObject selectedItemObject;

		private float cameraOriginalSize = 2.78f;

		private float cameraOriginalPosition;

		private float cameraShrinkSize = 5f;

		private float cameraShrinkPosition = -2.5f;

		private GameObject itemNameObject;

		private bool moveText;

		public float textPopupMoveSpeed = 0.3f;

		public bool burstDelay;

		private float timeSinceLastFrame;

		private void Start()
		{
			SceneManager.listOfMenuObjects.Add(base.gameObject);
			itemNameObject = base.transform.parent.Find("Item_Name").gameObject;
			SelectBurst();
			AdjustSelectionArrow();
			UpdateItemRatio();
			UpdateItemName();
			InitialOpenMenu();
		}

		private void InitialOpenMenu()
		{
			base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition = new Vector3(-263f, 241f, 0f);
			base.transform.Find("Main Menu Image").transform.Find("Arrow").GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize = cameraShrinkSize;
			GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.position = new Vector3(cameraShrinkPosition, 0f, -10f);
			isMenuOpen = true;
		}

		public void MenuClickCheck()
		{
			if (!isMenuOpen && menuState == "")
			{
				menuState = "opening";
			}
			else
			{
				menuState = "closing";
			}
		}

		public void NextClick()
		{
			currentSelectedItem++;
			if (currentSelectedItem >= totalNumberOfItems)
			{
				currentSelectedItem = 0;
			}
			UpdateItemName();
			UpdateItemRatio();
			SendBulletInformationToPlayer();
			burstDelay = true;
			for (int i = 0; i < SceneManager.listOfArms.Count; i++)
			{
				SceneManager.listOfArms[i].GetComponent<Player>().RemoveChargeObject();
			}
			NewCheck();
		}

		public void PrevClick()
		{
			currentSelectedItem--;
			if (currentSelectedItem < 0)
			{
				currentSelectedItem = totalNumberOfItems - 1;
			}
			UpdateItemName();
			UpdateItemRatio();
			SendBulletInformationToPlayer();
			burstDelay = true;
			for (int i = 0; i < SceneManager.listOfArms.Count; i++)
			{
				SceneManager.listOfArms[i].GetComponent<Player>().RemoveChargeObject();
			}
			NewCheck();
		}

		public void SelectBurst()
		{
			if (selectedItem != "burst")
			{
				selectedItem = "burst";
				currentSelectedItem = 0;
				totalNumberOfItems = base.transform.Find("LowerSection").transform.Find("Burst").transform.childCount;
				if (totalNumberOfItems > 0)
				{
					UpdateItemName();
				}
				UpdateItemRatio();
				AdjustSelectionArrow();
				HidePlayerAndTarget();
				SendBulletInformationToPlayer();
				burstDelay = true;
				NewCheck();
				CheckLeftClickText();
			}
		}

		public void SelectLoop()
		{
			if (selectedItem != "loop")
			{
				selectedItem = "loop";
				currentSelectedItem = 0;
				totalNumberOfItems = base.transform.Find("LowerSection").transform.Find("Loops").transform.childCount;
				if (totalNumberOfItems > 0)
				{
					UpdateItemName();
				}
				UpdateItemRatio();
				AdjustSelectionArrow();
				HidePlayerAndTarget();
				SendBulletInformationToPlayer();
				NewCheck();
				CheckLeftClickText();
				for (int i = 0; i < SceneManager.listOfArms.Count; i++)
				{
					SceneManager.listOfArms[i].GetComponent<Player>().BurstManualRemoval();
				}
			}
		}

		public void SelectProjectile()
		{
			if (selectedItem != "projectile")
			{
				selectedItem = "projectile";
				currentSelectedItem = 0;
				totalNumberOfItems = base.transform.Find("LowerSection").transform.Find("Projectiles").transform.childCount;
				if (totalNumberOfItems > 0)
				{
					UpdateItemName();
				}
				UpdateItemRatio();
				AdjustSelectionArrow();
				SendBulletInformationToPlayer();
				ShowPlayerAndTarget();
				NewCheck();
				CheckLeftClickText();
				for (int i = 0; i < SceneManager.listOfArms.Count; i++)
				{
					SceneManager.listOfArms[i].GetComponent<Player>().BurstManualRemoval();
				}
			}
		}

		public void SendBulletInformationToPlayer()
		{
			for (int i = 0; i < SceneManager.listOfArms.Count; i++)
			{
				if (selectedItem == "loop")
				{
					SceneManager.listOfArms[i].GetComponent<Player>().ReceiveLoopInformation(selectedItemObject);
				}
				if (selectedItem == "burst")
				{
					SceneManager.listOfArms[i].GetComponent<Player>().PlayBurst();
				}
			}
		}

		public void AdjustSelectionArrow()
		{
			GameObject gameObject = base.transform.Find("Main Menu Image").transform.Find("LowerSection").transform.Find("Selection_Arrow").gameObject;
			Vector3 localPosition = gameObject.GetComponent<RectTransform>().localPosition;
			if (selectedItem == "burst")
			{
				localPosition.y = 33f;
				gameObject.GetComponent<RectTransform>().localPosition = localPosition;
			}
			else if (selectedItem == "loop")
			{
				localPosition.y = -60f;
				gameObject.GetComponent<RectTransform>().localPosition = localPosition;
			}
			else if (selectedItem == "projectile")
			{
				localPosition.y = -150f;
				gameObject.GetComponent<RectTransform>().localPosition = localPosition;
			}
		}

		private void UpdateItemName()
		{
			Transform transform = null;
			if (selectedItem == "projectile")
			{
				transform = base.transform.Find("LowerSection").transform.Find("Projectiles");
			}
			else if (selectedItem == "burst")
			{
				transform = base.transform.Find("LowerSection").transform.Find("Burst");
			}
			else if (selectedItem == "loop")
			{
				transform = base.transform.Find("LowerSection").transform.Find("Loops");
			}
			base.transform.Find("Main Menu Image").transform.Find("UpperSection").transform.Find("Selection_text").GetComponent<TMP_Text>().text = transform.transform.GetChild(currentSelectedItem).name;
			itemNameObject.GetComponent<TMP_Text>().text = transform.transform.GetChild(currentSelectedItem).name;
			SetSelectedItem(transform.transform.GetChild(currentSelectedItem).gameObject);
			ResetItemNameText();
		}

		private void ResetItemNameText()
		{
			itemNameObject.GetComponent<RectTransform>().localPosition = new Vector3(620f, -585f, 0f);
			moveText = true;
		}

		private void SetSelectedItem(GameObject tempGameObject)
		{
			selectedItemObject = tempGameObject;
		}

		private void UpdateItemRatio()
		{
			int num = currentSelectedItem + 1;
			base.transform.Find("Main Menu Image").transform.Find("UpperSection").transform.Find("Ratio").GetComponent<TMP_Text>().text = num + " / " + totalNumberOfItems;
		}

		private void UpdateMenuState()
		{
			if (menuState == "opening")
			{
				Vector3 localPosition = base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition;
				localPosition.x += menuMoveSpeed * timeSinceLastFrame;
				base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition = localPosition;
				MoveCamera();
				if (localPosition.x >= -263f)
				{
					isMenuOpen = true;
					menuState = "";
					base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition = new Vector3(-263f, 241f, 0f);
					base.transform.Find("Main Menu Image").transform.Find("Arrow").GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				}
			}
			if (menuState == "closing")
			{
				Vector3 localPosition2 = base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition;
				localPosition2.x -= menuMoveSpeed * timeSinceLastFrame;
				base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition = localPosition2;
				MoveCamera();
				if (localPosition2.x <= -726f)
				{
					isMenuOpen = false;
					menuState = "";
					base.transform.Find("Main Menu Image").GetComponent<RectTransform>().localPosition = new Vector3(-726f, 241f, 0f);
					base.transform.Find("Main Menu Image").transform.Find("Arrow").GetComponent<RectTransform>().localScale = new Vector3(-1f, 1f, 1f);
				}
			}
		}

		private void ScrollWheelAndButtonCheck()
		{
			if (Input.mouseScrollDelta.y < 0f || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
			{
				NextClick();
			}
			if (Input.mouseScrollDelta.y > 0f || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S))
			{
				PrevClick();
			}
		}

		private void HidePlayerAndTarget()
		{
			for (int i = 0; i < SceneManager.listOfArms.Count; i++)
			{
				SceneManager.listOfArms[i].GetComponent<SpriteRenderer>().enabled = false;
				SceneManager.listOfArms[i].GetComponent<Player>().RemoveChargeObject();
			}
			for (int j = 0; j < SceneManager.listOfHeads.Count; j++)
			{
				SceneManager.listOfHeads[j].GetComponent<SpriteRenderer>().enabled = false;
			}
			for (int k = 0; k < SceneManager.listOfBodies.Count; k++)
			{
				SceneManager.listOfBodies[k].GetComponent<SpriteRenderer>().enabled = false;
			}
			for (int l = 0; l < SceneManager.listOfTargets.Count; l++)
			{
				SceneManager.listOfTargets[l].GetComponent<SpriteRenderer>().enabled = false;
			}
		}

		private void ShowPlayerAndTarget()
		{
			for (int i = 0; i < SceneManager.listOfArms.Count; i++)
			{
				SceneManager.listOfArms[i].GetComponent<SpriteRenderer>().enabled = true;
			}
			for (int j = 0; j < SceneManager.listOfHeads.Count; j++)
			{
				SceneManager.listOfHeads[j].GetComponent<SpriteRenderer>().enabled = true;
			}
			for (int k = 0; k < SceneManager.listOfBodies.Count; k++)
			{
				SceneManager.listOfBodies[k].GetComponent<SpriteRenderer>().enabled = true;
			}
			for (int l = 0; l < SceneManager.listOfTargets.Count; l++)
			{
				SceneManager.listOfTargets[l].GetComponent<SpriteRenderer>().enabled = true;
			}
		}

		private void MoveCamera()
		{
			if (menuState == "opening")
			{
				GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize += 0.05f * timeSinceLastFrame;
				GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.Translate(-0.05f * timeSinceLastFrame, 0f, 0f);
				if (GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize > cameraShrinkSize)
				{
					GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize = cameraShrinkSize;
				}
				if (GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.position.x < cameraShrinkPosition)
				{
					GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.position = new Vector3(cameraShrinkPosition, 0f, -10f);
				}
			}
			if (menuState == "closing")
			{
				GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize -= 0.05f * timeSinceLastFrame;
				GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.Translate(0.05f * timeSinceLastFrame, 0f, 0f);
				if (GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize < cameraOriginalSize)
				{
					GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize = cameraOriginalSize;
				}
				if (GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.position.x > cameraOriginalPosition)
				{
					GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform.position = new Vector3(cameraOriginalPosition, 0f, -10f);
				}
			}
		}

		private void UpdateItemNameTextPosition()
		{
			if (moveText)
			{
				itemNameObject.GetComponent<RectTransform>().Translate(0f, textPopupMoveSpeed * timeSinceLastFrame, 0f);
				if (itemNameObject.GetComponent<RectTransform>().localPosition.y >= -495f)
				{
					moveText = false;
					itemNameObject.GetComponent<RectTransform>().localPosition = new Vector3(itemNameObject.GetComponent<RectTransform>().localPosition.x, -495f, itemNameObject.GetComponent<RectTransform>().localPosition.z);
				}
			}
		}

		private void NewCheck()
		{
			if (selectedItemObject.GetComponent<NewStatus>() != null && selectedItemObject.GetComponent<NewStatus>().isNew)
			{
				base.transform.Find("Main Menu Image").transform.Find("NewImage").GetComponent<Image>().enabled = true;
			}
			else
			{
				base.transform.Find("Main Menu Image").transform.Find("NewImage").GetComponent<Image>().enabled = false;
			}
		}

		public void CheckLeftClickText()
		{
			if (selectedItem == "loop")
			{
				base.transform.Find("Main Menu Image").transform.Find("LeftClickText").GetComponent<TMP_Text>().enabled = false;
			}
			else
			{
				base.transform.Find("Main Menu Image").transform.Find("LeftClickText").GetComponent<TMP_Text>().enabled = true;
			}
		}

		public void OpenCartoonCoffeeWebsite()
		{
			Application.OpenURL("http://cartooncoffeegames.com/");
		}

		private void Update()
		{
			timeSinceLastFrame = Time.deltaTime / 0.001666f;
			burstDelay = false;
			UpdateMenuState();
			UpdateItemNameTextPosition();
			ScrollWheelAndButtonCheck();
		}
	}
}
