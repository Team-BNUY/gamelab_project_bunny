namespace AI.Core
{
    public class Node
    {
        private Node _parent;
        private int _cost;
        private StateSet _states;
        private Action _action;

        public Node(Node parent, int cost, StateSet allStates, Action action)
        {
            _parent = parent;
            _cost = cost;
            _states = new StateSet(allStates);
            _action = action;
        }

        public Node(Node parent, int cost, StateSet worldStates, StateSet beliefStates, Action action)
        {
            _parent = parent;
            _cost = cost;
            _states = worldStates.Union(beliefStates);
            _action = action;
        }
            
        // Properties
            
        public Node Parent => _parent;
        public int Cost => _cost;
        public StateSet States => _states;
        public Action Action => _action;
    }
}
