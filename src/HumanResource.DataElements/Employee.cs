using System;

namespace HumanResource.DataElements
{
    /// <summary>
    /// Represents Employee data transfer objects
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets/Sets first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets/Sets last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets/Sets date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets/Sets employee ID
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets/Sets email address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets/Sets gender
        /// </summary>
        public Gender Gender { get; set; }
    }
}
