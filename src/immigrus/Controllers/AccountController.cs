using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using immigrus.Models;
using immigrus.Services;
using immigrus.ViewModels.Account;
using System.Globalization;
using System.IO;
using Microsoft.AspNet.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.PlatformAbstractions;

namespace immigrus.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private ApplicationDbContext _dbContext;
        IApplicationEnvironment _hostingEnvironment;

        public AccountController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IApplicationEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _dbContext = dbContext;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }
        }









        //
        // POST: /Account/CreerInscription
       
        public IActionResult CreerInscription()
        {
            

            // If we got this far, something failed, redisplay form
            return View();
        }

        public IActionResult AjouterEnfants(string idclient)
        {


            // If we got this far, something failed, redisplay form
            ViewBag.idclient = idclient;
            return View();
        }

        [HttpPost]
        public IActionResult ValiderAjouterEnfants(Enfants enfants, ICollection<IFormFile> files)
        {

           
                string idpart = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                Random rnd = new Random();
                int num = rnd.Next(0, 4);

                string insId = "EF" + idpart + num;

                var rep = UploadMultipleBool(files, insId);

                if (rep.Result)
                {



                   

                    string fileName = "";
                    string cheminPhoto = "";
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            cheminPhoto = Parametre.SMALLPATH + insId + Path.GetExtension(fileName);
                        }
                    }


                try
                {
                    DateTime dt = Convert.ToDateTime(enfants.DateNais).ToUniversalTime();
                    enfants.DateNais = dt;
                    enfants.Id = insId;
                    enfants.Photo = cheminPhoto;

                    _dbContext.Enfants.Add(enfants);
                    _dbContext.SaveChanges();



                }
                catch (Exception)
                {

                    throw;
                }
                    
                   

                   

                }
                else
                {
                    ViewBag.errorMessage = "Veuillez choisir une photo";
                }


          
            // If we got this far, something failed, redisplay form
            return View("AjouterEnfants");
        }



   

        public IActionResult ChargerPays()
        {

            var lstpays = _dbContext.Pays.ToList();

            return Json(lstpays);
        }


        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerInscription(ApplicationUserViewModel model, ICollection<IFormFile> files)
        {
            //CultureInfo francais = CultureInfo.GetCultureInfo("fr-FR");
            //CultureInfo anglais = CultureInfo.GetCultureInfo("en-US");s
            if (ModelState.IsValid)
    {

                string idpart = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                Random rnd = new Random();
                int num = rnd.Next(0, 4);

                string insId = "IM" + idpart + num;

                var rep = UploadMultipleBool(files,insId);

                if (rep.Result)
            {

                

                string clientId = Guid.NewGuid().ToString();

                    string fileName="";
                    string cheminPhoto="";
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                           fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            cheminPhoto = Parametre.SMALLPATH +insId+ Path.GetExtension(fileName);
                        }
                    }


                            DateTime dt = Convert.ToDateTime(model.DateNais);
                //try
                //{

                //    IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR" );

                //    DateTime dt11 = DateTime.Parse(model.DateNais, culture, System.Globalization.DateTimeStyles.AssumeLocal);

                //}
                //catch (Exception)
                //{

                //    throw;
                //}

                //creation user
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, ClientsId = clientId,
                    Nom = model.Nom, Prenoms = model.Prenoms, DateNais = dt, PaysNais = model.PaysNais,
                    PaysEl = model.PaysEl, LieuNais = model.LieuNais, Sexe = model.Sexe, PaysRes = model.PaysRes,
                    ZipCode = model.ZipCode, AdrPos = model.AdrPos, StatutMarital = model.StatutMarital,
                    NbEnfts = model.NbEnfts, Diplome = model.Diplome, AutresDip = model.AutresDip,
                    DateCreation = DateTime.UtcNow,Etat="ACTIF",Password=model.Password,
                    ParainIdf = model.ParainIdf, Tel1 = model.Tel1, Tel2 = model.Tel2, Photo = cheminPhoto,
                    PhoneNumber = model.Tel1, PhoneNumberConfirmed = true, EmailConfirmed = true };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    // _logger.LogInformation(3, "User created a new account with password.");









                    //enregistrement inscription
                    Inscription inscription = new Inscription();



                    inscription.Id = insId;

                    string dat = DateTime.UtcNow.ToString();

                    inscription.DateTrans = DateTime.Parse(dat);
                    inscription.Annee = DateTime.UtcNow.Year.ToString();
                    inscription.Etat = "ACTIF";
                    inscription.Statut = Parametre.VALIDER;
                    inscription.ClientId = clientId;

                    try
                    {
                        _dbContext.Inscription.Add(inscription);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {

                        throw;
                    }






                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                AddErrors(result);


            }
                else
                {
                    ViewBag.errorMessage = "Veuillez choisir une photo";
                }

     }

            // If we got this far, something failed, redisplay form
            return View(model);
        }




        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierInscription(ApplicationUserViewModel model,string Id, ICollection<IFormFile> files)
        {
            //CultureInfo francais = CultureInfo.GetCultureInfo("fr-FR");
            //CultureInfo anglais = CultureInfo.GetCultureInfo("en-US");s
            if (ModelState.IsValid)
            {

                

                var rep = UploadMultipleBool(files, Id);

                //if (rep.Result)
                //{



                    //string clientId = Guid.NewGuid().ToString();

                    string fileName = "";
                    string cheminPhoto = "";
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            cheminPhoto = Parametre.SMALLPATH + Id + Path.GetExtension(fileName);
                        }
                    }


                    DateTime dt = Convert.ToDateTime(model.DateNais);
                    //try
                    //{

                    //    IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR" );

                    //    DateTime dt11 = DateTime.Parse(model.DateNais, culture, System.Globalization.DateTimeStyles.AssumeLocal);

                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}

                    //creation user
                    ApplicationUser currentUser = _dbContext.ApplicationUser.Where(e=>e.ClientsId==model.ClientsId).FirstOrDefault();



                    currentUser.UserName = model.Email;
                    currentUser.Email = model.Email;
                // currentUser.ClientsId = Id;
                currentUser.Password = model.Password;
                    currentUser.Nom = model.Nom;
                    currentUser.Prenoms = model.Prenoms;
                    currentUser.DateNais = dt;
                    currentUser.PaysNais = model.PaysNais;
                    currentUser.PaysEl = model.PaysEl;
                    currentUser.LieuNais = model.LieuNais;
                    currentUser.Sexe = model.Sexe;
                    currentUser.PaysRes = model.PaysRes;
                    currentUser.ZipCode = model.ZipCode;
                    currentUser.AdrPos = model.AdrPos;
                    currentUser.StatutMarital = model.StatutMarital;
                    currentUser.NbEnfts = model.NbEnfts;
                    currentUser.Diplome = model.Diplome;
                    currentUser.AutresDip = model.AutresDip;
                    //currentUser.DateCreation = DateTime.UtcNow;
                    //currentUser.Etat = "ACTIF";
                    currentUser.ParainIdf = model.ParainIdf;
                    currentUser.Tel1 = model.Tel1;
                    currentUser.Tel2 = model.Tel2;
                if (string.IsNullOrEmpty(cheminPhoto))
                {
                    
                }
                else
                {
                    currentUser.Photo = cheminPhoto;
                }
                    
                    currentUser.PhoneNumber = model.Tel1;
                    //currentUser.PhoneNumberConfirmed = true;
                    //currentUser.EmailConfirmed = true;
                    

                    var result = await _userManager.UpdateAsync(currentUser);
                    if (result.Succeeded)
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                        // await _signInManager.SignInAsync(user, isPersistent: false);
                        // _logger.LogInformation(3, "User created a new account with password.");









                        ////enregistrement inscription
                        //Inscription inscription = new Inscription();

                        var ins = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Id == Id).FirstOrDefault();


                        //inscription.Id = Id;

                        //string dat = DateTime.UtcNow.ToString();

                        //inscription.DateTrans = DateTime.Parse(dat);
                        //inscription.Annee = DateTime.UtcNow.Year.ToString();
                        //inscription.Etat = "ACTIF";
                        ins.Statut = model.Statut;
                        //inscription.ClientId = clientId;

                        try
                        {
                           // _dbContext.Inscription.Add(ins);
                            _dbContext.SaveChanges();
                        }
                        catch (Exception e)
                        {

                            throw;
                        }






                        return RedirectToAction("ListeInscription","Account");
                    }
                    AddErrors(result);


                //}
                //else
                //{
                //    ViewBag.errorMessage = "Veuillez choisir une photo";
                //}

            }

            // If we got this far, something failed, redisplay form
            return View();
        }




        public IActionResult ListeInscription()
        {



            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).OrderByDescending(t=>t.DateTrans).Take(30).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom+" "+ recup.Prenoms+" ("+ recup.Email+")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
              

                lstCustom.Add(cstrans);
                
            }


            ViewBag.Annee = DateTime.UtcNow.Year;
            return View(lstCustom);
        }

        public IActionResult ListeDV()
        {

            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).OrderBy(t => t.DateTrans).Take(30).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " " + recup.Prenoms + " (" + recup.Email + ")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.ConfimationNumber = item.ConfimationNumber;
                cstrans.Resultat = item.Resultat;


                lstCustom.Add(cstrans);

            }



            return View(lstCustom);
        }

        public IActionResult ListeConfirmationNumber()
        {

            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER && !string.IsNullOrEmpty(i.Resultat) && !string.IsNullOrEmpty(i.ConfimationNumber)).OrderBy(t => t.DateTrans).Take(30).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " " + recup.Prenoms + " (" + recup.Email + ")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.ConfimationNumber = item.ConfimationNumber;
                cstrans.Resultat = item.Resultat;


                lstCustom.Add(cstrans);

            }



            return View(lstCustom);

        }


        //[HttpPost]
        //public ActionResult Save(IEnumerable<IFormFile> files)
        //{
        //    // The Name of the Upload component is "files"
        //    if (files != null)
        //    {
        //        foreach (var file in files)
        //        {

        //        if (file.Length > 0)
        //        {
        //            var fileNames = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            await file.SaveAsAsync(Path.Combine(uploads, fileNames));
        //        }

        //        // Some browsers send file names with full path.
        //        // We are only interested in the file name.
        //        var fileName = Path.GetFileName(file.FileName);
        //            var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

        //            // The files are not actually saved in this demo
        //            // file.SaveAs(physicalPath);
        //        }
        //    }

        //    // Return an empty string to signify success
        //    return Content("");
        //}

        //public ActionResult Remove(string[] fileNames)
        //{
        //    // The parameter of the Remove action must be called "fileNames"

        //    if (fileNames != null)
        //    {
        //        foreach (var fullName in fileNames)
        //        {
        //            var fileName = Path.GetFileName(fullName);
        //            var physicalPath = Path.Combine(Microsoft.AspNet.Server.MapPath("~/App_Data"), fileName);

        //            // TODO: Verify user permissions

        //            if (System.IO.File.Exists(physicalPath))
        //            {
        //                // The files are not actually removed in this demo
        //                // System.IO.File.Delete(physicalPath);
        //            }
        //        }
        //    }

        //    // Return an empty string to signify success
        //    return Content("");
        //}

        [HttpPost]
        public async Task<IActionResult> UploadMultiple(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine(_hostingEnvironment.ApplicationBasePath + Parametre.FULLPATH, "uploads");
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    if (!fileName.EndsWith(".txt"))// Important for security if saving in webroot


                    {
                        try
                        {
                            await file.SaveAsAsync(Path.Combine(uploads, fileName));

                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                }
        
    }
            
            return Content("");
        }

        public async Task<bool> UploadMultipleBool(ICollection<IFormFile> files,string id)
        {
            bool rep = false;
            try
            {

            
            var uploads = Path.Combine(_hostingEnvironment.ApplicationBasePath + Parametre.FULLPATH);
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    if (!fileName.EndsWith(".txt"))// Important for security if saving in webroot
                           

                    {
                        try
                        {
                                string[] end = fileName.Split('.');
                            //await file.SaveAsAsync(Path.Combine(uploads, fileName));
                            await file.SaveAsAsync(Path.Combine(uploads, id + Path.GetExtension(fileName)));
                            rep= true;
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                }

            }

            }
            catch (Exception)
            {

                rep=false;
            }

            return rep;
        }

        public IActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            var uploads = Path.Combine(_hostingEnvironment.ApplicationBasePath + "\\wwwroot\\img\\", "uploads");


            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(uploads, fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                         System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public IActionResult ModifierInscription(string id,string clientsId)
        {

            var inscription = _dbContext.Inscription.Where(i => i.Id == id && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).FirstOrDefault();
            var appuser = _dbContext.ApplicationUser.Where(a => a.ClientsId == clientsId && a.Etat == "ACTIF").FirstOrDefault();


            ViewBag.inscription = inscription;
            //ViewBag.appuser = appuser;
            return View(appuser);
        }



        public IActionResult ModifierClients(string id, CustomInscription item)
        {

            

           // var inscription = _dbContext.Inscription.Where(i => i.Id == id && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).FirstOrDefault();
           // var appuser = _dbContext.ApplicationUser.Where(a => a.ClientsId == clientsId && a.Etat == "ACTIF").FirstOrDefault();


           // ViewBag.inscription = inscription;
            ////ViewBag.appuser = appuser;
            return View(item);
        }

        [HttpPost]
        public IActionResult ValiderModifierClients(string ConfirmationNumber,string datenaisan,string Tel1,string Tel2,string Email,string Id,string ClientsId)
        {

            

             var inscription = _dbContext.Inscription.Where(i => i.Id == Id && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF").FirstOrDefault();
             var appuser = _dbContext.ApplicationUser.Where(a => a.ClientsId == ClientsId && a.Etat == "ACTIF").FirstOrDefault();


            inscription.ConfimationNumber = ConfirmationNumber;
            appuser.Email = Email;
            appuser.Tel1=Tel1;
            appuser.Tel2=Tel2;
            appuser.Tel1 = Tel1;
           
            appuser.DateNais = Convert.ToDateTime( appuser.DateNais.Day+"/"+appuser.DateNais.Month+"/"+ datenaisan);

            _dbContext.SaveChanges();

            return RedirectToAction("ListeClients");
        }


        // GET: ApplicationUsers/Details/5
        public IActionResult DetailsInscription(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser =  _dbContext.ApplicationUser.Where(m => m.ClientsId == id).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Details/5
        public IActionResult DetailsInscriptionDV(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == id).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            return View(applicationUser);
        }

        public IActionResult DetailsResultatEchec(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == id).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var inscription = _dbContext.Inscription.Where(i => i.ClientId == id && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut==Parametre.ECHEC).FirstOrDefault();


            ViewBag.cn = inscription.ConfimationNumber;

            return View(applicationUser);
        }


        public IActionResult DetailsResultatAdmis(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == id).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var inscription = _dbContext.Inscription.Where(i => i.ClientId == id && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut==Parametre.ADMIS).FirstOrDefault();


            ViewBag.cn = inscription.ConfimationNumber;

            return View(applicationUser);
        }

        public IActionResult AjouterCN(string idins,string idclient)
        {
            if (idins == null || idclient==null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == idclient).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.idinscription = idins;
            return View(applicationUser);
        }


        public IActionResult AfficherCN(string idins, string idclient)
        {
            if (idins == null || idclient == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == idclient).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var inscription = _dbContext.Inscription.Where(i => i.Id == idins && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF").FirstOrDefault();


            ViewBag.cn = inscription.ConfimationNumber;
            return View(applicationUser);
        }




        public IActionResult ListeClients()
        {
            
            return View();
        }

        public IActionResult ListeClientss()
        {
            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).OrderByDescending(t => t.DateTrans).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " , " + recup.Prenoms;
                cstrans.Email = recup.Email;
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.Tel2 = recup.Tel2;
                cstrans.datenaisan = recup.DateNais.Year.ToString();
                cstrans.ConfimationNumber = item.ConfimationNumber;


                lstCustom.Add(cstrans);

            }


            ViewBag.Annee = DateTime.UtcNow.Year;
            return Json(lstCustom);
        }

        public IActionResult ListeClientsArchives()
        {
            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.VALIDER).OrderByDescending(t => t.DateTrans).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " , " + recup.Prenoms;
                cstrans.Email = recup.Email;
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.Tel2 = recup.Tel2;
                cstrans.datenaisan = recup.DateNais.Year.ToString();
                cstrans.ConfimationNumber = item.ConfimationNumber;


                lstCustom.Add(cstrans);

            }


            ViewBag.Annee = DateTime.UtcNow.Year;
            return View(lstCustom);
        }



        public IActionResult ResultatInscriptionDV(string idins, string idclient)
        {
            if (idins == null || idclient == null)
            {
                return HttpNotFound();
            }

            ApplicationUser applicationUser = _dbContext.ApplicationUser.Where(m => m.ClientsId == idclient).FirstOrDefault();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.idinscription = idins;
            return View(applicationUser);
        }

        [HttpPost]
        public IActionResult ValiderCN(string idins,string cn)
        {
            if (idins == null)
            {
                return HttpNotFound();
            }

            var inscription = _dbContext.Inscription.Where(i => i.Id == idins && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF").FirstOrDefault();

            inscription.ConfimationNumber = cn;
            _dbContext.SaveChanges();

            return RedirectToAction("ListeDV");
        }


        [HttpPost]
        public IActionResult ValiderResultatInscriptionDV(string idins, string resultat,string CaseNumber)
        {
            if (idins == null)
            {
                return HttpNotFound();
            }

            var inscription = _dbContext.Inscription.Where(i => i.Id == idins && i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF").FirstOrDefault();

            inscription.Resultat = resultat;
            if (!string.IsNullOrEmpty(CaseNumber))
            {
            inscription.CaseNumber = CaseNumber;
            }
           
            _dbContext.SaveChanges();

            return RedirectToAction("ListeDV");
        }



        public IActionResult ListeClientBloque()
        {

            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.BLOQUE).OrderBy(t => t.DateTrans).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " " + recup.Prenoms + " (" + recup.Email + ")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;


                lstCustom.Add(cstrans);

            }

            ViewBag.Annee = DateTime.UtcNow.Year;
            return View(lstCustom);
        }


        public IActionResult DebloquerClient(string idins)
        {

            var inscription = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.BLOQUE).FirstOrDefault();

            inscription.Statut = Parametre.VALIDER;

            _dbContext.SaveChanges();

            return View("ListeClientBloque");
        }


        public IActionResult ListeResultatEchec()
        {

            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.ECHEC).OrderBy(t => t.DateTrans).Take(100).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " " + recup.Prenoms + " (" + recup.Email + ")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.ConfimationNumber = item.ConfimationNumber;
                cstrans.Resultat = item.Resultat;


                lstCustom.Add(cstrans);

            }



            return View(lstCustom);
        }

        public IActionResult ListeResultatAdmis()
        {

            List<CustomInscription> lstCustom = new List<CustomInscription>();


            // var lstTrans = _context.Transactions.Where(t=>t.idUtilisateur.Equals(id)).OrderByDescending(c => c.DateTransaction).ToList();
            var lstTrans = _dbContext.Inscription.Where(i => i.Annee == DateTime.UtcNow.Year.ToString() && i.Etat == "ACTIF" && i.Statut == Parametre.ADMIS).OrderBy(t => t.DateTrans).Take(100).ToList();

            foreach (var item in lstTrans)
            {
                CustomInscription cstrans = new CustomInscription();
                var recup = _dbContext.ApplicationUser.Where(a => a.ClientsId == item.ClientId).FirstOrDefault();

                cstrans.ClientsId = item.ClientId;
                cstrans.Email = recup.Email;
                cstrans.InscriptionId = item.Id;
                cstrans.Nom = recup.Nom + " " + recup.Prenoms + " (" + recup.Email + ")";
                cstrans.Prenoms = recup.Prenoms;
                cstrans.Photo = recup.Photo;
                cstrans.Tel1 = recup.Tel1;
                cstrans.ConfimationNumber = item.ConfimationNumber;
                cstrans.Resultat = item.Resultat;


                lstCustom.Add(cstrans);

            }



            return View(lstCustom);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
