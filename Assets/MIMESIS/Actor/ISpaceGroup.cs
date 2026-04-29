using System;
using UnityEngine;

public interface ISpaceGroup : IDisposable
{
	VSpace? GetSpace(Vector3 realCoord);

	VSpace[] GetAroundSectors(int x, int y);

	VSpace[] GetAllAroundSectors(bool includeSelf = false);

	SPoint GetCenter();

	bool CheckEnter(int objectID, Vector3 realCoord);

	bool MoveObject(ISpaceActor sectorObject, Vector3 realCoord);

	bool AddObject(ISpaceActor sectorObject, Vector3 realCoord);

	bool RemoveObject(ISpaceActor sectorObject);

	void BeginBatchUpdate();

	void EndBatchUpdate();
}
