using Player;
using UnityEngine;

namespace Interfaces
{
    public interface INetworkTriggerable
    {
        /// <summary>
        /// Method that runs when you trigger a triggerable object or event
        /// </summary>
        void TriggerableTrigger(NetworkStudentController currentStudentController);
        
        void TriggerableEnter();
        void TriggerableExit();
    }
}