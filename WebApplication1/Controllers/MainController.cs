using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MainController : Controller
    {
        private string Encryption(string password)
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
        public long Sendverification(string uniqid, string email, string firstname, string lastname, string code)
        {
            SmtpClient cle = new SmtpClient();
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress("Admin@noreply.com");
            mailMsg.To.Add(email);
            mailMsg.Subject = "Account Verification";
            mailMsg.IsBodyHtml = true;
            mailMsg.Body += "Thank you " + firstname + " " + lastname + " for using our application." + "<br />";
            mailMsg.Body += "This is your verification code <b>" + code + "</b><br/>";
            mailMsg.Body += "Keep in touch with us wherever you are. Thank you!";
            cle.Send(mailMsg);

            Session["IDUser"] = uniqid;
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }
        public ActionResult Intro()
        {
            return View();
        }
        public ActionResult Login()
        {
            #pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (Session["UserID"] != null && Session["isActive"] != "0")
            #pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            {
                return RedirectToRoute("Main");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Registration regs)
        {
            string encoded = Encryption(regs.Password);
            if (ModelState.IsValid)
            {
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    //get username dan password dari DB
                    var obj = db.Registration.Where(model => model.Username.Equals(regs.Username) && model.Password.Equals(encoded)).FirstOrDefault();
                    if (obj != null)
                    {
                        if (obj.VerifyActivationCode == obj.UserActivationCode)
                        {
                            /*UPDATE SESSION ISACTIVE JADI 1*/
                            var ObjUpdate = db.Registration.Find(obj.Id);
                            String isActive = ObjUpdate.IsActive;
                            ObjUpdate.IsActive = "1";
                            db.Entry(ObjUpdate).State = EntityState.Detached;
                            db.Entry(ObjUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                            /************************************/

                            Session["UserID"] = obj.Id;
                            Session["isActive"] = obj.IsActive;
                            Session["UserName"] = obj.Username;
                            return RedirectToRoute("Main");
                        }
                        else
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "You're not verified your account";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Color = "red";
                        ViewBag.Message = "Wrong Username or Password";
                        return View();
                    }
                }
            }
            return View(regs);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string submit, string firstname, string lastname, string gender, string username, string password, string confirmpassword, string email, string telepon)
        {
            if (!string.IsNullOrEmpty(submit))
            {
                Registration regs = new Registration();

                Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Boolean matchEmail = regexEmail.IsMatch(email);

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

                if (username == "" || password == "" || email == "" || telepon == "" || firstname == "" || lastname == "")
                {
                    ViewBag.Color = "red";
                    ViewBag.Message = "No Empty Fields Are Allowed";
                    return View();
                }
                else if (matchEmail == false)
                {
                    ViewBag.Color = "red";
                    ViewBag.Message = "Wrong Email Format";
                    return View();
                }
                else if (phoneMatch == false)
                {
                    ViewBag.Color = "red";
                    ViewBag.Message = "Wrong Phone Number Format";
                    return View();
                }
                else if (confirmpassword != password)
                {
                    ViewBag.Color = "red";
                    ViewBag.Message = "Password Isn't Match";
                    return View();
                }
                else
                {
                    string encoded = Encryption(password);
                    Guid guid = Guid.NewGuid();
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        // Check Email and Username Availability
                        int objUser = db.Registration.Where(model => model.Username.Equals(username)).Count();
                        int objEmail = db.Registration.Where(model => model.Email.Equals(email)).Count();
                        if (objUser > 0 || objEmail > 0)
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "Username or Email Is Already Registered";
                            return View();
                        }
                        else if (objUser > 0 && objEmail > 0)
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "Username or Email Is Already Registered";
                            return View();
                        }
                        else
                        {
                            string uniqid = guid.ToString();
                            string VerifyCode = ScrambleWord(uniqid.Replace("-", ""));
                            string code = VerifyCode.Substring(0, 10);

                            regs.Id = uniqid;
                            regs.Firstname = firstname;
                            regs.Lastname = lastname;
                            regs.Username = username;
                            regs.Gender = gender;
                            regs.Password = encoded;
                            regs.ConfirmPassword = encoded;
                            regs.Email = email;
                            regs.Telepon = telepon;
                            regs.ThemeLink = "~/Content/theme/default.jpg";
                            regs.Photo = "~/Content/img/default.jpg";
                            regs.IsActive = "0";
                            regs.VerifyActivationCode = code;
                            db.Registration.Add(regs);
                            db.SaveChanges();

                            Session["IDUser"] = uniqid;
                            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            TempData["code"] = code;
                            TempData.Keep("code");
                            return RedirectToRoute("Verify", new { id = milliseconds });
                        }
                    }
                }
            }
            else
            {
                return new EmptyResult();
            }
        }

        public string ScrambleWord(string word)
        {
            char[] chars = new char[word.Length];
            Random rand = new Random(10000);
            int index = 0;
            while (word.Length > 0)
            {
                int next = rand.Next(0, word.Length - 1); 
                                                    
                chars[index] = word[next];
                word = word.Substring(0, next) + word.Substring(next + 1);
                ++index;
            }
            return new String(chars);
        }

        public ActionResult VerifyRegister(string id)
        {
            string session = (string)System.Web.HttpContext.Current.Session["IDUser"];
            if (id == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else if(session == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                ViewBag.code = TempData.Peek("code").ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult VerifyRegister(Registration regs, string id)
        {
            ViewBag.code = TempData.Peek("code").ToString();
            string session = (string)System.Web.HttpContext.Current.Session["IDUser"];
            if (id == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else if (session == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        var obj = db.Registration.Where(model => model.Id.Equals(session)).FirstOrDefault();
                        if (obj != null)
                        {
                            if (obj.VerifyActivationCode == regs.UserActivationCode)
                            {
                                var ObjUpdate = db.Registration.Find(obj.Id);
                                ObjUpdate.UserActivationCode = obj.VerifyActivationCode;
                                db.Entry(ObjUpdate).State = EntityState.Detached;
                                db.Entry(ObjUpdate).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData.Remove("code");

                                long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                                Session["TOURING"] = "SIMPLE_TOUR_STARTED_IN" + milliseconds.ToString().Substring(1, 5) + "ERA";
                            }
                            else
                            {
                                ViewBag.Color = "red";
                                ViewBag.Message = "Wrong Verification Code";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "Your ID Is Not Found";
                            return View();
                        }
                    }
                }
                return RedirectToRoute("SimpleTour");
            }
        }

        public ActionResult CheckEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckEmail( Registration regs)
        {
            if (ModelState.IsValid)
            {
                using (ASPMVCDB db = new ASPMVCDB())
                {
                    var obj = db.Registration.Where(model => model.Username.Equals(regs.Username) && model.Email.Equals(regs.Email)).FirstOrDefault();
                    if (obj != null && obj.Email == regs.Email && obj.Username == regs.Username)
                    {
                        if (obj.UserActivationCode != null)
                        {
                            Session["ID"] = obj.Id;
                            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            return RedirectToRoute("ChangePassword", new { id = milliseconds });
                        }
                        else
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "You're not verified your account";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Color = "red";
                        ViewBag.Message = "Wrong Username or Email";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ChangePassword(string id)
        {
            string session = (string)System.Web.HttpContext.Current.Session["ID"];
            if (id == null )
            {
                return RedirectToAction("Login", "Main");
            }
            else if(session == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(Registration regs, string id)
        {
            string session = (string)System.Web.HttpContext.Current.Session["ID"];
            if (id == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else if (session == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else if (regs.ConfirmPassword != regs.Password)
            {
                long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                return RedirectToRoute("ChangePassword", new { id = milliseconds });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (ASPMVCDB db = new ASPMVCDB())
                    {
                        var obj = db.Registration.Where(model => model.Id.Equals(session)).FirstOrDefault();
                        if (obj != null)
                        {
                            string encoded = Encryption(regs.Password);
                            var ObjUpdate = db.Registration.Find(obj.Id);
                            ObjUpdate.Password = encoded;
                            ObjUpdate.ConfirmPassword = encoded;
                            db.Entry(ObjUpdate).State = EntityState.Detached;
                            db.Entry(ObjUpdate).State = EntityState.Modified;
                            db.SaveChanges();

                            Session.Clear();
                            Session.Abandon();
                        }
                        else
                        {
                            ViewBag.Color = "red";
                            ViewBag.Message = "Your ID Is Not Found";
                            return View();
                        }
                    }
                }
                return RedirectToAction("Login", "Main");
            }
        }
        public ActionResult BasicTutorial()
        {
            string session = (string)System.Web.HttpContext.Current.Session["TOURING"];
            if (session == null)
            {
                return RedirectToAction("Login", "Main");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Finish()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login", "Main");
        }
    }
}