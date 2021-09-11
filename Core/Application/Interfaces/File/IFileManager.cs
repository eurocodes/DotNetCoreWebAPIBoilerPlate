using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Interfaces.File {
    public interface IFileManager {
        bool remove(string fileName);
        bool fileExists(string fileName);
        bool base64ToFile(string base64, string fileName, string path = "");
    }
}
