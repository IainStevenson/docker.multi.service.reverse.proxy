using System;

namespace Data.Model
{
    public interface IOwnedEntity 
    {

        /// <summary>
        ///     The  the owning entities Identifier
        /// </summary>
        Guid OwnerId { get; set; }
    }
}
