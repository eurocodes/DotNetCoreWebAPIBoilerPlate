using Core.Application.Interfaces.File;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.File {
    [RegisterAsSingleton]
    public class FileManager : IFileManager {
        public FileManager() {

        }
        public bool base64ToFile(string base64, string fileName, string path = "") {
            try {
                var uploads = Path.Combine("wwwroot", path);
                if (!Directory.Exists(uploads)) {
                    Directory.CreateDirectory(uploads);
                }
                string file = Path.Combine(uploads, fileName);
                string[] base64Strings = base64.Split(',');
                base64 = base64Strings[base64Strings.Length - 1];
                byte[] imageBytes = Convert.FromBase64String(base64);
                if (imageBytes.Length > 0) {
                    using (var stream = new FileStream(file, FileMode.Create)) {
                        stream.Write(imageBytes, 0, imageBytes.Length);
                        stream.Flush();
                        return true;
                    }
                }
            } catch {
                return false;
            }
            return false;
        }

        public bool fileExists(string fileName) {
            return System.IO.File.Exists(fileName);
        }

        public bool remove(string fileName) {
            System.IO.File.Delete(fileName);
            return true;
        }
    }
}
