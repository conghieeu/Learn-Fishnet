namespace UnityEngine.Rendering.Universal
{
	internal class URP_EnableDepthNormals : ScriptableRendererFeature
	{
		private class EnableDepthNormalsPass : ScriptableRenderPass
		{
			internal bool Setup(ScriptableRenderer renderer)
			{
				ConfigureInput(ScriptableRenderPassInput.Normal);
				return true;
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
			}
		}

		private EnableDepthNormalsPass m_SSAOPass;

		public override void Create()
		{
			if (m_SSAOPass == null)
			{
				m_SSAOPass = new EnableDepthNormalsPass();
			}
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (m_SSAOPass.Setup(renderer))
			{
				renderer.EnqueuePass(m_SSAOPass);
			}
		}
	}
}
