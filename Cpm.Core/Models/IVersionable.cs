using System;

namespace Cpm.Core.Models
{
    public interface IVersionable
    {
        int Version { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
    }
}