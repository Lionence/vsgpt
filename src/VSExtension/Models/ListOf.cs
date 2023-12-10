using System.Collections.Generic;

namespace Lionence.VSGPT.Models
{
    internal class ListOf<T>
        where T : class
    {
        ICollection<T> Data { get; set; }
    }
}
