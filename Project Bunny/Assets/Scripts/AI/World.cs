namespace AI
{
    public sealed class World
    {
        private static readonly World instance = new World();
        private static StateSet states;
        
        /// <summary>
        /// Static constructor to initialize the World's StateSet
        /// </summary>
        static World()
        {
            states = new StateSet();
        }

        // Properties
        
        public static World Instance => instance;
        public StateSet States => states;
    }
}
