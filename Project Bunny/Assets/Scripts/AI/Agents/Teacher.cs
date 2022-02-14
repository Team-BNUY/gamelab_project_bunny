namespace AI.Agents
{
    public class Teacher : Agent
    {
        protected override void Start()
        {
            var sentToTimeout = new State("sentStudentToTimeout", 1);
            var states = new StateSet(sentToTimeout);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);
            
            base.Start();
        }
    }
}
