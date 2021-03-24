using System;

namespace Data.Model
{
    public interface IEntity
    {
        Guid Id { get; set; }

        /// <summary>
        ///     When in UTC time this entity was created
        /// </summary>        
        DateTimeOffset Created { get; set; }

        /// <summary>
        ///     When in UTC time this entity was last modified, null if never modified
        /// </summary>
        DateTimeOffset? Modified { get; set; }

    }
}
