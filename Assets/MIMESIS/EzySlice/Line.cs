using UnityEngine;

namespace EzySlice
{
	public struct Line
	{
		private readonly Vector3 m_pos_a;

		private readonly Vector3 m_pos_b;

		public float dist => Vector3.Distance(m_pos_a, m_pos_b);

		public float distSq => (m_pos_a - m_pos_b).sqrMagnitude;

		public Vector3 positionA => m_pos_a;

		public Vector3 positionB => m_pos_b;

		public Line(Vector3 pta, Vector3 ptb)
		{
			m_pos_a = pta;
			m_pos_b = ptb;
		}
	}
}
