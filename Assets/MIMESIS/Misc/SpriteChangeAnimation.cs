using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteChangeAnimation : MonoBehaviour
{
	[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private int framesPerSprite = 3;

	private int frameCnt;

	private int currSprIdx;

	private bool isPlaying;

	private Image image;

	public bool IsPlaying => isPlaying;

	public Image Image
	{
		get
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			return image;
		}
	}

	public bool IsEnd => IsEndSpriteIdx(currSprIdx);

	public bool CanPlay
	{
		get
		{
			if (!isPlaying)
			{
				return !IsEnd;
			}
			return false;
		}
	}

	public async UniTask Play(CancellationToken cancellationToken)
	{
		TurnOn();
		isPlaying = true;
		for (int i = 0; i < sprites.Length; i++)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			await UpdateSprite(i, cancellationToken);
		}
		isPlaying = false;
	}

	public void TurnOn()
	{
		Reset();
		Image.enabled = true;
	}

	public void TurnOff()
	{
		Reset();
		Image.enabled = false;
	}

	private async UniTask UpdateSprite(int spriteIdx, CancellationToken cancellationToken)
	{
		if (!IsEndSpriteIdx(spriteIdx))
		{
			ChangeSpriteImage(spriteIdx);
			frameCnt = 0;
			while (frameCnt < framesPerSprite && !cancellationToken.IsCancellationRequested)
			{
				await UniTask.NextFrame();
				frameCnt++;
			}
		}
	}

	private void ChangeSpriteImage(int spriteIdx)
	{
		currSprIdx = spriteIdx;
		Image.sprite = sprites[currSprIdx];
	}

	private bool IsEndSpriteIdx(int spriteIdx)
	{
		return spriteIdx >= sprites.Length;
	}

	private void Reset()
	{
		currSprIdx = 0;
		frameCnt = 0;
		Image.sprite = sprites[currSprIdx];
	}
}
