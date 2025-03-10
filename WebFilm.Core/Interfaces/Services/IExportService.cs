using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IExportService
    {
        byte[] export(int exportType, int fileType, string className);
    }
}
