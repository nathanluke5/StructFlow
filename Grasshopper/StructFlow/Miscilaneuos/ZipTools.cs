using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace StructFlow.Misc
{
    public class ZipTools
    {
        public static bool ZipFolder(string folder, string zipfolder)
        {
            //https://johnlnelson.com/tag/c-create-zip-file/

            //Provide the folder to be zipped
            string folderToZip = @folder;

            //provide the path and name for the zip file to create
            string zipFile = "";
            if (string.IsNullOrEmpty(zipfolder))
                zipFile = @String.Format("{0}.zip", folder);
            else
            {
                string foldername = Path.GetFileName(folder);
                string str = Path.Combine(zipfolder, foldername);
                zipFile = "\"" + str + ".zip" + "\"";
            }

            string AddQuotesIfRequired(string path)
            {
                return !string.IsNullOrWhiteSpace(path) ?
                    path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                        "\"" + path + "\"" : path :
                        string.Empty;
            }

            if (System.IO.File.Exists(zipFile))
               System.IO.File.Delete(zipFile);

            //call the ZipFile method
            try
            {
                ZipFile.CreateFromDirectory(folderToZip,@zipFile);
                return true;
            }
            catch(System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }
    }
}
