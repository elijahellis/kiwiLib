using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace KiwiLib
{
    public class Package
    {
        string _HostURL;
        string _Name;
        int _Length;
        string _Location;
        string _Architechture;
        string _Category;

        #region Enumerating Functions

        public Package(string packageBlock, string HostURL)
        {
            //Split the block of text with the default dilimeting character ('|' is used)
            string[] PackagebyLines = packageBlock.Split(new string[] { "|" }, StringSplitOptions.None);

            //Add some values into the object
            foreach (string line in PackagebyLines)
            {
                if (line.StartsWith("Package:"))
                {
                    _Name = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Size:"))
                {
                    _Length = int.Parse(line.Substring(line.IndexOf(":") + 1));
                }
                if (line.StartsWith("Filename:"))
                {
                    _Location = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Architecture:"))
                {
                    _Architechture = line.Substring(line.IndexOf(":") + 1);
                }
                if (line.StartsWith("Section:"))
                {
                    _Category = line.Substring(line.IndexOf(":") + 1);
                    //System.Windows.Forms.MessageBox.Show(_Category); //Checking to see responsiveness
                }

                _HostURL = HostURL; //Add Host value, useful for determining package location
            }
        }

        #endregion       

        #region Package Properties

        public string Name
        {
            get
            {
                return _Name.Trim(); ;
            }
        }

        public string Host
        {
            get
            {
                return _HostURL.Replace("Packages.bz2", "").Replace("Packages.gz", "").Replace("Packages", "").Trim();
            }
        }

        public int Length
        {
            get
            {
                return _Length;
            }
        }

        public string Location
        {
            get
            {
                return _Location.Trim();
            }
        }

        public string Architechture
        {
            get
            {
                return _Architechture.Trim();
            }
        }

        public string Category
        {
            get
            {
                return _Category.Trim();
            }
        }

        #endregion

        #region Experimental
//Experimental, or 'beta' functions that may not be fully usable
 

        /// <summary>
        /// Retrieve the package's bytes as an array
        /// </summary>
        /// <returns>The package's corresponding byte-array</returns>
        public byte[] Download()
        {
            MemoryStream returnBytes = null;
            HttpWebRequest askDownload = (HttpWebRequest)HttpWebRequest.Create(WebUtility.HtmlEncode(Host  + Location));
            askDownload.Method = "GET";
            askDownload.UserAgent = "Telesphoreo APT-HTTP/1.0.592";

            var debianData = askDownload.GetResponse().GetResponseStream();
            long stopLen = askDownload.GetResponse().ContentLength;
            int i;
            for (i = 0; i != stopLen; i++)
            {
                returnBytes.WriteByte((byte)debianData.ReadByte());
            }

            return returnBytes.ToArray();
        }

        /// <summary>
        /// Download the package onto the local-harddisk
        /// </summary>
        /// <param name="Filename">Name to save the package as</param>
        public void Download(string Filename)
        {
            FileStream returnBytes = new FileStream(Filename, FileMode.Create);
            HttpWebRequest askDownload = (HttpWebRequest)HttpWebRequest.Create(WebUtility.HtmlEncode(Host + Location));
            askDownload.Method = "GET";
            askDownload.UserAgent = "Telesphoreo APT-HTTP/1.0.592";

            var debianData = askDownload.GetResponse().GetResponseStream();
            long stopLen = askDownload.GetResponse().ContentLength;
            int i;
            for (i = 0; i != stopLen; i++)
            {
                returnBytes.WriteByte((byte)debianData.ReadByte());
            }

            returnBytes.Close();
        }

        /// <summary>
        /// Simple way to build the package's path on the server
        /// </summary>
        /// <param name="thisLocation">Location property of the package</param>
        /// <param name="thisHost">Host property of the package</param>
        /// <returns>The package's built URL</returns>
        private string BuildPackageURL(string thisLocation, string thisHost)
        {
            string UrlEnd = string.Empty;
            var BuildURL = new StringBuilder();

            if (thisHost.EndsWith("Packages.gz"))
            {
                BuildURL.Append(thisHost.Replace("Packages.gz", ""));
            }
            else if (thisHost.EndsWith("Packages.bz2"))
            {
                BuildURL.Append(thisHost.Replace("Packages.bz2", ""));
            }

            if (thisLocation.EndsWith(".deb"))
            {
                BuildURL.Append(thisLocation.Replace(" ", string.Empty));
            }


            if (!string.IsNullOrEmpty(BuildURL.ToString()))
            {
                UrlEnd = BuildURL.ToString().Trim();
            }


            return UrlEnd;
        }

#endregion        


        
    }
}
