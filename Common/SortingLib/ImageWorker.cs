using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using SortingLib;
using SortingLib.DsOnlTableAdapters;

namespace ClimbingCompetition
{
    /// <summary>
    /// Класс для загрузки из/ сохранения в базу данных фотографий
    /// </summary>
    public static class ImageWorker
    {
        /// <summary>
        /// Преобразует входное изображение любого поддержимаего Windows формата в JPEG (набор байт)
        /// </summary>
        /// <param name="src">Исходное изображение типа System.Drawing.Image</param>
        /// <returns>набор байт JPEG кодировки</returns>
        public static byte[] GetBytesFromImage(Image src)
        {
            Bitmap bmp = new Bitmap(src);
            MemoryStream mstr = new MemoryStream();
            try
            {
                bmp.Save(mstr, ImageFormat.Jpeg);
                return mstr.ToArray();
            }
            finally { mstr.Close(); }
        }

        public static MemoryStream GetStreamFromImage(Image src)
        {
            Bitmap bmp = new Bitmap(src);
            MemoryStream mstr = new MemoryStream();
            bmp.Save(mstr, ImageFormat.Jpeg);
            return mstr;
        }

        /// <summary>
        /// Получает класс System.Drawing.Image из набора байт 
        /// </summary>
        /// <param name="src">исходный набор байт</param>
        /// <returns></returns>
        public static Image GetFromBytes(byte[] src)
        {
            MemoryStream mstr = new MemoryStream(src);
            try { return Image.FromStream(mstr); }
            finally { mstr.Close(); }
        }

        /// <summary>
        /// Имя папки на веб-сервере для хранения фоток
        /// </summary>
        public const string IMG_PATH = "photos";
        /// <summary>
        /// Префикс, показывающий, что эта фотка - фотка судьи
        /// </summary>
        public const string JUDGE_PREFIX = "J";

        /// <summary>
        /// Сохраняет хранящуюся в БД фото участника/судьи на сервер и возвращает относительный путь к этой фотке
        /// Если фотка уже есть на сервере, то возвращает пустую строку
        /// </summary>
        /// <param name="cn">Используемое соединение</param>
        /// <param name="iid">iid участника/судьи</param>
        /// <param name="judges">true, если нухна фотка судьи</param>
        /// <returns></returns>
        public static string GetOnlineImg(SqlConnection cn, int iid, bool judges)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                string table;
                if (judges)
                    table = "ONLjudges";
                else
                    table = "ONLclimbers";
                cmd.CommandText = "SELECT phLoaded FROM " + table + "(NOLOCK) WHERE iid=" + iid.ToString();
                object obj = cmd.ExecuteScalar();
                if (obj == null || obj == DBNull.Value)
                    return "";
                bool bRes = Convert.ToBoolean(obj);
                string dir = System.Threading.Thread.GetDomain().BaseDirectory;

                if (dir.Length > 0 && dir[dir.Length - 1] != '\\')
                    dir += "\\";

                if (!Directory.Exists(dir + IMG_PATH))
                    Directory.CreateDirectory(dir + IMG_PATH);
                string prfix;
                if (judges)
                    prfix = JUDGE_PREFIX;
                else
                    prfix = "";
                if (bRes && File.Exists(dir + IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg"))
                    return IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg";
                if (File.Exists(dir + IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg"))
                    try { File.Delete(dir + IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg"); }
                    catch { }
                ONLclimbersTableAdapter ota = new ONLclimbersTableAdapter();
                ota.Connection = cn;
                DsOnl.ONLclimbersDataTable dt = ota.GetDataByIid(iid);
                bool imageLoaded = false;
                try
                {
                    foreach (DsOnl.ONLclimbersRow row in dt.Rows)
                        if (row.IsphotoNull())
                        {
                            imageLoaded = true;
                            return "";
                        }
                        else
                        {
                            Image img = GetFromBytes(row.photo);
                            Bitmap bmp = new Bitmap(img);
                            bmp.Save(dir + IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg", ImageFormat.Jpeg);
                            imageLoaded = true;
                            return IMG_PATH + "\\" + prfix + iid.ToString() + ".jpg";
                        }
                }
                finally
                {
                    if (imageLoaded)
                    {
                        cmd.CommandText = "UPDATE " + table + " SET phLoaded = 1 WHERE iid=" + iid.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }
                return "";
            }

            catch { return ""; }

        }
    }
}
