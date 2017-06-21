using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Blumind.Configuration;

namespace Blumind.Model
{
    class MyIconLibrary : PictureLibrary
    {
        public static readonly MyIconLibrary Share;
        public static readonly string BaseDirectory;
        public static readonly string BaseDirectory2;

        static MyIconLibrary()
        {
            BaseDirectory = Path.Combine(Application.StartupPath, "Icons");
            BaseDirectory2 = Path.Combine(ProgramEnvironment.ApplicationDataDirectory, "Icons");
            Share = new MyIconLibrary();
        }

        private MyIconLibrary()
        {
            Refresh();
        }

        public void Refresh()
        {
            Clear();

            Refresh(BaseDirectory);
            Refresh(BaseDirectory2);
        }

        private void Refresh(string dir)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp")
                    {
                        using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            try
                            {
                                Image image = Image.FromStream(stream);
                                string name = Path.GetFileName(file);
                                Add(name, new Picture(name, image));
                            }
                            catch (System.Exception ex)
                            {
                                Helper.WriteLog(ex);
                            }

                            stream.Close();
                        }
                    }
                }
            }
        }

        public string AddNew(string file, Image image)
        {
            if (image == null)
                return null;

            if (string.IsNullOrEmpty(file))
                file = System.Guid.NewGuid().ToString();
            else if (file.Length > 50)
                file = file.Substring(file.Length - 50, 50);

            string dir = BaseDirectory2;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            ImageFormat format;
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".gif":
                    format = ImageFormat.Gif;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
                default:
                    file += ".png";
                    ext = ".png";
                    format = ImageFormat.Png;
                    break;
            }

            string name = Path.GetFileNameWithoutExtension(file);
            string filename = Path.Combine(dir, file);
            int index = 0;
            while(File.Exists(filename))
            {
                index++;
                name = string.Format("{0}{1}", name, index);
                filename = Path.Combine(dir, name + ext);
            }

            using (FileStream stream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
            {
                image.Save(stream, ImageFormat.Png);
                stream.Close();
            }

            Add(file, new Picture(file, image));
            return name + ext;
        }

        internal bool AddFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return false;

            Image image = null;
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    image = Image.FromStream(stream);
                }
                finally
                {
                stream.Close();
                }
            }

            if(image == null)
                return false;
            
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            string tf = Path.Combine(BaseDirectory2, Path.GetFileName(filename));
            int index = 1;
            while (File.Exists(tf))
            {
                tf = Path.Combine(BaseDirectory2, name + index.ToString() + ext);
                index++;
            }

            try
            {
                File.Copy(filename, tf, true);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                image.Dispose();
                return false;
            }

            name = Path.GetFileName(tf);
            Add(name, new Picture(name, image));

            return true;
        }

        public bool Delete(Picture picture)
        {
            if (!Remove(picture.Name))
                return false;

            try
            {
                File.Delete(Path.Combine(BaseDirectory2, picture.Name));
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
            }

            return true;
        }
    }
}
