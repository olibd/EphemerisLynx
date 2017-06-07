using System;
namespace Lynx.Core.Models
{
    public abstract class Model
    {
        /// <summary>
        /// Gets or sets the MUID (Model Unique ID).
        /// </summary>
        /// <value>The MUID.</value>
        private int MUID { get; set; }
    }
}
