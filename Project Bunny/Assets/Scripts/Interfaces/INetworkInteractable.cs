using Networking;
using Player;

namespace Interfaces
{
    public interface INetworkInteractable
    {
        /// <summary>
        /// Method for doing something when you hit left click
        /// </summary>
        void Click();
        
        /// <summary>
        /// Method for doing something when you let go of the left click
        /// </summary>
        void Release();
        
        /// <summary>
        /// Method that runs when you quit control of an interactable
        /// </summary>
        void Exit();

        /// <summary>
        /// Method that runs when you take control of an interactable
        /// </summary>
        void Enter(NetworkStudentController currentStudentController);
        
        /// <summary>
        /// Method that runs when you enter the trigger volume of an Interactable
        /// </summary>
        void TriggerEnter();
        
        /// <summary>
        /// Method that runs when you exit the trigger volume of an Interactable
        /// </summary>
        void TriggerExit();
    }
}