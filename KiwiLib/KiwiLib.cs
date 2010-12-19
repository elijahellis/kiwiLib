using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace KiwiLib
{
    public class TelesphoreoHelper
    {

        #region KiwiLib Functions

        #region Packagelist Retrieval

        private string[] GetPackages(string url)
        {
            //Variables and things
            var newRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            newRequest.Method = "GET";
            newRequest.UserAgent = "Telesphoreo APT-HTTP/1.0.592"; //Setup user-agent data to that of Cydia's default

            string decodedText; //Declare variable here, for 'using' statement

            //Create MemoryStream to copy the Package-list to
            using (System.IO.MemoryStream memDecomp = new System.IO.MemoryStream())
            {

                //Read all the bytes to the new MemoryStream (Thanks .NET 4, ;) )
                newRequest.GetResponse().GetResponseStream().CopyTo(memDecomp);

                //Decode the bytes into text
                decodedText = Encoding.UTF8.GetString(memDecomp.ToArray());
            }


            //Slit the text, usually with the '\n' character
            var splitText = decodedText.Split(new string[] { "\n" }, StringSplitOptions.None);
            var readableList = new List<string>();
            var buildItem = new StringBuilder();

            //Read the split-text into the StringBuilder
            foreach (string item in splitText)
            {
                if (item != "")
                {
                    buildItem.Append(item); //Append the actual value
                    buildItem.Append("|"); //Append the '|' character to the value as a dilimeter
                }
                else
                {
                    readableList.Add(buildItem.ToString());
                    buildItem.Remove(0, buildItem.Length); //Best way to get rid of the values while reusing the varible
                }
            }

            //Return the built-up list as a string[] type
            return readableList.ToArray();
        }

        private string[] GetPackagesGz(string url)
        {
            //Variables and things
            var newRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            newRequest.Method = "GET";
            newRequest.UserAgent = "Telesphoreo APT-HTTP/1.0.592"; //Setup user-agent data to that of Cydia's default

            GZipStream decompressFile; //Declared in the entire scope for easy access

            try
            {
                //Copy the bytes of the Package-list into the decompression stream
                decompressFile = new GZipStream(newRequest.GetResponse().GetResponseStream(), CompressionMode.Decompress);
            }
            catch (Exception ex)
            {
                //In the case of a wrong URL
                throw new Exception("The url returned a null-valued page, or the repository does not have a gzip compressed package listing.", ex);
            }
            //Create MemoryStream to copy the decompressed Package-list to
            var memDecomp = new System.IO.MemoryStream();

            //Read all the bytes to the new MemoryStream (Thanks .NET 4, ;) )
            decompressFile.CopyTo(memDecomp);

            //Decode the bytes into text
            string decodedText = Encoding.UTF8.GetString(memDecomp.ToArray());

            //Slit the text, usually with the '\n' character
            var splitText = decodedText.Split(new string[] { "\n" }, StringSplitOptions.None);
            var readableList = new List<string>();
            var buildItem = new StringBuilder();

            //Read the split-text into the StringBuilder
            foreach (string item in splitText)
            {
                if (item != "")
                {
                    buildItem.Append(item); //Append the actual value
                    buildItem.Append("|"); //Append the '|' character to the value as a dilimeter
                }
                else
                {
                    readableList.Add(buildItem.ToString());
                    buildItem.Remove(0, buildItem.Length); //Best way to get rid of the values while reusing the varible
                }
            }

            //Return the built-up list as a string[] type
            return readableList.ToArray();
        }

        private string[] GetPackagesBz2(string url)
        {
            //Variables and things
            var newRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            newRequest.Method = "GET";
            newRequest.UserAgent = "Telesphoreo APT-HTTP/1.0.592"; //Setup user-agent data to that of Cydia's default

            ICSharpCode.SharpZipLib.BZip2.BZip2InputStream decompressFile; //Declared in the entire scope for easy access

            try
            {
                //Copy the bytes of the Package-list into the decompression stream
                decompressFile = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(newRequest.GetResponse().GetResponseStream());
            }
            catch (Exception ex)
            {
                //In the case of a wrong URL
                throw new Exception("The url returned a null-valued page, or the repository does not have a bzip2 compressed package listing.", ex);
            }
            //Create MemoryStream to copy the decompressed Package-list to
            var memDecomp = new System.IO.MemoryStream();

            //Read all the bytes to the new MemoryStream (Thanks .NET 4, ;) )
            decompressFile.CopyTo(memDecomp);

            //Decode the bytes into text
            string decodedText = Encoding.UTF8.GetString(memDecomp.ToArray());

            //Slit the text, usually with the '\n' character
            var splitText = decodedText.Split(new string[] { "\n" }, StringSplitOptions.None);
            var readableList = new List<string>();
            var buildItem = new StringBuilder();

            //Read the split-text into the StringBuilder
            foreach (string item in splitText)
            {
                if (item != "")
                {
                    buildItem.Append(item); //Append the actual value
                    buildItem.Append("|"); //Append the '|' character to the value as a dilimeter
                }
                else
                {
                    readableList.Add(buildItem.ToString());
                    buildItem.Remove(0, buildItem.Length); //Best way to get rid of the values while reusing the varible
                }
            }

            //Return the built-up list as a string[] type
            return readableList.ToArray();
        }

        #endregion        

        #region Repo Information

        bool isGzip(string repoUrl)
        {
            WebClient newClient = new WebClient();
            newClient.Headers.Add("user-agent", "Telesphoreo APT-HTTP/1.0.592");
            try
            {
                newClient.DownloadData(new Uri(repoUrl + "/Packages.gz", UriKind.RelativeOrAbsolute));
                newClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }

        }

        bool isBzip2(string repoUrl)
        {
            WebClient newClient = new WebClient();
            newClient.Headers.Add("user-agent", "Telesphoreo APT-HTTP/1.0.592");
            try
            {
                newClient.DownloadData(new Uri(repoUrl + "/Packages.bz2", UriKind.RelativeOrAbsolute));
                newClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        bool isUncompressed(string repoUrl)
        {
            WebClient newClient = new WebClient();
            newClient.Headers.Add("user-agent", "Telesphoreo APT-HTTP/1.0.592");
            try
            {
                newClient.DownloadData(new Uri(repoUrl + "/Packages", UriKind.RelativeOrAbsolute));
                newClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        #endregion       

        public Package[] RetrievePackages(string repoUrl)
        {
            var packageList = new List<Package>();

            //Cycle through all the types of repos to determine the current one
            if (isBzip2(repoUrl)) //Check if repo is Bzip2'ed
            {
                foreach (string packageBlock in GetPackagesBz2(WebUtility.HtmlEncode(repoUrl + "/Packages.bz2")))
                {
                    if (string.IsNullOrEmpty(packageBlock) == false)
                    {
                        packageList.Add(new Package(packageBlock, repoUrl));
                    }
                }
            }
            else if (isGzip(repoUrl)) //Check if repo is Gzip'ed
            {
                foreach (string packageBlock in GetPackagesGz(WebUtility.HtmlEncode(repoUrl + "/Packages.gz")))
                {
                    if (string.IsNullOrEmpty(packageBlock) == false)
                    {
                        packageList.Add(new Package(packageBlock, repoUrl));
                    }
                }
            }
            else if (isUncompressed(repoUrl)) //Check if repo is Uncompressed
            {
                foreach (string packageBlock in GetPackages(WebUtility.HtmlEncode(repoUrl + "/Packages")))
                {
                    if (string.IsNullOrEmpty(packageBlock) == false)
                    {
                        packageList.Add(new Package(packageBlock, repoUrl));
                    }
                }
            }
                

            //Return the Array of PackageItem(s)
            return packageList.ToArray();
        }

        public Package RetrievePackage(string repoUrl, string packageName)
        {
            int currentPack = 0;
            var blockLines = RetrievePackages(repoUrl);
            int i;
            for (i = 0; i != blockLines.Length; i++)
            {
                if (blockLines[i].Name == packageName)
                {
                    currentPack = i;
                }
            }
            return blockLines[currentPack];
        }

    }




        #endregion

        #region KiwiLib[Package]-Only Properties

    public enum CategoryType
    {
        Networking, System, Other
    }

    public enum RepoType
    {
        Gzip, Bzip2,None
    }

    #endregion

}
