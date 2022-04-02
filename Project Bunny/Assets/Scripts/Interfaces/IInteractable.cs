using System.Runtime.CompilerServices;
using Player;
using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
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
        void Enter(StudentController currentStudentController);
    }
}