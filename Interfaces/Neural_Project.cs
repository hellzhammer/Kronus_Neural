using System;

namespace Kronus_Neural.Interfaces
{
    internal interface INeural_Project
    {
        void Run();
        void init_project();
    }

    public abstract class Neural_Project : INeural_Project
    {
        /// <summary>
        /// //basic usage
        /// 
        /// public override void Run(){
        /// 
        /// //*your code here*
        /// 
        /// //assess fitness
        /// FitnessTest();
        /// 
        /// //run genetic algorithm
        /// Train();
        /// 
        /// //update genetic dictionaries
        /// UpdateGeneDictionaries();
        /// 
        /// //*your code here*
        /// 
        /// }
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Run()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// init the project: load in your data, prepare networks, etc.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void init_project()
        {
            throw new NotImplementedException();
        }
    }
}
