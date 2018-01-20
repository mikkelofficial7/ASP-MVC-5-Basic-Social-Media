using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private string encryption(string password)
        {
            /**************************** ENCRYPTION PASSWORD ************************/
            string EncryptionKey = "PuTaNG1N4MoGa908oB0";
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }
            string encoded = password;
            return encoded;
            /**************************** ENCRYPTION PASSWORD ************************/
        }
        private string decryption(string password)
        {
            /*************** DECRYPTION PASSWORD **********************/
            string EncryptionKey = "PuTaNG1N4MoGa908oB0";
            byte[] cipherBytes = Convert.FromBase64String(password);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    password = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            string decode = password;
            return decode;
            /*************** DECRYPTION PASSWORD **********************/
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Main()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        List<Pesan> tweetContainer = db.Pesan.OrderByDescending(model => model.TanggalKirim).ToList();
                        if (tweetContainer.Count > 0)
                        {
                            tweetContainer.ForEach(m => m.Status = "Lama"); //update all record
                            db.SaveChanges();
                            return RedirectToRoute("Home");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                return RedirectToRoute("Home");
            }
        }
        public ActionResult RefreshTweet()
        {
            if (ModelState.IsValid)
            {
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    List<Pesan> tweetContainer = db.Pesan.OrderByDescending(model => model.TanggalKirim).ToList();
                    if (tweetContainer.Count > 0)
                    {
                        tweetContainer.ForEach(m => m.Status = "Lama"); //update all record
                        db.SaveChanges();
                        return RedirectToRoute("Home");
                    }
                    else
                    {
                        return RedirectToRoute("Home");
                    }
                }
            }
            return RedirectToRoute("Home");
        }
        public ActionResult Home()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    List<Pesan> tweetContainer = db.Pesan.OrderByDescending(model => model.TanggalKirim).ToList();
                    List<Like> listlikeother = db.Like.ToList();
                    List<Registration> friend = db.Registration.ToList();
                    var objTweet = db.Pesan.Count();

                    ViewBag.TweetContain = tweetContainer;
                    ViewBag.likeother = listlikeother;
                    ViewBag.IdTeman = friend;
                    ViewBag.tweet = objTweet.ToString();
                    return View();
                }
            }
        }
        [HttpPost]
        public ActionResult LoadAllTweet()
        {
            using (ASPMVCDB db = new ASPMVCDB())
            {
                string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                List<Pesan> tweetContainer = db.Pesan.Where(model => model.Status.Equals("Baru") && model.Id != Id).OrderByDescending(model => model.TanggalKirim).ToList();
                string jsontweet = JsonConvert.SerializeObject(tweetContainer);
                return Json(new { tweet = jsontweet }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Home(string isiPesan)
        {
            if (isiPesan != "")
            {
                string type = "/Sentiment/";
                string author = "/uClassify/";
                string APIKEY = "classify/?readKey=SPPGqs1fXsQy&text=";
                string baseAPI = "https://api.uclassify.com/v1";

                DateTime now = DateTime.Now;
                Pesan msg = new Pesan();
                Guid guid = Guid.NewGuid();
                string idPesan = "PS-" + guid.ToString();
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    string urlSentiment = baseAPI + author + type + APIKEY + isiPesan;
                    WebRequest requests = WebRequest.Create(urlSentiment);
                    WebResponse responses = requests.GetResponse();
                    Stream datas = responses.GetResponseStream();
                    StreamReader readers = new StreamReader(datas);
                    string JSON_SENTIMENT = readers.ReadToEnd();

                    if (JSON_SENTIMENT != "")
                    {
                        Sentiment feels = JsonConvert.DeserializeObject<Sentiment>(JSON_SENTIMENT);
                        double positive = Math.Round((Double.Parse(feels.positive.Replace('.', ',')) * 100));
                        double negative = Math.Round((Double.Parse(feels.negative.Replace('.', ',')) * 100));

                        msg.IdPesan = idPesan;
                        msg.IsiPesan = isiPesan;
                        msg.Id = Session["UserID"].ToString();
                        msg.TanggalKirim = now;
                        msg.Status = "Baru";
                        msg.Positive = positive.ToString();
                        msg.Negative = negative.ToString();
                    }
                    else
                    {
                        msg.IdPesan = idPesan;
                        msg.IsiPesan = isiPesan;
                        msg.Id = Session["UserID"].ToString();
                        msg.TanggalKirim = now;
                        msg.Status = "Baru";
                        msg.Positive = "Undefined";
                        msg.Negative = "Undefined";
                    }

                    db.Pesan.Add(msg);
                    db.SaveChanges();
                }
                return Content("sukses!!", "text/html");
            }
            else
            {
                return RedirectToRoute("Home");
            }
        }

        public ActionResult Profile()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        //get ID dari session_id
                        string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                        var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();
                        var objTweet = db.Pesan.Where(model => model.Id.Equals(Id)).Count();
                        var objFollowing = db.Friend.Where(model => model.IdAkun.Equals(Id)).Count();
                        var objFollower = db.Friend.Where(model => model.IdTeman.Equals(Id)).Count();
                        List<Pesan> tweetContainer = db.Pesan.Where(model => model.Id.Equals(Id)).OrderByDescending(model => model.TanggalKirim).ToList(); /*harus dijadiin object list*/
                        List<Like> listlike = db.Like.Where(model => !model.IdUser.Equals(Id)).ToList();

                        if (obj != null)
                        {
                            ViewBag.Nama = obj.Firstname + " " + obj.Lastname;
                            ViewBag.Photo = obj.Photo;
                            ViewBag.Username = obj.Username;
                            ViewBag.Tweet = objTweet.ToString();
                            ViewBag.TweetContain = tweetContainer;
                            ViewBag.Theme = obj.ThemeLink;
                            ViewBag.Following = objFollowing.ToString();
                            ViewBag.Follower = objFollower.ToString();
                            ViewBag.like = listlike;
                            return View();
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        /*JSON DATA AJAX*/
        [HttpPost]
        public ActionResult ProfileFollower()
        {
            string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var objFollower = db.Friend.Where(model => model.IdTeman.Equals(Id)).Count();
                string jsonfollower = JsonConvert.SerializeObject(objFollower);
                return Json(new { follower = jsonfollower }, JsonRequestBehavior.AllowGet);
            }
        }
        /*JSON DATA AJAX*/

        [HttpPost]
        public ActionResult Profiles()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        //get ID dari session_id
                        string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                        var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();

                        if (obj != null)
                        {
                            //************* UNLINK PHOTO****************
                            string filePath = Server.MapPath(obj.Photo);
                            FileInfo fileLink = new FileInfo(filePath);
                            if (fileLink.Exists && obj.Photo != "~/Content/img/default.jpg")
                            {
                                fileLink.Delete();
                            }

                            var ObjUpdate = db.Registration.Find(obj.Id);
                            ObjUpdate.Photo = "~/Content/img/default.jpg";
                            db.Entry(ObjUpdate).State = EntityState.Detached;
                            db.Entry(ObjUpdate).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToRoute("Profile");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult EditProfile(Registration regs)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                        var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();
                        if (obj != null)
                        {
                            string decode = decryption(obj.Password);

                            ViewBag.NamaDepan = obj.Firstname;
                            ViewBag.NamaBelakang = obj.Lastname;
                            ViewBag.Username = obj.Username;
                            ViewBag.Password = decode;
                            ViewBag.ConfirmPassword = decode;
                            ViewBag.Phone = obj.Telepon;
                            ViewBag.Photo = obj.Photo;
                            return View();
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        [HttpPost]
        public ActionResult EditProfile(string firstname, string lastname, string username, string password, string confirmpassword, string telepon, HttpPostedFileBase file)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            else if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                        var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();
                        int objUser = db.Registration.Where(model => model.Username.Equals(username)).Count();
                        if (obj != null)
                        {
                            if (password != confirmpassword)
                            {
                                string decodes = decryption(obj.Password);

                                ViewBag.NamaDepan = obj.Firstname;
                                ViewBag.NamaBelakang = obj.Lastname;
                                ViewBag.Username = obj.Username;
                                ViewBag.Password = decodes;
                                ViewBag.ConfirmPassword = decodes;
                                ViewBag.Phone = obj.Telepon;
                                ViewBag.Photo = obj.Photo;

                                ViewBag.Color = "red";
                                ViewBag.Message = "Password Isn't Match";
                            }
                            else
                            {
                                Boolean phoneMatch = true;
                                for (int i = 0; i < telepon.Length; i++)
                                {
                                    phoneMatch = Char.IsNumber(telepon, i);
                                    if (phoneMatch == true)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        phoneMatch = false;
                                        break;
                                    }
                                }

                                string encoded = encryption(password);

                                if (phoneMatch == true)
                                {
                                    if (objUser <= 0 || obj.Username == username)
                                    {
                                        if (file != null)
                                        {
                                            //********** UPLOAD PHOTO ********************
                                            string fileName = Path.GetFileName(file.FileName);
                                            string[] filenameSubstract = fileName.Split('.');
                                            fileName = fileName.Replace(filenameSubstract[0], (string)System.Web.HttpContext.Current.Session["UserID"]);

                                            string mainPath = Server.MapPath("~/");
                                            string specifiedPath = "/Content/img/";
                                            Directory.CreateDirectory(mainPath + specifiedPath);
                                            string path = "~" + specifiedPath + fileName;
                                            string rootedPath = mainPath + specifiedPath + fileName;
                                            file.SaveAs(rootedPath);

                                            Session["UserName"] = username;

                                            var ObjUpdate = db.Registration.Find(obj.Id);
                                            ObjUpdate.Firstname = firstname;
                                            ObjUpdate.Lastname = lastname;
                                            ObjUpdate.Username = username;
                                            ObjUpdate.Password = encoded;
                                            ObjUpdate.ConfirmPassword = encoded;
                                            ObjUpdate.Telepon = telepon;
                                            ObjUpdate.Photo = path;

                                            db.Entry(ObjUpdate).State = EntityState.Detached;
                                            db.Entry(ObjUpdate).State = EntityState.Modified;
                                            db.SaveChanges();

                                            string decode = decryption(encoded);

                                            ViewBag.NamaDepan = obj.Firstname;
                                            ViewBag.NamaBelakang = obj.Lastname;
                                            ViewBag.Username = obj.Username;
                                            ViewBag.Password = decode;
                                            ViewBag.ConfirmPassword = decode;
                                            ViewBag.Phone = obj.Telepon;
                                            ViewBag.Photo = obj.Photo;

                                            ViewBag.Color = "#4280f4";
                                            ViewBag.Message = "Successfully Edited";
                                            return View();
                                        }
                                        else
                                        {
                                            Session["UserName"] = username;

                                            var ObjUpdate = db.Registration.Find(obj.Id);
                                            ObjUpdate.Firstname = firstname;
                                            ObjUpdate.Lastname = lastname;
                                            ObjUpdate.Username = username;
                                            ObjUpdate.Password = encoded;
                                            ObjUpdate.ConfirmPassword = encoded;
                                            ObjUpdate.Telepon = telepon;
                                            ObjUpdate.Photo = obj.Photo;

                                            db.Entry(ObjUpdate).State = EntityState.Detached;
                                            db.Entry(ObjUpdate).State = EntityState.Modified;
                                            db.SaveChanges();

                                            string decode = decryption(encoded);
                                            
                                            ViewBag.NamaDepan = obj.Firstname;
                                            ViewBag.NamaBelakang = obj.Lastname;
                                            ViewBag.Username = obj.Username;
                                            ViewBag.Password = decode;
                                            ViewBag.ConfirmPassword = decode;
                                            ViewBag.Phone = obj.Telepon;
                                            ViewBag.Photo = obj.Photo;

                                            ViewBag.Color = "#4280f4";
                                            ViewBag.Message = "Successfully Edited";
                                            return View();
                                        }
                                    }
                                    else
                                    {
                                        string decode = decryption(encoded);

                                        ViewBag.NamaDepan = obj.Firstname;
                                        ViewBag.NamaBelakang = obj.Lastname;
                                        ViewBag.Username = obj.Username;
                                        ViewBag.Password = decode;
                                        ViewBag.ConfirmPassword = decode;
                                        ViewBag.Phone = obj.Telepon;
                                        ViewBag.Photo = obj.Photo;

                                        ViewBag.Color = "red";
                                        ViewBag.Message = "Username Is Already Used";
                                        return View();
                                    }
                                }
                                else
                                {
                                    string decode = decryption(encoded);

                                    ViewBag.NamaDepan = obj.Firstname;
                                    ViewBag.NamaBelakang = obj.Lastname;
                                    ViewBag.Username = obj.Username;
                                    ViewBag.Password = decode;
                                    ViewBag.ConfirmPassword = decode;
                                    ViewBag.Phone = obj.Telepon;
                                    ViewBag.Photo = obj.Photo;

                                    ViewBag.Color = "red";
                                    ViewBag.Message = "Wrong Phone Format";
                                }
                            }
                            return View();
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult Logout()
        {
            if (ModelState.IsValid)
            {
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    //get ID dari session_id
                    string Id = (string)System.Web.HttpContext.Current.Session["UserID"];

                    var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();
                    if (obj != null)
                    {
                        /*UPDATE SESSION ISACTIVE JADI 1*/
                        var ObjUpdate = db.Registration.Find(obj.Id);
                        String isActive = ObjUpdate.IsActive;
                        ObjUpdate.IsActive = "0";
                        db.Entry(ObjUpdate).State = EntityState.Detached;
                        db.Entry(ObjUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                        /************************************/
                        Session.Clear();
                        Session.Abandon();
                        return RedirectToAction("Login", "Main");
                    }
                    else
                    {
                        return RedirectToRoute("Home");
                    }
                }
            }
            else
            {
                return RedirectToRoute("Home");
            }
        }

        /*JSON DATA AJAX*/
        [HttpPost]
        public ActionResult Test(string id)
        {
            using (ASPMVCDB db = new ASPMVCDB())
            {
                List<Pesan> tweetContainer = db.Pesan.Where(model => model.Id.Equals(id)).OrderByDescending(model => model.TanggalKirim).ToList(); /*harus dijadiin object list*/
                var objFollowing = db.Friend.Where(model => model.IdAkun.Equals(id)).Count();
                var objFollower = db.Friend.Where(model => model.IdTeman.Equals(id)).Count();
                string json = JsonConvert.SerializeObject(tweetContainer);
                string jsonfollowing = JsonConvert.SerializeObject(objFollowing);
                string jsonfollower = JsonConvert.SerializeObject(objFollower);
                return Json(new { tweet = json, following = jsonfollowing, follower = jsonfollower }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult TestContain(string ids)
        //{
        //    string idSession = Session["UserID"].ToString();
        //    using (ASPMVCDB db = new ASPMVCDB())
        //    {
        //        var objTweet = db.Pesan.Where(model => model.Id.Equals(ids)).Count();
        //        List<Pesan> tweetContainer = db.Pesan.Where(model => model.Id.Equals(ids)).OrderByDescending(model => model.TanggalKirim).ToList(); /*harus dijadiin object list*/
        //        List<Like> listlike = db.Like.Where(model => model.IdUser.Equals(idSession)).ToList();
        //        List<Like> listlikeother = db.Like.Where(model => !model.IdUser.Equals(idSession)).ToList();
        //        string isi = "";
        //        if(tweetContainer.Count <= 0)
        //        {
        //            isi = "<div class='twpc-Tweet'><span class='Tweet-Content'>No Tweet Here</span></div>";
        //        }
        //        else
        //        {
        //            int i, j, k;
        //            int counter = 0;
        //            for (i = 0; i<tweetContainer.Count; i++)
        //            {
        //                isi += "<div class='twpc-Tweet'><span class='Tweet-Content'>" + tweetContainer[i].IsiPesan + "</span><br><span class='tweet-date'>Tweet from " + tweetContainer[i].TanggalKirim + "</span><a href='" + @Url.RouteUrl("UnlikeTweet", new { Id = tweetContainer[i].IdPesan, IdTeman = tweetContainer[i].Id }) + "' class='dislike' id='dislike'></a><a href = '" + @Url.RouteUrl("LikeTweet", new { Id = tweetContainer[i].IdPesan, IdTeman = tweetContainer[i].Id }) +"' class='likes' id='likes'></a>";
        //                for (j = 0; j < listlike.Count; j++)
        //                {
        //                    for (k = 0; k < listlikeother.Count; k++)
        //                    {
        //                        if (listlikeother[k].IdPesan == tweetContainer[i].IdPesan)
        //                        {
        //                            counter += 1;
        //                        }
        //                    }
        //                    if (listlike[j].IdPesan == tweetContainer[i].IdPesan && counter > 0)
        //                    {
        //                        isi += "<span class='like-status'>" + counter + " People and " + listlike[j].Status + "</span>";
        //                    }
        //                    else if (listlike[j].IdPesan == tweetContainer[i].IdPesan && counter <= 0)
        //                    {
        //                        isi += "<span class='like-status'>" + listlike[j].Status + "</span>";
        //                    }
        //                    counter = 0;
        //                }
        //                isi += "</div>";
        //            }
        //        }
        //        return Content(isi, "text/html");
        //    }
        //}
        /*JSON DATA AJAX*/

        public ActionResult FriendProfile(string id)
        {
            string idSession = Session["UserID"].ToString();
            if (id == idSession)
            {
                return RedirectToRoute("Profile");
            }
            else if (id == null)
            {
                return RedirectToRoute("Error");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        id = Request.Url.Segments[2];
                        var obj = db.Registration.Where(model => model.Id.Equals(id)).FirstOrDefault();
                        if (obj != null)
                        {
                            var objTweet = db.Pesan.Where(model => model.Id.Equals(id)).Count();
                            var objFollowing = db.Friend.Where(model => model.IdAkun.Equals(id)).Count();
                            var objFollower = db.Friend.Where(model => model.IdTeman.Equals(id)).Count();
                            List<Pesan> tweetContainer = db.Pesan.Where(model => model.Id.Equals(id)).OrderByDescending(model => model.TanggalKirim).ToList(); /*harus dijadiin object list*/
                            List<Like> listlikeother = db.Like.ToList();
                            var CountRelation = db.Friend.Where(model => model.IdTeman.Equals(id) && model.IdAkun.Equals(idSession)).Count();
                            var relation = db.Friend.Where(model => model.IdTeman.Equals(id) && model.IdAkun.Equals(idSession)).FirstOrDefault();
                            var numbrelation = db.Friend.Where(model => model.IdTeman.Equals(id) && model.IdAkun.Equals(idSession)).Count();

                            if (CountRelation > 0)
                            {
                                ViewBag.IdTeman = id;
                                ViewBag.Id = relation.IdPertemanan;
                                ViewBag.Nama = obj.Firstname + " " + obj.Lastname;
                                ViewBag.Photo = obj.Photo;
                                ViewBag.Username = obj.Username;
                                ViewBag.Tweet = objTweet.ToString();
                                ViewBag.TweetContain = tweetContainer;
                                ViewBag.Following = objFollowing.ToString();
                                ViewBag.Follower = objFollower.ToString();
                                ViewBag.likeother = listlikeother;
                                ViewBag.numbId = numbrelation;
                                ViewBag.Email = obj.Email;
                                ViewBag.Telp = obj.Telepon;
                                ViewBag.Theme = obj.ThemeLink;
                            }
                            else
                            {
                                ViewBag.IdTeman = id;
                                ViewBag.Id = id;
                                ViewBag.Nama = obj.Firstname + " " + obj.Lastname;
                                ViewBag.Photo = obj.Photo;
                                ViewBag.Username = obj.Username;
                                ViewBag.Tweet = objTweet.ToString();
                                ViewBag.TweetContain = tweetContainer;
                                ViewBag.Following = objFollowing.ToString();
                                ViewBag.Follower = objFollower.ToString();
                                ViewBag.likeother = listlikeother;
                                ViewBag.numbId = numbrelation;
                                ViewBag.Email = obj.Email;
                                ViewBag.Telp = obj.Telepon;
                                ViewBag.Theme = obj.ThemeLink;
                            }
                            return View();
                        }
                        else
                        {
                            return RedirectToRoute("Error");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }   
        }

        public ActionResult DeleteTweet(string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        var obj = db.Pesan.Where(model => model.IdPesan.Equals(Id)).FirstOrDefault();
                        List<Like> objlike = db.Like.Where(m => m.IdPesan.Equals(Id)).ToList();
                        if (obj != null)
                        {
                            Pesan idPesan = db.Pesan.Find(obj.IdPesan);
                            db.Pesan.Remove(idPesan);
                            //DELETE MORE THAN ONE RECORD
                            foreach(Like a in objlike)
                            {
                                db.Like.Remove(a);
                            }
                            db.SaveChanges();
                            return RedirectToRoute("Profile");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }
        public ActionResult DeleteTweets(string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        var obj = db.Pesan.Where(model => model.IdPesan.Equals(Id)).FirstOrDefault();
                        List<Like> objlike = db.Like.Where(m => m.IdPesan.Equals(Id)).ToList();
                        if (obj != null)
                        {
                            Pesan idPesan = db.Pesan.Find(obj.IdPesan);
                            db.Pesan.Remove(idPesan);
                            //DELETE MORE THAN ONE RECORD
                            foreach (Like a in objlike)
                            {
                                db.Like.Remove(a);
                            }
                            db.SaveChanges();
                            return RedirectToRoute("Main");
                        }
                        else
                        {
                            return RedirectToRoute("Main");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult RemoveFriend(string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        var obj = db.Friend.Where(model => model.IdPertemanan.Equals(Id)).FirstOrDefault();
                        if (obj != null)
                        {
                            string idTeman = obj.IdTeman;
                            Friend idPertemanan = db.Friend.Find(obj.IdPertemanan);
                            db.Friend.Remove(idPertemanan);
                            db.SaveChanges();
                            return RedirectToAction("FriendProfile", new { id = idTeman });
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult AddFriend(string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                Friend fr = new Friend();
                Guid guid = Guid.NewGuid();
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        fr.IdAkun = Session["UserID"].ToString();
                        fr.IdPertemanan = guid.ToString();
                        fr.IdTeman = Id;
                        db.Friend.Add(fr);
                        db.SaveChanges();
                        return RedirectToAction("FriendProfile", new { id = Id });
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult LikeTweet(string Id, string idTeman)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string idSession = Session["UserID"].ToString();
                        var obj = db.Like.Where(model => model.IdPesan.Equals(Id) && model.IdUser.Equals(idSession)).FirstOrDefault();
                        List<Pesan> tweetContainer = db.Pesan.OrderByDescending(model => model.TanggalKirim).ToList();
                        if (obj == null)
                        {
                            Like like = new Like();
                            Guid guid = Guid.NewGuid();
                            like.IdUser = Session["UserID"].ToString();
                            like.IdLike = "LK-" + guid.ToString();
                            like.IdPesan = Id;
                            db.Like.Add(like);
                            db.SaveChanges();

                            if (tweetContainer.Count > 0)
                            {
                                tweetContainer.ForEach(m => m.Status = "Lama"); //update all record
                                db.SaveChanges();
                                return RedirectToAction("FriendProfile", new { id = idTeman });
                            }
                            else
                            {
                                return RedirectToAction("FriendProfile", new { id = idTeman });
                            }
                            //return Content("<script language='javascript' type='text/javascript'>history.pushState({}, null, '/Users/" + idTeman + "');</script>");
                        }
                        else
                        {
                            return RedirectToAction("FriendProfile", new { id = idTeman });
                            //return Content("<script language='javascript' type='text/javascript'>history.pushState({}, null, '/Users/" + idTeman + "');</script>");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult UnlikeTweet(string Id, string idTeman)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string idSession = Session["UserID"].ToString();
                        var obj = db.Like.Where(model => model.IdPesan.Equals(Id) && model.IdUser.Equals(idSession)).FirstOrDefault();
                        List<Pesan> tweetContainer = db.Pesan.OrderByDescending(model => model.TanggalKirim).ToList();
                        if (obj != null)
                        {
                            Like idLike = db.Like.Find(obj.IdLike);
                            db.Like.Remove(idLike);
                            db.SaveChanges();

                            if (tweetContainer.Count > 0)
                            {
                                tweetContainer.ForEach(m => m.Status = "Lama"); //update all record
                                db.SaveChanges();
                                return RedirectToAction("FriendProfile", new { id = idTeman });
                            }
                            else
                            {
                                return RedirectToAction("FriendProfile", new { id = idTeman });
                            }
                        }
                        else
                        {
                            return RedirectToAction("FriendProfile", new { id = idTeman });
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult LikeTweetMain(string idTeman, string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string idSession = Session["UserID"].ToString();
                        var obj = db.Like.Where(model => model.IdPesan.Equals(Id) && model.IdUser.Equals(idSession)).FirstOrDefault();
                        if (obj == null)
                        {
                            Like like = new Like();
                            Guid guid = Guid.NewGuid();
                            like.IdUser = Session["UserID"].ToString();
                            like.IdLike = "LK-" + guid.ToString();
                            like.IdPesan = Id;
                            db.Like.Add(like);
                            db.SaveChanges();
                            return RedirectToRoute("Home");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult UnlikeTweetMain(string idTeman, string Id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        string idSession = Session["UserID"].ToString();
                        var obj = db.Like.Where(model => model.IdPesan.Equals(Id) && model.IdUser.Equals(idSession)).FirstOrDefault();
                        if (obj != null)
                        {
                            Like idLike = db.Like.Find(obj.IdLike);
                            db.Like.Remove(idLike);
                            db.SaveChanges();
                            return RedirectToRoute("Home");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }

        public ActionResult Following()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult FollowingPost()
        {
            string idSession = Session["UserID"].ToString();
            string isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var daftarFollowing = db.Friend.Where(m => m.IdAkun.Equals(idSession)).ToList();
                int i = 0;
                foreach(var a in daftarFollowing)
                {
                    var daftarFollowingDetail = db.Registration.Where(m => m.Id.Equals(a.IdTeman)).Select(m => new { m.Firstname, m.Lastname, m.Username, m.Photo }).FirstOrDefault();
                    isi += "<a href='"+ @Url.RouteUrl("FriendProfile", new { id = a.IdTeman }) + "'><div id='list-user'><img id='image-user' src='"+ @Url.Content(daftarFollowingDetail.Photo)+ "'/><div id='detail-user'>"+daftarFollowingDetail.Firstname+" "+daftarFollowingDetail.Lastname+" - @"+daftarFollowingDetail.Username+"</div></div></a>";
                    i++;
                }
                return Content(isi, "text/html");
            }
        }

        public ActionResult Follower()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult FollowerPost()
        {
            string idSession = Session["UserID"].ToString();
            string isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                List<Friend> daftarFollower = db.Friend.Where(m => m.IdTeman.Equals(idSession)).ToList();
                int i = 0;
                foreach (var a in daftarFollower)
                {
                    var daftarFollowingDetail = db.Registration.Where(m => m.Id.Equals(a.IdAkun)).Select(m => new { m.Firstname, m.Lastname, m.Username, m.Photo }).FirstOrDefault();
                    isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = a.IdAkun }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(daftarFollowingDetail.Photo) + "'/><div id='detail-user'>" + daftarFollowingDetail.Firstname + " " + daftarFollowingDetail.Lastname + " - @" + daftarFollowingDetail.Username + "</div></div></a>";
                    i++;
                }
                return Content(isi, "text/html");
            }
        }

        public ActionResult FriendFollowing(string id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                id = Request.Url.Segments[2];
                string idSession = Session["UserID"].ToString();
                if (id == idSession)
                {
                    return RedirectToRoute("Following");
                }
                ViewBag.id = id;
                return View();
            }
        }
        [HttpPost]
        public ActionResult FriendFollowingPost(string id)
        {
            string isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                List<Friend> daftarFollowing = db.Friend.Where(m => m.IdAkun.Equals(id)).ToList();
                int i = 0;
                foreach (var a in daftarFollowing)
                {
                    var daftarFollowingDetail = db.Registration.Where(m => m.Id.Equals(a.IdTeman)).Select(m => new { m.Firstname, m.Lastname, m.Username, m.Photo }).FirstOrDefault();
                    isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = a.IdTeman }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(daftarFollowingDetail.Photo) + "'/><div id='detail-user'>" + daftarFollowingDetail.Firstname + " " + daftarFollowingDetail.Lastname + " - @" + daftarFollowingDetail.Username + "</div></div></a>";
                    i++;
                }
                return Content(isi, "text/html");
            }
        }

        public ActionResult FriendFollower(string id)
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                id = Request.Url.Segments[2];
                string idSession = Session["UserID"].ToString();
                if (id == idSession)
                {
                    return RedirectToRoute("Follower");
                }
                ViewBag.id = id;
                return View();
            }
        }
        [HttpPost]
        public ActionResult FriendFollowerPost(string id)
        {
            string isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                List<Friend> daftarFollower = db.Friend.Where(m => m.IdTeman.Equals(id)).ToList();
                int i = 0;
                foreach (var a in daftarFollower)
                {
                    var daftarFollowingDetail = db.Registration.Where(m => m.Id.Equals(a.IdAkun)).Select(m => new { m.Firstname, m.Lastname, m.Username, m.Photo }).FirstOrDefault();
                    isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = a.IdAkun }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(daftarFollowingDetail.Photo) + "'/><div id='detail-user'>" + daftarFollowingDetail.Firstname + " " + daftarFollowingDetail.Lastname + " - @" + daftarFollowingDetail.Username + "</div></div></a>";
                    i++;
                }
                return Content(isi, "text/html");
            }
        }
        public ActionResult FriendList()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult FriendListPost()
        {
            string isi = "";
            string idSession = Session["UserID"].ToString();
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var daftarFollowingDetail = db.Registration.Where(m => !m.Id.Equals(idSession)).Select(m => new { m.Id, m.Firstname, m.Lastname, m.Username, m.Photo }).ToList();
                foreach (var a in daftarFollowingDetail)
                {
                    isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = a.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(a.Photo) + "'/><div id='detail-user'>" + a.Firstname + " " + a.Lastname + " - @" + a.Username + "</div></div></a>";
                }
                return Content(isi, "text/html");
            }
        }
        [HttpPost]
        public ActionResult Notif()
        {
            int counter = 0;
            string idSession = Session["UserID"].ToString();
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var alertpesan = db.Pesan.Where(m => m.Id.Equals(idSession)).Select(m => new { m.Negative, m.Positive }).ToList();
                foreach(var a in alertpesan)
                {
                    if(Convert.ToInt32(a.Negative) > Convert.ToInt32(a.Positive))
                    {
                        counter += 1;
                    }
                }
                return Content(counter.ToString(), "text/html");
            }
        }

        [HttpPost]
        public ActionResult SearchFriend(string uname)
        {
            var isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var person = db.Registration.Where(m => m.Username.Contains(uname)).ToList();
                foreach (var ppl in person)
                {
                    if (ppl.Id != Session["UserID"].ToString())
                    {
                        isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = ppl.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(ppl.Photo) + "'/><div id='detail-user'>" + ppl.Firstname + " " + ppl.Lastname + " - @" + ppl.Username + "</div></div></a>";
                    }
                }
                return Content(isi, "text/html");
            }
        }
        [HttpPost]
        public ActionResult SearchFriendFollowing(string uname)
        {
            var isi = "";
            string IdSess = (string)System.Web.HttpContext.Current.Session["UserID"];
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var idperson = db.Registration.Where(m => m.Username.Contains(uname)).Select(m => new { m.Id }).ToList(); //cari di tabel usernamenya
                foreach (var ppl in idperson)
                {
                    var Getid = db.Friend.Where(m => m.IdTeman.Contains(ppl.Id) && m.IdAkun.Equals(IdSess)).Select(m => new { m.IdTeman }).ToList(); //cari daftar teman bedasarkan idnya
                    foreach (var IdPpl in Getid)
                    {
                        var Getuname = db.Registration.Where(m => m.Id.Contains(IdPpl.IdTeman)).ToList();
                        foreach (var unameppl in Getuname)
                        {
                            isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = unameppl.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(unameppl.Photo) + "'/><div id='detail-user'>" + unameppl.Firstname + " " + unameppl.Lastname + " - @" + unameppl.Username + "</div></div></a>";
                        }
                    }
                }
                return Content(isi, "text/html");
            }
        }
        [HttpPost]
        public ActionResult SearchFriendFollower(string uname)
        {
            var isi = "";
            string IdSess = (string)System.Web.HttpContext.Current.Session["UserID"];
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var idperson = db.Registration.Where(m => m.Username.Contains(uname)).Select(m => new { m.Id }).ToList(); //cari di tabel usernamenya
                foreach (var ppl in idperson)
                {
                    var Getid = db.Friend.Where(m => m.IdAkun.Contains(ppl.Id) && m.IdTeman.Equals(IdSess)).Select(m => new { m.IdAkun }).ToList(); //cari daftar teman bedasarkan idnya
                    foreach (var IdPpl in Getid)
                    {
                        var Getuname = db.Registration.Where(m => m.Id.Contains(IdPpl.IdAkun)).ToList();
                        foreach (var unameppl in Getuname)
                        {
                            isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = unameppl.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(unameppl.Photo) + "'/><div id='detail-user'>" + unameppl.Firstname + " " + unameppl.Lastname + " - @" + unameppl.Username + "</div></div></a>";
                        }
                    }
                }
                return Content(isi, "text/html");
            }
        }
        [HttpPost]
        public ActionResult SearchFriendFollowingFR(string uname, string IdSess)
        {
            var isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var idperson = db.Registration.Where(m => m.Username.Contains(uname)).Select(m => new { m.Id }).ToList(); //cari di tabel usernamenya
                foreach (var ppl in idperson)
                {
                    var Getid = db.Friend.Where(m => m.IdTeman.Contains(ppl.Id) && m.IdAkun.Equals(IdSess)).Select(m => new { m.IdTeman }).ToList(); //cari daftar teman bedasarkan idnya
                    foreach (var IdPpl in Getid)
                    {
                        var Getuname = db.Registration.Where(m => m.Id.Contains(IdPpl.IdTeman)).ToList();
                        foreach (var unameppl in Getuname)
                        {
                            isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = unameppl.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(unameppl.Photo) + "'/><div id='detail-user'>" + unameppl.Firstname + " " + unameppl.Lastname + " - @" + unameppl.Username + "</div></div></a>";
                        }
                    }
                }
                return Content(isi, "text/html");
            }
        }
        [HttpPost]
        public ActionResult SearchFriendFollowerFR(string uname, string IdSess)
        {
            var isi = "";
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var idperson = db.Registration.Where(m => m.Username.Contains(uname)).Select(m => new { m.Id }).ToList(); //cari di tabel usernamenya
                foreach (var ppl in idperson)
                {
                    var Getid = db.Friend.Where(m => m.IdAkun.Contains(ppl.Id) && m.IdTeman.Equals(IdSess)).Select(m => new { m.IdAkun }).ToList(); //cari daftar teman bedasarkan idnya
                    foreach (var IdPpl in Getid)
                    {
                        var Getuname = db.Registration.Where(m => m.Id.Contains(IdPpl.IdAkun)).ToList();
                        foreach (var unameppl in Getuname)
                        {
                            isi += "<a href='" + @Url.RouteUrl("FriendProfile", new { id = unameppl.Id }) + "'><div id='list-user'><img id='image-user' src='" + @Url.Content(unameppl.Photo) + "'/><div id='detail-user'>" + unameppl.Firstname + " " + unameppl.Lastname + " - @" + unameppl.Username + "</div></div></a>";
                        }
                    }
                }
                return Content(isi, "text/html");
            }
        }

        [HttpPost]
        public ActionResult ThemeUpload(HttpPostedFileBase file)
        {
            string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
            using (ASPMVCDB db = new ASPMVCDB())
            {
                var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();
                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string[] filenameSubstract = fileName.Split('.');
                    fileName = fileName.Replace(filenameSubstract[0], (string)System.Web.HttpContext.Current.Session["UserID"]);

                    string mainPath = Server.MapPath("~/");
                    string specifiedPath1 = "/Content/";
                    string specifiedPath2 = "theme/";
                    Directory.CreateDirectory(mainPath + specifiedPath1 + specifiedPath2);
                    string path = "~" + specifiedPath1 + specifiedPath2 + fileName;
                    string rootedPath = mainPath + specifiedPath1 + specifiedPath2 + fileName;
                    file.SaveAs(rootedPath);

                    var ObjUpdate = db.Registration.Find(obj.Id);
                    ObjUpdate.ThemeLink = path;

                    db.Entry(ObjUpdate).State = EntityState.Detached;
                    db.Entry(ObjUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToRoute("Profile");
                }
            }
            return RedirectToRoute("Profile");
        }
        [HttpPost]
        public ActionResult ThemeRestore()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null && Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] == null || Session["isActive"] == "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        //get ID dari session_id
                        string Id = (string)System.Web.HttpContext.Current.Session["UserID"];
                        var obj = db.Registration.Where(model => model.Id.Equals(Id)).FirstOrDefault();

                        if (obj != null)
                        {
                            //************* UNLINK PHOTO****************
                            string filePath = Server.MapPath(obj.ThemeLink);
                            FileInfo fileLink = new FileInfo(filePath);
                            if (fileLink.Exists && obj.ThemeLink != "~/Content/theme/default.jpg")
                            {
                                fileLink.Delete();
                            }

                            var ObjUpdate = db.Registration.Find(obj.Id);
                            ObjUpdate.ThemeLink = "~/Content/theme/default.jpg";
                            db.Entry(ObjUpdate).State = EntityState.Detached;
                            db.Entry(ObjUpdate).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToRoute("Profile");
                        }
                        else
                        {
                            return RedirectToRoute("Home");
                        }
                    }
                }
                else
                {
                    return RedirectToRoute("Home");
                }
            }
        }
    }
}