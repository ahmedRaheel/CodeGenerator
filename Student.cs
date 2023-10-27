using System;
using System.Data;

namespace CodeGeneration.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Student
    {
        #region Constructor
        public Student ()
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// Id Property
        /// </summary> 
        public int Id { get; set; }
        /// <summary>
        /// Name Property
        /// </summary> 
        public string Name { get; set; }
        /// <summary>
        /// Email Property
        /// </summary> 
        public string Email { get; set; }
        /// <summary>
        /// Address Property
        /// </summary> 
        public string Address { get; set; }
        #endregion
    }
}
