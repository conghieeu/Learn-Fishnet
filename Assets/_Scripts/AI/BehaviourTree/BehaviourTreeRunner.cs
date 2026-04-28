using System.Collections.Generic;
using UnityEngine;

namespace MellowAbelson.AI.BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private float _tickInterval = 0.2f;

        private BtNode _rootNode;
        private float _timer;

        public BtNode RootNode
        {
            get => _rootNode;
            set => _rootNode = value;
        }

        private void Start()
        {
            if (_runOnStart && _rootNode != null)
                _rootNode.Reset();
        }

        private void Update()
        {
            if (_rootNode == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _tickInterval)
            {
                _timer = 0f;
                _rootNode.Execute();
            }
        }

        public void BuildTree(BtNode root)
        {
            _rootNode = root;
            _rootNode.Reset();
        }

        public void SetTickRate(float interval)
        {
            _tickInterval = Mathf.Max(0.01f, interval);
        }
    }
}
