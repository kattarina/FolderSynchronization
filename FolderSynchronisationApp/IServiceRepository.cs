using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronisationApp
{
    public interface IServiceRepository
    {
        Task DoWork();
    }
}
